using RanaMedia.Contract.DTOs.Seo;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Application.Services.Seo.Rules;

public class KeyphraseLengthRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_length";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase length",
                    "No focus keyphrase was set for this page. Set a keyphrase in order to calculate your SEO score.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var wordCount = context.FocusContentWords.Count > 0
            ? context.FocusContentWords.Count
            : HtmlContentParser.TokenizeWords(context.FocusKeyphrase).Count;

        if (wordCount <= 4)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase length", "Good job!", SeoCheckSeverity.Good, 8)
            };
        }

        if (wordCount <= 6)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase length",
                    $"Your keyphrase contains {wordCount} content words. Consider narrowing it down to 4 words or fewer.",
                    SeoCheckSeverity.Improvement, 4)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase length",
                $"Your keyphrase is {wordCount} words long, which is longer than the recommended maximum of 4 content words.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}

public class KeyphraseInSeoTitleRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_in_seo_title";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var title = (context.Request.SeoTitle ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase) || string.IsNullOrWhiteSpace(title))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in SEO title",
                    "Not all the words from your keyphrase appear in the SEO title.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var exactMatches = SeoTextMatcher.FindPhraseMatches(title, new[] { context.FocusKeyphrase });
        if (exactMatches.Count > 0 && exactMatches[0].Start <= 3)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in SEO title",
                    "The exact match of the focus keyphrase appears at the beginning of the SEO title. Good job!",
                    SeoCheckSeverity.Good, 9, matchedTokens: new List<string> { exactMatches[0].MatchedText })
            };
        }

        if (exactMatches.Count > 0 || SeoTextMatcher.ContainsAnyPhrase(title, context.AllKeyphrases))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in SEO title",
                    "The focus keyphrase appears in the SEO title, but not at the beginning. Try moving it closer to the start.",
                    SeoCheckSeverity.Improvement, 6)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in SEO title",
                "The exact match of the focus keyphrase does not appear in the SEO title.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}

public class SeoTitleWidthRule : ISeoCheckRule
{
    public string RuleId => "seo_title_width";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var title = (context.Request.SeoTitle ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "SEO title width",
                    "No SEO title has been specified. Add a concise title.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var pixelWidth = SeoTextMatcher.EstimateTitlePixelWidth(title);
        if (pixelWidth > context.Settings.TitleMaxPixelWidth)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "SEO title width",
                    "The SEO title is wider than the viewable limit. Try to make it shorter.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        if (pixelWidth < 180)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "SEO title width",
                    "The SEO title is quite short. You have room to add more descriptive context.",
                    SeoCheckSeverity.Improvement, 4)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "SEO title width", "Good job!", SeoCheckSeverity.Good, 7)
        };
    }
}

public class KeyphraseInSlugRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_in_slug";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var slug = (context.Request.Slug ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase) || string.IsNullOrWhiteSpace(slug))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in slug",
                    "More than half of your keyphrase should appear in the slug.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var words = context.FocusContentWords.Count > 0
            ? context.FocusContentWords
            : HtmlContentParser.TokenizeWords(context.FocusKeyphrase).Select(w => w.ToLowerInvariant()).ToList();

        var matched = words.Count(w => slug.Contains(w, StringComparison.OrdinalIgnoreCase));
        var ratio = words.Count > 0 ? (double)matched / words.Count : 0;

        if (ratio > 0.5)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in slug",
                    "More than half of your keyphrase appears in the slug. That's great!",
                    SeoCheckSeverity.Good, 7)
            };
        }

        if (matched > 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in slug",
                    "(Part of) your keyphrase does not appear in the slug. Change that!",
                    SeoCheckSeverity.Improvement, 3)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in slug",
                "Your keyphrase does not appear in the slug. Include keyphrase words in the URL slug.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}

