using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Domain.Enums;
using RanaMedia.Domain.Repositories.Editorial;
using RanaMedia.Domain.Repositories.System;

namespace RanaMedia.Application.BackgroundServices;

public class PopularScoreAggregationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PopularScoreAggregationService> _logger;

    public PopularScoreAggregationService(IServiceScopeFactory scopeFactory, ILogger<PopularScoreAggregationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                using var scope = _scopeFactory.CreateScope();
                var analytics = scope.ServiceProvider.GetRequiredService<IViewAnalyticsService>();
                await analytics.RecalculatePopularScoresAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PopularScoreAggregationService xətası baş verdi.");
            }
        }
    }
}

public class ScheduledPublishingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledPublishingService> _logger;

    public ScheduledPublishingService(IServiceScopeFactory scopeFactory, ILogger<ScheduledPublishingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                using var scope = _scopeFactory.CreateScope();
                var articleRepo = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var dueArticles = await articleRepo.GetScheduledDueAsync(DateTime.UtcNow, stoppingToken);
                if (dueArticles.Count > 0)
                {
                    foreach (var article in dueArticles)
                    {
                        article.Status = ArticleStatus.Published;
                        article.PublishedAt ??= article.ScheduledAt ?? DateTime.UtcNow;
                    }
                    await unitOfWork.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("{Count} cədvəlli məqalə avtomatik dərc edildi.", dueArticles.Count);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ScheduledPublishingService xətası baş verdi.");
            }
        }
    }
}

