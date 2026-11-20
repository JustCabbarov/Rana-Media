using System.ComponentModel.DataAnnotations;

namespace RanaMedia.Contract.DTOs.Auth;

public record RegisterDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    public string? Phone { get; set; }
}

public record LoginDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class LoginResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
}

public class RefreshTokenDTO
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CommunityGuidelinesAcceptedAt { get; set; }
    public int UnseenSavedArticleCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public List<string> RoleName { get; set; } = new();
}

public class UserFilterDTO
{
    public Guid? UserId { get; set; }
    public string? Search { get; set; }
    public string? Role { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class EditUserDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateRoleDTO
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class AssignRoleDTO
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string RoleName { get; set; } = string.Empty;
}

public class OTPrequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class PasswordResetDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}

public class AcceptGuidelinesResponseDTO
{
    public bool Accepted { get; set; }
    public DateTime AcceptedAt { get; set; }
}

