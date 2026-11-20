using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RanaMedia.Application.Exceptions;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.Services.System;

namespace RanaMedia.Presentation.Controllers.System;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenHandler _tokenHandler;
    private readonly IPasswordResetService _passwordResetService;

    public AuthController(
        IAuthService authService,
        ITokenHandler tokenHandler,
        IPasswordResetService passwordResetService)
    {
        _authService = authService;
        _tokenHandler = tokenHandler;
        _passwordResetService = passwordResetService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
    {
        await _authService.RegisterAsync(registerDto);
        return StatusCode(StatusCodes.Status201Created, new { message = "Qeydiyyat uğurla tamamlandı" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO request)
    {
        var response = await _tokenHandler.RefreshTokenAsync(request.RefreshToken);
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDTO request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    [HttpPost("accept-guidelines")]
    [Authorize]
    public async Task<IActionResult> AcceptCommunityGuidelines()
    {
        var userId = GetCurrentUserId();
        var result = await _authService.AcceptCommunityGuidelinesAsync(userId);
        return Ok(result);
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO request)
    {
        await _authService.AssignRoleAsync(request.UserId, request.RoleName);
        return Ok(new { message = "Rol uğurla təyin edildi" });
    }

    [HttpPost("create-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO request)
    {
        var result = await _authService.CreateRoleAsync(request);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return StatusCode(StatusCodes.Status201Created, new { message = "Rol uğurla yaradıldı" });
    }

    [HttpDelete("remove-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDTO request)
    {
        var result = await _authService.RemoveRoleAsync(request.UserId, request.RoleName);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return NoContent();
    }

    [HttpGet("users/{roleId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoleUsers(Guid roleId)
    {
        var users = await _authService.GetUserByRoleIdAsync(roleId);
        return Ok(users);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO passwordResetDto)
    {
        var result = await _passwordResetService.ResetPasswordAsync(
            passwordResetDto.Email,
            passwordResetDto.Token,
            passwordResetDto.NewPassword);

        if (!result.Succeeded)
            throw new UnauthorizedException("Şifrə sıfırlanması uğursuz oldu.");

        return Ok(new { Message = "Password reset successfully" });
    }

    [HttpPost("SendResetOtp")]
    public async Task<IActionResult> SendResetOtp([FromBody] OTPrequest request)
    {
        await _passwordResetService.SendOtpAsync(request);
        return Ok(new { Message = "Password reset OTP sent successfully" });
    }

    [HttpGet("GetCurrentUserData")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "Token tapılmadı" });

        var data = await _authService.GetUserByJwtTokenAsync(token);
        return Ok(data);
    }

    [HttpGet("GetAllUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDTO filter)
    {
        var result = await _authService.GetAllUsersAsync(filter);
        return Ok(result);
    }

    [HttpGet("GetAllRoles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _authService.GetAllRolesAsync();
        return Ok(result);
    }

    [HttpPut("edit")]
    [Authorize]
    public async Task<IActionResult> EditUser([FromQuery] Guid? userId, [FromBody] EditUserDTO editUserDto)
    {
        var currentId = GetCurrentUserId();
        var targetId = User.IsInRole("Admin") && userId.HasValue ? userId.Value : currentId;
        var result = await _authService.EditUserAsync(targetId, editUserDto);
        return Ok(result);
    }

    [HttpDelete("delete/User")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid userId)
    {
        await _authService.DeleteUserAsync(userId);
        return Ok(new { message = "İstifadəçi uğurla silindi" });
    }

    private Guid GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(raw, out var id))
            throw new UnauthorizedException("İstifadəçi identifikatoru tapılmadı.");
        return id;
    }
}

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int limit = 30)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _notificationService.GetMyNotificationsAsync(userId, limit));
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _notificationService.MarkAsReadAsync(userId, id);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}

