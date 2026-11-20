using Microsoft.AspNetCore.Http;
using RanaMedia.Contract.DTOs.Common;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Contract.DTOs.Forms;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Contract.Services.Editorial;

public interface IHomeService
{
    Task<HomePageDTO> GetHomePageAsync(CancellationToken ct = default);
    Task<CursorPagedResult<ArticleCardDTO>> GetMoreHomeArticlesAsync(string? afterCursor, int first = 30, CancellationToken ct = default);
}

public interface IArticleService
{
    Task<CursorPagedResult<ArticleCardDTO>> GetArticlesAsync(
        ArticleType? type = null,
        string sort = "latest",
        string? tag = null,
        string? afterCursor = null,
        int first = 12,
        CancellationToken ct = default);
    Task<ArticleDetailDTO> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<ArticleDetailDTO> GetPreviewAsync(string slug, string token, CancellationToken ct = default);
    Task<PaginatedResult<AdminArticleListItemDTO>> GetAdminArticlesAsync(
        ArticleStatus? status = null,
        ArticleType? type = null,
        string? search = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<AdminArticleEditDTO> GetAdminArticleByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminArticleResponseDTO> CreateAsync(CreateArticleDTO request, CancellationToken ct = default);
    Task<AdminArticleResponseDTO> UpdateAsync(Guid id, UpdateArticleDTO request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IVideoService
{
    Task<CursorPagedResult<ArticleCardDTO>> GetVideosAsync(string? filter = null, string? afterCursor = null, int first = 12, CancellationToken ct = default);
    Task<VideoDetailDTO> GetVideoBySlugAsync(string slug, CancellationToken ct = default);
}

public interface ISectionService
{
    Task<List<SectionSummaryDTO>> GetAllSectionsAsync(CancellationToken ct = default);
    Task<List<SectionCatalogDTO>> GetCatalogAsync(CancellationToken ct = default);
    Task<SectionDetailDTO> GetSectionBySlugAsync(string slug, string sort = "latest", string? afterCursor = null, int first = 24, CancellationToken ct = default);
}

public interface ITopicService
{
    Task<TopicDetailDTO> GetTopicBySlugAsync(string sectionSlug, string topicSlug, string sort = "latest", string? afterCursor = null, int first = 24, CancellationToken ct = default);
}

public interface IContributorService
{
    Task<ContributorProfileDTO> GetContributorBySlugAsync(string slug, string? authoredCursor = null, string? editedCursor = null, CancellationToken ct = default);
    Task<ContributorProfileDTO> CreateContributorAsync(CreateContributorDTO request, CancellationToken ct = default);
    Task<List<ContributorShortDTO>> GetAllContributorsAsync(CancellationToken ct = default);
    Task DeleteContributorAsync(Guid id, CancellationToken ct = default);
}

public interface ISearchService
{
    Task<CursorPagedResult<ArticleCardDTO>> SearchAsync(string query, ArticleType? type = null, string? afterCursor = null, int first = 30, CancellationToken ct = default);
}

public interface ILibraryService
{
    Task<LibrarySummaryDTO> GetUserLibraryAsync(Guid userId, CancellationToken ct = default);
    Task<List<ArticleCardDTO>> GetUserAnnotatedArticlesAsync(Guid userId, CancellationToken ct = default);
    Task<List<Guid>> GetSavedArticleIdsAsync(Guid userId, CancellationToken ct = default);
    Task SaveArticleAsync(Guid userId, string slug, CancellationToken ct = default);
    Task RemoveSavedArticleAsync(Guid userId, string slug, CancellationToken ct = default);
}

public interface IPageMarkService
{
    Task<List<PageMarkDTO>> GetUserArticleMarksAsync(Guid userId, string slug, CancellationToken ct = default);
    Task<List<PageMarkDTO>> ReplaceArticleMarksAsync(Guid userId, string slug, UpdatePageMarksRequestDTO request, CancellationToken ct = default);
}

public interface ICommentService
{
    Task<CursorPagedResult<CommentDTO>> GetArticleCommentsAsync(string slug, Guid? currentUserId, string sort = "newest", string? afterCursor = null, int first = 20, CancellationToken ct = default);
    Task<CommentDTO> CreateCommentAsync(Guid userId, string slug, CreateCommentDTO request, CancellationToken ct = default);
    Task<CommentDTO> UpdateCommentAsync(Guid userId, Guid commentId, UpdateCommentDTO request, CancellationToken ct = default);
    Task DeleteCommentAsync(Guid userId, bool isModerator, Guid commentId, CancellationToken ct = default);
    Task<CommentReplyDTO> CreateReplyAsync(Guid userId, Guid commentId, CreateReplyDTO request, CancellationToken ct = default);
    Task<CommentReplyDTO> UpdateReplyAsync(Guid userId, Guid replyId, UpdateCommentDTO request, CancellationToken ct = default);
    Task DeleteReplyAsync(Guid userId, bool isModerator, Guid replyId, CancellationToken ct = default);
    Task ToggleLikeAsync(Guid userId, Guid? commentId, Guid? replyId, bool like, CancellationToken ct = default);
    Task ReportAsync(Guid userId, Guid? commentId, Guid? replyId, ReportCommentDTO request, CancellationToken ct = default);
    Task ModerateCommentAsync(Guid commentId, bool moderate, string? reason, CancellationToken ct = default);
    Task FeatureCommentAsync(Guid commentId, bool feature, CancellationToken ct = default);
    Task<List<ArticleCardDTO>> GetConversationsAsync(int limit = 20, CancellationToken ct = default);
    Task<PaginatedResult<AdminCommentItemDTO>> GetAdminCommentsAsync(bool? onlyModerated = null, bool? onlyReported = null, int pageNumber = 1, int pageSize = 30, CancellationToken ct = default);
}

public interface IAdminDashboardService
{
    Task<AdminDashboardStatsDTO> GetStatsAsync(CancellationToken ct = default);
}

public interface INewsletterService
{
    Task SubscribeAsync(NewsletterSubscribeDTO request, CancellationToken ct = default);
    Task ConfirmSubscriptionAsync(string token, CancellationToken ct = default);
    Task UnsubscribeAsync(string token, CancellationToken ct = default);
}

public interface IPitchService
{
    Task<PitchResponseDTO> SubmitPitchAsync(CreatePitchDTO request, CancellationToken ct = default);
    Task<List<PitchResponseDTO>> GetAllPitchesAsync(SubmissionStatus? status = null, CancellationToken ct = default);
    Task<PitchResponseDTO> UpdatePitchStatusAsync(Guid id, SubmissionStatus status, string? editorNotes, CancellationToken ct = default);
}

public interface ISyndicationService
{
    Task<SyndicationResponseDTO> SubmitSyndicationAsync(CreateSyndicationDTO request, CancellationToken ct = default);
    Task<List<SyndicationResponseDTO>> GetAllRequestsAsync(SubmissionStatus? status = null, CancellationToken ct = default);
    Task<SyndicationResponseDTO> UpdateStatusAsync(Guid id, SubmissionStatus status, CancellationToken ct = default);
}

public interface IViewAnalyticsService
{
    Task TrackViewAsync(TrackViewDTO request, Guid? userId, string clientFingerprint, CancellationToken ct = default);
    Task RecalculatePopularScoresAsync(CancellationToken ct = default);
}

public interface IFeedService
{
    Task<string> GenerateRssAsync(ArticleType? type = null, string baseUrl = "https://ranamedia.co", CancellationToken ct = default);
    Task<string> GenerateAtomAsync(ArticleType? type = null, string baseUrl = "https://ranamedia.co", CancellationToken ct = default);
    Task<string> GenerateSitemapXmlAsync(string baseUrl = "https://ranamedia.co", CancellationToken ct = default);
}

public interface IMediaStorageService
{
    Task<MediaUploadResultDTO> UploadAsync(IFormFile file, MediaFolder folder, CancellationToken ct = default);
    Task DeleteAsync(string relativeUrl, CancellationToken ct = default);
}

public interface ITurnstileVerifier
{
    Task<bool> VerifyAsync(string? token, string? remoteIp = null, CancellationToken ct = default);
}

