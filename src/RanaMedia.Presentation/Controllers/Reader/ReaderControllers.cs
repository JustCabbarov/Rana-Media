using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RanaMedia.Application.Exceptions;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Contract.Services.Editorial;

namespace RanaMedia.Presentation.Controllers.Reader;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    public LibraryController(ILibraryService libraryService) => _libraryService = libraryService;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetLibrary(CancellationToken ct)
        => Ok(await _libraryService.GetUserLibraryAsync(UserId, ct));

    [HttpGet("annotated")]
    public async Task<IActionResult> GetAnnotated(CancellationToken ct)
        => Ok(await _libraryService.GetUserAnnotatedArticlesAsync(UserId, ct));

    [HttpGet("saved-ids")]
    public async Task<IActionResult> GetSavedIds(CancellationToken ct)
        => Ok(await _libraryService.GetSavedArticleIdsAsync(UserId, ct));

    [HttpPost("saves/{slug}")]
    public async Task<IActionResult> SaveArticle(string slug, CancellationToken ct)
    {
        await _libraryService.SaveArticleAsync(UserId, slug, ct);
        return Ok(new { success = true });
    }

    [HttpDelete("saves/{slug}")]
    public async Task<IActionResult> RemoveSavedArticle(string slug, CancellationToken ct)
    {
        await _libraryService.RemoveSavedArticleAsync(UserId, slug, ct);
        return NoContent();
    }
}

[Route("api/articles/{slug}/pagemarks")]
[ApiController]
[Authorize]
public class PageMarksController : ControllerBase
{
    private readonly IPageMarkService _pageMarkService;
    public PageMarksController(IPageMarkService pageMarkService) => _pageMarkService = pageMarkService;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMarks(string slug, CancellationToken ct)
        => Ok(await _pageMarkService.GetUserArticleMarksAsync(UserId, slug, ct));

    [HttpPut]
    public async Task<IActionResult> ReplaceMarks(string slug, [FromBody] UpdatePageMarksRequestDTO request, CancellationToken ct)
        => Ok(await _pageMarkService.ReplaceArticleMarksAsync(UserId, slug, request, ct));
}

[Route("api")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentsController(ICommentService commentService) => _commentService = commentService;

    private Guid? TryGetUserId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : null;

    private Guid RequiredUserId() =>
        TryGetUserId() ?? throw new UnauthorizedException("İstifadəçi daxil olmayıb.");

    [HttpGet("articles/{slug}/comments")]
    public async Task<IActionResult> GetArticleComments(
        string slug,
        [FromQuery] string sort = "newest",
        [FromQuery] string? afterCursor = null,
        [FromQuery] int first = 20,
        CancellationToken ct = default)
        => Ok(await _commentService.GetArticleCommentsAsync(slug, TryGetUserId(), sort, afterCursor, first, ct));

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] int limit = 20, CancellationToken ct = default)
        => Ok(await _commentService.GetConversationsAsync(limit, ct));

    [HttpPost("articles/{slug}/comments")]
    [Authorize]
    public async Task<IActionResult> CreateComment(string slug, [FromBody] CreateCommentDTO request, CancellationToken ct)
    {
        var created = await _commentService.CreateCommentAsync(RequiredUserId(), slug, request, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("comments/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDTO request, CancellationToken ct)
        => Ok(await _commentService.UpdateCommentAsync(RequiredUserId(), id, request, ct));

    [HttpDelete("comments/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid id, CancellationToken ct)
    {
        bool isMod = User.IsInRole("Admin") || User.IsInRole("Moderator");
        await _commentService.DeleteCommentAsync(RequiredUserId(), isMod, id, ct);
        return NoContent();
    }

    [HttpPost("comments/{id:guid}/replies")]
    [Authorize]
    public async Task<IActionResult> CreateReply(Guid id, [FromBody] CreateReplyDTO request, CancellationToken ct)
    {
        var created = await _commentService.CreateReplyAsync(RequiredUserId(), id, request, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("replies/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateReply(Guid id, [FromBody] UpdateCommentDTO request, CancellationToken ct)
        => Ok(await _commentService.UpdateReplyAsync(RequiredUserId(), id, request, ct));

    [HttpDelete("replies/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReply(Guid id, CancellationToken ct)
    {
        bool isMod = User.IsInRole("Admin") || User.IsInRole("Moderator");
        await _commentService.DeleteReplyAsync(RequiredUserId(), isMod, id, ct);
        return NoContent();
    }

    [HttpPost("comments/{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> LikeComment(Guid id, CancellationToken ct)
    {
        await _commentService.ToggleLikeAsync(RequiredUserId(), id, null, true, ct);
        return Ok(new { success = true });
    }

    [HttpDelete("comments/{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> UnlikeComment(Guid id, CancellationToken ct)
    {
        await _commentService.ToggleLikeAsync(RequiredUserId(), id, null, false, ct);
        return NoContent();
    }

    [HttpPost("replies/{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> LikeReply(Guid id, CancellationToken ct)
    {
        await _commentService.ToggleLikeAsync(RequiredUserId(), null, id, true, ct);
        return Ok(new { success = true });
    }

    [HttpDelete("replies/{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> UnlikeReply(Guid id, CancellationToken ct)
    {
        await _commentService.ToggleLikeAsync(RequiredUserId(), null, id, false, ct);
        return NoContent();
    }

    [HttpPost("comments/{id:guid}/report")]
    [Authorize]
    public async Task<IActionResult> ReportComment(Guid id, [FromBody] ReportCommentDTO request, CancellationToken ct)
    {
        await _commentService.ReportAsync(RequiredUserId(), id, null, request, ct);
        return Ok(new { success = true });
    }

    [HttpPost("replies/{id:guid}/report")]
    [Authorize]
    public async Task<IActionResult> ReportReply(Guid id, [FromBody] ReportCommentDTO request, CancellationToken ct)
    {
        await _commentService.ReportAsync(RequiredUserId(), null, id, request, ct);
        return Ok(new { success = true });
    }
}

