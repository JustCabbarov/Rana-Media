/* ==========================================================================
   Rana Media Public Magazine Bootstrap
   ========================================================================== */
(function (global) {
  'use strict';

  document.addEventListener('DOMContentLoaded', async function () {
    var Rana = global.Rana;
    document.documentElement.lang = Rana.getLang();

    function renderChrome(headerOpts) {
      Rana.ui.renderOfflineBanner();
      Rana.ui.renderHeader(headerOpts);
      Rana.ui.renderMegaMenu();
      Rana.ui.renderSearchOverlay();
      Rana.ui.renderFooter();
    }

    Rana.onLangChange = function () {
      renderChrome();
      if (Rana.router) Rana.router.resolve();
    };

    renderChrome();
    await Rana.auth.initSession();
    Rana.ui.renderHeader();
    await Rana.router.resolve();

    global.addEventListener('hashchange', function () {
      Rana.ui.toggleMegaMenu(false);
      Rana.ui.toggleSearchOverlay(false);
      Rana.router.resolve();
    });

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') {
        Rana.ui.toggleMegaMenu(false);
        Rana.ui.toggleSearchOverlay(false);
        Rana.ui.closeModal();
      }
    });
  });
})(window);
