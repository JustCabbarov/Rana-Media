/* ==========================================================================
   Rana Media Shared UI Components (Aeon Header, MegaMenu, Cards, Footer, Modals)
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});
  var U = Rana.utils;

  var ICONS = {
    ranaWordmark: function (extraClass) {
      return '<svg viewBox="0 0 185 48" xmlns="http://www.w3.org/2000/svg" role="img" aria-label="Rana Media" class="brand-logo-svg ' + (extraClass || '') + '">' +
        '<text x="92" y="38" text-anchor="middle" font-family="Newsreader, Georgia, Times New Roman, serif" font-size="44" font-weight="700" letter-spacing="-1.8">rana</text>' +
      '</svg>';
    },
    bookmark: function (saved, size) {
      var s = size || 19;
      if (saved) {
        return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor" aria-label="Saved"><path d="M17 3H7c-1.1 0-1.99.9-1.99 2L5 21l7-3 7 3V5c0-1.1-.9-2-2-2z"/></svg>';
      }
      return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor" aria-label="Save"><path d="M17 3H7c-1.1 0-1.99.9-1.99 2L5 21l7-3 7 3V5c0-1.1-.9-2-2-2zm0 15l-5-2.18L7 18V5h10v13z"/></svg>';
    },
    commentsBubble: function (size) {
      var s = size || 18;
      return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor" aria-label="Comments"><path d="M21.303,16.553l1.107,3.783l-3.48-2.322c-0.689,0.234-1.422,0.371-2.193,0.371c-1.041,0-2.024-0.234-2.907-0.642 c2.034-1.697,3.333-4.275,3.333-7.166c0-1.994-0.625-3.836-1.677-5.347c0.406-0.073,0.823-0.117,1.251-0.117 c3.762,0,6.814,2.971,6.814,6.634c0,1.594-0.578,3.057-1.539,4.2L21.303,16.553z M15.255,10.684c0,3.861-3.213,6.99-7.176,6.99 c-0.812,0-1.583-0.144-2.31-0.391l-3.667,2.446l1.166-3.985l-0.744-0.638c-1.013-1.203-1.621-2.743-1.621-4.423 c0-3.858,3.215-6.987,7.176-6.987C12.042,3.696,15.255,6.825,15.255,10.684z"/></svg>';
    },
    playCircle: function (size) {
      var s = size || 58;
      return '<svg viewBox="0 0 1225 1225" width="' + s + '" height="' + s + '" fill="none" stroke="white" aria-hidden="true">' +
        '<polygon stroke-width="34" stroke-miterlimit="10" points="457.2,844.9 457.2,349.9 839.6,599.6 "/>' +
        '<circle stroke-width="34" stroke-miterlimit="10" cx="615" cy="615" r="587.4"/></svg>';
    },
    search: function (size) {
      var s = size || 16;
      return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-label="Search"><circle cx="11" cy="11" r="7.5"/><line x1="21" y1="21" x2="16.5" y2="16.5"/></svg>';
    },
    share: function (size) {
      var s = size || 18;
      return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor" aria-label="Share"><path d="M18,16.08C17.24,16.08 16.56,16.38 16.04,16.85L8.91,12.7C8.96,12.47 9,12.24 9,12C9,11.76 8.96,11.53 8.91,11.3L15.96,7.19C16.5,7.69 17.21,8 18,8A3,3 0 0,0 21,5A3,3 0 0,0 18,2A3,3 0 0,0 15,5C15,5.24 15.04,5.47 15.09,5.7L8.04,9.81C7.5,9.31 6.79,9 6,9A3,3 0 0,0 3,12A3,3 0 0,0 6,15C6.79,15 7.5,14.69 8.04,14.19L15.16,18.34C15.11,18.55 15.08,18.77 15.08,19C15.08,20.61 16.39,21.91 18,21.91C19.61,21.91 20.92,20.61 20.92,19A2.92,2.92 0 0,0 18,16.08Z"/></svg>';
    },
    audioPlay: function () {
      return '<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><path d="M6 4l15 8-15 8V4zm2 3.4v9.2L16.8 12 8 7.4z"/></svg>';
    },
    audioPause: function () {
      return '<svg width="22" height="22" viewBox="0 0 24 24" fill="currentColor"><path d="M6 5h4v14H6zm8 0h4v14h-4z"/></svg>';
    },
    arrowRight: function () {
      return '<svg width="17" height="17" viewBox="0 0 24 24" fill="currentColor"><path d="M12 4l-1.41 1.41L16.17 11H4v2h12.17l-5.58 5.59L12 20l8-8z"/></svg>';
    },
    bell: function (size) {
      var s = size || 17;
      return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-label="Notifications"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>';
    },
    eye: function (size) {
      var s = size || 17;
      return '<svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
    },
    socialLinksHtml: function (size) {
      var s = size || 18;
      return '<a href="#/page/about" title="Facebook" aria-label="Facebook"><svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor"><path d="M22 12c0-5.52-4.48-10-10-10S2 6.48 2 12c0 4.99 3.66 9.12 8.44 9.88v-6.99H7.9v-2.89h2.54V9.8c0-2.5 1.49-3.89 3.78-3.89 1.09 0 2.24.2 2.24.2v2.46h-1.26c-1.24 0-1.63.77-1.63 1.56v1.87h2.78l-.45 2.89h-2.33v6.99C18.34 21.12 22 16.99 22 12z"/></svg></a>' +
        '<a href="#/page/about" title="Instagram" aria-label="Instagram"><svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor"><path d="M12 2.16c3.2 0 3.58.01 4.85.07 3.25.15 4.77 1.69 4.92 4.92.06 1.27.07 1.65.07 4.85s-.01 3.58-.07 4.85c-.15 3.23-1.66 4.77-4.92 4.92-1.27.06-1.64.07-4.85.07-3.2 0-3.58-.01-4.85-.07-3.26-.15-4.77-1.7-4.92-4.92-.06-1.27-.07-1.64-.07-4.85 0-3.2.01-3.58.07-4.85C2.38 3.86 3.9 2.31 7.15 2.16 8.42 2.17 8.8 2.16 12 2.16zm0 3.68a6.16 6.16 0 1 0 0 12.32 6.16 6.16 0 0 0 0-12.32zm0 10.16a4 4 0 1 1 0-8 4 4 0 0 1 0 8zm6.4-10.4a1.44 1.44 0 1 1 0-2.88 1.44 1.44 0 0 1 0 2.88z"/></svg></a>' +
        '<a href="#/videos" title="YouTube" aria-label="YouTube"><svg width="' + s + '" height="' + s + '" viewBox="0 0 24 24" fill="currentColor"><path d="M23.5 6.2a3 3 0 0 0-2.1-2.1C19.5 3.5 12 3.5 12 3.5s-7.5 0-9.4.6A3 3 0 0 0 .5 6.2 31 31 0 0 0 0 12a31 31 0 0 0 .5 5.8 3 3 0 0 0 2.1 2.1c1.9.6 9.4.6 9.4.6s7.5 0 9.4-.6a3 3 0 0 0 2.1-2.1A31 31 0 0 0 24 12a31 31 0 0 0-.5-5.8zM9.6 15.6V8.4l6.3 3.6-6.3 3.6z"/></svg></a>';
    }
  };

  function toast(message, type) {
    var root = document.getElementById('toast-root');
    if (!root) return;
    var el = document.createElement('div');
    el.className = 'toast-item' + (type ? ' toast-item--' + type : '');
    el.textContent = message;
    root.appendChild(el);
    setTimeout(function () {
      el.style.opacity = '0';
      el.style.transition = 'opacity 0.25s ease';
      setTimeout(function () { el.remove(); }, 260);
    }, 3400);
  }

  var activeModalOnClose = null;

  function openModal(html, maxWidth, onClose) {
    var root = document.getElementById('modal-root');
    if (!root) return;
    if (activeModalOnClose) {
      var prev = activeModalOnClose;
      activeModalOnClose = null;
      prev();
    }
    activeModalOnClose = typeof onClose === 'function' ? onClose : null;
    root.innerHTML =
      '<div class="modal-backdrop" id="active-modal-backdrop">' +
        '<div class="modal-panel" style="' + (maxWidth ? 'max-width:' + maxWidth + ';' : '') + '" role="dialog" aria-modal="true">' +
          '<button type="button" class="modal-close-btn" id="active-modal-close" aria-label="Close">×</button>' +
          html +
        '</div>' +
      '</div>';

    var backdrop = document.getElementById('active-modal-backdrop');
    var closeBtn = document.getElementById('active-modal-close');
    if (closeBtn) closeBtn.addEventListener('click', closeModal);
    if (backdrop) {
      backdrop.addEventListener('click', function (e) {
        if (e.target === backdrop) closeModal();
      });
    }
  }

  function closeModal() {
    var root = document.getElementById('modal-root');
    if (root) root.innerHTML = '';
    if (activeModalOnClose) {
      var cb = activeModalOnClose;
      activeModalOnClose = null;
      cb();
    }
  }

  function defaultModalTexts() {
    var isEn = false;
    try {
      isEn = Rana.config.isAdminPath
        ? (global.localStorage.getItem('rana_admin_lang') === 'en')
        : (Rana.getLang() === 'en');
    } catch (_) {}
    return isEn
      ? { confirmTitle: 'Confirm action', inputTitle: 'Enter details', ok: 'Confirm', del: 'Delete', cancel: 'Cancel' }
      : { confirmTitle: 'Təsdiq', inputTitle: 'Məlumat daxil edin', ok: 'Təsdiq et', del: 'Sil', cancel: 'Ləğv et' };
  }

  function confirmModal(opts) {
    opts = typeof opts === 'string' ? { message: opts } : (opts || {});
    var txt = defaultModalTexts();
    var title = opts.title || txt.confirmTitle;
    var okLabel = opts.confirmText || (opts.danger ? txt.del : txt.ok);
    var cancelLabel = opts.cancelText || txt.cancel;
    var okStyle = opts.danger ? ' style="background:#991b1b;border-color:#991b1b;"' : '';

    return new Promise(function (resolve) {
      var settled = false;
      function finish(val) {
        if (settled) return;
        settled = true;
        activeModalOnClose = null;
        closeModal();
        resolve(val);
      }
      var html =
        '<h3 class="font-serif" style="margin:0 0 10px;font-size:24px;line-height:1.2;">' + U.escapeHtml(title) + '</h3>' +
        '<p style="margin:0 0 22px;font-size:15px;line-height:1.55;color:var(--ink-dark);">' + U.escapeHtml(opts.message || '') + '</p>' +
        '<div style="display:flex;justify-content:flex-end;gap:10px;flex-wrap:wrap;">' +
          '<button type="button" class="aeon-btn aeon-btn--ghost" id="rana-cm-cancel">' + U.escapeHtml(cancelLabel) + '</button>' +
          '<button type="button" class="aeon-btn" id="rana-cm-ok"' + okStyle + '>' + U.escapeHtml(okLabel) + '</button>' +
        '</div>';

      openModal(html, '430px', function () {
        if (!settled) { settled = true; resolve(false); }
      });

      var okBtn = document.getElementById('rana-cm-ok');
      var cancelBtn = document.getElementById('rana-cm-cancel');
      if (okBtn) {
        okBtn.addEventListener('click', function () { finish(true); });
        setTimeout(function () { okBtn.focus(); }, 20);
      }
      if (cancelBtn) cancelBtn.addEventListener('click', function () { finish(false); });
    });
  }

  function promptModal(opts) {
    opts = typeof opts === 'string' ? { message: opts } : (opts || {});
    var txt = defaultModalTexts();
    var title = opts.title || txt.inputTitle;
    var okLabel = opts.confirmText || txt.ok;
    var cancelLabel = opts.cancelText || txt.cancel;

    return new Promise(function (resolve) {
      var settled = false;
      function finish(val) {
        if (settled) return;
        settled = true;
        activeModalOnClose = null;
        closeModal();
        resolve(val);
      }
      var html =
        '<h3 class="font-serif" style="margin:0 0 10px;font-size:24px;line-height:1.2;">' + U.escapeHtml(title) + '</h3>' +
        (opts.message ? '<p style="margin:0 0 14px;font-size:14.5px;line-height:1.5;color:var(--ink-mid);">' + U.escapeHtml(opts.message) + '</p>' : '') +
        '<form id="rana-pm-form" class="form-grid" style="gap:16px;">' +
          '<input type="text" id="rana-pm-input" class="field-input" value="' + U.escapeHtml(opts.defaultValue || '') + '" placeholder="' + U.escapeHtml(opts.placeholder || '') + '"' + (opts.required ? ' required' : '') + ' />' +
          '<div style="display:flex;justify-content:flex-end;gap:10px;flex-wrap:wrap;">' +
            '<button type="button" class="aeon-btn aeon-btn--ghost" id="rana-pm-cancel">' + U.escapeHtml(cancelLabel) + '</button>' +
            '<button type="submit" class="aeon-btn">' + U.escapeHtml(okLabel) + '</button>' +
          '</div>' +
        '</form>';

      openModal(html, '450px', function () {
        if (!settled) { settled = true; resolve(null); }
      });

      var form = document.getElementById('rana-pm-form');
      var inp = document.getElementById('rana-pm-input');
      var cancelBtn = document.getElementById('rana-pm-cancel');
      if (inp) {
        setTimeout(function () { inp.focus(); inp.select(); }, 20);
      }
      if (cancelBtn) cancelBtn.addEventListener('click', function () { finish(null); });
      if (form) {
        form.addEventListener('submit', function (e) {
          e.preventDefault();
          finish(inp ? inp.value : '');
        });
      }
    });
  }

  function renderOfflineBanner() {
    var root = document.getElementById('offline-banner');
    if (!root) return;
    if (!Rana.state.offlineMode) {
      root.innerHTML = '';
      return;
    }
    root.innerHTML =
      '<div class="offline-banner">' +
        '<span>' + U.escapeHtml(Rana.t('offlineNotice', { api: Rana.config.API_BASE })) + '</span>' +
        '<span style="display:flex;gap:14px;">' +
          '<button type="button" id="change-api-btn">' + U.escapeHtml(Rana.t('changeApiUrl')) + '</button>' +
          '<button type="button" id="retry-api-btn">' + U.escapeHtml(Rana.t('retryConnect')) + '</button>' +
          '<a href="admin/index.html">' + U.escapeHtml(Rana.t('adminStudio')) + '</a>' +
        '</span>' +
      '</div>';
    var changeApiBtn = document.getElementById('change-api-btn');
    if (changeApiBtn) {
      changeApiBtn.addEventListener('click', async function () {
        var next = await promptModal({
          title: Rana.t('promptApiTitle'),
          message: Rana.t('promptApiMsg'),
          defaultValue: Rana.config.API_BASE,
          required: true
        });
        if (next && next.trim()) {
          Rana.config.setApiBase(next);
          Rana.state.offlineMode = false;
          if (Rana.router && Rana.router.resolve) Rana.router.resolve();
        }
      });
    }
    var retryBtn = document.getElementById('retry-api-btn');
    if (retryBtn) {
      retryBtn.addEventListener('click', function () {
        Rana.state.offlineMode = false;
        if (Rana.router && Rana.router.resolve) Rana.router.resolve();
      });
    }
  }

  function renderHeader(options) {
    options = options || {};
    var header = document.getElementById('site-header');
    if (!header) return;

    var secCls = U.sectionClass(options.sectionSlug || 'society');
    var overlayCls = options.overlay ? ' site-header--overlay' : '';
    header.className = 'site-header no-print ' + secCls + overlayCls;

    var user = Rana.state.user;
    var isStaff = Rana.auth && Rana.auth.hasStaffRole();
    var unread = user ? (user.unreadNotificationsCount || 0) : 0;
    var unseenSaved = user ? (user.unseenSavedArticleCount || 0) : 0;
    var adminHref = 'admin/index.html';

    var lang = Rana.getLang();
    var langSwitcherHtml =
      '<div class="site-lang-switcher" role="group" aria-label="Language">' +
        '<button type="button" class="site-lang-btn' + (lang === 'az' ? ' is-active' : '') + '" data-site-lang="az">AZ</button>' +
        '<button type="button" class="site-lang-btn' + (lang === 'en' ? ' is-active' : '') + '" data-site-lang="en">EN</button>' +
      '</div>';

    var rightUserHtml = '';
    if (user) {
      rightUserHtml =
        '<a href="#/library" class="site-header__nav-link" title="' + U.escapeHtml(Rana.t('navLibrary')) + '">' + U.escapeHtml(Rana.t('navLibrary')) +
          (unseenSaved > 0 ? '<span class="header-badge-pill">' + unseenSaved + '</span>' : '') +
        '</a>' +
        '<span class="site-header__slash hide-sm">/</span>' +
        '<button type="button" class="site-header__nav-btn" id="hdr-notif-btn" title="' + U.escapeHtml(Rana.t('notifTitle')) + '">' +
          ICONS.bell(16) +
          (unread > 0 ? '<span class="header-badge-pill">' + unread + '</span>' : '') +
        '</button>' +
        '<span class="site-header__slash hide-sm">/</span>' +
        '<button type="button" class="site-header__nav-btn" id="hdr-account-btn">' +
          '<span class="hide-sm">' + U.escapeHtml(user.firstName || user.email) + '</span>' +
        '</button>' +
        (isStaff ? '<span class="site-header__slash hide-md">/</span><a href="' + adminHref + '" class="header-admin-pill hide-md">' + U.escapeHtml(Rana.t('navStudio')) + '</a>' : '');
    } else {
      rightUserHtml =
        '<button type="button" class="site-header__nav-btn" id="hdr-signin-btn">' + U.escapeHtml(Rana.t('navSignIn')) + '</button>';
    }

    header.innerHTML =
      '<div class="site-header__inner">' +
        '<div class="site-header__side">' +
          '<div class="site-header__mono-group">' +
            '<button type="button" class="site-header__nav-btn" id="hdr-menu-btn">' + U.escapeHtml(Rana.t('navMenu')) + '</button>' +
            '<span class="site-header__slash">/</span>' +
            '<button type="button" class="site-header__nav-btn" id="hdr-search-btn" aria-label="Search">' +
              ICONS.search(15) + '<span class="hide-md">' + U.escapeHtml(Rana.t('navSearch')) + '</span>' +
            '</button>' +
          '</div>' +
          '<span class="site-header__slash hide-md">/</span>' +
          '<div class="site-header__socials hide-md">' + ICONS.socialLinksHtml(17) + '</div>' +
        '</div>' +
        '<a href="#/" class="brand-logo-link">' + ICONS.ranaWordmark() + '</a>' +
        '<div class="site-header__side site-header__side--right">' +
          '<div class="site-header__mono-group">' +
            langSwitcherHtml +
            '<span class="site-header__slash hide-sm">/</span>' +
            '<a href="#/newsletter" class="site-header__nav-link hide-sm">' + U.escapeHtml(Rana.t('navNewsletter')) + '</a>' +
            '<span class="site-header__slash hide-sm">/</span>' +
            rightUserHtml +
          '</div>' +
        '</div>' +
      '</div>';

    header.querySelectorAll('[data-site-lang]').forEach(function (b) {
      b.addEventListener('click', function () {
        Rana.setLang(b.getAttribute('data-site-lang'));
      });
    });

    var menuBtn = document.getElementById('hdr-menu-btn');
    var searchBtn = document.getElementById('hdr-search-btn');
    var signinBtn = document.getElementById('hdr-signin-btn');
    var accountBtn = document.getElementById('hdr-account-btn');
    var notifBtn = document.getElementById('hdr-notif-btn');

    if (menuBtn) menuBtn.addEventListener('click', function () { toggleMegaMenu(true); });
    if (searchBtn) searchBtn.addEventListener('click', function () { toggleSearchOverlay(true); });
    if (signinBtn) signinBtn.addEventListener('click', function () { Rana.auth.openAuthModal('login'); });
    if (accountBtn) accountBtn.addEventListener('click', function () { Rana.auth.openAccountMenu(); });
    if (notifBtn) notifBtn.addEventListener('click', function () { Rana.auth.openNotificationsModal(); });
  }

  function renderMegaMenu() {
    var root = document.getElementById('mega-menu-root');
    if (!root) return;
    var adminHref = 'admin/index.html';

    var sectionLinks = Rana.SECTIONS.map(function (sec) {
      return '<a href="#/section/' + sec.slug + '" class="mega-menu__section-link" style="color:' + sec.color + '">' + U.escapeHtml(U.sectionTitle(sec.slug, sec.title)) + '</a>';
    }).join('');

    root.innerHTML =
      '<div class="mega-menu no-print" id="mega-menu" aria-hidden="true">' +
        '<div class="mega-menu__sheet">' +
          '<div class="mega-menu__container">' +
            '<div class="mega-menu__top">' +
              '<button type="button" class="mega-menu__close" id="mega-menu-close">' +
                '<span style="font-size:20px;">×</span> ' + U.escapeHtml(Rana.t('close')) +
              '</button>' +
              '<div class="mega-menu__socials">' + ICONS.socialLinksHtml(19) + '</div>' +
            '</div>' +
            '<div class="mega-menu__grid">' +
              '<div class="mega-menu__sections">' + sectionLinks + '</div>' +
              '<div class="mega-menu__pages">' +
                '<a href="#/essays" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pageEssays')) + '</a>' +
                '<a href="#/videos" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pageVideos')) + '</a>' +
                '<div class="mega-menu__page-spacer"></div>' +
                '<a href="#/popular" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pagePopular')) + '</a>' +
                '<a href="#/conversations" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pageConversations')) + '</a>' +
                '<div class="mega-menu__page-spacer"></div>' +
                '<a href="#/pitch" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pageWriteForUs')) + '</a>' +
                '<a href="#/syndication" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pageRepublish')) + '</a>' +
                '<a href="#/page/about" class="mega-menu__page-link">' + U.escapeHtml(Rana.t('pageAbout')) + '</a>' +
              '</div>' +
            '</div>' +
            '<div class="mega-menu__bottom">' +
              '<div class="mega-menu__utility-links">' +
                '<a href="#/newsletter">' + U.escapeHtml(Rana.t('navNewsletter')) + '</a>' +
                '<a href="#/library">' + U.escapeHtml(Rana.t('libTitle')) + '</a>' +
                '<a href="#/page/community-guidelines">' + U.escapeHtml(Rana.t('pageGuidelines')) + '</a>' +
                '<a href="' + adminHref + '">' + U.escapeHtml(Rana.t('adminStudio')) + '</a>' +
              '</div>' +
              '<form class="mega-menu__search-form" id="mega-search-form">' +
                '<input type="search" id="mega-search-input" placeholder="' + U.escapeHtml(Rana.t('searchPh')) + '" />' +
                '<button type="submit" aria-label="Search">' + ICONS.search(16) + '</button>' +
              '</form>' +
            '</div>' +
          '</div>' +
        '</div>' +
      '</div>';

    var mm = document.getElementById('mega-menu');
    document.getElementById('mega-menu-close').addEventListener('click', function () { toggleMegaMenu(false); });
    mm.addEventListener('click', function (e) {
      if (e.target === mm) toggleMegaMenu(false);
    });
    mm.querySelectorAll('a[href^="#"]').forEach(function (a) {
      a.addEventListener('click', function () { toggleMegaMenu(false); });
    });
    document.getElementById('mega-search-form').addEventListener('submit', function (e) {
      e.preventDefault();
      var q = document.getElementById('mega-search-input').value.trim();
      toggleMegaMenu(false);
      global.location.hash = '#/search?q=' + encodeURIComponent(q);
    });
  }

  function toggleMegaMenu(open) {
    var mm = document.getElementById('mega-menu');
    if (!mm) return;
    mm.classList.toggle('is-open', !!open);
    mm.setAttribute('aria-hidden', open ? 'false' : 'true');
  }

  function renderSearchOverlay() {
    var root = document.getElementById('search-overlay-root');
    if (!root) return;
    root.innerHTML =
      '<div class="search-overlay no-print" id="search-overlay">' +
        '<div class="search-overlay__box">' +
          '<button type="button" class="search-overlay__close" id="search-overlay-close"><span style="font-size:20px;">×</span> ' + U.escapeHtml(Rana.t('close')) + '</button>' +
          '<form id="overlay-search-form" class="search-overlay__input-row">' +
            '<input type="search" id="overlay-search-input" placeholder="' + U.escapeHtml(Rana.t('searchOverlayPh')) + '" autocomplete="off" />' +
            '<button type="submit" aria-label="Submit search">' + ICONS.search(26) + '</button>' +
          '</form>' +
          '<p style="margin-top:16px;font-family:var(--font-mono);font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('searchOverlayHint')) + '</p>' +
        '</div>' +
      '</div>';

    document.getElementById('search-overlay-close').addEventListener('click', function () { toggleSearchOverlay(false); });
    document.getElementById('overlay-search-form').addEventListener('submit', function (e) {
      e.preventDefault();
      var q = document.getElementById('overlay-search-input').value.trim();
      toggleSearchOverlay(false);
      global.location.hash = '#/search?q=' + encodeURIComponent(q);
    });
  }

  function toggleSearchOverlay(open) {
    var el = document.getElementById('search-overlay');
    if (!el) return;
    el.classList.toggle('is-open', !!open);
    if (open) {
      var input = document.getElementById('overlay-search-input');
      if (input) setTimeout(function () { input.focus(); }, 60);
    }
  }

  function renderFooter() {
    var root = document.getElementById('site-footer');
    if (!root) return;
    root.className = 'site-footer no-print';
    var adminHref = 'admin/index.html';

    root.innerHTML =
      '<div class="site-footer__inner">' +
        '<div class="site-footer__cols">' +
          '<nav class="site-footer__nav">' +
            '<a href="#/essays">' + U.escapeHtml(Rana.t('pageEssays')) + '</a>' +
            '<a href="#/videos">' + U.escapeHtml(Rana.t('pageVideos')) + '</a>' +
            '<a href="#/popular">' + U.escapeHtml(Rana.t('pagePopular')) + '</a>' +
            '<a href="#/conversations">' + U.escapeHtml(Rana.t('pageConversations')) + '</a>' +
          '</nav>' +
          '<nav class="site-footer__nav">' +
            '<a href="#/page/about">' + U.escapeHtml(Rana.t('pageAbout')) + '</a>' +
            '<a href="#/pitch">' + U.escapeHtml(Rana.t('pageWriteForUs')) + '</a>' +
            '<a href="#/syndication">' + U.escapeHtml(Rana.t('pageRepublish')) + '</a>' +
            '<a href="#/page/contact">' + U.escapeHtml(Rana.t('pageContact')) + '</a>' +
            '<a href="#/page/support">' + U.escapeHtml(Rana.t('pageSupport')) + '</a>' +
          '</nav>' +
          '<nav class="site-footer__nav">' +
            '<a href="#/page/community-guidelines">' + U.escapeHtml(Rana.t('pageGuidelines')) + '</a>' +
          '</nav>' +
          '<div>' +
            '<div style="color:var(--ink-black);font-weight:500;">' + U.escapeHtml(Rana.t('followUs')) + '</div>' +
            '<div class="site-footer__social-row">' + ICONS.socialLinksHtml(20) + '</div>' +
          '</div>' +
        '</div>' +
        '<div class="site-footer__newsletter">' +
          '<form id="footer-newsletter-form">' +
            '<p class="site-footer__nl-title">' + U.escapeHtml(Rana.t('nlSignupTitle')) + '</p>' +
            '<p style="margin:0 0 14px;">' + U.escapeHtml(Rana.t('nlSignupSub')) + '</p>' +
            '<input type="email" name="email" required class="field-input" placeholder="' + U.escapeHtml(Rana.t('nlEmailPh')) + '" />' +
            '<input type="text" name="website" tabindex="-1" autocomplete="off" style="position:absolute;left:-9999px;opacity:0;" aria-hidden="true" />' +
            '<div class="site-footer__nl-checks">' +
              '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="daily" checked /></span><span>' + U.escapeHtml(Rana.t('nlDaily')) + '</span></label>' +
              '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="weekly" /></span><span>' + U.escapeHtml(Rana.t('nlWeekly')) + '</span></label>' +
            '</div>' +
            '<button type="submit" class="aeon-btn"><span>' + U.escapeHtml(Rana.t('nlSubscribe')) + '</span>' + ICONS.arrowRight() + '</button>' +
            '<div style="margin-top:16px;font-size:12px;"><a href="#/page/newsletter-privacy" style="text-decoration:underline;">' + U.escapeHtml(Rana.t('nlPrivacyLink')) + '</a></div>' +
          '</form>' +
        '</div>' +
      '</div>' +
      '<div class="site-footer__legal">' +
        '<p>' + U.escapeHtml(Rana.t('footerCopyright')) + ' <a href="#/page/privacy"><u>' + U.escapeHtml(Rana.t('privacyPolicy')) + '</u></a>. <a href="#/page/terms-of-use"><u>' + U.escapeHtml(Rana.t('termsOfUse')) + '</u></a>.</p>' +
      '</div>';

    var nlForm = document.getElementById('footer-newsletter-form');
    if (nlForm) {
      nlForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        var fd = new FormData(nlForm);
        var email = String(fd.get('email') || '').trim();
        var daily = !!nlForm.querySelector('input[name="daily"]').checked;
        var weekly = !!nlForm.querySelector('input[name="weekly"]').checked;
        var website = String(fd.get('website') || '');
        if (!daily && !weekly) {
          toast(Rana.t('nlChooseFreq'), 'warn');
          return;
        }
        try {
          await Rana.api.subscribeNewsletter({
            email: email, daily: daily, weekly: weekly,
            source: 'newsletter-signup-footer', signupId: 'footer', website: website
          });
          nlForm.reset();
          toast(Rana.t('nlConfirmSent'), 'success');
        } catch (err) {
          toast(err.isNetworkError ? Rana.t('nlPreviewSaved') : err.message, err.isNetworkError ? 'success' : 'warn');
        }
      });
    }
  }

  function renderArticleCard(article, opts) {
    opts = opts || {};
    var compact = !!opts.compact;
    var isVideo = String(article.type).toLowerCase() === 'video';
    var typeLabel = U.typeLabel(article.type);
    var secCls = U.sectionClass(article.section);
    var topicTitle = (article.primaryTopic && article.primaryTopic.title) || (article.section && U.sectionTitle(article.section.slug, article.section.title)) || 'Ideas';
    var imgUrl = U.resolveMediaUrl(article.cardImageUrl || article.featuredImageUrl);
    var alt = article.cardImageAlt || article.title || '';
    var saved = Rana.state.savedIds.has(article.id) || Rana.state.savedSlugs.has(article.slug);
    var href = U.articleUrl(article);

    var authorsText = '';
    if (isVideo) {
      authorsText = article.creditsShort || (article.videoDurationMinutes ? Rana.t('cardMinutes', { n: article.videoDurationMinutes }) : Rana.t('cardWatch'));
    } else if (Array.isArray(article.authors) && article.authors.length) {
      authorsText = article.authors.map(function (a) { return a.name; }).join(' & ');
    }

    return '' +
      '<article class="aeon-card ' + secCls + (compact ? ' aeon-card--compact' : '') + '" data-article-href="' + href + '">' +
        '<a href="' + href + '" class="aeon-card__media">' +
          (article.commentsEnabled ? '<div class="aeon-card__comment-badge" title="' + U.escapeHtml(Rana.t('commentsTitle')) + '">' + ICONS.commentsBubble(15) + '</div>' : '') +
          (isVideo ? '<div class="aeon-card__play-badge">' + ICONS.playCircle(compact ? 42 : 56) + '</div>' : '') +
          '<img class="aeon-card__img" src="' + U.escapeHtml(imgUrl) + '" alt="' + U.escapeHtml(alt) + '" loading="lazy" onerror="this.onerror=null;this.src=\'assets/images/sample-cosmos.svg\';" />' +
          '<div class="aeon-card__tint"></div>' +
        '</a>' +
        '<div class="aeon-card__body">' +
          '<div class="aeon-card__meta">' +
            '<button type="button" class="aeon-card__save-btn' + (saved ? ' is-saved' : '') + '" data-save-slug="' + U.escapeHtml(article.slug) + '" data-save-id="' + U.escapeHtml(article.id || '') + '" aria-label="Save article">' +
              ICONS.bookmark(saved, compact ? 16 : 19) +
              '<span class="aeon-card__save-label">' + U.escapeHtml(saved ? Rana.t('cardSaved') : Rana.t('cardSave')) + '</span>' +
            '</button>' +
            '<p class="aeon-card__meta-text" style="margin:0;">' +
              '<span class="aeon-card__type-prefix">' + U.escapeHtml(typeLabel) + '</span>' +
              '<span class="aeon-card__topic">' + U.escapeHtml(topicTitle) + '</span>' +
            '</p>' +
          '</div>' +
          '<a href="' + href + '">' +
            '<h3 class="aeon-card__title">' + U.escapeHtml(article.title) + '</h3>' +
          '</a>' +
          (!isVideo && article.standfirstShort
            ? '<a href="' + href + '"><p class="aeon-card__standfirst">' + U.escapeHtml(article.standfirstShort) + '</p></a>'
            : '') +
          (authorsText ? '<p class="aeon-card__author">' + U.escapeHtml(authorsText) + '</p>' : '') +
        '</div>' +
      '</article>';
  }

  function bindCardInteractions(container) {
    if (!container) return;
    container.querySelectorAll('[data-save-slug]').forEach(function (btn) {
      btn.addEventListener('click', async function (e) {
        e.preventDefault();
        e.stopPropagation();
        var slug = btn.getAttribute('data-save-slug');
        var id = btn.getAttribute('data-save-id');
        if (!Rana.state.user && !Rana.api.getAccessToken()) {
          Rana.auth.openAuthModal('login');
          toast(Rana.t('loginToSave'));
          return;
        }
        var isSaved = btn.classList.contains('is-saved');
        try {
          if (isSaved) {
            await Rana.api.unsaveArticle(slug);
            if (id) Rana.state.savedIds.delete(id);
            Rana.state.savedSlugs.delete(slug);
            btn.classList.remove('is-saved');
            btn.innerHTML = ICONS.bookmark(false, 19) + '<span class="aeon-card__save-label">' + U.escapeHtml(Rana.t('cardSave')) + '</span>';
            toast(Rana.t('removedFromLib'));
          } else {
            await Rana.api.saveArticle(slug);
            if (id) Rana.state.savedIds.add(id);
            Rana.state.savedSlugs.add(slug);
            btn.classList.add('is-saved');
            btn.innerHTML = ICONS.bookmark(true, 19) + '<span class="aeon-card__save-label">' + U.escapeHtml(Rana.t('cardSaved')) + '</span>';
            toast(Rana.t('savedToLib'), 'success');
          }
        } catch (err) {
          toast(err.message || Rana.t('opFailed'), 'warn');
        }
      });
    });
  }

  Rana.ui = {
    ICONS: ICONS,
    toast: toast,
    openModal: openModal,
    closeModal: closeModal,
    confirmModal: confirmModal,
    promptModal: promptModal,
    renderOfflineBanner: renderOfflineBanner,
    renderHeader: renderHeader,
    renderMegaMenu: renderMegaMenu,
    toggleMegaMenu: toggleMegaMenu,
    renderSearchOverlay: renderSearchOverlay,
    toggleSearchOverlay: toggleSearchOverlay,
    renderFooter: renderFooter,
    renderArticleCard: renderArticleCard,
    bindCardInteractions: bindCardInteractions
  };
})(window);
