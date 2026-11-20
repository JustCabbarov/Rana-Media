# Rana Media — Standalone Frontend Project

Pure **HTML5 + CSS3 + Modular Vanilla JS** frontend for **Rana Media** (Aeon-inspired editorial layout in a green & forest palette), decoupled from the `.NET 8` Web API backend.

## Structure

- `index.html` — Public Editorial Magazine (`Essays`, `Videos`, `Sections`, `Topics`, `PageMarks`, `Comments`, `My Library`, `Newsletter`, `Pitch`, `Syndication`)
- `admin/index.html` — Editorial Admin Studio & real-time Yoast-style SEO Analysis panel
- `css/tokens.css`, `css/aeon.css`, `css/admin.css` — Green editorial design system tokens & layouts
- `js/*.js` — API client, router, auth/OTP, public views, and admin studio
- `assets/images/` — Curated local editorial SVG artworks (green palette)

## Running the Frontend

Start the `.NET 8` backend API first (e.g. `http://localhost:5000`):

```powershell
dotnet run --project "../src/RanaMedia.Presentation/RanaMedia.Presentation.csproj"
```

Then serve `RanaMedia.Frontend` on its own port (`3000` or Live Server):

```powershell
# Option 1 (Node):
npx serve -l 3000 .

# Option 2 (Python):
python -m http.server 3000
```

Or use VS Code **Live Server**, or open `index.html` directly in your browser.

## URLs & Backend API Configuration

- **Public Magazine:** `http://localhost:3000/`
- **Admin Studio & SEO:** `http://localhost:3000/admin/index.html`
- **Backend API Base:** Defaults to `http://localhost:5000`. You can override it anytime via:
  - Query string: `?api=http://localhost:5050` (persisted in `localStorage.rana_api_base`)
  - Console / Browser: `localStorage.setItem('rana_api_base', 'http://localhost:5050')`
  - Or via the **Change API** / **API URL dəyiş** control in the Admin sign-in card or offline banner.
