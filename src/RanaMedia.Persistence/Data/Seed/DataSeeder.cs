using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Persistence.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedSectionsAndTopicsAsync(context);
        await SeedStaticPagesAsync(context);
        await SeedSampleEditorialContentAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
    {
        var roles = new[]
        {
            ("Admin", "Full system administration access"),
            ("Editor", "Editorial publishing, SEO analysis, and pitch review"),
            ("Moderator", "Comment moderation and community guidelines enforcement"),
            ("Author", "Article contributor and author replies"),
            ("User", "Standard reader account")
        };

        foreach (var (name, desc) in roles)
        {
            if (!await roleManager.RoleExistsAsync(name))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = name,
                    Description = desc,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<AppUser> userManager)
    {
        const string adminEmail = "admin@ranamedia.co";
        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing != null) return;

        var admin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Rana",
            LastName = "Admin",
            IsActive = true,
            CommunityGuidelinesAcceptedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Editor", "Moderator" });
        }
    }

    private static async Task SeedSectionsAndTopicsAsync(AppDbContext context)
    {
        if (await context.Sections.AnyAsync()) return;

        var catalog = new List<(string Slug, string Title, string Color, string Desc, int Order, (string Slug, string Title)[] Topics)>
        {
            ("philosophy", "Philosophy", "#0f5c43", "Philosophy essays and videos exploring metaphysics, ethics, knowledge, and the good life.", 1, new[]
            {
                ("beauty-aesthetics", "Beauty and aesthetics"),
                ("bioethics", "Bioethics"),
                ("comparative-philosophy", "Comparative philosophy"),
                ("cosmopolitanism", "Cosmopolitanism"),
                ("death", "Death"),
                ("ethics", "Ethics"),
                ("history-of-ideas", "History of ideas"),
                ("knowledge", "Knowledge"),
                ("logic-and-probability", "Logic and probability"),
                ("meaning-the-good-life", "Meaning and the good life"),
                ("metaphysics", "Metaphysics"),
                ("philosophy-of-language", "Philosophy of language"),
                ("philosophy-of-mind", "Philosophy of mind"),
                ("philosophy-of-religion", "Philosophy of religion"),
                ("philosophy-of-science", "Philosophy of science"),
                ("political-philosophy", "Political philosophy"),
                ("thinkers-and-theories", "Thinkers and theories"),
                ("values-beliefs", "Values and beliefs"),
                ("virtues-and-vices", "Virtues and vices")
            }),
            ("science", "Science", "#115e59", "Science essays and documentaries across physics, biology, cosmology, AI, and medicine.", 2, new[]
            {
                ("astronomy", "Astronomy"),
                ("biology", "Biology"),
                ("biotechnology", "Biotechnology"),
                ("chemistry", "Chemistry"),
                ("complexity", "Complexity"),
                ("computing-artificial-intelligence", "Computing and AI"),
                ("cosmology", "Cosmology"),
                ("deep-time", "Deep time"),
                ("earth-science-and-climate", "Earth science and climate"),
                ("ecology-environmental-sciences", "Ecology and environmental sciences"),
                ("engineering", "Engineering"),
                ("evolution", "Evolution"),
                ("genetics", "Genetics"),
                ("history-of-science", "History of science"),
                ("human-evolution", "Human evolution"),
                ("human-reproduction", "Human reproduction"),
                ("illness-disease", "Illness and disease"),
                ("mathematics", "Mathematics"),
                ("medicine", "Medicine"),
                ("oceans-and-water", "Oceans and water"),
                ("palaeontology", "Palaeontology"),
                ("physics", "Physics"),
                ("quantum-theory", "Quantum theory"),
                ("space-exploration", "Space exploration")
            }),
            ("psychology", "Psychology", "#2d6a4f", "Psychology essays and films examining consciousness, cognition, emotion, and wellbeing.", 3, new[]
            {
                ("addiction", "Addiction"),
                ("ageing-and-death", "Ageing and death"),
                ("childhood-adolescence", "Childhood and adolescence"),
                ("cognition-intelligence", "Cognition and intelligence"),
                ("consciousness-altered-states", "Consciousness and altered states"),
                ("family-life", "Family life"),
                ("gender-and-identity", "Gender and identity"),
                ("language-and-linguistics", "Language and linguistics"),
                ("life-stages", "Life stages"),
                ("love-friendship", "Love and friendship"),
                ("mental-health", "Mental health"),
                ("mood-emotion", "Mood and emotion"),
                ("neurodiversity", "Neurodiversity"),
                ("neuroscience", "Neuroscience"),
                ("personality", "Personality"),
                ("pleasure-and-pain", "Pleasure and pain"),
                ("psychiatry-psychotherapy", "Psychiatry and psychotherapy"),
                ("self-improvement", "Self-improvement"),
                ("sex-and-sexuality", "Sex and sexuality"),
                ("sleep-dreams", "Sleep and dreams"),
                ("social-psychology", "Social psychology"),
                ("spirituality", "Spirituality"),
                ("teaching-and-learning", "Teaching and learning"),
                ("technology-the-self", "Technology and the self"),
                ("wellbeing", "Wellbeing")
            }),
            ("society", "Society", "#14532d", "Society essays and stories covering history, politics, anthropology, technology, and justice.", 4, new[]
            {
                ("animals-and-humans", "Animals and humans"),
                ("anthropology", "Anthropology"),
                ("archaeology", "Archaeology"),
                ("automation-robotics", "Automation and robotics"),
                ("cities", "Cities"),
                ("demography-migration", "Demography and migration"),
                ("economic-history", "Economic history"),
                ("economics", "Economics"),
                ("education", "Education"),
                ("environmental-history", "Environmental history"),
                ("fairness-equality", "Fairness and equality"),
                ("future-of-technology", "Future of technology"),
                ("gender", "Gender"),
                ("global-history", "Global history"),
                ("history", "History"),
                ("history-of-technology", "History of technology"),
                ("human-rights-and-justice", "Human rights and justice"),
                ("information-and-communication", "Information and communication"),
                ("making", "Making"),
                ("nations-and-empires", "Nations and empires"),
                ("politics-government", "Politics and government"),
                ("poverty-development", "Poverty and development"),
                ("progress-modernity", "Progress and modernity"),
                ("public-health", "Public health"),
                ("race-and-ethnicity", "Race and ethnicity"),
                ("religion", "Religion"),
                ("the-ancient-world", "The ancient world"),
                ("the-environment", "The environment"),
                ("the-future", "The future"),
                ("war-and-peace", "War and peace"),
                ("work", "Work")
            }),
            ("culture", "Culture", "#4a6b22", "Culture essays and films exploring art, literature, music, architecture, and travel.", 5, new[]
            {
                ("architecture", "Architecture"),
                ("art", "Art"),
                ("biography-and-memoir", "Biography and memoir"),
                ("dance-theatre", "Dance and theatre"),
                ("design-fashion", "Design and fashion"),
                ("film-and-visual-culture", "Film and visual culture"),
                ("food-drink", "Food and drink"),
                ("home", "Home"),
                ("music", "Music"),
                ("nature-and-landscape", "Nature and landscape"),
                ("rituals-celebrations", "Rituals and celebrations"),
                ("sports-games", "Sports and games"),
                ("stories-literature", "Stories and literature"),
                ("subcultures", "Subcultures"),
                ("travel", "Travel")
            })
        };

        foreach (var sec in catalog)
        {
            var sectionEntity = new Section
            {
                Slug = sec.Slug,
                Title = sec.Title,
                Description = sec.Desc,
                MetaDescription = sec.Desc,
                AccentColorHex = sec.Color,
                SortOrder = sec.Order
            };

            foreach (var (tSlug, tTitle) in sec.Topics)
            {
                sectionEntity.Topics.Add(new Topic
                {
                    Slug = tSlug,
                    Title = tTitle,
                    Description = $"{tTitle} essays and videos in {sec.Title}.",
                    MetaDescription = $"Explore {tTitle} on Rana Media."
                });
            }

            await context.Sections.AddAsync(sectionEntity);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedStaticPagesAsync(AppDbContext context)
    {
        if (await context.StaticPages.AnyAsync()) return;

        var pages = new[]
        {
            new StaticPage
            {
                Slug = "about",
                Title = "About",
                BodyHtml = "<p>Rana Media is an independent digital magazine of ideas, philosophy, science, psychology, society, and culture.</p>"
            },
            new StaticPage
            {
                Slug = "community-guidelines",
                Title = "Community guidelines",
                BodyHtml = "<p>We welcome your considered, civil comments in response to this article. You are able to make one comment per article, and you can edit your comment within one hour.</p><ul><li>No hate speech, personal attacks, bullying, or harassment.</li><li>No spam, self-promotion, or off-topic remarks.</li></ul>"
            },
            new StaticPage
            {
                Slug = "republish",
                Title = "Republish",
                BodyHtml = "<p>Essays licensed under our Permissive License may be republished after submitting a syndication request.</p>"
            },
            new StaticPage
            {
                Slug = "people",
                Title = "People",
                BodyHtml = "<p>Meet the editorial and engineering team behind Rana Media.</p>"
            },
            new StaticPage
            {
                Slug = "contact",
                Title = "Contact",
                BodyHtml = "<p>Reach our editorial desk at editorial@ranamedia.co.</p>"
            },
            new StaticPage
            {
                Slug = "support",
                Title = "Support",
                BodyHtml = "<p>Frequently asked questions about reader accounts, newsletters, comments, and syndication.</p>"
            },
            new StaticPage
            {
                Slug = "privacy",
                Title = "Privacy Policy",
                BodyHtml = "<p>Your privacy and reading preferences are protected.</p>"
            },
            new StaticPage
            {
                Slug = "newsletter-privacy",
                Title = "Newsletter Privacy Policy",
                BodyHtml = "<p>We send daily and weekly curated newsletters only with your explicit consent.</p>"
            },
            new StaticPage
            {
                Slug = "terms-of-use",
                Title = "Terms of Use",
                BodyHtml = "<p>Terms governing the use of Rana Media services and publications.</p>"
            }
        };

        await context.StaticPages.AddRangeAsync(pages);
        await context.CallToActions.AddAsync(new CallToAction
        {
            Kind = "newsletter",
            Title = "Sign up to our newsletter",
            BodyHtml = "<p>Updates on everything new — daily essays and weekly highlights.</p>",
            ButtonText = "Subscribe",
            ButtonUrl = "/newsletter"
        });
        await context.FeatureFlags.AddRangeAsync(
            new FeatureFlag { Key = "comments_enabled", IsEnabled = true, Description = "Enable reader comments globally" },
            new FeatureFlag { Key = "pagemarks_enabled", IsEnabled = true, Description = "Enable highlights and annotations" }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedSampleEditorialContentAsync(AppDbContext context)
    {
        if (await context.Articles.AnyAsync()) return;

        var philosophy = await context.Sections.Include(s => s.Topics).FirstAsync(s => s.Slug == "philosophy");
        var metaphysics = philosophy.Topics.First(t => t.Slug == "metaphysics");
        var ethics = philosophy.Topics.First(t => t.Slug == "ethics");

        var science = await context.Sections.Include(s => s.Topics).FirstAsync(s => s.Slug == "science");
        var biology = science.Topics.First(t => t.Slug == "biology");

        var psychology = await context.Sections.Include(s => s.Topics).FirstAsync(s => s.Slug == "psychology");
        var moodEmotion = psychology.Topics.First(t => t.Slug == "mood-emotion");

        var society = await context.Sections.Include(s => s.Topics).FirstAsync(s => s.Slug == "society");
        var anthropology = society.Topics.First(t => t.Slug == "anthropology");

        var culture = await context.Sections.Include(s => s.Topics).FirstAsync(s => s.Slug == "culture");
        var art = culture.Topics.First(t => t.Slug == "art");

        var author1 = new Contributor
        {
            Slug = "leyla-aliyeva",
            Name = "Leyla Aliyeva",
            Occupation = "Associate Professor of Philosophy",
            Company = "Baku Ideas Institute",
            ShortBioHtml = "<p>is a philosopher researching metaphysics and the epistemology of simplicity.</p>",
            BioHtml = "<p>Leyla Aliyeva writes on existence, complexity, and epistemology.</p>"
        };
        var author2 = new Contributor
        {
            Slug = "derek-van-zoonen",
            Name = "Derek van Zoonen",
            Occupation = "Philosopher & Psychologist",
            Company = "University of Groningen",
            ShortBioHtml = "<p>researches emotional phenomenology, grief, and moral psychology.</p>",
            BioHtml = "<p>Derek van Zoonen explores how unspoken emotions shape self-knowledge.</p>"
        };
        var author3 = new Contributor
        {
            Slug = "lhamsuren-munkh-erdene",
            Name = "Lhamsuren Munkh-Erdene",
            Occupation = "Historical Anthropologist",
            Company = "Institute for Advanced Study",
            ShortBioHtml = "<p>studies Eurasian pastoralist polities, mobility, and state formation.</p>",
            BioHtml = "<p>Lhamsuren Munkh-Erdene investigates the origins of nomadic empires.</p>"
        };
        var editor = new Contributor
        {
            Slug = "sam-dresser",
            Name = "Sam Dresser",
            Occupation = "Senior Editor",
            Company = "Rana Media",
            ShortBioHtml = "<p>is Senior Editor at Rana Media.</p>",
            BioHtml = "<p>Sam Dresser commissions and edits philosophy and science essays.</p>"
        };
        await context.Contributors.AddRangeAsync(author1, author2, author3, editor);

        var essayBody = """
            <p><span class="ld-dropcap">W</span>hy is there something rather than nothing? This foundational metaphysical question invites us to explore simplicity, existence, and the architecture of reality. When properly considered, the ordinary fact that things exist shifts from seeming trivial to profoundly astonishing.</p>
            <h2>The puzzle of non-ontological truths</h2>
            <p>Even if nothing existed, mathematical and logical truths would remain coherent. Understanding why existence is simpler than emptiness challenges centuries of philosophical assumptions. See also our <a href="#/section/philosophy/metaphysics">Metaphysics collection</a> and the <a href="https://plato.stanford.edu" target="_blank" rel="noopener">Stanford Encyclopedia of Philosophy</a>.</p>
            <p class="pullquote">There would have been a whole architecture of non-ontological truths even if nothing had existed</p>
            <p>Many proposed explanations for why there is something rather than nothing never confront the intuition that pure emptiness ought to be simpler and easier than a cosmos teeming with galaxies and minds.</p>
            <blockquote>No theory, no conceptual scheme, can explain the existence of anything without assuming the existence of something.</blockquote>
            <img src="/uploads/images/sample-cosmos.svg" alt="Why anything exists metaphysical cosmos illustration" />
            <h3>Simplicity as an explanatory bridge</h3>
            <p>When a single fundamental principle accounts for countless contingent facts, existence becomes the most economical hypothesis available to philosophy and physics alike.</p>
            """;

        var essay = new Article
        {
            Slug = "why-anything-exists-instead-of-nothing",
            Title = "Why anything exists",
            Type = ArticleType.Essay,
            Status = ArticleStatus.Published,
            StandfirstShort = "Here is a fresh philosophical inquiry into why reality exists rather than pure emptiness",
            StandfirstLong = "Here is a fresh philosophical inquiry into why reality exists rather than pure emptiness — and why nothingness is more complex than it seems.",
            BodyHtml = essayBody,
            BodyPlainText = "Why is there something rather than nothing? This foundational metaphysical question invites us to explore simplicity, existence, and the architecture of reality.",
            MetaTitle = "Why anything exists | Rana Media Essays",
            MetaDescription = "Explore a fresh metaphysical answer to why there is something rather than nothing in this longform philosophical essay.",
            SectionId = philosophy.Id,
            PrimaryTopicId = metaphysics.Id,
            WordCount = 2400,
            ReadingTimeMinutes = 12,
            ViewsCount = 1520,
            PopularScore = 95.5,
            IsHomepageFeatured = true,
            FeaturedImageUrl = "/uploads/images/sample-cosmos.svg",
            FeaturedImageAlt = "Atmospheric vector landscape of cosmic icebergs and mirrored water",
            FeaturedImageCaptionHtml = "<p><em>Cosmic Monolith</em> — Rana Media Editorial Artwork</p>",
            CardImageUrl = "/uploads/images/sample-cosmos.svg",
            CardImageAlt = "Atmospheric vector landscape of cosmic icebergs and mirrored water",
            CardImageWidth = 1600,
            CardImageHeight = 900,
            DisplaySettingsJson = "{\"alignX\":\"center\",\"alignY\":\"center\",\"backdropStrength\":3,\"featureImgCaptionAlign\":\"right\"}",
            AudioEnabled = true,
            AudioDurationSeconds = 720,
            CommentsEnabled = true,
            CommentsOpen = true,
            PublishedAt = DateTime.UtcNow.AddDays(-2)
        };
        essay.ArticleTopics.Add(new ArticleTopic { TopicId = metaphysics.Id });
        essay.ArticleContributors.Add(new ArticleContributor { Contributor = author1, Role = ContributorRole.Author, DisplayOrder = 1 });
        essay.ArticleContributors.Add(new ArticleContributor { Contributor = editor, Role = ContributorRole.Editor, DisplayOrder = 1 });

        var video = new Article
        {
            Slug = "teaching-birds-long-lost-songs",
            Title = "A physicist’s plan to teach birds long-lost songs",
            Type = ArticleType.Video,
            Status = ArticleStatus.Published,
            StandfirstShort = "When bird populations decline, certain melodies vanish. One researcher tries to revive them.",
            StandfirstLong = "A short documentary following acoustic ecologists helping endangered songbirds reclaim their ancestral melodies.",
            BodyHtml = "<p>When songbird populations shrink to a handful of individuals, juvenile birds lose the elders who once taught them their species’ acoustic heritage. Acoustic physicists and conservationists are now testing gentle playback tutors in the wild.</p>",
            BodyPlainText = "When songbird populations shrink, juvenile birds lose the elders who once taught them their species acoustic heritage.",
            MetaTitle = "Teaching birds long-lost songs | Rana Media Videos",
            MetaDescription = "Watch how acoustic scientists help endangered songbirds reclaim forgotten melodies.",
            SectionId = science.Id,
            PrimaryTopicId = biology.Id,
            WordCount = 320,
            ReadingTimeMinutes = 2,
            ViewsCount = 940,
            PopularScore = 88.0,
            VideoHoster = VideoHoster.YouTube,
            VideoEmbedUrl = "https://www.youtube.com/embed/0fSX4tBWYWc",
            VideoDurationMinutes = 14,
            CreditsShort = "14 minutes",
            CreditsHtml = "<p>Directed by Clara Jensen. Sound design by Rana Acoustic Archive.</p>",
            CardImageUrl = "/uploads/images/sample-birds.svg",
            CardImageAlt = "Golden acoustic waves rising behind a songbird silhouette on a quiet branch",
            CardImageWidth = 1600,
            CardImageHeight = 900,
            FeaturedImageUrl = "/uploads/images/sample-birds.svg",
            FeaturedImageAlt = "Golden acoustic waves rising behind a songbird silhouette on a quiet branch",
            CommentsEnabled = true,
            CommentsOpen = true,
            PublishedAt = DateTime.UtcNow.AddDays(-1)
        };
        video.ArticleTopics.Add(new ArticleTopic { TopicId = biology.Id });

        var psychEssay = new Article
        {
            Slug = "love-with-nowhere-to-go",
            Title = "Love with nowhere to go",
            Type = ArticleType.Essay,
            Status = ArticleStatus.Published,
            StandfirstShort = "Grief is not only for the dead. When we are unable to honour other profound losses, we become strangers to ourselves",
            StandfirstLong = "Exploring disenfranchised grief, quiet transformations, and the moral vocabulary needed to name losses without funerals.",
            BodyHtml = "<p><span class=\"ld-dropcap\">W</span>e possess elaborate rituals for mourning a death, yet almost none for mourning a vanished version of ourselves, a fractured friendship, or an unlived future. Naming non-finite grief restores orientation to emotional life.</p><h2>The grammar of unnamed loss</h2><p>Without shared concepts, solitary sorrow often disguises itself as restlessness or detachment. Recognising loss where there is no graveside requires moral imagination.</p><p class=\"pullquote\">To name a hidden grief is to allow memory and present reality to inhabit the same room</p><p>By widening our emotional lexicon, we recover tenderness for the chapters that ended quietly without ceremony.</p>",
            BodyPlainText = "We possess elaborate rituals for mourning a death, yet almost none for mourning a vanished version of ourselves.",
            MetaTitle = "Love with nowhere to go | Rana Media",
            MetaDescription = "Grief is not only for the dead: why naming quiet losses restores emotional clarity.",
            SectionId = psychology.Id,
            PrimaryTopicId = moodEmotion.Id,
            WordCount = 2900,
            ReadingTimeMinutes = 14,
            ViewsCount = 1180,
            PopularScore = 91.2,
            FeaturedImageUrl = "/uploads/images/sample-mind.svg",
            FeaturedImageAlt = "Atmospheric burgundy composition of mirrored arches and ascending steps",
            CardImageUrl = "/uploads/images/sample-mind.svg",
            CardImageAlt = "Atmospheric burgundy composition of mirrored arches and ascending steps",
            CardImageWidth = 1600,
            CardImageHeight = 900,
            CommentsEnabled = true,
            CommentsOpen = true,
            PublishedAt = DateTime.UtcNow.AddDays(-3)
        };
        psychEssay.ArticleTopics.Add(new ArticleTopic { TopicId = moodEmotion.Id });
        psychEssay.ArticleContributors.Add(new ArticleContributor { Contributor = author2, Role = ContributorRole.Author, DisplayOrder = 1 });
        psychEssay.ArticleContributors.Add(new ArticleContributor { Contributor = editor, Role = ContributorRole.Editor, DisplayOrder = 1 });

        var societyEssay = new Article
        {
            Slug = "the-horse-made-the-state",
            Title = "The horse made the state",
            Type = ArticleType.Essay,
            Status = ArticleStatus.Published,
            StandfirstShort = "Civilisation wasn’t made possible by grain alone, but by transportation – and the reach it enabled, from the steppe to the sea",
            StandfirstLong = "Re-examining early political institutions through mobility, mounted communication networks, and steppe statecraft.",
            BodyHtml = "<p><span class=\"ld-dropcap\">F</span>or generations, historians treated sedentary grain agriculture as the sole crucible of state power. Yet across the Eurasian steppe, rapid equestrian mobility generated vast federations and legal codes long before agrarian empires could match their reach.</p><h2>Distance as political architecture</h2><p>Control over corridors of movement reshaped taxation, diplomacy, and cosmopolitan trade from the Black Sea to East Asia.</p>",
            BodyPlainText = "For generations, historians treated sedentary grain agriculture as the sole crucible of state power.",
            MetaTitle = "The horse made the state | Rana Media",
            MetaDescription = "How mobility and steppe logistics transformed early human polities and empires.",
            SectionId = society.Id,
            PrimaryTopicId = anthropology.Id,
            WordCount = 3400,
            ReadingTimeMinutes = 17,
            ViewsCount = 1310,
            PopularScore = 93.4,
            FeaturedImageUrl = "/uploads/images/sample-city.svg",
            FeaturedImageAlt = "Architectural crimson and stone colonnade under sweeping light",
            CardImageUrl = "/uploads/images/sample-city.svg",
            CardImageAlt = "Architectural crimson and stone colonnade under sweeping light",
            CardImageWidth = 1600,
            CardImageHeight = 900,
            CommentsEnabled = true,
            CommentsOpen = true,
            PublishedAt = DateTime.UtcNow.AddDays(-4)
        };
        societyEssay.ArticleTopics.Add(new ArticleTopic { TopicId = anthropology.Id });
        societyEssay.ArticleContributors.Add(new ArticleContributor { Contributor = author3, Role = ContributorRole.Author, DisplayOrder = 1 });

        var cultureVideo = new Article
        {
            Slug = "a-cubist-painting-becomes-an-immersive-world",
            Title = "A Cubist painting becomes two kids’ immersive animated world",
            Type = ArticleType.Video,
            Status = ArticleStatus.Published,
            StandfirstShort = "Stepping inside fractured planes of ochre and ultramarine through hand-crafted animation.",
            StandfirstLong = "An imaginative short film translating modernist geometry into a tactile childhood fable.",
            BodyHtml = "<p>What happens when early twentieth-century avant-garde painting becomes a playground? Animators spent three years painting frame-by-frame oils on glass to honour Cubist perspective.</p>",
            BodyPlainText = "An imaginative short film translating modernist geometry into a tactile childhood fable.",
            MetaTitle = "A Cubist painting becomes an animated world | Rana Media Videos",
            MetaDescription = "Watch a poetic short animation inside a living Cubist canvas.",
            SectionId = culture.Id,
            PrimaryTopicId = art.Id,
            WordCount = 210,
            ReadingTimeMinutes = 1,
            ViewsCount = 780,
            PopularScore = 84.5,
            VideoHoster = VideoHoster.YouTube,
            VideoEmbedUrl = "https://www.youtube.com/embed/ysz5S6PUM-U",
            VideoDurationMinutes = 6,
            CreditsShort = "6 minutes",
            CreditsHtml = "<p>Animated by Studio Mirabelle.</p>",
            CardImageUrl = "/uploads/images/sample-art.svg",
            CardImageAlt = "Modernist Cubist composition in ochre, terracotta, and cobalt forms",
            CardImageWidth = 1600,
            CardImageHeight = 900,
            FeaturedImageUrl = "/uploads/images/sample-art.svg",
            FeaturedImageAlt = "Modernist Cubist composition in ochre, terracotta, and cobalt forms",
            CommentsEnabled = true,
            CommentsOpen = true,
            PublishedAt = DateTime.UtcNow.AddDays(-5)
        };
        cultureVideo.ArticleTopics.Add(new ArticleTopic { TopicId = art.Id });

        var ethicsEssay = new Article
        {
            Slug = "in-praise-of-the-dilettante",
            Title = "In praise of the dilettante",
            Type = ArticleType.Essay,
            Status = ArticleStatus.Published,
            StandfirstShort = "A wayfaring cosmopolitan who never gets stuck, the dilettante is the right anti-hero for our hyper-specialised age",
            StandfirstLong = "Recovering the Renaissance sense of dilettante: one who pursues ideas out of delight rather than narrow credentialism.",
            BodyHtml = "<p><span class=\"ld-dropcap\">O</span>riginally derived from the Italian <em>dilettare</em> — to delight in — the dilettante once stood for curiosity unfettered by guild boundaries. Today, overspecialisation often mistakes narrowness for rigour.</p><h2>Delight as intellectual method</h2><p>Crossing disciplinary borders invites fruitful misunderstandings and unexpected synthesis across ethics, art, and civic life.</p>",
            BodyPlainText = "Originally derived from the Italian dilettare, the dilettante once stood for curiosity unfettered by guild boundaries.",
            MetaTitle = "In praise of the dilettante | Rana Media",
            MetaDescription = "Why Renaissance delight and broad curiosity still matter in an overspecialised world.",
            SectionId = philosophy.Id,
            PrimaryTopicId = ethics.Id,
            WordCount = 2150,
            ReadingTimeMinutes = 11,
            ViewsCount = 1040,
            PopularScore = 89.0,
            FeaturedImageUrl = "/uploads/images/sample-ethics.svg",
            FeaturedImageAlt = "Misty horizon framed by classical geometric archways",
            CardImageUrl = "/uploads/images/sample-ethics.svg",
            CardImageAlt = "Misty horizon framed by classical geometric archways",
            CardImageWidth = 1600,
            CardImageHeight = 900,
            CommentsEnabled = true,
            CommentsOpen = true,
            PublishedAt = DateTime.UtcNow.AddDays(-6)
        };
        ethicsEssay.ArticleTopics.Add(new ArticleTopic { TopicId = ethics.Id });
        ethicsEssay.ArticleContributors.Add(new ArticleContributor { Contributor = author1, Role = ContributorRole.Author, DisplayOrder = 1 });

        await context.Articles.AddRangeAsync(essay, video, psychEssay, societyEssay, cultureVideo, ethicsEssay);
        await context.SaveChangesAsync();
    }
}

