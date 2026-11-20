using Microsoft.AspNetCore.Identity;

namespace RanaMedia.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? CommunityGuidelinesAcceptedAt { get; set; }

    public Contributor? ContributorProfile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<UserSave> UserSaves { get; set; } = new List<UserSave>();
    public ICollection<PageMark> PageMarks { get; set; } = new List<PageMark>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<CommentReply> CommentReplies { get; set; } = new List<CommentReply>();
}

