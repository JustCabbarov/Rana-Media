namespace RanaMedia.Domain.Entities;

public class Section : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string AccentColorHex { get; set; } = "#9d1d20";
    public int SortOrder { get; set; }

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}

