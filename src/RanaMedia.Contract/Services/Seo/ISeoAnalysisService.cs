using RanaMedia.Contract.DTOs.Seo;

namespace RanaMedia.Contract.Services.Seo;

public interface ISeoAnalysisService
{
    Task<SeoAnalysisResponseDTO> AnalyzeLiveAsync(AnalyzeSeoRequestDTO request, CancellationToken ct = default);
    Task<SeoAnalysisResponseDTO> AnalyzeAndPersistForArticleAsync(Guid articleId, AnalyzeSeoRequestDTO request, CancellationToken ct = default);
    Task<SeoAnalysisResponseDTO?> GetArticleSeoProfileAsync(Guid articleId, CancellationToken ct = default);
    Task<List<KeyphraseUsageItemDTO>> GetKeyphraseUsageAsync(string keyphrase, Guid? excludeArticleId = null, CancellationToken ct = default);
}

