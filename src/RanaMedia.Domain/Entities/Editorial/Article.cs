using RanaMedia.Domain.Enums;

namespace RanaMedia.Domain.Entities;

public class Article : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ArticleType Type { get; set; } = ArticleType.Essay;
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public string StandfirstShort { get; set; } = string.Empty;
    public string StandfirstLong { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string BodyPlainText { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    public Guid SectionId { get; set; }
    public Section Section { get; set; } = null!;

    public Guid? PrimaryTopicId { get; set; }
    public Topic? PrimaryTopic { get; set; }

    public int WordCount { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public long ViewsCount { get; set; }
    public long LoggedInViewsCount { get; set; }
    public long EngagedViewsCount { get; set; }
    public double PopularScore { get; set; }
    public bool IsHomepageFeatured { get; set; } = false;

    public string? FeaturedImageUrl { get; set; }
    public string? FeaturedImageAlt { get; set; }
    public string? FeaturedImageCaptionHtml { get; set; }
    public string? CardImageUrl { get; set; }
    public string? CardImageAlt { get; set; }
    public int? CardImageWidth { get; set; }
    public int? CardImageHeight { get; set; }
    public string? DynamicImageUrl { get; set; }
    public bool DynamicImageCardEnabled { get; set; }
    public bool DynamicImageArticleEnabled { get; set; }
    public string DisplaySettingsJson { get; set; } = "{\"alignX\":\"center\",\"alignY\":\"center\",\"backdropStrength\":2,\"featureImgCaptionAlign\":\"right\"}";

    public bool AudioEnabled { get; set; }
    public string? AudioUrl { get; set; }
    public int AudioDurationSeconds { get; set; }

    public VideoHoster VideoHoster { get; set; } = VideoHoster.None;
    public string? VideoEmbedUrl { get; set; }
    public int VideoDurationMinutes { get; set; }
    public string? CreditsShort { get; set; }
    public string? CreditsHtml { get; set; }

    public bool CreativeCommons { get; set; }
    public string LicenseName { get; set; } = "Aeon Permissive (syndication allowed)";
    public string LicenseShort { get; set; } = "Aeon Permissive License";
    public bool RepublicationAllowed { get; set; } = true;
    public bool CommercialUseAllowed { get; set; } = true;

    public bool CommentsEnabled { get; set; } = true;
    public bool CommentsOpen { get; set; } = true;
    public bool NewsletterHidden { get; set; } = false;
    public string? PreviewToken { get; set; }

    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? ExpiredAt { get; set; }

    public ICollection<ArticleTopic> ArticleTopics { get; set; } = new List<ArticleTopic>();
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    public ICollection<ArticleContributor> ArticleContributors { get; set; } = new List<ArticleContributor>();
    public ICollection<ArticleRelation> RelatedFrom { get; set; } = new List<ArticleRelation>();
    public ICollection<ArticleRelation> RelatedTo { get; set; } = new List<ArticleRelation>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<UserSave> UserSaves { get; set; } = new List<UserSave>();
    public ICollection<PageMark> PageMarks { get; set; } = new List<PageMark>();
    public ICollection<ArticleViewEvent> ViewEvents { get; set; } = new List<ArticleViewEvent>();
    public ArticleSeoProfile? SeoProfile { get; set; }
}

