---
name: frontend-design
description: Designs and generates modern, production-ready UI for CoreTriageAI, a personal expense tracker built on ASP.NET Core MVC with Razor Views, Bootstrap 5 CDN, plain CSS, and vanilla JS. Creates polished fintech-styled pages, components, and layouts consistent with CoreTriageAI's navy (#1e3a5f) and gold (#f5a623) design language. Use this skill whenever the user asks to design, build, create, redesign, improve, or style any CoreTriageAI page, screen, section, or component.
disable-model-invocation: false
---

# CoreTriageAI UI Designer

You are designing frontend UI for **CoreTriageAI**, a personal expense tracker built on ASP.NET Core MVC. Output polished, fintech-styled Razor Views, plain CSS, and vanilla JavaScript that follows CoreTriageAI's navy and gold design language.

## Stack Context

| Layer | Technology | Notes |
|-------|-----------|-------|
| **Backend** | ASP.NET Core MVC | C#, Entity Framework Core |
| **Views** | Razor (`.cshtml`) | Always extend `_Layout.cshtml` |
| **Markup** | HTML 5 | Semantic tags (`<header>`, `<nav>`, `<main>`, `<section>`, `<footer>`) |
| **Styles** | Plain CSS 3 | `wwwroot/css/` — no Tailwind, no SCSS preprocessors |
| **Scripts** | Vanilla JavaScript (ES6+) | `wwwroot/js/` — no jQuery, no React/Vue/Angular |
| **UI Framework** | Bootstrap 5.3 CDN | Use utilities, override defaults with `.sw-` prefix |
| **Icons** | Lucide Icons CDN | `<i data-lucide="icon-name"></i>` |
| **Charts** | Chart.js CDN | For dashboards and data visualization |
| **Font** | Poppins (Google Fonts CDN) | 400 (body), 500 (UI), 600 (subheadings), 700 (headings) |

## Pre-Design Checklist

Before generating new UI, always:

1. **Review Existing Conventions**
   - Check `wwwroot/css/` for existing color tokens, spacing scales, and component patterns
   - Look for `.sw-` prefixed classes already in use
   - Identify established component library (buttons, cards, forms, tables)

2. **Verify Color Palette**
   - Use project's defined primary color (usually navy `#1e3a5f`)
   - Use project's defined accent color (usually gold `#f5a623`)
   - Check for semantic colors (success, error, info, warning)

3. **Check Responsive Breakpoints**
   - Confirm mobile breakpoints (usually < 640px, 640–1024px, ≥ 1024px)
   - Verify sidebar collapse behavior on tablet/mobile
   - Test grid layout assumptions

4. **Examine Existing Pages**
   - Look at `_Layout.cshtml` for header, footer, sidebar structure
   - Check existing views for Razor patterns (Tag Helpers, validation, auth checks)
   - Identify form validation approach

## Design Language Defaults

### Color Palette
- **Primary Background:** `#1e3a5f` (deep navy) — headers, footers, sidebars, hero sections
- **Secondary Background:** `#f7f8fa` (light gray) — page backgrounds, content areas
- **Surface:** `#ffffff` (white) — cards, modals, forms
- **Accent:** `#f5a623` (gold) — buttons, active states, highlights, stat numbers
- **Text Primary:** `#111827` (dark gray) — body text, headings
- **Text Secondary:** `#6b7280` (medium gray) — labels, helper text, muted info
- **Border:** `#e5e7eb` (light gray) — dividers, card borders
- **Success:** `#10b981` (emerald) — confirmations, positive indicators
- **Error:** `#ef4444` (red) — warnings, destructive actions, validation errors
- **Info:** `#3b82f6` (blue) — informational messages
- **Warning:** `#f59e0b` (amber) — caution, pending states

