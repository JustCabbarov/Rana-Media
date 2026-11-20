namespace RanaMedia.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
}

