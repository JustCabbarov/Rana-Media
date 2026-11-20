/* ==========================================================================
   Rana Media Authentication, Profile, Notifications & Community Guidelines UI
   ========================================================================== */
(function (global) {
  'use strict';

  var Rana = (global.Rana = global.Rana || {});
  var U = Rana.utils;

  async function initSession() {
    var token = Rana.api.getAccessToken();
    if (!token) {
      Rana.state.user = null;
      Rana.state.savedIds = new Set();
      return null;
    }
    try {
      var user = await Rana.api.getCurrentUser();
      Rana.state.user = user;
      try {
        var ids = await Rana.api.getSavedIds();
        Rana.state.savedIds = new Set(Array.isArray(ids) ? ids : []);
      } catch (_) {}
      return user;
    } catch (err) {
      if (err.status === 401 || err.status === 403) {
        Rana.api.clearTokens();
        Rana.state.user = null;
      }
      return null;
    }
  }

  function hasRole(role) {
    var user = Rana.state.user;
    if (!user || !Array.isArray(user.roleName)) return false;
    return user.roleName.some(function (r) { return String(r).toLowerCase() === role.toLowerCase(); });
  }

  function hasStaffRole() {
    return hasRole('Admin') || hasRole('Editor') || hasRole('Moderator');
  }

  async function logout() {
    var rt = Rana.api.getRefreshToken();
    try {
      if (rt) await Rana.api.logout(rt);
    } catch (_) {}
    Rana.api.clearTokens();
    Rana.state.user = null;
    Rana.state.savedIds = new Set();
    Rana.state.savedSlugs = new Set();
    Rana.ui.closeModal();
    Rana.ui.toast(Rana.t('authSignOutToast'));
    if (Rana.config.isAdminPath && global.RanaAdmin) {
      global.RanaAdmin.boot();
    } else if (Rana.router) {
      Rana.ui.renderHeader();
      Rana.router.resolve();
    }
  }

  function openAuthModal(initialTab, onSuccess) {
    var tab = initialTab || 'login';
    var html =
      '<div style="margin-bottom:14px;">' + Rana.ui.ICONS.ranaWordmark() + '</div>' +
      '<div class="modal-tabs">' +
        '<button type="button" class="modal-tab-btn' + (tab === 'login' ? ' is-active' : '') + '" data-auth-tab="login">' + U.escapeHtml(Rana.t('authSignIn')) + '</button>' +
        '<button type="button" class="modal-tab-btn' + (tab === 'register' ? ' is-active' : '') + '" data-auth-tab="register">' + U.escapeHtml(Rana.t('authRegister')) + '</button>' +
        '<button type="button" class="modal-tab-btn' + (tab === 'otp' ? ' is-active' : '') + '" data-auth-tab="otp">' + U.escapeHtml(Rana.t('authReset')) + '</button>' +
      '</div>' +
      '<div id="auth-modal-body"></div>';

    Rana.ui.openModal(html, '460px');

    function switchTab(nextTab) {
      tab = nextTab;
      document.querySelectorAll('[data-auth-tab]').forEach(function (b) {
        b.classList.toggle('is-active', b.getAttribute('data-auth-tab') === nextTab);
      });
      var container = document.getElementById('auth-modal-body');
      if (!container) return;

      if (nextTab === 'login') {
        container.innerHTML =
          '<form id="auth-login-form" class="form-grid">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authEmail')) + '</label><input type="email" name="email" required class="field-input" placeholder="you@example.com" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authPassword')) + '</label><input type="password" name="password" required class="field-input" placeholder="••••••••" /></div>' +
            '<div id="auth-form-err" style="color:var(--seo-red);font-size:13.5px;"></div>' +
            '<button type="submit" class="aeon-btn" style="width:100%;">' + U.escapeHtml(Rana.t('authSignIn')) + '</button>' +
          '</form>';
        document.getElementById('auth-login-form').addEventListener('submit', async function (e) {
          e.preventDefault();
          var errEl = document.getElementById('auth-form-err');
          errEl.textContent = '';
          var fd = new FormData(e.target);
          try {
            var res = await Rana.api.login({
              email: String(fd.get('email') || '').trim(),
              password: String(fd.get('password') || '')
            });
            Rana.api.setTokens(res.accessToken, res.refreshToken);
            await initSession();
            Rana.ui.closeModal();
            Rana.ui.toast(Rana.t('authWelcome', { name: (Rana.state.user && Rana.state.user.firstName) || '' }), 'success');
            if (onSuccess) onSuccess();
            else if (Rana.router) { Rana.ui.renderHeader(); Rana.router.resolve(); }
          } catch (err) {
            errEl.textContent = err.message || Rana.t('opFailed');
          }
        });
      } else if (nextTab === 'register') {
        container.innerHTML =
          '<form id="auth-register-form" class="form-grid">' +
            '<div class="form-grid form-grid--2">' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authFirstName')) + '</label><input type="text" name="firstName" required class="field-input" /></div>' +
              '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authLastName')) + '</label><input type="text" name="lastName" required class="field-input" /></div>' +
            '</div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authEmail')) + '</label><input type="email" name="email" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authPassHint')) + '</label><input type="password" name="password" minlength="8" required class="field-input" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authPhoneOpt')) + '</label><input type="text" name="phone" class="field-input" /></div>' +
            '<div id="auth-form-err" style="color:var(--seo-red);font-size:13.5px;"></div>' +
            '<button type="submit" class="aeon-btn" style="width:100%;">' + U.escapeHtml(Rana.t('authCreateAcct')) + '</button>' +
          '</form>';
        document.getElementById('auth-register-form').addEventListener('submit', async function (e) {
          e.preventDefault();
          var errEl = document.getElementById('auth-form-err');
          errEl.textContent = '';
          var fd = new FormData(e.target);
          try {
            await Rana.api.register({
              firstName: String(fd.get('firstName') || '').trim(),
              lastName: String(fd.get('lastName') || '').trim(),
              email: String(fd.get('email') || '').trim(),
              password: String(fd.get('password') || ''),
              phone: String(fd.get('phone') || '').trim() || null
            });
            Rana.ui.toast(Rana.t('authRegDone'), 'success');
            switchTab('login');
          } catch (err) {
            errEl.textContent = err.message || Rana.t('opFailed');
          }
        });
      } else if (nextTab === 'otp') {
        container.innerHTML =
          '<form id="auth-otp-send" class="form-grid" style="margin-bottom:18px;padding-bottom:18px;border-bottom:1px solid var(--border-hairline);">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authOtpStep1')) + '</label><input type="email" name="email" id="otp-email-input" required class="field-input" placeholder="you@example.com" /></div>' +
            '<button type="submit" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(Rana.t('authOtpSendBtn')) + '</button>' +
          '</form>' +
          '<form id="auth-otp-reset" class="form-grid">' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authOtpStep2')) + '</label><input type="text" name="token" required class="field-input" placeholder="6-digit code" /></div>' +
            '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authOtpNewPass')) + '</label><input type="password" name="newPassword" minlength="8" required class="field-input" /></div>' +
            '<div id="auth-form-err" style="color:var(--seo-red);font-size:13.5px;"></div>' +
            '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('authOtpResetBtn')) + '</button>' +
          '</form>';
        document.getElementById('auth-otp-send').addEventListener('submit', async function (e) {
          e.preventDefault();
          var email = document.getElementById('otp-email-input').value.trim();
          try {
            await Rana.api.sendResetOtp(email);
            Rana.ui.toast(Rana.t('authOtpSentToast'), 'success');
          } catch (err) {
            document.getElementById('auth-form-err').textContent = err.message;
          }
        });
        document.getElementById('auth-otp-reset').addEventListener('submit', async function (e) {
          e.preventDefault();
          var email = document.getElementById('otp-email-input').value.trim();
          var fd = new FormData(e.target);
          try {
            await Rana.api.resetPassword({
              email: email,
              token: String(fd.get('token') || '').trim(),
              newPassword: String(fd.get('newPassword') || '')
            });
            Rana.ui.toast(Rana.t('authPassUpdatedToast'), 'success');
            switchTab('login');
          } catch (err) {
            document.getElementById('auth-form-err').textContent = err.message;
          }
        });
      }
    }

    document.querySelectorAll('[data-auth-tab]').forEach(function (b) {
      b.addEventListener('click', function () { switchTab(b.getAttribute('data-auth-tab')); });
    });
    switchTab(tab);
  }

  function openAccountMenu() {
    var user = Rana.state.user;
    if (!user) return openAuthModal('login');
    var adminHref = 'admin/index.html';

    var html =
      '<h2 class="font-serif" style="margin:0 0 4px;font-size:28px;">' + U.escapeHtml(user.fullName || (user.firstName + ' ' + user.lastName)) + '</h2>' +
      '<p style="margin:0 0 18px;font-family:var(--font-mono);font-size:12px;color:var(--ink-mid);">' + U.escapeHtml(user.email) + ' · ' + U.escapeHtml((user.roleName || []).join(', ')) + '</p>' +
      '<form id="edit-profile-form" class="form-grid">' +
        '<div class="form-grid form-grid--2">' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authFirstName')) + '</label><input type="text" name="firstName" value="' + U.escapeHtml(user.firstName || '') + '" class="field-input" /></div>' +
          '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('authLastName')) + '</label><input type="text" name="lastName" value="' + U.escapeHtml(user.lastName || '') + '" class="field-input" /></div>' +
        '</div>' +
        '<div class="field-group"><label class="field-label">' + U.escapeHtml(Rana.t('acctBio')) + '</label><textarea name="bio" class="field-textarea" style="min-height:80px;">' + U.escapeHtml(user.bio || '') + '</textarea></div>' +
        '<div style="font-size:13px;color:var(--ink-mid);">' + U.escapeHtml(Rana.t('acctGlLabel')) + ' ' +
          (user.communityGuidelinesAcceptedAt
            ? '<strong style="color:#166534;">' + U.escapeHtml(Rana.t('acctGlAccepted', { date: U.formatDate(user.communityGuidelinesAcceptedAt) })) + '</strong>'
            : '<button type="button" id="acct-accept-gl" style="text-decoration:underline;color:var(--aeon-red);">' + U.escapeHtml(Rana.t('acctGlAcceptNow')) + '</button>') +
        '</div>' +
        '<div style="display:flex;flex-wrap:wrap;gap:10px;margin-top:8px;">' +
          '<button type="submit" class="aeon-btn">' + U.escapeHtml(Rana.t('acctSaveProfile')) + '</button>' +
          (hasStaffRole() ? '<a href="' + adminHref + '" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(Rana.t('acctOpenStudio')) + '</a>' : '') +
          '<button type="button" id="acct-logout-btn" class="aeon-btn aeon-btn--ghost" style="margin-left:auto;">' + U.escapeHtml(Rana.t('acctSignOut')) + '</button>' +
        '</div>' +
      '</form>';

    Rana.ui.openModal(html, '520px');

    var acceptBtn = document.getElementById('acct-accept-gl');
    if (acceptBtn) {
      acceptBtn.addEventListener('click', async function () {
        await acceptGuidelines();
        openAccountMenu();
      });
    }
    document.getElementById('acct-logout-btn').addEventListener('click', logout);
    document.getElementById('edit-profile-form').addEventListener('submit', async function (e) {
      e.preventDefault();
      var fd = new FormData(e.target);
      try {
        var updated = await Rana.api.editUser({
          firstName: String(fd.get('firstName') || '').trim(),
          lastName: String(fd.get('lastName') || '').trim(),
          bio: String(fd.get('bio') || '').trim()
        });
        Rana.state.user = updated;
        Rana.ui.renderHeader();
        Rana.ui.closeModal();
        Rana.ui.toast(Rana.t('acctUpdated'), 'success');
      } catch (err) {
        Rana.ui.toast(err.message, 'warn');
      }
    });
  }

  async function acceptGuidelines() {
    if (!Rana.state.user) {
      openAuthModal('login');
      return false;
    }
    try {
      var res = await Rana.api.acceptGuidelines();
      Rana.state.user.communityGuidelinesAcceptedAt = (res && res.acceptedAt) || new Date().toISOString();
      Rana.ui.toast('Community Guidelines təsdiqləndi.', 'success');
      return true;
    } catch (err) {
      Rana.ui.toast(err.message || 'Təsdiq zamanı xəta baş verdi.', 'warn');
      return false;
    }
  }

  async function openNotificationsModal() {
    Rana.ui.openModal('<h3 class="font-serif" style="margin:0 0 16px;font-size:26px;">' + U.escapeHtml(Rana.t('notifTitle')) + '</h3><div id="notif-list">' + U.escapeHtml(Rana.t('loading')) + '</div>', '520px');
    try {
      var items = await Rana.api.getNotifications(30);
      var box = document.getElementById('notif-list');
      if (!box) return;
      if (!items || !items.length) {
        box.innerHTML = '<p style="color:var(--ink-mid);">' + U.escapeHtml(Rana.t('notifEmpty')) + '</p>';
        return;
      }
      box.innerHTML =
        '<div style="display:flex;justify-content:flex-end;margin-bottom:12px;">' +
          '<button type="button" id="mark-all-notifs" class="aeon-btn aeon-btn--secondary">' + U.escapeHtml(Rana.t('notifMarkAll')) + '</button>' +
        '</div>' +
        items.map(function (n) {
          return '<div style="padding:12px 0;border-bottom:1px solid var(--border-hairline);opacity:' + (n.isRead ? '0.62' : '1') + ';">' +
            '<div style="font-weight:600;font-size:15px;">' + U.escapeHtml(n.title) + '</div>' +
            '<div style="font-size:14px;color:var(--ink-dark);margin:4px 0;">' + U.escapeHtml(n.message) + '</div>' +
            '<div style="font-family:var(--font-mono);font-size:11px;color:var(--ink-mid);">' + U.formatDateTime(n.createdAt) + '</div>' +
          '</div>';
        }).join('');
      document.getElementById('mark-all-notifs').addEventListener('click', async function () {
        await Rana.api.markAllNotificationsRead();
        if (Rana.state.user) Rana.state.user.unreadNotificationsCount = 0;
        Rana.ui.renderHeader();
        openNotificationsModal();
      });
    } catch (err) {
      var b = document.getElementById('notif-list');
      if (b) b.innerHTML = '<p style="color:var(--seo-red);">' + U.escapeHtml(err.message) + '</p>';
    }
  }

  Rana.auth = {
    initSession: initSession,
    hasRole: hasRole,
    hasStaffRole: hasStaffRole,
    logout: logout,
    openAuthModal: openAuthModal,
    openAccountMenu: openAccountMenu,
    acceptGuidelines: acceptGuidelines,
    openNotificationsModal: openNotificationsModal
  };
})(window);