public class MetaDescriptionLengthRule : ISeoCheckRule
{
    public string RuleId => "meta_description_length";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var meta = (context.Request.MetaDescription ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(meta))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Meta description length",
                    "No meta description has been specified. Search engines will display copy from the page instead. Make sure to write one!",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        if (meta.Length < context.Settings.MetaDescriptionMinLength)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Meta description length",
                    $"The meta description is too short ({meta.Length} characters). Aim for {context.Settings.MetaDescriptionMinLength}-{context.Settings.MetaDescriptionMaxLength} characters.",
                    SeoCheckSeverity.Improvement, 4)
            };
        }

        if (meta.Length > context.Settings.MetaDescriptionMaxLength)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Meta description length",
                    $"The meta description is over {context.Settings.MetaDescriptionMaxLength} characters ({meta.Length}). Reducing the length will prevent truncation.",
                    SeoCheckSeverity.Improvement, 4)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Meta description length", "Well done!", SeoCheckSeverity.Good, 7)
        };
    }
}

public class KeyphraseInMetaDescriptionRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_in_meta_description";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var meta = (context.Request.MetaDescription ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(meta) || string.IsNullOrWhiteSpace(context.FocusKeyphrase))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in meta description",
                    "The meta description does not contain the focus keyphrase.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var count = SeoTextMatcher.FindPhraseMatches(meta, context.AllKeyphrases).Count;
        if (count is >= 1 and <= 2)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in meta description",
                    "Keyphrase or synonym appears in the meta description. Well done!",
                    SeoCheckSeverity.Good, 6)
            };
        }

        if (count > 2)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in meta description",
                    $"The focus keyphrase appears {count} times in the meta description. Keep it natural (1-2 times).",
                    SeoCheckSeverity.Improvement, 3)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in meta description",
                "The meta description has been specified, but it does not contain the keyphrase.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}

public class KeyphraseInIntroductionRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_in_introduction";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase) || context.Document.Paragraphs.Count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in introduction",
                    "Your keyphrase or its synonyms do not appear in the first paragraph. Make sure the topic is clear immediately.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var firstPara = context.Document.Paragraphs[0];
        var matches = SeoTextMatcher.FindPhraseMatches(firstPara.PlainText, context.AllKeyphrases);
        if (matches.Count > 0)
        {
            var highlights = matches.Select(m => new SeoHighlightMatchDTO
            {
                RuleId = RuleId,
                ParagraphIndex = firstPara.Index,
                StartOffset = m.Start,
                Length = m.Length,
                MatchedText = m.MatchedText,
                ContextSnippet = BuildSnippet(firstPara.PlainText, m.Start, m.Length)
            }).ToList();

            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in introduction",
                    "Well done! Your keyphrase or its synonyms appear in the first paragraph.",
                    SeoCheckSeverity.Good, 8, highlightable: true, matchedTokens: matches.Select(m => m.MatchedText).Distinct().ToList()),
                Highlights = highlights
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in introduction",
                "Your keyphrase or its synonyms do not appear in the first paragraph. Make sure the topic is clear immediately.",
                SeoCheckSeverity.Problem, 0)
        };
    }

    internal static string BuildSnippet(string text, int start, int length)
    {
        var s = Math.Max(0, start - 35);
        var e = Math.Min(text.Length, start + length + 35);
        return text[s..e];
    }
}

