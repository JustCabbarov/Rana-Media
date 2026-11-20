using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RanaMedia.Application.BackgroundServices;
using RanaMedia.Application.Caching;
using RanaMedia.Application.Options;
using RanaMedia.Application.Services.Editorial;
using RanaMedia.Application.Services.Seo;
using RanaMedia.Application.Services.Seo.Rules;
using RanaMedia.Application.Services.System;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Contract.Services.Seo;
using RanaMedia.Contract.Services.System;

namespace RanaMedia.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Options
        services.Configure<JwtOption>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<MediaStorageSettings>(configuration.GetSection("MediaStorageSettings"));
        services.Configure<CommentSettings>(configuration.GetSection("CommentSettings"));
        services.Configure<SeoAnalyzerSettings>(configuration.GetSection("SeoAnalyzerSettings"));
        services.Configure<TurnstileSettings>(configuration.GetSection("TurnstileSettings"));

        // Caching
        services.AddMemoryCache(options => options.SizeLimit = 10_000);
        services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();

        // System Services (AzercardReportingSystem pattern)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenHandler, TokenHandler>();
        services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
        services.AddScoped<IPasswordResetService, PasswordResetService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<INotificationService, NotificationService>();

        // Editorial & Engagement Services
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ISectionService, SectionService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IContributorService, ContributorService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<IPageMarkService, PageMarkService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<INewsletterService, NewsletterService>();
        services.AddScoped<IPitchService, PitchService>();
        services.AddScoped<ISyndicationService, SyndicationService>();
        services.AddScoped<IViewAnalyticsService, ViewAnalyticsService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();

        // Yoast-style SEO Analysis Engine & 15 Rules
        services.AddScoped<ISeoCheckRule, KeyphraseLengthRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseInSeoTitleRule>();
        services.AddScoped<ISeoCheckRule, SeoTitleWidthRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseInSlugRule>();
        services.AddScoped<ISeoCheckRule, MetaDescriptionLengthRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseInMetaDescriptionRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseInIntroductionRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseDensityRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseDistributionRule>();
        services.AddScoped<ISeoCheckRule, KeyphraseInSubheadingsRule>();
        services.AddScoped<ISeoCheckRule, TextLengthRule>();
        services.AddScoped<ISeoCheckRule, OutboundLinksRule>();
        services.AddScoped<ISeoCheckRule, InternalLinksRule>();
        services.AddScoped<ISeoCheckRule, ImagesAndAltRule>();
        services.AddScoped<ISeoCheckRule, PreviouslyUsedKeyphraseRule>();
        services.AddScoped<ISeoAnalysisService, SeoAnalysisService>();

        // Background Services
        services.AddHostedService<PopularScoreAggregationService>();
        services.AddHostedService<ScheduledPublishingService>();

        return services;
    }
}

