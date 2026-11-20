using Microsoft.AspNetCore.Identity;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.DTOs.Common;
using RanaMedia.Contract.DTOs.Forms;
using RanaMedia.Domain.Entities;

namespace RanaMedia.Contract.Services.System;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto);
    Task RegisterAsync(RegisterDTO registerDto);
    Task LogoutAsync(string refreshToken);
    Task AssignRoleAsync(Guid userId, string roleName);
    Task<IdentityResult> CreateRoleAsync(CreateRoleDTO role);
    Task<IList<AppUser>> GetUserByRoleIdAsync(Guid roleId);
    Task<IdentityResult> RemoveRoleAsync(Guid userId, string roleName);
    Task<UserDTO> GetUserByJwtTokenAsync(string jwtToken);
    Task<PaginatedResult<UserDTO>> GetAllUsersAsync(UserFilterDTO filter);
    Task<List<CreateRoleDTO>> GetAllRolesAsync();
    Task<UserDTO> EditUserAsync(Guid userId, EditUserDTO editUserDto);
    Task DeleteUserAsync(Guid userId);
    Task<AcceptGuidelinesResponseDTO> AcceptCommunityGuidelinesAsync(Guid userId);
}

public interface ITokenHandler
{
    Task<string> CreateAccessTokenAsync(AppUser user);
    Task<string> CreateRefreshTokenAsync(AppUser user);
    Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
    Task RevokeTokenAsync(string refreshToken);
}

public interface IGenericService<TVM, TEntity>
    where TVM : class
    where TEntity : BaseEntity, new()
{
    Task<TVM> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    Task<IEnumerable<TVM>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    Task<TVM> AddAsync(TVM entity);
    Task<TVM> UpdateAsync(TVM entity);
    Task<bool> DeleteAsync(Guid id);
}

public interface IPasswordResetService
{
    Task SendOtpAsync(OTPrequest request);
    Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
}

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    Task SendOtpEmailAsync(string toEmail, string otp);
}

public interface INotificationService
{
    Task<List<NotificationDTO>> GetMyNotificationsAsync(Guid userId, int limit = 30);
    Task MarkAsReadAsync(Guid userId, Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
}

