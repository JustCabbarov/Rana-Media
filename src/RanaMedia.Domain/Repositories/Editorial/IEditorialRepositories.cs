using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Domain.Repositories.Editorial;

public interface IArticleRepository : IGenericRepository<Article>
{
    Task<Article?> GetBySlugDetailedAsync(string slug, bool includeUnpublished = false, CancellationToken ct = default);
    Task<Article?> GetFeaturedHomepageArticleAsync(CancellationToken ct = default);
    Task<List<Article>> GetPopularArticlesAsync(int count, ArticleType? type = null, string? sectionSlug = null, CancellationToken ct = default);
    Task<(List<Article> Items, bool HasNextPage, string? EndCursor)> GetPagedAsync(
        int first,
        string? afterCursor,
        ArticleType? type = null,
        string? sectionSlug = null,
        string? topicSlug = null,
        string? tagSlug = null,
        bool orderByPopular = false,
        string? videoCategoryFilter = null,
        CancellationToken ct = default);
    Task<(List<Article> Items, int TotalCount, bool HasNextPage, string? EndCursor)> SearchAsync(
        string query,
        ArticleType? type,
        int first,
        string? afterCursor,
        CancellationToken ct = default);
    Task<List<Article>> GetScheduledDueAsync(DateTime utcNow, CancellationToken ct = default);
    Task<List<Article>> GetPublishedForFeedAsync(int limit, ArticleType? type = null, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeArticleId = null, CancellationToken ct = default);
}

public interface ISectionRepository : IGenericRepository<Section>
{
    Task<Section?> GetBySlugWithTopicsAsync(string slug, CancellationToken ct = default);
    Task<List<Section>> GetAllWithTopicsAsync(CancellationToken ct = default);
}

public interface ITopicRepository : IGenericRepository<Topic>
{
    Task<Topic?> GetBySlugWithSectionAsync(string slug, string? sectionSlug = null, CancellationToken ct = default);
}

public interface IContributorRepository : IGenericRepository<Contributor>
{
    Task<Contributor?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<(List<Article> Items, bool HasNextPage, string? EndCursor)> GetContributorArticlesAsync(
        Guid contributorId,
        ContributorRole role,
        int first,
        string? afterCursor,
        CancellationToken ct = default);
}

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<bool> UserHasRootCommentAsync(Guid articleId, Guid userId, CancellationToken ct = default);
    Task<(List<Comment> Items, bool HasNextPage, string? EndCursor)> GetArticleCommentsAsync(
        Guid articleId,
        int first,
        string? afterCursor,
        string sort = "newest",
        CancellationToken ct = default);
    Task<Comment?> GetByIdWithDetailsAsync(Guid commentId, CancellationToken ct = default);
    Task<CommentReply?> GetReplyByIdAsync(Guid replyId, CancellationToken ct = default);
    Task<CommentLike?> GetLikeAsync(Guid userId, Guid? commentId, Guid? replyId, CancellationToken ct = default);
    Task AddReplyAsync(CommentReply reply, CancellationToken ct = default);
    Task AddLikeAsync(CommentLike like, CancellationToken ct = default);
    void RemoveLike(CommentLike like);
    Task AddReportAsync(CommentReport report, CancellationToken ct = default);
    Task<List<Article>> GetActiveConversationArticlesAsync(int limit = 20, CancellationToken ct = default);
}

public interface IUserSaveRepository : IGenericRepository<UserSave>
{
    Task<List<Guid>> GetSavedArticleIdsAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetUnseenSavedCountAsync(Guid userId, CancellationToken ct = default);
    Task<UserSave?> GetByUserAndArticleAsync(Guid userId, Guid articleId, CancellationToken ct = default);
    Task<List<UserSave>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default);
}

public interface IPageMarkRepository : IGenericRepository<PageMark>
{
    Task<List<PageMark>> GetUserArticleMarksAsync(Guid userId, Guid articleId, CancellationToken ct = default);
    Task<List<Article>> GetUserAnnotatedArticlesAsync(Guid userId, CancellationToken ct = default);
    Task ReplaceArticleMarksAsync(Guid userId, Guid articleId, List<PageMark> newMarks, CancellationToken ct = default);
}

public interface ISeoRepository : IGenericRepository<ArticleSeoProfile>
{
    Task<ArticleSeoProfile?> GetByArticleIdAsync(Guid articleId, CancellationToken ct = default);
    Task<List<ArticleSeoProfile>> GetArticlesUsingKeyphraseAsync(string normalizedKeyphrase, Guid? excludeArticleId = null, CancellationToken ct = default);
}
