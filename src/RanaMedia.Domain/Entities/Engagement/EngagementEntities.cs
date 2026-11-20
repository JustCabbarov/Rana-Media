using RanaMedia.Domain.Enums;

namespace RanaMedia.Domain.Entities;

public class UserSave : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public bool IsSeen { get; set; } = false;
}

public class PageMark : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public PageMarkType Type { get; set; }
    public int OccurrenceIndex { get; set; }
    public string SelectedText { get; set; } = string.Empty;
    public string? CommentText { get; set; }
    public int? AnnotationNumber { get; set; }
}

public class Comment : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public string BodyHtml { get; set; } = string.Empty;
    public DateTime EditableUntil { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsModerated { get; set; } = false;
    public string? ModerationReason { get; set; }
    public int LikesCount { get; set; }

    public ICollection<CommentReply> Replies { get; set; } = new List<CommentReply>();
    public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();
    public ICollection<CommentReport> Reports { get; set; } = new List<CommentReport>();
}

public class CommentReply : BaseEntity
{
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public string BodyHtml { get; set; } = string.Empty;
    public DateTime EditableUntil { get; set; }
    public bool IsModerated { get; set; } = false;
    public int LikesCount { get; set; }

    public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();
    public ICollection<CommentReport> Reports { get; set; } = new List<CommentReport>();
}

public class CommentLike : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
    public Guid? ReplyId { get; set; }
    public CommentReply? Reply { get; set; }
}

public class CommentReport : BaseEntity
{
    public Guid ReporterUserId { get; set; }
    public AppUser ReporterUser { get; set; } = null!;
    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
    public Guid? ReplyId { get; set; }
    public CommentReply? Reply { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
}

