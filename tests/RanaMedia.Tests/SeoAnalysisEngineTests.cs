using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RanaMedia.Application.Options;
using RanaMedia.Application.Services.Seo;
using RanaMedia.Application.Services.Seo.Rules;
using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Persistence.Data;
using RanaMedia.Persistence.Repositories.Editorial;
using RanaMedia.Persistence.Repositories.System;
using Xunit;

namespace RanaMedia.Tests;

public class SeoAnalysisEngineTests
{
    private static (SeoAnalysisService Service, AppDbContext Db) CreateService()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"seo_test_{Guid.NewGuid()}")
            .Options;
        var db = new AppDbContext(opts);

        var rules = new ISeoCheckRule[]
        {
            new KeyphraseLengthRule(),
            new KeyphraseInSeoTitleRule(),
            new SeoTitleWidthRule(),
            new KeyphraseInSlugRule(),
            new MetaDescriptionLengthRule(),
            new KeyphraseInMetaDescriptionRule(),
            new KeyphraseInIntroductionRule(),
            new KeyphraseDensityRule(),
            new KeyphraseDistributionRule(),
            new KeyphraseInSubheadingsRule(),
            new TextLengthRule(),
            new OutboundLinksRule(),
            new InternalLinksRule(),
            new ImagesAndAltRule(),
            new PreviouslyUsedKeyphraseRule()
        };

        var service = new SeoAnalysisService(
            rules,
            new SeoRepository(db),
            new ArticleRepository(db),
            new UnitOfWork(db),
            Microsoft.Extensions.Options.Options.Create(new SeoAnalyzerSettings()));

        return (service, db);
    }

    [Fact]
    public async Task AnalyzeLive_ScreenshotScenario_DetectsExactProblemsAndGoodResults()
    {
        var (service, _) = CreateService();

        // Build exactly 811 words across paragraphs with 2 occurrences after paragraph 1,
        // no links, no images, keyphrase not in H2/H3, long SEO title starting with keyphrase, no meta description
        var intro = "<p>" + string.Join(" ", Enumerable.Repeat("mountain trail gear review guide", 20)) + "</p>"; // 100 words
        var middle1 = "<p>When choosing the best hiking water bottle on rugged trails, durability matters. " +
                      string.Join(" ", Enumerable.Repeat("hydration pack weight trail safety", 70)) + "</p>"; // 362 words
        var subheading = "<h2>Staying hydrated on long mountain climbs</h2>"; // 6 words
        var middle2 = "<p>Carrying the best hiking water bottle saves space in ultralight packs. " +
                      string.Join(" ", Enumerable.Repeat("stainless insulated bottle cap lid", 66)) + "</p>"; // ~343 words

        var bodyHtml = intro + subheading + middle1 + middle2;
        var words = HtmlContentParser.TokenizeWords(HtmlContentParser.StripTags(bodyHtml));
        // Adjust exact word count to 811 if needed
        if (words.Count != 811)
        {
            int diff = 811 - words.Count;
            if (diff > 0)
            {
                bodyHtml += "<p>" + string.Join(" ", Enumerable.Repeat("trail", diff)) + "</p>";
            }
        }

        var result = await service.AnalyzeLiveAsync(new AnalyzeSeoRequestDTO
        {
            FocusKeyphrase = "best hiking water bottle",
            SeoTitle = "Best hiking water bottle: complete trail hydration comparison guide for mountain trekkers and ultralight campers",
            Slug = "best-hiking-water-bottle-guide",
            MetaDescription = "",
            BodyHtml = bodyHtml
        });

        Assert.Equal(811, result.Metrics.WordCount);
        Assert.Equal(SeoTrafficLight.Red, result.Status);

        var problemIds = result.Problems.Select(p => p.RuleId).ToHashSet();
        Assert.Contains("keyphrase_distribution", problemIds);
        Assert.Contains("outbound_links", problemIds);
        Assert.Contains("images_and_alt", problemIds);
        Assert.Contains("internal_links", problemIds);
        Assert.Contains("keyphrase_in_introduction", problemIds);
        Assert.Contains("keyphrase_density", problemIds);
        Assert.Contains("meta_description_length", problemIds);
        Assert.Contains("keyphrase_in_subheading", problemIds);
        Assert.Contains("seo_title_width", problemIds);

        var goodIds = result.GoodResults.Select(g => g.RuleId).ToHashSet();
        Assert.Contains("keyphrase_in_seo_title", goodIds);
        Assert.Contains("keyphrase_length", goodIds);
        Assert.Contains("previously_used_keyphrase", goodIds);
        Assert.Contains("keyphrase_in_slug", goodIds);
        Assert.Contains("text_length", goodIds);
        Assert.Contains(result.GoodResults, g => g.RuleId == "text_length" && g.Description.Contains("811 words"));
    }

    [Fact]
    public async Task AnalyzeLive_FullyOptimizedArticle_ReturnsGreenWithHighlightsAndRelatedKeyphrases()
    {
        var (service, _) = CreateService();

        var filler = string.Join(" ", Enumerable.Repeat("philosophical inquiry nature existence simplicity", 18));
        var bodyHtml = $"""
            <p>Understanding <strong>why anything exists</strong> begins with examining why reality contains objects rather than emptiness. {filler}</p>
            <h2>Exploring why anything exists in modern metaphysics</h2>
            <p>Many thinkers argue why anything exists by comparing empty worlds with ontological simplicity. Read more in our <a href="/philosophy/metaphysics">Metaphysics section</a> and <a href="https://plato.stanford.edu">Stanford Encyclopedia</a>. {filler}</p>
            <img src="/uploads/images/cosmos.jpg" alt="Diagram explaining why anything exists" />
            <h3>Why anything exists and cosmic simplicity</h3>
            <p>Ultimately, asking why anything exists reveals deep connections between logic and cosmology. Another way to frame ontological foundations is through modal metaphysics. {filler}</p>
            <p>Whether rooted in physics or philosophy, why anything exists remains a foundational puzzle. {filler}</p>
            """;

        var result = await service.AnalyzeLiveAsync(new AnalyzeSeoRequestDTO
        {
            FocusKeyphrase = "why anything exists",
            Synonyms = new List<string> { "ontological foundations" },
            RelatedKeyphrases = new List<RelatedKeyphraseInputDTO>
            {
                new() { Keyphrase = "modern metaphysics" }
            },
            SeoTitle = "Why anything exists: a guide to ontology",
            Slug = "why-anything-exists-ontology",
            MetaDescription = "Discover why anything exists instead of nothing through modern metaphysics, cosmological simplicity, and non-ontological truths.",
            BodyHtml = bodyHtml
        });

        Assert.Equal(SeoTrafficLight.Green, result.Status);
        Assert.Empty(result.Problems);
        Assert.True(result.OverallScore >= 85);
        Assert.NotEmpty(result.Highlights);
        Assert.Single(result.RelatedKeyphraseResults);
    }

    [Fact]
    public async Task AnalyzeLive_DuplicateFocusKeyphrase_DetectsCannibalization()
    {
        var (service, db) = CreateService();

        var section = new Section { Slug = "philosophy", Title = "Philosophy", AccentColorHex = "#0c796e" };
        var existingArticle = new Article
        {
            Slug = "existing-essay",
            Title = "Existing Essay",
            Section = section,
            Status = ArticleStatus.Published,
            BodyHtml = "<p>Content</p>"
        };
        db.Sections.Add(section);
        db.Articles.Add(existingArticle);
        db.ArticleSeoProfiles.Add(new ArticleSeoProfile
        {
            Article = existingArticle,
            FocusKeyphrase = "consciousness mystery"
        });
        await db.SaveChangesAsync();

        var result = await service.AnalyzeLiveAsync(new AnalyzeSeoRequestDTO
        {
            FocusKeyphrase = "consciousness mystery",
            SeoTitle = "Consciousness mystery explained",
            Slug = "consciousness-mystery-new",
            BodyHtml = "<p>Consciousness mystery explored.</p>"
        });

        Assert.Single(result.ConflictingArticles);
        Assert.Contains(result.Improvements, i => i.RuleId == "previously_used_keyphrase");
    }
}

