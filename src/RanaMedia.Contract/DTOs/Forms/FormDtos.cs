using System.ComponentModel.DataAnnotations;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Contract.DTOs.Forms;

public class NewsletterSubscribeDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    public bool Daily { get; set; }
    public bool Weekly { get; set; }
    public string? Source { get; set; }
    public string? SignupId { get; set; }
    public string? Website { get; set; } // Honeypot: must remain empty
    public string? TurnstileToken { get; set; }
}

public class CreatePitchDTO
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(10)]
    public string Bio { get; set; } = string.Empty;
    public string? Website { get; set; }
    public bool PreviousContributor { get; set; }
    [Required, MaxLength(250)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Field { get; set; } = string.Empty;
    [Required]
    public string Format { get; set; } = "Essay";
    [Required, MinLength(50)]
    public string Pitch { get; set; } = string.Empty;
    public string? TurnstileToken { get; set; }
}

public class SyndicationMediumDTO
{
    public bool Print { get; set; }
    public bool Online { get; set; }
    public string? PublicationUrl { get; set; }
    public int? EstimatedCirculation { get; set; }
}

public class CreateSyndicationDTO
{
    [Required]
    public string ArticleSlug { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Company { get; set; } = string.Empty;
    [Required]
    public string Publication { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string PostCode { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string State { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string Language { get; set; } = "English";
    public SyndicationMediumDTO Medium { get; set; } = new();
    public bool TermsAndConditions { get; set; }
    public string? TurnstileToken { get; set; }
}

public class PitchResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; }
    public string? EditorNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SyndicationResponseDTO
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleSlug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Publication { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TrackViewDTO
{
    [Required]
    public string Slug { get; set; } = string.Empty;
    public ViewEventType EventType { get; set; } = ViewEventType.View;
}

public class StaticPageDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class CallToActionDTO
{
    public Guid Id { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
}

public class FeatureFlagDTO
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? Description { get; set; }
}

public class NotificationDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

