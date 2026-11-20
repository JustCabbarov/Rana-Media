using RanaMedia.Domain.Enums;

namespace RanaMedia.Domain.Entities;

public class NewsletterSubscription : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public bool Daily { get; set; }
    public bool Weekly { get; set; }
    public NewsletterStatus Status { get; set; } = NewsletterStatus.Pending;
    public string? Source { get; set; }
    public string? SignupId { get; set; }
    public string ConfirmToken { get; set; } = string.Empty;
    public string UnsubscribeToken { get; set; } = string.Empty;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
}

public class PitchSubmission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? Website { get; set; }
    public bool PreviousContributor { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Format { get; set; } = "Essay";
    public string PitchText { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.New;
    public string? EditorNotes { get; set; }
}

public class SyndicationRequest : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Publication { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Language { get; set; } = "English";
    public string MediumJson { get; set; } = "{}";
    public bool TermsAccepted { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.New;
}

public class StaticPage : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsPublished { get; set; } = true;
}

public class CallToAction : BaseEntity
{
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FeatureFlag : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? Description { get; set; }
}

