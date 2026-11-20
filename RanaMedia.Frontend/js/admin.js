/* ==========================================================================
   Rana Studio — Multilingual (AZ / EN) Editorial Admin & Yoast SEO Panel
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});
  var U = Rana.utils;
  var UI = Rana.ui;

  var LANG_KEY = 'rana_admin_lang';
  function loadInitialLang() {
    try {
      var saved = global.localStorage.getItem(LANG_KEY);
      return saved === 'en' ? 'en' : 'az';
    } catch (_) {
      return 'az';
    }
  }

  var I18N = {
    az: {
      pageTitle: 'Rana Studio | Redaksiya İdarə Paneli və SEO',
      authSub: 'Redaksiya Studiyasına Giriş',
      authNoRole: 'Cari hesabda Admin, Editor və ya Moderator rolu yoxdur.',
      emailLabel: 'Redaksiya E-poçtu',
      passwordLabel: 'Şifrə',
      enterStudio: 'Redaksiya Studiyasına Daxil Ol',
      backToMag: '← İctimai Jurnala Qayıt',
      signOut: 'Çıxış',
      fillAdmin: 'Standart admini doldur',
      changeApi: 'API Dəyiş',
      promptApiUrl: 'Backend API əsas ünvanını daxil edin:',
      loginFailed: 'Giriş alınmadı.',
      brandSub: 'Redaksiya İdarə Paneli',
      navOverview: 'Ümumi Baxış',
      navEditorial: 'Redaksiya',
      navCommunity: 'İcma',
      navSystem: 'Sistem',
      tabDashboard: 'İdarə Paneli',
      tabArticles: 'Məqalələr',
      tabComposer: '+ Yeni Məqalə və SEO',
      tabContributors: 'Müəlliflər',
      tabMedia: 'Media Yükləmələri',
      tabModeration: 'Şərh Moderasiyası',
      tabSubmissions: 'Təkliflər və Sindikasiya',
      tabUsers: 'İstifadəçilər və Rollar',
      titleDashboard: 'Redaksiya İcmalı',
      titleArticles: 'Məqalələr və Nəşrlər',
      titleComposerEdit: 'Məqalə Redaktəsi və SEO Analizi',
      titleComposerNew: 'Yeni Məqalə Yaz',
      titleContributors: 'Müəlliflər və Redaktorlar',
      titleModeration: 'Şərh Moderasiyası',
      titleSubmissions: 'Təkliflər və Sindikasiya',
      titleMedia: 'Media və Yükləmələr Kitabxanası',
      titleUsers: 'İstifadəçilər və Rol İcazələri',
      viewPublicMag: 'İctimai Jurnala Bax ↗',
      loadingStats: 'Statistika yüklənir…',
      kpiPublished: 'Dərc Edilmiş Məqalələr',
      kpiDraft: 'Qaralama',
      kpiScheduled: 'Planlaşdırılmış',
      kpiViews: 'Ümumi Baxış',
      kpiViewsSub: 'Cəmi {n} material üzrə',
      kpiAvgSeo: 'Orta SEO Balı',
      kpiAnalyzedSub: '{n} analiz edilmiş məqalə',
      kpiComments: 'Şərhlər',
      kpiModerated: 'Gizlədilmiş',
      kpiPending: 'Gözləyən Müraciətlər',
      kpiPitches: 'Təkliflər',
      kpiSyndication: 'Sindikasiya',
      kpiUsers: 'Qeydiyyatlı İstifadəçilər',
      kpiSubscribers: 'Aktiv Bülleten Abunəçisi',
      seoHealthTitle: 'Yoast SEO Sağlamlıq İcmalı',
      btnComposeArticle: '+ Məqalə Yaz',
      seoGoodGreen: 'Yaxşı (Yaşıl)',
      seoOkOrange: 'Orta (Narıncı)',
      seoRedWork: 'Təkmilləşdirilməli (Qırmızı)',
      loadingArticles: 'Məqalələr yüklənir…',
      phSearchArticle: 'Başlıq və ya slug…',
      allStatus: 'Bütün Statuslar',
      allTypes: 'Bütün Növlər',
      stPublished: 'Dərc olunub',
      stDraft: 'Qaralama',
      stScheduled: 'Planlaşdırılıb',
      stArchived: 'Arxiv',
      tpEssay: 'Esse',
      tpVideo: 'Video',
      tpIdea: 'İdeya',
      tpClassic: 'Klassik',
      btnFilter: 'Filtrlə',
      btnNewArticle: '+ Yeni Məqalə',
      thTitleSection: 'Başlıq və Bölmə',
      thType: 'Növ',
      thStatus: 'Status',
      thSeoLight: 'SEO İşığı',
      thViews: 'Baxış',
      thComments: 'Şərh',
      thActions: 'Əməliyyatlar',
      heroBadge: 'Hero',
      notAnalyzed: 'Analiz edilməyib',
      btnEditSeo: 'Redaktə və SEO',
      btnView: 'Bax ↗',
      btnDelete: 'Sil',
      noArticlesFound: 'Məqalə tapılmadı.',
      pageSummary: 'Cəmi: {total} · Səhifə {page} / {pages}',
      btnPrev: 'Əvvəlki',
      btnNext: 'Növbəti',
      confirmDelArticle: 'Bu məqaləni silmək istədiyinizə əminsiniz?',
      toastArticleDeleted: 'Məqalə silindi.',
      loadingArticleEdit: 'Məqalə və SEO profili açılır…',
      btnBackArticles: '← Məqalələrə Qayıt',
      btnAnalyzeSaveSeo: 'SEO-nu Analiz Et və Saxla',
      btnUpdateArticle: 'Məqaləni Yenilə',
      btnPublishSave: 'Məqaləni Saxla / Dərc Et',
      phHeadline: 'Məqalə Başlığı (məs. Why anything exists)',
      lblSlug: 'URL Slug',
      phSlug: 'avtomatik-slug',
      lblType: 'Növ',
      lblStatus: 'Status',
      lblSection: 'Bölmə',
      lblTopic: 'Əsas Mövzu',
      optNone: 'Yoxdur',
      lblShortStandfirst: 'Qısa Anons (Kartda)',
      phShortStandfirst: 'Kartlarda görünən qısa təsvir cümləsi',
      lblLongStandfirst: 'Uzun Alt Başlıq (Hero)',
      lblBodyHtml: 'Məqalənin HTML Mətni',
      toolWriteHtml: 'HTML Yaz',
      toolPreviewHl: 'Önizləmə + SEO Vurğulama',
      toolParagraph: 'Paraqraf',
      toolPullquote: 'Sitat (Pullquote)',
      toolQuote: 'Blok Sitat',
      toolLink: 'Keçid',
      toolUploadImg: '+ Şəkil Yüklə',
      panelSnippetMeta: 'Axtarış Snippet Metadata və Vizual Təqdimat',
      lblMetaTitle: 'SEO Meta Başlıq',
      phMetaTitle: 'Boş qalsa Məqalə Başlığı götürülür',
      lblTags: 'Teqlər (vergüllə ayrılmış)',
      lblMetaDesc: 'SEO Meta Təsvir (120–156 simvol)',
      lblHeroImgUrl: 'Hero Şəkil URL',
      btnUpload: 'Yüklə',
      lblHeroImgAlt: 'Hero Şəkil Alt Mətni',
      lblCardImgUrl: 'Kart Şəkil URL',
      lblCardImgAlt: 'Kart Şəkil Alt Mətni',
      lblHeroAlign: 'Hero Mətn Mövqeyi X / Y / Fon',
      optCenter: 'Mərkəz',
      optLeft: 'Sol',
      optRight: 'Sağ',
      optTop: 'Yuxarı',
      optBottom: 'Aşağı',
      lblCommissionEditor: 'Məsul Redaktor',
      lblAuthors: 'Müəlliflər',
      lblAudioUrlSec: 'Audio URL və Müddət (san)',
      lblVideoHosterUrl: 'Video Platforma və Embed URL',
      lblVideoMinCredits: 'Video Müddəti (dəq) və Titrlər',
      phCreditsShort: 'məs. 14 dəqiqə',
      chkHomepageHero: 'Ana Səhifə Hero',
      chkAudio: 'Audio',
      chkCommentsOn: 'Şərhlər Aktiv',
      chkOpen: 'Açıq',
      chkSyndication: 'Sindikasiya',
      promptLinkUrl: 'Keçid URL (daxili /essays/... və ya xarici https://...):',
      promptImgAlt: 'Şəkil üçün Alt mətn yazın (SEO üçün tövsiyə olunur):',
      toastImgUploaded: 'Şəkil serverə yükləndi (/uploads).',
      toastRequiredFields: 'Başlıq, Qısa Anons və Mətn mütləqdir.',
      toastArticleSaved: 'Məqalə uğurla yadda saxlanıldı!',
      toastArticleSaveErr: 'Məqalə saxlanılarkən xəta baş verdi.',
      toastSeoSaved: 'SEO profili bazada yadda saxlanıldı!',
      seoAnalysisHeading: 'SEO analizi',
      seoSetFocusHint: 'Aşağıda əsas açar ifadə daxil edin',
      lblFocusKp: 'Əsas açar ifadə (Focus keyphrase)',
      phFocusKp: 'məs. varlıq fəlsəfəsi',
      btnAddSynonyms: '+ Sinonimlər əlavə et',
      btnAddRelatedKp: '+ Əlaqəli açar ifadə əlavə et',
      lblSynonymsInput: 'Sinonimlər (vergüllə və ya Enter)',
      phAddSynonym: 'Sinonim əlavə et…',
      btnAdd: 'Əlavə et',
      lblRelatedKpNum: 'Əlaqəli Açar İfadə #{n}',
      btnRemove: 'Sil',
      phRelatedKp: 'Əlaqəli açar ifadə…',
      lblGooglePreview: 'Google Axtarış Önizləməsi və Uzunluq',
      defaultSeoTitle: 'Məqalənizin SEO Başlığı',
      defaultSerpDesc: 'Redaktorda meta təsvir sahəsini dolduraraq axtarış nəticəsi təsvirini daxil edin.',
      lblSeoTitleWidth: 'SEO Başlıq Eni',
      lblMetaDescShort: 'Meta Təsvir',
      charsUnit: 'simvol',
      cannibalAlert: 'Açar İfadə Təkrarı (Cannibalization) Xəbərdarlığı',
      usedBy: 'İstifadə edən məqalələr:',
      lblAnalysisResults: 'Analiz nəticələri',
      wordsDensity: '{w} söz · {d}% sıxlıq',
      grpProblems: 'Problemlər ({n})',
      grpImprovements: 'Təkmilləşdirmələr ({n})',
      grpGood: 'Yaxşı nəticələr ({n})',
      eyeHighlightTitle: 'Məqalə önizləməsində uyğunluqları vurğula',
      contribHeading: 'Redaksiya Müəllifləri ({n})',
      thName: 'Ad',
      thSlug: 'Slug',
      contribCreateHeading: '+ Yeni Müəllif Yarat',
      lblFullName: 'Tam Ad',
      lblOccupation: 'Vəzifə / İxtisas',
      lblInstitution: 'Qurum',
      lblShortBio: 'Qısa Bio HTML (yan paneldə görünür)',
      lblFullBio: 'Tam Bio HTML',
      btnSaveContrib: 'Müəllifi Saxla',
      toastContribAdded: 'Müəllif əlavə edildi!',
      confirmDelContrib: 'Bu müəllifi silmək istədiyinizə əminsiniz?',
      toastContribDeleted: 'Müəllif silindi.',
      loadingComments: 'Şərhlər yüklənir…',
      modHeading: 'Şərh Moderasiyası ({n})',
      modAll: 'Hamısı',
      modReported: 'Yalnız Şikayət Edilənlər',
      modModerated: 'Yalnız Gizlədilənlər',
      thReaderArticle: 'Oxucu və Məqalə',
      thComment: 'Şərh',
      thSignals: 'Siqnallar',
      lblLikes: 'Bəyənmə',
      lblReplies: 'Cavab',
      lblReports: 'Şikayət',
      lblReason: 'Səbəb:',
      stHidden: 'Gizli',
      stVisible: 'Görünən',
      badgeFeatured: 'Seçilmiş',
      btnRestore: 'Bərpa et',
      btnHide: 'Gizlət',
      btnFeature: 'Önə çıxar',
      btnUnfeature: 'Önə çıxarmadan götür',
      noCommentsFound: 'Şərh tapılmadı.',
      promptHideReason: 'Gizlətmə səbəbi:',
      defaultHideReason: 'İcma Qaydalarını pozur',
      toastCommentHidden: 'Şərh gizlədildi.',
      toastCommentRestored: 'Şərh bərpa edildi.',
      confirmDelComment: 'Şərhi silmək istədiyinizə əminsiniz?',
      loadingSubmissions: 'Müraciətlər yüklənir…',
      tabPitches: 'Redaksiya Təklifləri ({n})',
      tabSyndications: 'Sindikasiya Müraciətləri ({n})',
      thAuthor: 'Müəllif',
      thPitchTitleField: 'Təklif Başlığı və Sahə',
      thDecision: 'Qərar',
      thApplicant: 'Müraciətçi',
      thPubArticle: 'Nəşr və Məqalə',
      btnApprove: 'Təsdiqlə',
      btnReject: 'Rədd et',
      noPitches: 'Təklif müraciəti yoxdur.',
      noSyndications: 'Sindikasiya müraciəti yoxdur.',
      promptEditorNotes: 'Redaktor qeydi (istəyə bağlı):',
      toastStatusUpdated: 'Status yeniləndi.',
      mediaHeading: 'Redaksiya Mediası Yüklə (wwwroot/uploads)',
      folderImages: 'Şəkillər (Images)',
      folderVideos: 'Videolar (Videos)',
      folderAudio: 'Audio',
      folderAvatars: 'Avatarlar',
      btnUploadFile: 'Fayl Yüklə',
      btnCopyUrl: 'URL Kopyala',
      noMediaSession: 'Bu sessiyada yüklənmiş fayl yoxdur. Yeni fayl seçib yükləyin.',
      toastFileUploaded: 'Fayl yükləndi: ',
      toastUrlCopied: 'URL kopyalandı!',
      toastFileDeleted: 'Fayl silindi.',
      loadingUsers: 'İstifadəçilər yüklənir…',
      usersHeading: 'İstifadəçilər və Rollar ({n})',
      phUserSearch: 'Ad və ya e-poçt…',
      btnSearch: 'Axtar',
      btnCreateRole: '+ Yeni Rol Yarat',
      thUser: 'İstifadəçi',
      thRoles: 'Rollar',
      thStatusGl: 'Status və Qaydalar',
      thManage: 'İdarə et',
      btnAssign: '+ Təyin et',
      stActive: 'Aktiv',
      stInactive: 'Deaktiv',
      lblGuidelines: 'Qaydalar:',
      glAccepted: 'Qəbul edilib',
      glPending: 'Gözləyir',
      btnActivate: 'Aktivləşdir',
      btnDeactivate: 'Deaktiv et',
      promptNewRole: 'Yeni rol adı:',
      toastRoleCreated: 'Rol yaradıldı!',
      toastRoleAssigned: 'Rol təyin edildi.',
      toastRoleRemoved: 'Rol silindi.',
      confirmDelUser: 'İstifadəçini silmək istədiyinizə əminsiniz?'
    },
    en: {
      pageTitle: 'Rana Studio | Editorial Admin & SEO Analysis',
      authSub: 'Editorial Studio Sign In',
      authNoRole: 'This account does not have an Admin, Editor, or Moderator role.',
      emailLabel: 'Editorial Email',
      passwordLabel: 'Password',
      enterStudio: 'Enter Editorial Studio',
      backToMag: '← Back to Public Magazine',
      signOut: 'Sign Out',
      fillAdmin: 'Fill default admin',
      changeApi: 'Change API',
      promptApiUrl: 'Backend API Base URL:',
      loginFailed: 'Sign in failed.',
      brandSub: 'Editorial Admin Studio',
      navOverview: 'Overview',
      navEditorial: 'Editorial',
      navCommunity: 'Community',
      navSystem: 'System',
      tabDashboard: 'Dashboard',
      tabArticles: 'Articles',
      tabComposer: '+ New Article & SEO',
      tabContributors: 'Contributors',
      tabMedia: 'Media Uploads',
      tabModeration: 'Comment Moderation',
      tabSubmissions: 'Pitches & Syndication',
      tabUsers: 'Users & Roles',
      titleDashboard: 'Editorial Overview',
      titleArticles: 'Articles & Publications',
      titleComposerEdit: 'Edit Article & SEO Analysis',
      titleComposerNew: 'Compose New Article',
      titleContributors: 'Contributors & Editors',
      titleModeration: 'Comment Moderation',
      titleSubmissions: 'Pitches & Syndication',
      titleMedia: 'Media & Uploads Library',
      titleUsers: 'Users & Role Access',
      viewPublicMag: 'View Public Magazine ↗',
      loadingStats: 'Loading statistics…',
      kpiPublished: 'Published Articles',
      kpiDraft: 'Draft',
      kpiScheduled: 'Scheduled',
      kpiViews: 'Total Views',
      kpiViewsSub: 'Across {n} pieces',
      kpiAvgSeo: 'Average SEO Score',
      kpiAnalyzedSub: '{n} analyzed articles',
      kpiComments: 'Comments',
      kpiModerated: 'Moderated',
      kpiPending: 'Pending Review',
      kpiPitches: 'Pitches',
      kpiSyndication: 'Syndication',
      kpiUsers: 'Registered Users',
      kpiSubscribers: 'Active Newsletter',
      seoHealthTitle: 'Yoast SEO Health Overview',
      btnComposeArticle: '+ Compose Article',
      seoGoodGreen: 'Good (Green)',
      seoOkOrange: 'OK (Orange)',
      seoRedWork: 'Needs Work (Red)',
      loadingArticles: 'Loading articles…',
      phSearchArticle: 'Title or slug…',
      allStatus: 'All Status',
      allTypes: 'All Types',
      stPublished: 'Published',
      stDraft: 'Draft',
      stScheduled: 'Scheduled',
      stArchived: 'Archived',
      tpEssay: 'Essay',
      tpVideo: 'Video',
      tpIdea: 'Idea',
      tpClassic: 'Classic',
      btnFilter: 'Filter',
      btnNewArticle: '+ New Article',
      thTitleSection: 'Title & Section',
      thType: 'Type',
      thStatus: 'Status',
      thSeoLight: 'SEO Light',
      thViews: 'Views',
      thComments: 'Comments',
      thActions: 'Actions',
      heroBadge: 'Hero',
      notAnalyzed: 'Not analyzed',
      btnEditSeo: 'Edit & SEO',
      btnView: 'View ↗',
      btnDelete: 'Delete',
      noArticlesFound: 'No articles found.',
      pageSummary: 'Total: {total} · Page {page} of {pages}',
      btnPrev: 'Prev',
      btnNext: 'Next',
      confirmDelArticle: 'Are you sure you want to delete this article?',
      toastArticleDeleted: 'Article deleted.',
      loadingArticleEdit: 'Loading article & SEO profile…',
      btnBackArticles: '← Back to Articles',
      btnAnalyzeSaveSeo: 'Analyze & Save SEO',
      btnUpdateArticle: 'Update Article',
      btnPublishSave: 'Publish / Save Article',
      phHeadline: 'Article Headline (e.g. Why anything exists)',
      lblSlug: 'URL Slug',
      phSlug: 'auto-generated-slug',
      lblType: 'Type',
      lblStatus: 'Status',
      lblSection: 'Section',
      lblTopic: 'Primary Topic',
      optNone: 'None',
      lblShortStandfirst: 'Standfirst Short (Card Teaser)',
      phShortStandfirst: 'One crisp sentence shown on cards',
      lblLongStandfirst: 'Standfirst Long (Hero Subtitle)',
      lblBodyHtml: 'Article Body HTML',
      toolWriteHtml: 'Write HTML',
      toolPreviewHl: 'Preview + SEO Highlights',
      toolParagraph: 'Paragraph',
      toolPullquote: 'Pullquote',
      toolQuote: 'Quote',
      toolLink: 'Link',
      toolUploadImg: '+ Upload Image',
      panelSnippetMeta: 'Search Snippet Metadata & Visual Presentation',
      lblMetaTitle: 'SEO Meta Title',
      phMetaTitle: 'Defaults to Headline',
      lblTags: 'Tags (comma separated)',
      lblMetaDesc: 'SEO Meta Description (120–156 chars)',
      lblHeroImgUrl: 'Hero Image URL',
      btnUpload: 'Upload',
      lblHeroImgAlt: 'Hero Image Alt Text',
      lblCardImgUrl: 'Card Image URL',
      lblCardImgAlt: 'Card Image Alt Text',
      lblHeroAlign: 'Hero Align X / Y / Backdrop',
      optCenter: 'Center',
      optLeft: 'Left',
      optRight: 'Right',
      optTop: 'Top',
      optBottom: 'Bottom',
      lblCommissionEditor: 'Commissioning Editor',
      lblAuthors: 'Authors',
      lblAudioUrlSec: 'Audio Narration URL & Seconds',
      lblVideoHosterUrl: 'Video Hoster & Embed URL',
      lblVideoMinCredits: 'Video Duration (min) & Credits',
      phCreditsShort: 'e.g. 14 minutes',
      chkHomepageHero: 'Homepage Hero',
      chkAudio: 'Audio',
      chkCommentsOn: 'Comments On',
      chkOpen: 'Open',
      chkSyndication: 'Syndication',
      promptLinkUrl: 'Link URL (internal /essays/... or external https://...):',
      promptImgAlt: 'Enter Alt text for image (recommended for SEO):',
      toastImgUploaded: 'Image uploaded to server (/uploads).',
      toastRequiredFields: 'Title, Standfirst Short and Body are required.',
      toastArticleSaved: 'Article saved successfully!',
      toastArticleSaveErr: 'Error saving article.',
      toastSeoSaved: 'SEO profile saved to database!',
      seoAnalysisHeading: 'SEO analysis',
      seoSetFocusHint: 'Set a focus keyphrase below',
      lblFocusKp: 'Focus keyphrase',
      phFocusKp: 'e.g. philosophy of existence',
      btnAddSynonyms: '+ Add synonyms',
      btnAddRelatedKp: '+ Add related keyphrase',
      lblSynonymsInput: 'Synonyms (comma separated or Enter)',
      phAddSynonym: 'Add synonym…',
      btnAdd: 'Add',
      lblRelatedKpNum: 'Related Keyphrase #{n}',
      btnRemove: 'Remove',
      phRelatedKp: 'Related keyphrase…',
      lblGooglePreview: 'Google Search Preview & Length',
      defaultSeoTitle: 'Your Article SEO Title',
      defaultSerpDesc: 'Please provide a meta description by editing the snippet field in the composer.',
      lblSeoTitleWidth: 'SEO Title Width',
      lblMetaDescShort: 'Meta Description',
      charsUnit: 'chars',
      cannibalAlert: 'Keyphrase Cannibalization Alert',
      usedBy: 'Used by:',
      lblAnalysisResults: 'Analysis results',
      wordsDensity: '{w} words · {d}% density',
      grpProblems: 'Problems ({n})',
      grpImprovements: 'Improvements ({n})',
      grpGood: 'Good results ({n})',
      eyeHighlightTitle: 'Highlight occurrences in article preview',
      contribHeading: 'Editorial Contributors ({n})',
      thName: 'Name',
      thSlug: 'Slug',
      contribCreateHeading: '+ Create Contributor',
      lblFullName: 'Full Name',
      lblOccupation: 'Occupation',
      lblInstitution: 'Institution',
      lblShortBio: 'Short Bio HTML (appears in aside)',
      lblFullBio: 'Full Bio HTML',
      btnSaveContrib: 'Save Contributor',
      toastContribAdded: 'Contributor added!',
      confirmDelContrib: 'Are you sure you want to delete this contributor?',
      toastContribDeleted: 'Contributor deleted.',
      loadingComments: 'Loading comments…',
      modHeading: 'Comment Moderation ({n})',
      modAll: 'All',
      modReported: 'Reported Only',
      modModerated: 'Moderated Only',
      thReaderArticle: 'Reader & Article',
      thComment: 'Comment',
      thSignals: 'Signals',
      lblLikes: 'Likes',
      lblReplies: 'Replies',
      lblReports: 'Reports',
      lblReason: 'Reason:',
      stHidden: 'Hidden',
      stVisible: 'Visible',
      badgeFeatured: 'Featured',
      btnRestore: 'Restore',
      btnHide: 'Hide',
      btnFeature: 'Feature',
      btnUnfeature: 'Unfeature',
      noCommentsFound: 'No comments found.',
      promptHideReason: 'Moderation reason:',
      defaultHideReason: 'Violates Community Guidelines',
      toastCommentHidden: 'Comment hidden.',
      toastCommentRestored: 'Comment restored.',
      confirmDelComment: 'Are you sure you want to delete this comment?',
      loadingSubmissions: 'Loading submissions…',
      tabPitches: 'Editorial Pitches ({n})',
      tabSyndications: 'Syndication Requests ({n})',
      thAuthor: 'Author',
      thPitchTitleField: 'Pitch Title & Field',
      thDecision: 'Decision',
      thApplicant: 'Applicant',
      thPubArticle: 'Publication & Article',
      btnApprove: 'Approve',
      btnReject: 'Reject',
      noPitches: 'No pitch submissions.',
      noSyndications: 'No syndication requests.',
      promptEditorNotes: 'Editor notes (optional):',
      toastStatusUpdated: 'Status updated.',
      mediaHeading: 'Upload Editorial Media (wwwroot/uploads)',
      folderImages: 'Images',
      folderVideos: 'Videos',
      folderAudio: 'Audio',
      folderAvatars: 'Avatars',
      btnUploadFile: 'Upload File',
      btnCopyUrl: 'Copy URL',
      noMediaSession: 'No files uploaded in this session yet. Select a file to upload.',
      toastFileUploaded: 'File uploaded: ',
      toastUrlCopied: 'URL copied!',
      toastFileDeleted: 'File deleted.',
      loadingUsers: 'Loading users…',
      usersHeading: 'Users & Roles ({n})',
      phUserSearch: 'Name or email…',
      btnSearch: 'Search',
      btnCreateRole: '+ Create Role',
      thUser: 'User',
      thRoles: 'Roles',
      thStatusGl: 'Status & Guidelines',
      thManage: 'Manage',
      btnAssign: '+ Assign',
      stActive: 'Active',
      stInactive: 'Inactive',
      lblGuidelines: 'Guidelines:',
      glAccepted: 'Accepted',
      glPending: 'Pending',
      btnActivate: 'Activate',
      btnDeactivate: 'Deactivate',
      promptNewRole: 'New role name:',
      toastRoleCreated: 'Role created!',
      toastRoleAssigned: 'Role assigned.',
      toastRoleRemoved: 'Role removed.',
      confirmDelUser: 'Are you sure you want to delete this user?'
    }
  };

  var Admin = {
    lang: loadInitialLang(),
    tab: 'dashboard',
    editingArticleId: null,
    catalog: null,
    contributors: [],
    uploadedMedia: [],
    composer: null,
    seoTimer: null,
    activeHighlightRuleId: null,
    previewMode: false
  };

  function t(key, vars) {
    var dict = I18N[Admin.lang] || I18N.az;
    var str = dict[key] !== undefined ? dict[key] : (I18N.en[key] !== undefined ? I18N.en[key] : key);
    if (vars) {
      Object.keys(vars).forEach(function (k) {
        str = str.replace(new RegExp('\\{' + k + '\\}', 'g'), String(vars[k]));
      });
    }
    return str;
  }

  function langSwitcherHtml() {
    return '<div class="lang-switcher" role="group" aria-label="Language">' +
      '<button type="button" class="lang-btn' + (Admin.lang === 'az' ? ' is-active' : '') + '" data-set-lang="az">AZ</button>' +
      '<button type="button" class="lang-btn' + (Admin.lang === 'en' ? ' is-active' : '') + '" data-set-lang="en">EN</button>' +
    '</div>';
  }

  function setLang(next) {
    if (next !== 'az' && next !== 'en') return;
    if (Admin.tab === 'composer') syncComposerFromDom();
    Admin.lang = next;
    try { global.localStorage.setItem(LANG_KEY, next); } catch (_) {}
    document.documentElement.lang = next;
    document.title = t('pageTitle');
    if (!Rana.state.user || !Rana.auth.hasStaffRole()) {
      renderAuthGate();
    } else {
      renderShell();
      loadActiveTab();
    }
  }

  function bindLangSwitchers(scopeEl) {
    (scopeEl || document).querySelectorAll('[data-set-lang]').forEach(function (b) {
      b.addEventListener('click', function () {
        setLang(b.getAttribute('data-set-lang'));
      });
    });
  }

  function rootEl() { return document.getElementById('admin-root'); }

  function defaultComposer() {
    return {
      title: '',
      slug: '',
      type: 'Essay',
      status: 'Draft',
      standfirstShort: '',
      standfirstLong: '',
      bodyHtml: '<p></p>',
      metaTitle: '',
      metaDescription: '',
      sectionId: (Admin.catalog && Admin.catalog[0] && Admin.catalog[0].id) || '',
      primaryTopicId: '',
      topicIds: [],
      tagNames: [],
      authorIds: [],
      editorId: '',
      isHomepageFeatured: false,
      featuredImageUrl: '',
      featuredImageAlt: '',
      featuredImageCaptionHtml: '',
      cardImageUrl: '',
      cardImageAlt: '',
      settings: { alignX: 'center', alignY: 'center', backdropStrength: 2, featureImgCaptionAlign: 'right' },
      audioEnabled: false,
      audioUrl: '',
      audioDurationSeconds: 0,
      videoHoster: 'None',
      videoEmbedUrl: '',
      videoDurationMinutes: 0,
      creditsShort: '',
      creditsHtml: '',
      republicationAllowed: true,
      commercialUseAllowed: true,
      commentsEnabled: true,
      commentsOpen: true,
      scheduledAt: '',
      focusKeyphrase: '',
      synonyms: [],
      relatedKeyphrases: [],
      seoResult: null,
      showSynonymsInput: false
    };
  }

  /* ---------------------- AUTH GATE & BOOTSTRAP ---------------------- */
  async function boot() {
    document.documentElement.lang = Admin.lang;
    document.title = t('pageTitle');
    await Rana.auth.initSession();
    if (!Rana.state.user || !Rana.auth.hasStaffRole()) {
      renderAuthGate();
      return;
    }
    try {
      Admin.catalog = await Rana.api.getSectionsCatalog();
    } catch (_) {
      Admin.catalog = Rana.demo.sections;
    }
    try {
      Admin.contributors = await Rana.api.adminGetContributors();
    } catch (_) {
      Admin.contributors = Object.values(Rana.demo.contributors);
    }
    if (!Admin.composer) Admin.composer = defaultComposer();
    renderShell();
    loadActiveTab();
  }

  function renderAuthGate() {
    var root = rootEl();
    if (!root) return;
    var hasUserButNoRole = !!(Rana.state.user && !Rana.auth.hasStaffRole());
    root.innerHTML =
      '<div class="admin-auth-gate">' +
        '<div class="admin-auth-card">' +
          '<div style="display:flex;justify-content:space-between;align-items:flex-start;gap:12px;margin-bottom:10px;">' +
            '<div>' + UI.ICONS.ranaWordmark() + '</div>' +
            langSwitcherHtml() +
          '</div>' +
          '<div style="font-family:var(--font-mono);font-size:11px;letter-spacing:2px;text-transform:uppercase;color:var(--ink-mid);margin-bottom:24px;">' + U.escapeHtml(t('authSub')) + '</div>' +
          (hasUserButNoRole
            ? '<div style="padding:12px 14px;background:var(--seo-red-bg);border:1px solid #fecaca;color:#991b1b;font-size:13.5px;margin-bottom:18px;">' + U.escapeHtml(t('authNoRole')) + '</div>'
            : '') +
          '<form id="admin-login-form" class="form-grid">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('emailLabel')) + '</label><input type="email" name="email" id="admin-email-in" required class="field-input" placeholder="admin@ranamedia.co" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('passwordLabel')) + '</label><input type="password" name="password" id="admin-pass-in" required class="field-input" placeholder="••••••••" /></div>' +
            '<div id="admin-login-err" style="color:var(--seo-red);font-size:13.5px;"></div>' +
            '<button type="submit" class="aeon-btn" style="width:100%;">' + U.escapeHtml(t('enterStudio')) + '</button>' +
          '</form>' +
          '<div style="margin-top:20px;padding-top:16px;border-top:1px solid var(--border-hairline);display:flex;justify-content:space-between;align-items:center;font-family:var(--font-mono);font-size:11.5px;gap:8px;flex-wrap:wrap;">' +
            '<a href="../index.html" style="text-decoration:underline;">' + U.escapeHtml(t('backToMag')) + '</a>' +
            (hasUserButNoRole ? '<button type="button" id="admin-switch-acct" style="text-decoration:underline;color:var(--aeon-red);">' + U.escapeHtml(t('signOut')) + '</button>' : '<button type="button" id="fill-seed-hint" style="text-decoration:underline;color:var(--ink-mid);">' + U.escapeHtml(t('fillAdmin')) + '</button>') +
          '</div>' +
          '<div style="margin-top:10px;display:flex;justify-content:space-between;align-items:center;font-family:var(--font-mono);font-size:11px;color:var(--ink-mid);">' +
            '<span>API: ' + U.escapeHtml(Rana.config.API_BASE) + '</span>' +
            '<button type="button" id="admin-change-api" style="text-decoration:underline;color:var(--brand-primary);">' + U.escapeHtml(t('changeApi')) + '</button>' +
          '</div>' +
        '</div>' +
      '</div>';

    bindLangSwitchers(root);

    var changeApiBtn = document.getElementById('admin-change-api');
    if (changeApiBtn) {
      changeApiBtn.addEventListener('click', async function () {
        var next = await UI.promptModal({
          title: t('changeApi'),
          message: t('promptApiUrl'),
          defaultValue: Rana.config.API_BASE,
          required: true
        });
        if (next && next.trim()) {
          Rana.config.setApiBase(next);
          boot();
        }
      });
    }

    var fillBtn = document.getElementById('fill-seed-hint');
    if (fillBtn) {
      fillBtn.addEventListener('click', function () {
        document.getElementById('admin-email-in').value = 'admin@ranamedia.co';
        document.getElementById('admin-pass-in').value = 'Admin123!';
      });
    }
    var sw = document.getElementById('admin-switch-acct');
    if (sw) sw.addEventListener('click', Rana.auth.logout);

    document.getElementById('admin-login-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var errEl = document.getElementById('admin-login-err');
      errEl.textContent = '';
      try {
        var res = await Rana.api.login({
          email: document.getElementById('admin-email-in').value.trim(),
          password: document.getElementById('admin-pass-in').value
        });
        Rana.api.setTokens(res.accessToken, res.refreshToken);
        await boot();
      } catch (err) {
        errEl.textContent = err.message || t('loginFailed');
      }
    });
  }

  /* ---------------------- ADMIN SHELL ---------------------- */
  function renderShell() {
    var user = Rana.state.user;
    var isAdmin = Rana.auth.hasRole('Admin');
    var isEditor = Rana.auth.hasRole('Editor') || isAdmin;
    var isMod = Rana.auth.hasRole('Moderator') || isAdmin;
    var tabTitles = {
      dashboard: t('titleDashboard'),
      articles: t('titleArticles'),
      composer: Admin.editingArticleId ? t('titleComposerEdit') : t('titleComposerNew'),
      contributors: t('titleContributors'),
      moderation: t('titleModeration'),
      submissions: t('titleSubmissions'),
      media: t('titleMedia'),
      users: t('titleUsers')
    };

    var publicHref = '../index.html';

    rootEl().innerHTML =
      '<div class="admin-shell">' +
        '<aside class="admin-sidebar">' +
          '<div class="admin-brand">' +
            UI.ICONS.ranaWordmark() +
            '<div class="admin-brand__sub">' + U.escapeHtml(t('brandSub')) + '</div>' +
          '</div>' +
          '<nav>' +
            '<div class="admin-nav-group">' +
              '<div class="admin-nav-label">' + U.escapeHtml(t('navOverview')) + '</div>' +
              '<button type="button" class="admin-nav-btn' + (Admin.tab === 'dashboard' ? ' is-active' : '') + '" data-admin-tab="dashboard">' + U.escapeHtml(t('tabDashboard')) + '</button>' +
            '</div>' +
            (isEditor
              ? '<div class="admin-nav-group">' +
                  '<div class="admin-nav-label">' + U.escapeHtml(t('navEditorial')) + '</div>' +
                  '<button type="button" class="admin-nav-btn' + (Admin.tab === 'articles' ? ' is-active' : '') + '" data-admin-tab="articles">' + U.escapeHtml(t('tabArticles')) + '</button>' +
                  '<button type="button" class="admin-nav-btn' + (Admin.tab === 'composer' ? ' is-active' : '') + '" data-admin-tab="composer">' + U.escapeHtml(t('tabComposer')) + '</button>' +
                  '<button type="button" class="admin-nav-btn' + (Admin.tab === 'contributors' ? ' is-active' : '') + '" data-admin-tab="contributors">' + U.escapeHtml(t('tabContributors')) + '</button>' +
                  '<button type="button" class="admin-nav-btn' + (Admin.tab === 'media' ? ' is-active' : '') + '" data-admin-tab="media">' + U.escapeHtml(t('tabMedia')) + '</button>' +
                '</div>'
              : '') +
            '<div class="admin-nav-group">' +
              '<div class="admin-nav-label">' + U.escapeHtml(t('navCommunity')) + '</div>' +
              (isMod ? '<button type="button" class="admin-nav-btn' + (Admin.tab === 'moderation' ? ' is-active' : '') + '" data-admin-tab="moderation">' + U.escapeHtml(t('tabModeration')) + '</button>' : '') +
              (isEditor ? '<button type="button" class="admin-nav-btn' + (Admin.tab === 'submissions' ? ' is-active' : '') + '" data-admin-tab="submissions">' + U.escapeHtml(t('tabSubmissions')) + '</button>' : '') +
            '</div>' +
            (isAdmin
              ? '<div class="admin-nav-group">' +
                  '<div class="admin-nav-label">' + U.escapeHtml(t('navSystem')) + '</div>' +
                  '<button type="button" class="admin-nav-btn' + (Admin.tab === 'users' ? ' is-active' : '') + '" data-admin-tab="users">' + U.escapeHtml(t('tabUsers')) + '</button>' +
                '</div>'
              : '') +
          '</nav>' +
          '<div class="admin-sidebar__footer">' +
            '<a href="' + publicHref + '" target="_blank">' + U.escapeHtml(t('viewPublicMag')) + '</a>' +
            '<button type="button" id="admin-logout-btn" style="text-align:left;color:var(--aeon-red);">' + U.escapeHtml(t('signOut')) + '</button>' +
          '</div>' +
        '</aside>' +
        '<div class="admin-main">' +
          '<header class="admin-topbar">' +
            '<h1 class="admin-topbar__title">' + U.escapeHtml(tabTitles[Admin.tab] || 'Studio') + '</h1>' +
            '<div class="admin-topbar__user">' +
              langSwitcherHtml() +
              '<span>' + U.escapeHtml(user.fullName || user.email) + '</span>' +
              (user.roleName || []).map(function (r) { return '<span class="admin-role-chip">' + U.escapeHtml(r) + '</span>'; }).join('') +
            '</div>' +
          '</header>' +
          '<div class="admin-workspace" id="admin-workspace"></div>' +
        '</div>' +
      '</div>';

    bindLangSwitchers(rootEl());
    document.getElementById('admin-logout-btn').addEventListener('click', Rana.auth.logout);
    document.querySelectorAll('[data-admin-tab]').forEach(function (btn) {
      btn.addEventListener('click', function () {
        var next = btn.getAttribute('data-admin-tab');
        if (next === 'composer' && Admin.tab !== 'composer') {
          Admin.editingArticleId = null;
          Admin.composer = defaultComposer();
          Admin.activeHighlightRuleId = null;
          Admin.previewMode = false;
        }
        Admin.tab = next;
        renderShell();
        loadActiveTab();
      });
    });
  }

  function wsEl() { return document.getElementById('admin-workspace'); }

  function loadActiveTab() {
    switch (Admin.tab) {
      case 'dashboard': return loadDashboard();
      case 'articles': return loadArticlesList();
      case 'composer': return renderComposer();
      case 'contributors': return loadContributorsTab();
      case 'moderation': return loadModerationTab();
      case 'submissions': return loadSubmissionsTab();
      case 'media': return loadMediaTab();
      case 'users': return loadUsersTab();
      default: return loadDashboard();
    }
  }

  /* ---------------------- 1. DASHBOARD ---------------------- */
  async function loadDashboard() {
    wsEl().innerHTML = '<p style="font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(t('loadingStats')) + '</p>';
    try {
      var stats = await Rana.api.adminGetStats();
      var totalSeo = (stats.greenSeoCount || 0) + (stats.orangeSeoCount || 0) + (stats.redSeoCount || 0);

      wsEl().innerHTML =
        '<div class="kpi-grid">' +
          '<div class="kpi-card"><div class="kpi-card__label">' + U.escapeHtml(t('kpiPublished')) + '</div><div class="kpi-card__value">' + stats.publishedArticles + '</div><div class="kpi-card__sub">' + U.escapeHtml(t('kpiDraft')) + ': ' + stats.draftArticles + ' · ' + U.escapeHtml(t('kpiScheduled')) + ': ' + stats.scheduledArticles + '</div></div>' +
          '<div class="kpi-card"><div class="kpi-card__label">' + U.escapeHtml(t('kpiViews')) + '</div><div class="kpi-card__value">' + Number(stats.totalViews || 0).toLocaleString() + '</div><div class="kpi-card__sub">' + U.escapeHtml(t('kpiViewsSub', { n: stats.totalArticles })) + '</div></div>' +
          '<div class="kpi-card"><div class="kpi-card__label">' + U.escapeHtml(t('kpiAvgSeo')) + '</div><div class="kpi-card__value">' + stats.averageSeoScore + '<span style="font-size:18px;color:var(--ink-mid);">/100</span></div><div class="kpi-card__sub">' + U.escapeHtml(t('kpiAnalyzedSub', { n: totalSeo })) + '</div></div>' +
          '<div class="kpi-card"><div class="kpi-card__label">' + U.escapeHtml(t('kpiComments')) + '</div><div class="kpi-card__value">' + stats.totalComments + '</div><div class="kpi-card__sub">' + U.escapeHtml(t('kpiModerated')) + ': ' + stats.moderatedComments + '</div></div>' +
          '<div class="kpi-card"><div class="kpi-card__label">' + U.escapeHtml(t('kpiPending')) + '</div><div class="kpi-card__value">' + (stats.pendingPitches + stats.pendingSyndications) + '</div><div class="kpi-card__sub">' + U.escapeHtml(t('kpiPitches')) + ': ' + stats.pendingPitches + ' · ' + U.escapeHtml(t('kpiSyndication')) + ': ' + stats.pendingSyndications + '</div></div>' +
          '<div class="kpi-card"><div class="kpi-card__label">' + U.escapeHtml(t('kpiUsers')) + '</div><div class="kpi-card__value">' + stats.totalUsers + '</div><div class="kpi-card__sub">' + U.escapeHtml(t('kpiSubscribers')) + ': ' + stats.activeSubscribers + '</div></div>' +
        '</div>' +
        '<div class="paper-panel">' +
          '<div class="paper-panel__header"><h2 class="paper-panel__title">' + U.escapeHtml(t('seoHealthTitle')) + '</h2><button type="button" class="aeon-btn" id="dash-new-article">' + U.escapeHtml(t('btnComposeArticle')) + '</button></div>' +
          '<div style="display:flex;flex-wrap:wrap;gap:24px;align-items:center;">' +
            '<span class="status-pill status-pill--Green"><span class="seo-dot seo-dot--Green"></span> ' + U.escapeHtml(t('seoGoodGreen')) + ': ' + stats.greenSeoCount + '</span>' +
            '<span class="status-pill status-pill--Orange"><span class="seo-dot seo-dot--Orange"></span> ' + U.escapeHtml(t('seoOkOrange')) + ': ' + stats.orangeSeoCount + '</span>' +
            '<span class="status-pill status-pill--Red"><span class="seo-dot seo-dot--Red"></span> ' + U.escapeHtml(t('seoRedWork')) + ': ' + stats.redSeoCount + '</span>' +
          '</div>' +
        '</div>';

      document.getElementById('dash-new-article').addEventListener('click', function () {
        Admin.editingArticleId = null;
        Admin.composer = defaultComposer();
        Admin.tab = 'composer';
        renderShell();
        renderComposer();
      });
    } catch (err) {
      wsEl().innerHTML = '<div class="paper-panel"><p style="color:var(--seo-red);">' + U.escapeHtml(err.message) + '</p></div>';
    }
  }

  /* ---------------------- 2. ARTICLES LIST ---------------------- */
  async function loadArticlesList(pageNumber) {
    var filters = { status: '', type: '', search: '', pageNumber: pageNumber || 1, pageSize: 15 };
    async function fetchAndRender() {
      wsEl().innerHTML = '<p style="font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(t('loadingArticles')) + '</p>';
      try {
        var page = await Rana.api.adminGetArticles(filters);
        var items = page.items || [];
        var publicHrefBase = '../index.html';

        wsEl().innerHTML =
          '<div class="paper-panel">' +
            '<div class="paper-panel__header">' +
              '<div style="display:flex;flex-wrap:wrap;gap:10px;align-items:center;">' +
                '<input type="search" id="art-filter-search" class="field-input" style="width:240px;" placeholder="' + U.escapeHtml(t('phSearchArticle')) + '" value="' + U.escapeHtml(filters.search) + '" />' +
                '<select id="art-filter-status" class="field-select" style="width:150px;"><option value="">' + U.escapeHtml(t('allStatus')) + '</option><option value="Published">' + U.escapeHtml(t('stPublished')) + '</option><option value="Draft">' + U.escapeHtml(t('stDraft')) + '</option><option value="Scheduled">' + U.escapeHtml(t('stScheduled')) + '</option><option value="Archived">' + U.escapeHtml(t('stArchived')) + '</option></select>' +
                '<select id="art-filter-type" class="field-select" style="width:135px;"><option value="">' + U.escapeHtml(t('allTypes')) + '</option><option value="Essay">' + U.escapeHtml(t('tpEssay')) + '</option><option value="Video">' + U.escapeHtml(t('tpVideo')) + '</option><option value="Idea">' + U.escapeHtml(t('tpIdea')) + '</option><option value="Classic">' + U.escapeHtml(t('tpClassic')) + '</option></select>' +
                '<button type="button" id="art-filter-apply" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(t('btnFilter')) + '</button>' +
              '</div>' +
              '<button type="button" id="art-create-btn" class="aeon-btn">' + U.escapeHtml(t('btnNewArticle')) + '</button>' +
            '</div>' +
            '<div class="admin-table-wrap">' +
              '<table class="admin-table">' +
                '<thead><tr><th>' + U.escapeHtml(t('thTitleSection')) + '</th><th>' + U.escapeHtml(t('thType')) + '</th><th>' + U.escapeHtml(t('thStatus')) + '</th><th>' + U.escapeHtml(t('thSeoLight')) + '</th><th>' + U.escapeHtml(t('thViews')) + '</th><th>' + U.escapeHtml(t('thComments')) + '</th><th>' + U.escapeHtml(t('thActions')) + '</th></tr></thead>' +
                '<tbody>' +
                  (items.length
                    ? items.map(function (a) {
                        var seoCls = a.seoStatus || 'Red';
                        var artUrl = publicHrefBase + (a.type === 'Video' ? '#/video/' : '#/essay/') + encodeURIComponent(a.slug);
                        return '<tr>' +
                          '<td><div style="font-family:var(--font-serif);font-size:18px;font-weight:700;">' + U.escapeHtml(a.title) + (a.isHomepageFeatured ? ' <span class="comment-badge">' + U.escapeHtml(t('heroBadge')) + '</span>' : '') + '</div>' +
                            '<div style="font-family:var(--font-mono);font-size:11.5px;color:var(--ink-mid);margin-top:3px;"><span style="color:' + U.escapeHtml(a.sectionColor) + ';font-weight:600;">' + U.escapeHtml(a.sectionTitle) + '</span> / ' + U.escapeHtml(a.slug) + '</div></td>' +
                          '<td>' + U.escapeHtml(a.type) + '</td>' +
                          '<td><span class="status-pill status-pill--' + a.status + '">' + U.escapeHtml(a.status) + '</span></td>' +
                          '<td>' + (a.seoScore !== null && a.seoScore !== undefined
                            ? '<span class="status-pill status-pill--' + seoCls + '"><span class="seo-dot seo-dot--' + seoCls + '"></span>' + a.seoScore + '/100</span><div style="font-size:11px;color:var(--ink-mid);margin-top:3px;">' + U.escapeHtml(a.focusKeyphrase || '') + '</div>'
                            : '<span style="font-size:12px;color:var(--ink-light);">' + U.escapeHtml(t('notAnalyzed')) + '</span>') + '</td>' +
                          '<td style="font-family:var(--font-mono);">' + (a.viewsCount || 0) + '</td>' +
                          '<td style="font-family:var(--font-mono);">' + (a.commentAndRepliesCount || 0) + '</td>' +
                          '<td style="white-space:nowrap;">' +
                            '<button type="button" class="aeon-btn aeon-btn--secondary" data-edit-art="' + a.id + '" style="margin-right:6px;">' + U.escapeHtml(t('btnEditSeo')) + '</button>' +
                            '<a href="' + artUrl + '" target="_blank" class="aeon-btn aeon-btn--ghost" style="padding:7px 10px;font-size:12px;margin-right:6px;">' + U.escapeHtml(t('btnView')) + '</a>' +
                            '<button type="button" class="aeon-btn aeon-btn--ghost" data-del-art="' + a.id + '" style="padding:7px 10px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('btnDelete')) + '</button>' +
                          '</td>' +
                        '</tr>';
                      }).join('')
                    : '<tr><td colspan="7" style="text-align:center;color:var(--ink-mid);padding:32px;">' + U.escapeHtml(t('noArticlesFound')) + '</td></tr>') +
                '</tbody>' +
              '</table>' +
            '</div>' +
            '<div style="display:flex;justify-content:space-between;align-items:center;margin-top:16px;font-family:var(--font-mono);font-size:12px;">' +
              '<span>' + U.escapeHtml(t('pageSummary', { total: page.totalCount, page: page.pageNumber, pages: Math.max(1, page.totalPages) })) + '</span>' +
              '<div style="display:flex;gap:8px;">' +
                '<button type="button" class="aeon-btn aeon-btn--secondary" id="art-prev"' + (!page.hasPreviousPage ? ' disabled' : '') + '>' + U.escapeHtml(t('btnPrev')) + '</button>' +
                '<button type="button" class="aeon-btn aeon-btn--secondary" id="art-next"' + (!page.hasNextPage ? ' disabled' : '') + '>' + U.escapeHtml(t('btnNext')) + '</button>' +
              '</div>' +
            '</div>' +
          '</div>';

        document.getElementById('art-filter-status').value = filters.status;
        document.getElementById('art-filter-type').value = filters.type;
        document.getElementById('art-filter-apply').addEventListener('click', function () {
          filters.search = document.getElementById('art-filter-search').value.trim();
          filters.status = document.getElementById('art-filter-status').value;
          filters.type = document.getElementById('art-filter-type').value;
          filters.pageNumber = 1;
          fetchAndRender();
        });
        document.getElementById('art-prev').addEventListener('click', function () { filters.pageNumber--; fetchAndRender(); });
        document.getElementById('art-next').addEventListener('click', function () { filters.pageNumber++; fetchAndRender(); });
        document.getElementById('art-create-btn').addEventListener('click', function () {
          Admin.editingArticleId = null;
          Admin.composer = defaultComposer();
          Admin.tab = 'composer';
          renderShell();
          renderComposer();
        });
        wsEl().querySelectorAll('[data-edit-art]').forEach(function (b) {
          b.addEventListener('click', function () { openArticleForEdit(b.getAttribute('data-edit-art')); });
        });
        wsEl().querySelectorAll('[data-del-art]').forEach(function (b) {
          b.addEventListener('click', async function () {
            if (!(await UI.confirmModal({ message: t('confirmDelArticle'), danger: true }))) return;
            try {
              await Rana.api.adminDeleteArticle(b.getAttribute('data-del-art'));
              UI.toast(t('toastArticleDeleted'));
              fetchAndRender();
            } catch (err) { UI.toast(err.message, 'warn'); }
          });
        });
      } catch (err) {
        wsEl().innerHTML = '<div class="paper-panel"><p style="color:var(--seo-red);">' + U.escapeHtml(err.message) + '</p></div>';
      }
    }
    fetchAndRender();
  }

  async function openArticleForEdit(id) {
    wsEl().innerHTML = '<p style="font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(t('loadingArticleEdit')) + '</p>';
    try {
      var dto = await Rana.api.adminGetArticle(id);
      Admin.editingArticleId = id;
      Admin.activeHighlightRuleId = null;
      Admin.previewMode = false;
      Admin.composer = Object.assign(defaultComposer(), dto, {
        seoResult: dto.seoAnalysis || null,
        showSynonymsInput: !!(dto.synonyms && dto.synonyms.length)
      });
      Admin.tab = 'composer';
      renderShell();
      renderComposer();
      if (!Admin.composer.seoResult) scheduleLiveSeo();
    } catch (err) {
      UI.toast(err.message, 'warn');
      loadArticlesList();
    }
  }

  /* ---------------------- 3. EDITORIAL COMPOSER + YOAST SEO PANEL ---------------------- */
  function syncComposerFromDom() {
    var c = Admin.composer;
    if (!c) return;
    var g = function (id) { return document.getElementById(id); };
    if (g('cmp-title')) c.title = g('cmp-title').value;
    if (g('cmp-slug')) c.slug = g('cmp-slug').value.trim().toLowerCase();
    if (g('cmp-type')) c.type = g('cmp-type').value;
    if (g('cmp-status')) c.status = g('cmp-status').value;
    if (g('cmp-section')) c.sectionId = g('cmp-section').value;
    if (g('cmp-topic')) c.primaryTopicId = g('cmp-topic').value;
    if (g('cmp-short')) c.standfirstShort = g('cmp-short').value;
    if (g('cmp-long')) c.standfirstLong = g('cmp-long').value;
    if (g('cmp-body')) c.bodyHtml = g('cmp-body').value;
    if (g('cmp-meta-title')) c.metaTitle = g('cmp-meta-title').value;
    if (g('cmp-meta-desc')) c.metaDescription = g('cmp-meta-desc').value;
    if (g('cmp-tags')) c.tagNames = g('cmp-tags').value.split(',').map(function (s) { return s.trim(); }).filter(Boolean);
    if (g('cmp-editor')) c.editorId = g('cmp-editor').value;
    if (g('cmp-feat-img')) c.featuredImageUrl = g('cmp-feat-img').value.trim();
    if (g('cmp-feat-alt')) c.featuredImageAlt = g('cmp-feat-alt').value.trim();
    if (g('cmp-card-img')) c.cardImageUrl = g('cmp-card-img').value.trim();
    if (g('cmp-card-alt')) c.cardImageAlt = g('cmp-card-alt').value.trim();
    if (g('cmp-align-x')) c.settings.alignX = g('cmp-align-x').value;
    if (g('cmp-align-y')) c.settings.alignY = g('cmp-align-y').value;
    if (g('cmp-backdrop')) c.settings.backdropStrength = Number(g('cmp-backdrop').value || 2);
    if (g('cmp-audio-on')) c.audioEnabled = g('cmp-audio-on').checked;
    if (g('cmp-audio-url')) c.audioUrl = g('cmp-audio-url').value.trim();
    if (g('cmp-audio-sec')) c.audioDurationSeconds = Number(g('cmp-audio-sec').value || 0);
    if (g('cmp-vid-hoster')) c.videoHoster = g('cmp-vid-hoster').value;
    if (g('cmp-vid-url')) c.videoEmbedUrl = g('cmp-vid-url').value.trim();
    if (g('cmp-vid-min')) c.videoDurationMinutes = Number(g('cmp-vid-min').value || 0);
    if (g('cmp-credits-short')) c.creditsShort = g('cmp-credits-short').value.trim();
    if (g('cmp-featured-home')) c.isHomepageFeatured = g('cmp-featured-home').checked;
    if (g('cmp-cmt-enabled')) c.commentsEnabled = g('cmp-cmt-enabled').checked;
    if (g('cmp-cmt-open')) c.commentsOpen = g('cmp-cmt-open').checked;
    if (g('cmp-repub')) c.republicationAllowed = g('cmp-repub').checked;
    if (g('cmp-focus-kp')) c.focusKeyphrase = g('cmp-focus-kp').value;
    var checkedAuthors = Array.from(document.querySelectorAll('input[name="cmp-authors"]:checked')).map(function (el) { return el.value; });
    if (document.querySelector('input[name="cmp-authors"]')) c.authorIds = checkedAuthors;
  }

  function buildSeoRequest() {
    var c = Admin.composer;
    return {
      articleId: Admin.editingArticleId || null,
      focusKeyphrase: (c.focusKeyphrase || '').trim(),
      synonyms: c.synonyms || [],
      relatedKeyphrases: (c.relatedKeyphrases || []).filter(function (rk) { return rk.keyphrase && rk.keyphrase.trim(); }),
      seoTitle: (c.metaTitle || c.title || '').trim(),
      slug: (c.slug || U.slugify(c.title)).trim(),
      metaDescription: (c.metaDescription || '').trim() || null,
      bodyHtml: c.bodyHtml || '',
      featuredImageUrl: c.featuredImageUrl || c.cardImageUrl || null,
      featuredImageAlt: c.featuredImageAlt || c.cardImageAlt || null
    };
  }

  function scheduleLiveSeo() {
    if (Admin.seoTimer) clearTimeout(Admin.seoTimer);
    Admin.seoTimer = setTimeout(runLiveSeo, 420);
  }

  async function runLiveSeo() {
    syncComposerFromDom();
    var req = buildSeoRequest();
    try {
      Admin.composer.seoResult = await Rana.api.adminAnalyzeSeoLive(req);
    } catch (err) {
      if (err.isNetworkError) Admin.composer.seoResult = Rana.demo.analyzeSeoOffline(req);
    }
    renderYoastCard();
    if (Admin.previewMode) renderBodyPreview();
  }

  function applyHighlightMarkup(bodyHtml) {
    var c = Admin.composer;
    var seo = c.seoResult;
    if (!Admin.activeHighlightRuleId || !seo || !c.focusKeyphrase) return bodyHtml;

    var phrases = [c.focusKeyphrase.trim()].concat(c.synonyms || []).filter(Boolean);
    var div = document.createElement('div');
    div.innerHTML = bodyHtml || '';

    var targets = Array.from(div.querySelectorAll(Admin.activeHighlightRuleId === 'keyphrase_in_introduction' ? 'p:first-of-type' : 'p, h2, h3'));
    var escaped = phrases.map(function (p) { return p.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); });
    if (!escaped.length) return bodyHtml;
    var rx = new RegExp('(' + escaped.join('|') + ')', 'gi');

    targets.forEach(function (node) {
      var walker = document.createTreeWalker(node, NodeFilter.SHOW_TEXT, null);
      var textNodes = [];
      var cur;
      while ((cur = walker.nextNode())) textNodes.push(cur);
      textNodes.forEach(function (tn) {
        if (!rx.test(tn.nodeValue)) return;
        rx.lastIndex = 0;
        var span = document.createElement('span');
        span.innerHTML = U.escapeHtml(tn.nodeValue).replace(rx, '<mark class="seo-hl-active">$1</mark>');
        tn.parentNode.replaceChild(span, tn);
      });
    });

    return div.innerHTML;
  }

  function renderBodyPreview() {
    var box = document.getElementById('cmp-preview-box');
    if (!box) return;
    var html = applyHighlightMarkup(U.resolveBodyMediaUrls(Admin.composer.bodyHtml || ''));
    box.innerHTML = '<div class="article-prose">' + html + '</div>';
    if (Admin.activeHighlightRuleId) {
      var firstMark = box.querySelector('mark.seo-hl-active');
      if (firstMark) firstMark.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  function renderYoastCard() {
    var mount = document.getElementById('yoast-card-mount');
    if (!mount) return;

    var active = document.activeElement;
    var focusMeta = null;
    if (active && mount.contains(active)) {
      var rkAttr = active.getAttribute('data-rk-input');
      if (active.id === 'cmp-focus-kp' || active.id === 'yoast-syn-input') {
        focusMeta = { id: active.id, start: active.selectionStart, end: active.selectionEnd };
      } else if (rkAttr !== null) {
        focusMeta = { rkIdx: rkAttr, start: active.selectionStart, end: active.selectionEnd };
      }
    }
    var synDraft = (document.getElementById('yoast-syn-input') || {}).value || '';

    var c = Admin.composer;
    var seo = c.seoResult || Rana.demo.analyzeSeoOffline(buildSeoRequest());
    var status = seo.status || 'Red';
    var seoTitle = (c.metaTitle || c.title || t('defaultSeoTitle'));
    var slugPreview = (c.slug || U.slugify(c.title) || 'article-slug');
    var metaDesc = (c.metaDescription || '').trim();
    var m = seo.metrics || {};

    var titlePx = m.seoTitlePixelWidth || (seoTitle.length * 9);
    var titlePct = Math.min(100, Math.round((titlePx / 600) * 100));
    var titleBarColor = titlePx === 0 ? '#d1d5dc' : (titlePx <= 600 ? '#7ad03a' : '#dc3232');

    var descLen = metaDesc.length;
    var descPct = Math.min(100, Math.round((descLen / 156) * 100));
    var descBarColor = descLen === 0 ? '#d1d5dc' : (descLen >= 120 && descLen <= 156 ? '#7ad03a' : '#ee7c1b');

    function localizeSeoCheck(item, severity) {
      if (Admin.lang !== 'az') return { title: item.title, description: item.description };
      var rid = item.ruleId || '';
      var d = item.description || '';
      var nums = d.match(/\d+/g) || [];
      switch (rid) {
        case 'keyphrase_length':
          return {
            title: 'Açar ifadə uzunluğu',
            description: severity === 'Good'
              ? 'Əla iş!'
              : severity === 'Improvement'
                ? 'Açar ifadə bir qədər uzundur (4 sözdən çoxdur). Daha qısa əsas açar ifadə seçin.'
                : (/No focus|Set a focus/i.test(d)
                    ? 'Əsas açar ifadə təyin edilməyib. Məqalə üçün əsas açar ifadə daxil edin.'
                    : 'Açar ifadə çox uzundur (6+ söz). Qısaldılması tövsiyə olunur.')
          };
        case 'keyphrase_in_seo_title':
          return {
            title: 'SEO başlığında açar ifadə',
            description: severity === 'Good'
              ? 'Əsas açar ifadənin dəqiq uyğunluğu SEO başlığının tərkibində yer alır. Əla!'
              : severity === 'Improvement'
                ? 'Açar ifadə SEO başlığında görünür, lakin başlığın əvvəlinə yaxın yerləşdirmək daha effektlidir.'
                : 'Əsas açar ifadənin dəqiq uyğunluğu SEO başlığında yoxdur.'
          };
        case 'seo_title_width':
          return {
            title: 'SEO başlığının eni',
            description: severity === 'Good'
              ? 'Əla iş! SEO başlığının uzunluğu axtarış nəticələri üçün idealdır.'
              : severity === 'Improvement'
                ? 'SEO başlığı bir qədər qısadır. Daha dolğun təsviri kontekst əlavə edə bilərsiniz.'
                : (/wider|600/i.test(d) || (m.seoTitlePixelWidth > 600)
                    ? 'SEO başlığı çox uzundur (600px limitini aşır) və axtarış nəticələrində kəsilə bilər.'
                    : 'SEO başlığı boşdur. Məqalə başlığı və ya SEO Meta Başlıq daxil edin.')
          };
        case 'keyphrase_in_slug':
          return {
            title: 'URL slug-da açar ifadə',
            description: severity === 'Good'
              ? 'Açar ifadəniz URL slug daxilində yer alır. Əla!'
              : severity === 'Improvement'
                ? 'Açar ifadənin yalnız bir hissəsi URL slug-da var. Tam açar ifadəni əlavə edin.'
                : 'Açar ifadəniz URL slug-da yoxdur. Açar sözləri URL slug-a əlavə edin.'
          };
        case 'meta_description_length': {
          var ml = m.metaDescriptionLength || (nums[0] ? Number(nums[0]) : 0);
          return {
            title: 'Meta təsvirin uzunluğu',
            description: severity === 'Good'
              ? 'Əla! Meta təsvirin uzunluğu idealdır (120–156 simvol).'
              : severity === 'Improvement'
                ? (ml > 156
                    ? 'Meta təsvir çox uzundur (' + ml + ' simvol). 156 simvoldan aşağı saxlayın.'
                    : 'Meta təsvir çox qısadır (' + ml + ' simvol). 120–156 simvol aralığını hədəfləyin.')
                : 'Meta təsvir təyin edilməyib. Axtarış nəticələri üçün meta təsvir yazın.'
          };
        }
        case 'keyphrase_in_meta_description':
          return {
            title: 'Meta təsvirdə açar ifadə',
            description: severity === 'Good'
              ? 'Açar ifadə və ya sinonimi meta təsvirdə yer alır. Əla!'
              : severity === 'Improvement'
                ? 'Açar ifadənin yalnız bir hissəsi meta təsvirdə var. Tam ifadəni daxil edin.'
                : 'Açar ifadə və ya sinonimi meta təsvirdə tapılmadı.'
          };
        case 'keyphrase_in_introduction':
          return {
            title: 'Giriş paraqrafında açar ifadə',
            description: severity === 'Good'
              ? 'Əsas açar ifadəniz və ya sinonimi ilk paraqrafda yer alır. Əla!'
              : 'Açar ifadəniz və ya sinonimləri ilk paraqrafda yoxdur. Mövzunu dərhal aydın göstərin.'
          };
        case 'keyphrase_density': {
          var occ = m.keyphraseOccurrences !== undefined ? m.keyphraseOccurrences : (nums[0] || 0);
          var minO = m.recommendedMinOccurrences || (nums[1] || 2);
          var maxO = m.recommendedMaxOccurrences || (nums[1] || 8);
          return {
            title: 'Açar ifadə sıxlığı',
            description: severity === 'Good'
              ? 'Açar ifadə mətndə ' + occ + ' dəfə tapıldı — bu, mətnin uzunluğuna görə idealdır.'
              : (/more than|maximum/i.test(d)
                  ? 'Açar ifadə ' + occ + ' dəfə istifadə olunub; bu, tövsiyə edilən maksimum (' + maxO + ') həddən çoxdur.'
                  : 'Açar ifadə ' + occ + ' dəfə tapıldı. Bu, mətnin uzunluğu üçün tövsiyə olunan minimum (' + minO + ' dəfə) həddən azdır!')
          };
        }
        case 'keyphrase_distribution':
          return {
            title: 'Açar ifadənin paylanması',
            description: severity === 'Good'
              ? 'Açar ifadəniz məqalə boyunca bərabər paylanıb. Əla!'
              : severity === 'Improvement'
                ? 'Açar ifadənin paylanmasını yaxşılaşdırmaq üçün mətnin digər hissələrində də istifadə edin.'
                : 'Əsas açar ifadə məqalə boyunca bərabər paylanmayıb. Məqalənin bütün hissələrinə yayın.'
          };
        case 'keyphrase_in_subheading':
          return {
            title: 'Alt başlıqlarda açar ifadə',
            description: severity === 'Good'
              ? 'H2 və H3 alt başlıqlarınızda əsas açar ifadə və ya sinonimlər yer alır. Əla!'
              : severity === 'Improvement'
                ? 'H2 və H3 alt başlıqlarında açar ifadə və ya sinonimlərdən bir qədər daha çox istifadə edin!'
                : 'H2/H3 alt başlıqlarında açar ifadə və ya sinonimlər tapılmadı. Alt başlıq əlavə edib açar sözü daxil edin.'
          };
        case 'text_length': {
          var wc = m.wordCount !== undefined ? m.wordCount : (nums[0] || 0);
          return {
            title: 'Mətnin uzunluğu',
            description: severity === 'Good'
              ? 'Mətndə ' + wc + ' söz var. Bu, tövsiyə olunan minimum 300 söz həddindən yüksəkdir. Əla!'
              : severity === 'Improvement'
                ? 'Mətndə ' + wc + ' söz var. Tövsiyə olunan minimum 300 sözdən bir qədər azdır; mətni genişləndirin.'
                : 'Mətndə ' + wc + ' söz var. Bu, tövsiyə olunan minimum 300 söz həddindən xeyli aşağıdır.'
          };
        }
        case 'outbound_links':
          return {
            title: 'Xarici keçidlər (Outbound links)',
            description: severity === 'Good'
              ? 'Səhifədə xarici keçidlər mövcuddur. Əla!'
              : 'Bu səhifədə heç bir xarici keçid yoxdur. Etibarlı mənbələrə keçid əlavə edin!'
          };
        case 'internal_links':
          return {
            title: 'Daxili keçidlər (Internal links)',
            description: severity === 'Good'
              ? 'Səhifədə daxili keçidlər mövcuddur. Əla!'
              : 'Bu səhifədə heç bir daxili keçid yoxdur; saytdakı digər məqalələrə keçid əlavə edin!'
          };
        case 'images_and_alt':
          return {
            title: 'Şəkil açar ifadəsi (Image Keyphrase)',
            description: severity === 'Good'
              ? 'Şəkillərdə əsas açar ifadə və ya sinonimləri əks etdirən alt mətnlər var. Əla!'
              : severity === 'Improvement'
                ? 'Səhifədə şəkillər var, lakin heç birinin alt atributunda əsas açar ifadə və ya sinonimlər yoxdur.'
                : 'Səhifədə şəkil yoxdur. Müvafiq şəkil və açar ifadəli alt mətn əlavə edin.'
          };
        case 'previously_used_keyphrase':
          return {
            title: 'Əvvəllər istifadə edilmiş açar ifadə',
            description: severity === 'Good'
              ? 'Bu açar ifadədən əvvəllər istifadə etməmisiniz — çox yaxşıdır!'
              : severity === 'Improvement'
                ? (/No focus/i.test(d) ? 'Açar ifadə daxil edin.' : 'Bu açar ifadə əvvəllər 1 başqa məqalədə istifadə edilib. Fərqli ifadə seçməyi düşünün.')
                : 'Bu açar ifadə əvvəllər bir neçə məqalədə istifadə edilib. SEO toqquşması (cannibalization) yarana bilər!'
          };
        default:
          return { title: item.title, description: item.description };
      }
    }

    function renderChecks(items, severity) {
      return items.map(function (item) {
        var eyeActive = Admin.activeHighlightRuleId === item.ruleId;
        var loc = localizeSeoCheck(item, severity);
        return '<li class="yoast-check-item">' +
          '<div class="yoast-check-left">' +
            '<span class="seo-dot seo-dot--' + severity + '"></span>' +
            '<div><span class="yoast-rule-title">' + U.escapeHtml(loc.title) + ':</span> ' + U.escapeHtml(loc.description) + '</div>' +
          '</div>' +
          (item.highlightable
            ? '<button type="button" class="yoast-eye-btn' + (eyeActive ? ' is-active' : '') + '" data-seo-eye="' + U.escapeHtml(item.ruleId) + '" title="' + U.escapeHtml(t('eyeHighlightTitle')) + '">' + UI.ICONS.eye(15) + '</button>'
            : '') +
        '</li>';
      }).join('');
    }

    var synonymsHtml = c.showSynonymsInput
      ? '<div style="margin-top:10px;padding:10px;background:#f4f8f5;border:1px solid var(--border-hairline);">' +
          '<label class="field-label">' + U.escapeHtml(t('lblSynonymsInput')) + '</label>' +
          '<div style="display:flex;gap:6px;margin-top:4px;">' +
            '<input type="text" id="yoast-syn-input" class="field-input" style="padding:7px 10px;font-size:13px;" placeholder="' + U.escapeHtml(t('phAddSynonym')) + '" />' +
            '<button type="button" id="yoast-syn-add" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(t('btnAdd')) + '</button>' +
          '</div>' +
          '<div class="chip-row">' +
            (c.synonyms || []).map(function (s, idx) {
              return '<span class="tag-chip">' + U.escapeHtml(s) + '<button type="button" data-del-syn="' + idx + '">×</button></span>';
            }).join('') +
          '</div>' +
        '</div>'
      : '';

    var relatedHtml = (c.relatedKeyphrases || []).map(function (rk, idx) {
      var res = (seo.relatedKeyphraseResults || [])[idx];
      var rkStatus = res ? res.status : 'Orange';
      return '<div style="margin-top:10px;padding:10px;background:#f5f9f6;border:1px solid var(--border-mid);">' +
        '<div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:6px;">' +
          '<span style="font-family:var(--font-mono);font-size:11px;display:inline-flex;align-items:center;gap:6px;"><span class="seo-dot seo-dot--' + rkStatus + '"></span>' + U.escapeHtml(t('lblRelatedKpNum', { n: idx + 1 })) + (res ? ' (' + res.score + '/100)' : '') + '</span>' +
          '<button type="button" data-del-rk="' + idx + '" style="font-family:var(--font-mono);font-size:11px;color:var(--aeon-red);">' + U.escapeHtml(t('btnRemove')) + '</button>' +
        '</div>' +
        '<input type="text" class="field-input" style="padding:7px 10px;font-size:13px;" data-rk-input="' + idx + '" value="' + U.escapeHtml(rk.keyphrase || '') + '" placeholder="' + U.escapeHtml(t('phRelatedKp')) + '" />' +
      '</div>';
    }).join('');

    mount.innerHTML =
      '<div class="yoast-card">' +
        '<div class="yoast-card__header">' +
          '<div class="yoast-card__title-wrap">' +
            '<span class="seo-dot seo-dot--lg seo-dot--' + status + '"></span>' +
            '<div>' +
              '<h3 class="yoast-card__heading">' + U.escapeHtml(t('seoAnalysisHeading')) + '</h3>' +
              '<div class="yoast-card__subphrase">' + U.escapeHtml(c.focusKeyphrase || t('seoSetFocusHint')) + '</div>' +
            '</div>' +
          '</div>' +
          '<span class="yoast-score-pill">' + (seo.overallScore || 0) + '/100</span>' +
        '</div>' +
        '<div class="yoast-section">' +
          '<label class="field-label">' + U.escapeHtml(t('lblFocusKp')) + '</label>' +
          '<input type="text" id="cmp-focus-kp" class="field-input" style="margin-top:5px;padding:9px 12px;" value="' + U.escapeHtml(c.focusKeyphrase || '') + '" placeholder="' + U.escapeHtml(t('phFocusKp')) + '" />' +
          '<div>' +
            '<button type="button" class="yoast-link-action" id="yoast-toggle-syn">' + U.escapeHtml(t('btnAddSynonyms')) + '</button>' +
            '<button type="button" class="yoast-link-action" id="yoast-add-rk">' + U.escapeHtml(t('btnAddRelatedKp')) + '</button>' +
          '</div>' +
          synonymsHtml +
          relatedHtml +
        '</div>' +
        '<div class="yoast-section">' +
          '<div style="font-family:var(--font-mono);font-size:11px;text-transform:uppercase;color:var(--ink-mid);letter-spacing:1px;">' + U.escapeHtml(t('lblGooglePreview')) + '</div>' +
          '<div class="yoast-serp-preview">' +
            '<div class="yoast-serp-url">https://ranamedia.co › essays › ' + U.escapeHtml(slugPreview) + '</div>' +
            '<div class="yoast-serp-title">' + U.escapeHtml(seoTitle) + '</div>' +
            '<div class="yoast-serp-desc">' + U.escapeHtml(metaDesc || t('defaultSerpDesc')) + '</div>' +
          '</div>' +
          '<div style="margin-top:10px;font-size:12px;color:var(--ink-mid);display:flex;justify-content:space-between;"><span>' + U.escapeHtml(t('lblSeoTitleWidth')) + '</span><span>' + titlePx + ' / 600px</span></div>' +
          '<div class="yoast-meter-track"><div class="yoast-meter-fill" style="width:' + titlePct + '%;background:' + titleBarColor + ';"></div></div>' +
          '<div style="margin-top:8px;font-size:12px;color:var(--ink-mid);display:flex;justify-content:space-between;"><span>' + U.escapeHtml(t('lblMetaDescShort')) + '</span><span>' + descLen + ' / 156 ' + U.escapeHtml(t('charsUnit')) + '</span></div>' +
          '<div class="yoast-meter-track"><div class="yoast-meter-fill" style="width:' + descPct + '%;background:' + descBarColor + ';"></div></div>' +
        '</div>' +
        ((seo.conflictingArticles && seo.conflictingArticles.length)
          ? '<div class="yoast-section" style="background:var(--seo-red-bg);">' +
              '<div style="font-size:12.5px;font-weight:700;color:#991b1b;margin-bottom:4px;">' + U.escapeHtml(t('cannibalAlert')) + '</div>' +
              '<div style="font-size:12px;color:#7f1d1d;">' + U.escapeHtml(t('usedBy')) + ' ' + seo.conflictingArticles.map(function (ca) { return U.escapeHtml(ca.title); }).join(', ') + '</div>' +
            '</div>'
          : '') +
        '<div class="yoast-section" style="border-bottom:0;">' +
          '<div style="font-size:13.5px;font-weight:700;margin-bottom:6px;display:flex;justify-content:space-between;">' +
            '<span>' + U.escapeHtml(t('lblAnalysisResults')) + '</span>' +
            '<span style="font-family:var(--font-mono);font-size:11.5px;color:var(--ink-mid);">' + U.escapeHtml(t('wordsDensity', { w: m.wordCount || 0, d: m.keyphraseDensityPercent || 0 })) + '</span>' +
          '</div>' +
          '<details open>' +
            '<summary class="yoast-group-header" style="cursor:pointer;">' + U.escapeHtml(t('grpProblems', { n: (seo.problems || []).length })) + '</summary>' +
            '<ul class="yoast-check-list">' + renderChecks(seo.problems || [], 'Problem') + '</ul>' +
          '</details>' +
          ((seo.improvements && seo.improvements.length)
            ? '<details open><summary class="yoast-group-header" style="cursor:pointer;">' + U.escapeHtml(t('grpImprovements', { n: seo.improvements.length })) + '</summary><ul class="yoast-check-list">' + renderChecks(seo.improvements, 'Improvement') + '</ul></details>'
            : '') +
          '<details open>' +
            '<summary class="yoast-group-header" style="cursor:pointer;">' + U.escapeHtml(t('grpGood', { n: (seo.goodResults || []).length })) + '</summary>' +
            '<ul class="yoast-check-list">' + renderChecks(seo.goodResults || [], 'Good') + '</ul>' +
          '</details>' +
        '</div>' +
      '</div>';

    var kpIn = document.getElementById('cmp-focus-kp');
    if (kpIn) kpIn.addEventListener('input', function () { Admin.composer.focusKeyphrase = kpIn.value; scheduleLiveSeo(); });

    document.getElementById('yoast-toggle-syn').addEventListener('click', function () {
      Admin.composer.showSynonymsInput = !Admin.composer.showSynonymsInput;
      renderYoastCard();
    });

    document.getElementById('yoast-add-rk').addEventListener('click', function () {
      Admin.composer.relatedKeyphrases.push({ keyphrase: '', synonyms: [] });
      renderYoastCard();
    });

    var synAdd = document.getElementById('yoast-syn-add');
    var synIn = document.getElementById('yoast-syn-input');
    if (synAdd && synIn) {
      var addSynFn = function () {
        var vals = synIn.value.split(',').map(function (v) { return v.trim(); }).filter(Boolean);
        vals.forEach(function (v) {
          if (Admin.composer.synonyms.indexOf(v) === -1) Admin.composer.synonyms.push(v);
        });
        synIn.value = '';
        renderYoastCard();
        scheduleLiveSeo();
      };
      synAdd.addEventListener('click', addSynFn);
      synIn.addEventListener('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); addSynFn(); } });
    }

    mount.querySelectorAll('[data-del-syn]').forEach(function (b) {
      b.addEventListener('click', function () {
        Admin.composer.synonyms.splice(Number(b.getAttribute('data-del-syn')), 1);
        renderYoastCard();
        scheduleLiveSeo();
      });
    });

    mount.querySelectorAll('[data-del-rk]').forEach(function (b) {
      b.addEventListener('click', function () {
        Admin.composer.relatedKeyphrases.splice(Number(b.getAttribute('data-del-rk')), 1);
        renderYoastCard();
        scheduleLiveSeo();
      });
    });

    mount.querySelectorAll('[data-rk-input]').forEach(function (inp) {
      inp.addEventListener('input', function () {
        var idx = Number(inp.getAttribute('data-rk-input'));
        Admin.composer.relatedKeyphrases[idx].keyphrase = inp.value;
        scheduleLiveSeo();
      });
    });

    mount.querySelectorAll('[data-seo-eye]').forEach(function (b) {
      b.addEventListener('click', function () {
        var rid = b.getAttribute('data-seo-eye');
        Admin.activeHighlightRuleId = Admin.activeHighlightRuleId === rid ? null : rid;
        if (Admin.activeHighlightRuleId && !Admin.previewMode) {
          Admin.previewMode = true;
          toggleEditorPreviewUi(true);
        }
        renderYoastCard();
        if (Admin.previewMode) renderBodyPreview();
      });
    });

    if (synDraft && synIn) {
      synIn.value = synDraft;
    }
    if (focusMeta) {
      var targetEl = focusMeta.id
        ? document.getElementById(focusMeta.id)
        : mount.querySelector('[data-rk-input="' + focusMeta.rkIdx + '"]');
      if (targetEl) {
        targetEl.focus();
        if (typeof focusMeta.start === 'number') {
          try {
            targetEl.setSelectionRange(focusMeta.start, focusMeta.end);
          } catch (_) {}
        }
      }
    }
  }

  function toggleEditorPreviewUi(previewOn) {
    Admin.previewMode = previewOn;
    var ta = document.getElementById('cmp-body');
    var pv = document.getElementById('cmp-preview-box');
    var wBtn = document.getElementById('mode-write-btn');
    var pBtn = document.getElementById('mode-preview-btn');
    if (!ta || !pv) return;
    if (previewOn) {
      Admin.composer.bodyHtml = ta.value;
      ta.classList.add('hidden');
      pv.classList.remove('hidden');
      renderBodyPreview();
    } else {
      pv.classList.add('hidden');
      ta.classList.remove('hidden');
    }
    if (wBtn) wBtn.classList.toggle('is-active', !previewOn);
    if (pBtn) pBtn.classList.toggle('is-active', previewOn);
  }

  function renderComposer() {
    var c = Admin.composer;
    var sections = Admin.catalog || Rana.demo.sections;
    if (!c.sectionId && sections[0]) c.sectionId = sections[0].id;
    var currentSec = sections.find(function (s) { return s.id === c.sectionId; }) || sections[0];
    var topics = (currentSec && currentSec.topics) || [];

    wsEl().innerHTML =
      '<div class="composer-split">' +
        '<div>' +
          '<div class="paper-panel">' +
            '<div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px;flex-wrap:wrap;gap:10px;">' +
              '<button type="button" id="cmp-back-list" class="aeon-btn aeon-btn--ghost" style="padding:7px 12px;font-size:12px;">' + U.escapeHtml(t('btnBackArticles')) + '</button>' +
              '<div style="display:flex;gap:10px;flex-wrap:wrap;">' +
                (Admin.editingArticleId ? '<button type="button" id="cmp-save-seo-only" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(t('btnAnalyzeSaveSeo')) + '</button>' : '') +
                '<button type="button" id="cmp-save-article" class="aeon-btn">' + U.escapeHtml(Admin.editingArticleId ? t('btnUpdateArticle') : t('btnPublishSave')) + '</button>' +
              '</div>' +
            '</div>' +
            '<input type="text" id="cmp-title" class="composer-title-input" value="' + U.escapeHtml(c.title) + '" placeholder="' + U.escapeHtml(t('phHeadline')) + '" />' +
            '<div class="form-grid form-grid--2" style="margin-top:16px;">' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblSlug')) + '</label><input type="text" id="cmp-slug" class="field-input" value="' + U.escapeHtml(c.slug) + '" placeholder="' + U.escapeHtml(t('phSlug')) + '" /></div>' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblType')) + '</label><select id="cmp-type" class="field-select"><option value="Essay">' + U.escapeHtml(t('tpEssay')) + '</option><option value="Video">' + U.escapeHtml(t('tpVideo')) + '</option><option value="Idea">' + U.escapeHtml(t('tpIdea')) + '</option><option value="Classic">' + U.escapeHtml(t('tpClassic')) + '</option></select></div>' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblStatus')) + '</label><select id="cmp-status" class="field-select"><option value="Draft">' + U.escapeHtml(t('stDraft')) + '</option><option value="Published">' + U.escapeHtml(t('stPublished')) + '</option><option value="Scheduled">' + U.escapeHtml(t('stScheduled')) + '</option><option value="Archived">' + U.escapeHtml(t('stArchived')) + '</option></select></div>' +
              '</div>' +
            '</div>' +
            '<div class="form-grid form-grid--2" style="margin-top:14px;">' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblSection')) + '</label><select id="cmp-section" class="field-select">' +
                sections.map(function (s) { return '<option value="' + s.id + '">' + U.escapeHtml(s.title) + '</option>'; }).join('') +
              '</select></div>' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblTopic')) + '</label><select id="cmp-topic" class="field-select"><option value="">' + U.escapeHtml(t('optNone')) + '</option>' +
                topics.map(function (tp) { return '<option value="' + tp.id + '">' + U.escapeHtml(tp.title) + '</option>'; }).join('') +
              '</select></div>' +
            '</div>' +
            '<div class="form-grid" style="margin-top:14px;">' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblShortStandfirst')) + '</label><input type="text" id="cmp-short" class="field-input" value="' + U.escapeHtml(c.standfirstShort) + '" placeholder="' + U.escapeHtml(t('phShortStandfirst')) + '" /></div>' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblLongStandfirst')) + '</label><textarea id="cmp-long" class="field-textarea" style="min-height:68px;">' + U.escapeHtml(c.standfirstLong) + '</textarea></div>' +
            '</div>' +
            '<div style="margin-top:18px;">' +
              '<label class="field-label" style="display:block;margin-bottom:6px;">' + U.escapeHtml(t('lblBodyHtml')) + '</label>' +
              '<div class="editor-toolbar">' +
                '<button type="button" class="editor-tool-btn is-active" id="mode-write-btn">' + U.escapeHtml(t('toolWriteHtml')) + '</button>' +
                '<button type="button" class="editor-tool-btn" id="mode-preview-btn">' + U.escapeHtml(t('toolPreviewHl')) + '</button>' +
                '<span style="opacity:0.35;">|</span>' +
                '<button type="button" class="editor-tool-btn" data-ins="h2">H2</button>' +
                '<button type="button" class="editor-tool-btn" data-ins="h3">H3</button>' +
                '<button type="button" class="editor-tool-btn" data-ins="p">' + U.escapeHtml(t('toolParagraph')) + '</button>' +
                '<button type="button" class="editor-tool-btn" data-ins="pullquote">' + U.escapeHtml(t('toolPullquote')) + '</button>' +
                '<button type="button" class="editor-tool-btn" data-ins="blockquote">' + U.escapeHtml(t('toolQuote')) + '</button>' +
                '<button type="button" class="editor-tool-btn" data-ins="link">' + U.escapeHtml(t('toolLink')) + '</button>' +
                '<label class="editor-tool-btn" style="cursor:pointer;">' + U.escapeHtml(t('toolUploadImg')) + '<input type="file" id="cmp-inline-img-file" accept="image/*" class="hidden" /></label>' +
              '</div>' +
              '<textarea id="cmp-body" class="composer-body-textarea">' + U.escapeHtml(c.bodyHtml) + '</textarea>' +
              '<div id="cmp-preview-box" class="composer-preview-pane hidden"></div>' +
            '</div>' +
          '</div>' +

          '<div class="paper-panel">' +
            '<h3 class="paper-panel__title" style="margin-bottom:16px;">' + U.escapeHtml(t('panelSnippetMeta')) + '</h3>' +
            '<div class="form-grid">' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblMetaTitle')) + '</label><input type="text" id="cmp-meta-title" class="field-input" value="' + U.escapeHtml(c.metaTitle || '') + '" placeholder="' + U.escapeHtml(t('phMetaTitle')) + '" /></div>' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblTags')) + '</label><input type="text" id="cmp-tags" class="field-input" value="' + U.escapeHtml((c.tagNames || []).join(', ')) + '" placeholder="originals, classics" /></div>' +
              '</div>' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblMetaDesc')) + '</label><textarea id="cmp-meta-desc" class="field-textarea" style="min-height:68px;">' + U.escapeHtml(c.metaDescription || '') + '</textarea></div>' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblHeroImgUrl')) + '</label><div style="display:flex;gap:6px;"><input type="text" id="cmp-feat-img" class="field-input" value="' + U.escapeHtml(c.featuredImageUrl || '') + '" /><label class="aeon-btn aeon-btn--secondary" style="cursor:pointer;white-space:nowrap;">' + U.escapeHtml(t('btnUpload')) + '<input type="file" id="cmp-feat-upload" accept="image/*" class="hidden" /></label></div><img id="cmp-feat-preview" src="' + U.escapeHtml(c.featuredImageUrl ? U.resolveMediaUrl(c.featuredImageUrl) : '') + '" alt="" style="margin-top:8px;max-height:110px;width:auto;border:1px solid var(--border-hairline);' + (c.featuredImageUrl ? '' : 'display:none;') + '" /></div>' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblHeroImgAlt')) + '</label><input type="text" id="cmp-feat-alt" class="field-input" value="' + U.escapeHtml(c.featuredImageAlt || '') + '" /></div>' +
              '</div>' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblCardImgUrl')) + '</label><div style="display:flex;gap:6px;"><input type="text" id="cmp-card-img" class="field-input" value="' + U.escapeHtml(c.cardImageUrl || '') + '" /><label class="aeon-btn aeon-btn--secondary" style="cursor:pointer;white-space:nowrap;">' + U.escapeHtml(t('btnUpload')) + '<input type="file" id="cmp-card-upload" accept="image/*" class="hidden" /></label></div><img id="cmp-card-preview" src="' + U.escapeHtml(c.cardImageUrl ? U.resolveMediaUrl(c.cardImageUrl) : '') + '" alt="" style="margin-top:8px;max-height:110px;width:auto;border:1px solid var(--border-hairline);' + (c.cardImageUrl ? '' : 'display:none;') + '" /></div>' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblCardImgAlt')) + '</label><input type="text" id="cmp-card-alt" class="field-input" value="' + U.escapeHtml(c.cardImageAlt || '') + '" /></div>' +
              '</div>' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblHeroAlign')) + '</label><div style="display:flex;gap:6px;"><select id="cmp-align-x" class="field-select"><option value="center">' + U.escapeHtml(t('optCenter')) + '</option><option value="left">' + U.escapeHtml(t('optLeft')) + '</option><option value="right">' + U.escapeHtml(t('optRight')) + '</option></select><select id="cmp-align-y" class="field-select"><option value="center">' + U.escapeHtml(t('optCenter')) + '</option><option value="top">' + U.escapeHtml(t('optTop')) + '</option><option value="bottom">' + U.escapeHtml(t('optBottom')) + '</option></select><input type="number" id="cmp-backdrop" min="0" max="5" class="field-input" style="width:72px;" value="' + ((c.settings && c.settings.backdropStrength) || 2) + '" /></div></div>' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblCommissionEditor')) + '</label><select id="cmp-editor" class="field-select"><option value="">' + U.escapeHtml(t('optNone')) + '</option>' +
                  Admin.contributors.map(function (ct) { return '<option value="' + ct.id + '">' + U.escapeHtml(ct.name) + '</option>'; }).join('') +
                '</select></div>' +
              '</div>' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblAuthors')) + '</label><div style="display:flex;flex-wrap:wrap;gap:14px;padding:8px 0;">' +
                Admin.contributors.map(function (ct) {
                  var chk = (c.authorIds || []).indexOf(ct.id) !== -1 ? ' checked' : '';
                  return '<label class="aeon-checkbox-label" style="font-size:14px;"><span class="aeon-checkbox-box"><input type="checkbox" name="cmp-authors" value="' + ct.id + '"' + chk + ' /></span><span>' + U.escapeHtml(ct.name) + '</span></label>';
                }).join('') +
              '</div></div>' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblAudioUrlSec')) + '</label><div style="display:flex;gap:6px;"><input type="text" id="cmp-audio-url" class="field-input" value="' + U.escapeHtml(c.audioUrl || '') + '" placeholder="/uploads/audio/..." /><input type="number" id="cmp-audio-sec" class="field-input" style="width:90px;" value="' + (c.audioDurationSeconds || 0) + '" placeholder="Sec" /></div></div>' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblVideoHosterUrl')) + '</label><div style="display:flex;gap:6px;"><select id="cmp-vid-hoster" class="field-select" style="width:120px;"><option value="None">' + U.escapeHtml(t('optNone')) + '</option><option value="YouTube">YouTube</option><option value="Vimeo">Vimeo</option><option value="SelfHosted">SelfHosted</option></select><input type="text" id="cmp-vid-url" class="field-input" value="' + U.escapeHtml(c.videoEmbedUrl || '') + '" placeholder="https://www.youtube.com/embed/..." /></div></div>' +
              '</div>' +
              '<div class="form-grid form-grid--2">' +
                '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblVideoMinCredits')) + '</label><div style="display:flex;gap:6px;"><input type="number" id="cmp-vid-min" class="field-input" style="width:85px;" value="' + (c.videoDurationMinutes || 0) + '" /><input type="text" id="cmp-credits-short" class="field-input" value="' + U.escapeHtml(c.creditsShort || '') + '" placeholder="' + U.escapeHtml(t('phCreditsShort')) + '" /></div></div>' +
                '<div class="field-group" style="justify-content:flex-end;gap:10px;">' +
                  '<div style="display:flex;flex-wrap:wrap;gap:14px;">' +
                    '<label class="aeon-checkbox-label" style="font-size:13.5px;"><span class="aeon-checkbox-box"><input type="checkbox" id="cmp-featured-home"' + (c.isHomepageFeatured ? ' checked' : '') + ' /></span><span>' + U.escapeHtml(t('chkHomepageHero')) + '</span></label>' +
                    '<label class="aeon-checkbox-label" style="font-size:13.5px;"><span class="aeon-checkbox-box"><input type="checkbox" id="cmp-audio-on"' + (c.audioEnabled ? ' checked' : '') + ' /></span><span>' + U.escapeHtml(t('chkAudio')) + '</span></label>' +
                    '<label class="aeon-checkbox-label" style="font-size:13.5px;"><span class="aeon-checkbox-box"><input type="checkbox" id="cmp-cmt-enabled"' + (c.commentsEnabled ? ' checked' : '') + ' /></span><span>' + U.escapeHtml(t('chkCommentsOn')) + '</span></label>' +
                    '<label class="aeon-checkbox-label" style="font-size:13.5px;"><span class="aeon-checkbox-box"><input type="checkbox" id="cmp-cmt-open"' + (c.commentsOpen ? ' checked' : '') + ' /></span><span>' + U.escapeHtml(t('chkOpen')) + '</span></label>' +
                    '<label class="aeon-checkbox-label" style="font-size:13.5px;"><span class="aeon-checkbox-box"><input type="checkbox" id="cmp-repub"' + (c.republicationAllowed ? ' checked' : '') + ' /></span><span>' + U.escapeHtml(t('chkSyndication')) + '</span></label>' +
                  '</div>' +
                '</div>' +
              '</div>' +
            '</div>' +
          '</div>' +
        '</div>' +
        '<aside id="yoast-card-mount"></aside>' +
      '</div>';

    document.getElementById('cmp-type').value = c.type || 'Essay';
    document.getElementById('cmp-status').value = c.status || 'Draft';
    document.getElementById('cmp-section').value = c.sectionId || '';
    document.getElementById('cmp-topic').value = c.primaryTopicId || '';
    document.getElementById('cmp-align-x').value = (c.settings && c.settings.alignX) || 'center';
    document.getElementById('cmp-align-y').value = (c.settings && c.settings.alignY) || 'center';
    document.getElementById('cmp-editor').value = c.editorId || '';
    document.getElementById('cmp-vid-hoster').value = c.videoHoster || 'None';

    document.getElementById('cmp-back-list').addEventListener('click', function () {
      Admin.tab = 'articles';
      renderShell();
      loadArticlesList();
    });

    document.getElementById('cmp-title').addEventListener('input', function (e) {
      if (!Admin.editingArticleId) {
        var slugEl = document.getElementById('cmp-slug');
        if (slugEl) slugEl.value = U.slugify(e.target.value);
      }
      scheduleLiveSeo();
    });

    ['cmp-slug', 'cmp-body', 'cmp-meta-title', 'cmp-meta-desc', 'cmp-feat-img', 'cmp-feat-alt', 'cmp-card-img'].forEach(function (id) {
      var el = document.getElementById(id);
      if (el) el.addEventListener('input', scheduleLiveSeo);
    });

    document.getElementById('cmp-section').addEventListener('change', function (e) {
      var sec = (Admin.catalog || []).find(function (s) { return s.id === e.target.value; });
      var tSelect = document.getElementById('cmp-topic');
      tSelect.innerHTML = '<option value="">' + U.escapeHtml(t('optNone')) + '</option>' + ((sec && sec.topics) || []).map(function (tp) {
        return '<option value="' + tp.id + '">' + U.escapeHtml(tp.title) + '</option>';
      }).join('');
    });

    document.getElementById('mode-write-btn').addEventListener('click', function () { toggleEditorPreviewUi(false); });
    document.getElementById('mode-preview-btn').addEventListener('click', function () { toggleEditorPreviewUi(true); });
    if (Admin.previewMode) toggleEditorPreviewUi(true);

    wsEl().querySelectorAll('[data-ins]').forEach(function (btn) {
      btn.addEventListener('click', async function () {
        var kind = btn.getAttribute('data-ins');
        var ta = document.getElementById('cmp-body');
        if (!ta) return;
        var snippet = '';
        if (kind === 'h2') snippet = '\n<h2>Subheading title</h2>\n';
        else if (kind === 'h3') snippet = '\n<h3>Section subheading</h3>\n';
        else if (kind === 'p') snippet = '\n<p>Write your editorial paragraph here.</p>\n';
        else if (kind === 'pullquote') snippet = '\n<p class="pullquote">Memorable editorial pullquote</p>\n';
        else if (kind === 'blockquote') snippet = '\n<blockquote>Notable quotation text.</blockquote>\n';
        else if (kind === 'link') {
          var href = await UI.promptModal({ title: t('toolLink'), message: t('promptLinkUrl'), defaultValue: 'https://', required: true });
          if (!href || !href.trim()) return;
          snippet = '<a href="' + U.escapeHtml(href.trim()) + '" target="_blank" rel="noopener">link text</a>';
        }
        var pos = ta.selectionStart || ta.value.length;
        ta.value = ta.value.slice(0, pos) + snippet + ta.value.slice(pos);
        ta.focus();
        scheduleLiveSeo();
      });
    });

    function syncImgPreview(inputId, previewId) {
      var inp = document.getElementById(inputId);
      var img = document.getElementById(previewId);
      if (!inp || !img) return;
      var val = inp.value.trim();
      if (val) {
        img.src = U.resolveMediaUrl(val);
        img.style.display = 'block';
      } else {
        img.removeAttribute('src');
        img.style.display = 'none';
      }
    }

    ['cmp-feat-img', 'cmp-card-img'].forEach(function (id) {
      var el = document.getElementById(id);
      if (el) el.addEventListener('input', function () {
        syncImgPreview(id, id === 'cmp-feat-img' ? 'cmp-feat-preview' : 'cmp-card-preview');
      });
    });

    function wireImageUpload(inputId, targetUrlId, previewId, insertToBody) {
      var inp = document.getElementById(inputId);
      if (!inp) return;
      inp.addEventListener('change', async function () {
        if (!inp.files || !inp.files[0]) return;
        try {
          var up = await Rana.api.adminUploadMedia(inp.files[0], 'Images');
          Admin.uploadedMedia.unshift(up);
          if (targetUrlId) {
            document.getElementById(targetUrlId).value = up.relativeUrl;
            if (previewId) syncImgPreview(targetUrlId, previewId);
          }
          if (insertToBody) {
            var alt = (await UI.promptModal({ title: t('toolUploadImg'), message: t('promptImgAlt'), defaultValue: Admin.composer.focusKeyphrase || 'Editorial artwork' })) || '';
            var ta = document.getElementById('cmp-body');
            ta.value += '\n<img src="' + U.escapeHtml(up.relativeUrl) + '" alt="' + U.escapeHtml(alt) + '" />\n';
          }
          UI.toast(t('toastImgUploaded'), 'success');
          scheduleLiveSeo();
        } catch (err) { UI.toast(err.message, 'warn'); }
        finally { inp.value = ''; }
      });
    }
    wireImageUpload('cmp-inline-img-file', null, null, true);
    wireImageUpload('cmp-feat-upload', 'cmp-feat-img', 'cmp-feat-preview', false);
    wireImageUpload('cmp-card-upload', 'cmp-card-img', 'cmp-card-preview', false);

    document.getElementById('cmp-save-article').addEventListener('click', async function () {
      syncComposerFromDom();
      var payload = Object.assign({}, Admin.composer);
      delete payload.seoResult;
      delete payload.showSynonymsInput;
      if (!payload.primaryTopicId) payload.primaryTopicId = null;
      if (!payload.editorId) payload.editorId = null;
      if (!payload.scheduledAt) payload.scheduledAt = null;
      if (!payload.slug) payload.slug = U.slugify(payload.title);
      else payload.slug = U.slugify(payload.slug);
      payload.topicIds = payload.primaryTopicId ? [payload.primaryTopicId] : [];
      if (!payload.standfirstShort || !payload.title || !payload.bodyHtml) {
        UI.toast(t('toastRequiredFields'), 'warn');
        return;
      }
      try {
        var res = Admin.editingArticleId
          ? await Rana.api.adminUpdateArticle(Admin.editingArticleId, payload)
          : await Rana.api.adminCreateArticle(payload);
        Admin.editingArticleId = res.article.id;
        if (res.seoAnalysis) Admin.composer.seoResult = res.seoAnalysis;
        UI.toast(t('toastArticleSaved'), 'success');
        renderShell();
        renderComposer();
      } catch (err) {
        UI.toast(err.message || t('toastArticleSaveErr'), 'warn');
      }
    });

    var seoOnlyBtn = document.getElementById('cmp-save-seo-only');
    if (seoOnlyBtn) {
      seoOnlyBtn.addEventListener('click', async function () {
        syncComposerFromDom();
        try {
          Admin.composer.seoResult = await Rana.api.adminAnalyzeSeoPersist(Admin.editingArticleId, buildSeoRequest());
          UI.toast(t('toastSeoSaved'), 'success');
          renderYoastCard();
        } catch (err) { UI.toast(err.message, 'warn'); }
      });
    }

    renderYoastCard();
    if (!Admin.composer.seoResult) scheduleLiveSeo();
  }

  /* ---------------------- 4. CONTRIBUTORS ---------------------- */
  async function loadContributorsTab() {
    try { Admin.contributors = await Rana.api.adminGetContributors(); } catch (_) {}
    wsEl().innerHTML =
      '<div class="composer-split" style="grid-template-columns:1.2fr 1fr;">' +
        '<div class="paper-panel">' +
          '<h2 class="paper-panel__title" style="margin-bottom:16px;">' + U.escapeHtml(t('contribHeading', { n: Admin.contributors.length })) + '</h2>' +
          '<div class="admin-table-wrap"><table class="admin-table"><thead><tr><th>' + U.escapeHtml(t('thName')) + '</th><th>' + U.escapeHtml(t('thSlug')) + '</th><th>' + U.escapeHtml(t('thActions')) + '</th></tr></thead><tbody>' +
            Admin.contributors.map(function (c) {
              return '<tr><td><strong>' + U.escapeHtml(c.name) + '</strong><div style="font-size:12.5px;color:var(--ink-mid);">' + U.stripHtml(c.shortBioHtml || '') + '</div></td><td style="font-family:var(--font-mono);font-size:12px;">' + U.escapeHtml(c.slug) + '</td><td style="white-space:nowrap;"><button type="button" class="aeon-btn aeon-btn--ghost" data-del-contrib="' + c.id + '" style="padding:6px 10px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('btnDelete')) + '</button></td></tr>';
            }).join('') +
          '</tbody></table></div>' +
        '</div>' +
        '<div class="paper-panel">' +
          '<h2 class="paper-panel__title" style="margin-bottom:16px;">' + U.escapeHtml(t('contribCreateHeading')) + '</h2>' +
          '<form id="new-contrib-form" class="form-grid">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblFullName')) + '</label><input type="text" name="name" id="nc-name" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblSlug')) + '</label><input type="text" name="slug" id="nc-slug" required class="field-input" /></div>' +
            '<div class="form-grid form-grid--2">' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblOccupation')) + '</label><input type="text" name="occupation" class="field-input" /></div>' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblInstitution')) + '</label><input type="text" name="company" class="field-input" /></div>' +
            '</div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblShortBio')) + '</label><input type="text" name="shortBioHtml" class="field-input" placeholder="<p>is a philosopher...</p>" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(t('lblFullBio')) + '</label><textarea name="bioHtml" class="field-textarea" style="min-height:80px;"></textarea></div>' +
            '<button type="submit" class="aeon-btn">' + U.escapeHtml(t('btnSaveContrib')) + '</button>' +
          '</form>' +
        '</div>' +
      '</div>';

    document.getElementById('nc-name').addEventListener('input', function (e) {
      document.getElementById('nc-slug').value = U.slugify(e.target.value);
    });
    document.getElementById('new-contrib-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var fd = new FormData(e.target);
      try {
        await Rana.api.adminCreateContributor({
          name: fd.get('name').trim(),
          slug: fd.get('slug').trim(),
          occupation: fd.get('occupation').trim() || null,
          company: fd.get('company').trim() || null,
          shortBioHtml: fd.get('shortBioHtml').trim() || null,
          bioHtml: fd.get('bioHtml').trim() || ''
        });
        UI.toast(t('toastContribAdded'), 'success');
        loadContributorsTab();
      } catch (err) { UI.toast(err.message, 'warn'); }
    });
    wsEl().querySelectorAll('[data-del-contrib]').forEach(function (b) {
      b.addEventListener('click', async function () {
        if (!(await UI.confirmModal({ message: t('confirmDelContrib'), danger: true }))) return;
        try {
          await Rana.api.adminDeleteContributor(b.getAttribute('data-del-contrib'));
          UI.toast(t('toastContribDeleted'));
          loadContributorsTab();
        } catch (err) { UI.toast(err.message, 'warn'); }
      });
    });
  }

  /* ---------------------- 5. COMMENT MODERATION ---------------------- */
  async function loadModerationTab() {
    var mode = 'all';
    async function refresh() {
      wsEl().innerHTML = '<p style="font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(t('loadingComments')) + '</p>';
      var params = { pageNumber: 1, pageSize: 40 };
      if (mode === 'reported') params.onlyReported = true;
      if (mode === 'moderated') params.onlyModerated = true;
      try {
        var page = await Rana.api.adminGetComments(params);
        var items = page.items || [];
        wsEl().innerHTML =
          '<div class="paper-panel">' +
            '<div class="paper-panel__header">' +
              '<h2 class="paper-panel__title">' + U.escapeHtml(t('modHeading', { n: page.totalCount })) + '</h2>' +
              '<div style="display:flex;gap:8px;flex-wrap:wrap;">' +
                '<button type="button" class="aeon-btn aeon-btn--secondary" data-mod-mode="all">' + U.escapeHtml(t('modAll')) + '</button>' +
                '<button type="button" class="aeon-btn aeon-btn--secondary" data-mod-mode="reported">' + U.escapeHtml(t('modReported')) + '</button>' +
                '<button type="button" class="aeon-btn aeon-btn--secondary" data-mod-mode="moderated">' + U.escapeHtml(t('modModerated')) + '</button>' +
              '</div>' +
            '</div>' +
            '<div class="admin-table-wrap"><table class="admin-table"><thead><tr><th>' + U.escapeHtml(t('thReaderArticle')) + '</th><th>' + U.escapeHtml(t('thComment')) + '</th><th>' + U.escapeHtml(t('thSignals')) + '</th><th>' + U.escapeHtml(t('thActions')) + '</th></tr></thead><tbody>' +
              (items.length
                ? items.map(function (c) {
                    return '<tr>' +
                      '<td><strong>' + U.escapeHtml(c.userName) + '</strong><div style="font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(c.articleTitle) + '</div><div style="font-family:var(--font-mono);font-size:11px;color:var(--ink-light);">' + U.formatDateTime(c.createdAt) + '</div></td>' +
                      '<td style="max-width:420px;">' + U.escapeHtml(c.bodyHtml) + (c.moderationReason ? '<div style="margin-top:4px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('lblReason')) + ' ' + U.escapeHtml(c.moderationReason) + '</div>' : '') + '</td>' +
                      '<td style="font-family:var(--font-mono);font-size:12px;">' + U.escapeHtml(t('lblLikes')) + ': ' + c.likesCount + ' · ' + U.escapeHtml(t('lblReplies')) + ': ' + c.repliesCount + '<br/>' + U.escapeHtml(t('lblReports')) + ': <strong style="color:' + (c.reportsCount > 0 ? 'var(--seo-red)' : 'inherit') + ';">' + c.reportsCount + '</strong><br/>' + (c.isModerated ? '<span class="status-pill status-pill--Red">' + U.escapeHtml(t('stHidden')) + '</span>' : '<span class="status-pill status-pill--Green">' + U.escapeHtml(t('stVisible')) + '</span>') + (c.isFeatured ? ' <span class="comment-badge">' + U.escapeHtml(t('badgeFeatured')) + '</span>' : '') + '</td>' +
                      '<td style="white-space:nowrap;">' +
                        (c.isModerated
                          ? '<button type="button" class="aeon-btn aeon-btn--secondary" data-unmod="' + c.id + '">' + U.escapeHtml(t('btnRestore')) + '</button> '
                          : '<button type="button" class="aeon-btn aeon-btn--secondary" data-mod="' + c.id + '">' + U.escapeHtml(t('btnHide')) + '</button> ') +
                        '<button type="button" class="aeon-btn aeon-btn--ghost" data-feat="' + c.id + '" data-feat-next="' + (!c.isFeatured) + '" style="padding:7px 10px;font-size:12px;">' + U.escapeHtml(c.isFeatured ? t('btnUnfeature') : t('btnFeature')) + '</button> ' +
                        '<button type="button" class="aeon-btn aeon-btn--ghost" data-del-cmt="' + c.id + '" style="padding:7px 10px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('btnDelete')) + '</button>' +
                      '</td>' +
                    '</tr>';
                  }).join('')
                : '<tr><td colspan="4" style="text-align:center;color:var(--ink-mid);padding:28px;">' + U.escapeHtml(t('noCommentsFound')) + '</td></tr>') +
            '</tbody></table></div>' +
          '</div>';

        wsEl().querySelectorAll('[data-mod-mode]').forEach(function (b) {
          b.addEventListener('click', function () { mode = b.getAttribute('data-mod-mode'); refresh(); });
        });
        wsEl().querySelectorAll('[data-mod]').forEach(function (b) {
          b.addEventListener('click', async function () {
            var reason = await UI.promptModal({ title: t('btnHide'), message: t('promptHideReason'), defaultValue: t('defaultHideReason') });
            if (reason === null) return;
            await Rana.api.adminModerateComment(b.getAttribute('data-mod'), true, reason);
            UI.toast(t('toastCommentHidden'));
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-unmod]').forEach(function (b) {
          b.addEventListener('click', async function () {
            await Rana.api.adminModerateComment(b.getAttribute('data-unmod'), false, null);
            UI.toast(t('toastCommentRestored'), 'success');
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-feat]').forEach(function (b) {
          b.addEventListener('click', async function () {
            await Rana.api.adminFeatureComment(b.getAttribute('data-feat'), b.getAttribute('data-feat-next') === 'true');
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-del-cmt]').forEach(function (b) {
          b.addEventListener('click', async function () {
            if (!(await UI.confirmModal({ message: t('confirmDelComment'), danger: true }))) return;
            await Rana.api.deleteComment(b.getAttribute('data-del-cmt'));
            refresh();
          });
        });
      } catch (err) {
        wsEl().innerHTML = '<div class="paper-panel"><p style="color:var(--seo-red);">' + U.escapeHtml(err.message) + '</p></div>';
      }
    }
    refresh();
  }

  /* ---------------------- 6. SUBMISSIONS (PITCHES & SYNDICATION) ---------------------- */
  async function loadSubmissionsTab() {
    var subTab = 'pitches';
    async function refresh() {
      wsEl().innerHTML = '<p style="font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(t('loadingSubmissions')) + '</p>';
      try {
        var pitches = await Rana.api.adminGetPitches();
        var synds = await Rana.api.adminGetSyndications();

        wsEl().innerHTML =
          '<div class="paper-panel">' +
            '<div class="paper-panel__header">' +
              '<div class="sort-tabs" style="margin:0;">' +
                '<button type="button" class="sort-tabs__btn' + (subTab === 'pitches' ? ' is-active' : '') + '" data-sub-tab="pitches">' + U.escapeHtml(t('tabPitches', { n: pitches.length })) + '</button>' +
                '<button type="button" class="sort-tabs__btn' + (subTab === 'syndication' ? ' is-active' : '') + '" data-sub-tab="syndication">' + U.escapeHtml(t('tabSyndications', { n: synds.length })) + '</button>' +
              '</div>' +
            '</div>' +
            (subTab === 'pitches'
              ? '<div class="admin-table-wrap"><table class="admin-table"><thead><tr><th>' + U.escapeHtml(t('thAuthor')) + '</th><th>' + U.escapeHtml(t('thPitchTitleField')) + '</th><th>' + U.escapeHtml(t('thStatus')) + '</th><th>' + U.escapeHtml(t('thDecision')) + '</th></tr></thead><tbody>' +
                  (pitches.length ? pitches.map(function (p) {
                    return '<tr>' +
                      '<td><strong>' + U.escapeHtml(p.name) + '</strong><div style="font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(p.email) + '</div></td>' +
                      '<td><div style="font-weight:600;">' + U.escapeHtml(p.title) + '</div><div style="font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(p.field + ' · ' + p.format) + '</div>' + (p.editorNotes ? '<div style="font-size:12px;margin-top:4px;">' + U.escapeHtml(p.editorNotes) + '</div>' : '') + '</td>' +
                      '<td><span class="status-pill status-pill--' + p.status + '">' + U.escapeHtml(p.status) + '</span></td>' +
                      '<td style="white-space:nowrap;">' +
                        '<button type="button" class="aeon-btn aeon-btn--secondary" data-pitch-st="' + p.id + '" data-val="Approved">' + U.escapeHtml(t('btnApprove')) + '</button> ' +
                        '<button type="button" class="aeon-btn aeon-btn--ghost" data-pitch-st="' + p.id + '" data-val="Rejected" style="padding:7px 10px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('btnReject')) + '</button>' +
                      '</td></tr>';
                  }).join('') : '<tr><td colspan="4" style="text-align:center;color:var(--ink-mid);padding:24px;">' + U.escapeHtml(t('noPitches')) + '</td></tr>') +
                '</tbody></table></div>'
              : '<div class="admin-table-wrap"><table class="admin-table"><thead><tr><th>' + U.escapeHtml(t('thApplicant')) + '</th><th>' + U.escapeHtml(t('thPubArticle')) + '</th><th>' + U.escapeHtml(t('thStatus')) + '</th><th>' + U.escapeHtml(t('thDecision')) + '</th></tr></thead><tbody>' +
                  (synds.length ? synds.map(function (s) {
                    return '<tr>' +
                      '<td><strong>' + U.escapeHtml(s.name) + '</strong><div style="font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(s.country) + '</div></td>' +
                      '<td><div style="font-weight:600;">' + U.escapeHtml(s.publication) + '</div><div style="font-family:var(--font-mono);font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(s.articleSlug) + '</div></td>' +
                      '<td><span class="status-pill status-pill--' + s.status + '">' + U.escapeHtml(s.status) + '</span></td>' +
                      '<td style="white-space:nowrap;">' +
                        '<button type="button" class="aeon-btn aeon-btn--secondary" data-synd-st="' + s.id + '" data-val="Approved">' + U.escapeHtml(t('btnApprove')) + '</button> ' +
                        '<button type="button" class="aeon-btn aeon-btn--ghost" data-synd-st="' + s.id + '" data-val="Rejected" style="padding:7px 10px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('btnReject')) + '</button>' +
                      '</td></tr>';
                  }).join('') : '<tr><td colspan="4" style="text-align:center;color:var(--ink-mid);padding:24px;">' + U.escapeHtml(t('noSyndications')) + '</td></tr>') +
                '</tbody></table></div>') +
          '</div>';

        wsEl().querySelectorAll('[data-sub-tab]').forEach(function (b) {
          b.addEventListener('click', function () { subTab = b.getAttribute('data-sub-tab'); refresh(); });
        });
        wsEl().querySelectorAll('[data-pitch-st]').forEach(function (b) {
          b.addEventListener('click', async function () {
            var notes = await UI.promptModal({ title: b.getAttribute('data-val') === 'Approved' ? t('btnApprove') : t('btnReject'), message: t('promptEditorNotes'), defaultValue: '' });
            if (notes === null) return;
            await Rana.api.adminUpdatePitch(b.getAttribute('data-pitch-st'), b.getAttribute('data-val'), notes);
            UI.toast(t('toastStatusUpdated'), 'success');
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-synd-st]').forEach(function (b) {
          b.addEventListener('click', async function () {
            await Rana.api.adminUpdateSyndication(b.getAttribute('data-synd-st'), b.getAttribute('data-val'));
            UI.toast(t('toastStatusUpdated'), 'success');
            refresh();
          });
        });
      } catch (err) {
        wsEl().innerHTML = '<div class="paper-panel"><p style="color:var(--seo-red);">' + U.escapeHtml(err.message) + '</p></div>';
      }
    }
    refresh();
  }

  /* ---------------------- 7. MEDIA UPLOADS LIBRARY ---------------------- */
  function loadMediaTab() {
    wsEl().innerHTML =
      '<div class="paper-panel">' +
        '<h2 class="paper-panel__title" style="margin-bottom:14px;">' + U.escapeHtml(t('mediaHeading')) + '</h2>' +
        '<form id="media-up-form" style="display:flex;flex-wrap:wrap;gap:12px;align-items:center;margin-bottom:24px;">' +
          '<select name="folder" class="field-select" style="width:175px;"><option value="Images">' + U.escapeHtml(t('folderImages')) + '</option><option value="Videos">' + U.escapeHtml(t('folderVideos')) + '</option><option value="Audio">' + U.escapeHtml(t('folderAudio')) + '</option><option value="Avatars">' + U.escapeHtml(t('folderAvatars')) + '</option></select>' +
          '<input type="file" name="file" required class="field-input" style="max-width:340px;" />' +
          '<button type="submit" class="aeon-btn">' + U.escapeHtml(t('btnUploadFile')) + '</button>' +
        '</form>' +
        '<div id="uploaded-media-grid" class="articles-grid" style="padding:0;">' +
          (Admin.uploadedMedia.length
            ? Admin.uploadedMedia.map(function (m, idx) {
                var isImg = /\.(jpg|jpeg|png|webp|gif|svg)$/i.test(m.relativeUrl || '');
                return '<div class="paper-panel" style="margin:0;">' +
                  (isImg ? '<img src="' + U.escapeHtml(U.resolveMediaUrl(m.relativeUrl)) + '" alt="" style="width:100%;aspect-ratio:16/9;object-fit:cover;margin-bottom:10px;border:1px solid var(--border-hairline);background:#f4f8f5;" />' : '') +
                  '<div style="font-family:var(--font-mono);font-size:12px;word-break:break-all;margin-bottom:8px;">' + U.escapeHtml(m.relativeUrl) + '</div>' +
                  '<div style="font-size:12px;color:var(--ink-mid);margin-bottom:10px;">' + Math.round((m.sizeBytes || 0) / 1024) + ' KB · ' + U.escapeHtml(m.contentType || '') + '</div>' +
                  '<div style="display:flex;gap:8px;">' +
                    '<button type="button" class="aeon-btn aeon-btn--secondary" data-copy-url="' + U.escapeHtml(m.relativeUrl) + '">' + U.escapeHtml(t('btnCopyUrl')) + '</button>' +
                    '<button type="button" class="aeon-btn aeon-btn--ghost" data-del-media="' + idx + '" style="color:var(--seo-red);">' + U.escapeHtml(t('btnDelete')) + '</button>' +
                  '</div>' +
                '</div>';
              }).join('')
            : '<p style="color:var(--ink-mid);">' + U.escapeHtml(t('noMediaSession')) + '</p>') +
        '</div>' +
      '</div>';

    document.getElementById('media-up-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var file = e.target.file.files[0];
      var folder = e.target.folder.value;
      if (!file) return;
      try {
        var res = await Rana.api.adminUploadMedia(file, folder);
        Admin.uploadedMedia.unshift(res);
        UI.toast(t('toastFileUploaded') + res.relativeUrl, 'success');
        loadMediaTab();
      } catch (err) { UI.toast(err.message, 'warn'); }
    });

    wsEl().querySelectorAll('[data-copy-url]').forEach(function (b) {
      b.addEventListener('click', function () {
        navigator.clipboard.writeText(b.getAttribute('data-copy-url')).then(function () { UI.toast(t('toastUrlCopied'), 'success'); });
      });
    });

    wsEl().querySelectorAll('[data-del-media]').forEach(function (b) {
      b.addEventListener('click', async function () {
        var idx = Number(b.getAttribute('data-del-media'));
        var item = Admin.uploadedMedia[idx];
        await Rana.api.adminDeleteMedia(item.relativeUrl);
        Admin.uploadedMedia.splice(idx, 1);
        UI.toast(t('toastFileDeleted'));
        loadMediaTab();
      });
    });
  }

  /* ---------------------- 8. USERS & ROLES (ADMIN) ---------------------- */
  async function loadUsersTab() {
    var filters = { search: '', role: '', pageNumber: 1, pageSize: 20 };
    async function refresh() {
      wsEl().innerHTML = '<p style="font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(t('loadingUsers')) + '</p>';
      try {
        var page = await Rana.api.adminGetUsers(filters);
        var roles = await Rana.api.adminGetRoles();
        var users = page.items || [];

        wsEl().innerHTML =
          '<div class="paper-panel">' +
            '<div class="paper-panel__header">' +
              '<h2 class="paper-panel__title">' + U.escapeHtml(t('usersHeading', { n: page.totalCount })) + '</h2>' +
              '<div style="display:flex;gap:8px;flex-wrap:wrap;">' +
                '<input type="search" id="usr-search" class="field-input" style="width:220px;" placeholder="' + U.escapeHtml(t('phUserSearch')) + '" value="' + U.escapeHtml(filters.search) + '" />' +
                '<button type="button" id="usr-filter-btn" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(t('btnSearch')) + '</button>' +
                '<button type="button" id="usr-create-role" class="aeon-btn">' + U.escapeHtml(t('btnCreateRole')) + '</button>' +
              '</div>' +
            '</div>' +
            '<div class="admin-table-wrap"><table class="admin-table"><thead><tr><th>' + U.escapeHtml(t('thUser')) + '</th><th>' + U.escapeHtml(t('thRoles')) + '</th><th>' + U.escapeHtml(t('thStatusGl')) + '</th><th>' + U.escapeHtml(t('thManage')) + '</th></tr></thead><tbody>' +
              users.map(function (u) {
                return '<tr>' +
                  '<td><strong>' + U.escapeHtml(u.fullName) + '</strong><div style="font-size:12.5px;color:var(--ink-mid);">' + U.escapeHtml(u.email) + '</div></td>' +
                  '<td>' + (u.roleName || []).map(function (r) {
                    return '<span class="admin-role-chip" style="margin-right:4px;">' + U.escapeHtml(r) + ' <button type="button" data-rm-role="' + u.id + '" data-role="' + U.escapeHtml(r) + '" style="color:var(--aeon-red);margin-left:3px;">×</button></span>';
                  }).join('') +
                    '<div style="margin-top:6px;display:flex;gap:4px;">' +
                      '<select class="field-select" style="padding:4px 6px;font-size:12px;width:120px;" id="assign-sel-' + u.id + '">' +
                        roles.map(function (r) { return '<option value="' + U.escapeHtml(r.name) + '">' + U.escapeHtml(r.name) + '</option>'; }).join('') +
                      '</select>' +
                      '<button type="button" class="aeon-btn aeon-btn--secondary" style="padding:4px 8px;font-size:11px;" data-assign-role="' + u.id + '">' + U.escapeHtml(t('btnAssign')) + '</button>' +
                    '</div></td>' +
                  '<td style="font-size:12.5px;">' + (u.isActive ? '<span class="status-pill status-pill--Green">' + U.escapeHtml(t('stActive')) + '</span>' : '<span class="status-pill status-pill--Red">' + U.escapeHtml(t('stInactive')) + '</span>') + '<div style="margin-top:4px;color:var(--ink-mid);">' + U.escapeHtml(t('lblGuidelines')) + ' ' + U.escapeHtml(u.communityGuidelinesAcceptedAt ? t('glAccepted') : t('glPending')) + '</div></td>' +
                  '<td style="white-space:nowrap;">' +
                    '<button type="button" class="aeon-btn aeon-btn--secondary" data-toggle-active="' + u.id + '" data-next="' + (!u.isActive) + '">' + U.escapeHtml(u.isActive ? t('btnDeactivate') : t('btnActivate')) + '</button> ' +
                    '<button type="button" class="aeon-btn aeon-btn--ghost" data-del-usr="' + u.id + '" style="padding:7px 10px;font-size:12px;color:var(--seo-red);">' + U.escapeHtml(t('btnDelete')) + '</button>' +
                  '</td>' +
                '</tr>';
              }).join('') +
            '</tbody></table></div>' +
          '</div>';

        document.getElementById('usr-filter-btn').addEventListener('click', function () {
          filters.search = document.getElementById('usr-search').value.trim();
          refresh();
        });
        document.getElementById('usr-create-role').addEventListener('click', async function () {
          var name = await UI.promptModal({ title: t('btnCreateRole'), message: t('promptNewRole'), required: true });
          if (!name || !name.trim()) return;
          try {
            await Rana.api.adminCreateRole({ name: name.trim(), description: name.trim() + ' role' });
            UI.toast(t('toastRoleCreated'), 'success');
            refresh();
          } catch (err) { UI.toast(err.message, 'warn'); }
        });
        wsEl().querySelectorAll('[data-assign-role]').forEach(function (b) {
          b.addEventListener('click', async function () {
            var uid = b.getAttribute('data-assign-role');
            var role = document.getElementById('assign-sel-' + uid).value;
            await Rana.api.adminAssignRole(uid, role);
            UI.toast(t('toastRoleAssigned'), 'success');
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-rm-role]').forEach(function (b) {
          b.addEventListener('click', async function () {
            await Rana.api.adminRemoveRole(b.getAttribute('data-rm-role'), b.getAttribute('data-role'));
            UI.toast(t('toastRoleRemoved'));
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-toggle-active]').forEach(function (b) {
          b.addEventListener('click', async function () {
            await Rana.api.editUser({ isActive: b.getAttribute('data-next') === 'true' }, b.getAttribute('data-toggle-active'));
            refresh();
          });
        });
        wsEl().querySelectorAll('[data-del-usr]').forEach(function (b) {
          b.addEventListener('click', async function () {
            if (!(await UI.confirmModal({ message: t('confirmDelUser'), danger: true }))) return;
            await Rana.api.adminDeleteUser(b.getAttribute('data-del-usr'));
            refresh();
          });
        });
      } catch (err) {
        wsEl().innerHTML = '<div class="paper-panel"><p style="color:var(--seo-red);">' + U.escapeHtml(err.message) + '</p></div>';
      }
    }
    refresh();
  }

  global.RanaAdmin = { boot: boot, setLang: setLang };

  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape' && Rana.ui && Rana.ui.closeModal) Rana.ui.closeModal();
  });

  document.addEventListener('DOMContentLoaded', boot);
})(window);
