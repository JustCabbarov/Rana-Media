using RanaMedia.Domain.Enums;

namespace RanaMedia.Domain.Entities;

public class ArticleTopic : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public Guid TopicId { get; set; }
    public Topic Topic { get; set; } = null!;
}

public class ArticleTag : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

public class ArticleContributor : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public Guid ContributorId { get; set; }
    public Contributor Contributor { get; set; } = null!;
    public ContributorRole Role { get; set; } = ContributorRole.Author;
    public int DisplayOrder { get; set; }
}

public class ArticleRelation : BaseEntity
{
    public Guid SourceArticleId { get; set; }
    public Article SourceArticle { get; set; } = null!;
    public Guid RelatedArticleId { get; set; }
    public Article RelatedArticle { get; set; } = null!;
    public int DisplayOrder { get; set; }
}

public class ArticleViewEvent : BaseEntity
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public Guid? UserId { get; set; }
    public string SessionHash { get; set; } = string.Empty;
    public ViewEventType EventType { get; set; } = ViewEventType.View;
}

