using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using AutoMapper;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Options;
using RanaMedia.Application.Profiles;
using RanaMedia.Application.Services.Editorial;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using RanaMedia.Persistence.Data;
using RanaMedia.Persistence.Repositories.Editorial;
using RanaMedia.Persistence.Repositories.System;
using Xunit;

namespace RanaMedia.Tests;

public class CommentRulesTests
{
    private static (CommentService Service, AppDbContext Db, AppUser User, Article Article) Setup()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"comment_test_{Guid.NewGuid()}")
            .Options;
        var db = new AppDbContext(opts);

        var userStore = new UserStore<AppUser, AppRole, AppDbContext, Guid>(db);
        var userManager = new UserManager<AppUser>(
            userStore,
            Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
            new PasswordHasher<AppUser>(),
            Array.Empty<IUserValidator<AppUser>>(),
            Array.Empty<IPasswordValidator<AppUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            NullLogger<UserManager<AppUser>>.Instance);

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<CustomProfile>(), NullLoggerFactory.Instance).CreateMapper();

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "reader@test.local",
            Email = "reader@test.local",
            FirstName = "Test",
            LastName = "Reader"
        };
        db.Users.Add(user);

        var section = new Section { Slug = "science", Title = "Science", AccentColorHex = "#035a6d" };
        var article = new Article
        {
            Slug = "quantum-reality",
            Title = "Quantum Reality",
            Section = section,
            Status = ArticleStatus.Published,
            CommentsEnabled = true,
            CommentsOpen = true,
            BodyHtml = "<p>Test body</p>"
        };
        db.Sections.Add(section);
        db.Articles.Add(article);
        db.SaveChanges();

        var service = new CommentService(
            new CommentRepository(db),
            new ArticleRepository(db),
            userManager,
            new UnitOfWork(db),
            Microsoft.Extensions.Options.Options.Create(new CommentSettings
            {
                RequireGuidelinesAcceptance = true,
                MaxRootCommentsPerArticlePerUser = 1,
                EditWindowMinutes = 60
            }),
            mapper);

        return (service, db, user, article);
    }

    [Fact]
    public async Task CreateComment_WithoutAcceptingCommunityGuidelines_ThrowsForbidden()
    {
        var (service, _, user, article) = Setup();

        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.CreateCommentAsync(user.Id, article.Slug, new CreateCommentDTO { Body = "Great essay!" }));

        Assert.Equal("COMMUNITY_GUIDELINES_NOT_ACCEPTED", ex.ErrorCode);
    }

    [Fact]
    public async Task CreateComment_SecondRootCommentSameUserSameArticle_ThrowsConflict()
    {
        var (service, db, user, article) = Setup();
        user.CommunityGuidelinesAcceptedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var first = await service.CreateCommentAsync(user.Id, article.Slug, new CreateCommentDTO { Body = "First comment" });
        Assert.NotNull(first);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateCommentAsync(user.Id, article.Slug, new CreateCommentDTO { Body = "Second comment" }));

        Assert.Equal("ONE_COMMENT_PER_ARTICLE", ex.ErrorCode);
    }

    [Fact]
    public async Task UpdateComment_WithinOneHourSucceeds_AfterOneHourThrowsForbidden()
    {
        var (service, db, user, article) = Setup();
        user.CommunityGuidelinesAcceptedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var created = await service.CreateCommentAsync(user.Id, article.Slug, new CreateCommentDTO { Body = "Original thought" });

        // Within 1 hour succeeds
        var updated = await service.UpdateCommentAsync(user.Id, created.Id, new UpdateCommentDTO { Body = "Refined thought" });
        Assert.Equal("Refined thought", updated.BodyHtml);

        // Simulate > 1 hour elapsed
        var entity = await db.Comments.FirstAsync(c => c.Id == created.Id);
        entity.EditableUntil = DateTime.UtcNow.AddMinutes(-1);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.UpdateCommentAsync(user.Id, created.Id, new UpdateCommentDTO { Body = "Too late edit" }));

        Assert.Equal("COMMENT_EDIT_WINDOW_EXPIRED", ex.ErrorCode);
    }
}
