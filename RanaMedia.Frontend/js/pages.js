/* ==========================================================================
   Rana Media Public Editorial Pages (Home, Section, Topic, Essay, Video,
   Author, Search, Library, Newsletter, Pitch, Syndication, Static, Comments)
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});
  var U = Rana.utils;
  var UI = Rana.ui;

  var engagedTimer = null;
  var audioTimer = null;

  function cleanupPageTimers() {
    if (engagedTimer) { clearTimeout(engagedTimer); engagedTimer = null; }
    if (audioTimer) { clearInterval(audioTimer); audioTimer = null; }
    var pop = document.getElementById('pagemark-popover');
    if (pop) pop.remove();
  }

  function mainEl() { return document.getElementById('app-main'); }

  function setMainClass(cls) {
    var m = mainEl();
    if (m) m.className = 'flex-1 ' + (cls || '');
  }

  function showLoading() {
    var m = mainEl();
    if (m) m.innerHTML = '<div style="max-width:1200px;margin:72px auto;padding:24px;text-align:center;font-family:var(--font-mono);font-size:13px;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('loading')) + '</div>';
  }

  function handleFallback(err) {
    if (err && err.isNetworkError) {
      Rana.state.offlineMode = true;
      UI.renderOfflineBanner();
      return true;
    }
    return false;
  }

  function renderHeroFeatured(featured) {
    if (!featured) return '';
    var s = featured.settings || {};
    var alignX = (s.alignX || 'center').toLowerCase();
    var alignY = (s.alignY || 'center').toLowerCase();
    var strength = typeof s.backdropStrength === 'number' ? Math.min(1, Math.max(0.05, s.backdropStrength / 5)) : 0.4;
    var secCls = U.sectionClass(featured.section);
    var href = U.articleUrl(featured);
    var imgUrl = U.resolveMediaUrl(featured.featuredImageUrl || featured.cardImageUrl);

    return '' +
      '<section class="hero-featured ' + secCls + '">' +
        '<a href="' + href + '" class="hero-featured__desktop">' +
          '<div class="hero-featured__media">' +
            '<img src="' + U.escapeHtml(imgUrl) + '" alt="' + U.escapeHtml(featured.featuredImageAlt || featured.title) + '" />' +
            '<div class="hero-featured__grid-overlay">' +
              '<div class="hero-featured__text-slot hero-align-x-' + alignX + ' hero-align-y-' + alignY + '" style="--backdrop-strength:' + strength + '">' +
                (featured.primaryTopic ? '<div class="hero-featured__kicker">' + U.escapeHtml(featured.primaryTopic.title) + '</div>' : '') +
                '<h1 class="hero-featured__title">' + U.escapeHtml(featured.title) + '</h1>' +
                '<h2 class="hero-featured__standfirst">' + U.escapeHtml(featured.standfirstLong || featured.standfirstShort || '') + '</h2>' +
              '</div>' +
            '</div>' +
          '</div>' +
          (featured.featuredImageCaptionHtml
            ? '<div class="hero-featured__caption-row"><small class="hero-featured__caption">' + featured.featuredImageCaptionHtml + '</small></div>'
            : '') +
        '</a>' +
        '<div class="hero-featured__mobile">' + UI.renderArticleCard(featured) + '</div>' +
      '</section>';
  }

  function attachLoadMore(btnId, gridEl, initialCursor, fetcher) {
    var cursor = initialCursor;
    var btn = document.getElementById(btnId);
    if (!btn) return;
    if (!cursor) { btn.parentElement.classList.add('hidden'); return; }
    btn.addEventListener('click', async function () {
      btn.disabled = true;
      btn.textContent = Rana.t('loadingUpper');
      try {
        var page = await fetcher(cursor);
        var nodes = page.nodes || [];
        gridEl.insertAdjacentHTML('beforeend', nodes.map(function (a) { return UI.renderArticleCard(a); }).join(''));
        UI.bindCardInteractions(gridEl);
        cursor = page.hasNextPage ? page.endCursor : null;
        if (!cursor) btn.parentElement.classList.add('hidden');
        else { btn.disabled = false; btn.textContent = Rana.t('moreBtn'); }
      } catch (err) {
        UI.toast(err.message || Rana.t('opFailed'), 'warn');
        btn.disabled = false;
        btn.textContent = Rana.t('moreBtn');
      }
    });
  }

  /* ---------------------- HOME PAGE ---------------------- */
  async function renderHome() {
    cleanupPageTimers();
    showLoading();
    U.setDocumentTitle(null);

    var data;
    try {
      data = await Rana.api.getHome();
    } catch (err) {
      if (handleFallback(err)) data = Rana.demo.home;
      else { mainEl().innerHTML = '<div class="editorial-page"><p>' + U.escapeHtml(err.message) + '</p></div>'; return; }
    }

    UI.renderOfflineBanner();
    var featSec = (data.featuredEssay && data.featuredEssay.section && data.featuredEssay.section.slug) || 'society';
    UI.renderHeader({ sectionSlug: featSec, overlay: false });
    setMainClass(U.sectionClass(featSec));

    var nodes = (data.articles && data.articles.nodes) || [];
    var topGridNodes = nodes.slice(0, 6);
    var restGridNodes = nodes.slice(6);
    var popularNodes = data.popularThisMonth || [];

    var html =
      renderHeroFeatured(data.featuredEssay) +
      '<div class="editorial-container">' +
        '<div class="articles-grid" id="home-top-grid">' +
          topGridNodes.map(function (a) { return UI.renderArticleCard(a); }).join('') +
        '</div>' +
        (popularNodes.length
          ? '<section class="popular-band">' +
              '<div class="popular-band__inner">' +
                '<h2 class="popular-band__heading">' + U.escapeHtml(Rana.t('popularThisMonth')) + '</h2>' +
                '<div class="popular-grid" id="home-popular-grid">' +
                  popularNodes.slice(0, 4).map(function (a) { return UI.renderArticleCard(a, { compact: true }); }).join('') +
                '</div>' +
              '</div>' +
            '</section>'
          : '') +
        '<div class="articles-grid" id="home-more-grid">' +
          restGridNodes.map(function (a) { return UI.renderArticleCard(a); }).join('') +
        '</div>' +
        '<div class="load-more-wrap">' +
          '<button type="button" class="load-more-btn" id="home-load-more">' + U.escapeHtml(Rana.t('moreBtn')) + '</button>' +
        '</div>' +
      '</div>';

    mainEl().innerHTML = html;
    UI.bindCardInteractions(mainEl());
    attachLoadMore(
      'home-load-more',
      document.getElementById('home-more-grid'),
      data.articles && data.articles.hasNextPage ? data.articles.endCursor : null,
      function (c) { return Rana.api.getHomeMore(c, 18); }
    );
  }

  /* ---------------------- COLLECTION / FEED LISTS ---------------------- */
  async function renderCollection(mode, query) {
    cleanupPageTimers();
    showLoading();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');

    var sort = (query && query.sort) || (mode === 'essays-popular' || mode === 'popular' ? 'popular' : 'latest');
    var videoFilter = (query && query.filter) || '';
    var titleMap = {
      'essays': Rana.t('pageEssays'),
      'essays-popular': Rana.t('pagePopularEssays'),
      'videos': Rana.t('pageVideos'),
      'popular': Rana.t('pagePopular'),
      'conversations': Rana.t('pageConversations')
    };
    U.setDocumentTitle(titleMap[mode] || 'Articles');

    var result;
    try {
      if (mode === 'essays' || mode === 'essays-popular') {
        result = await Rana.api.getEssays(sort, null, 12);
      } else if (mode === 'videos') {
        result = await Rana.api.getVideos(videoFilter || null, null, 12);
      } else if (mode === 'popular') {
        result = await Rana.api.getPopular(null, 24);
      } else if (mode === 'conversations') {
        var list = await Rana.api.getConversations(24);
        result = { nodes: list || [], hasNextPage: false };
      }
    } catch (err) {
      if (handleFallback(err)) {
        var pool = Rana.demo.articles.slice();
        if (mode.indexOf('essay') === 0) pool = pool.filter(function (a) { return a.type === 'Essay'; });
        else if (mode === 'videos') pool = pool.filter(function (a) { return a.type === 'Video'; });
        result = { nodes: pool, hasNextPage: false };
      } else {
        mainEl().innerHTML = '<div class="editorial-page"><p>' + U.escapeHtml(err.message) + '</p></div>';
        return;
      }
    }

    UI.renderOfflineBanner();
    var tabsHtml = '';
    if (mode === 'essays' || mode === 'essays-popular') {
      tabsHtml =
        '<div class="sort-tabs">' +
          '<a href="#/essays" class="sort-tabs__btn' + (sort === 'latest' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortLatest')) + '</a>' +
          '<a href="#/essays/popular" class="sort-tabs__btn' + (sort === 'popular' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortPopular')) + '</a>' +
        '</div>';
    } else if (mode === 'videos') {
      tabsHtml =
        '<div class="sort-tabs">' +
          '<a href="#/videos" class="sort-tabs__btn' + (!videoFilter ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortLatest')) + '</a>' +
          '<a href="#/videos?filter=popular" class="sort-tabs__btn' + (videoFilter === 'popular' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortPopular')) + '</a>' +
          '<a href="#/videos?filter=originals" class="sort-tabs__btn' + (videoFilter === 'originals' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortOriginals')) + '</a>' +
          '<a href="#/videos?filter=exclusives" class="sort-tabs__btn' + (videoFilter === 'exclusives' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortExclusives')) + '</a>' +
        '</div>';
    }

    var nodes = result.nodes || [];
    mainEl().innerHTML =
      '<div class="section-header-band">' +
        '<h1 class="section-header-band__title">' + U.escapeHtml(titleMap[mode] || 'Articles') + '</h1>' +
        tabsHtml +
      '</div>' +
      '<div class="editorial-container">' +
        '<div class="articles-grid" id="collection-grid">' +
          (nodes.length ? nodes.map(function (a) { return UI.renderArticleCard(a); }).join('') : '<p style="grid-column:1/-1;text-align:center;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('emptyCollection')) + '</p>') +
        '</div>' +
        '<div class="load-more-wrap"><button type="button" class="load-more-btn" id="col-load-more">' + U.escapeHtml(Rana.t('moreBtn')) + '</button></div>' +
      '</div>';

    UI.bindCardInteractions(mainEl());
    attachLoadMore('col-load-more', document.getElementById('collection-grid'), result.hasNextPage ? result.endCursor : null, function (c) {
      if (mode === 'videos') return Rana.api.getVideos(videoFilter || null, c, 12);
      if (mode === 'popular') return Rana.api.getPopular(c, 24);
      return Rana.api.getEssays(sort, c, 12);
    });
  }

  /* ---------------------- SECTION & TOPIC PAGES ---------------------- */
  async function renderSection(sectionSlug, topicSlug, query) {
    cleanupPageTimers();
    showLoading();
    var sort = (query && query.sort) || 'latest';
    var secCls = U.sectionClass(sectionSlug);
    UI.renderHeader({ sectionSlug: sectionSlug });
    setMainClass(secCls);

    var detail;
    try {
      detail = topicSlug
        ? await Rana.api.getTopicBySlug(sectionSlug, topicSlug, sort, null, 24)
        : await Rana.api.getSectionBySlug(sectionSlug, sort, null, 24);
    } catch (err) {
      if (handleFallback(err)) {
        var secDemo = Rana.demo.sections.find(function (s) { return s.slug === sectionSlug; }) || Rana.demo.sections[0];
        var topicDemo = topicSlug ? (secDemo.topics.find(function (t) { return t.slug === topicSlug; }) || secDemo.topics[0]) : null;
        var filtered = Rana.demo.articles.filter(function (a) {
          var sOk = a.section && a.section.slug === secDemo.slug;
          var tOk = !topicDemo || (a.primaryTopic && a.primaryTopic.slug === topicDemo.slug);
          return sOk && tOk;
        });
        detail = topicDemo
          ? { title: topicDemo.title, description: 'Essays and videos in ' + topicDemo.title + '.', section: secDemo, siblingTopics: secDemo.topics, articles: { nodes: filtered, hasNextPage: false } }
          : { title: secDemo.title, slug: secDemo.slug, description: secDemo.description, topics: secDemo.topics, articles: { nodes: filtered, hasNextPage: false } };
      } else {
        mainEl().innerHTML = '<div class="editorial-page"><p>' + U.escapeHtml(err.message) + '</p></div>';
        return;
      }
    }

    UI.renderOfflineBanner();
    var displaySecTitle = detail.slug ? U.sectionTitle(detail.slug, detail.title) : detail.title;
    U.setDocumentTitle(displaySecTitle);
    var topics = detail.topics || detail.siblingTopics || [];
    var basePath = topicSlug ? '#/section/' + sectionSlug + '/' + topicSlug : '#/section/' + sectionSlug;

    var html =
      '<div class="section-header-band">' +
        (topicSlug && detail.section
          ? '<div style="font-family:var(--font-mono);font-size:12px;margin-bottom:10px;"><a href="#/section/' + detail.section.slug + '" style="color:var(--section-color);text-decoration:underline;">' + U.escapeHtml(U.sectionTitle(detail.section.slug, detail.section.title)) + '</a></div>'
          : '') +
        '<h1 class="section-header-band__title">' + U.escapeHtml(displaySecTitle) + '</h1>' +
        (detail.description ? '<h2 class="section-header-band__desc">' + U.escapeHtml(detail.description) + '</h2>' : '') +
        (topics.length
          ? '<div class="topic-cloud">' +
              topics.map(function (t) {
                var active = topicSlug === t.slug ? ' is-active' : '';
                return '<a href="#/section/' + sectionSlug + '/' + t.slug + '" class="topic-cloud__item' + active + '">' + U.escapeHtml(t.title) + '</a>';
              }).join('') +
            '</div>'
          : '') +
        '<div class="sort-tabs">' +
          '<a href="' + basePath + '?sort=latest" class="sort-tabs__btn' + (sort === 'latest' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortLatest')) + '</a>' +
          '<a href="' + basePath + '?sort=popular" class="sort-tabs__btn' + (sort === 'popular' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('sortPopular')) + '</a>' +
        '</div>' +
      '</div>' +
      '<div class="editorial-container">' +
        '<div class="articles-grid" id="section-grid">' +
          ((detail.articles && detail.articles.nodes && detail.articles.nodes.length)
            ? detail.articles.nodes.map(function (a) { return UI.renderArticleCard(a); }).join('')
            : '<p style="grid-column:1/-1;text-align:center;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('emptySection')) + '</p>') +
        '</div>' +
        '<div class="load-more-wrap"><button type="button" class="load-more-btn" id="sec-load-more">' + U.escapeHtml(Rana.t('moreBtn')) + '</button></div>' +
      '</div>';

    mainEl().innerHTML = html;
    UI.bindCardInteractions(mainEl());
    attachLoadMore('sec-load-more', document.getElementById('section-grid'), detail.articles && detail.articles.hasNextPage ? detail.articles.endCursor : null, function (c) {
      return topicSlug
        ? Rana.api.getTopicBySlug(sectionSlug, topicSlug, sort, c, 24).then(function (r) { return r.articles; })
        : Rana.api.getSectionBySlug(sectionSlug, sort, c, 24).then(function (r) { return r.articles; });
    });
  }

  /* ---------------------- COMMENTS THREAD MODULE ---------------------- */
  async function mountCommentsSection(container, article) {
    if (!container) return;
    var currentSort = 'newest';
    var state = { comments: [], hasNext: false, cursor: null };

    async function loadComments(append) {
      try {
        var page = await Rana.api.getArticleComments(article.slug, currentSort, append ? state.cursor : null, 20);
        state.comments = append ? state.comments.concat(page.nodes || []) : (page.nodes || []);
        state.hasNext = !!page.hasNextPage;
        state.cursor = page.endCursor;
      } catch (err) {
        if (!append && err.isNetworkError) {
          state.comments = [];
          state.hasNext = false;
        }
      }
      render();
    }

    function minutesRemaining(editableUntil) {
      if (!editableUntil) return 0;
      var diff = new Date(editableUntil).getTime() - Date.now();
      return Math.max(0, Math.ceil(diff / 60000));
    }

    function renderCommentItem(c) {
      var user = Rana.state.user;
      var isOwner = user && user.id === c.user.id;
      var isStaff = Rana.auth.hasStaffRole();
      var rem = minutesRemaining(c.editableUntil);
      var canEdit = isOwner && rem > 0;

      var repliesHtml = (c.replies || []).map(function (r) {
        var rOwner = user && user.id === r.user.id;
        var rRem = minutesRemaining(r.editableUntil);
        var rCanEdit = rOwner && rRem > 0;
        return '' +
          '<div class="comment-card" style="padding:14px 0;">' +
            '<div class="comment-card__top">' +
              '<span class="comment-user">' +
                '<span class="comment-avatar">' + U.escapeHtml(U.initials(r.user.name)) + '</span>' +
                '<span>' + U.escapeHtml(r.user.name) + '</span>' +
                (r.user.isArticleAuthor ? '<span class="comment-badge">' + U.escapeHtml(Rana.t('cmtAuthorBadge')) + '</span>' : '') +
              '</span>' +
              '<span style="font-family:var(--font-mono);font-size:11px;color:var(--ink-mid);">' + U.formatDateTime(r.createdAt) + '</span>' +
            '</div>' +
            '<div class="comment-body" id="reply-body-' + r.id + '">' + U.escapeHtml(r.bodyHtml) + '</div>' +
            '<div class="comment-actions">' +
              '<button type="button" data-like-reply="' + r.id + '">' + U.escapeHtml(Rana.t('cmtLike')) + ' (' + (r.likesCount || 0) + ')</button>' +
              '<button type="button" data-report-reply="' + r.id + '">' + U.escapeHtml(Rana.t('cmtReport')) + '</button>' +
              (rCanEdit ? '<button type="button" data-edit-reply="' + r.id + '">' + U.escapeHtml(Rana.t('cmtEditLeft', { m: rRem })) + '</button>' : '') +
              ((rOwner || isStaff) ? '<button type="button" data-del-reply="' + r.id + '">' + U.escapeHtml(Rana.t('cmtDelete')) + '</button>' : '') +
            '</div>' +
            '<div id="edit-reply-slot-' + r.id + '"></div>' +
          '</div>';
      }).join('');

      return '' +
        '<div class="comment-card' + (c.isFeatured ? ' is-featured' : '') + '" id="comment-' + c.id + '">' +
          '<div class="comment-card__top">' +
            '<span class="comment-user">' +
              '<span class="comment-avatar">' + U.escapeHtml(U.initials(c.user.name)) + '</span>' +
              '<span>' + U.escapeHtml(c.user.name) + '</span>' +
              (c.user.isArticleAuthor ? '<span class="comment-badge">' + U.escapeHtml(Rana.t('cmtAuthorBadge')) + '</span>' : '') +
              (c.isFeatured ? '<span class="comment-badge" style="background:var(--aeon-red);color:#fff;">' + U.escapeHtml(Rana.t('cmtFeaturedBadge')) + '</span>' : '') +
            '</span>' +
            '<span style="font-family:var(--font-mono);font-size:11px;color:var(--ink-mid);">' + U.formatDateTime(c.createdAt) + '</span>' +
          '</div>' +
          '<div class="comment-body" id="comment-body-' + c.id + '">' + U.escapeHtml(c.bodyHtml) + '</div>' +
          '<div class="comment-actions">' +
            '<button type="button" data-like-comment="' + c.id + '">' + U.escapeHtml(Rana.t('cmtLike')) + ' (' + (c.likesCount || 0) + ')</button>' +
            '<button type="button" data-reply-toggle="' + c.id + '">' + U.escapeHtml(Rana.t('cmtReply')) + '</button>' +
            '<button type="button" data-report-comment="' + c.id + '">' + U.escapeHtml(Rana.t('cmtReport')) + '</button>' +
            (canEdit ? '<button type="button" data-edit-comment="' + c.id + '">' + U.escapeHtml(Rana.t('cmtEditLeft', { m: rem })) + '</button>' : '') +
            ((isOwner || isStaff) ? '<button type="button" data-del-comment="' + c.id + '">' + U.escapeHtml(Rana.t('cmtDelete')) + '</button>' : '') +
          '</div>' +
          '<div id="edit-comment-slot-' + c.id + '"></div>' +
          '<div id="reply-box-' + c.id + '"></div>' +
          (repliesHtml ? '<div class="comment-replies">' + repliesHtml + '</div>' : '') +
        '</div>';
    }

    function render() {
      var user = Rana.state.user;
      var myRoot = user ? state.comments.find(function (c) { return c.user && c.user.id === user.id; }) : null;
      var guidelinesAccepted = user && !!user.communityGuidelinesAcceptedAt;

      var composerHtml = '';
      if (!article.commentsEnabled || !article.commentsOpen) {
        composerHtml = '<p style="color:var(--ink-mid);font-family:var(--font-mono);font-size:13px;">' + U.escapeHtml(Rana.t('cmtClosed')) + '</p>';
      } else if (!user) {
        composerHtml =
          '<div class="guidelines-card">' +
            '<p style="margin:0 0 12px;">' + U.escapeHtml(Rana.t('cmtLoginPrompt')) + '</p>' +
            '<button type="button" class="aeon-btn" id="cmt-signin-btn">' + U.escapeHtml(Rana.t('cmtLoginBtn')) + '</button>' +
          '</div>';
      } else if (!guidelinesAccepted) {
        composerHtml =
          '<div class="guidelines-card">' +
            '<h4 class="font-serif" style="margin:0 0 8px;font-size:22px;">' + U.escapeHtml(Rana.t('cmtGlTitle')) + '</h4>' +
            '<p style="margin:0 0 12px;font-size:14.5px;line-height:1.55;">' + U.escapeHtml(Rana.t('cmtGlDesc')) + '</p>' +
            '<div style="display:flex;flex-wrap:wrap;gap:12px;align-items:center;">' +
              '<button type="button" class="aeon-btn" id="cmt-accept-gl-btn">' + U.escapeHtml(Rana.t('cmtGlAccept')) + '</button>' +
              '<a href="#/page/community-guidelines" style="font-family:var(--font-mono);font-size:12px;text-decoration:underline;">' + U.escapeHtml(Rana.t('cmtGlReadFull')) + '</a>' +
            '</div>' +
          '</div>';
      } else if (myRoot) {
        composerHtml =
          '<div class="guidelines-card" style="display:flex;flex-wrap:wrap;align-items:center;justify-content:space-between;gap:12px;">' +
            '<span style="font-size:14.5px;">' + U.escapeHtml(Rana.t('cmtOneLimit')) + '</span>' +
            '<button type="button" class="aeon-btn aeon-btn--secondary" id="jump-my-comment">' + U.escapeHtml(Rana.t('cmtJumpMine')) + '</button>' +
          '</div>';
      } else {
        composerHtml =
          '<form id="new-root-comment-form" class="comment-composer">' +
            '<textarea name="body" required minlength="2" maxlength="5000" placeholder="' + U.escapeHtml(Rana.t('cmtPlaceholder')) + '"></textarea>' +
            '<div style="display:flex;align-items:center;justify-content:space-between;gap:12px;flex-wrap:wrap;">' +
              '<span style="font-family:var(--font-mono);font-size:11.5px;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('cmtRuleHint')) + '</span>' +
              '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('cmtPostBtn')) + '</button>' +
            '</div>' +
          '</form>';
      }

      container.innerHTML =
        '<div class="comments-header">' +
          '<h2>' + U.escapeHtml(Rana.t('commentsTitle')) + ' (' + state.comments.length + ')</h2>' +
          '<div class="sort-tabs" style="margin:0;">' +
            '<button type="button" class="sort-tabs__btn' + (currentSort === 'newest' ? ' is-active' : '') + '" data-cmt-sort="newest">' + U.escapeHtml(Rana.t('sortNewest')) + '</button>' +
            '<button type="button" class="sort-tabs__btn' + (currentSort === 'popular' ? ' is-active' : '') + '" data-cmt-sort="popular">' + U.escapeHtml(Rana.t('sortPopular')) + '</button>' +
          '</div>' +
        '</div>' +
        composerHtml +
        '<div id="comments-list">' +
          (state.comments.length
            ? state.comments.map(renderCommentItem).join('')
            : ((article.commentsEnabled && article.commentsOpen)
                ? '<p style="color:var(--ink-mid);">' + U.escapeHtml(Rana.t('cmtEmpty')) + '</p>'
                : '')) +
        '</div>' +
        (state.hasNext ? '<div style="margin-top:24px;"><button type="button" class="load-more-btn" id="cmt-load-more">' + U.escapeHtml(Rana.t('cmtMore')) + '</button></div>' : '');

      // Bind interactions
      var signinBtn = document.getElementById('cmt-signin-btn');
      if (signinBtn) signinBtn.addEventListener('click', function () { Rana.auth.openAuthModal('login', function () { loadComments(false); }); });

      var glBtn = document.getElementById('cmt-accept-gl-btn');
      if (glBtn) glBtn.addEventListener('click', async function () {
        if (await Rana.auth.acceptGuidelines()) render();
      });

      var jumpBtn = document.getElementById('jump-my-comment');
      if (jumpBtn && myRoot) {
        jumpBtn.addEventListener('click', function () {
          var el = document.getElementById('comment-' + myRoot.id);
          if (el) el.scrollIntoView({ behavior: 'smooth', block: 'center' });
        });
      }

      var newForm = document.getElementById('new-root-comment-form');
      if (newForm) {
        newForm.addEventListener('submit', async function (e) {
          e.preventDefault();
          var body = e.target.body.value.trim();
          try {
            await Rana.api.createComment(article.slug, body);
            UI.toast(Rana.t('cmtPosted'), 'success');
            await loadComments(false);
          } catch (err) {
            UI.toast(err.message || Rana.t('cmtPostErr'), 'warn');
          }
        });
      }

      container.querySelectorAll('[data-cmt-sort]').forEach(function (b) {
        b.addEventListener('click', function () {
          currentSort = b.getAttribute('data-cmt-sort');
          loadComments(false);
        });
      });

      var moreBtn = document.getElementById('cmt-load-more');
      if (moreBtn) moreBtn.addEventListener('click', function () { loadComments(true); });

      container.querySelectorAll('[data-edit-comment]').forEach(function (btn) {
        btn.addEventListener('click', function () {
          var id = btn.getAttribute('data-edit-comment');
          var slot = document.getElementById('edit-comment-slot-' + id);
          var existing = (document.getElementById('comment-body-' + id) || {}).textContent || '';
          slot.innerHTML =
            '<form class="edit-cmt-form" style="margin-top:12px;">' +
              '<textarea class="field-textarea" style="min-height:80px;" required>' + U.escapeHtml(existing) + '</textarea>' +
              '<div style="display:flex;gap:8px;margin-top:8px;">' +
                '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('cmtSaveChanges')) + '</button>' +
                '<button type="button" class="aeon-btn aeon-btn--ghost cancel-edit">' + U.escapeHtml(Rana.t('cmtCancel')) + '</button>' +
              '</div>' +
            '</form>';
          slot.querySelector('.cancel-edit').addEventListener('click', function () { slot.innerHTML = ''; });
          slot.querySelector('form').addEventListener('submit', async function (e) {
            e.preventDefault();
            try {
              await Rana.api.updateComment(id, slot.querySelector('textarea').value.trim());
              UI.toast(Rana.t('cmtUpdated'), 'success');
              await loadComments(false);
            } catch (err) { UI.toast(err.message, 'warn'); }
          });
        });
      });

      container.querySelectorAll('[data-del-comment]').forEach(function (btn) {
        btn.addEventListener('click', async function () {
          if (!(await UI.confirmModal({ message: Rana.t('cmtDelConfirm'), danger: true }))) return;
          try {
            await Rana.api.deleteComment(btn.getAttribute('data-del-comment'));
            UI.toast(Rana.t('cmtDeleted'));
            await loadComments(false);
          } catch (err) { UI.toast(err.message, 'warn'); }
        });
      });

      container.querySelectorAll('[data-reply-toggle]').forEach(function (btn) {
        btn.addEventListener('click', function () {
          if (!Rana.state.user) return Rana.auth.openAuthModal('login');
          if (!Rana.state.user.communityGuidelinesAcceptedAt) {
            UI.toast(Rana.t('cmtReplyNeedGl'), 'warn');
            return;
          }
          var cid = btn.getAttribute('data-reply-toggle');
          var box = document.getElementById('reply-box-' + cid);
          box.innerHTML =
            '<form style="margin-top:12px;">' +
              '<textarea class="field-textarea" style="min-height:76px;" required placeholder="' + U.escapeHtml(Rana.t('cmtReplyPh')) + '"></textarea>' +
              '<div style="display:flex;gap:8px;margin-top:8px;">' +
                '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('cmtPostReply')) + '</button>' +
                '<button type="button" class="aeon-btn aeon-btn--ghost cancel-rep">' + U.escapeHtml(Rana.t('cmtCancel')) + '</button>' +
              '</div>' +
            '</form>';
          box.querySelector('.cancel-rep').addEventListener('click', function () { box.innerHTML = ''; });
          box.querySelector('form').addEventListener('submit', async function (e) {
            e.preventDefault();
            try {
              await Rana.api.createReply(cid, box.querySelector('textarea').value.trim());
              UI.toast(Rana.t('cmtReplySent'), 'success');
              await loadComments(false);
            } catch (err) { UI.toast(err.message, 'warn'); }
          });
        });
      });

      container.querySelectorAll('[data-edit-reply]').forEach(function (btn) {
        btn.addEventListener('click', function () {
          var id = btn.getAttribute('data-edit-reply');
          var slot = document.getElementById('edit-reply-slot-' + id);
          var existing = (document.getElementById('reply-body-' + id) || {}).textContent || '';
          slot.innerHTML =
            '<form style="margin-top:10px;">' +
              '<textarea class="field-textarea" style="min-height:72px;" required>' + U.escapeHtml(existing) + '</textarea>' +
              '<div style="display:flex;gap:8px;margin-top:8px;">' +
                '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('cmtSaveChanges')) + '</button>' +
                '<button type="button" class="aeon-btn aeon-btn--ghost cancel-er">' + U.escapeHtml(Rana.t('cmtCancel')) + '</button>' +
              '</div>' +
            '</form>';
          slot.querySelector('.cancel-er').addEventListener('click', function () { slot.innerHTML = ''; });
          slot.querySelector('form').addEventListener('submit', async function (e) {
            e.preventDefault();
            try {
              await Rana.api.updateReply(id, slot.querySelector('textarea').value.trim());
              UI.toast(Rana.t('cmtReplyUpdated'), 'success');
              await loadComments(false);
            } catch (err) { UI.toast(err.message, 'warn'); }
          });
        });
      });

      container.querySelectorAll('[data-del-reply]').forEach(function (btn) {
        btn.addEventListener('click', async function () {
          if (!(await UI.confirmModal({ message: Rana.t('cmtReplyDelConfirm'), danger: true }))) return;
          try {
            await Rana.api.deleteReply(btn.getAttribute('data-del-reply'));
            UI.toast(Rana.t('cmtReplyDeleted'));
            await loadComments(false);
          } catch (err) { UI.toast(err.message, 'warn'); }
        });
      });

      container.querySelectorAll('[data-like-comment]').forEach(function (btn) {
        btn.addEventListener('click', async function () {
          if (!Rana.state.user) return Rana.auth.openAuthModal('login');
          try { await Rana.api.likeComment(btn.getAttribute('data-like-comment'), true); await loadComments(false); }
          catch (err) { UI.toast(err.message, 'warn'); }
        });
      });

      container.querySelectorAll('[data-like-reply]').forEach(function (btn) {
        btn.addEventListener('click', async function () {
          if (!Rana.state.user) return Rana.auth.openAuthModal('login');
          try { await Rana.api.likeReply(btn.getAttribute('data-like-reply'), true); await loadComments(false); }
          catch (err) { UI.toast(err.message, 'warn'); }
        });
      });

      function openReport(isReply, id) {
        if (!Rana.state.user) return Rana.auth.openAuthModal('login');
        UI.openModal(
          '<h3 class="font-serif" style="margin:0 0 12px;font-size:24px;">' + U.escapeHtml(Rana.t('cmtReportTitle')) + '</h3>' +
          '<form id="report-cmt-form" class="form-grid">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('cmtReportReason')) + '</label><textarea name="reason" required minlength="3" class="field-textarea" placeholder="' + U.escapeHtml(Rana.t('cmtReportPh')) + '"></textarea></div>' +
            '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('cmtReportSubmit')) + '</button>' +
          '</form>', '440px');
        document.getElementById('report-cmt-form').addEventListener('submit', async function (e) {
          e.preventDefault();
          var r = e.target.reason.value.trim();
          try {
            if (isReply) await Rana.api.reportReply(id, r);
            else await Rana.api.reportComment(id, r);
            UI.closeModal();
            UI.toast(Rana.t('cmtReportSent'), 'success');
          } catch (err) { UI.toast(err.message, 'warn'); }
        });
      }

      container.querySelectorAll('[data-report-comment]').forEach(function (b) {
        b.addEventListener('click', function () { openReport(false, b.getAttribute('data-report-comment')); });
      });
      container.querySelectorAll('[data-report-reply]').forEach(function (b) {
        b.addEventListener('click', function () { openReport(true, b.getAttribute('data-report-reply')); });
      });
    }

    await loadComments(false);
  }

  /* ---------------------- ESSAY READER PAGE ---------------------- */
  async function renderEssay(slug, query) {
    cleanupPageTimers();
    showLoading();

    var article;
    try {
      article = (query && query.preview && query.token)
        ? await Rana.api.getArticlePreview(slug, query.token)
        : await Rana.api.getArticleBySlug(slug);
    } catch (err) {
      if (handleFallback(err)) {
        article = Rana.demo.articles.find(function (a) { return a.slug === slug; }) || Rana.demo.articles[0];
      } else {
        mainEl().innerHTML = '<div class="editorial-page"><h1>' + U.escapeHtml(Rana.t('articleNotFound')) + '</h1><p>' + U.escapeHtml(err.message) + '</p></div>';
        return;
      }
    }

    var secSlug = (article.section && article.section.slug) || 'philosophy';
    UI.renderOfflineBanner();
    UI.renderHeader({ sectionSlug: secSlug, overlay: true });
    setMainClass(U.sectionClass(secSlug));
    U.setDocumentTitle(article.title);

    // Analytics pings
    if (!Rana.state.offlineMode) {
      Rana.api.trackView(article.slug, 'View').catch(function () {});
      engagedTimer = setTimeout(function () {
        Rana.api.trackView(article.slug, 'EngagedView').catch(function () {});
      }, 15000);
    }

    var heroImg = U.resolveMediaUrl(article.featuredImageUrl || article.cardImageUrl);
    var authorsHtml = (article.authors || []).map(function (a) {
      return '<a href="#/author/' + U.escapeHtml(a.slug) + '">' + U.escapeHtml(a.name) + '</a> ' + (a.shortBioHtml || '');
    }).join('<br/>');

    var s = article.settings || {};
    var alignX = (s.alignX || 'center').toLowerCase();
    var alignY = (s.alignY || 'center').toLowerCase();
    var strength = typeof s.backdropStrength === 'number' ? Math.min(1, Math.max(0.08, s.backdropStrength / 5)) : 0.35;
    var isSaved = Rana.state.savedIds.has(article.id) || Rana.state.savedSlugs.has(article.slug);

    mainEl().innerHTML =
      '<div class="essay-hero-wrap">' +
        '<div class="essay-hero">' +
          '<img src="' + U.escapeHtml(heroImg) + '" alt="' + U.escapeHtml(article.featuredImageAlt || article.title) + '" />' +
          '<div class="essay-hero__overlay">' +
            '<div class="hero-featured__text-slot hero-align-x-' + alignX + ' hero-align-y-' + alignY + '" style="--backdrop-strength:' + strength + '">' +
              '<h1 class="hero-featured__title">' + U.escapeHtml(article.title) + '</h1>' +
              '<h2 class="hero-featured__standfirst">' + U.escapeHtml(article.standfirstLong || article.standfirstShort || '') + '</h2>' +
            '</div>' +
          '</div>' +
        '</div>' +
        (article.featuredImageCaptionHtml
          ? '<div class="essay-hero__caption-row"><small class="hero-featured__caption">' + article.featuredImageCaptionHtml + '</small></div>'
          : '') +
      '</div>' +
      '<div class="essay-layout">' +
        '<aside class="essay-aside">' +
          (authorsHtml ? '<div class="essay-aside__bio">' + authorsHtml + '</div>' : '') +
          (article.editor ? '<div class="essay-aside__editor">' + U.escapeHtml(Rana.t('editedBy')) + ' <a href="#/author/' + U.escapeHtml(article.editor.slug) + '">' + U.escapeHtml(article.editor.name) + '</a></div>' : '') +
          '<div class="essay-aside__stats">' +
            '<span>' + U.escapeHtml(Rana.t('wordsCount', { n: (article.wordCount || 1800).toLocaleString() })) + '</span>' +
            (article.publishedAt ? '<span>· ' + U.formatDate(article.publishedAt) + '</span>' : '') +
          '</div>' +
          '<div class="essay-aside__topics">' +
            (article.topics || []).map(function (t) {
              return '<a href="#/section/' + secSlug + '/' + U.escapeHtml(t.slug) + '">' + U.escapeHtml(t.title) + '</a>';
            }).join('') +
          '</div>' +
          (article.republicationAllowed !== false
            ? '<a href="#/syndication?slug=' + encodeURIComponent(article.slug) + '" class="essay-syndicate-btn">' + U.escapeHtml(Rana.t('syndicateEssay')) + '</a>'
            : '') +
          '<div class="essay-sticky-tools no-print">' +
            '<button type="button" class="essay-tool-btn' + (isSaved ? ' is-active' : '') + '" id="essay-save-btn">' +
              UI.ICONS.bookmark(isSaved, 19) + '<span>' + U.escapeHtml(isSaved ? Rana.t('cardSaved') : Rana.t('cardSave')) + '</span>' +
            '</button>' +
            '<button type="button" class="essay-tool-btn" id="essay-share-btn">' +
              UI.ICONS.share(18) + '<span>' + U.escapeHtml(Rana.t('share')) + '</span>' +
            '</button>' +
            '<button type="button" class="essay-tool-btn" id="essay-notes-toggle">' +
              '<span>' + U.escapeHtml(Rana.t('notesCount')) + ' (<span id="pagemarks-count">0</span>)</span>' +
            '</button>' +
            '<button type="button" class="essay-tool-btn" id="essay-jump-comments">' +
              UI.ICONS.commentsBubble(17) + '<span>' + U.escapeHtml(Rana.t('commentsTitle')) + '</span>' +
            '</button>' +
          '</div>' +
        '</aside>' +
        '<div>' +
          (article.audioEnabled
            ? '<div class="audio-bar no-print">' +
                '<button type="button" class="audio-bar__play" id="essay-audio-play" title="Listen">' + UI.ICONS.audioPlay() + '</button>' +
                '<div class="audio-bar__meta">' +
                  '<p class="audio-bar__title">' + U.escapeHtml(Rana.t('listenEssay')) + '</p>' +
                  '<p class="audio-bar__sub">' + U.escapeHtml(Rana.t('listenMinutes', { n: Math.max(1, Math.round((article.audioDurationSeconds || 600) / 60)) })) + '</p>' +
                '</div>' +
                '<div class="audio-bar__scrubber">' +
                  '<span id="audio-cur-time">0:00</span>' +
                  '<input type="range" id="audio-range" min="0" max="100" value="0" />' +
                  '<span id="audio-tot-time">' + Math.max(1, Math.round((article.audioDurationSeconds || 600) / 60)) + ':00</span>' +
                '</div>' +
                (article.audioUrl ? '<audio id="essay-audio-el" src="' + U.escapeHtml(U.resolveMediaUrl(article.audioUrl)) + '"></audio>' : '') +
              '</div>'
            : '') +
          '<article class="article-prose" id="essay-prose-body">' + U.resolveBodyMediaUrls(article.bodyHtml || '') + '</article>' +
          '<div class="annotations-drawer hidden" id="annotations-drawer"></div>' +
          '<section class="comments-section" id="essay-comments-root"></section>' +
        '</div>' +
      '</div>' +
      ((article.related && article.related.length)
        ? '<div class="editorial-container" style="border-top:1px solid var(--border-hairline);padding-top:24px;">' +
            '<div class="articles-grid">' +
              article.related.slice(0, 3).map(function (r) { return UI.renderArticleCard(r); }).join('') +
            '</div>' +
          '</div>'
        : '');

    UI.bindCardInteractions(mainEl());

    // Essay Aside Controls
    var saveBtn = document.getElementById('essay-save-btn');
    saveBtn.addEventListener('click', async function () {
      if (!Rana.state.user) return Rana.auth.openAuthModal('login');
      var currentlySaved = saveBtn.classList.contains('is-active');
      try {
        if (currentlySaved) {
          await Rana.api.unsaveArticle(article.slug);
          Rana.state.savedIds.delete(article.id);
          Rana.state.savedSlugs.delete(article.slug);
          saveBtn.classList.remove('is-active');
          saveBtn.innerHTML = UI.ICONS.bookmark(false, 19) + '<span>' + U.escapeHtml(Rana.t('cardSave')) + '</span>';
        } else {
          await Rana.api.saveArticle(article.slug);
          Rana.state.savedIds.add(article.id);
          Rana.state.savedSlugs.add(article.slug);
          saveBtn.classList.add('is-active');
          saveBtn.innerHTML = UI.ICONS.bookmark(true, 19) + '<span>' + U.escapeHtml(Rana.t('cardSaved')) + '</span>';
        }
      } catch (err) { UI.toast(err.message, 'warn'); }
    });

    document.getElementById('essay-share-btn').addEventListener('click', function () {
      if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(global.location.href).then(function () {
          UI.toast(Rana.t('linkCopied'), 'success');
        }).catch(function () { UI.toast('Link: ' + global.location.href); });
      } else {
        UI.toast('Link: ' + global.location.href);
      }
    });

    document.getElementById('essay-jump-comments').addEventListener('click', function () {
      var c = document.getElementById('essay-comments-root');
      if (c) c.scrollIntoView({ behavior: 'smooth' });
    });

    // Audio Narration Player
    var audioBtn = document.getElementById('essay-audio-play');
    if (audioBtn) {
      var audioEl = document.getElementById('essay-audio-el');
      var rangeEl = document.getElementById('audio-range');
      var curTimeEl = document.getElementById('audio-cur-time');
      var playing = false;
      var simSec = 0;
      var totalSec = article.audioDurationSeconds || 600;

      function fmtSec(sec) {
        var m = Math.floor(sec / 60);
        var s2 = Math.floor(sec % 60);
        return m + ':' + (s2 < 10 ? '0' : '') + s2;
      }

      audioBtn.addEventListener('click', function () {
        if (!playing) {
          Rana.api.trackView(article.slug, 'AudioPlay').catch(function () {});
          playing = true;
          audioBtn.innerHTML = UI.ICONS.audioPause();
          if (audioEl) {
            audioEl.play().catch(function () {});
          } else {
            audioTimer = setInterval(function () {
              simSec = Math.min(totalSec, simSec + 1);
              curTimeEl.textContent = fmtSec(simSec);
              rangeEl.value = Math.round((simSec / totalSec) * 100);
            }, 1000);
          }
        } else {
          playing = false;
          audioBtn.innerHTML = UI.ICONS.audioPlay();
          if (audioEl) audioEl.pause();
          if (audioTimer) { clearInterval(audioTimer); audioTimer = null; }
        }
      });

      if (audioEl) {
        audioEl.addEventListener('timeupdate', function () {
          if (!audioEl.duration) return;
          curTimeEl.textContent = fmtSec(audioEl.currentTime);
          rangeEl.value = Math.round((audioEl.currentTime / audioEl.duration) * 100);
        });
        rangeEl.addEventListener('input', function () {
          if (audioEl.duration) audioEl.currentTime = (rangeEl.value / 100) * audioEl.duration;
        });
      }
    }

    // PageMarks (Highlights & Annotations)
    await initPageMarks(article.slug);

    // Comments Thread
    await mountCommentsSection(document.getElementById('essay-comments-root'), article);
  }

  async function initPageMarks(slug) {
    var proseEl = document.getElementById('essay-prose-body');
    var countEl = document.getElementById('pagemarks-count');
    var drawer = document.getElementById('annotations-drawer');
    var toggleBtn = document.getElementById('essay-notes-toggle');
    if (!proseEl) return;

    var marks = [];
    if (Rana.state.user && !Rana.state.offlineMode) {
      try { marks = await Rana.api.getPageMarks(slug); } catch (_) { marks = []; }
    }

    function renderMarks() {
      if (countEl) countEl.textContent = String(marks.length);
      // Clear previous marks
      proseEl.querySelectorAll('mark.pagemark-hl').forEach(function (m) {
        var parent = m.parentNode;
        parent.replaceChild(document.createTextNode(m.textContent), m);
        parent.normalize();
      });

      // Highlight matching phrases inside <p> nodes
      var pNodes = Array.from(proseEl.querySelectorAll('p'));
      marks.forEach(function (mark, idx) {
        var phrase = (mark.text || '').trim();
        if (!phrase) return;
        for (var i = 0; i < pNodes.length; i++) {
          var p = pNodes[i];
          var html = p.innerHTML;
          var pos = html.indexOf(phrase);
          if (pos !== -1) {
            p.innerHTML = html.slice(0, pos) +
              '<mark class="pagemark-hl" data-pm-idx="' + idx + '" title="' + U.escapeHtml(mark.comment || 'Highlight') + '">' +
              U.escapeHtml(phrase) +
              '</mark>' +
              html.slice(pos + phrase.length);
            break;
          }
        }
      });

      if (drawer) {
        if (!marks.length) {
          drawer.innerHTML = '<h4 class="font-mono" style="margin:0 0 8px;font-size:12px;letter-spacing:1.5px;text-transform:uppercase;">' + U.escapeHtml(Rana.t('hlTitle')) + '</h4><p style="margin:0;font-size:14px;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('hlEmpty')) + '</p>';
        } else {
          drawer.innerHTML =
            '<h4 class="font-mono" style="margin:0 0 12px;font-size:12px;letter-spacing:1.5px;text-transform:uppercase;">' + U.escapeHtml(Rana.t('hlMyTitle', { n: marks.length })) + '</h4>' +
            marks.map(function (m, i) {
              return '<div class="annotation-item">' +
                '<div style="font-style:italic;color:var(--ink-dark);">“' + U.escapeHtml(m.text) + '”</div>' +
                (m.comment ? '<div style="margin-top:4px;font-weight:500;">' + U.escapeHtml(Rana.t('hlNotePrefix', { n: m.annotationNumber || (i + 1) })) + ' ' + U.escapeHtml(m.comment) + '</div>' : '') +
                '<button type="button" data-del-pm="' + i + '" style="margin-top:6px;font-family:var(--font-mono);font-size:11px;color:var(--aeon-red);text-decoration:underline;">' + U.escapeHtml(Rana.t('hlRemove')) + '</button>' +
              '</div>';
            }).join('');
          drawer.querySelectorAll('[data-del-pm]').forEach(function (btn) {
            btn.addEventListener('click', async function () {
              marks.splice(Number(btn.getAttribute('data-del-pm')), 1);
              try {
                marks = await Rana.api.replacePageMarks(slug, marks);
              } catch (_) {}
              renderMarks();
            });
          });
        }
      }
    }

    if (toggleBtn && drawer) {
      toggleBtn.addEventListener('click', function () {
        drawer.classList.toggle('hidden');
        if (!drawer.classList.contains('hidden')) drawer.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
      });
    }

    proseEl.addEventListener('mouseup', function () {
      var old = document.getElementById('pagemark-popover');
      if (old) old.remove();

      var sel = global.getSelection();
      if (!sel || sel.isCollapsed) return;
      var selectedText = sel.toString().trim();
      if (selectedText.length < 4 || selectedText.length > 400) return;

      var range = sel.getRangeAt(0);
      var rect = range.getBoundingClientRect();
      var pop = document.createElement('div');
      pop.id = 'pagemark-popover';
      pop.className = 'pagemark-popover no-print';
      pop.style.left = Math.max(12, Math.min(global.innerWidth - 220, rect.left)) + 'px';
      pop.style.top = Math.max(12, rect.top - 42) + 'px';
      pop.innerHTML =
        '<button type="button" id="pm-add-hl">' + U.escapeHtml(Rana.t('hlBtnHighlight')) + '</button>' +
        '<span>|</span>' +
        '<button type="button" id="pm-add-note">' + U.escapeHtml(Rana.t('hlBtnAnnotate')) + '</button>';
      document.body.appendChild(pop);

      async function saveSelection(type, commentText) {
        pop.remove();
        if (!Rana.state.user) return Rana.auth.openAuthModal('login');
        marks.push({
          type: type,
          index: 0,
          text: selectedText,
          comment: commentText || null,
          annotationNumber: type === 'Annotation' ? marks.filter(function (m) { return m.type === 'Annotation'; }).length + 1 : null
        });
        try {
          marks = await Rana.api.replacePageMarks(slug, marks);
          UI.toast(type === 'Annotation' ? Rana.t('hlSavedNote') : Rana.t('hlSavedMark'), 'success');
          renderMarks();
        } catch (err) {
          UI.toast(err.message || Rana.t('hlSaveErr'), 'warn');
        }
      }

      document.getElementById('pm-add-hl').addEventListener('click', function () { saveSelection('Highlight', null); });
      document.getElementById('pm-add-note').addEventListener('click', async function () {
        var note = await UI.promptModal({
          title: Rana.t('hlPromptTitle'),
          message: Rana.t('hlPromptMsg'),
          required: true
        });
        if (note && note.trim()) saveSelection('Annotation', note.trim());
        else pop.remove();
      });
    });

    renderMarks();
  }

  /* ---------------------- VIDEO CINEMA PAGE ---------------------- */
  async function renderVideo(slug) {
    cleanupPageTimers();
    showLoading();

    var video;
    try {
      video = await Rana.api.getVideoBySlug(slug);
    } catch (err) {
      if (handleFallback(err)) {
        video = Rana.demo.articles.find(function (a) { return a.slug === slug; }) || Rana.demo.articles[1];
      } else {
        mainEl().innerHTML = '<div class="editorial-page"><h1>Video tapılmadı</h1><p>' + U.escapeHtml(err.message) + '</p></div>';
        return;
      }
    }

    var secSlug = (video.section && video.section.slug) || 'science';
    UI.renderOfflineBanner();
    UI.renderHeader({ sectionSlug: secSlug, overlay: false });
    setMainClass(U.sectionClass(secSlug));
    U.setDocumentTitle(video.title);

    if (!Rana.state.offlineMode) {
      Rana.api.trackView(video.slug, 'View').catch(function () {});
    }

    var poster = U.resolveMediaUrl(video.featuredImageUrl || video.cardImageUrl);
    var isSaved = Rana.state.savedIds.has(video.id) || Rana.state.savedSlugs.has(video.slug);
    var recent = video.recentVideos || Rana.demo.articles.filter(function (a) { return a.type === 'Video' && a.slug !== video.slug; });

    mainEl().innerHTML =
      '<div class="video-stage-band">' +
        '<div class="video-stage" id="video-stage-box">' +
          '<button type="button" class="video-stage__poster-btn" id="video-play-poster">' +
            '<img src="' + U.escapeHtml(poster) + '" alt="' + U.escapeHtml(video.title) + '" />' +
            '<div class="video-stage__overlay-content">' +
              '<h1 class="video-stage__title">' + U.escapeHtml(video.title) + '</h1>' +
              UI.ICONS.playCircle(92) +
              '<div class="video-stage__duration">' + U.escapeHtml(Rana.t('cardMinutes', { n: video.videoDurationMinutes || 10 })) + '</div>' +
            '</div>' +
          '</button>' +
        '</div>' +
      '</div>' +
      '<div class="essay-layout">' +
        '<aside class="essay-aside" style="padding-top:16px;">' +
          (video.creditsHtml ? '<div class="essay-aside__bio">' + video.creditsHtml + '</div>' : '') +
          '<div class="essay-aside__topics">' +
            (video.topics || []).map(function (t) {
              return '<a href="#/section/' + secSlug + '/' + U.escapeHtml(t.slug) + '">' + U.escapeHtml(t.title) + '</a>';
            }).join('') +
          '</div>' +
          '<div class="essay-sticky-tools no-print">' +
            '<button type="button" class="essay-tool-btn' + (isSaved ? ' is-active' : '') + '" id="video-save-btn">' +
              UI.ICONS.bookmark(isSaved, 19) + '<span>' + U.escapeHtml(isSaved ? Rana.t('cardSaved') : Rana.t('cardSave')) + '</span>' +
            '</button>' +
            '<button type="button" class="essay-tool-btn" id="video-share-btn">' +
              UI.ICONS.share(18) + '<span>' + U.escapeHtml(Rana.t('share')) + '</span>' +
            '</button>' +
          '</div>' +
        '</aside>' +
        '<div>' +
          '<h2 class="font-serif" style="margin:0 0 12px;font-size:32px;line-height:1.2;">' + U.escapeHtml(video.title) + '</h2>' +
          '<p style="font-size:19px;line-height:1.5;color:var(--ink-mid);margin:0 0 24px;">' + U.escapeHtml(video.standfirstLong || video.standfirstShort || '') + '</p>' +
          '<div class="article-prose">' + U.resolveBodyMediaUrls(video.bodyHtml || '') + '</div>' +
          '<section class="comments-section" id="video-comments-root"></section>' +
        '</div>' +
      '</div>' +
      (recent.length
        ? '<div class="editorial-container" style="border-top:1px solid var(--border-hairline);padding-top:24px;">' +
            '<div class="articles-grid">' + recent.slice(0, 3).map(function (r) { return UI.renderArticleCard(r); }).join('') + '</div>' +
          '</div>'
        : '');

    UI.bindCardInteractions(mainEl());

    document.getElementById('video-play-poster').addEventListener('click', function () {
      var box = document.getElementById('video-stage-box');
      if (!box) return;
      var embed = video.videoEmbedUrl || '';
      if (String(video.videoHoster) === 'SelfHosted' && embed) {
        box.innerHTML = '<video controls autoplay src="' + U.escapeHtml(U.resolveMediaUrl(embed)) + '"></video>';
      } else if (embed) {
        var sep = embed.indexOf('?') !== -1 ? '&' : '?';
        box.innerHTML = '<iframe src="' + U.escapeHtml(embed + sep + 'autoplay=1') + '" allow="autoplay; fullscreen" allowfullscreen></iframe>';
      } else {
        UI.toast('Video embed URL təyin edilməyib.');
      }
      if (!Rana.state.offlineMode) Rana.api.trackView(video.slug, 'EngagedView').catch(function () {});
    });

    var saveBtn = document.getElementById('video-save-btn');
    saveBtn.addEventListener('click', async function () {
      if (!Rana.state.user) return Rana.auth.openAuthModal('login');
      var s = saveBtn.classList.contains('is-active');
      if (s) { await Rana.api.unsaveArticle(video.slug); saveBtn.classList.remove('is-active'); saveBtn.innerHTML = UI.ICONS.bookmark(false, 19) + '<span>' + U.escapeHtml(Rana.t('cardSave')) + '</span>'; }
      else { await Rana.api.saveArticle(video.slug); saveBtn.classList.add('is-active'); saveBtn.innerHTML = UI.ICONS.bookmark(true, 19) + '<span>' + U.escapeHtml(Rana.t('cardSaved')) + '</span>'; }
    });
    document.getElementById('video-share-btn').addEventListener('click', function () {
      if (navigator.clipboard) navigator.clipboard.writeText(global.location.href).then(function () { UI.toast(Rana.t('linkCopied'), 'success'); });
    });

    await mountCommentsSection(document.getElementById('video-comments-root'), video);
  }

  /* ---------------------- CONTRIBUTOR PROFILE ---------------------- */
  async function renderContributor(slug) {
    cleanupPageTimers();
    showLoading();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');

    var profile;
    try {
      profile = await Rana.api.getContributor(slug);
    } catch (err) {
      if (handleFallback(err)) {
        var c = Rana.demo.contributors[slug] || Rana.demo.contributors['leyla-aliyeva'];
        profile = Object.assign({}, c, {
          authoredArticles: { nodes: Rana.demo.articles.filter(function (a) { return (a.authors || []).some(function (au) { return au.slug === c.slug; }); }), hasNextPage: false },
          editedArticles: { nodes: Rana.demo.articles.slice(0, 2), hasNextPage: false }
        });
      } else {
        mainEl().innerHTML = '<div class="editorial-page"><p>' + U.escapeHtml(err.message) + '</p></div>';
        return;
      }
    }

    UI.renderOfflineBanner();
    U.setDocumentTitle(profile.name);
    var activeTab = 'authored';

    function draw() {
      var list = activeTab === 'authored' ? (profile.authoredArticles.nodes || []) : (profile.editedArticles.nodes || []);
      mainEl().innerHTML =
        '<div class="contributor-header">' +
          '<div class="contributor-avatar">' +
            (profile.avatarUrl ? '<img src="' + U.escapeHtml(U.resolveMediaUrl(profile.avatarUrl)) + '" alt="' + U.escapeHtml(profile.name) + '" />' : U.escapeHtml(U.initials(profile.name))) +
          '</div>' +
          '<div style="flex:1;min-width:240px;">' +
            '<h1 class="font-serif" style="margin:0 0 6px;font-size:38px;">' + U.escapeHtml(profile.name) + '</h1>' +
            ((profile.occupation || profile.company)
              ? '<p style="margin:0 0 12px;font-family:var(--font-mono);font-size:13px;color:var(--ink-mid);">' + U.escapeHtml([profile.occupation, profile.company].filter(Boolean).join(' · ')) + '</p>'
              : '') +
            '<div style="font-size:16px;line-height:1.6;color:var(--ink-dark);">' + (profile.bioHtml || profile.shortBioHtml || '') + '</div>' +
          '</div>' +
        '</div>' +
        '<div class="sort-tabs" style="margin-top:28px;">' +
          '<button type="button" class="sort-tabs__btn' + (activeTab === 'authored' ? ' is-active' : '') + '" data-c-tab="authored">' + U.escapeHtml(Rana.t('contribAuthored')) + '</button>' +
          '<button type="button" class="sort-tabs__btn' + (activeTab === 'edited' ? ' is-active' : '') + '" data-c-tab="edited">' + U.escapeHtml(Rana.t('contribEdited')) + '</button>' +
        '</div>' +
        '<div class="editorial-container"><div class="articles-grid">' +
          (list.length ? list.map(function (a) { return UI.renderArticleCard(a); }).join('') : '<p style="grid-column:1/-1;text-align:center;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('emptyCollection')) + '</p>') +
        '</div></div>';

      UI.bindCardInteractions(mainEl());
      mainEl().querySelectorAll('[data-c-tab]').forEach(function (b) {
        b.addEventListener('click', function () { activeTab = b.getAttribute('data-c-tab'); draw(); });
      });
    }
    draw();
  }

  /* ---------------------- SEARCH PAGE ---------------------- */
  async function renderSearch(query) {
    cleanupPageTimers();
    showLoading();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');
    var q = (query && query.q) || '';
    var type = (query && query.type) || '';
    U.setDocumentTitle(q ? (Rana.t('navSearch') + ': ' + q) : Rana.t('navSearch'));

    var result;
    try {
      result = await Rana.api.search(q, type || null, null, 24);
    } catch (err) {
      if (handleFallback(err)) {
        var norm = q.toLowerCase();
        var filtered = Rana.demo.articles.filter(function (a) {
          var tMatch = !type || a.type.toLowerCase() === type.toLowerCase();
          var qMatch = !norm || a.title.toLowerCase().indexOf(norm) !== -1 || (a.standfirstShort || '').toLowerCase().indexOf(norm) !== -1;
          return tMatch && qMatch;
        });
        result = { nodes: filtered, totalCount: filtered.length, hasNextPage: false };
      } else {
        result = { nodes: [], totalCount: 0, hasNextPage: false };
      }
    }

    UI.renderOfflineBanner();
    var nodes = result.nodes || [];
    mainEl().innerHTML =
      '<div class="section-header-band" style="max-width:860px;">' +
        '<form id="page-search-form" class="search-overlay__input-row" style="margin-bottom:20px;">' +
          '<input type="search" id="page-search-input" value="' + U.escapeHtml(q) + '" placeholder="' + U.escapeHtml(Rana.t('searchPagePh')) + '" />' +
          '<button type="submit">' + UI.ICONS.search(24) + '</button>' +
        '</form>' +
        '<div class="sort-tabs">' +
          '<a href="#/search?q=' + encodeURIComponent(q) + '" class="sort-tabs__btn' + (!type ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('searchAll')) + ' (' + (result.totalCount || nodes.length) + ')</a>' +
          '<a href="#/search?q=' + encodeURIComponent(q) + '&type=Essay" class="sort-tabs__btn' + (type === 'Essay' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('pageEssays')) + '</a>' +
          '<a href="#/search?q=' + encodeURIComponent(q) + '&type=Video" class="sort-tabs__btn' + (type === 'Video' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('pageVideos')) + '</a>' +
          '<a href="#/search?q=' + encodeURIComponent(q) + '&type=Idea" class="sort-tabs__btn' + (type === 'Idea' ? ' is-active' : '') + '">' + U.escapeHtml(Rana.t('typeIdea')) + '</a>' +
        '</div>' +
      '</div>' +
      '<div class="editorial-container"><div class="articles-grid" id="search-grid">' +
        (nodes.length ? nodes.map(function (a) { return UI.renderArticleCard(a); }).join('') : '<p style="grid-column:1/-1;text-align:center;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('searchEmpty')) + '</p>') +
      '</div></div>';

    UI.bindCardInteractions(mainEl());
    document.getElementById('page-search-form').addEventListener('submit', function (e) {
      e.preventDefault();
      var v = document.getElementById('page-search-input').value.trim();
      global.location.hash = '#/search?q=' + encodeURIComponent(v) + (type ? '&type=' + encodeURIComponent(type) : '');
    });
  }

  /* ---------------------- MY LIBRARY ---------------------- */
  async function renderLibrary() {
    cleanupPageTimers();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');
    U.setDocumentTitle(Rana.t('libTitle'));

    if (!Rana.state.user && !Rana.api.getAccessToken()) {
      mainEl().innerHTML =
        '<div class="editorial-page" style="text-align:center;">' +
          '<h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('libTitle')) + '</h1>' +
          '<p class="editorial-page__subtitle">' + U.escapeHtml(Rana.t('libGuestSub')) + '</p>' +
          '<button type="button" class="aeon-btn" id="lib-signin-btn">' + U.escapeHtml(Rana.t('authSignIn')) + '</button>' +
        '</div>';
      document.getElementById('lib-signin-btn').addEventListener('click', function () {
        Rana.auth.openAuthModal('login', renderLibrary);
      });
      return;
    }

    showLoading();
    var savedItems = [], annotatedItems = [], tab = 'saved';
    try {
      var lib = await Rana.api.getLibrary();
      savedItems = (lib && lib.items) ? lib.items.map(function (i) { return i.article; }) : [];
      annotatedItems = await Rana.api.getAnnotatedArticles();
    } catch (err) {
      UI.toast(err.message || 'Kitabxana yüklənmədi.', 'warn');
    }

    function draw() {
      var list = tab === 'saved' ? savedItems : annotatedItems;
      mainEl().innerHTML =
        '<div class="section-header-band">' +
          '<h1 class="section-header-band__title">' + U.escapeHtml(Rana.t('libTitle')) + '</h1>' +
          '<div class="sort-tabs">' +
            '<button type="button" class="sort-tabs__btn' + (tab === 'saved' ? ' is-active' : '') + '" data-lib-tab="saved">' + U.escapeHtml(Rana.t('libSavedTab', { n: savedItems.length })) + '</button>' +
            '<button type="button" class="sort-tabs__btn' + (tab === 'annotated' ? ' is-active' : '') + '" data-lib-tab="annotated">' + U.escapeHtml(Rana.t('libAnnotatedTab', { n: annotatedItems.length })) + '</button>' +
          '</div>' +
        '</div>' +
        '<div class="editorial-container"><div class="articles-grid">' +
          (list.length ? list.map(function (a) { return UI.renderArticleCard(a); }).join('') : '<p style="grid-column:1/-1;text-align:center;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('libEmpty')) + '</p>') +
        '</div></div>';

      UI.bindCardInteractions(mainEl());
      mainEl().querySelectorAll('[data-lib-tab]').forEach(function (b) {
        b.addEventListener('click', function () { tab = b.getAttribute('data-lib-tab'); draw(); });
      });
    }
    draw();
  }

  /* ---------------------- NEWSLETTER / PITCH / SYNDICATION / STATIC ---------------------- */
  async function renderNewsletter(subroute, query) {
    cleanupPageTimers();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');
    U.setDocumentTitle(Rana.t('navNewsletter'));

    if (subroute === 'confirm' && query && query.token) {
      try {
        await Rana.api.confirmNewsletter(query.token);
        mainEl().innerHTML = '<div class="editorial-page"><h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('nlConfirmedTitle')) + '</h1><p class="editorial-page__subtitle">' + U.escapeHtml(Rana.t('nlConfirmedSub')) + '</p><a href="#/" class="aeon-btn">' + U.escapeHtml(Rana.t('nlBackHome')) + '</a></div>';
      } catch (err) {
        mainEl().innerHTML = '<div class="editorial-page"><h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('nlConfirmFail')) + '</h1><p>' + U.escapeHtml(err.message) + '</p></div>';
      }
      return;
    }

    if (subroute === 'unsubscribe' && query && query.token) {
      try {
        await Rana.api.unsubscribeNewsletter(query.token);
        mainEl().innerHTML = '<div class="editorial-page"><h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('nlUnsubTitle')) + '</h1><p class="editorial-page__subtitle">' + U.escapeHtml(Rana.t('nlUnsubSub')) + '</p></div>';
      } catch (err) {
        mainEl().innerHTML = '<div class="editorial-page"><h1 class="editorial-page__title">Error</h1><p>' + U.escapeHtml(err.message) + '</p></div>';
      }
      return;
    }

    mainEl().innerHTML =
      '<div class="editorial-page">' +
        '<h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('nlPageTitle')) + '</h1>' +
        '<p class="editorial-page__subtitle">' + U.escapeHtml(Rana.t('nlPageSub')) + '</p>' +
        '<form id="page-nl-form" class="form-grid" style="max-width:480px;">' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('nlEmailPh')) + '</label><input type="email" name="email" required class="field-input" placeholder="reader@domain.com" /></div>' +
          '<input type="text" name="website" tabindex="-1" autocomplete="off" style="position:absolute;left:-9999px;" />' +
          '<div style="display:flex;gap:24px;">' +
            '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="daily" checked /></span><span>' + U.escapeHtml(Rana.t('nlDailyEd')) + '</span></label>' +
            '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="weekly" /></span><span>' + U.escapeHtml(Rana.t('nlWeeklyDig')) + '</span></label>' +
          '</div>' +
          '<div><button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('nlSubscribe')) + ' ' + UI.ICONS.arrowRight() + '</button></div>' +
        '</form>' +
      '</div>';

    document.getElementById('page-nl-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var f = e.target;
      try {
        await Rana.api.subscribeNewsletter({
          email: f.email.value.trim(),
          daily: f.daily.checked,
          weekly: f.weekly.checked,
          website: f.website.value,
          source: 'newsletter-page'
        });
        f.reset();
        UI.toast(Rana.t('nlConfirmSent'), 'success');
      } catch (err) { UI.toast(err.message, 'warn'); }
    });
  }

  function renderPitch() {
    cleanupPageTimers();
    UI.renderHeader({ sectionSlug: 'philosophy' });
    setMainClass('section-philosophy');
    U.setDocumentTitle(Rana.t('pageWriteForUs'));

    mainEl().innerHTML =
      '<div class="editorial-page">' +
        '<h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('pitchTitle')) + '</h1>' +
        '<p class="editorial-page__subtitle">' + U.escapeHtml(Rana.t('pitchSub')) + '</p>' +
        '<form id="pitch-form" class="form-grid">' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchName')) + '</label><input type="text" name="name" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchEmail')) + '</label><input type="email" name="email" required class="field-input" /></div>' +
          '</div>' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchBio')) + '</label><textarea name="bio" required minlength="10" class="field-textarea" placeholder="' + U.escapeHtml(Rana.t('pitchBioPh')) + '"></textarea></div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchWebsite')) + '</label><input type="url" name="website" class="field-input" placeholder="https://..." /></div>' +
            '<div class="field-group" style="justify-content:flex-end;padding-bottom:8px;"><label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="prev" /></span><span>' + U.escapeHtml(Rana.t('pitchPrev')) + '</span></label></div>' +
          '</div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchWorkTitle')) + '</label><input type="text" name="title" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchField')) + '</label><select name="field" class="field-select"><option value="Philosophy">' + U.escapeHtml(Rana.t('secPhilosophy')) + '</option><option value="Science">' + U.escapeHtml(Rana.t('secScience')) + '</option><option value="Psychology">' + U.escapeHtml(Rana.t('secPsychology')) + '</option><option value="Society">' + U.escapeHtml(Rana.t('secSociety')) + '</option><option value="Culture">' + U.escapeHtml(Rana.t('secCulture')) + '</option></select></div>' +
          '</div>' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchFormat')) + '</label><select name="format" class="field-select"><option value="Essay">' + U.escapeHtml(Rana.t('pitchFmtEssay')) + '</option><option value="Idea">' + U.escapeHtml(Rana.t('pitchFmtIdea')) + '</option></select></div>' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchBody')) + '</label><textarea name="pitch" required minlength="50" class="field-textarea" style="min-height:170px;" placeholder="' + U.escapeHtml(Rana.t('pitchBodyPh')) + '"></textarea></div>' +
          '<div><button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('pitchSubmit')) + ' ' + UI.ICONS.arrowRight() + '</button></div>' +
        '</form>' +
      '</div>';

    document.getElementById('pitch-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var f = e.target;
      try {
        await Rana.api.submitPitch({
          name: f.name.value.trim(),
          email: f.email.value.trim(),
          bio: f.bio.value.trim(),
          website: f.website.value.trim() || null,
          previousContributor: f.prev.checked,
          title: f.title.value.trim(),
          field: f.field.value,
          format: f.format.value,
          pitch: f.pitch.value.trim()
        });
        f.reset();
        UI.toast(Rana.t('pitchSuccess'), 'success');
      } catch (err) { UI.toast(err.message || Rana.t('opFailed'), 'warn'); }
    });
  }

  function renderSyndication(query) {
    cleanupPageTimers();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');
    U.setDocumentTitle(Rana.t('pageRepublish'));
    var defaultSlug = (query && (query.slug || query.article_slug)) || '';

    mainEl().innerHTML =
      '<div class="editorial-page">' +
        '<h1 class="editorial-page__title">' + U.escapeHtml(Rana.t('syndTitle')) + '</h1>' +
        '<p class="editorial-page__subtitle">' + U.escapeHtml(Rana.t('syndSub')) + '</p>' +
        '<form id="synd-form" class="form-grid">' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndSlug')) + '</label><input type="text" name="articleSlug" required value="' + U.escapeHtml(defaultSlug) + '" class="field-input" placeholder="e.g. why-anything-exists-instead-of-nothing" /></div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchName')) + '</label><input type="text" name="name" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('pitchEmail')) + '</label><input type="email" name="email" required class="field-input" /></div>' +
          '</div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndOrg')) + '</label><input type="text" name="company" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndPub')) + '</label><input type="text" name="publication" required class="field-input" /></div>' +
          '</div>' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndAddr')) + '</label><input type="text" name="address" required class="field-input" /></div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndCity')) + '</label><input type="text" name="city" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndState')) + '</label><input type="text" name="state" required class="field-input" /></div>' +
          '</div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndPost')) + '</label><input type="text" name="postCode" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndCountry')) + '</label><input type="text" name="country" required class="field-input" /></div>' +
          '</div>' +
          '<div class="form-grid form-grid--2">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndLang')) + '</label><input type="text" name="language" value="English" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('syndPubUrl')) + '</label><input type="url" name="pubUrl" class="field-input" placeholder="https://..." /></div>' +
          '</div>' +
          '<div style="display:flex;flex-wrap:wrap;gap:24px;">' +
            '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="online" checked /></span><span>' + U.escapeHtml(Rana.t('syndOnline')) + '</span></label>' +
            '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="print" /></span><span>' + U.escapeHtml(Rana.t('syndPrint')) + '</span></label>' +
          '</div>' +
          '<label class="aeon-checkbox-label"><span class="aeon-checkbox-box"><input type="checkbox" name="terms" required /></span><span>' + U.escapeHtml(Rana.t('syndTerms')) + '</span></label>' +
          '<div><button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('syndSubmit')) + ' ' + UI.ICONS.arrowRight() + '</button></div>' +
        '</form>' +
      '</div>';

    document.getElementById('synd-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var f = e.target;
      try {
        await Rana.api.submitSyndication({
          articleSlug: f.articleSlug.value.trim(),
          name: f.name.value.trim(),
          email: f.email.value.trim(),
          company: f.company.value.trim(),
          publication: f.publication.value.trim(),
          address: f.address.value.trim(),
          city: f.city.value.trim(),
          state: f.state.value.trim(),
          postCode: f.postCode.value.trim(),
          country: f.country.value.trim(),
          language: f.language.value.trim(),
          medium: { print: f.print.checked, online: f.online.checked, publicationUrl: f.pubUrl.value.trim() || null },
          termsAndConditions: f.terms.checked
        });
        f.reset();
        UI.toast(Rana.t('syndSuccess'), 'success');
      } catch (err) { UI.toast(err.message || Rana.t('opFailed'), 'warn'); }
    });
  }

  async function renderStaticPage(slug) {
    cleanupPageTimers();
    showLoading();
    UI.renderHeader({ sectionSlug: 'society' });
    setMainClass('section-society');

    var page;
    try {
      page = await Rana.api.getStaticPage(slug);
    } catch (err) {
      if (handleFallback(err)) {
        page = Rana.demo.staticPages[slug] || Rana.demo.staticPages.about;
      } else {
        mainEl().innerHTML = '<div class="editorial-page"><h1 class="editorial-page__title">Səhifə tapılmadı</h1><p>' + U.escapeHtml(err.message) + '</p></div>';
        return;
      }
    }

    UI.renderOfflineBanner();
    U.setDocumentTitle(page.title);
    mainEl().innerHTML =
      '<div class="editorial-page">' +
        '<h1 class="editorial-page__title">' + U.escapeHtml(page.title) + '</h1>' +
        (page.subtitle ? '<p class="editorial-page__subtitle">' + U.escapeHtml(page.subtitle) + '</p>' : '') +
        '<div class="article-prose">' + (page.bodyHtml || '') + '</div>' +
      '</div>';
  }

  Rana.pages = {
    renderHome: renderHome,
    renderCollection: renderCollection,
    renderSection: renderSection,
    renderEssay: renderEssay,
    renderVideo: renderVideo,
    renderContributor: renderContributor,
    renderSearch: renderSearch,
    renderLibrary: renderLibrary,
    renderNewsletter: renderNewsletter,
    renderPitch: renderPitch,
    renderSyndication: renderSyndication,
    renderStaticPage: renderStaticPage
  };
})(window);
