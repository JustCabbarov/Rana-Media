namespace RanaMedia.Domain.Entities;

public class Contributor : BaseEntity
{
    public Guid? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Occupation { get; set; }
    public string? Company { get; set; }
    public string BioHtml { get; set; } = string.Empty;
    public string? ShortBioHtml { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Twitter { get; set; }
    public string? Bluesky { get; set; }
    public string? LinkedIn { get; set; }
    public string? AcademiaEdu { get; set; }
    public string? Website { get; set; }

    public ICollection<ArticleContributor> ArticleContributions { get; set; } = new List<ArticleContributor>();
}

