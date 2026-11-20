using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Contract.Services.Seo;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Presentation.Controllers.Admin;

[Route("api/admin/articles")]
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public class AdminArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;
    public AdminArticlesController(IArticleService articleService) => _articleService = articleService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ArticleStatus? status = null,
        [FromQuery] ArticleType? type = null,
        [FromQuery] string? search = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _articleService.GetAdminArticlesAsync(status, type, search, pageNumber, pageSize, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _articleService.GetAdminArticleByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleDTO request, CancellationToken ct)
    {
        var result = await _articleService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDTO request, CancellationToken ct)
        => Ok(await _articleService.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _articleService.DeleteAsync(id, ct);
        return NoContent();
    }
}

[Route("api/admin/seo")]
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public class AdminSeoController : ControllerBase
{
    private readonly ISeoAnalysisService _seoService;
    public AdminSeoController(ISeoAnalysisService seoService) => _seoService = seoService;

    /// <summary>
    /// Yoast-style real-time SEO analysis for draft or existing article content.
    /// </summary>
    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeLive([FromBody] AnalyzeSeoRequestDTO request, CancellationToken ct)
        => Ok(await _seoService.AnalyzeLiveAsync(request, ct));

    [HttpPost("articles/{articleId:guid}/analyze")]
    public async Task<IActionResult> AnalyzeAndPersist(Guid articleId, [FromBody] AnalyzeSeoRequestDTO request, CancellationToken ct)
        => Ok(await _seoService.AnalyzeAndPersistForArticleAsync(articleId, request, ct));

    [HttpGet("articles/{articleId:guid}")]
    public async Task<IActionResult> GetArticleProfile(Guid articleId, CancellationToken ct)
    {
        var profile = await _seoService.GetArticleSeoProfileAsync(articleId, ct);
        if (profile == null) return NotFound(new { message = "Bu məqalə üçün SEO profili tapılmadı." });
        return Ok(profile);
    }

    [HttpGet("keyphrase-usage")]
    public async Task<IActionResult> GetKeyphraseUsage([FromQuery] string keyphrase, [FromQuery] Guid? excludeArticleId = null, CancellationToken ct = default)
        => Ok(await _seoService.GetKeyphraseUsageAsync(keyphrase, excludeArticleId, ct));
}

[Route("api/admin/contributors")]
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public class AdminContributorsController : ControllerBase
{
    private readonly IContributorService _contributorService;
    public AdminContributorsController(IContributorService contributorService) => _contributorService = contributorService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _contributorService.GetAllContributorsAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContributorDTO request, CancellationToken ct)
    {
        var created = await _contributorService.CreateContributorAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _contributorService.DeleteContributorAsync(id, ct);
        return NoContent();
    }
}

[Route("api/admin/comments")]
[ApiController]
[Authorize(Roles = "Admin,Moderator")]
public class AdminModerationController : ControllerBase
{
    private readonly ICommentService _commentService;
    public AdminModerationController(ICommentService commentService) => _commentService = commentService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? onlyModerated = null,
        [FromQuery] bool? onlyReported = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
        => Ok(await _commentService.GetAdminCommentsAsync(onlyModerated, onlyReported, pageNumber, pageSize, ct));

    [HttpPost("{id:guid}/moderate")]
    public async Task<IActionResult> Moderate(Guid id, [FromBody] ModerateCommentDTO? body, CancellationToken ct)
    {
        await _commentService.ModerateCommentAsync(id, true, body?.Reason, ct);
        return Ok(new { success = true });
    }

    [HttpPost("{id:guid}/unmoderate")]
    public async Task<IActionResult> Unmoderate(Guid id, CancellationToken ct)
    {
        await _commentService.ModerateCommentAsync(id, false, null, ct);
        return Ok(new { success = true });
    }

    [HttpPost("{id:guid}/feature")]
    public async Task<IActionResult> Feature(Guid id, CancellationToken ct)
    {
        await _commentService.FeatureCommentAsync(id, true, ct);
        return Ok(new { success = true });
    }

    [HttpPost("{id:guid}/unfeature")]
    public async Task<IActionResult> Unfeature(Guid id, CancellationToken ct)
    {
        await _commentService.FeatureCommentAsync(id, false, ct);
        return Ok(new { success = true });
    }
}

[Route("api/admin/submissions")]
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public class AdminSubmissionsController : ControllerBase
{
    private readonly IPitchService _pitchService;
    private readonly ISyndicationService _syndicationService;

    public AdminSubmissionsController(IPitchService pitchService, ISyndicationService syndicationService)
    {
        _pitchService = pitchService;
        _syndicationService = syndicationService;
    }

    [HttpGet("pitches")]
    public async Task<IActionResult> GetPitches([FromQuery] SubmissionStatus? status = null, CancellationToken ct = default)
        => Ok(await _pitchService.GetAllPitchesAsync(status, ct));

    [HttpPatch("pitches/{id:guid}")]
    public async Task<IActionResult> UpdatePitch(Guid id, [FromQuery] SubmissionStatus status, [FromQuery] string? notes = null, CancellationToken ct = default)
        => Ok(await _pitchService.UpdatePitchStatusAsync(id, status, notes, ct));

    [HttpGet("syndication")]
    public async Task<IActionResult> GetSyndications([FromQuery] SubmissionStatus? status = null, CancellationToken ct = default)
        => Ok(await _syndicationService.GetAllRequestsAsync(status, ct));

    [HttpPatch("syndication/{id:guid}")]
    public async Task<IActionResult> UpdateSyndication(Guid id, [FromQuery] SubmissionStatus status, CancellationToken ct = default)
        => Ok(await _syndicationService.UpdateStatusAsync(id, status, ct));
}

[Route("api/admin/media")]
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public class AdminMediaController : ControllerBase
{
    private readonly IMediaStorageService _mediaStorage;
    public AdminMediaController(IMediaStorageService mediaStorage) => _mediaStorage = mediaStorage;

    [HttpPost("upload")]
    [RequestSizeLimit(110_000_000)]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] MediaFolder folder = MediaFolder.Images, CancellationToken ct = default)
    {
        var result = await _mediaStorage.UploadAsync(file, folder, ct);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string relativeUrl, CancellationToken ct)
    {
        await _mediaStorage.DeleteAsync(relativeUrl, ct);
        return NoContent();
    }
}

[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "Admin,Editor,Moderator")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _dashboardService;
    public AdminDashboardController(IAdminDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
        => Ok(await _dashboardService.GetStatsAsync(ct));
}