### Typography
- **Font Family:** Poppins (Google Fonts CDN)
- **Body:** 400 weight, 16px base, 1.5 line-height
- **Headings:** 600 weight (h4–h6) or 700 weight (h1–h3)
- **UI Text:** 500 weight (buttons, labels, navigation)
- **Size Scale:** 12px, 14px, 16px, 18px, 20px, 24px, 32px, 40px (no arbitrary sizes)
- **Currency Display:** Always `font-variant-numeric: tabular-nums`, prefix with "PKR " or project currency

### Spacing & Layout
- **Grid Unit:** 8px baseline — use multiples only (4px, 8px, 16px, 24px, 32px, 40px, 48px, 56px)
- **Card Padding:** 24px
- **Input Height:** 40px (including padding)
- **Button Padding:** 10px 24px (primary), 8px 22px (secondary)
- **Border Radius:** 12px (cards, large buttons), 8px (inputs, smaller buttons), 4px (very small)
- **Gap Between Elements:** 16px–24px (consistent spacing)

### Component Defaults

**Buttons:**
- Primary (gold, filled): `background: #f5a623`, `color: white`, hover: `#e09517`
- Secondary (navy, outline): `border: 2px solid #1e3a5f`, `color: #1e3a5f`, hover: navy background
- Danger (red, filled): `background: #ef4444`, `color: white`, hover: `#dc2626`

**Cards:**
- White background, `border: 1px solid #e5e7eb`, `border-radius: 12px`
- Padding: 24px
- Shadow: `0 1px 3px rgba(0, 0, 0, 0.05)`, hover shadow: `0 4px 6px rgba(0, 0, 0, 0.08)`

**Forms:**
- Input height: 40px, padding: 10px 12px, border: `1px solid #e5e7eb`
- Focus state: border gold, shadow `0 0 0 3px rgba(245, 166, 35, 0.1)`
- Invalid state: border red `#ef4444`
- Labels: 500 weight, 14px, margin-bottom: 6px

**Tables:**
- Header background: `#f7f8fa`, border-bottom: `2px solid #e5e7eb`
- Row padding: 12px 16px, border-bottom: `1px solid #e5e7eb`
- Hover state: background `#f9fafb`
- Responsive: use `overflow-x: auto` wrapper on mobile

**Alerts & Toasts:**
- Padding: 12px 16px, border-radius: 8px, border-left: 4px solid
- Success: bg `#ecfdf5`, text `#065f46`, border `#10b981`
- Error: bg `#fef2f2`, text `#7f1d1d`, border `#ef4444`
- Info: bg `#eff6ff`, text `#1e40af`, border `#3b82f6`

### Layout Architecture

**Public Pages (Home, Login, Register, Terms, Privacy):**
- Sticky navy header with logo (gold text) and nav
- Full-width hero section with navy background, centered white content, gold CTA
- Light gray content areas with white cards
- Navy footer with centered links

**Authenticated Pages (Dashboard, Admin, Settings):**
- Sticky navy header with logo and user menu
- Left sidebar (280px, navy background, gold active state) — collapses to offcanvas on mobile
- Main content area with light gray background
- White cards/components with consistent spacing

**Responsive Breakpoints:**
- Mobile: < 640px (vertical stack, full-width, hamburger nav)
- Tablet: 640px–1024px (sidebar collapses to offcanvas)
- Desktop: ≥ 1024px (sidebar visible, full layout)


## Anti-Patterns ❌

**Avoid:**
- Default Bootstrap blue anywhere — override all primary colors with navy/gold
- Hardcoded URLs — always use `asp-controller`, `asp-action`, `asp-for` Tag Helpers
- Arbitrary spacing (not multiples of 4px or 8px)
- More than one accent color — gold only for CTAs and highlights
- Icons without semantic meaning — don't sprinkle icons randomly
- Unstyled form inputs — all use `.sw-input`, `.sw-select`, `.sw-textarea`
- Tables without responsive wrapper — always `overflow-x: auto` on mobile
- Mobile-last design — design responsive from the start
- `@Html.BeginForm()` — use `<form asp-*>` Tag Helpers only
- Inline styles — all styles in `wwwroot/css/`
- jQuery or heavy JS frameworks — vanilla ES6+ only

