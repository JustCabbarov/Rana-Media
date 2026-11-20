/* ==========================================================================
   Rana Media API Client with Automatic JWT Refresh & ProblemDetails Handling
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});
  var refreshPromise = null;

  var TOKEN_KEY = 'rana_access_token';
  var REFRESH_KEY = 'rana_refresh_token';

  function getAccessToken() {
    try { return global.localStorage.getItem(TOKEN_KEY); } catch (_) { return null; }
  }

  function getRefreshToken() {
    try { return global.localStorage.getItem(REFRESH_KEY); } catch (_) { return null; }
  }

  function setTokens(access, refresh) {
    try {
      if (access) global.localStorage.setItem(TOKEN_KEY, access);
      if (refresh) global.localStorage.setItem(REFRESH_KEY, refresh);
    } catch (_) {}
  }

  function clearTokens() {
    try {
      global.localStorage.removeItem(TOKEN_KEY);
      global.localStorage.removeItem(REFRESH_KEY);
    } catch (_) {}
  }

  function ApiError(status, message, errorCode, details, isNetworkError) {
    this.name = 'ApiError';
    this.status = status || 0;
    this.message = message || 'Server xətası baş verdi.';
    this.errorCode = errorCode || null;
    this.details = details || null;
    this.isNetworkError = !!isNetworkError;
  }
  ApiError.prototype = Object.create(Error.prototype);

  function extractErrorMessage(payload, fallbackStatus) {
    if (!payload) return 'HTTP ' + fallbackStatus;
    if (typeof payload === 'string') return payload;
    if (payload.detail) return payload.detail;
    if (payload.message) return payload.message;
    if (payload.Message) return payload.Message;
    if (payload.title) return payload.title;
    if (Array.isArray(payload.errors) && payload.errors.length) {
      return payload.errors.map(function (e) { return e.message || e; }).join(' ');
    }
    if (payload.errors && typeof payload.errors === 'object') {
      var msgs = [];
      Object.keys(payload.errors).forEach(function (k) {
        var val = payload.errors[k];
        if (Array.isArray(val)) msgs = msgs.concat(val);
        else if (typeof val === 'string') msgs.push(val);
      });
      if (msgs.length) return msgs.join(' ');
    }
    return 'Xəta kodu: ' + fallbackStatus;
  }

  async function tryRefreshTokens() {
    var rt = getRefreshToken();
    if (!rt) return false;

    if (!refreshPromise) {
      refreshPromise = (async function () {
        try {
          var res = await fetch(Rana.config.API_BASE + '/api/Auth/refresh', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ refreshToken: rt })
          });
          if (!res.ok) {
            clearTokens();
            return false;
          }
          var data = await res.json();
          if (data && data.accessToken) {
            setTokens(data.accessToken, data.refreshToken || rt);
            return true;
          }
          clearTokens();
          return false;
        } catch (_) {
          return false;
        } finally {
          refreshPromise = null;
        }
      })();
    }

    return refreshPromise;
  }

  async function request(path, options) {
    options = options || {};
    var url = path.indexOf('http') === 0 ? path : (Rana.config.API_BASE + path);
    var headers = Object.assign({}, options.headers || {});
    var isFormData = (typeof FormData !== 'undefined') && (options.body instanceof FormData);

    if (!isFormData && options.body && !headers['Content-Type']) {
      headers['Content-Type'] = 'application/json';
    }

    var token = getAccessToken();
    if (token && !options.skipAuth) {
      headers['Authorization'] = 'Bearer ' + token;
    }

    var fetchOpts = {
      method: options.method || 'GET',
      headers: headers
    };
    if (options.body !== undefined) {
      fetchOpts.body = isFormData || typeof options.body === 'string'
        ? options.body
        : JSON.stringify(options.body);
    }

    var res;
    try {
      res = await fetch(url, fetchOpts);
    } catch (netErr) {
      throw new ApiError(0, 'Backend API ilə əlaqə yaradılmadı.', 'NETWORK_ERROR', netErr, true);
    }

    if (res.status === 401 && !options._retried && !options.skipAuth) {
      var refreshed = await tryRefreshTokens();
      if (refreshed) {
        return request(path, Object.assign({}, options, { _retried: true }));
      }
    }

    if (res.status === 204 || res.status === 202) {
      return null;
    }

    var ct = res.headers.get('content-type') || '';
    var payload = null;
    if (ct.indexOf('application/json') !== -1 || ct.indexOf('application/problem+json') !== -1) {
      try { payload = await res.json(); } catch (_) { payload = null; }
    } else {
      var text = await res.text();
      payload = text;
    }

    if (!res.ok) {
      var code = payload && typeof payload === 'object' ? (payload.errorCode || payload.code || null) : null;
      throw new ApiError(res.status, extractErrorMessage(payload, res.status), code, payload, false);
    }

    Rana.state.offlineMode = false;
    return payload;
  }

  function qs(params) {
    var parts = [];
    Object.keys(params || {}).forEach(function (k) {
      var v = params[k];
      if (v !== null && v !== undefined && v !== '') {
        parts.push(encodeURIComponent(k) + '=' + encodeURIComponent(v));
      }
    });
    return parts.length ? '?' + parts.join('&') : '';
  }

  Rana.api = {
    ApiError: ApiError,
    getAccessToken: getAccessToken,
    getRefreshToken: getRefreshToken,
    setTokens: setTokens,
    clearTokens: clearTokens,
    request: request,

    // Public Editorial
    getHome: function () { return request('/api/Home'); },
    getHomeMore: function (afterCursor, first) {
      return request('/api/Home/more' + qs({ afterCursor: afterCursor, first: first || 30 }));
    },
    getArticles: function (params) { return request('/api/Articles' + qs(params)); },
    getEssays: function (sort, afterCursor, first) {
      return request('/api/essays' + qs({ sort: sort || 'latest', afterCursor: afterCursor, first: first || 12 }));
    },
    getPopular: function (afterCursor, first) {
      return request('/api/popular' + qs({ afterCursor: afterCursor, first: first || 24 }));
    },
    getArticleBySlug: function (slug) { return request('/api/Articles/' + encodeURIComponent(slug)); },
    getArticlePreview: function (slug, token) {
      return request('/api/Articles/' + encodeURIComponent(slug) + '/preview' + qs({ token: token }));
    },
    getVideos: function (filter, afterCursor, first) {
      return request('/api/Videos' + qs({ filter: filter, afterCursor: afterCursor, first: first || 12 }));
    },
    getVideoBySlug: function (slug) { return request('/api/Videos/' + encodeURIComponent(slug)); },
    getSections: function () { return request('/api/Sections'); },
    getSectionsCatalog: function () { return request('/api/Sections/catalog'); },
    getSectionBySlug: function (slug, sort, afterCursor, first) {
      return request('/api/Sections/' + encodeURIComponent(slug) + qs({ sort: sort || 'latest', afterCursor: afterCursor, first: first || 24 }));
    },
    getTopicBySlug: function (sectionSlug, topicSlug, sort, afterCursor, first) {
      return request('/api/sections/' + encodeURIComponent(sectionSlug) + '/topics/' + encodeURIComponent(topicSlug) + qs({ sort: sort || 'latest', afterCursor: afterCursor, first: first || 24 }));
    },
    getContributor: function (slug, authoredCursor, editedCursor) {
      return request('/api/users/' + encodeURIComponent(slug) + qs({ authoredCursor: authoredCursor, editedCursor: editedCursor }));
    },
    search: function (q, type, afterCursor, first) {
      return request('/api/Search' + qs({ q: q || '', type: type, afterCursor: afterCursor, first: first || 30 }));
    },
    trackView: function (slug, eventType) {
      return request('/api/view', { method: 'POST', body: { slug: slug, eventType: eventType || 'View' } });
    },

    // Forms & Static
    subscribeNewsletter: function (body) { return request('/api/newsletter/subscribe', { method: 'POST', body: body }); },
    confirmNewsletter: function (token) { return request('/api/newsletter/confirm' + qs({ token: token })); },
    unsubscribeNewsletter: function (token) { return request('/api/newsletter/unsubscribe' + qs({ token: token })); },
    submitPitch: function (body) { return request('/api/pitches', { method: 'POST', body: body }); },
    submitSyndication: function (body) { return request('/api/syndication', { method: 'POST', body: body }); },
    getStaticPage: function (slug) { return request('/api/pages/' + encodeURIComponent(slug)); },

    // Reader: Library, PageMarks, Comments, Notifications
    getLibrary: function () { return request('/api/Library'); },
    getAnnotatedArticles: function () { return request('/api/Library/annotated'); },
    getSavedIds: function () { return request('/api/Library/saved-ids'); },
    saveArticle: function (slug) { return request('/api/Library/saves/' + encodeURIComponent(slug), { method: 'POST' }); },
    unsaveArticle: function (slug) { return request('/api/Library/saves/' + encodeURIComponent(slug), { method: 'DELETE' }); },
    getPageMarks: function (slug) { return request('/api/articles/' + encodeURIComponent(slug) + '/pagemarks'); },
    replacePageMarks: function (slug, pageMarks) {
      return request('/api/articles/' + encodeURIComponent(slug) + '/pagemarks', { method: 'PUT', body: { pageMarks: pageMarks } });
    },
    getArticleComments: function (slug, sort, afterCursor, first) {
      return request('/api/articles/' + encodeURIComponent(slug) + '/comments' + qs({ sort: sort || 'newest', afterCursor: afterCursor, first: first || 20 }));
    },
    getConversations: function (limit) { return request('/api/conversations' + qs({ limit: limit || 24 })); },
    createComment: function (slug, body) {
      return request('/api/articles/' + encodeURIComponent(slug) + '/comments', { method: 'POST', body: { body: body } });
    },
    updateComment: function (id, body) { return request('/api/comments/' + id, { method: 'PUT', body: { body: body } }); },
    deleteComment: function (id) { return request('/api/comments/' + id, { method: 'DELETE' }); },
    createReply: function (commentId, body) { return request('/api/comments/' + commentId + '/replies', { method: 'POST', body: { body: body } }); },
    updateReply: function (id, body) { return request('/api/replies/' + id, { method: 'PUT', body: { body: body } }); },
    deleteReply: function (id) { return request('/api/replies/' + id, { method: 'DELETE' }); },
    likeComment: function (id, on) { return request('/api/comments/' + id + '/like', { method: on ? 'POST' : 'DELETE' }); },
    likeReply: function (id, on) { return request('/api/replies/' + id + '/like', { method: on ? 'POST' : 'DELETE' }); },
    reportComment: function (id, reason) { return request('/api/comments/' + id + '/report', { method: 'POST', body: { reason: reason } }); },
    reportReply: function (id, reason) { return request('/api/replies/' + id + '/report', { method: 'POST', body: { reason: reason } }); },

    getNotifications: function (limit) { return request('/api/Notifications' + qs({ limit: limit || 30 })); },
    markNotificationRead: function (id) { return request('/api/Notifications/' + id + '/read', { method: 'POST' }); },
    markAllNotificationsRead: function () { return request('/api/Notifications/read-all', { method: 'POST' }); },

    // Auth
    register: function (dto) { return request('/api/Auth/register', { method: 'POST', body: dto, skipAuth: true }); },
    login: function (dto) { return request('/api/Auth/login', { method: 'POST', body: dto, skipAuth: true }); },
    logout: function (refreshToken) { return request('/api/Auth/logout', { method: 'POST', body: { refreshToken: refreshToken } }); },
    acceptGuidelines: function () { return request('/api/Auth/accept-guidelines', { method: 'POST' }); },
    getCurrentUser: function () { return request('/api/Auth/GetCurrentUserData'); },
    editUser: function (dto, userId) { return request('/api/Auth/edit' + qs({ userId: userId }), { method: 'PUT', body: dto }); },
    sendResetOtp: function (email) { return request('/api/Auth/SendResetOtp', { method: 'POST', body: { email: email }, skipAuth: true }); },
    resetPassword: function (dto) { return request('/api/Auth/ResetPassword', { method: 'POST', body: dto, skipAuth: true }); },

    // Admin Studio
    adminGetStats: function () { return request('/api/admin/dashboard/stats'); },
    adminGetArticles: function (params) { return request('/api/admin/articles' + qs(params)); },
    adminGetArticle: function (id) { return request('/api/admin/articles/' + id); },
    adminCreateArticle: function (dto) { return request('/api/admin/articles', { method: 'POST', body: dto }); },
    adminUpdateArticle: function (id, dto) { return request('/api/admin/articles/' + id, { method: 'PUT', body: dto }); },
    adminDeleteArticle: function (id) { return request('/api/admin/articles/' + id, { method: 'DELETE' }); },
    adminAnalyzeSeoLive: function (dto) { return request('/api/admin/seo/analyze', { method: 'POST', body: dto }); },
    adminAnalyzeSeoPersist: function (articleId, dto) {
      return request('/api/admin/seo/articles/' + articleId + '/analyze', { method: 'POST', body: dto });
    },
    adminGetKeyphraseUsage: function (keyphrase, excludeArticleId) {
      return request('/api/admin/seo/keyphrase-usage' + qs({ keyphrase: keyphrase, excludeArticleId: excludeArticleId }));
    },
    adminGetContributors: function () { return request('/api/admin/contributors'); },
    adminCreateContributor: function (dto) { return request('/api/admin/contributors', { method: 'POST', body: dto }); },
    adminDeleteContributor: function (id) { return request('/api/admin/contributors/' + id, { method: 'DELETE' }); },
    adminGetComments: function (params) { return request('/api/admin/comments' + qs(params)); },
    adminModerateComment: function (id, moderate, reason) {
      return request('/api/admin/comments/' + id + '/' + (moderate ? 'moderate' : 'unmoderate'), {
        method: 'POST',
        body: moderate ? { reason: reason || '' } : undefined
      });
    },
    adminFeatureComment: function (id, feature) {
      return request('/api/admin/comments/' + id + '/' + (feature ? 'feature' : 'unfeature'), { method: 'POST' });
    },
    adminGetPitches: function (status) { return request('/api/admin/submissions/pitches' + qs({ status: status })); },
    adminUpdatePitch: function (id, status, notes) {
      return request('/api/admin/submissions/pitches/' + id + qs({ status: status, notes: notes }), { method: 'PATCH' });
    },
    adminGetSyndications: function (status) { return request('/api/admin/submissions/syndication' + qs({ status: status })); },
    adminUpdateSyndication: function (id, status) {
      return request('/api/admin/submissions/syndication/' + id + qs({ status: status }), { method: 'PATCH' });
    },
    adminUploadMedia: function (file, folder) {
      var fd = new FormData();
      fd.append('file', file);
      return request('/api/admin/media/upload' + qs({ folder: folder || 'Images' }), { method: 'POST', body: fd });
    },
    adminDeleteMedia: function (relativeUrl) {
      return request('/api/admin/media' + qs({ relativeUrl: relativeUrl }), { method: 'DELETE' });
    },
    adminGetUsers: function (params) { return request('/api/Auth/GetAllUsers' + qs(params)); },
    adminGetRoles: function () { return request('/api/Auth/GetAllRoles'); },
    adminCreateRole: function (dto) { return request('/api/Auth/create-role', { method: 'POST', body: dto }); },
    adminAssignRole: function (userId, roleName) {
      return request('/api/Auth/assign-role', { method: 'POST', body: { userId: userId, roleName: roleName } });
    },
    adminRemoveRole: function (userId, roleName) {
      return request('/api/Auth/remove-role', { method: 'DELETE', body: { userId: userId, roleName: roleName } });
    },
    adminDeleteUser: function (userId) { return request('/api/Auth/delete/User' + qs({ userId: userId }), { method: 'DELETE' }); }
  };
})(window);
