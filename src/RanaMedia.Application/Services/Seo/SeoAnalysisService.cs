using System.Text.Json;
using Microsoft.Extensions.Options;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Options;
using RanaMedia.Application.Services.Seo.Rules;
using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Contract.Services.Seo;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.Seo;

public class SeoAnalysisService : ISeoAnalysisService
{
    private readonly IEnumerable<ISeoCheckRule> _rules;
    private readonly ISeoRepository _seoRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SeoAnalyzerSettings _settings;

    public SeoAnalysisService(
        IEnumerable<ISeoCheckRule> rules,
        ISeoRepository seoRepository,
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork,
        IOptions<SeoAnalyzerSettings> settings)
    {
        _rules = rules;
        _seoRepository = seoRepository;
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
    }

    public async Task<SeoAnalysisResponseDTO> AnalyzeLiveAsync(AnalyzeSeoRequestDTO request, CancellationToken ct = default)
    {
        if (request == null) throw new NotNullExceptions(nameof(request));

        var parsedDoc = HtmlContentParser.Parse(request.BodyHtml, _settings.SiteDomain);
        var conflicting = await GetKeyphraseUsageAsync(request.FocusKeyphrase, request.ArticleId, ct);

        var primaryResult = EvaluateKeyphrase(
            request,
            parsedDoc,
            conflicting,
            _rules);

        var response = new SeoAnalysisResponseDTO
        {
            OverallScore = primaryResult.Score,
            Status = primaryResult.Status,
            Problems = primaryResult.Problems,
            Improvements = primaryResult.Improvements,
            GoodResults = primaryResult.GoodResults,
            Highlights = primaryResult.Highlights,
            ConflictingArticles = conflicting,
            Metrics = BuildMetrics(request, parsedDoc, primaryResult.Occurrences, conflicting.Count)
        };

        // Score each "+ Add related keyphrase"
        foreach (var related in request.RelatedKeyphrases.Where(r => !string.IsNullOrWhiteSpace(r.Keyphrase)))
        {
            var relatedRequest = new AnalyzeSeoRequestDTO
            {
                ArticleId = request.ArticleId,
                FocusKeyphrase = related.Keyphrase.Trim(),
                Synonyms = related.Synonyms ?? new List<string>(),
                SeoTitle = request.SeoTitle,
                Slug = request.Slug,
                MetaDescription = request.MetaDescription,
                BodyHtml = request.BodyHtml,
                FeaturedImageUrl = request.FeaturedImageUrl,
                FeaturedImageAlt = request.FeaturedImageAlt
            };

            var relatedRules = _rules.Where(r => r.SupportsRelatedKeyphrase);
            var relEval = EvaluateKeyphrase(relatedRequest, parsedDoc, new List<KeyphraseUsageItemDTO>(), relatedRules);

            response.RelatedKeyphraseResults.Add(new RelatedKeyphraseResultDTO
            {
                Keyphrase = related.Keyphrase.Trim(),
                Synonyms = related.Synonyms ?? new List<string>(),
                Score = relEval.Score,
                Status = relEval.Status,
                Problems = relEval.Problems,
                Improvements = relEval.Improvements,
                GoodResults = relEval.GoodResults
            });
        }

        return response;
    }

