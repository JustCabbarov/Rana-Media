using System.ComponentModel.DataAnnotations;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Contract.DTOs.Engagement;

public class CommentUserDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsArticleAuthor { get; set; }
}

public class CommentReplyDTO
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditableUntil { get; set; }
    public bool CanEditNow { get; set; }
    public bool IsModerated { get; set; }
    public int LikesCount { get; set; }
    public CommentUserDTO User { get; set; } = new();
}

public class CommentDTO
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditableUntil { get; set; }
    public bool CanEditNow { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsModerated { get; set; }
    public int LikesCount { get; set; }
    public CommentUserDTO User { get; set; } = new();
    public List<CommentReplyDTO> Replies { get; set; } = new();
}

public class CreateCommentDTO
{
    [Required, MinLength(2), MaxLength(5000)]
    public string Body { get; set; } = string.Empty;
}

public class UpdateCommentDTO
{
    [Required, MinLength(2), MaxLength(5000)]
    public string Body { get; set; } = string.Empty;
}

public class CreateReplyDTO
{
    [Required, MinLength(2), MaxLength(5000)]
    public string Body { get; set; } = string.Empty;
}

public class ReportCommentDTO
{
    [Required, MinLength(3), MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class ModerateCommentDTO
{
    public string? Reason { get; set; }
}

public class PageMarkDTO
{
    public Guid Id { get; set; }
    public PageMarkType Type { get; set; }
    public int Index { get; set; }
    [Required]
    public string Text { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public int? AnnotationNumber { get; set; }
}

public class UpdatePageMarksRequestDTO
{
    public List<PageMarkDTO> PageMarks { get; set; } = new();
}

public class SavedArticleDTO
{
    public Guid SaveId { get; set; }
    public bool IsSeen { get; set; }
    public DateTime SavedAt { get; set; }
    public ArticleCardDTO Article { get; set; } = null!;
}

public class LibrarySummaryDTO
{
    public int UnseenCount { get; set; }
    public List<SavedArticleDTO> Items { get; set; } = new();
}