## Ambiguity Handling

When requirements are vague, make **reasonable assumptions upfront** rather than over-questioning:

- **Color not specified?** Use navy for backgrounds, gold for CTAs
- **Layout not specified?** Use card-based layout with 24px padding
- **Spacing not specified?** Use 16px–24px gaps between major sections
- **Mobile not mentioned?** Design responsive (mobile < 640px, desktop ≥ 1024px)
- **Validation not mentioned?** Include field-level validation with `.sw-invalid-feedback`
- **Icons not mentioned?** Use Lucide icons for nav items, buttons, and section headings
- **No form structure?** Use `.sw-form-group`, `.sw-label`, `.sw-input` stack

State your assumptions in the **UI Plan** so the user can correct if needed.

## CSS Class Reference

| Class | Purpose | Example Use |
|-------|---------|-------------|
| `.sw-container` | Main page wrapper | Page layout container |
| `.sw-header` | Sticky navigation bar | Top nav, 64px height |
| `.sw-sidebar` | Left navigation sidebar | 280px on desktop, collapses on mobile |
| `.sw-content` | Main content area | Primary page content |
| `.sw-footer` | Page footer | Bottom nav, copyright |
| `.sw-card` | Content card | White, bordered, shadowed |
| `.sw-btn-primary` | Gold filled button | CTA, form submit |
| `.sw-btn-secondary` | Navy outline button | Cancel, back, secondary actions |
| `.sw-btn-danger` | Red filled button | Delete, destructive actions |
| `.sw-form-group` | Form field wrapper | Input + label + validation |
| `.sw-input` | Text input | Text, email, date, etc. |
| `.sw-select` | Dropdown select | Category, status, etc. |
| `.sw-textarea` | Multiline input | Description, notes, etc. |
| `.sw-table` | Data table | Expenses, transactions, etc. |
| `.sw-table.responsive` | Responsive table wrapper | Mobile: `overflow-x: auto` |
| `.sw-alert` | Alert/notification box | Error, success, info messages |
| `.sw-alert-success` | Success alert | Green background |
| `.sw-alert-error` | Error alert | Red background |
| `.sw-alert-info` | Info alert | Blue background |
| `.sw-toast` | Temporary notification | Auto-hide notification |
| `.sw-stat-card` | Dashboard stat display | Large number cards |
| `.sw-stat-value` | Large currency number | Gold text, tabular-nums |
| `.sw-stat-label` | Stat label text | Gray, uppercase |
| `.sw-label` | Form label | 500 weight, 14px |
| `.sw-invalid-feedback` | Validation error text | Red, 12px |

## CDN Dependencies

Place in `_Layout.cshtml` `<head>`:

```html
<!-- Google Fonts - Poppins -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap" rel="stylesheet">

<!-- Bootstrap 5.3 CSS -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">

<!-- Lucide Icons -->
<script src="https://cdn.jsdelivr.net/npm/lucide@latest"></script>

<!-- Chart.js (optional, for dashboards) -->
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
```

Place before closing `</body>`:

```html
<!-- Bootstrap JS Bundle -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

<!-- Initialize Lucide Icons -->
<script>
  document.addEventListener('DOMContentLoaded', () => {
    lucide.createIcons();
  });
</script>
```

## Quick Start Checklist

When starting a new ASP.NET MVC UI project:

- [ ] Review `wwwroot/css/` for existing component library
- [ ] Check `_Layout.cshtml` for header, footer, sidebar structure
- [ ] Verify color palette matches project standard (navy + gold)
- [ ] Confirm responsive breakpoints
- [ ] Create new CSS file in `wwwroot/css/{feature}.css`
- [ ] Use `.sw-` prefix for all custom classes
- [ ] Always use Razor Tag Helpers for forms and links
- [ ] Include validation error states
- [ ] Test responsive layout (mobile, tablet, desktop)
- [ ] Initialize Lucide icons in JavaScript
