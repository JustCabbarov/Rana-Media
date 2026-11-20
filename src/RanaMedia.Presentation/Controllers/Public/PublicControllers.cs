using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RanaMedia.Contract.DTOs.Forms;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Contract.Services.System;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Presentation.Controllers.Public;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;
    public HomeController(IHomeService homeService) => _homeService = homeService;

    [HttpGet]
    [OutputCache(Duration = 120)]
    public async Task<IActionResult> GetHome(CancellationToken ct)
        => Ok(await _homeService.GetHomePageAsync(ct));

    [HttpGet("more")]
    public async Task<IActionResult> GetMore([FromQuery] string? afterCursor, [FromQuery] int first = 30, CancellationToken ct = default)
        => Ok(await _homeService.GetMoreHomeArticlesAsync(afterCursor, first, ct));
}

[Route("api/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;
    public ArticlesController(IArticleService articleService) => _articleService = articleService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ArticleType? type = null,
        [FromQuery] string sort = "latest",
        [FromQuery] string? tag = null,
        [FromQuery] string? afterCursor = null,
        [FromQuery] int first = 12,
        CancellationToken ct = default)
        => Ok(await _articleService.GetArticlesAsync(type, sort, tag, afterCursor, first, ct));

    [HttpGet("/api/essays")]
    public async Task<IActionResult> GetEssays([FromQuery] string sort = "latest", [FromQuery] string? afterCursor = null, [FromQuery] int first = 12, CancellationToken ct = default)
        => Ok(await _articleService.GetArticlesAsync(ArticleType.Essay, sort, null, afterCursor, first, ct));

    [HttpGet("/api/essays/popular")]
    public async Task<IActionResult> GetPopularEssays([FromQuery] string? afterCursor = null, [FromQuery] int first = 12, CancellationToken ct = default)
        => Ok(await _articleService.GetArticlesAsync(ArticleType.Essay, "popular", null, afterCursor, first, ct));

    [HttpGet("/api/popular")]
    public async Task<IActionResult> GetPopular([FromQuery] string? afterCursor = null, [FromQuery] int first = 24, CancellationToken ct = default)
        => Ok(await _articleService.GetArticlesAsync(null, "popular", null, afterCursor, first, ct));

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
        => Ok(await _articleService.GetBySlugAsync(slug, ct));

    [HttpGet("{slug}/preview")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> GetPreview(string slug, [FromQuery] string token, CancellationToken ct)
        => Ok(await _articleService.GetPreviewAsync(slug, token, ct));
}

[Route("api/[controller]")]
[ApiController]
public class VideosController : ControllerBase
{
    private readonly IVideoService _videoService;
    public VideosController(IVideoService videoService) => _videoService = videoService;

    [HttpGet]
    public async Task<IActionResult> GetVideos([FromQuery] string? filter = null, [FromQuery] string? afterCursor = null, [FromQuery] int first = 12, CancellationToken ct = default)
        => Ok(await _videoService.GetVideosAsync(filter, afterCursor, first, ct));

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetVideoBySlug(string slug, CancellationToken ct)
        => Ok(await _videoService.GetVideoBySlugAsync(slug, ct));
}

[Route("api/[controller]")]
[ApiController]
public class SectionsController : ControllerBase
{
    private readonly ISectionService _sectionService;
    public SectionsController(ISectionService sectionService) => _sectionService = sectionService;

    [HttpGet]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _sectionService.GetAllSectionsAsync(ct));

    [HttpGet("catalog")]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> GetCatalog(CancellationToken ct)
        => Ok(await _sectionService.GetCatalogAsync(ct));

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] string sort = "latest", [FromQuery] string? afterCursor = null, [FromQuery] int first = 24, CancellationToken ct = default)
        => Ok(await _sectionService.GetSectionBySlugAsync(slug, sort, afterCursor, first, ct));
}

[Route("api/sections/{sectionSlug}/topics")]
[ApiController]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;
    public TopicsController(ITopicService topicService) => _topicService = topicService;

    [HttpGet("{topicSlug}")]
    public async Task<IActionResult> GetBySlug(string sectionSlug, string topicSlug, [FromQuery] string sort = "latest", [FromQuery] string? afterCursor = null, [FromQuery] int first = 24, CancellationToken ct = default)
        => Ok(await _topicService.GetTopicBySlugAsync(sectionSlug, topicSlug, sort, afterCursor, first, ct));
}

[Route("api/users")]
[ApiController]
public class ContributorsController : ControllerBase
{
    private readonly IContributorService _contributorService;
    public ContributorsController(IContributorService contributorService) => _contributorService = contributorService;

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetProfile(string slug, [FromQuery] string? authoredCursor = null, [FromQuery] string? editedCursor = null, CancellationToken ct = default)
        => Ok(await _contributorService.GetContributorBySlugAsync(slug, authoredCursor, editedCursor, ct));
}

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    public SearchController(ISearchService searchService) => _searchService = searchService;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q = "", [FromQuery] ArticleType? type = null, [FromQuery] string? afterCursor = null, [FromQuery] int first = 30, CancellationToken ct = default)
        => Ok(await _searchService.SearchAsync(q, type, afterCursor, first, ct));
}

