using RanaMedia.Domain.Enums;

namespace RanaMedia.Contract.DTOs.Seo;

public class RelatedKeyphraseInputDTO
{
    public string Keyphrase { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = new();
}

public class AnalyzeSeoRequestDTO
{
    public Guid? ArticleId { get; set; }
    public string FocusKeyphrase { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = new();
    public List<RelatedKeyphraseInputDTO> RelatedKeyphrases { get; set; } = new();
    public string SeoTitle { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public string? FeaturedImageAlt { get; set; }
}

public class SeoCheckItemDTO
{
    public string RuleId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SeoCheckSeverity Severity { get; set; }
    public int ScoreImpact { get; set; }
    public bool Highlightable { get; set; }
    public List<string> MatchedTokens { get; set; } = new();
}

public class SeoHighlightMatchDTO
{
    public string RuleId { get; set; } = string.Empty;
    public int ParagraphIndex { get; set; }
    public int StartOffset { get; set; }
    public int Length { get; set; }
    public string MatchedText { get; set; } = string.Empty;
    public string ContextSnippet { get; set; } = string.Empty;
}

public class RelatedKeyphraseResultDTO
{
    public string Keyphrase { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = new();
    public int Score { get; set; }
    public SeoTrafficLight Status { get; set; }
    public List<SeoCheckItemDTO> Problems { get; set; } = new();
    public List<SeoCheckItemDTO> Improvements { get; set; } = new();
    public List<SeoCheckItemDTO> GoodResults { get; set; } = new();
}

public class SeoMetricsDTO
{
    public int WordCount { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public int ParagraphCount { get; set; }
    public int SubheadingCount { get; set; }
    public int SeoTitleCharLength { get; set; }
    public int SeoTitlePixelWidth { get; set; }
    public int MetaDescriptionLength { get; set; }
    public int KeyphraseOccurrences { get; set; }
    public double KeyphraseDensityPercent { get; set; }
    public int RecommendedMinOccurrences { get; set; }
    public int RecommendedMaxOccurrences { get; set; }
    public int InternalLinkCount { get; set; }
    public int OutboundLinkCount { get; set; }
    public int ImageCount { get; set; }
    public int ImagesWithKeyphraseAlt { get; set; }
    public int PreviousArticlesUsingKeyphraseCount { get; set; }
}

public class KeyphraseUsageItemDTO
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ArticleStatus Status { get; set; }
    public string FocusKeyphrase { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
}

public class SeoAnalysisResponseDTO
{
    public int OverallScore { get; set; }
    public SeoTrafficLight Status { get; set; }
    public int ProblemsCount => Problems.Count;
    public int ImprovementsCount => Improvements.Count;
    public int GoodResultsCount => GoodResults.Count;

    public List<SeoCheckItemDTO> Problems { get; set; } = new();
    public List<SeoCheckItemDTO> Improvements { get; set; } = new();
    public List<SeoCheckItemDTO> GoodResults { get; set; } = new();

    public List<RelatedKeyphraseResultDTO> RelatedKeyphraseResults { get; set; } = new();
    public List<SeoHighlightMatchDTO> Highlights { get; set; } = new();
    public SeoMetricsDTO Metrics { get; set; } = new();
    public List<KeyphraseUsageItemDTO> ConflictingArticles { get; set; } = new();
}