    public async Task<SeoAnalysisResponseDTO> AnalyzeAndPersistForArticleAsync(Guid articleId, AnalyzeSeoRequestDTO request, CancellationToken ct = default)
    {
        request.ArticleId = articleId;
        var result = await AnalyzeLiveAsync(request, ct);

        var existing = await _seoRepository.GetByArticleIdAsync(articleId, ct);
        if (existing == null)
        {
            existing = new ArticleSeoProfile
            {
                ArticleId = articleId
            };
            await _seoRepository.AddAsync(existing);
        }

        existing.FocusKeyphrase = (request.FocusKeyphrase ?? string.Empty).Trim();
        existing.SynonymsJson = JsonSerializer.Serialize(request.Synonyms ?? new List<string>());
        existing.SeoTitle = request.SeoTitle;
        existing.MetaDescription = request.MetaDescription;
        existing.OverallScore = result.OverallScore;
        existing.OverallStatus = result.Status;
        existing.ProblemsCount = result.ProblemsCount;
        existing.ImprovementsCount = result.ImprovementsCount;
        existing.GoodResultsCount = result.GoodResultsCount;
        existing.LastAnalysisJson = JsonSerializer.Serialize(result);
        existing.LastAnalyzedAt = DateTime.UtcNow;

        // Replace related keyphrases
        foreach (var oldRel in existing.RelatedKeyphrases.ToList())
        {
            oldRel.IsDeleted = true;
        }
        foreach (var rel in result.RelatedKeyphraseResults)
        {
            existing.RelatedKeyphrases.Add(new RelatedKeyphrase
            {
                Keyphrase = rel.Keyphrase,
                SynonymsJson = JsonSerializer.Serialize(rel.Synonyms),
                Score = rel.Score,
                Status = rel.Status,
                ResultsJson = JsonSerializer.Serialize(rel)
            });
        }

        existing.Snapshots.Add(new SeoAnalysisSnapshot
        {
            FocusKeyphrase = existing.FocusKeyphrase,
            OverallScore = result.OverallScore,
            OverallStatus = result.Status,
            ProblemsCount = result.ProblemsCount,
            ImprovementsCount = result.ImprovementsCount,
            GoodResultsCount = result.GoodResultsCount,
            ResultsJson = JsonSerializer.Serialize(new
            {
                result.Problems,
                result.Improvements,
                result.GoodResults
            })
        });

        await _unitOfWork.SaveChangesAsync(ct);
        return result;
    }

    public async Task<SeoAnalysisResponseDTO?> GetArticleSeoProfileAsync(Guid articleId, CancellationToken ct = default)
    {
        var profile = await _seoRepository.GetByArticleIdAsync(articleId, ct);
        if (profile == null) return null;

        try
        {
            return JsonSerializer.Deserialize<SeoAnalysisResponseDTO>(profile.LastAnalysisJson);
        }
        catch
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) return null;

