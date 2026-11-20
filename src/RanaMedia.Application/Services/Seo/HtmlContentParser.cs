using System.Net;
using System.Text.RegularExpressions;

namespace RanaMedia.Application.Services.Seo;

public record ParsedParagraph(int Index, string PlainText, string RawHtml);
public record ParsedSubheading(string Level, string PlainText);
public record ParsedLink(string Href, string Text, bool IsInternal, bool IsOutbound);
public record ParsedImage(string Src, string Alt);

public class ParsedHtmlDocument
{
    public string PlainText { get; set; } = string.Empty;
    public List<string> Words { get; set; } = new();
    public List<ParsedParagraph> Paragraphs { get; set; } = new();
    public List<ParsedSubheading> Subheadings { get; set; } = new();
    public List<ParsedLink> Links { get; set; } = new();
    public List<ParsedImage> Images { get; set; } = new();
}

public static partial class HtmlContentParser
{
    [GeneratedRegex(@"<script\b[^>]*>.*?</script>|<style\b[^>]*>.*?</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ScriptStyleRegex();

    [GeneratedRegex(@"<p\b[^>]*>(.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ParagraphRegex();

    [GeneratedRegex(@"<(h[23])\b[^>]*>(.*?)</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex SubheadingRegex();

    [GeneratedRegex(@"<a\b[^>]*?href\s*=\s*[""']([^""']*)[""'][^>]*>(.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex LinkRegex();

    [GeneratedRegex(@"<img\b([^>]*)/?>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ImageRegex();

    [GeneratedRegex(@"src\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase)]
    private static partial Regex SrcAttrRegex();

    [GeneratedRegex(@"alt\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase)]
    private static partial Regex AltAttrRegex();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex AnyTagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultiSpaceRegex();

    [GeneratedRegex(@"[\p{L}\p{N}]+(?:['’-][\p{L}\p{N}]+)*")]
    private static partial Regex WordTokenRegex();

    public static string StripTags(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var withoutScripts = ScriptStyleRegex().Replace(html, " ");
        var withoutTags = AnyTagRegex().Replace(withoutScripts, " ");
        var decoded = WebUtility.HtmlDecode(withoutTags);
        return MultiSpaceRegex().Replace(decoded, " ").Trim();
    }

    public static List<string> TokenizeWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new List<string>();
        return WordTokenRegex().Matches(text).Select(m => m.Value).ToList();
    }

    public static ParsedHtmlDocument Parse(string html, string siteDomain)
    {
        var doc = new ParsedHtmlDocument();
        if (string.IsNullOrWhiteSpace(html)) return doc;

        var cleanHtml = ScriptStyleRegex().Replace(html, " ");
        doc.PlainText = StripTags(cleanHtml);
        doc.Words = TokenizeWords(doc.PlainText);

        // Paragraphs
        var pMatches = ParagraphRegex().Matches(cleanHtml);
        if (pMatches.Count > 0)
        {
            int idx = 0;
            foreach (Match m in pMatches)
            {
                var pText = StripTags(m.Groups[1].Value);
                if (!string.IsNullOrWhiteSpace(pText))
                {
                    doc.Paragraphs.Add(new ParsedParagraph(idx++, pText, m.Value));
                }
            }
        }
        else if (!string.IsNullOrWhiteSpace(doc.PlainText))
        {
            var lines = cleanHtml.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            foreach (var line in lines)
            {
                var t = StripTags(line);
                if (!string.IsNullOrWhiteSpace(t))
                    doc.Paragraphs.Add(new ParsedParagraph(idx++, t, line));
            }
        }

        // H2 / H3 subheadings
        foreach (Match m in SubheadingRegex().Matches(cleanHtml))
        {
            var level = m.Groups[1].Value.ToUpperInvariant();
            var text = StripTags(m.Groups[2].Value);
            if (!string.IsNullOrWhiteSpace(text))
                doc.Subheadings.Add(new ParsedSubheading(level, text));
        }

        // Links
        var cleanDomain = siteDomain.Trim().ToLowerInvariant().TrimStart('.');
        foreach (Match m in LinkRegex().Matches(cleanHtml))
        {
            var href = WebUtility.HtmlDecode(m.Groups[1].Value).Trim();
            var text = StripTags(m.Groups[2].Value);
            if (string.IsNullOrWhiteSpace(href) || href.StartsWith('#') || href.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                continue;

            bool isInternal = false;
            bool isOutbound = false;

            if (href.StartsWith('/') && !href.StartsWith("//"))
            {
                isInternal = true;
            }
            else if (Uri.TryCreate(href, UriKind.Absolute, out var uri) &&
                     (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                var host = uri.Host.ToLowerInvariant();
                if (host == cleanDomain || host.EndsWith("." + cleanDomain))
                    isInternal = true;
                else
                    isOutbound = true;
            }

            doc.Links.Add(new ParsedLink(href, text, isInternal, isOutbound));
        }

        // Images
        foreach (Match m in ImageRegex().Matches(cleanHtml))
        {
            var attrs = m.Groups[1].Value;
            var srcMatch = SrcAttrRegex().Match(attrs);
            var altMatch = AltAttrRegex().Match(attrs);
            var src = srcMatch.Success ? WebUtility.HtmlDecode(srcMatch.Groups[1].Value).Trim() : string.Empty;
            var alt = altMatch.Success ? WebUtility.HtmlDecode(altMatch.Groups[1].Value).Trim() : string.Empty;
            doc.Images.Add(new ParsedImage(src, alt));
        }

        return doc;
    }
}

