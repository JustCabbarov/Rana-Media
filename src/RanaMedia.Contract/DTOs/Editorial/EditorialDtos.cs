using System.ComponentModel.DataAnnotations;
using RanaMedia.Contract.DTOs.Common;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Contract.DTOs.Editorial;

public class DisplaySettingsDTO
{
    public string AlignX { get; set; } = "center";
    public string AlignY { get; set; } = "center";
    public int BackdropStrength { get; set; } = 2;
    public string FeatureImgCaptionAlign { get; set; } = "right";
    public string? BadgeColor { get; set; }
    public string? CardPlayIconColor { get; set; }
}

public class TagDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class SectionSummaryDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AccentColorHex { get; set; } = string.Empty;
}

public class TopicSummaryDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public SectionSummaryDTO? Section { get; set; }
}

public class SectionDetailDTO : SectionSummaryDTO
{
    public string Description { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public List<TopicSummaryDTO> Topics { get; set; } = new();
    public CursorPagedResult<ArticleCardDTO> Articles { get; set; } = new();
}

public class TopicDetailDTO : TopicSummaryDTO
{
    public string Description { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public List<TopicSummaryDTO> SiblingTopics { get; set; } = new();
    public CursorPagedResult<ArticleCardDTO> Articles { get; set; } = new();
}

public class ContributorShortDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShortBioHtml { get; set; }
    public string? AvatarUrl { get; set; }
}

public class ContributorProfileDTO : ContributorShortDTO
{
    public string? Occupation { get; set; }
    public string? Company { get; set; }
    public string BioHtml { get; set; } = string.Empty;
    public string? Twitter { get; set; }
    public string? Bluesky { get; set; }
    public string? LinkedIn { get; set; }
    public string? AcademiaEdu { get; set; }
    public string? Website { get; set; }
    public CursorPagedResult<ArticleCardDTO> AuthoredArticles { get; set; } = new();
    public CursorPagedResult<ArticleCardDTO> EditedArticles { get; set; } = new();
}

public class CreateContributorDTO
{
    public Guid? AppUserId { get; set; }
    [Required]
    public string Slug { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Occupation { get; set; }
    public string? Company { get; set; }
    public string BioHtml { get; set; } = string.Empty;
    public string? ShortBioHtml { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Twitter { get; set; }
    public string? Bluesky { get; set; }
    public string? LinkedIn { get; set; }
    public string? AcademiaEdu { get; set; }
    public string? Website { get; set; }
}

public class ArticleCardDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ArticleType Type { get; set; }
    public string StandfirstShort { get; set; } = string.Empty;
    public string StandfirstLong { get; set; } = string.Empty;
    public int VideoDurationMinutes { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public string? CreditsShort { get; set; }
    public bool CommentsEnabled { get; set; }
    public DisplaySettingsDTO Settings { get; set; } = new();
    public List<TagDTO> Tags { get; set; } = new();
    public List<ContributorShortDTO> Authors { get; set; } = new();
    public string? CardImageUrl { get; set; }
    public string? CardImageAlt { get; set; }
    public int? CardImageWidth { get; set; }
    public int? CardImageHeight { get; set; }
    public string? DynamicImageUrl { get; set; }
    public bool DynamicImageCardEnabled { get; set; }
    public TopicSummaryDTO? PrimaryTopic { get; set; }
    public SectionSummaryDTO? Section { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class ArticleDetailDTO : ArticleCardDTO
{
    public ArticleStatus Status { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public int WordCount { get; set; }
    public long ViewsCount { get; set; }
    public long LoggedInViewsCount { get; set; }
    public long EngagedViewsCount { get; set; }

    public string? FeaturedImageUrl { get; set; }
    public string? FeaturedImageAlt { get; set; }
    public string? FeaturedImageCaptionHtml { get; set; }
    public bool DynamicImageArticleEnabled { get; set; }

    public bool AudioEnabled { get; set; }
    public string? AudioUrl { get; set; }
    public int AudioDurationSeconds { get; set; }

    public bool CreativeCommons { get; set; }
    public string LicenseName { get; set; } = string.Empty;
    public string LicenseShort { get; set; } = string.Empty;
    public bool RepublicationAllowed { get; set; }
    public bool CommercialUseAllowed { get; set; }

    public bool CommentsOpen { get; set; }
    public int CommentAndRepliesCount { get; set; }
    public bool NewsletterHidden { get; set; }

    public ContributorShortDTO? Editor { get; set; }
    public List<TopicSummaryDTO> Topics { get; set; } = new();
    public List<ArticleCardDTO> Related { get; set; } = new();
    public List<CommentDTO> TopComments { get; set; } = new();
}

public class VideoDetailDTO : ArticleDetailDTO
{
    public VideoHoster VideoHoster { get; set; }
    public string? VideoEmbedUrl { get; set; }
    public string? CreditsHtml { get; set; }
    public bool IsExpired { get; set; }
    public List<ArticleCardDTO> RecentVideos { get; set; } = new();
}

public class HomePageDTO
{
    public ArticleDetailDTO? FeaturedEssay { get; set; }
    public List<ArticleCardDTO> PopularThisMonth { get; set; } = new();
    public CursorPagedResult<ArticleCardDTO> Articles { get; set; } = new();
}

public class CreateArticleDTO
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Slug { get; set; } = string.Empty;
    public ArticleType Type { get; set; } = ArticleType.Essay;
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    [Required]
    public string StandfirstShort { get; set; } = string.Empty;
    public string StandfirstLong { get; set; } = string.Empty;
    [Required]
    public string BodyHtml { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    [Required]
    public Guid SectionId { get; set; }
    public Guid? PrimaryTopicId { get; set; }
    public List<Guid> TopicIds { get; set; } = new();
    public List<string> TagNames { get; set; } = new();
    public List<Guid> AuthorIds { get; set; } = new();
    public Guid? EditorId { get; set; }
    public List<Guid> RelatedArticleIds { get; set; } = new();

    public bool IsHomepageFeatured { get; set; }
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
    public DisplaySettingsDTO Settings { get; set; } = new();

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
    public DateTime? ScheduledAt { get; set; }

    // Yoast SEO fields (saved & analyzed automatically)
    public string? FocusKeyphrase { get; set; }
    public List<string> Synonyms { get; set; } = new();
    public List<RelatedKeyphraseInputDTO> RelatedKeyphrases { get; set; } = new();
}

public class UpdateArticleDTO : CreateArticleDTO
{
}

public class AdminArticleResponseDTO
{
    public ArticleDetailDTO Article { get; set; } = null!;
    public SeoAnalysisResponseDTO? SeoAnalysis { get; set; }
}

public class MediaUploadResultDTO
{
    public string FileName { get; set; } = string.Empty;
    public string RelativeUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}

public class TopicOptionDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class SectionCatalogDTO : SectionSummaryDTO
{
    public string Description { get; set; } = string.Empty;
    public List<TopicOptionDTO> Topics { get; set; } = new();
}

public class AdminArticleListItemDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ArticleType Type { get; set; }
    public ArticleStatus Status { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public string SectionSlug { get; set; } = string.Empty;
    public string SectionColor { get; set; } = string.Empty;
    public string? PrimaryTopicTitle { get; set; }
    public List<string> Authors { get; set; } = new();
    public int WordCount { get; set; }
    public long ViewsCount { get; set; }
    public int CommentAndRepliesCount { get; set; }
    public bool IsHomepageFeatured { get; set; }
    public bool CommentsEnabled { get; set; }
    public bool CommentsOpen { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? SeoScore { get; set; }
    public SeoTrafficLight? SeoStatus { get; set; }
    public string? FocusKeyphrase { get; set; }
}

public class AdminArticleEditDTO : CreateArticleDTO
{
    public Guid Id { get; set; }
    public string PreviewToken { get; set; } = string.Empty;
    public long ViewsCount { get; set; }
    public int WordCount { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public DateTime? PublishedAt { get; set; }
    public SeoAnalysisResponseDTO? SeoAnalysis { get; set; }
}

public class AdminCommentItemDTO
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
    public string ArticleSlug { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsModerated { get; set; }
    public string? ModerationReason { get; set; }
    public bool IsFeatured { get; set; }
    public int LikesCount { get; set; }
    public int RepliesCount { get; set; }
    public int ReportsCount { get; set; }
}

public class AdminDashboardStatsDTO
{
    public int TotalArticles { get; set; }
    public int PublishedArticles { get; set; }
    public int DraftArticles { get; set; }
    public int ScheduledArticles { get; set; }
    public long TotalViews { get; set; }
    public int TotalUsers { get; set; }
    public int TotalComments { get; set; }
    public int ModeratedComments { get; set; }
    public int PendingPitches { get; set; }
    public int PendingSyndications { get; set; }
    public int ActiveSubscribers { get; set; }
    public int AverageSeoScore { get; set; }
    public int GreenSeoCount { get; set; }
    public int OrangeSeoCount { get; set; }
    public int RedSeoCount { get; set; }
}