[Route("api")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly IViewAnalyticsService _analyticsService;
    public AnalyticsController(IViewAnalyticsService analyticsService) => _analyticsService = analyticsService;

    [HttpPost("view")]
    public async Task<IActionResult> TrackView([FromBody] TrackViewDTO request, CancellationToken ct)
    {
        Guid? userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : null;
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = HttpContext.Request.Headers.UserAgent.ToString();
        var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{ip}|{ua}")));

        await _analyticsService.TrackViewAsync(request, userId, fingerprint, ct);
        return Accepted();
    }
}

[Route("api")]
[ApiController]
public class FormsController : ControllerBase
{
    private readonly INewsletterService _newsletterService;
    private readonly IPitchService _pitchService;
    private readonly ISyndicationService _syndicationService;
    private readonly IGenericService<StaticPageDTO, StaticPage> _staticPageService;
    private readonly IGenericService<CallToActionDTO, CallToAction> _ctaService;

    public FormsController(
        INewsletterService newsletterService,
        IPitchService pitchService,
        ISyndicationService syndicationService,
        IGenericService<StaticPageDTO, StaticPage> staticPageService,
        IGenericService<CallToActionDTO, CallToAction> ctaService)
    {
        _newsletterService = newsletterService;
        _pitchService = pitchService;
        _syndicationService = syndicationService;
        _staticPageService = staticPageService;
        _ctaService = ctaService;
    }

    [HttpPost("newsletter/subscribe")]
    public async Task<IActionResult> SubscribeNewsletter([FromBody] NewsletterSubscribeDTO request, CancellationToken ct)
    {
        await _newsletterService.SubscribeAsync(request, ct);
        return Ok(new { success = true, message = "Təsdiq e-poçtu göndərildi." });
    }

    [HttpGet("newsletter/confirm")]
    public async Task<IActionResult> ConfirmNewsletter([FromQuery] string token, CancellationToken ct)
    {
        await _newsletterService.ConfirmSubscriptionAsync(token, ct);
        return Ok(new { success = true, message = "Newsletter abunəliyiniz təsdiqləndi." });
    }

    [HttpGet("newsletter/unsubscribe")]
    public async Task<IActionResult> UnsubscribeNewsletter([FromQuery] string token, CancellationToken ct)
    {
        await _newsletterService.UnsubscribeAsync(token, ct);
        return Ok(new { success = true, message = "Abunəlik dayandırıldı." });
    }

    [HttpPost("pitches")]
    public async Task<IActionResult> SubmitPitch([FromBody] CreatePitchDTO request, CancellationToken ct)
    {
        var result = await _pitchService.SubmitPitchAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("syndication")]
    public async Task<IActionResult> SubmitSyndication([FromBody] CreateSyndicationDTO request, CancellationToken ct)
    {
        var result = await _syndicationService.SubmitSyndicationAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("pages/{slug}")]
    public async Task<IActionResult> GetStaticPage(string slug)
    {
        var all = await _staticPageService.GetAllAsync();
        var page = all.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        if (page == null) return NotFound(new { message = $"Səhifə tapılmadı: {slug}" });
        return Ok(page);
    }

    [HttpGet("call-to-actions")]
    public async Task<IActionResult> GetCallToActions() => Ok(await _ctaService.GetAllAsync());
}

[ApiController]
public class FeedsController : ControllerBase
{
    private readonly IFeedService _feedService;
    public FeedsController(IFeedService feedService) => _feedService = feedService;

    private string BaseUrl => $"{Request.Scheme}://{Request.Host}";

    [HttpGet("/feed.rss")]
    public async Task<IActionResult> GetMainRss(CancellationToken ct)
        => Content(await _feedService.GenerateRssAsync(null, BaseUrl, ct), "application/rss+xml; charset=utf-8");

    [HttpGet("/feed.atom")]
    public async Task<IActionResult> GetMainAtom(CancellationToken ct)
        => Content(await _feedService.GenerateAtomAsync(null, BaseUrl, ct), "application/atom+xml; charset=utf-8");

    [HttpGet("/essays/feed.rss")]
    public async Task<IActionResult> GetEssaysRss(CancellationToken ct)
        => Content(await _feedService.GenerateRssAsync(ArticleType.Essay, BaseUrl, ct), "application/rss+xml; charset=utf-8");

    [HttpGet("/videos/feed.rss")]
    public async Task<IActionResult> GetVideosRss(CancellationToken ct)
        => Content(await _feedService.GenerateRssAsync(ArticleType.Video, BaseUrl, ct), "application/rss+xml; charset=utf-8");

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> GetSitemap(CancellationToken ct)
        => Content(await _feedService.GenerateSitemapXmlAsync(BaseUrl, ct), "application/xml; charset=utf-8");
}

