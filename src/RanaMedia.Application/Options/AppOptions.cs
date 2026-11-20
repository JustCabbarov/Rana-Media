namespace RanaMedia.Application.Options;

public class JwtOption
{
    public string Issuer { get; set; } = "RanaMedia.API";
    public string Audience { get; set; } = "RanaMedia.Client";
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string FromName { get; set; } = "Rana Media";
    public string FromEmail { get; set; } = "noreply@ranamedia.local";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class MediaStorageSettings
{
    public string RootFolder { get; set; } = "wwwroot/uploads";
    public string BasePublicPath { get; set; } = "/uploads";
    public int MaxImageSizeMb { get; set; } = 10;
    public int MaxVideoSizeMb { get; set; } = 25;
    public int MaxAudioSizeMb { get; set; } = 100;
}

public class CommentSettings
{
    public bool RequireGuidelinesAcceptance { get; set; } = true;
    public int MaxRootCommentsPerArticlePerUser { get; set; } = 1;
    public int EditWindowMinutes { get; set; } = 60;
}

public class SeoAnalyzerSettings
{
    public string SiteDomain { get; set; } = "ranamedia.co";
    public int TitleMaxPixelWidth { get; set; } = 600;
    public int MetaDescriptionMinLength { get; set; } = 120;
    public int MetaDescriptionMaxLength { get; set; } = 156;
    public int MinRecommendedWordCount { get; set; } = 300;
    public double MinDensityPercent { get; set; } = 0.5;
    public double MaxDensityPercent { get; set; } = 3.0;
}

public class TurnstileSettings
{
    public bool Enabled { get; set; } = false;
    public string SecretKey { get; set; } = string.Empty;
}

