using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Options;
using RanaMedia.Contract.DTOs.Common;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.Editorial;

public class LibraryService : ILibraryService
{
    private readonly IUserSaveRepository _saveRepository;
    private readonly IPageMarkRepository _pageMarkRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LibraryService(
        IUserSaveRepository saveRepository,
        IPageMarkRepository pageMarkRepository,
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _saveRepository = saveRepository;
        _pageMarkRepository = pageMarkRepository;
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LibrarySummaryDTO> GetUserLibraryAsync(Guid userId, CancellationToken ct = default)
    {
        var saves = await _saveRepository.GetUserLibraryAsync(userId, ct);
        int unseen = saves.Count(s => !s.IsSeen);

        foreach (var s in saves.Where(x => !x.IsSeen))
        {
            s.IsSeen = true;
        }
        if (unseen > 0) await _unitOfWork.SaveChangesAsync(ct);

        return new LibrarySummaryDTO
        {
            UnseenCount = 0,
            Items = saves.Select(s => new SavedArticleDTO
            {
                SaveId = s.Id,
                IsSeen = s.IsSeen,
                SavedAt = s.CreatedAt,
                Article = _mapper.Map<ArticleCardDTO>(s.Article)
            }).ToList()
        };
    }

    public async Task<List<ArticleCardDTO>> GetUserAnnotatedArticlesAsync(Guid userId, CancellationToken ct = default)
    {
        var articles = await _pageMarkRepository.GetUserAnnotatedArticlesAsync(userId, ct);
        return _mapper.Map<List<ArticleCardDTO>>(articles);
    }

    public async Task<List<Guid>> GetSavedArticleIdsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _saveRepository.GetSavedArticleIdsAsync(userId, ct);
    }