public class KeyphraseDensityRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_density";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var wordCount = context.Document.Words.Count;
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase) || wordCount == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase density",
                    "The focus keyphrase was found 0 times. Add it naturally throughout your text.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var highlights = new List<SeoHighlightMatchDTO>();
        foreach (var para in context.Document.Paragraphs)
        {
            var matches = SeoTextMatcher.FindPhraseMatches(para.PlainText, context.AllKeyphrases);
            foreach (var m in matches)
            {
                highlights.Add(new SeoHighlightMatchDTO
                {
                    RuleId = RuleId,
                    ParagraphIndex = para.Index,
                    StartOffset = m.Start,
                    Length = m.Length,
                    MatchedText = m.MatchedText,
                    ContextSnippet = KeyphraseInIntroductionRule.BuildSnippet(para.PlainText, m.Start, m.Length)
                });
            }
        }

        var count = highlights.Count;
        if (count == 0)
        {
            count = SeoTextMatcher.FindPhraseMatches(context.Document.PlainText, context.AllKeyphrases).Count;
        }

        var (minRec, maxRec) = GetRecommendedBounds(wordCount, context.Settings);
        if (count < minRec)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase density",
                    $"The keyphrase was found {count} times. That's less than the recommended minimum of {minRec} times for a text of this length. Focus on your keyphrase!",
                    SeoCheckSeverity.Problem, 0, highlightable: count > 0,
                    matchedTokens: highlights.Select(h => h.MatchedText).Distinct().ToList()),
                Highlights = highlights
            };
        }

        if (count > maxRec)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase density",
                    $"The keyphrase was found {count} times. That's more than the recommended maximum of {maxRec} times for a text of this length.",
                    SeoCheckSeverity.Problem, 0, highlightable: true,
                    matchedTokens: highlights.Select(h => h.MatchedText).Distinct().ToList()),
                Highlights = highlights
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase density",
                $"The focus keyphrase was found {count} times. This is great!",
                SeoCheckSeverity.Good, 9, highlightable: true,
                matchedTokens: highlights.Select(h => h.MatchedText).Distinct().ToList()),
            Highlights = highlights
        };
    }

    public static (int MinRecommended, int MaxRecommended) GetRecommendedBounds(int wordCount, Options.SeoAnalyzerSettings settings)
    {
        if (wordCount <= 0) return (1, 2);
        var minRec = Math.Max(2, (int)Math.Floor(wordCount / 300.0) + 1);
        var maxRec = Math.Max(minRec + 2, (int)Math.Ceiling(wordCount * (settings.MaxDensityPercent / 100.0)));
        return (minRec, maxRec);
    }
}

public class KeyphraseDistributionRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_distribution";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var paragraphs = context.Document.Paragraphs;
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase) || paragraphs.Count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase distribution",
                    "Have you evenly distributed your focus keyphrase throughout the whole text? Add your keyphrase across all sections.",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        // Split paragraphs into up to 4 segments (quartiles) to measure even distribution
        int segments = Math.Min(4, paragraphs.Count);
        int matchedSegments = 0;
        for (int s = 0; s < segments; s++)
        {
            int startIdx = s * paragraphs.Count / segments;
            int endIdx = (s + 1) * paragraphs.Count / segments;
            var segmentText = string.Join(" ", paragraphs.Skip(startIdx).Take(Math.Max(1, endIdx - startIdx)).Select(p => p.PlainText));
            if (SeoTextMatcher.ContainsAnyPhrase(segmentText, context.AllKeyphrases))
                matchedSegments++;
        }

        var totalMatches = SeoTextMatcher.FindPhraseMatches(context.Document.PlainText, context.AllKeyphrases).Count;

        if (matchedSegments >= Math.Min(3, segments) && totalMatches >= 3)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase distribution",
                    "Great! Your keyphrase or its synonyms are evenly distributed throughout the text.",
                    SeoCheckSeverity.Good, 7, highlightable: true)
            };
        }

        if (matchedSegments >= 2 && totalMatches >= 3)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase distribution",
                    "Large parts of your text do not include the keyphrase or its synonyms. Try distributing them more evenly.",
                    SeoCheckSeverity.Improvement, 3)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase distribution",
                "Have you evenly distributed your focus keyphrase throughout the whole text? Spread occurrences evenly across your article.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}

public class KeyphraseInSubheadingsRule : ISeoCheckRule
{
    public string RuleId => "keyphrase_in_subheading";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var headings = context.Document.Subheadings;
        if (headings.Count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in subheading",
                    "Use more keyphrases or synonyms in your H2 and H3 subheadings!",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        int matchedCount = headings.Count(h => SeoTextMatcher.ContainsAnyPhrase(h.PlainText, context.AllKeyphrases));
        double ratio = (double)matchedCount / headings.Count;

        if (matchedCount == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in subheading",
                    "Use more keyphrases or synonyms in your H2 and H3 subheadings!",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        if (ratio > 0.75 && headings.Count > 1)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in subheading",
                    $"More than 75% of your H2 and H3 subheadings ({matchedCount}/{headings.Count}) reflect the topic. Consider varying some headings.",
                    SeoCheckSeverity.Improvement, 4)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Keyphrase in subheading",
                $"{matchedCount} of your {headings.Count} subheadings reflect the topic of your copy. Good job!",
                SeoCheckSeverity.Good, 7)
        };
    }
}

