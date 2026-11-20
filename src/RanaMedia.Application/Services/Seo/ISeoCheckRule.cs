using System.Text.RegularExpressions;
using RanaMedia.Application.Options;
using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Application.Services.Seo;

public class SeoRuleEvaluation
{
    public SeoCheckItemDTO Check { get; set; } = new();
    public List<SeoHighlightMatchDTO> Highlights { get; set; } = new();
}

public class SeoCheckContext
{
    public AnalyzeSeoRequestDTO Request { get; init; } = null!;
    public ParsedHtmlDocument Document { get; init; } = null!;
    public SeoAnalyzerSettings Settings { get; init; } = new();
    public List<KeyphraseUsageItemDTO> ConflictingArticles { get; init; } = new();

    public string FocusKeyphrase => (Request.FocusKeyphrase ?? string.Empty).Trim();
    public List<string> AllKeyphrases { get; init; } = new();
    public List<string> FocusContentWords { get; init; } = new();
}

public interface ISeoCheckRule
{
    string RuleId { get; }
    bool SupportsRelatedKeyphrase { get; }
    SeoRuleEvaluation Evaluate(SeoCheckContext context);
}

public static class SeoTextMatcher
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "are", "as", "at", "be", "but", "by", "for", "if", "in", "into",
        "is", "it", "no", "not", "of", "on", "or", "such", "that", "the", "their", "then",
        "there", "these", "they", "this", "to", "was", "will", "with", "what", "when", "where",
        "who", "why", "how", "ve", "ile", "bir", "bu", "da", "de", "ki", "ucun", "üçün"
    };

    public static List<string> GetContentWords(string keyphrase)
    {
        return HtmlContentParser.TokenizeWords(keyphrase)
            .Select(w => w.ToLowerInvariant())
            .Where(w => !StopWords.Contains(w))
            .Distinct()
            .ToList();
    }

    public static List<(int Start, int Length, string MatchedText)> FindPhraseMatches(string text, IEnumerable<string> phrases)
    {
        var results = new List<(int Start, int Length, string MatchedText)>();
        if (string.IsNullOrWhiteSpace(text)) return results;

        var validPhrases = phrases
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(p => p.Length)
            .ToList();

        if (validPhrases.Count == 0) return results;

        var pattern = string.Join("|", validPhrases.Select(p => $@"\b{Regex.Escape(p)}\b"));
        var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        foreach (Match m in regex.Matches(text))
        {
            results.Add((m.Index, m.Length, m.Value));
        }

        return results;
    }

    public static bool ContainsAnyPhrase(string text, IEnumerable<string> phrases)
    {
        return FindPhraseMatches(text, phrases).Count > 0;
    }

    public static int EstimateTitlePixelWidth(string title)
    {
        if (string.IsNullOrEmpty(title)) return 0;
        double width = 0;
        foreach (var ch in title)
        {
            if ("iljtI1|!.,:;'` ".Contains(ch)) width += 4.5;
            else if ("mwMWQGOD@%".Contains(ch)) width += 14.5;
            else if (char.IsUpper(ch)) width += 11.5;
            else if (char.IsDigit(ch)) width += 9.5;
            else width += 8.8;
        }
        return (int)Math.Round(width);
    }

    public static SeoCheckItemDTO MakeItem(
        string ruleId,
        string title,
        string description,
        SeoCheckSeverity severity,
        int scoreImpact,
        bool highlightable = false,
        List<string>? matchedTokens = null)
    {
        return new SeoCheckItemDTO
        {
            RuleId = ruleId,
            Title = title,
            Description = description,
            Severity = severity,
            ScoreImpact = scoreImpact,
            Highlightable = highlightable,
            MatchedTokens = matchedTokens ?? new List<string>()
        };
    }
}

