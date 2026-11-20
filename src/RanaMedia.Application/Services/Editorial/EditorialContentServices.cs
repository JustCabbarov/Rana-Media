using System.Security.Cryptography;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RanaMedia.Application.Caching;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Services.Seo;
using RanaMedia.Contract.DTOs.Common;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Contract.Services.Seo;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.Editorial;

public class HomeService : IHomeService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IInMemoryCacheService _cache;
    private readonly IMapper _mapper;

    public HomeService(IArticleRepository articleRepository, IInMemoryCacheService cache, IMapper mapper)
    {
        _articleRepository = articleRepository;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<HomePageDTO> GetHomePageAsync(CancellationToken ct = default)
    {
        return (await _cache.GetOrCreateAsync("home_page", async () =>
        {
            var featured = await _articleRepository.GetFeaturedHomepageArticleAsync(ct);
            var popular = await _articleRepository.GetPopularArticlesAsync(4, null, null, ct);
            var (latest, hasNext, cursor) = await _articleRepository.GetPagedAsync(12, null, ct: ct);

            return new HomePageDTO
            {
                FeaturedEssay = featured != null ? _mapper.Map<ArticleDetailDTO>(featured) : null,
                PopularThisMonth = _mapper.Map<List<ArticleCardDTO>>(popular),
                Articles = new CursorPagedResult<ArticleCardDTO>
                {
                    Nodes = _mapper.Map<List<ArticleCardDTO>>(latest),
                    HasNextPage = hasNext,
                    EndCursor = cursor
                }
            };
        }, TimeSpan.FromMinutes(3)))!;
    }

    public async Task<CursorPagedResult<ArticleCardDTO>> GetMoreHomeArticlesAsync(string? afterCursor, int first = 30, CancellationToken ct = default)
    {
        var (items, hasNext, cursor) = await _articleRepository.GetPagedAsync(first, afterCursor, ct: ct);
        return new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(items),
            HasNextPage = hasNext,
            EndCursor = cursor
        };
    }
}

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IGenericRepository<Topic> _topicRepository;
    private readonly IGenericRepository<Tag> _tagRepository;
    private readonly IGenericRepository<Contributor> _contributorRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly ISeoAnalysisService _seoService;
    private readonly IInMemoryCacheService _cache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ArticleService(
        IArticleRepository articleRepository,
        IGenericRepository<Topic> topicRepository,
        IGenericRepository<Tag> tagRepository,
        IGenericRepository<Contributor> contributorRepository,
        IGenericRepository<Section> sectionRepository,
        ISeoAnalysisService seoService,
        IInMemoryCacheService cache,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _articleRepository = articleRepository;
        _topicRepository = topicRepository;
        _tagRepository = tagRepository;
        _contributorRepository = contributorRepository;
        _sectionRepository = sectionRepository;
        _seoService = seoService;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CursorPagedResult<ArticleCardDTO>> GetArticlesAsync(
        ArticleType? type = null,
        string sort = "latest",
        string? tag = null,
        string? afterCursor = null,
        int first = 12,
        CancellationToken ct = default)
    {
        bool popular = sort.Equals("popular", StringComparison.OrdinalIgnoreCase);
        var (items, hasNext, cursor) = await _articleRepository.GetPagedAsync(first, afterCursor, type, null, null, tag, popular, null, ct);

        return new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(items),
            HasNextPage = hasNext,
            EndCursor = cursor
        };
    }

    public async Task<ArticleDetailDTO> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, includeUnpublished: false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        return _mapper.Map<ArticleDetailDTO>(article);
    }

    public async Task<ArticleDetailDTO> GetPreviewAsync(string slug, string token, CancellationToken ct = default)
    {
        var article = await _articleRepository.GetBySlugDetailedAsync(slug, includeUnpublished: true, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {slug}");

        if (article.Status != ArticleStatus.Published &&
            (string.IsNullOrWhiteSpace(token) || article.PreviewToken != token))
        {
            throw new ForbiddenException("Preview token yanlışdır və ya icazəsizdir.", "INVALID_PREVIEW_TOKEN");
        }

        return _mapper.Map<ArticleDetailDTO>(article);
    }

    public async Task<PaginatedResult<AdminArticleListItemDTO>> GetAdminArticlesAsync(
        ArticleStatus? status = null,
        ArticleType? type = null,
        string? search = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _articleRepository.Query()
            .AsNoTracking()
            .Include(a => a.Section)
            .Include(a => a.PrimaryTopic)
            .Include(a => a.ArticleContributors).ThenInclude(ac => ac.Contributor)
            .Include(a => a.Comments).ThenInclude(c => c.Replies)
            .Include(a => a.SeoProfile)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(a =>
                a.Title.ToLower().Contains(term) ||
                a.Slug.ToLower().Contains(term) ||
                a.StandfirstShort.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(ct);
        var entities = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = entities.Select(a => new AdminArticleListItemDTO
        {
            Id = a.Id,
            Slug = a.Slug,
            Title = a.Title,
            Type = a.Type,
            Status = a.Status,
            SectionTitle = a.Section?.Title ?? string.Empty,
            SectionSlug = a.Section?.Slug ?? string.Empty,
            SectionColor = a.Section?.AccentColorHex ?? "#9d120d",
            PrimaryTopicTitle = a.PrimaryTopic?.Title,
            Authors = a.ArticleContributors
                .Where(ac => ac.Role == ContributorRole.Author)
                .OrderBy(ac => ac.DisplayOrder)
                .Select(ac => ac.Contributor.Name)
                .ToList(),
            WordCount = a.WordCount,
            ViewsCount = a.ViewsCount,
            CommentAndRepliesCount = a.Comments.Count(c => !c.IsModerated) + a.Comments.Where(c => !c.IsModerated).Sum(c => c.Replies.Count(r => !r.IsModerated)),
            IsHomepageFeatured = a.IsHomepageFeatured,
            CommentsEnabled = a.CommentsEnabled,
            CommentsOpen = a.CommentsOpen,
            PublishedAt = a.PublishedAt,
            ScheduledAt = a.ScheduledAt,
            CreatedAt = a.CreatedAt,
            SeoScore = a.SeoProfile?.OverallScore,
            SeoStatus = a.SeoProfile?.OverallStatus,
            FocusKeyphrase = a.SeoProfile?.FocusKeyphrase
        }).ToList();

        return new PaginatedResult<AdminArticleListItemDTO>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<AdminArticleEditDTO> GetAdminArticleByIdAsync(Guid id, CancellationToken ct = default)
    {
        var article = await _articleRepository.Query()
            .AsNoTracking()
            .Include(a => a.ArticleTopics)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.ArticleContributors)
            .Include(a => a.RelatedFrom)
            .Include(a => a.SeoProfile).ThenInclude(p => p!.RelatedKeyphrases)
            .FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {id}");

        DisplaySettingsDTO settings;
        try
        {
            settings = string.IsNullOrWhiteSpace(article.DisplaySettingsJson)
                ? new DisplaySettingsDTO()
                : JsonSerializer.Deserialize<DisplaySettingsDTO>(article.DisplaySettingsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new DisplaySettingsDTO();
        }
        catch
        {
            settings = new DisplaySettingsDTO();
        }

        List<string> synonyms = new();
        if (article.SeoProfile != null && !string.IsNullOrWhiteSpace(article.SeoProfile.SynonymsJson))
        {
            try { synonyms = JsonSerializer.Deserialize<List<string>>(article.SeoProfile.SynonymsJson) ?? new List<string>(); }
            catch { synonyms = new List<string>(); }
        }

        var relatedKeyphrases = new List<RelatedKeyphraseInputDTO>();
        if (article.SeoProfile?.RelatedKeyphrases != null)
        {
            foreach (var rk in article.SeoProfile.RelatedKeyphrases)
            {
                List<string> rkSyns = new();
                try { rkSyns = JsonSerializer.Deserialize<List<string>>(rk.SynonymsJson) ?? new List<string>(); }
                catch { rkSyns = new List<string>(); }
                relatedKeyphrases.Add(new RelatedKeyphraseInputDTO { Keyphrase = rk.Keyphrase, Synonyms = rkSyns });
            }
        }

        var seoAnalysis = await _seoService.GetArticleSeoProfileAsync(id, ct);

        return new AdminArticleEditDTO
        {
            Id = article.Id,
            PreviewToken = article.PreviewToken ?? string.Empty,
            ViewsCount = article.ViewsCount,
            WordCount = article.WordCount,
            ReadingTimeMinutes = article.ReadingTimeMinutes,
            PublishedAt = article.PublishedAt,
            SeoAnalysis = seoAnalysis,
            Title = article.Title,
            Slug = article.Slug,
            Type = article.Type,
            Status = article.Status,
            StandfirstShort = article.StandfirstShort,
            StandfirstLong = article.StandfirstLong,
            BodyHtml = article.BodyHtml,
            MetaTitle = article.MetaTitle,
            MetaDescription = article.MetaDescription,
            SectionId = article.SectionId,
            PrimaryTopicId = article.PrimaryTopicId,
            TopicIds = article.ArticleTopics.Select(at => at.TopicId).ToList(),
            TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList(),
            AuthorIds = article.ArticleContributors.Where(ac => ac.Role == ContributorRole.Author).OrderBy(ac => ac.DisplayOrder).Select(ac => ac.ContributorId).ToList(),
            EditorId = article.ArticleContributors.Where(ac => ac.Role == ContributorRole.Editor).Select(ac => (Guid?)ac.ContributorId).FirstOrDefault(),
            RelatedArticleIds = article.RelatedFrom.OrderBy(r => r.DisplayOrder).Select(r => r.RelatedArticleId).ToList(),
            IsHomepageFeatured = article.IsHomepageFeatured,
            FeaturedImageUrl = article.FeaturedImageUrl,
            FeaturedImageAlt = article.FeaturedImageAlt,
            FeaturedImageCaptionHtml = article.FeaturedImageCaptionHtml,
            CardImageUrl = article.CardImageUrl,
            CardImageAlt = article.CardImageAlt,
            CardImageWidth = article.CardImageWidth,
            CardImageHeight = article.CardImageHeight,
            DynamicImageUrl = article.DynamicImageUrl,
            DynamicImageCardEnabled = article.DynamicImageCardEnabled,
            DynamicImageArticleEnabled = article.DynamicImageArticleEnabled,
            Settings = settings,
            AudioEnabled = article.AudioEnabled,
            AudioUrl = article.AudioUrl,
            AudioDurationSeconds = article.AudioDurationSeconds,
            VideoHoster = article.VideoHoster,
            VideoEmbedUrl = article.VideoEmbedUrl,
            VideoDurationMinutes = article.VideoDurationMinutes,
            CreditsShort = article.CreditsShort,
            CreditsHtml = article.CreditsHtml,
            CreativeCommons = article.CreativeCommons,
            LicenseName = article.LicenseName,
            LicenseShort = article.LicenseShort,
            RepublicationAllowed = article.RepublicationAllowed,
            CommercialUseAllowed = article.CommercialUseAllowed,
            CommentsEnabled = article.CommentsEnabled,
            CommentsOpen = article.CommentsOpen,
            NewsletterHidden = article.NewsletterHidden,
            ScheduledAt = article.ScheduledAt,
            FocusKeyphrase = article.SeoProfile?.FocusKeyphrase,
            Synonyms = synonyms,
            RelatedKeyphrases = relatedKeyphrases
        };
    }

    public async Task<AdminArticleResponseDTO> CreateAsync(CreateArticleDTO request, CancellationToken ct = default)
    {
        if (request == null) throw new NotNullExceptions();
        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await _articleRepository.SlugExistsAsync(slug, null, ct))
            throw new ConflictException($"Bu slug artıq mövcuddur: {slug}", "DUPLICATE_SLUG");

        _ = await _sectionRepository.GetByIdAsync(request.SectionId)
            ?? throw new NotFoundException($"Bölmə tapılmadı: {request.SectionId}");

        var article = _mapper.Map<Article>(request);
        article.Slug = slug;
        article.PreviewToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        PopulatePlainTextAndMetrics(article);

        if (article.Status == ArticleStatus.Published && !article.PublishedAt.HasValue)
            article.PublishedAt = DateTime.UtcNow;

        await SyncTopicsTagsContributorsAsync(article, request, ct);

        await _articleRepository.AddAsync(article);
        await _unitOfWork.SaveChangesAsync(ct);

        var seoResult = await RunSeoIfConfiguredAsync(article, request, ct);
        _cache.Remove("home_page");

        var detailed = await _articleRepository.GetBySlugDetailedAsync(article.Slug, true, ct) ?? article;
        return new AdminArticleResponseDTO
        {
            Article = _mapper.Map<ArticleDetailDTO>(detailed),
            SeoAnalysis = seoResult
        };
    }

    public async Task<AdminArticleResponseDTO> UpdateAsync(Guid id, UpdateArticleDTO request, CancellationToken ct = default)
    {
        if (request == null) throw new NotNullExceptions();
        var article = await _articleRepository.Query()
            .Include(a => a.ArticleTopics)
            .Include(a => a.ArticleTags)
            .Include(a => a.ArticleContributors)
            .Include(a => a.RelatedFrom)
            .FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {id}");

        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await _articleRepository.SlugExistsAsync(slug, id, ct))
            throw new ConflictException($"Bu slug artıq mövcuddur: {slug}", "DUPLICATE_SLUG");

        article.Title = request.Title.Trim();
        article.Slug = slug;
        article.Type = request.Type;
        article.Status = request.Status;
        article.StandfirstShort = request.StandfirstShort;
        article.StandfirstLong = request.StandfirstLong;
        article.BodyHtml = request.BodyHtml;
        article.MetaTitle = request.MetaTitle;
        article.MetaDescription = request.MetaDescription;
        article.SectionId = request.SectionId;
        article.PrimaryTopicId = request.PrimaryTopicId;
        article.IsHomepageFeatured = request.IsHomepageFeatured;
        article.FeaturedImageUrl = request.FeaturedImageUrl;
        article.FeaturedImageAlt = request.FeaturedImageAlt;
        article.FeaturedImageCaptionHtml = request.FeaturedImageCaptionHtml;
        article.CardImageUrl = request.CardImageUrl;
        article.CardImageAlt = request.CardImageAlt;
        article.CardImageWidth = request.CardImageWidth;
        article.CardImageHeight = request.CardImageHeight;
        article.DynamicImageUrl = request.DynamicImageUrl;
        article.DynamicImageCardEnabled = request.DynamicImageCardEnabled;
        article.DynamicImageArticleEnabled = request.DynamicImageArticleEnabled;
        article.DisplaySettingsJson = JsonSerializer.Serialize(request.Settings);
        article.AudioEnabled = request.AudioEnabled;
        article.AudioUrl = request.AudioUrl;
        article.AudioDurationSeconds = request.AudioDurationSeconds;
        article.VideoHoster = request.VideoHoster;
        article.VideoEmbedUrl = request.VideoEmbedUrl;
        article.VideoDurationMinutes = request.VideoDurationMinutes;
        article.CreditsShort = request.CreditsShort;
        article.CreditsHtml = request.CreditsHtml;
        article.CreativeCommons = request.CreativeCommons;
        article.LicenseName = request.LicenseName;
        article.LicenseShort = request.LicenseShort;
        article.RepublicationAllowed = request.RepublicationAllowed;
        article.CommercialUseAllowed = request.CommercialUseAllowed;
        article.CommentsEnabled = request.CommentsEnabled;
        article.CommentsOpen = request.CommentsOpen;
        article.NewsletterHidden = request.NewsletterHidden;
        article.ScheduledAt = request.ScheduledAt;

        if (article.Status == ArticleStatus.Published && !article.PublishedAt.HasValue)
            article.PublishedAt = DateTime.UtcNow;

        PopulatePlainTextAndMetrics(article);

        article.ArticleTopics.Clear();
        article.ArticleTags.Clear();
        article.ArticleContributors.Clear();
        article.RelatedFrom.Clear();
        await SyncTopicsTagsContributorsAsync(article, request, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        var seoResult = await RunSeoIfConfiguredAsync(article, request, ct);
        _cache.Remove("home_page");

        var detailed = await _articleRepository.GetBySlugDetailedAsync(article.Slug, true, ct) ?? article;
        return new AdminArticleResponseDTO
        {
            Article = _mapper.Map<ArticleDetailDTO>(detailed),
            SeoAnalysis = seoResult
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _articleRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync(ct);
        _cache.Remove("home_page");
    }

    private async Task SyncTopicsTagsContributorsAsync(Article article, CreateArticleDTO request, CancellationToken ct)
    {
        var topicIds = new HashSet<Guid>(request.TopicIds);
        if (request.PrimaryTopicId.HasValue) topicIds.Add(request.PrimaryTopicId.Value);

        foreach (var tid in topicIds)
        {
            var topic = await _topicRepository.GetByIdAsync(tid);
            if (topic != null)
                article.ArticleTopics.Add(new ArticleTopic { TopicId = tid });
        }

        foreach (var tagName in request.TagNames.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var cleanName = tagName.Trim();
            var tagSlug = cleanName.ToLowerInvariant().Replace(' ', '-');
            var tag = await _tagRepository.Query().FirstOrDefaultAsync(t => t.Slug == tagSlug, ct);
            if (tag == null)
            {
                tag = new Tag { Name = cleanName, Slug = tagSlug };
                await _tagRepository.AddAsync(tag);
            }
            article.ArticleTags.Add(new ArticleTag { Tag = tag });
        }

        int order = 1;
        foreach (var authorId in request.AuthorIds.Distinct())
        {
            var contrib = await _contributorRepository.GetByIdAsync(authorId);
            if (contrib != null)
                article.ArticleContributors.Add(new ArticleContributor { ContributorId = authorId, Role = ContributorRole.Author, DisplayOrder = order++ });
        }

        if (request.EditorId.HasValue)
        {
            var ed = await _contributorRepository.GetByIdAsync(request.EditorId.Value);
            if (ed != null)
                article.ArticleContributors.Add(new ArticleContributor { ContributorId = ed.Id, Role = ContributorRole.Editor, DisplayOrder = 1 });
        }

        int relOrder = 1;
        foreach (var relId in request.RelatedArticleIds.Where(r => r != article.Id).Distinct())
        {
            article.RelatedFrom.Add(new ArticleRelation { RelatedArticleId = relId, DisplayOrder = relOrder++ });
        }
    }

    private async Task<SeoAnalysisResponseDTO?> RunSeoIfConfiguredAsync(Article article, CreateArticleDTO request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.FocusKeyphrase)) return null;

        return await _seoService.AnalyzeAndPersistForArticleAsync(article.Id, new AnalyzeSeoRequestDTO
        {
            ArticleId = article.Id,
            FocusKeyphrase = request.FocusKeyphrase,
            Synonyms = request.Synonyms ?? new List<string>(),
            RelatedKeyphrases = request.RelatedKeyphrases ?? new List<RelatedKeyphraseInputDTO>(),
            SeoTitle = request.MetaTitle ?? request.Title,
            Slug = article.Slug,
            MetaDescription = request.MetaDescription,
            BodyHtml = request.BodyHtml,
            FeaturedImageUrl = request.FeaturedImageUrl ?? request.CardImageUrl,
            FeaturedImageAlt = request.FeaturedImageAlt ?? request.CardImageAlt
        }, ct);
    }

    private static void PopulatePlainTextAndMetrics(Article article)
    {
        var plain = HtmlContentParser.StripTags(article.BodyHtml);
        var words = HtmlContentParser.TokenizeWords(plain);
        article.BodyPlainText = plain;
        article.WordCount = words.Count;
        article.ReadingTimeMinutes = Math.Max(1, (int)Math.Ceiling(words.Count / 200.0));
    }
}