public class TextLengthRule : ISeoCheckRule
{
    public string RuleId => "text_length";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var wordCount = context.Document.Words.Count;
        var minWords = context.Settings.MinRecommendedWordCount;

        if (wordCount >= minWords)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Text length",
                    $"The text contains {wordCount} words. Good job!",
                    SeoCheckSeverity.Good, 8)
            };
        }

        if (wordCount >= 200)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Text length",
                    $"The text contains {wordCount} words. This is slightly below the recommended minimum of {minWords} words.",
                    SeoCheckSeverity.Improvement, 3)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Text length",
                $"The text contains {wordCount} words. This is far below the recommended minimum of {minWords} words.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}

public class OutboundLinksRule : ISeoCheckRule
{
    public string RuleId => "outbound_links";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var count = context.Document.Links.Count(l => l.IsOutbound);
        if (count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Outbound links",
                    "No outbound links appear in this page. Add some!",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Outbound links",
                $"There are {count} outbound link(s) on this page. Good job!",
                SeoCheckSeverity.Good, 6)
        };
    }
}

public class InternalLinksRule : ISeoCheckRule
{
    public string RuleId => "internal_links";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var count = context.Document.Links.Count(l => l.IsInternal);
        if (count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Internal links",
                    "No internal links appear in this page, make sure to add some!",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Internal links",
                $"You have {count} internal link(s) on this page. Good job!",
                SeoCheckSeverity.Good, 6)
        };
    }
}

public class ImagesAndAltRule : ISeoCheckRule
{
    public string RuleId => "images_and_alt";
    public bool SupportsRelatedKeyphrase => true;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        var images = new List<ParsedImage>(context.Document.Images);
        if (!string.IsNullOrWhiteSpace(context.Request.FeaturedImageUrl))
        {
            images.Add(new ParsedImage(context.Request.FeaturedImageUrl, context.Request.FeaturedImageAlt ?? string.Empty));
        }

        if (images.Count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Images",
                    "No images appear on this page. Add some!",
                    SeoCheckSeverity.Problem, 0)
            };
        }

        var withAltKeyphrase = images.Count(img =>
            !string.IsNullOrWhiteSpace(img.Alt) &&
            SeoTextMatcher.ContainsAnyPhrase(img.Alt, context.AllKeyphrases));

        if (withAltKeyphrase > 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Image Keyphrase",
                    $"Good job! {withAltKeyphrase} of {images.Count} image(s) have alt attributes reflecting your topic.",
                    SeoCheckSeverity.Good, 7)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Image Keyphrase",
                "Images appear on this page, but none have alt attributes containing your focus keyphrase or synonyms.",
                SeoCheckSeverity.Improvement, 3)
        };
    }
}

public class PreviouslyUsedKeyphraseRule : ISeoCheckRule
{
    public string RuleId => "previously_used_keyphrase";
    public bool SupportsRelatedKeyphrase => false;

    public SeoRuleEvaluation Evaluate(SeoCheckContext context)
    {
        if (string.IsNullOrWhiteSpace(context.FocusKeyphrase))
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Previously used keyphrase",
                    "Set a focus keyphrase to check for duplicate usage across articles.",
                    SeoCheckSeverity.Improvement, 0)
            };
        }

        var count = context.ConflictingArticles.Count;
        if (count == 0)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Previously used keyphrase",
                    "You've not used this keyphrase before, very good.",
                    SeoCheckSeverity.Good, 8)
            };
        }

        if (count == 1)
        {
            return new SeoRuleEvaluation
            {
                Check = SeoTextMatcher.MakeItem(RuleId, "Previously used keyphrase",
                    $"You've used this keyphrase once before ({context.ConflictingArticles[0].Slug}). Consider using a unique focus keyphrase.",
                    SeoCheckSeverity.Improvement, 3)
            };
        }

        return new SeoRuleEvaluation
        {
            Check = SeoTextMatcher.MakeItem(RuleId, "Previously used keyphrase",
                $"You've used this focus keyphrase {count} times before! Avoid keyword cannibalization.",
                SeoCheckSeverity.Problem, 0)
        };
    }
}
