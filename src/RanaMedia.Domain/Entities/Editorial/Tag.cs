namespace RanaMedia.Domain.Entities;

public class Tag : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}

