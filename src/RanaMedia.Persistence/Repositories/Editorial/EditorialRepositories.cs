using System.Text;
using Microsoft.EntityFrameworkCore;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Persistence.Data;
using RanaMedia.Persistence.Repositories.System;

namespace RanaMedia.Persistence.Repositories.Editorial;

internal static class CursorHelper
{
    public static int DecodeOffset(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor)) return 0;
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var str = Encoding.UTF8.GetString(bytes);
            return int.TryParse(str, out var offset) && offset >= 0 ? offset : 0;
        }
        catch
        {
            return 0;
        }
    }

    public static string EncodeOffset(int offset)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(offset.ToString()));
    }
}

public class ArticleRepository : GenericRepository<Article>, IArticleRepository
{
    public ArticleRepository(AppDbContext context) : base(context) { }

    private IQueryable<Article> BaseCardQuery() =>
        _dbSet.AsNoTracking()
            .Include(a => a.Section)
            .Include(a => a.PrimaryTopic).ThenInclude(t => t!.Section)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor);

    private IQueryable<Article> BaseDetailedQuery() =>
        _dbSet.AsNoTracking()
            .Include(a => a.Section)
            .Include(a => a.PrimaryTopic).ThenInclude(t => t!.Section)
            .Include(a => a.ArticleTopics).ThenInclude(at => at.Topic).ThenInclude(t => t.Section)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Include(a => a.RelatedFrom).ThenInclude(r => r.RelatedArticle).ThenInclude(ra => ra.Section)
            .Include(a => a.RelatedFrom).ThenInclude(r => r.RelatedArticle).ThenInclude(ra => ra.PrimaryTopic)
            .Include(a => a.RelatedFrom).ThenInclude(r => r.RelatedArticle).ThenInclude(ra => ra.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Include(a => a.Comments.Where(c => !c.IsModerated))
                .ThenInclude(c => c.User)
            .Include(a => a.Comments.Where(c => !c.IsModerated))
                .ThenInclude(c => c.Replies.Where(r => !r.IsModerated))
                .ThenInclude(r => r.User);

    public async Task<Article?> GetBySlugDetailedAsync(string slug, bool includeUnpublished = false, CancellationToken ct = default)
    {
        var query = BaseDetailedQuery().Where(a => a.Slug == slug);
        if (!includeUnpublished)
            query = query.Where(a => a.Status == ArticleStatus.Published || a.Status == ArticleStatus.Expired);

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<Article?> GetFeaturedHomepageArticleAsync(CancellationToken ct = default)
    {
        var featured = await BaseDetailedQuery()
            .Where(a => a.Status == ArticleStatus.Published && a.IsHomepageFeatured)
            .OrderByDescending(a => a.PublishedAt)
            .FirstOrDefaultAsync(ct);

        if (featured != null) return featured;

        return await BaseDetailedQuery()
            .Where(a => a.Status == ArticleStatus.Published && a.Type == ArticleType.Essay)
            .OrderByDescending(a => a.PublishedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Article>> GetPopularArticlesAsync(int count, ArticleType? type = null, string? sectionSlug = null, CancellationToken ct = default)
    {
        var query = BaseCardQuery().Where(a => a.Status == ArticleStatus.Published);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(sectionSlug))
            query = query.Where(a => a.Section.Slug == sectionSlug);

        return await query
            .OrderByDescending(a => a.PopularScore)
            .ThenByDescending(a => a.ViewsCount)
            .ThenByDescending(a => a.PublishedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<(List<Article> Items, bool HasNextPage, string? EndCursor)> GetPagedAsync(
        int first,
        string? afterCursor,
        ArticleType? type = null,
        string? sectionSlug = null,
        string? topicSlug = null,
        string? tagSlug = null,
        bool orderByPopular = false,
        string? videoCategoryFilter = null,
        CancellationToken ct = default)
    {
        var offset = CursorHelper.DecodeOffset(afterCursor);
        var query = BaseCardQuery().Where(a => a.Status == ArticleStatus.Published);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(sectionSlug))
            query = query.Where(a => a.Section.Slug == sectionSlug);

        if (!string.IsNullOrWhiteSpace(topicSlug))
            query = query.Where(a =>
                (a.PrimaryTopic != null && a.PrimaryTopic.Slug == topicSlug) ||
                a.ArticleTopics.Any(at => at.Topic.Slug == topicSlug));

        if (!string.IsNullOrWhiteSpace(tagSlug))
            query = query.Where(a => a.ArticleTags.Any(at => at.Tag.Slug == tagSlug || at.Tag.Name == tagSlug));

        if (!string.IsNullOrWhiteSpace(videoCategoryFilter))
            query = query.Where(a => a.ArticleTags.Any(at => at.Tag.Slug == videoCategoryFilter.ToLower()));

        query = orderByPopular
            ? query.OrderByDescending(a => a.PopularScore).ThenByDescending(a => a.ViewsCount).ThenByDescending(a => a.PublishedAt)
            : query.OrderByDescending(a => a.PublishedAt).ThenByDescending(a => a.CreatedAt);

        var items = await query.Skip(offset).Take(first + 1).ToListAsync(ct);
        var hasNext = items.Count > first;
        if (hasNext) items.RemoveAt(first);

        var endCursor = hasNext ? CursorHelper.EncodeOffset(offset + first) : null;
        return (items, hasNext, endCursor);
    }

    public async Task<(List<Article> Items, int TotalCount, bool HasNextPage, string? EndCursor)> SearchAsync(
        string queryText,
        ArticleType? type,
        int first,
        string? afterCursor,
        CancellationToken ct = default)
    {
        var offset = CursorHelper.DecodeOffset(afterCursor);
        var normalized = (queryText ?? string.Empty).Trim().ToLowerInvariant();

        var query = BaseCardQuery().Where(a => a.Status == ArticleStatus.Published);
        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(normalized))
        {
            query = query.Where(a =>
                a.Title.ToLower().Contains(normalized) ||
                a.StandfirstShort.ToLower().Contains(normalized) ||
                a.StandfirstLong.ToLower().Contains(normalized) ||
                a.BodyPlainText.ToLower().Contains(normalized) ||
                a.Section.Title.ToLower().Contains(normalized) ||
                (a.PrimaryTopic != null && a.PrimaryTopic.Title.ToLower().Contains(normalized)) ||
                a.ArticleContributors.Any(ac => ac.Contributor.Name.ToLower().Contains(normalized)));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip(offset)
            .Take(first + 1)
            .ToListAsync(ct);

        var hasNext = items.Count > first;
        if (hasNext) items.RemoveAt(first);
        var endCursor = hasNext ? CursorHelper.EncodeOffset(offset + first) : null;

        return (items, totalCount, hasNext, endCursor);
    }

    public async Task<List<Article>> GetScheduledDueAsync(DateTime utcNow, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(a => a.Status == ArticleStatus.Scheduled && a.ScheduledAt != null && a.ScheduledAt <= utcNow)
            .ToListAsync(ct);
    }

    public async Task<List<Article>> GetPublishedForFeedAsync(int limit, ArticleType? type = null, CancellationToken ct = default)
    {
        var query = BaseCardQuery().Where(a => a.Status == ArticleStatus.Published);
        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        return await query
            .OrderByDescending(a => a.PublishedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeArticleId = null, CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(
            a => a.Slug == slug && (!excludeArticleId.HasValue || a.Id != excludeArticleId.Value),
            ct);
    }
}

public class SectionRepository : GenericRepository<Section>, ISectionRepository
{
    public SectionRepository(AppDbContext context) : base(context) { }

    public async Task<Section?> GetBySlugWithTopicsAsync(string slug, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(s => s.Topics.OrderBy(t => t.Title))
            .FirstOrDefaultAsync(s => s.Slug == slug, ct);
    }

    public async Task<List<Section>> GetAllWithTopicsAsync(CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(s => s.Topics.OrderBy(t => t.Title))
            .OrderBy(s => s.SortOrder)
            .ToListAsync(ct);
    }
}

public class TopicRepository : GenericRepository<Topic>, ITopicRepository
{
    public TopicRepository(AppDbContext context) : base(context) { }

    public async Task<Topic?> GetBySlugWithSectionAsync(string slug, string? sectionSlug = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking()
            .Include(t => t.Section).ThenInclude(s => s.Topics.OrderBy(st => st.Title))
            .Where(t => t.Slug == slug);

        if (!string.IsNullOrWhiteSpace(sectionSlug))
            query = query.Where(t => t.Section.Slug == sectionSlug);

        return await query.FirstOrDefaultAsync(ct);
    }
}

public class ContributorRepository : GenericRepository<Contributor>, IContributorRepository
{
    public ContributorRepository(AppDbContext context) : base(context) { }

    public async Task<Contributor?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug, ct);
    }

    public async Task<(List<Article> Items, bool HasNextPage, string? EndCursor)> GetContributorArticlesAsync(
        Guid contributorId,
        ContributorRole role,
        int first,
        string? afterCursor,
        CancellationToken ct = default)
    {
        var offset = CursorHelper.DecodeOffset(afterCursor);
        var query = _context.Articles
            .AsNoTracking()
            .Include(a => a.Section)
            .Include(a => a.PrimaryTopic).ThenInclude(t => t!.Section)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Where(a => a.Status == ArticleStatus.Published &&
                        a.ArticleContributors.Any(ac => ac.ContributorId == contributorId && ac.Role == role))
            .OrderByDescending(a => a.PublishedAt);

        var items = await query.Skip(offset).Take(first + 1).ToListAsync(ct);
        var hasNext = items.Count > first;
        if (hasNext) items.RemoveAt(first);
        return (items, hasNext, hasNext ? CursorHelper.EncodeOffset(offset + first) : null);
    }
}

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context) { }

    public async Task<bool> UserHasRootCommentAsync(Guid articleId, Guid userId, CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(c => c.ArticleId == articleId && c.UserId == userId, ct);
    }

    public async Task<(List<Comment> Items, bool HasNextPage, string? EndCursor)> GetArticleCommentsAsync(
        Guid articleId,
        int first,
        string? afterCursor,
        string sort = "newest",
        CancellationToken ct = default)
    {
        var offset = CursorHelper.DecodeOffset(afterCursor);
        var query = _dbSet.AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Replies.Where(r => !r.IsModerated).OrderBy(r => r.CreatedAt))
                .ThenInclude(r => r.User)
            .Where(c => c.ArticleId == articleId && !c.IsModerated);

        query = sort.Equals("popular", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(c => c.IsFeatured).ThenByDescending(c => c.LikesCount).ThenByDescending(c => c.CreatedAt)
            : query.OrderByDescending(c => c.IsFeatured).ThenByDescending(c => c.CreatedAt);

        var items = await query.Skip(offset).Take(first + 1).ToListAsync(ct);
        var hasNext = items.Count > first;
        if (hasNext) items.RemoveAt(first);
        return (items, hasNext, hasNext ? CursorHelper.EncodeOffset(offset + first) : null);
    }

    public async Task<Comment?> GetByIdWithDetailsAsync(Guid commentId, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Article).ThenInclude(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Include(c => c.Replies.OrderBy(r => r.CreatedAt)).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.Id == commentId, ct);
    }

    public async Task<CommentReply?> GetReplyByIdAsync(Guid replyId, CancellationToken ct = default)
    {
        return await _context.CommentReplies
            .Include(r => r.User)
            .Include(r => r.Comment)
            .FirstOrDefaultAsync(r => r.Id == replyId, ct);
    }

    public async Task<CommentLike?> GetLikeAsync(Guid userId, Guid? commentId, Guid? replyId, CancellationToken ct = default)
    {
        return await _context.CommentLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.CommentId == commentId && l.ReplyId == replyId, ct);
    }

    public async Task AddReplyAsync(CommentReply reply, CancellationToken ct = default)
    {
        await _context.CommentReplies.AddAsync(reply, ct);
    }

    public async Task AddLikeAsync(CommentLike like, CancellationToken ct = default)
    {
        await _context.CommentLikes.AddAsync(like, ct);
    }

    public void RemoveLike(CommentLike like)
    {
        _context.CommentLikes.Remove(like);
    }

    public async Task AddReportAsync(CommentReport report, CancellationToken ct = default)
    {
        await _context.CommentReports.AddAsync(report, ct);
    }

    public async Task<List<Article>> GetActiveConversationArticlesAsync(int limit = 20, CancellationToken ct = default)
    {
        return await _context.Articles
            .AsNoTracking()
            .Include(a => a.Section)
            .Include(a => a.PrimaryTopic)
            .Include(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Where(a => a.Status == ArticleStatus.Published && a.CommentsEnabled && a.Comments.Any(c => !c.IsModerated))
            .OrderByDescending(a => a.Comments.Max(c => c.CreatedAt))
            .Take(limit)
            .ToListAsync(ct);
    }
}

