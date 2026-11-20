using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using RanaMedia.Application.Exceptions;
using RanaMedia.Application.Options;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Presentation.Infrastructure;

public class LocalFileStorageService : IMediaStorageService
{
    private readonly MediaStorageSettings _settings;
    private readonly IWebHostEnvironment _env;

    private static readonly Dictionary<MediaFolder, HashSet<string>> AllowedExtensions = new()
    {
        [MediaFolder.Images] = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" },
        [MediaFolder.Avatars] = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" },
        [MediaFolder.Videos] = new(StringComparer.OrdinalIgnoreCase) { ".mp4", ".webm" },
        [MediaFolder.Audio] = new(StringComparer.OrdinalIgnoreCase) { ".mp3", ".m4a", ".wav", ".ogg" }
    };

    public LocalFileStorageService(IOptions<MediaStorageSettings> settings, IWebHostEnvironment env)
    {
        _settings = settings.Value;
        _env = env;
    }

    public async Task<MediaUploadResultDTO> UploadAsync(IFormFile file, MediaFolder folder, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
            throw new BusinessRuleException("Yüklənəcək fayl boşdur.", "EMPTY_FILE");

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions[folder].Contains(ext))
            throw new BusinessRuleException($"Bu fayl formatına icazə verilmir: {ext}", "UNSUPPORTED_MEDIA_TYPE");

        long maxMb = folder switch
        {
            MediaFolder.Videos => _settings.MaxVideoSizeMb,
            MediaFolder.Audio => _settings.MaxAudioSizeMb,
            _ => _settings.MaxImageSizeMb
        };

        if (file.Length > maxMb * 1024L * 1024L)
            throw new BusinessRuleException($"Fayl ölçüsü maksimum {maxMb} MB ola bilər.", "FILE_TOO_LARGE");

        var now = DateTime.UtcNow;
        var folderName = folder.ToString().ToLowerInvariant();
        var webRoot = string.IsNullOrWhiteSpace(_env.WebRootPath)
            ? Path.Combine(_env.ContentRootPath, "wwwroot")
            : _env.WebRootPath;

        var targetDir = Path.Combine(webRoot, "uploads", folderName, now.Year.ToString("0000"), now.Month.ToString("00"));
        Directory.CreateDirectory(targetDir);

        var fileName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
        var fullPath = Path.Combine(targetDir, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        var relativeUrl = $"{_settings.BasePublicPath.TrimEnd('/')}/{folderName}/{now:yyyy}/{now:MM}/{fileName}";
        return new MediaUploadResultDTO
        {
            FileName = fileName,
            RelativeUrl = relativeUrl,
            ContentType = file.ContentType,
            SizeBytes = file.Length
        };
    }

    public Task DeleteAsync(string relativeUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl)) return Task.CompletedTask;

        var webRoot = string.IsNullOrWhiteSpace(_env.WebRootPath)
            ? Path.Combine(_env.ContentRootPath, "wwwroot")
            : _env.WebRootPath;

        var trimmed = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(webRoot, trimmed));
        var uploadsRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads"));

        if (fullPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}

public class CloudflareTurnstileVerifier : ITurnstileVerifier
{
    private readonly TurnstileSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CloudflareTurnstileVerifier> _logger;

    public CloudflareTurnstileVerifier(
        IOptions<TurnstileSettings> settings,
        IHttpClientFactory httpClientFactory,
        ILogger<CloudflareTurnstileVerifier> logger)
    {
        _settings = settings.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string? token, string? remoteIp = null, CancellationToken ct = default)
    {
        if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.SecretKey))
            return true;

        if (string.IsNullOrWhiteSpace(token))
            return false;

        try
        {
            var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _settings.SecretKey,
                ["response"] = token,
                ["remoteip"] = remoteIp ?? string.Empty
            });

            var resp = await client.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content, ct);
            if (!resp.IsSuccessStatusCode) return false;

            var payload = await resp.Content.ReadFromJsonAsync<TurnstileVerifyResponse>(cancellationToken: ct);
            return payload?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Turnstile verification failed.");
            return false;
        }
    }

    private sealed class TurnstileVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}

