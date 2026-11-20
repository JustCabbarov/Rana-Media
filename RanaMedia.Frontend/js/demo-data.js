/* ==========================================================================
   Offline Fallback Dataset & Offline SEO Preview Engine
   Used ONLY when the .NET 8 backend API is unreachable (static/file preview)
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});

  var SECTIONS_FULL = [
    {
      id: 'sec-phil', slug: 'philosophy', title: 'Philosophy', accentColorHex: '#0f5c43',
      description: 'Essays and videos on philosophy, the history of ideas, ethics and life’s big questions.',
      topics: [
        { id: 't-meta', slug: 'metaphysics', title: 'Metaphysics' },
        { id: 't-eth', slug: 'ethics', title: 'Ethics' },
        { id: 't-mind', slug: 'philosophy-of-mind', title: 'Philosophy of mind' },
        { id: 't-good', slug: 'meaning-the-good-life', title: 'Meaning and the good life' },
        { id: 't-pol', slug: 'political-philosophy', title: 'Political philosophy' }
      ]
    },
    {
      id: 'sec-sci', slug: 'science', title: 'Science', accentColorHex: '#115e59',
      description: 'Essays and documentaries across physics, biology, cosmology, AI, and medicine.',
      topics: [
        { id: 't-bio', slug: 'biology', title: 'Biology' },
        { id: 't-phy', slug: 'physics', title: 'Physics' },
        { id: 't-cos', slug: 'cosmology', title: 'Cosmology' }
      ]
    },
    {
      id: 'sec-psy', slug: 'psychology', title: 'Psychology', accentColorHex: '#2d6a4f',
      description: 'Exploring consciousness, cognition, mood, emotion, and human wellbeing.',
      topics: [
        { id: 't-mood', slug: 'mood-emotion', title: 'Mood and emotion' },
        { id: 't-neuro', slug: 'neurodiversity', title: 'Neurodiversity' },
        { id: 't-tech', slug: 'technology-the-self', title: 'Technology and the self' }
      ]
    },
    {
      id: 'sec-soc', slug: 'society', title: 'Society', accentColorHex: '#14532d',
      description: 'History, politics, anthropology, migration, technology, and social imagination.',
      topics: [
        { id: 't-ant', slug: 'anthropology', title: 'Anthropology' },
        { id: 't-hum', slug: 'human-rights-and-justice', title: 'Human rights and justice' },
        { id: 't-dem', slug: 'demography-migration', title: 'Demography and migration' }
      ]
    },
    {
      id: 'sec-cul', slug: 'culture', title: 'Culture', accentColorHex: '#4a6b22',
      description: 'Essays and films examining art, literature, music, architecture, and travel.',
      topics: [
        { id: 't-art', slug: 'art', title: 'Art' },
        { id: 't-lit', slug: 'stories-literature', title: 'Stories and literature' }
      ]
    }
  ];

  var CONTRIBUTORS = {
    'leyla-aliyeva': {
      id: 'c-1', slug: 'leyla-aliyeva', name: 'Leyla Aliyeva',
      occupation: 'Associate Professor of Philosophy', company: 'Baku Ideas Institute',
      shortBioHtml: '<p>is a philosopher researching metaphysics and the epistemology of simplicity.</p>',
      bioHtml: '<p>Leyla Aliyeva writes on existence, complexity, and epistemology.</p>'
    },
    'derek-van-zoonen': {
      id: 'c-2', slug: 'derek-van-zoonen', name: 'Derek van Zoonen',
      occupation: 'Philosopher & Psychologist', company: 'University of Groningen',
      shortBioHtml: '<p>researches emotional phenomenology, grief, and moral psychology.</p>',
      bioHtml: '<p>Derek van Zoonen explores how unspoken emotions shape self-knowledge.</p>'
    },
    'lhamsuren-munkh-erdene': {
      id: 'c-3', slug: 'lhamsuren-munkh-erdene', name: 'Lhamsuren Munkh-Erdene',
      occupation: 'Historical Anthropologist', company: 'Institute for Advanced Study',
      shortBioHtml: '<p>studies Eurasian pastoralist polities, mobility, and state formation.</p>',
      bioHtml: '<p>Lhamsuren Munkh-Erdene investigates the origins of nomadic empires.</p>'
    },
    'sam-dresser': {
      id: 'c-4', slug: 'sam-dresser', name: 'Sam Dresser',
      occupation: 'Senior Editor', company: 'Rana Media',
      shortBioHtml: '<p>is Senior Editor at Rana Media.</p>',
      bioHtml: '<p>Sam Dresser commissions and edits philosophy and science essays.</p>'
    }
  };

  var ARTICLES = [
    {
      id: 'a-1', slug: 'why-anything-exists-instead-of-nothing',
      title: 'Why anything exists',
      type: 'Essay', status: 'Published',
      standfirstShort: 'Here is a fresh philosophical inquiry into why reality exists rather than pure emptiness',
      standfirstLong: 'Here’s a new answer to the old question ‘why is there something rather than nothing?’ (It’s neither simple nor easy)',
      bodyHtml: '<p><span class="ld-dropcap">W</span>hy is there something rather than nothing? This foundational metaphysical question invites us to explore simplicity, existence, and the architecture of reality. When properly considered, the ordinary fact that things exist shifts from seeming trivial to profoundly astonishing.</p><h2>The puzzle of non-ontological truths</h2><p>Even if nothing existed, mathematical and logical truths would remain coherent. Understanding why existence is simpler than emptiness challenges centuries of philosophical assumptions.</p><p class="pullquote">There would have been a whole architecture of non-ontological truths even if nothing had existed</p><p>Many proposed explanations for why there is something rather than nothing never confront the intuition that pure emptiness ought to be simpler and easier than a cosmos teeming with galaxies and minds.</p><blockquote>No theory, no conceptual scheme, can explain the existence of anything without assuming the existence of something.</blockquote><h3>Simplicity as an explanatory bridge</h3><p>When a single fundamental principle accounts for countless contingent facts, existence becomes the most economical hypothesis available to philosophy and physics alike.</p>',
      wordCount: 2400, readingTimeMinutes: 12, viewsCount: 1520, commentAndRepliesCount: 2,
      commentsEnabled: true, commentsOpen: true, audioEnabled: true, audioDurationSeconds: 720,
      cardImageUrl: '/uploads/images/sample-cosmos.svg', featuredImageUrl: '/uploads/images/sample-cosmos.svg',
      featuredImageCaptionHtml: '<p><em>Cosmic Monolith</em> — Rana Media Editorial Artwork</p>',
      settings: { alignX: 'center', alignY: 'center', backdropStrength: 3 },
      section: { id: 'sec-phil', slug: 'philosophy', title: 'Philosophy', accentColorHex: '#0f5c43' },
      primaryTopic: { id: 't-meta', slug: 'metaphysics', title: 'Metaphysics' },
      topics: [{ id: 't-meta', slug: 'metaphysics', title: 'Metaphysics' }, { id: 't-eth', slug: 'ethics', title: 'Ethics' }],
      authors: [CONTRIBUTORS['leyla-aliyeva']], editor: CONTRIBUTORS['sam-dresser'],
      republicationAllowed: true, publishedAt: '2026-11-12T11:00:00Z'
    },
    {
      id: 'a-2', slug: 'teaching-birds-long-lost-songs',
      title: 'A physicist’s audacious plan to teach birds long-lost songs',
      type: 'Video', status: 'Published',
      standfirstShort: 'When bird populations decline, certain melodies vanish. One researcher tries to revive them.',
      standfirstLong: 'A short documentary following acoustic ecologists helping endangered songbirds reclaim their ancestral melodies.',
      bodyHtml: '<p>When songbird populations shrink to a handful of individuals, juvenile birds lose the elders who once taught them their species’ acoustic heritage. Acoustic physicists and conservationists are now testing gentle playback tutors in the wild.</p>',
      wordCount: 320, readingTimeMinutes: 2, videoDurationMinutes: 14, viewsCount: 940, commentAndRepliesCount: 1,
      commentsEnabled: true, commentsOpen: true,
      videoHoster: 'YouTube', videoEmbedUrl: 'https://www.youtube.com/embed/0fSX4tBWYWc',
      creditsShort: '14 minutes', creditsHtml: '<p>Directed by Clara Jensen.</p>',
      cardImageUrl: '/uploads/images/sample-birds.svg', featuredImageUrl: '/uploads/images/sample-birds.svg',
      section: { id: 'sec-sci', slug: 'science', title: 'Science', accentColorHex: '#115e59' },
      primaryTopic: { id: 't-bio', slug: 'biology', title: 'Biology' },
      topics: [{ id: 't-bio', slug: 'biology', title: 'Biology' }],
      authors: [], publishedAt: '2026-11-15T09:00:00Z'
    },
    {
      id: 'a-3', slug: 'love-with-nowhere-to-go',
      title: 'Love with nowhere to go',
      type: 'Essay', status: 'Published',
      standfirstShort: 'Grief is not only for the dead. When we are unable to honour other profound losses, we become strangers to ourselves',
      standfirstLong: 'Exploring disenfranchised grief, quiet transformations, and the moral vocabulary needed to name losses without funerals.',
      bodyHtml: '<p><span class="ld-dropcap">W</span>e possess elaborate rituals for mourning a death, yet almost none for mourning a vanished version of ourselves, a fractured friendship, or an unlived future. Naming non-finite grief restores orientation to emotional life.</p><h2>The grammar of unnamed loss</h2><p>Without shared concepts, solitary sorrow often disguises itself as restlessness or detachment.</p><p class="pullquote">To name a hidden grief is to allow memory and present reality to inhabit the same room</p><p>By widening our emotional lexicon, we recover tenderness for the chapters that ended quietly without ceremony.</p>',
      wordCount: 2900, readingTimeMinutes: 14, viewsCount: 1180, commentAndRepliesCount: 1,
      commentsEnabled: true, commentsOpen: true,
      cardImageUrl: '/uploads/images/sample-mind.svg', featuredImageUrl: '/uploads/images/sample-mind.svg',
      section: { id: 'sec-psy', slug: 'psychology', title: 'Psychology', accentColorHex: '#2d6a4f' },
      primaryTopic: { id: 't-mood', slug: 'mood-emotion', title: 'Mood and emotion' },
      topics: [{ id: 't-mood', slug: 'mood-emotion', title: 'Mood and emotion' }],
      authors: [CONTRIBUTORS['derek-van-zoonen']], editor: CONTRIBUTORS['sam-dresser'],
      republicationAllowed: true, publishedAt: '2026-11-11T10:00:00Z'
    },
    {
      id: 'a-4', slug: 'the-horse-made-the-state',
      title: 'The horse made the state',
      type: 'Essay', status: 'Published',
      standfirstShort: 'Civilisation wasn’t made possible by grain alone, but by transportation – and the conquest it enabled, from the steppe to the sea',
      standfirstLong: 'Re-examining early political institutions through mobility, mounted communication networks, and steppe statecraft.',
      bodyHtml: '<p><span class="ld-dropcap">F</span>or generations, historians treated sedentary grain agriculture as the sole crucible of state power. Yet across the Eurasian steppe, rapid equestrian mobility generated vast federations and legal codes long before agrarian empires could match their reach.</p>',
      wordCount: 3400, readingTimeMinutes: 17, viewsCount: 1310, commentAndRepliesCount: 1,
      commentsEnabled: true, commentsOpen: true,
      cardImageUrl: '/uploads/images/sample-city.svg', featuredImageUrl: '/uploads/images/sample-city.svg',
      section: { id: 'sec-soc', slug: 'society', title: 'Society', accentColorHex: '#14532d' },
      primaryTopic: { id: 't-ant', slug: 'anthropology', title: 'Anthropology' },
      topics: [{ id: 't-ant', slug: 'anthropology', title: 'Anthropology' }],
      authors: [CONTRIBUTORS['lhamsuren-munkh-erdene']],
      republicationAllowed: true, publishedAt: '2026-11-10T10:00:00Z'
    },
    {
      id: 'a-5', slug: 'a-cubist-painting-becomes-an-immersive-world',
      title: 'A Cubist painting becomes two kids’ immersive animated world',
      type: 'Video', status: 'Published',
      standfirstShort: 'Stepping inside fractured planes of ochre and ultramarine through hand-crafted animation.',
      standfirstLong: 'An imaginative short film translating modernist geometry into a tactile childhood fable.',
      bodyHtml: '<p>Animators spent three years painting frame-by-frame oils on glass to honour Cubist perspective.</p>',
      wordCount: 210, readingTimeMinutes: 1, videoDurationMinutes: 3, viewsCount: 780, commentAndRepliesCount: 0,
      commentsEnabled: true, commentsOpen: true,
      videoHoster: 'YouTube', videoEmbedUrl: 'https://www.youtube.com/embed/ysz5S6PUM-U',
      creditsShort: '3 minutes',
      cardImageUrl: '/uploads/images/sample-art.svg', featuredImageUrl: '/uploads/images/sample-art.svg',
      section: { id: 'sec-cul', slug: 'culture', title: 'Culture', accentColorHex: '#4a6b22' },
      primaryTopic: { id: 't-art', slug: 'art', title: 'Art' },
      topics: [{ id: 't-art', slug: 'art', title: 'Art' }],
      authors: [], publishedAt: '2026-11-08T10:00:00Z'
    },
    {
      id: 'a-6', slug: 'in-praise-of-the-dilettante',
      title: 'In praise of the dilettante',
      type: 'Essay', status: 'Published',
      standfirstShort: 'A wayfaring cosmopolitan who never gets stuck, the dilettante is the right anti-hero for our hyper-specialised age',
      standfirstLong: 'Recovering the Renaissance sense of dilettante: one who pursues ideas out of delight rather than narrow credentialism.',
      bodyHtml: '<p><span class="ld-dropcap">O</span>riginally derived from the Italian <em>dilettare</em> — to delight in — the dilettante once stood for curiosity unfettered by guild boundaries.</p>',
      wordCount: 2150, readingTimeMinutes: 11, viewsCount: 1040, commentAndRepliesCount: 1,
      commentsEnabled: true, commentsOpen: true,
      cardImageUrl: '/uploads/images/sample-ethics.svg', featuredImageUrl: '/uploads/images/sample-ethics.svg',
      section: { id: 'sec-phil', slug: 'philosophy', title: 'Philosophy', accentColorHex: '#0f5c43' },
      primaryTopic: { id: 't-eth', slug: 'ethics', title: 'Ethics' },
      topics: [{ id: 't-eth', slug: 'ethics', title: 'Ethics' }],
      authors: [CONTRIBUTORS['leyla-aliyeva']],
      republicationAllowed: true, publishedAt: '2026-11-06T10:00:00Z'
    }
  ];

  // Assign related links cleanly
  ARTICLES.forEach(function (a, i) {
    a.related = ARTICLES.filter(function (_, j) { return j !== i; }).slice(0, 3);
  });

  var STATIC_PAGES = {
    about: { slug: 'about', title: 'About', bodyHtml: '<p>Rana Media is an independent digital magazine of ideas, philosophy, science, psychology, society, and culture — publishing deep essays and original short documentaries.</p>' },
    'community-guidelines': { slug: 'community-guidelines', title: 'Community guidelines', bodyHtml: '<p>We welcome considered, civil comments in response to articles. Before commenting, readers must confirm our guidelines. Each reader may publish <strong>1 root comment per article</strong> and may edit it within <strong>1 hour</strong> of posting.</p>' },
    republish: { slug: 'republish', title: 'Republish', bodyHtml: '<p>Essays marked as permissive may be republished after submitting a formal syndication request.</p>' },
    people: { slug: 'people', title: 'People', bodyHtml: '<p>Meet our editorial board, commissioning editors, and contributing philosophers and scientists.</p>' },
    contact: { slug: 'contact', title: 'Contact', bodyHtml: '<p>Editorial desk: editorial@ranamedia.co</p>' },
    support: { slug: 'support', title: 'Support', bodyHtml: '<p>Need help with your reader account, saved library, or newsletters? Reach out to support@ranamedia.co.</p>' },
    privacy: { slug: 'privacy', title: 'Privacy Policy', bodyHtml: '<p>Your reading activity, saved articles, and annotations remain private.</p>' },
    'newsletter-privacy': { slug: 'newsletter-privacy', title: 'Newsletter Privacy Policy', bodyHtml: '<p>Daily and weekly newsletters are sent strictly upon double opt-in confirmation.</p>' },
    'terms-of-use': { slug: 'terms-of-use', title: 'Terms of Use', bodyHtml: '<p>Terms governing reader participation, syndication, and editorial usage across Rana Media.</p>' }
  };

  function analyzeSeoOffline(req) {
    var focus = (req.focusKeyphrase || '').trim();
    var kpLower = focus.toLowerCase();
    var allPhrases = [kpLower].concat((req.synonyms || []).map(function (s) { return s.trim().toLowerCase(); })).filter(Boolean);
    var bodyHtml = req.bodyHtml || '';
    var plain = Rana.utils.stripHtml(bodyHtml);
    var words = plain ? plain.split(/\s+/).filter(Boolean) : [];
    var wordCount = words.length;

    var tmp = document.createElement('div');
    tmp.innerHTML = bodyHtml;
    var paragraphs = Array.from(tmp.querySelectorAll('p')).map(function (p) { return (p.textContent || '').trim(); }).filter(Boolean);
    if (!paragraphs.length && plain) paragraphs = [plain];
    var headings = Array.from(tmp.querySelectorAll('h2,h3')).map(function (h) { return (h.textContent || '').trim(); });
    var links = Array.from(tmp.querySelectorAll('a'));
    var internalLinks = links.filter(function (a) {
      var href = a.getAttribute('href') || '';
      return href.indexOf('/') === 0 || href.indexOf('#') === 0 || href.indexOf('ranamedia.co') !== -1;
    }).length;
    var outboundLinks = links.length - internalLinks;
    var imgs = Array.from(tmp.querySelectorAll('img'));
    var imageCount = imgs.length + (req.featuredImageUrl ? 1 : 0);

    var occurrences = 0;
    var highlights = [];
    if (allPhrases.length) {
      paragraphs.forEach(function (pText, pIdx) {
        allPhrases.forEach(function (phrase) {
          var idx = 0;
          var lowerP = pText.toLowerCase();
          while ((idx = lowerP.indexOf(phrase, idx)) !== -1) {
            occurrences++;
            highlights.push({
              ruleId: 'keyphrase_density',
              paragraphIndex: pIdx,
              startOffset: idx,
              length: phrase.length,
              matchedText: pText.substr(idx, phrase.length),
              contextSnippet: pText.slice(Math.max(0, idx - 24), idx + phrase.length + 24)
            });
            idx += phrase.length;
          }
        });
      });
    }

    var title = (req.seoTitle || '').trim();
    var slug = (req.slug || '').trim().toLowerCase();
    var metaDesc = (req.metaDescription || '').trim();
    var kpWords = kpLower.split(/\s+/).filter(Boolean);
    var problems = [], improvements = [], goodResults = [];

    function push(ruleId, checkTitle, desc, sev, impact, hl) {
      var item = { ruleId: ruleId, title: checkTitle, description: desc, severity: sev, scoreImpact: impact, highlightable: !!hl, matchedTokens: [] };
      if (sev === 'Problem') problems.push(item);
      else if (sev === 'Improvement') improvements.push(item);
      else goodResults.push(item);
    }

    if (!kpWords.length) push('keyphrase_length', 'Focus keyphrase', 'No focus keyphrase was set for this page.', 'Problem', 0, false);
    else if (kpWords.length <= 4) push('keyphrase_length', 'Keyphrase length', 'Good job!', 'Good', 7, false);
    else push('keyphrase_length', 'Keyphrase length', 'The keyphrase is ' + kpWords.length + ' content words long. Aim for 1–4 words.', 'Improvement', 4, false);

    if (kpLower && title.toLowerCase().indexOf(kpLower) === 0)
      push('keyphrase_in_seo_title', 'Keyphrase in SEO title', 'The exact match of the focus keyphrase appears at the beginning of the SEO title. Good job!', 'Good', 8, false);
    else if (kpLower && title.toLowerCase().indexOf(kpLower) !== -1)
      push('keyphrase_in_seo_title', 'Keyphrase in SEO title', 'Focus keyphrase appears in the SEO title, though not at the beginning.', 'Improvement', 5, false);
    else
      push('keyphrase_in_seo_title', 'Keyphrase in SEO title', 'The focus keyphrase does not appear in the SEO title.', 'Problem', 0, false);

    var estWidth = title.length * 9;
    if (title.length === 0) push('seo_title_width', 'SEO title width', 'Please create an SEO title.', 'Problem', 0, false);
    else if (estWidth > 600) push('seo_title_width', 'SEO title width', 'The SEO title is wider than the viewable limit (600px). Try to make it shorter.', 'Problem', 0, false);
    else push('seo_title_width', 'SEO title width', 'The SEO title has a nice length.', 'Good', 6, false);

    var slugMatched = kpWords.filter(function (w) { return slug.indexOf(w) !== -1; }).length;
    if (kpWords.length && slugMatched >= Math.ceil(kpWords.length / 2))
      push('keyphrase_in_slug', 'Keyphrase in slug', 'More than half of your keyphrase appears in the slug. That’s great!', 'Good', 6, false);
    else
      push('keyphrase_in_slug', 'Keyphrase in slug', 'Your keyphrase does not appear enough in the URL slug.', 'Improvement', 3, false);

    if (!metaDesc) push('meta_description_length', 'Meta description length', 'No meta description has been specified. Make sure to write one!', 'Problem', 0, false);
    else if (metaDesc.length < 120) push('meta_description_length', 'Meta description length', 'Meta description is short (' + metaDesc.length + ' chars; aim for 120–156).', 'Improvement', 4, false);
    else if (metaDesc.length > 156) push('meta_description_length', 'Meta description length', 'Meta description exceeds 156 characters.', 'Improvement', 4, false);
    else push('meta_description_length', 'Meta description length', 'Well done! Meta description length is optimal.', 'Good', 7, false);

    if (kpLower && metaDesc.toLowerCase().indexOf(kpLower) !== -1)
      push('keyphrase_in_meta_description', 'Keyphrase in meta description', 'Keyphrase appears in the meta description. Well done!', 'Good', 7, false);
    else
      push('keyphrase_in_meta_description', 'Keyphrase in meta description', 'Focus keyphrase or synonyms do not appear in the meta description.', 'Improvement', 3, false);

    var firstP = (paragraphs[0] || '').toLowerCase();
    if (allPhrases.some(function (p) { return firstP.indexOf(p) !== -1; }))
      push('keyphrase_in_introduction', 'Keyphrase in introduction', 'Your keyphrase or its synonyms appear in the first paragraph. Well done!', 'Good', 7, true);
    else
      push('keyphrase_in_introduction', 'Keyphrase in introduction', 'Your keyphrase or its synonyms do not appear in the first paragraph. Make sure the topic is clear immediately.', 'Problem', 0, true);

    var minOcc = wordCount >= 300 ? Math.max(2, Math.floor(wordCount / 300)) : 2;
    if (occurrences === 0 || occurrences < minOcc)
      push('keyphrase_density', 'Keyphrase density', 'The keyphrase was found ' + occurrences + ' times. That’s less than the recommended minimum of ' + minOcc + ' times for a text of this length.', 'Problem', 0, true);
    else
      push('keyphrase_density', 'Keyphrase density', 'The focus keyphrase was found ' + occurrences + ' times. That’s great!', 'Good', 8, true);

    if (occurrences >= 3)
      push('keyphrase_distribution', 'Keyphrase distribution', 'Great! Your keyphrase is distributed across the article.', 'Good', 7, true);
    else
      push('keyphrase_distribution', 'Keyphrase distribution', 'Large parts of the text lack the keyphrase or its synonyms.', 'Problem', 0, true);

    if (headings.length && headings.some(function (h) { return allPhrases.some(function (p) { return h.toLowerCase().indexOf(p) !== -1; }); }))
      push('keyphrase_in_subheading', 'Keyphrase in subheading', 'Your keyphrase reflects the topic of your subheadings. Good job!', 'Good', 6, true);
    else
      push('keyphrase_in_subheading', 'Keyphrase in subheading', 'Use more keyphrases or synonyms in your H2 and H3 subheadings!', 'Problem', 0, false);

    if (wordCount >= 300) push('text_length', 'Text length', 'The text contains ' + wordCount + ' words. Good job!', 'Good', 8, false);
    else push('text_length', 'Text length', 'The text contains ' + wordCount + ' words. Try writing at least 300 words.', 'Problem', 0, false);

    if (outboundLinks > 0) push('outbound_links', 'Outbound links', 'Good job! Found ' + outboundLinks + ' outbound link(s).', 'Good', 6, false);
    else push('outbound_links', 'Outbound links', 'No outbound links appear in this page. Add some!', 'Problem', 0, false);

    if (internalLinks > 0) push('internal_links', 'Internal links', 'You have enough internal links (' + internalLinks + '). Good job!', 'Good', 6, false);
    else push('internal_links', 'Internal links', 'No internal links appear in this page, make sure to add some!', 'Problem', 0, false);

    if (imageCount > 0) push('images_and_alt', 'Images', 'Good job! Found ' + imageCount + ' image(s).', 'Good', 6, false);
    else push('images_and_alt', 'Images', 'No images appear on this page. Add some!', 'Problem', 0, false);

    push('previously_used_keyphrase', 'Previously used keyphrase', 'You’ve not used this keyphrase before, very good.', 'Good', 7, false);

    var totalScore = problems.concat(improvements, goodResults).reduce(function (acc, i) { return acc + i.scoreImpact; }, 0);
    var status = totalScore >= 70 && problems.length <= 1 ? 'Green' : (totalScore >= 40 ? 'Orange' : 'Red');

    return {
      overallScore: Math.min(100, totalScore),
      status: status,
      problems: problems,
      improvements: improvements,
      goodResults: goodResults,
      relatedKeyphraseResults: (req.relatedKeyphrases || []).filter(function (rk) { return rk.keyphrase; }).map(function (rk) {
        return { keyphrase: rk.keyphrase, synonyms: rk.synonyms || [], score: 65, status: 'Orange', problems: [], improvements: [], goodResults: [] };
      }),
      highlights: highlights,
      conflictingArticles: [],
      metrics: {
        wordCount: wordCount,
        readingTimeMinutes: Math.max(1, Math.ceil(wordCount / 200)),
        paragraphCount: paragraphs.length,
        subheadingCount: headings.length,
        seoTitleCharLength: title.length,
        seoTitlePixelWidth: estWidth,
        metaDescriptionLength: metaDesc.length,
        keyphraseOccurrences: occurrences,
        keyphraseDensityPercent: wordCount ? +(occurrences / wordCount * 100).toFixed(2) : 0,
        recommendedMinOccurrences: minOcc,
        recommendedMaxOccurrences: Math.max(minOcc + 2, Math.ceil(wordCount * 0.03 / Math.max(1, kpWords.length))),
        internalLinkCount: internalLinks,
        outboundLinkCount: outboundLinks,
        imageCount: imageCount,
        imagesWithKeyphraseAlt: req.featuredImageAlt && kpLower && req.featuredImageAlt.toLowerCase().indexOf(kpLower) !== -1 ? 1 : 0,
        previousArticlesUsingKeyphraseCount: 0
      }
    };
  }

  Rana.demo = {
    sections: SECTIONS_FULL,
    contributors: CONTRIBUTORS,
    articles: ARTICLES,
    staticPages: STATIC_PAGES,
    home: {
      featuredEssay: ARTICLES[0],
      popularThisMonth: [ARTICLES[0], ARTICLES[2], ARTICLES[3], ARTICLES[5]],
      articles: { nodes: ARTICLES, hasNextPage: false, endCursor: null }
    },
    analyzeSeoOffline: analyzeSeoOffline
  };
})(window);