public class UserSaveRepository : GenericRepository<UserSave>, IUserSaveRepository
{
    public UserSaveRepository(AppDbContext context) : base(context) { }

    public async Task<List<Guid>> GetSavedArticleIdsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _dbSet.Where(s => s.UserId == userId).Select(s => s.ArticleId).ToListAsync(ct);
    }

    public async Task<int> GetUnseenSavedCountAsync(Guid userId, CancellationToken ct = default)
    {
        return await _dbSet.CountAsync(s => s.UserId == userId && !s.IsSeen, ct);
    }

    public async Task<UserSave?> GetByUserAndArticleAsync(Guid userId, Guid articleId, CancellationToken ct = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId && s.ArticleId == articleId, ct);
    }

    public async Task<List<UserSave>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(s => s.Article).ThenInclude(a => a.Section)
            .Include(s => s.Article).ThenInclude(a => a.PrimaryTopic)
            .Include(s => s.Article).ThenInclude(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
}

public class PageMarkRepository : GenericRepository<PageMark>, IPageMarkRepository
{
    public PageMarkRepository(AppDbContext context) : base(context) { }

    public async Task<List<PageMark>> GetUserArticleMarksAsync(Guid userId, Guid articleId, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(pm => pm.UserId == userId && pm.ArticleId == articleId)
            .OrderBy(pm => pm.OccurrenceIndex)
            .ToListAsync(ct);
    }

