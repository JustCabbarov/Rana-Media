/* ==========================================================================
   Rana Media Client-Side Hash Router
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});

  function parseHash() {
    var raw = global.location.hash || '#/';
    var noHash = raw.replace(/^#/, '');
    if (!noHash.startsWith('/')) noHash = '/' + noHash;
    var parts = noHash.split('?');
    var path = parts[0].replace(/\/+$/, '') || '/';
    var query = {};
    if (parts[1]) {
      new URLSearchParams(parts[1]).forEach(function (v, k) { query[k] = v; });
    }
    return { path: path, query: query, segments: path === '/' ? [] : path.slice(1).split('/') };
  }

  function resolve() {
    var r = parseHash();
    var seg = r.segments;
    var P = Rana.pages;
    global.scrollTo({ top: 0, behavior: 'instant' });

    if (r.path === '/' || r.path === '') return P.renderHome();
    if (r.path === '/essays') return P.renderCollection('essays', r.query);
    if (r.path === '/essays/popular') return P.renderCollection('essays-popular', r.query);
    if (r.path === '/videos') return P.renderCollection('videos', r.query);
    if (r.path === '/popular') return P.renderCollection('popular', r.query);
    if (r.path === '/conversations') return P.renderCollection('conversations', r.query);

    if (seg[0] === 'section' && seg.length === 2) return P.renderSection(seg[1], null, r.query);
    if (seg[0] === 'section' && seg.length >= 3) return P.renderSection(seg[1], seg[2], r.query);

    if ((seg[0] === 'essay' || seg[0] === 'essays') && seg[1]) return P.renderEssay(decodeURIComponent(seg[1]), r.query);
    if ((seg[0] === 'video' || seg[0] === 'videos') && seg[1]) return P.renderVideo(decodeURIComponent(seg[1]));
    if (seg[0] === 'author' && seg[1]) return P.renderContributor(decodeURIComponent(seg[1]));
    if (seg[0] === 'search') return P.renderSearch(r.query);
    if (seg[0] === 'library') return P.renderLibrary();

    if (seg[0] === 'newsletter') return P.renderNewsletter(seg[1] || 'index', r.query);
    if (seg[0] === 'pitch' || seg[0] === 'write') return P.renderPitch();
    if (seg[0] === 'syndication' || seg[0] === 'republish') return P.renderSyndication(r.query);
    if (seg[0] === 'page' && seg[1]) return P.renderStaticPage(decodeURIComponent(seg[1]));

    return P.renderHome();
  }

  Rana.router = {
    resolve: resolve,
    parseHash: parseHash
  };
})(window);