    public async Task SaveArticleAsync(Guid userId, string slug, CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        var existing = await _saveRepository.GetByUserAndArticleAsync(userId, article.Id, ct);
        if (existing != null) return;

        await _saveRepository.AddAsync(new UserSave
        {
            UserId = userId,
            ArticleId = article.Id,
            IsSeen = false
        });
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveSavedArticleAsync(Guid userId, string slug, CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, true, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        var existing = await _saveRepository.GetByUserAndArticleAsync(userId, article.Id, ct);
        if (existing == null) return;

        await _saveRepository.DeleteAsync(existing.Id);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class PageMarkService : IPageMarkService
{
    private readonly IPageMarkRepository _pageMarkRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PageMarkService(
        IPageMarkRepository pageMarkRepository,
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _pageMarkRepository = pageMarkRepository;
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PageMarkDTO>> GetUserArticleMarksAsync(Guid userId, string slug, CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        var marks = await _pageMarkRepository.GetUserArticleMarksAsync(userId, article.Id, ct);
        return _mapper.Map<List<PageMarkDTO>>(marks);
    }

    public async Task<List<PageMarkDTO>> ReplaceArticleMarksAsync(Guid userId, string slug, UpdatePageMarksRequestDTO request, CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        var newMarks = (request.PageMarks ?? new List<PageMarkDTO>())
            .Select((dto, idx) =>
            {
                var entity = _mapper.Map<PageMark>(dto);
                entity.Id = Guid.NewGuid();
                entity.UserId = userId;
                entity.ArticleId = article.Id;
                if (entity.Type == Domain.Enums.PageMarkType.Annotation && !entity.AnnotationNumber.HasValue)
                    entity.AnnotationNumber = idx + 1;
                return entity;
            })
            .ToList();

        await _pageMarkRepository.ReplaceArticleMarksAsync(userId, article.Id, newMarks, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<List<PageMarkDTO>>(newMarks);
    }
}

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CommentSettings _settings;
    private readonly IMapper _mapper;

    public CommentService(
        ICommentRepository commentRepository,
        IArticleRepository articleRepository,
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork,
        IOptions<CommentSettings> settings,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _articleRepository = articleRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
        _mapper = mapper;
    }

    public async Task<CursorPagedResult<CommentDTO>> GetArticleCommentsAsync(
        string slug,
        Guid? currentUserId,
        string sort = "newest",
        string? afterCursor = null,
        int first = 20,
        CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        var authorUserIds = article.ArticleContributors
            .Where(ac => ac.Contributor.AppUserId.HasValue)
            .Select(ac => ac.Contributor.AppUserId!.Value)
            .ToHashSet();

        var (items, hasNext, cursor) = await _commentRepository.GetArticleCommentsAsync(article.Id, first, afterCursor, sort, ct);
        var dtos = _mapper.Map<List<CommentDTO>>(items);
        foreach (var dto in dtos)
        {
            dto.User.IsArticleAuthor = authorUserIds.Contains(dto.User.Id);
            foreach (var r in dto.Replies)
                r.User.IsArticleAuthor = authorUserIds.Contains(r.User.Id);
        }

        return new CursorPagedResult<CommentDTO>
        {
            Nodes = dtos,
            HasNextPage = hasNext,
            EndCursor = cursor
        };
    }

    public async Task<CommentDTO> CreateCommentAsync(Guid userId, string slug, CreateCommentDTO request, CancellationToken ct = default)
    {
        var user = await EnsureCanParticipateAsync(userId);
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        if (!article.CommentsEnabled || !article.CommentsOpen)
            throw new ForbiddenException("Bu məqalə üçün şərhlər aktiv deyil.", "COMMENTS_CLOSED");

        if (_settings.MaxRootCommentsPerArticlePerUser == 1 &&
            await _commentRepository.UserHasRootCommentAsync(article.Id, userId, ct))
        {
            throw new ConflictException("Hər məqaləyə yalnız 1 əsas şərh yaza bilərsiniz.", "ONE_COMMENT_PER_ARTICLE");
        }

        var now = DateTime.UtcNow;
        var comment = new Comment
        {
            ArticleId = article.Id,
            UserId = userId,
            BodyHtml = request.Body.Trim(),
            CreatedAt = now,
            EditableUntil = now.AddMinutes(_settings.EditWindowMinutes)
        };

        await _commentRepository.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync(ct);

        comment.User = user;
        return _mapper.Map<CommentDTO>(comment);
    }

    public async Task<CommentDTO> UpdateCommentAsync(Guid userId, Guid commentId, UpdateCommentDTO request, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdWithDetailsAsync(commentId, ct)
            ?? throw new NotFoundException("Şərh tapılmadı.");

        if (comment.UserId != userId)
            throw new ForbiddenException("Yalnız öz şərhinizi redaktə edə bilərsiniz.", "NOT_COMMENT_OWNER");

        if (DateTime.UtcNow > comment.EditableUntil)
            throw new ForbiddenException("Şərhi yalnız yazıldıqdan sonra 1 saat ərzində redaktə etmək mümkündür.", "COMMENT_EDIT_WINDOW_EXPIRED");

        comment.BodyHtml = request.Body.Trim();
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CommentDTO>(comment);
    }

    public async Task DeleteCommentAsync(Guid userId, bool isModerator, Guid commentId, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Şərh tapılmadı.");

        if (!isModerator && comment.UserId != userId)
            throw new ForbiddenException("Bu şərhi silmək icazəniz yoxdur.", "NOT_COMMENT_OWNER");

        await _commentRepository.DeleteAsync(commentId);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<CommentReplyDTO> CreateReplyAsync(Guid userId, Guid commentId, CreateReplyDTO request, CancellationToken ct = default)
    {
        var user = await EnsureCanParticipateAsync(userId);
        var comment = await _commentRepository.GetByIdWithDetailsAsync(commentId, ct)
            ?? throw new NotFoundException("Şərh tapılmadı.");

        if (comment.Article == null || !comment.Article.CommentsEnabled || !comment.Article.CommentsOpen)
            throw new ForbiddenException("Bu məqalə üçün şərhlər aktiv deyil.", "COMMENTS_CLOSED");

        var now = DateTime.UtcNow;
        var reply = new CommentReply
        {
            CommentId = commentId,
            UserId = userId,
            BodyHtml = request.Body.Trim(),
            CreatedAt = now,
            EditableUntil = now.AddMinutes(_settings.EditWindowMinutes)
        };

        await _commentRepository.AddReplyAsync(reply, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        reply.User = user;
        return _mapper.Map<CommentReplyDTO>(reply);
    }

    public async Task<CommentReplyDTO> UpdateReplyAsync(Guid userId, Guid replyId, UpdateCommentDTO request, CancellationToken ct = default)
    {
        var reply = await _commentRepository.GetReplyByIdAsync(replyId, ct)
            ?? throw new NotFoundException("Cavab tapılmadı.");

        if (reply.UserId != userId)
            throw new ForbiddenException("Yalnız öz cavabınızı redaktə edə bilərsiniz.", "NOT_REPLY_OWNER");

        if (DateTime.UtcNow > reply.EditableUntil)
            throw new ForbiddenException("Cavabı yalnız yazıldıqdan sonra 1 saat ərzində redaktə etmək mümkündür.", "COMMENT_EDIT_WINDOW_EXPIRED");

        reply.BodyHtml = request.Body.Trim();
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CommentReplyDTO>(reply);
    }

    public async Task DeleteReplyAsync(Guid userId, bool isModerator, Guid replyId, CancellationToken ct = default)
    {
        var reply = await _commentRepository.GetReplyByIdAsync(replyId, ct)
            ?? throw new NotFoundException("Cavab tapılmadı.");

        if (!isModerator && reply.UserId != userId)
            throw new ForbiddenException("Bu cavabı silmək icazəniz yoxdur.", "NOT_REPLY_OWNER");

        reply.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ToggleLikeAsync(Guid userId, Guid? commentId, Guid? replyId, bool like, CancellationToken ct = default)
    {
        if (!commentId.HasValue && !replyId.HasValue)
            throw new NotNullExceptions("CommentId və ya ReplyId tələb olunur.");

        var existing = await _commentRepository.GetLikeAsync(userId, commentId, replyId, ct);

        if (like && existing == null)
        {
            await _commentRepository.AddLikeAsync(new CommentLike { UserId = userId, CommentId = commentId, ReplyId = replyId }, ct);
            if (commentId.HasValue)
            {
                var c = await _commentRepository.GetByIdAsync(commentId.Value) ?? throw new NotFoundException("Şərh tapılmadı.");
                c.LikesCount++;
            }
            else if (replyId.HasValue)
            {
                var r = await _commentRepository.GetReplyByIdAsync(replyId.Value, ct) ?? throw new NotFoundException("Cavab tapılmadı.");
                r.LikesCount++;
            }
        }
        else if (!like && existing != null)
        {
            _commentRepository.RemoveLike(existing);
            if (commentId.HasValue)
            {
                var c = await _commentRepository.GetByIdAsync(commentId.Value);
                if (c != null && c.LikesCount > 0) c.LikesCount--;
            }
            else if (replyId.HasValue)
            {
                var r = await _commentRepository.GetReplyByIdAsync(replyId.Value, ct);
                if (r != null && r.LikesCount > 0) r.LikesCount--;
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ReportAsync(Guid userId, Guid? commentId, Guid? replyId, ReportCommentDTO request, CancellationToken ct = default)
    {
        if (!commentId.HasValue && !replyId.HasValue)
            throw new NotNullExceptions("CommentId və ya ReplyId tələb olunur.");

        await _commentRepository.AddReportAsync(new CommentReport
        {
            ReporterUserId = userId,
            CommentId = commentId,
            ReplyId = replyId,
            Reason = request.Reason.Trim()
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ModerateCommentAsync(Guid commentId, bool moderate, string? reason, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Şərh tapılmadı.");

        comment.IsModerated = moderate;
        comment.ModerationReason = moderate ? reason : null;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task FeatureCommentAsync(Guid commentId, bool feature, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Şərh tapılmadı.");

        comment.IsFeatured = feature;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<ArticleCardDTO>> GetConversationsAsync(int limit = 20, CancellationToken ct = default)
    {
        var articles = await _commentRepository.GetActiveConversationArticlesAsync(limit, ct);
        return _mapper.Map<List<ArticleCardDTO>>(articles);
    }

    public async Task<PaginatedResult<AdminCommentItemDTO>> GetAdminCommentsAsync(
        bool? onlyModerated = null,
        bool? onlyReported = null,
        int pageNumber = 1,
        int pageSize = 30,
        CancellationToken ct = default)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _commentRepository.Query()
            .AsNoTracking()
            .Include(c => c.Article)
            .Include(c => c.User)
            .Include(c => c.Replies)
            .Include(c => c.Reports)
            .AsQueryable();

        if (onlyModerated.HasValue)
            query = query.Where(c => c.IsModerated == onlyModerated.Value);

        if (onlyReported == true)
            query = query.Where(c => c.Reports.Any());

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.Reports.Count)
            .ThenByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<AdminCommentItemDTO>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = items.Select(c => new AdminCommentItemDTO
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                ArticleTitle = c.Article?.Title ?? string.Empty,
                ArticleSlug = c.Article?.Slug ?? string.Empty,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.FullName : "Reader",
                BodyHtml = c.BodyHtml,
                CreatedAt = c.CreatedAt,
                IsModerated = c.IsModerated,
                ModerationReason = c.ModerationReason,
                IsFeatured = c.IsFeatured,
                LikesCount = c.LikesCount,
                RepliesCount = c.Replies.Count,
                ReportsCount = c.Reports.Count
            }).ToList()
        };
    }

    private async Task<AppUser> EnsureCanParticipateAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedException("İstifadəçi tapılmadı.");

        if (_settings.RequireGuidelinesAcceptance && !user.CommunityGuidelinesAcceptedAt.HasValue)
        {
            throw new ForbiddenException(
                "Şərh yazmazdan əvvəl Community Guidelines şərtlərini qəbul etməlisiniz.",
                "COMMUNITY_GUIDELINES_NOT_ACCEPTED");
        }

        return user;
    }
}

