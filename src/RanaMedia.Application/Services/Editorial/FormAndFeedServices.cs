using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RanaMedia.Application.Exceptions;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.DTOs.Forms;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Contract.Services.System;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.Services.Editorial;

public class NewsletterService : INewsletterService
{
    private readonly IGenericRepository<NewsletterSubscription> _repo;
    private readonly ITurnstileVerifier _turnstile;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;

    public NewsletterService(
        IGenericRepository<NewsletterSubscription> repo,
        ITurnstileVerifier turnstile,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _turnstile = turnstile;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
    }

    public async Task SubscribeAsync(NewsletterSubscribeDTO request, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(request.Website))
            throw new BusinessRuleException("Bot fəaliyyəti aşkarlandı.", "HONEYPOT_TRIGGERED");

        if (!request.Daily && !request.Weekly)
            throw new BusinessRuleException("Ən azı bir variant seçin: Daily və ya Weekly.", "FREQUENCY_REQUIRED");

        if (!await _turnstile.VerifyAsync(request.TurnstileToken, null, ct))
            throw new ForbiddenException("Turnstile doğrulaması uğursuz oldu.", "TURNSTILE_FAILED");

        var email = request.Email.Trim().ToLowerInvariant();
        var existing = await _repo.Query().FirstOrDefaultAsync(n => n.Email == email, ct);