public class VideoService : IVideoService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public VideoService(IArticleRepository articleRepository, IMapper mapper)
    {
        _articleRepository = articleRepository;
        _mapper = mapper;
    }

    public async Task<CursorPagedResult<ArticleCardDTO>> GetVideosAsync(string? filter = null, string? afterCursor = null, int first = 12, CancellationToken ct = default)
    {
        bool popular = string.Equals(filter, "popular", StringComparison.OrdinalIgnoreCase);
        string? categoryTag = filter?.ToLowerInvariant() is "originals" or "exclusives" ? filter.ToLowerInvariant() : null;

        var (items, hasNext, cursor) = await _articleRepository.GetPagedAsync(first, afterCursor, ArticleType.Video, null, null, null, popular, categoryTag, ct);
        return new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(items),
            HasNextPage = hasNext,
            EndCursor = cursor
        };
    }

    public async Task<VideoDetailDTO> GetVideoBySlugAsync(string slug, CancellationToken ct = default)
    {
        var video = await _articleRepository.GetBySlugDetailedAsync(slug, includeUnpublished: false, ct)
            ?? throw new NotFoundException($"Video tapılmadı: {slug}");

        if (video.Type != ArticleType.Video)
            throw new NotFoundException($"Video tapılmadı: {slug}");

        var dto = _mapper.Map<VideoDetailDTO>(video);
        var (recent, _, _) = await _articleRepository.GetPagedAsync(12, null, ArticleType.Video, ct: ct);
        dto.RecentVideos = _mapper.Map<List<ArticleCardDTO>>(recent);
        return dto;
    }
}

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public SectionService(ISectionRepository sectionRepository, IArticleRepository articleRepository, IMapper mapper)
    {
        _sectionRepository = sectionRepository;
        _articleRepository = articleRepository;
        _mapper = mapper;
    }

    public async Task<List<SectionSummaryDTO>> GetAllSectionsAsync(CancellationToken ct = default)
    {
        var sections = await _sectionRepository.GetAllWithTopicsAsync(ct);
        return _mapper.Map<List<SectionSummaryDTO>>(sections);
    }

    public async Task<List<SectionCatalogDTO>> GetCatalogAsync(CancellationToken ct = default)
    {
        var sections = await _sectionRepository.GetAllWithTopicsAsync(ct);
        return sections.Select(s => new SectionCatalogDTO
        {
            Id = s.Id,
            Slug = s.Slug,
            Title = s.Title,
            AccentColorHex = s.AccentColorHex,
            Description = s.Description,
            Topics = s.Topics
                .OrderBy(t => t.Title)
                .Select(t => new TopicOptionDTO
                {
                    Id = t.Id,
                    Slug = t.Slug,
                    Title = t.Title
                })
                .ToList()
        }).ToList();
    }

    public async Task<SectionDetailDTO> GetSectionBySlugAsync(string slug, string sort = "latest", string? afterCursor = null, int first = 24, CancellationToken ct = default)
    {
        var section = await _sectionRepository.GetBySlugWithTopicsAsync(slug.ToLowerInvariant(), ct)
            ?? throw new NotFoundException($"Bölmə tapılmadı: {slug}");

        bool popular = sort.Equals("popular", StringComparison.OrdinalIgnoreCase);
        var (articles, hasNext, cursor) = await _articleRepository.GetPagedAsync(first, afterCursor, null, section.Slug, null, null, popular, null, ct);

        var dto = _mapper.Map<SectionDetailDTO>(section);
        dto.Articles = new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(articles),
            HasNextPage = hasNext,
            EndCursor = cursor
        };
        return dto;
    }
}

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public TopicService(ITopicRepository topicRepository, IArticleRepository articleRepository, IMapper mapper)
    {
        _topicRepository = topicRepository;
        _articleRepository = articleRepository;
        _mapper = mapper;
    }

    public async Task<TopicDetailDTO> GetTopicBySlugAsync(string sectionSlug, string topicSlug, string sort = "latest", string? afterCursor = null, int first = 24, CancellationToken ct = default)
    {
        var topic = await _topicRepository.GetBySlugWithSectionAsync(topicSlug.ToLowerInvariant(), sectionSlug.ToLowerInvariant(), ct)
            ?? throw new NotFoundException($"Mövzu tapılmadı: {sectionSlug}/{topicSlug}");

        bool popular = sort.Equals("popular", StringComparison.OrdinalIgnoreCase);
        var (articles, hasNext, cursor) = await _articleRepository.GetPagedAsync(first, afterCursor, null, topic.Section.Slug, topic.Slug, null, popular, null, ct);

        var dto = _mapper.Map<TopicDetailDTO>(topic);
        dto.SiblingTopics = _mapper.Map<List<TopicSummaryDTO>>(topic.Section.Topics.Where(t => t.Id != topic.Id).ToList());
        dto.Articles = new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(articles),
            HasNextPage = hasNext,
            EndCursor = cursor
        };
        return dto;
    }
}

