using RanaMedia.Domain.Enums;

namespace RanaMedia.Domain.Entities;

public class ArticleSeoProfile : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public string FocusKeyphrase { get; set; } = string.Empty;
    public string SynonymsJson { get; set; } = "[]";
    public string? SeoTitle { get; set; }
    public string? MetaDescription { get; set; }

    public int OverallScore { get; set; }
    public SeoTrafficLight OverallStatus { get; set; } = SeoTrafficLight.Red;
    public int ProblemsCount { get; set; }
    public int ImprovementsCount { get; set; }
    public int GoodResultsCount { get; set; }

    public string LastAnalysisJson { get; set; } = "{}";
    public DateTime LastAnalyzedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RelatedKeyphrase> RelatedKeyphrases { get; set; } = new List<RelatedKeyphrase>();
    public ICollection<SeoAnalysisSnapshot> Snapshots { get; set; } = new List<SeoAnalysisSnapshot>();
}

public class RelatedKeyphrase : BaseEntity
{
    public Guid ArticleSeoProfileId { get; set; }
    public ArticleSeoProfile ArticleSeoProfile { get; set; } = null!;

    public string Keyphrase { get; set; } = string.Empty;
    public string SynonymsJson { get; set; } = "[]";
    public int Score { get; set; }
    public SeoTrafficLight Status { get; set; } = SeoTrafficLight.Red;
    public string ResultsJson { get; set; } = "{}";
}

public class SeoAnalysisSnapshot : BaseEntity
{
    public Guid ArticleSeoProfileId { get; set; }
    public ArticleSeoProfile ArticleSeoProfile { get; set; } = null!;

    public string FocusKeyphrase { get; set; } = string.Empty;
    public int OverallScore { get; set; }
    public SeoTrafficLight OverallStatus { get; set; }
    public int ProblemsCount { get; set; }
    public int ImprovementsCount { get; set; }
    public int GoodResultsCount { get; set; }
    public string ResultsJson { get; set; } = "{}";
}