        if (existing == null)
        {
            existing = new NewsletterSubscription
            {
                Email = email,
                Daily = request.Daily,
                Weekly = request.Weekly,
                Source = request.Source,
                SignupId = request.SignupId,
                Status = NewsletterStatus.Pending,
                ConfirmToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)),
                UnsubscribeToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(24))
            };
            await _repo.AddAsync(existing);
        }
        else
        {
            existing.Daily = request.Daily;
            existing.Weekly = request.Weekly;
            existing.Source = request.Source;
            existing.Status = NewsletterStatus.Pending;
            existing.ConfirmToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(24));
        }

        await _unitOfWork.SaveChangesAsync(ct);

        await _emailSender.SendEmailAsync(
            email,
            "Rana Media Newsletter Təsdiqi",
            $"<p>Newsletter abunəliyinizi təsdiqləmək üçün token: <b>{existing.ConfirmToken}</b></p>");
    }

    public async Task ConfirmSubscriptionAsync(string token, CancellationToken ct = default)
    {
        var sub = await _repo.Query().FirstOrDefaultAsync(n => n.ConfirmToken == token, ct)
            ?? throw new NotFoundException("Təsdiq tokeni tapılmadı.");

        sub.Status = NewsletterStatus.Active;
        sub.ConfirmedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UnsubscribeAsync(string token, CancellationToken ct = default)
    {
        var sub = await _repo.Query().FirstOrDefaultAsync(n => n.UnsubscribeToken == token, ct)
            ?? throw new NotFoundException("Abunəlik tapılmadı.");

        sub.Status = NewsletterStatus.Unsubscribed;
        sub.UnsubscribedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class PitchService : IPitchService
{
    private readonly IGenericRepository<PitchSubmission> _repo;
    private readonly ITurnstileVerifier _turnstile;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PitchService(
        IGenericRepository<PitchSubmission> repo,
        ITurnstileVerifier turnstile,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repo = repo;
        _turnstile = turnstile;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PitchResponseDTO> SubmitPitchAsync(CreatePitchDTO request, CancellationToken ct = default)
    {
        if (!await _turnstile.VerifyAsync(request.TurnstileToken, null, ct))
            throw new ForbiddenException("Turnstile doğrulaması uğursuz oldu.", "TURNSTILE_FAILED");

        var entity = new PitchSubmission
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Bio = request.Bio.Trim(),
            Website = request.Website,
            PreviousContributor = request.PreviousContributor,
            Title = request.Title.Trim(),
            Field = request.Field.Trim(),
            Format = request.Format.Trim(),
            PitchText = request.Pitch.Trim(),
            Status = SubmissionStatus.New
        };

        await _repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<PitchResponseDTO>(entity);
    }

    public async Task<List<PitchResponseDTO>> GetAllPitchesAsync(SubmissionStatus? status = null, CancellationToken ct = default)
    {
        var query = _repo.Query().AsNoTracking();
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        var items = await query.OrderByDescending(p => p.CreatedAt).ToListAsync(ct);
        return _mapper.Map<List<PitchResponseDTO>>(items);
    }

    public async Task<PitchResponseDTO> UpdatePitchStatusAsync(Guid id, SubmissionStatus status, string? editorNotes, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id) ?? throw new NotFoundException("Müraciət tapılmadı.");
        entity.Status = status;
        entity.EditorNotes = editorNotes;
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<PitchResponseDTO>(entity);
    }
}

public class SyndicationService : ISyndicationService
{
    private readonly IGenericRepository<SyndicationRequest> _repo;
    private readonly IArticleRepository _articleRepository;
    private readonly ITurnstileVerifier _turnstile;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SyndicationService(
        IGenericRepository<SyndicationRequest> repo,
        IArticleRepository articleRepository,
        ITurnstileVerifier turnstile,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repo = repo;
        _articleRepository = articleRepository;
        _turnstile = turnstile;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SyndicationResponseDTO> SubmitSyndicationAsync(CreateSyndicationDTO request, CancellationToken ct = default)
    {
        if (!request.TermsAndConditions)
            throw new BusinessRuleException("Şərtlər qəbul edilməlidir.", "TERMS_REQUIRED");

        if (!await _turnstile.VerifyAsync(request.TurnstileToken, null, ct))
            throw new ForbiddenException("Turnstile doğrulaması uğursuz oldu.", "TURNSTILE_FAILED");

        var article = await _articleRepository.GetBySlugDetailedAsync(request.ArticleSlug, false, ct)
            ?? throw new NotFoundException($"Məqalə tapılmadı: {request.ArticleSlug}");

        if (!article.RepublicationAllowed)
            throw new ForbiddenException("Bu məqalənin təkrar nəşrinə icazə verilmir.", "REPUBLICATION_NOT_ALLOWED");

        var entity = new SyndicationRequest
        {
            ArticleId = article.Id,
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Company = request.Company.Trim(),
            Publication = request.Publication.Trim(),
            Address = request.Address.Trim(),
            PostCode = request.PostCode.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            Country = request.Country.Trim(),
            Language = request.Language.Trim(),
            MediumJson = JsonSerializer.Serialize(request.Medium),
            TermsAccepted = request.TermsAndConditions,
            Status = SubmissionStatus.New
        };

        await _repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        entity.Article = article;
        return _mapper.Map<SyndicationResponseDTO>(entity);
    }

    public async Task<List<SyndicationResponseDTO>> GetAllRequestsAsync(SubmissionStatus? status = null, CancellationToken ct = default)
    {
        var query = _repo.Query().Include(s => s.Article).AsNoTracking();
        if (status.HasValue) query = query.Where(s => s.Status == status.Value);
        var items = await query.OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
        return _mapper.Map<List<SyndicationResponseDTO>>(items);
    }

    public async Task<SyndicationResponseDTO> UpdateStatusAsync(Guid id, SubmissionStatus status, CancellationToken ct = default)
    {
        var entity = await _repo.Query().Include(s => s.Article).FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Sindikasiya müraciəti tapılmadı.");
        entity.Status = status;
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<SyndicationResponseDTO>(entity);
    }
}

public class ViewAnalyticsService : IViewAnalyticsService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IGenericRepository<ArticleViewEvent> _viewEventRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ViewAnalyticsService(
        IArticleRepository articleRepository,
        IGenericRepository<ArticleViewEvent> viewEventRepo,
        IUnitOfWork unitOfWork)
    {
        _articleRepository = articleRepository;
        _viewEventRepo = viewEventRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task TrackViewAsync(TrackViewDTO request, Guid? userId, string clientFingerprint, CancellationToken ct = default)
    {
        var article = await _articleRepository.Query().FirstOrDefaultAsync(a => a.Slug == request.Slug, ct);
        if (article == null) return;

        var cutoff = DateTime.UtcNow.AddMinutes(-15);
        var alreadyTracked = await _viewEventRepo.Query().AnyAsync(e =>
            e.ArticleId == article.Id &&
            e.SessionHash == clientFingerprint &&
            e.EventType == request.EventType &&
            e.CreatedAt >= cutoff, ct);

        if (alreadyTracked) return;

        await _viewEventRepo.AddAsync(new ArticleViewEvent
        {
            ArticleId = article.Id,
            UserId = userId,
            SessionHash = clientFingerprint,
            EventType = request.EventType
        });

        if (request.EventType == ViewEventType.View)
        {
            article.ViewsCount++;
            if (userId.HasValue) article.LoggedInViewsCount++;
        }
        else if (request.EventType == ViewEventType.EngagedView)
        {
            article.EngagedViewsCount++;
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RecalculatePopularScoresAsync(CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-30);
        var counts = await _viewEventRepo.Query()
            .Where(e => e.CreatedAt >= since)
            .GroupBy(e => e.ArticleId)
            .Select(g => new
            {
                ArticleId = g.Key,
                Views = g.Count(x => x.EventType == ViewEventType.View),
                Engaged = g.Count(x => x.EventType == ViewEventType.EngagedView)
            })
            .ToListAsync(ct);

        var lookup = counts.ToDictionary(c => c.ArticleId);
        var articles = await _articleRepository.Query().Where(a => a.Status == ArticleStatus.Published).ToListAsync(ct);
        foreach (var article in articles)
        {
            if (lookup.TryGetValue(article.Id, out var c))
                article.PopularScore = c.Views + (c.Engaged * 2.5);
            else
                article.PopularScore = article.ViewsCount * 0.1;
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class FeedService : IFeedService
{
    private readonly IArticleRepository _articleRepository;
    private readonly ISectionRepository _sectionRepository;

    public FeedService(IArticleRepository articleRepository, ISectionRepository sectionRepository)
    {
        _articleRepository = articleRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<string> GenerateRssAsync(ArticleType? type = null, string baseUrl = "https://ranamedia.co", CancellationToken ct = default)
    {
        var items = await _articleRepository.GetPublishedForFeedAsync(25, type, ct);
        var root = baseUrl.TrimEnd('/');

        var channel = new XElement("channel",
            new XElement("title", "Rana Media | a world of ideas"),
            new XElement("link", root),
            new XElement("description", "In-depth essays and curated videos across philosophy, science, psychology, society, and culture."),
            new XElement("language", "en"),
            new XElement("lastBuildDate", DateTime.UtcNow.ToString("r")),
            items.Select(a =>
            {
                var pathPrefix = a.Type == ArticleType.Video ? "videos" : "essays";
                var link = $"{root}/{pathPrefix}/{a.Slug}";
                return new XElement("item",
                    new XElement("title", a.Title),
                    new XElement("link", link),
                    new XElement("guid", new XAttribute("isPermaLink", "false"), $"urn:id:{a.Id}"),
                    new XElement("pubDate", (a.PublishedAt ?? a.CreatedAt).ToString("r")),
                    new XElement("description", a.StandfirstLong));
            })
        );

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("rss", new XAttribute("version", "2.0"), channel));

        return doc.ToString();
    }

    public async Task<string> GenerateAtomAsync(ArticleType? type = null, string baseUrl = "https://ranamedia.co", CancellationToken ct = default)
    {
        XNamespace ns = "http://www.w3.org/2005/Atom";
        var items = await _articleRepository.GetPublishedForFeedAsync(25, type, ct);
        var root = baseUrl.TrimEnd('/');

        var feed = new XElement(ns + "feed",
            new XElement(ns + "title", "Rana Media"),
            new XElement(ns + "id", root + "/"),
            new XElement(ns + "updated", DateTime.UtcNow.ToString("o")),
            items.Select(a =>
            {
                var pathPrefix = a.Type == ArticleType.Video ? "videos" : "essays";
                var link = $"{root}/{pathPrefix}/{a.Slug}";
                return new XElement(ns + "entry",
                    new XElement(ns + "title", a.Title),
                    new XElement(ns + "id", link),
                    new XElement(ns + "link", new XAttribute("href", link)),
                    new XElement(ns + "updated", (a.UpdatedAt ?? a.PublishedAt ?? a.CreatedAt).ToString("o")),
                    new XElement(ns + "summary", a.StandfirstLong));
            }));

        return new XDocument(new XDeclaration("1.0", "utf-8", null), feed).ToString();
    }

    public async Task<string> GenerateSitemapXmlAsync(string baseUrl = "https://ranamedia.co", CancellationToken ct = default)
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var root = baseUrl.TrimEnd('/');
        var sections = await _sectionRepository.GetAllWithTopicsAsync(ct);
        var articles = await _articleRepository.GetPublishedForFeedAsync(500, null, ct);

        var urls = new List<XElement>
        {
            new(ns + "url", new XElement(ns + "loc", $"{root}/")),
            new(ns + "url", new XElement(ns + "loc", $"{root}/essays")),
            new(ns + "url", new XElement(ns + "loc", $"{root}/videos")),
            new(ns + "url", new XElement(ns + "loc", $"{root}/popular"))
        };

        foreach (var s in sections)
        {
            urls.Add(new XElement(ns + "url", new XElement(ns + "loc", $"{root}/{s.Slug}")));
            foreach (var t in s.Topics)
                urls.Add(new XElement(ns + "url", new XElement(ns + "loc", $"{root}/{s.Slug}/{t.Slug}")));
        }

        foreach (var a in articles)
        {
            var pathPrefix = a.Type == ArticleType.Video ? "videos" : (a.Type == ArticleType.Idea ? "ideas" : "essays");
            urls.Add(new XElement(ns + "url",
                new XElement(ns + "loc", $"{root}/{pathPrefix}/{a.Slug}"),
                new XElement(ns + "lastmod", (a.UpdatedAt ?? a.PublishedAt ?? a.CreatedAt).ToString("yyyy-MM-dd"))));
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(ns + "urlset", urls)).ToString();
    }
}

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IArticleRepository _articleRepo;
    private readonly ICommentRepository _commentRepo;
    private readonly IGenericRepository<PitchSubmission> _pitchRepo;
    private readonly IGenericRepository<SyndicationRequest> _syndicationRepo;
    private readonly IGenericRepository<NewsletterSubscription> _newsletterRepo;
    private readonly ISeoRepository _seoRepo;
    private readonly UserManager<AppUser> _userManager;

    public AdminDashboardService(
        IArticleRepository articleRepo,
        ICommentRepository commentRepo,
        IGenericRepository<PitchSubmission> pitchRepo,
        IGenericRepository<SyndicationRequest> syndicationRepo,
        IGenericRepository<NewsletterSubscription> newsletterRepo,
        ISeoRepository seoRepo,
        UserManager<AppUser> userManager)
    {
        _articleRepo = articleRepo;
        _commentRepo = commentRepo;
        _pitchRepo = pitchRepo;
        _syndicationRepo = syndicationRepo;
        _newsletterRepo = newsletterRepo;
        _seoRepo = seoRepo;
        _userManager = userManager;
    }

    public async Task<AdminDashboardStatsDTO> GetStatsAsync(CancellationToken ct = default)
    {
        var articles = await _articleRepo.Query().AsNoTracking()
            .Select(a => new { a.Status, a.ViewsCount })
            .ToListAsync(ct);

        var comments = await _commentRepo.Query().AsNoTracking()
            .Select(c => new { c.IsModerated })
            .ToListAsync(ct);

        var seoProfiles = await _seoRepo.Query().AsNoTracking()
            .Select(p => new { p.OverallScore, p.OverallStatus })
            .ToListAsync(ct);

        var pendingPitches = await _pitchRepo.Query().CountAsync(p => p.Status == SubmissionStatus.New || p.Status == SubmissionStatus.InReview, ct);
        var pendingSyndications = await _syndicationRepo.Query().CountAsync(s => s.Status == SubmissionStatus.New || s.Status == SubmissionStatus.InReview, ct);
        var activeSubs = await _newsletterRepo.Query().CountAsync(n => n.Status == NewsletterStatus.Active, ct);
        var totalUsers = await _userManager.Users.CountAsync(ct);

        return new AdminDashboardStatsDTO
        {
            TotalArticles = articles.Count,
            PublishedArticles = articles.Count(a => a.Status == ArticleStatus.Published),
            DraftArticles = articles.Count(a => a.Status == ArticleStatus.Draft),
            ScheduledArticles = articles.Count(a => a.Status == ArticleStatus.Scheduled),
            TotalViews = articles.Sum(a => a.ViewsCount),
            TotalUsers = totalUsers,
            TotalComments = comments.Count,
            ModeratedComments = comments.Count(c => c.IsModerated),
            PendingPitches = pendingPitches,
            PendingSyndications = pendingSyndications,
            ActiveSubscribers = activeSubs,
            AverageSeoScore = seoProfiles.Count > 0 ? (int)Math.Round(seoProfiles.Average(p => p.OverallScore)) : 0,
            GreenSeoCount = seoProfiles.Count(p => p.OverallStatus == SeoTrafficLight.Green),
            OrangeSeoCount = seoProfiles.Count(p => p.OverallStatus == SeoTrafficLight.Orange),
            RedSeoCount = seoProfiles.Count(p => p.OverallStatus == SeoTrafficLight.Red)
        };
    }
}

