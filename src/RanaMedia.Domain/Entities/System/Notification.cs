namespace RanaMedia.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info";
    public string? LinkUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
}

