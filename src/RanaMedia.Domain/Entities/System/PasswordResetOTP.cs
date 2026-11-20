namespace RanaMedia.Domain.Entities;

public class PasswordResetOTP : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
}

