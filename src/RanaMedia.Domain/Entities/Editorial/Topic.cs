namespace RanaMedia.Domain.Entities;

public class Topic : BaseEntity
{
    public Guid SectionId { get; set; }
    public Section Section { get; set; } = null!;

    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;

    public ICollection<ArticleTopic> ArticleTopics { get; set; } = new List<ArticleTopic>();
    public ICollection<Article> PrimaryArticles { get; set; } = new List<Article>();
}

