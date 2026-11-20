/* ==========================================================================
   Rana Media Configuration, Public i18n (AZ / EN) & Shared Helpers
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});

  var isHttp = global.location && /^https?:/.test(global.location.protocol);
  var queryApi = null;
  try {
    var sp = new URLSearchParams((global.location && global.location.search) || '');
    var q = sp.get('api');
    if (q && /^https?:\/\//i.test(q.trim())) {
      queryApi = q.trim().replace(/\/+$/, '');
      global.localStorage.setItem('rana_api_base', queryApi);
    }
  } catch (_) {}

  var storedBase = null;
  try {
    storedBase = global.localStorage.getItem('rana_api_base');
  } catch (_) {}

  var defaultApi = (global.RANA_API_BASE || queryApi || storedBase || 'http://localhost:5000').replace(/\/+$/, '');

  Rana.config = {
    API_BASE: defaultApi,
    SITE_NAME: 'Rana Media',
    DOMAIN: 'ranamedia.co',
    isFileProtocol: !isHttp,
    isAdminPath: !!(global.location && /\/admin(\/|$)/i.test(global.location.pathname)),
    setApiBase: function (url) {
      var clean = String(url || '').trim().replace(/\/+$/, '');
      if (!clean) return Rana.config.API_BASE;
      Rana.config.API_BASE = clean;
      try { global.localStorage.setItem('rana_api_base', clean); } catch (_) {}
      return clean;
    }
  };

  var SITE_LANG_KEY = 'rana_site_lang';
  Rana.getLang = function () {
    try {
      return global.localStorage.getItem(SITE_LANG_KEY) === 'en' ? 'en' : 'az';
    } catch (_) {
      return 'az';
    }
  };
  Rana.setLang = function (next) {
    if (next !== 'az' && next !== 'en') return;
    try { global.localStorage.setItem(SITE_LANG_KEY, next); } catch (_) {}
    document.documentElement.lang = next;
    if (typeof Rana.onLangChange === 'function') Rana.onLangChange(next);
  };

  Rana.I18N = {
    az: {
      siteTagline: 'Rana Media | İdeyalar Dünyası',
      navMenu: 'MENYU',
      navSearch: 'AXTAR',
      navNewsletter: 'BÜLLETEN',
      navSignIn: 'DAXİL OL',
      navLibrary: 'KİTABXANA',
      navStudio: 'STUDİYA',
      close: 'BAĞLA',
      searchPh: 'Esse, video və müəllif axtar…',
      searchOverlayPh: 'Esselər, videolar, ideyalar axtar…',
      searchOverlayHint: 'Başlıq, anons, tam mətn, mövzu və müəlliflər üzrə axtarmaq üçün Enter basın.',
      offlineNotice: 'Önizləmə Rejimi — Backend API ({api}) cavab vermir. Nümunə məqalələrlə göstərilir.',
      changeApiUrl: 'API URL dəyiş',
      retryConnect: 'Yenidən qoşul',
      adminStudio: 'Admin Studiya',
      promptApiTitle: 'Backend API',
      promptApiMsg: 'Backend API ünvanını daxil edin:',
      secPhilosophy: 'Fəlsəfə',
      secScience: 'Elm',
      secPsychology: 'Psixologiya',
      secSociety: 'Cəmiyyət',
      secCulture: 'Mədəniyyət',
      typeEssay: 'esse',
      typeVideo: 'video',
      typeIdea: 'ideya',
      typeClassic: 'klassik',
      pageEssays: 'Esselər',
      pagePopularEssays: 'Populyar Esselər',
      pageVideos: 'Videolar',
      pagePopular: 'Populyar',
      pageConversations: 'Müzakirələr',
      pageWriteForUs: 'Bizimlə yaz',
      pageRepublish: 'Təkrar nəşr',
      pageAbout: 'Haqqımızda',
      pageContact: 'Əlaqə',
      pageSupport: 'Dəstək',
      pageGuidelines: 'İcma Qaydaları',
      pageEditorialAdmin: 'Redaksiya Admin',
      pageRss: 'RSS Lenti',
      pageAtom: 'Atom Lenti',
      followUs: 'Rana Media-nı izləyin',
      nlSignupTitle: 'Bülletenimizə abunə olun',
      nlSignupSub: 'Rana Media-da yayımlanan bütün yeniliklər e-poçtunuzda.',
      nlEmailPh: 'E-poçt ünvanınız',
      nlDaily: 'Gündəlik',
      nlWeekly: 'Həftəlik',
      nlSubscribe: 'Abunə ol',
      nlPrivacyNote: 'Bülleten məxfilik siyasətimizə baxın.',
      nlPrivacyLink: 'Bülleten məxfilik siyasəti',
      nlChooseFreq: 'Zəhmət olmasa Gündəlik və ya Həftəlik seçin.',
      nlConfirmSent: 'Təsdiq e-poçtu göndərildi.',
      nlPreviewSaved: 'Önizləmə rejimində abunəlik qeydə alındı.',
      footerCopyright: '© Rana Media. Bütün hüquqlar qorunur.',
      privacyPolicy: 'Məxfilik Siyasəti',
      termsOfUse: 'İstifadə Şərtləri',
      cardSave: 'Saxla',
      cardSaved: 'Saxlanılıb',
      cardWatch: 'İzlə',
      cardMinutes: '{n} dəqiqə',
      loginToSave: 'Məqaləni kitabxanaya əlavə etmək üçün daxil olun.',
      savedToLib: 'Kitabxananıza əlavə edildi.',
      removedFromLib: 'Kitabxanadan çıxarıldı.',
      opFailed: 'Əməliyyat alınmadı.',
      loading: 'Yüklənir…',
      loadingUpper: 'YÜKLƏNİR…',
      moreBtn: 'DAHA ÇOX',
      popularThisMonth: 'Bu ay populyar olanlar',
      sortLatest: 'Ən yenilər',
      sortPopular: 'Populyar',
      sortOriginals: 'Orijinallar',
      sortExclusives: 'Eksklüzivlər',
      sortNewest: 'Ən yeni',
      emptySection: 'Bu mövzuda hələ məqalə dərc edilməyib.',
      emptyCollection: 'Bu bölmədə hələ material yoxdur.',
      wordsCount: '{n} söz',
      editedBy: 'Redaktor:',
      syndicateEssay: 'BU ESSENİ TƏKRAR NƏŞR ET',
      share: 'Paylaş',
      notesCount: 'Qeydlər',
      commentsTitle: 'Şərhlər',
      listenEssay: 'Bu esseni dinləyin',
      listenMinutes: '{n} dəqiqəlik səsli oxu',
      linkCopied: 'Məqalə linki panoya kopyalandı.',
      hlTitle: 'Vurğulamalar və Qeydlər',
      hlMyTitle: 'Vurğulamalarım və Qeydlərim ({n})',
      hlEmpty: 'Məqalədə istənilən cümləni seçərək vurğulaya və ya qeyd əlavə edə bilərsiniz.',
      hlNotePrefix: 'Qeyd #{n}:',
      hlRemove: 'Sil',
      hlBtnHighlight: 'Vurğula',
      hlBtnAnnotate: 'Qeyd yaz',
      hlPromptTitle: 'Qeyd (Annotation)',
      hlPromptMsg: 'Qeydinizi yazın:',
      hlSavedNote: 'Qeyd əlavə edildi.',
      hlSavedMark: 'Mətn vurğulandı.',
      hlSaveErr: 'Yadda saxlanmadı.',
      cmtClosed: 'Bu məqalə üçün şərhlər bağlanıb.',
      cmtLoginPrompt: 'Müzakirəyə qoşulmaq üçün hesabınıza daxil olun.',
      cmtLoginBtn: 'Şərh yazmaq üçün daxil ol',
      cmtGlTitle: 'İcma Qaydaları (Community Guidelines)',
      cmtGlDesc: 'Şərh yazmazdan əvvəl İcma Qaydalarını qəbul edin. Hər istifadəçi bir məqaləyə yalnız 1 əsas şərh yaza bilər və yazdığı şərhi ilk 1 saat ərzində redaktə edə bilər.',
      cmtGlAccept: 'İcma Qaydalarını Qəbul Et və Davam Et',
      cmtGlReadFull: 'Tam qaydaları oxu',
      cmtOneLimit: 'Siz bu məqaləyə artıq 1 əsas şərh yazmısınız (hər məqaləyə 1 əsas şərh limiti var). Digər şərhlərə cavab (Reply) yaza bilərsiniz.',
      cmtJumpMine: 'Öz şərhimə keç',
      cmtPlaceholder: 'Bu məqaləyə düşüncəli rəy yazın… (hər məqaləyə 1 əsas şərh, 1 saat ərzində redaktə imkanı)',
      cmtRuleHint: 'Hər məqaləyə 1 əsas şərh · 1 saat redaktə pəncərəsi',
      cmtPostBtn: 'Şərh Yaz',
      cmtEmpty: 'Hələ şərh yoxdur. İlk şərhi siz yazın.',
      cmtMore: 'DAHA ÇOX ŞƏRH',
      cmtPosted: 'Şərhiniz dərc edildi! İlk 1 saat ərzində redaktə edə bilərsiniz.',
      cmtPostErr: 'Şərh göndərilə bilmədi.',
      cmtAuthorBadge: 'Müəllif',
      cmtFeaturedBadge: 'Seçilmiş',
      cmtLike: 'Bəyən',
      cmtReply: 'Cavab ver',
      cmtReport: 'Şikayət et',
      cmtEditLeft: 'Redaktə ({m} dəq qalıb)',
      cmtDelete: 'Sil',
      cmtSaveChanges: 'Dəyişiklikləri Saxla',
      cmtCancel: 'Ləğv et',
      cmtUpdated: 'Şərh yeniləndi.',
      cmtDelConfirm: 'Şərhi silmək istədiyinizə əminsiniz?',
      cmtDeleted: 'Şərh silindi.',
      cmtReplyNeedGl: 'Cavab yazmazdan əvvəl İcma Qaydalarını təsdiq edin.',
      cmtReplyPh: 'Cavabınızı yazın…',
      cmtPostReply: 'Cavab Göndər',
      cmtReplySent: 'Cavab göndərildi.',
      cmtReplyUpdated: 'Cavab yeniləndi.',
      cmtReplyDelConfirm: 'Cavabı silmək istədiyinizə əminsiniz?',
      cmtReplyDeleted: 'Cavab silindi.',
      cmtReportTitle: 'Şərhi Şikayət Et',
      cmtReportReason: 'Səbəb',
      cmtReportPh: 'İcma Qaydalarının necə pozulduğunu təsvir edin…',
      cmtReportSubmit: 'Şikayət Göndər',
      cmtReportSent: 'Şikayət moderatorlara göndərildi.',
      authSignIn: 'Daxil ol',
      authRegister: 'Qeydiyyat',
      authReset: 'Şifrəni sıfırla',
      authEmail: 'E-poçt',
      authPassword: 'Şifrə',
      authFirstName: 'Ad',
      authLastName: 'Soyad',
      authPassHint: 'Şifrə (min 8 simvol, rəqəm və böyük hərf)',
      authPhoneOpt: 'Telefon (istəyə bağlı)',
      authCreateAcct: 'Oxucu Hesabı Yarat',
      authRegDone: 'Qeydiyyat tamamlandı! İndi daxil ola bilərsiniz.',
      authOtpStep1: '1. E-poçta OTP Kodu Göndər',
      authOtpSendBtn: 'Sıfırlama Kodu Göndər',
      authOtpStep2: '2. Təsdiq Kodu (OTP)',
      authOtpNewPass: 'Yeni Şifrə',
      authOtpResetBtn: 'Şifrəni Yenilə',
      authOtpSentToast: 'OTP kodu e-poçtunuza göndərildi.',
      authPassUpdatedToast: 'Şifrə yeniləndi!',
      authWelcome: 'Xoş gəldiniz, {name}!',
      authSignOutToast: 'Sistemdən çıxış edildi.',
      acctBio: 'Haqqında (Bio)',
      acctGlLabel: 'İcma Qaydaları:',
      acctGlAccepted: 'Qəbul edilib ({date})',
      acctGlAcceptNow: 'İndi təsdiqlə',
      acctSaveProfile: 'Profili Saxla',
      acctOpenStudio: 'Admin Studiyanı Aç',
      acctSignOut: 'Çıxış',
      acctUpdated: 'Profil yeniləndi.',
      notifTitle: 'Bildirişlər',
      notifEmpty: 'Hazırda bildiriş yoxdur.',
      notifMarkAll: 'Hamısını oxunmuş et',
      contribAuthored: 'Müəllif olduqları',
      contribEdited: 'Redaktə etdikləri',
      searchPagePh: 'Məqalə, mövzu, müəllif axtar…',
      searchAll: 'Hamısı',
      searchEmpty: 'Axtarış üzrə nəticə tapılmadı.',
      libTitle: 'Kitabxanam',
      libGuestSub: 'Saxladığınız məqalələri və mətn qeydlərinizi görmək üçün daxil olun.',
      libSavedTab: 'Saxlanılan Məqalələr ({n})',
      libAnnotatedTab: 'Vurğulanmış və Qeydli ({n})',
      libEmpty: 'Bu siyahı boşdur.',
      nlPageTitle: 'Rana Media-ya abunə olun',
      nlPageSub: 'Ən son esselərimiz və qısa sənədli filmlərimiz birbaşa e-poçtunuza gəlsin — pulsuz və keyfiyyətli.',
      nlDailyEd: 'Gündəlik Nəşr',
      nlWeeklyDig: 'Həftəlik İcmal',
      nlConfirmedTitle: 'Abunəlik təsdiqləndi',
      nlConfirmedSub: 'Bülleten abunəliyiniz uğurla aktivləşdirildi.',
      nlBackHome: 'Rana Media-ya qayıt',
      nlConfirmFail: 'Təsdiq alınmadı',
      nlUnsubTitle: 'Abunəlik dayandırıldı',
      nlUnsubSub: 'Bülleten abunəliyiniz ləğv edildi.',
      pitchTitle: 'Rana Media üçün yazın',
      pitchSub: 'Orijinal esse və ya fəlsəfi ideya təklifinizi redaksiya heyətimizə göndərin.',
      pitchName: 'Adınız',
      pitchEmail: 'E-poçt ünvanı',
      pitchBio: 'Qısa bioqrafiya',
      pitchBioPh: 'Akademik fəaliyyətiniz, tədqiqat sahəniz və ya dərc olunmuş işləriniz…',
      pitchWebsite: 'Vebsayt (istəyə bağlı)',
      pitchPrev: 'Daha əvvəl bizimlə dərc olunmusunuz',
      pitchWorkTitle: 'İşçi başlıq',
      pitchField: 'Sahə / Bölmə',
      pitchFormat: 'Format',
      pitchFmtEssay: 'Geniş Esse (2,500–4,000 söz)',
      pitchFmtIdea: 'Qısa İdeya (800–1,200 söz)',
      pitchBody: 'Təklifiniz (min 50 simvol)',
      pitchBodyPh: 'Əsas tezisi, mövzunun aktuallığını və arqumentin strukturunu izah edin…',
      pitchSubmit: 'Təklifi Göndər',
      pitchSuccess: 'Yazı təklifiniz redaksiyaya uğurla göndərildi!',
      syndTitle: 'Esseni Təkrar Nəşr Et (Sindikasiya)',
      syndSub: 'Rana Media essesini çap və ya onlayn nəşrinizdə təkrar dərc etmək üçün icazə müraciəti göndərin.',
      syndSlug: 'Məqalə Slug-ı',
      syndOrg: 'Təşkilat / Şirkət',
      syndPub: 'Nəşr Adı',
      syndAddr: 'Ünvan',
      syndCity: 'Şəhər',
      syndState: 'Rayon / Region',
      syndPost: 'Poçt Kodu',
      syndCountry: 'Ölkə',
      syndLang: 'Dil',
      syndPubUrl: 'Nəşr URL-i',
      syndOnline: 'Onlayn Təkrar Nəşr',
      syndPrint: 'Çap Təkrar Nəşr',
      syndTerms: 'Sindikasiya və müəllif istinadı şərtlərini qəbul edirəm.',
      syndSubmit: 'Sindikasiya Müraciəti Göndər',
      syndSuccess: 'Sindikasiya müraciətiniz qeydə alındı!',
      articleNotFound: 'Məqalə tapılmadı',
      videoNotFound: 'Video tapılmadı',
      pageNotFound: 'Səhifə tapılmadı'
    },
    en: {
      siteTagline: 'Rana Media | a world of ideas',
      navMenu: 'MENU',
      navSearch: 'SEARCH',
      navNewsletter: 'NEWSLETTER',
      navSignIn: 'SIGN IN',
      navLibrary: 'LIBRARY',
      navStudio: 'STUDIO',
      close: 'CLOSE',
      searchPh: 'Search essays, videos, authors…',
      searchOverlayPh: 'Search essays, videos, ideas…',
      searchOverlayHint: 'Press Enter to search across titles, standfirsts, full text, topics and contributors.',
      offlineNotice: 'Editorial Preview Mode — Backend API ({api}) unreachable. Showing sample articles.',
      changeApiUrl: 'Change API URL',
      retryConnect: 'Reconnect',
      adminStudio: 'Admin Studio',
      promptApiTitle: 'Backend API',
      promptApiMsg: 'Enter Backend API Base URL:',
      secPhilosophy: 'Philosophy',
      secScience: 'Science',
      secPsychology: 'Psychology',
      secSociety: 'Society',
      secCulture: 'Culture',
      typeEssay: 'essay',
      typeVideo: 'video',
      typeIdea: 'idea',
      typeClassic: 'classic',
      pageEssays: 'Essays',
      pagePopularEssays: 'Popular Essays',
      pageVideos: 'Videos',
      pagePopular: 'Popular',
      pageConversations: 'Conversations',
      pageWriteForUs: 'Write for us',
      pageRepublish: 'Republish',
      pageAbout: 'About',
      pageContact: 'Contact',
      pageSupport: 'Support',
      pageGuidelines: 'Community Guidelines',
      pageEditorialAdmin: 'Editorial Admin',
      pageRss: 'RSS Feed',
      pageAtom: 'Atom Feed',
      followUs: 'Follow Rana Media',
      nlSignupTitle: 'Sign up to our newsletter',
      nlSignupSub: 'Updates on everything new at Rana Media.',
      nlEmailPh: 'Your email address',
      nlDaily: 'Daily',
      nlWeekly: 'Weekly',
      nlSubscribe: 'Subscribe',
      nlPrivacyNote: 'See our newsletter privacy policy.',
      nlPrivacyLink: 'newsletter privacy policy',
      nlChooseFreq: 'Please select Daily or Weekly.',
      nlConfirmSent: 'Confirmation email sent.',
      nlPreviewSaved: 'Subscribed in preview mode.',
      footerCopyright: '© Rana Media. All rights reserved.',
      privacyPolicy: 'Privacy Policy',
      termsOfUse: 'Terms of Use',
      cardSave: 'Save',
      cardSaved: 'Saved',
      cardWatch: 'Watch',
      cardMinutes: '{n} minutes',
      loginToSave: 'Sign in to save articles to your library.',
      savedToLib: 'Saved to My Library.',
      removedFromLib: 'Removed from My Library.',
      opFailed: 'Action failed.',
      loading: 'Loading…',
      loadingUpper: 'LOADING…',
      moreBtn: 'MORE',
      popularThisMonth: 'Popular this month',
      sortLatest: 'Latest',
      sortPopular: 'Popular',
      sortOriginals: 'Originals',
      sortExclusives: 'Exclusives',
      sortNewest: 'Newest',
      emptySection: 'No articles published in this topic yet.',
      emptyCollection: 'No items in this collection yet.',
      wordsCount: '{n} words',
      editedBy: 'Edited by',
      syndicateEssay: 'SYNDICATE THIS ESSAY',
      share: 'Share',
      notesCount: 'Notes',
      commentsTitle: 'Comments',
      listenEssay: 'Listen to this essay',
      listenMinutes: '{n} minute listen',
      linkCopied: 'Article link copied to clipboard.',
      hlTitle: 'Highlights & Annotations',
      hlMyTitle: 'My Highlights & Annotations ({n})',
      hlEmpty: 'Select any sentence in the article to highlight or add an annotation.',
      hlNotePrefix: 'Note #{n}:',
      hlRemove: 'Remove',
      hlBtnHighlight: 'Highlight',
      hlBtnAnnotate: 'Annotate',
      hlPromptTitle: 'Annotation',
      hlPromptMsg: 'Write your annotation:',
      hlSavedNote: 'Annotation added.',
      hlSavedMark: 'Text highlighted.',
      hlSaveErr: 'Could not save.',
      cmtClosed: 'Comments are closed for this article.',
      cmtLoginPrompt: 'Sign in to join the conversation.',
      cmtLoginBtn: 'Sign in to comment',
      cmtGlTitle: 'Community Guidelines',
      cmtGlDesc: 'Before commenting, please accept our Community Guidelines. Each reader may publish 1 root comment per article and edit it within 1 hour.',
      cmtGlAccept: 'Accept Community Guidelines & Continue',
      cmtGlReadFull: 'Read full guidelines',
      cmtOneLimit: 'You have already posted 1 root comment on this article. You can still reply to existing comments.',
      cmtJumpMine: 'Jump to my comment',
      cmtPlaceholder: 'Share a considered response to this article… (1 root comment per article, editable within 1 hour)',
      cmtRuleHint: '1 root comment per article · 1 hour edit window',
      cmtPostBtn: 'Post Comment',
      cmtEmpty: 'No comments yet. Be the first to share a response.',
      cmtMore: 'MORE COMMENTS',
      cmtPosted: 'Comment posted! You can edit it within the next hour.',
      cmtPostErr: 'Could not post comment.',
      cmtAuthorBadge: 'Author',
      cmtFeaturedBadge: 'Featured',
      cmtLike: 'Like',
      cmtReply: 'Reply',
      cmtReport: 'Report',
      cmtEditLeft: 'Edit ({m}m left)',
      cmtDelete: 'Delete',
      cmtSaveChanges: 'Save Changes',
      cmtCancel: 'Cancel',
      cmtUpdated: 'Comment updated.',
      cmtDelConfirm: 'Are you sure you want to delete this comment?',
      cmtDeleted: 'Comment deleted.',
      cmtReplyNeedGl: 'Accept Community Guidelines before replying.',
      cmtReplyPh: 'Write a reply…',
      cmtPostReply: 'Post Reply',
      cmtReplySent: 'Reply posted.',
      cmtReplyUpdated: 'Reply updated.',
      cmtReplyDelConfirm: 'Are you sure you want to delete this reply?',
      cmtReplyDeleted: 'Reply deleted.',
      cmtReportTitle: 'Report Comment',
      cmtReportReason: 'Reason',
      cmtReportPh: 'Describe how this violates Community Guidelines…',
      cmtReportSubmit: 'Submit Report',
      cmtReportSent: 'Report sent to moderators.',
      authSignIn: 'Sign In',
      authRegister: 'Register',
      authReset: 'Reset Password',
      authEmail: 'Email',
      authPassword: 'Password',
      authFirstName: 'First Name',
      authLastName: 'Last Name',
      authPassHint: 'Password (min 8, digit & uppercase)',
      authPhoneOpt: 'Phone (optional)',
      authCreateAcct: 'Create Reader Account',
      authRegDone: 'Registration complete! You can now sign in.',
      authOtpStep1: '1. Send OTP to Email',
      authOtpSendBtn: 'Send Reset OTP Code',
      authOtpStep2: '2. Verification Code (OTP)',
      authOtpNewPass: 'New Password',
      authOtpResetBtn: 'Reset Password',
      authOtpSentToast: 'OTP code sent to your email.',
      authPassUpdatedToast: 'Password updated!',
      authWelcome: 'Welcome, {name}!',
      authSignOutToast: 'Signed out.',
      acctBio: 'Bio',
      acctGlLabel: 'Community Guidelines:',
      acctGlAccepted: 'Accepted ({date})',
      acctGlAcceptNow: 'Accept now',
      acctSaveProfile: 'Save Profile',
      acctOpenStudio: 'Open Admin Studio',
      acctSignOut: 'Sign Out',
      acctUpdated: 'Profile updated.',
      notifTitle: 'Notifications',
      notifEmpty: 'No notifications yet.',
      notifMarkAll: 'Mark all read',
      contribAuthored: 'Authored',
      contribEdited: 'Edited',
      searchPagePh: 'Search articles, topics, contributors…',
      searchAll: 'All',
      searchEmpty: 'No results found.',
      libTitle: 'My Library',
      libGuestSub: 'Sign in to view your saved articles and highlights.',
      libSavedTab: 'Saved Articles ({n})',
      libAnnotatedTab: 'Highlighted & Annotated ({n})',
      libEmpty: 'This list is empty.',
      nlPageTitle: 'Subscribe to Rana Media',
      nlPageSub: 'Get our latest essays and short documentaries delivered straight to your inbox — free, thoughtful, and unhurried.',
      nlDailyEd: 'Daily Edition',
      nlWeeklyDig: 'Weekly Digest',
      nlConfirmedTitle: 'Subscription confirmed',
      nlConfirmedSub: 'Your newsletter subscription has been activated.',
      nlBackHome: 'Back to Rana Media',
      nlConfirmFail: 'Confirmation failed',
      nlUnsubTitle: 'Unsubscribed',
      nlUnsubSub: 'Your newsletter subscription has been cancelled.',
      pitchTitle: 'Write for Rana Media',
      pitchSub: 'Pitch an original essay or philosophical idea to our editorial commissioning desk.',
      pitchName: 'Your Name',
      pitchEmail: 'Email Address',
      pitchBio: 'Short Biography',
      pitchBioPh: 'Your academic background, research field, or published work…',
      pitchWebsite: 'Website (optional)',
      pitchPrev: 'Previously published with us',
      pitchWorkTitle: 'Working Title',
      pitchField: 'Field / Section',
      pitchFormat: 'Format',
      pitchFmtEssay: 'Longform Essay (2,500–4,000 words)',
      pitchFmtIdea: 'Short Idea (800–1,200 words)',
      pitchBody: 'Your Pitch (min 50 characters)',
      pitchBodyPh: 'Explain the central thesis, why it matters now, and how the argument develops…',
      pitchSubmit: 'Submit Pitch',
      pitchSuccess: 'Your pitch has been submitted to our editorial desk!',
      syndTitle: 'Syndicate an Essay',
      syndSub: 'Request permission to republish a Rana Media essay in your print or online publication.',
      syndSlug: 'Article Slug',
      syndOrg: 'Organisation / Company',
      syndPub: 'Publication Name',
      syndAddr: 'Street Address',
      syndCity: 'City',
      syndState: 'State / Region',
      syndPost: 'Post Code',
      syndCountry: 'Country',
      syndLang: 'Language',
      syndPubUrl: 'Publication URL',
      syndOnline: 'Online Republication',
      syndPrint: 'Print Republication',
      syndTerms: 'I accept the syndication attribution terms and conditions.',
      syndSubmit: 'Submit Syndication Request',
      syndSuccess: 'Your syndication request has been submitted!',
      articleNotFound: 'Article not found',
      videoNotFound: 'Video not found',
      pageNotFound: 'Page not found'
    }
  };

  Rana.t = function (key, vars) {
    var lang = Rana.getLang();
    var dict = Rana.I18N[lang] || Rana.I18N.az;
    var str = dict[key] !== undefined ? dict[key] : (Rana.I18N.en[key] !== undefined ? Rana.I18N.en[key] : key);
    if (vars) {
      Object.keys(vars).forEach(function (k) {
        str = str.replace(new RegExp('\\{' + k + '\\}', 'g'), String(vars[k]));
      });
    }
    return str;
  };

  Rana.state = {
    offlineMode: false,
    user: null,
    savedIds: new Set(),
    savedSlugs: new Set(),
    notifications: [],
    sectionsCatalog: null
  };

  Rana.SECTIONS = [
    { slug: 'philosophy', title: 'Philosophy', color: '#0f5c43', tint: '#bfeadd' },
    { slug: 'science',    title: 'Science',    color: '#115e59', tint: '#b8ebe6' },
    { slug: 'psychology', title: 'Psychology', color: '#2d6a4f', tint: '#c6ebd9' },
    { slug: 'society',    title: 'Society',    color: '#14532d', tint: '#c2ebd0' },
    { slug: 'culture',    title: 'Culture',    color: '#4a6b22', tint: '#d9ebb8' }
  ];

  Rana.utils = {
    escapeHtml: function (str) {
      if (str === null || str === undefined) return '';
      return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
    },

    stripHtml: function (html) {
      if (!html) return '';
      var tmp = document.createElement('div');
      tmp.innerHTML = html;
      return (tmp.textContent || tmp.innerText || '').trim();
    },

    slugify: function (text) {
      return String(text || '')
        .toLowerCase()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .replace(/[^a-z0-9\s-]/g, '')
        .trim()
        .replace(/[\s_-]+/g, '-')
        .replace(/^-+|-+$/g, '');
    },

    formatDate: function (iso) {
      if (!iso) return '';
      var d = new Date(iso);
      if (isNaN(d.getTime())) return '';
      var loc = Rana.getLang() === 'az' ? 'az-AZ' : 'en-GB';
      return d.toLocaleDateString(loc, { day: 'numeric', month: 'long', year: 'numeric' });
    },

    formatDateTime: function (iso) {
      if (!iso) return '';
      var d = new Date(iso);
      if (isNaN(d.getTime())) return '';
      var loc = Rana.getLang() === 'az' ? 'az-AZ' : 'en-GB';
      return d.toLocaleDateString(loc, { day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' });
    },

    sectionTitle: function (slug, fallback) {
      var map = {
        philosophy: 'secPhilosophy',
        science: 'secScience',
        psychology: 'secPsychology',
        society: 'secSociety',
        culture: 'secCulture'
      };
      var k = map[(slug || '').toLowerCase()];
      return k ? Rana.t(k) : (fallback || slug || '');
    },

    typeLabel: function (type) {
      var map = {
        essay: 'typeEssay',
        video: 'typeVideo',
        idea: 'typeIdea',
        classic: 'typeClassic'
      };
      var k = map[String(type || 'essay').toLowerCase()];
      return k ? Rana.t(k) : String(type || '').toLowerCase();
    },

    sectionClass: function (sectionSlugOrObj) {
      var slug = typeof sectionSlugOrObj === 'string'
        ? sectionSlugOrObj
        : (sectionSlugOrObj && sectionSlugOrObj.slug) || '';
      slug = slug.toLowerCase();
      var valid = ['philosophy', 'science', 'psychology', 'society', 'culture'];
      return valid.indexOf(slug) !== -1 ? 'section-' + slug : 'section-society';
    },

    sectionColor: function (slug) {
      var found = Rana.SECTIONS.find(function (s) { return s.slug === (slug || '').toLowerCase(); });
      return found ? found.color : '#14532d';
    },

    resolveMediaUrl: function (url) {
      var localPrefix = Rana.config.isAdminPath ? '../assets/images/' : 'assets/images/';
      if (!url) return localPrefix + 'sample-cosmos.svg';
      var sampleMap = {
        '/uploads/images/sample-cosmos.jpg': '/uploads/images/sample-cosmos.svg',
        '/uploads/images/sample-birds.jpg': '/uploads/images/sample-birds.svg',
        '/uploads/images/sample-mind.jpg': '/uploads/images/sample-mind.svg',
        '/uploads/images/sample-city.jpg': '/uploads/images/sample-city.svg',
        '/uploads/images/sample-art.jpg': '/uploads/images/sample-art.svg',
        '/uploads/images/sample-ethics.jpg': '/uploads/images/sample-ethics.svg'
      };
      var clean = sampleMap[url] || url;
      if (/^https?:\/\//i.test(clean) || clean.indexOf('data:') === 0) return clean;
      var sampleMatch = clean.match(/^\/?uploads\/images\/(sample-[a-z0-9-]+\.svg)$/i);
      if (sampleMatch && (Rana.state.offlineMode || Rana.config.isFileProtocol)) {
        return localPrefix + sampleMatch[1];
      }
      if (clean.indexOf('assets/') === 0) {
        return Rana.config.isAdminPath ? '../' + clean : clean;
      }
      if (clean.indexOf('uploads/') === 0) {
        return Rana.config.API_BASE + '/' + clean;
      }
      if (clean.charAt(0) === '/') {
        return Rana.config.API_BASE + clean;
      }
      return localPrefix + 'sample-cosmos.svg';
    },

    resolveBodyMediaUrls: function (html) {
      if (!html) return '';
      var tmp = document.createElement('div');
      tmp.innerHTML = html;
      tmp.querySelectorAll('img[src], video[src], audio[src], source[src]').forEach(function (el) {
        var raw = el.getAttribute('src');
        if (raw) el.setAttribute('src', Rana.utils.resolveMediaUrl(raw));
      });
      return tmp.innerHTML;
    },

    articleUrl: function (article) {
      var slug = encodeURIComponent(article.slug || '');
      if (String(article.type).toLowerCase() === 'video') {
        return '#/video/' + slug;
      }
      return '#/essay/' + slug;
    },

    initials: function (name) {
      var parts = String(name || 'R').trim().split(/\s+/);
      if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    },

    setDocumentTitle: function (title) {
      document.title = title ? (title + ' | Rana Media') : Rana.t('siteTagline');
    }
  };

  document.documentElement.lang = Rana.getLang();
})(window);