public class ContributorService : IContributorService
{
    private readonly IContributorRepository _contributorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContributorService(IContributorRepository contributorRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _contributorRepository = contributorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContributorProfileDTO> GetContributorBySlugAsync(string slug, string? authoredCursor = null, string? editedCursor = null, CancellationToken ct = default)
    {
        var contributor = await _contributorRepository.GetBySlugAsync(slug.ToLowerInvariant(), ct)
            ?? throw new NotFoundException($"Müəllif tapılmadı: {slug}");

        var (authored, aNext, aEnd) = await _contributorRepository.GetContributorArticlesAsync(contributor.Id, ContributorRole.Author, 12, authoredCursor, ct);
        var (edited, eNext, eEnd) = await _contributorRepository.GetContributorArticlesAsync(contributor.Id, ContributorRole.Editor, 12, editedCursor, ct);

        var dto = _mapper.Map<ContributorProfileDTO>(contributor);
        dto.AuthoredArticles = new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(authored),
            HasNextPage = aNext,
            EndCursor = aEnd
        };
        dto.EditedArticles = new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(edited),
            HasNextPage = eNext,
            EndCursor = eEnd
        };
        return dto;
    }

    public async Task<ContributorProfileDTO> CreateContributorAsync(CreateContributorDTO request, CancellationToken ct = default)
    {
        if (request == null) throw new NotNullExceptions();
        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await _contributorRepository.GetBySlugAsync(slug, ct) != null)
            throw new ConflictException($"Bu müəllif slug-ı artıq mövcuddur: {slug}", "DUPLICATE_CONTRIBUTOR_SLUG");

        var entity = _mapper.Map<Contributor>(request);
        entity.Slug = slug;
        await _contributorRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<ContributorProfileDTO>(entity);
    }

    public async Task<List<ContributorShortDTO>> GetAllContributorsAsync(CancellationToken ct = default)
    {
        var items = await _contributorRepository.GetAllAsync();
        return _mapper.Map<List<ContributorShortDTO>>(items.OrderBy(c => c.Name).ToList());
    }

    public async Task DeleteContributorAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _contributorRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Müəllif tapılmadı: {id}");
        await _contributorRepository.DeleteAsync(existing.Id);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class SearchService : ISearchService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public SearchService(IArticleRepository articleRepository, IMapper mapper)
    {
        _articleRepository = articleRepository;
        _mapper = mapper;
    }

    public async Task<CursorPagedResult<ArticleCardDTO>> SearchAsync(string query, ArticleType? type = null, string? afterCursor = null, int first = 30, CancellationToken ct = default)
    {
        var (items, total, hasNext, cursor) = await _articleRepository.SearchAsync(query, type, first, afterCursor, ct);
        return new CursorPagedResult<ArticleCardDTO>
        {
            Nodes = _mapper.Map<List<ArticleCardDTO>>(items),
            TotalCount = total,
            HasNextPage = hasNext,
            EndCursor = cursor
        };
    }
}

