using Microsoft.Extensions.DependencyInjection;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;
using RanaMedia.Persistence.Repositories.Editorial;
using RanaMedia.Persistence.Repositories.System;

namespace RanaMedia.Persistence.Extension;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IContributorRepository, ContributorRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserSaveRepository, UserSaveRepository>();
        services.AddScoped<IPageMarkRepository, PageMarkRepository>();
        services.AddScoped<ISeoRepository, SeoRepository>();

        return services;
    }
}