            return await AnalyzeLiveAsync(new AnalyzeSeoRequestDTO
            {
                ArticleId = articleId,
                FocusKeyphrase = profile.FocusKeyphrase,
                SeoTitle = profile.SeoTitle ?? article.MetaTitle ?? article.Title,
                Slug = article.Slug,
                MetaDescription = profile.MetaDescription ?? article.MetaDescription,
                BodyHtml = article.BodyHtml,
                FeaturedImageUrl = article.FeaturedImageUrl,
                FeaturedImageAlt = article.FeaturedImageAlt
            }, ct);
        }
    }

    public async Task<List<KeyphraseUsageItemDTO>> GetKeyphraseUsageAsync(string keyphrase, Guid? excludeArticleId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyphrase)) return new List<KeyphraseUsageItemDTO>();

        var profiles = await _seoRepository.GetArticlesUsingKeyphraseAsync(keyphrase, excludeArticleId, ct);
        return profiles
            .Where(p => p.Article != null)
            .Select(p => new KeyphraseUsageItemDTO
            {
                ArticleId = p.ArticleId,
                Title = p.Article.Title,
                Slug = p.Article.Slug,
                Status = p.Article.Status,
                FocusKeyphrase = p.FocusKeyphrase,
                PublishedAt = p.Article.PublishedAt
            })
            .ToList();
    }

    private (int Score, SeoTrafficLight Status, List<SeoCheckItemDTO> Problems, List<SeoCheckItemDTO> Improvements, List<SeoCheckItemDTO> GoodResults, List<SeoHighlightMatchDTO> Highlights, int Occurrences)
        EvaluateKeyphrase(
            AnalyzeSeoRequestDTO request,
            ParsedHtmlDocument parsedDoc,
            List<KeyphraseUsageItemDTO> conflicting,
            IEnumerable<ISeoCheckRule> rules)
    {
        var allPhrases = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.FocusKeyphrase))
            allPhrases.Add(request.FocusKeyphrase.Trim());

        if (request.Synonyms != null)
            allPhrases.AddRange(request.Synonyms.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()));

        var context = new SeoCheckContext
        {
            Request = request,
            Document = parsedDoc,
            Settings = _settings,
            ConflictingArticles = conflicting,
            AllKeyphrases = allPhrases.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            FocusContentWords = SeoTextMatcher.GetContentWords(request.FocusKeyphrase)
        };

        var problems = new List<SeoCheckItemDTO>();
        var improvements = new List<SeoCheckItemDTO>();
        var goodResults = new List<SeoCheckItemDTO>();
        var highlights = new List<SeoHighlightMatchDTO>();
        int totalEarned = 0;
        int totalPossible = 0;

        foreach (var rule in rules)
        {
            var eval = rule.Evaluate(context);
            totalEarned += eval.Check.ScoreImpact;
            totalPossible += GetRuleMaxScore(rule.RuleId);

            switch (eval.Check.Severity)
            {
                case SeoCheckSeverity.Problem:
                    problems.Add(eval.Check);
                    break;
                case SeoCheckSeverity.Improvement:
                    improvements.Add(eval.Check);
                    break;
                default:
                    goodResults.Add(eval.Check);
                    break;
            }

            if (eval.Highlights.Count > 0)
                highlights.AddRange(eval.Highlights);
        }

        int score = totalPossible > 0
            ? (int)Math.Clamp(Math.Round((double)totalEarned / totalPossible * 100), 0, 100)
            : 0;

        var status = problems.Count >= 3 || score < 50
            ? SeoTrafficLight.Red
            : (problems.Count > 0 || score < 75 ? SeoTrafficLight.Orange : SeoTrafficLight.Green);

        var occurrences = SeoTextMatcher.FindPhraseMatches(parsedDoc.PlainText, context.AllKeyphrases).Count;
        return (score, status, problems, improvements, goodResults, highlights, occurrences);
    }

    private SeoMetricsDTO BuildMetrics(AnalyzeSeoRequestDTO request, ParsedHtmlDocument doc, int occurrences, int conflictingCount)
    {
        var wordCount = doc.Words.Count;
        var (minRec, maxRec) = KeyphraseDensityRule.GetRecommendedBounds(wordCount, _settings);
        var allPhrases = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.FocusKeyphrase)) allPhrases.Add(request.FocusKeyphrase.Trim());
        if (request.Synonyms != null) allPhrases.AddRange(request.Synonyms.Where(s => !string.IsNullOrWhiteSpace(s)));

        var images = new List<ParsedImage>(doc.Images);
        if (!string.IsNullOrWhiteSpace(request.FeaturedImageUrl))
            images.Add(new ParsedImage(request.FeaturedImageUrl, request.FeaturedImageAlt ?? string.Empty));

        return new SeoMetricsDTO
        {
            WordCount = wordCount,
            ReadingTimeMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 200.0)),
            ParagraphCount = doc.Paragraphs.Count,
            SubheadingCount = doc.Subheadings.Count,
            SeoTitleCharLength = (request.SeoTitle ?? string.Empty).Trim().Length,
            SeoTitlePixelWidth = SeoTextMatcher.EstimateTitlePixelWidth((request.SeoTitle ?? string.Empty).Trim()),
            MetaDescriptionLength = (request.MetaDescription ?? string.Empty).Trim().Length,
            KeyphraseOccurrences = occurrences,
            KeyphraseDensityPercent = wordCount > 0 ? Math.Round((double)occurrences / wordCount * 100, 2) : 0,
            RecommendedMinOccurrences = minRec,
            RecommendedMaxOccurrences = maxRec,
            InternalLinkCount = doc.Links.Count(l => l.IsInternal),
            OutboundLinkCount = doc.Links.Count(l => l.IsOutbound),
            ImageCount = images.Count,
            ImagesWithKeyphraseAlt = images.Count(i => !string.IsNullOrWhiteSpace(i.Alt) && SeoTextMatcher.ContainsAnyPhrase(i.Alt, allPhrases)),
            PreviousArticlesUsingKeyphraseCount = conflictingCount
        };
    }

    private static int GetRuleMaxScore(string ruleId) => ruleId switch
    {
        "keyphrase_in_seo_title" => 9,
        "keyphrase_density" => 9,
        "keyphrase_length" => 8,
        "keyphrase_in_introduction" => 8,
        "text_length" => 8,
        "previously_used_keyphrase" => 8,
        "seo_title_width" => 7,
        "keyphrase_in_slug" => 7,
        "meta_description_length" => 7,
        "keyphrase_distribution" => 7,
        "keyphrase_in_subheading" => 7,
        "images_and_alt" => 7,
        "keyphrase_in_meta_description" => 6,
        "outbound_links" => 6,
        "internal_links" => 6,
        _ => 7
    };
}

