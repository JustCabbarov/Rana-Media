using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RanaMedia.Domain.Entities;
using System.Linq.Expressions;

namespace RanaMedia.Persistence.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetOTP> PasswordResetOtps => Set<PasswordResetOTP>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Contributor> Contributors => Set<Contributor>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleTopic> ArticleTopics => Set<ArticleTopic>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
    public DbSet<ArticleContributor> ArticleContributors => Set<ArticleContributor>();
    public DbSet<ArticleRelation> ArticleRelations => Set<ArticleRelation>();
    public DbSet<ArticleViewEvent> ArticleViewEvents => Set<ArticleViewEvent>();

    public DbSet<UserSave> UserSaves => Set<UserSave>();
    public DbSet<PageMark> PageMarks => Set<PageMark>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentReply> CommentReplies => Set<CommentReply>();
    public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
    public DbSet<CommentReport> CommentReports => Set<CommentReport>();

    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();
    public DbSet<PitchSubmission> PitchSubmissions => Set<PitchSubmission>();
    public DbSet<SyndicationRequest> SyndicationRequests => Set<SyndicationRequest>();
    public DbSet<StaticPage> StaticPages => Set<StaticPage>();
    public DbSet<CallToAction> CallToActions => Set<CallToAction>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    public DbSet<ArticleSeoProfile> ArticleSeoProfiles => Set<ArticleSeoProfile>();
    public DbSet<RelatedKeyphrase> RelatedKeyphrases => Set<RelatedKeyphrase>();
    public DbSet<SeoAnalysisSnapshot> SeoAnalysisSnapshots => Set<SeoAnalysisSnapshot>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var isNpgsql = Database.IsNpgsql();

        // Global soft-delete filter for all BaseEntity types and AppUser
        builder.Entity<AppUser>().HasQueryFilter(u => !u.IsDeleted);
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Id))
                    .ValueGeneratedNever();

                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(prop), parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

        // System
        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Editorial
        builder.Entity<Section>().HasIndex(s => s.Slug).IsUnique();
        builder.Entity<Topic>().HasIndex(t => new { t.SectionId, t.Slug }).IsUnique();
        builder.Entity<Tag>().HasIndex(t => t.Slug).IsUnique();
        builder.Entity<Contributor>().HasIndex(c => c.Slug).IsUnique();
        builder.Entity<Contributor>()
            .HasOne(c => c.AppUser)
            .WithOne(u => u.ContributorProfile)
            .HasForeignKey<Contributor>(c => c.AppUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Article>().HasIndex(a => a.Slug).IsUnique();
        builder.Entity<Article>().HasIndex(a => new { a.Status, a.Type, a.PublishedAt });

        builder.Entity<Article>()
            .HasOne(a => a.Section)
            .WithMany(s => s.Articles)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Article>()
            .HasOne(a => a.PrimaryTopic)
            .WithMany(t => t.PrimaryArticles)
            .HasForeignKey(a => a.PrimaryTopicId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ArticleTopic>()
            .HasIndex(at => new { at.ArticleId, at.TopicId })
            .IsUnique();

        builder.Entity<ArticleTag>()
            .HasIndex(at => new { at.ArticleId, at.TagId })
            .IsUnique();

        builder.Entity<ArticleContributor>()
            .HasIndex(ac => new { ac.ArticleId, ac.ContributorId, ac.Role })
            .IsUnique();

        builder.Entity<ArticleRelation>()
            .HasOne(ar => ar.SourceArticle)
            .WithMany(a => a.RelatedFrom)
            .HasForeignKey(ar => ar.SourceArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ArticleRelation>()
            .HasOne(ar => ar.RelatedArticle)
            .WithMany(a => a.RelatedTo)
            .HasForeignKey(ar => ar.RelatedArticleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Engagement
        builder.Entity<UserSave>()
            .HasIndex(us => new { us.UserId, us.ArticleId })
            .IsUnique();

        builder.Entity<PageMark>()
            .HasIndex(pm => new { pm.UserId, pm.ArticleId });

        // Enforces 1 active root comment per user per article in PostgreSQL
        var commentIndex = builder.Entity<Comment>()
            .HasIndex(c => new { c.ArticleId, c.UserId })
            .IsUnique();
        if (isNpgsql)
        {
            commentIndex.HasFilter("\"IsDeleted\" = false");
        }

        builder.Entity<CommentReply>()
            .HasOne(r => r.Comment)
            .WithMany(c => c.Replies)
            .HasForeignKey(r => r.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CommentReply>()
            .HasOne(r => r.User)
            .WithMany(u => u.CommentReplies)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CommentLike>()
            .HasIndex(cl => new { cl.UserId, cl.CommentId, cl.ReplyId });

        // Forms
        builder.Entity<NewsletterSubscription>().HasIndex(n => n.Email).IsUnique();
        builder.Entity<StaticPage>().HasIndex(p => p.Slug).IsUnique();
        builder.Entity<FeatureFlag>().HasIndex(f => f.Key).IsUnique();

        // SEO
        builder.Entity<ArticleSeoProfile>()
            .HasOne(s => s.Article)
            .WithOne(a => a.SeoProfile)
            .HasForeignKey<ArticleSeoProfile>(s => s.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ArticleSeoProfile>()
            .HasIndex(s => s.FocusKeyphrase);

        if (isNpgsql)
        {
            builder.Entity<Article>().Property(a => a.DisplaySettingsJson).HasColumnType("jsonb");
            builder.Entity<SyndicationRequest>().Property(s => s.MediumJson).HasColumnType("jsonb");
            builder.Entity<ArticleSeoProfile>().Property(s => s.SynonymsJson).HasColumnType("jsonb");
            builder.Entity<ArticleSeoProfile>().Property(s => s.LastAnalysisJson).HasColumnType("jsonb");
            builder.Entity<RelatedKeyphrase>().Property(r => r.SynonymsJson).HasColumnType("jsonb");
            builder.Entity<RelatedKeyphrase>().Property(r => r.ResultsJson).HasColumnType("jsonb");
            builder.Entity<SeoAnalysisSnapshot>().Property(s => s.ResultsJson).HasColumnType("jsonb");
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AppUser>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