    public async Task<List<Article>> GetUserAnnotatedArticlesAsync(Guid userId, CancellationToken ct = default)
    {
        var articleIds = await _dbSet
            .Where(pm => pm.UserId == userId)
            .Select(pm => pm.ArticleId)
            .Distinct()
            .ToListAsync(ct);

        return await _context.Articles
            .AsNoTracking()
            .Include(a => a.Section)
            .Include(a => a.PrimaryTopic)
            .Include(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Where(a => articleIds.Contains(a.Id))
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(ct);
    }

    public async Task ReplaceArticleMarksAsync(Guid userId, Guid articleId, List<PageMark> newMarks, CancellationToken ct = default)
    {
        var existing = await _dbSet.Where(pm => pm.UserId == userId && pm.ArticleId == articleId).ToListAsync(ct);
        foreach (var item in existing)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }
        if (newMarks.Count > 0)
        {
            await _dbSet.AddRangeAsync(newMarks, ct);
        }
    }
}

public class SeoRepository : GenericRepository<ArticleSeoProfile>, ISeoRepository
{
    public SeoRepository(AppDbContext context) : base(context) { }

    public async Task<ArticleSeoProfile?> GetByArticleIdAsync(Guid articleId, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(s => s.RelatedKeyphrases)
            .Include(s => s.Snapshots.OrderByDescending(sn => sn.CreatedAt).Take(10))
            .FirstOrDefaultAsync(s => s.ArticleId == articleId, ct);
    }

    public async Task<List<ArticleSeoProfile>> GetArticlesUsingKeyphraseAsync(string normalizedKeyphrase, Guid? excludeArticleId = null, CancellationToken ct = default)
    {
        var lower = normalizedKeyphrase.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(lower)) return new List<ArticleSeoProfile>();

        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Article)
            .Where(s => s.FocusKeyphrase.ToLower() == lower &&
                        (!excludeArticleId.HasValue || s.ArticleId != excludeArticleId.Value) &&
                        !s.Article.IsDeleted)
            .ToListAsync(ct);
    }
}

