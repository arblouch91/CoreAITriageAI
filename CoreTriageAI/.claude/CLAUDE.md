---
description: 
alwaysApply: true
---

# CoreTriageAI — Claude Project Context

## Project Overview
**CoreTriageAI** is an AI-powered complaint management system for a bank. Customers submit complaints that are automatically analyzed and triaged by an LLM (via OpenRouter) for department routing, priority classification, and sentiment analysis. Staff manage complaints through a dashboard with filtering, sorting, and pagination.

- **Framework**: ASP.NET Core MVC on .NET 10
- **Language**: C# with nullable reference types and implicit usings enabled
- **Database**: SQL Server (SQLEXPRESS) via Entity Framework Core 10
- **Frontend**: Bootstrap 5.3.0 (CDN), Lucide Icons (CDN), Poppins (Google Fonts CDN), vanilla JS — no npm, no build pipeline
- **AI Integration**: OpenRouter LLM API (system prompt enforces JSON response)
- **Auth**: Not yet configured

---

## Architecture

```
CoreTriageAI/
├── Controllers/          # MVC controllers (one per feature domain)
├── Data/                 # EF Core DbContext (AppDbContext)
├── Models/               # Domain entities + ViewModels
├── Services/             # LLM integration (OpenRouterService)
├── Views/                # Razor views, organized by controller name
│   └── Shared/           # _Layout.cshtml, partials, error page
├── Migrations/           # EF Core migrations (never edit manually)
└── wwwroot/              # Static assets (css/, js/, lib/)
```

The app follows standard ASP.NET Core MVC conventions with a thin service layer for AI calls.

---

## Tech Stack Details

### Backend
- ASP.NET Core MVC (not Razor Pages, not Web API)
- Entity Framework Core 10 — code-first, migrations-based
- `AppDbContext` in `CoreTriageAI.Data` namespace
- Connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`
- SQL Server Integrated Security (Windows auth) on `localhost\SQLEXPRESS01`, database `CoreTriageAI`
- `OpenRouterService` in `CoreTriageAI.Services` — calls OpenRouter LLM, strips markdown fences, deserializes JSON response
- OpenRouter API key stored in `appsettings.json` under `OpenRouterApiKey`

### Frontend
- Bootstrap 5.3.0 — utility classes preferred, custom CSS only when Bootstrap can't do it
- Custom styles go in `wwwroot/css/site.css`
- Custom scripts go in `wwwroot/js/site.js`
- Lucide Icons CDN: `<i data-lucide="icon-name"></i>`, initialized via `lucide.createIcons()`
- Poppins font (400, 500, 600, 700) via Google Fonts CDN
- No TypeScript, no Webpack, no npm — keep it that way unless explicitly asked to change

---

## Domain Model

### `Complain` Entity (`CoreTriageAI.Models`)
| Property | Type | Notes |
|---|---|---|
| `Id` | int | PK, auto-increment |
| `Name` | string | Customer name, required |
| `Email` | string | Customer email, required |
| `PhoneNumber` | string | Customer phone, required |
| `ComplainText` | string | Full complaint text, required |
| `Department` | string | AI-assigned department routing, required |
| `Priority` | string | `"low"` / `"medium"` / `"high"`, AI-assigned |
| `SentimentScore` | decimal? | 0.00–1.00, AI-assigned, precision (5,2) |
| `SentimentLabel` | string? | `"Extremely Negative"` / `"Negative"` / `"Neutral"` / `"Positive"` |
| `CreatedAt` | DateTime | Server default: `GETUTCDATE()` |

### `ComplainListViewModel` (`CoreTriageAI.Models`)
Contains: complaints list, pagination info, active filters, aggregate stats (total count, high-priority count, average sentiment).

---

## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Namespace | Match folder path | `CoreTriageAI.Controllers` |
| Classes | PascalCase | `ComplainController` |
| Methods (actions) | PascalCase | `Index`, `Submit`, `List` |
| Properties | PascalCase | `CreatedAt`, `SentimentScore` |
| EF DbSet | Plural noun | `Complains` |
| Migrations | Descriptive snake-case label | `CreateComplainsTable` |
| Partial views | Underscore prefix | `_ValidationScriptsPartial.cshtml` |
| ViewModels | `<Feature>ViewModel` | `ComplainListViewModel` |
| CSS classes | `.sw-` prefix | `.sw-card`, `.sw-btn-primary` |

---

## Development Workflow

### Running the app
```bash
dotnet watch run --project CoreTriageAI
HTTP:  http://localhost:5070
HTTPS: https://localhost:7010
```

Use `dotnet watch run`. It enables .NET Hot Reload:
- Method body edits apply instantly with no restart
- New controllers / classes trigger an automatic rebuild + restart
- Razor Runtime Compilation handles `.cshtml` changes live

### EF Core migrations
```bash
dotnet ef migrations add <MigrationName> --project CoreTriageAI
dotnet ef database update --project CoreTriageAI
```

Never edit generated migration files after they have been applied to any database.

---

## Code Style

- File-scoped namespaces (`namespace CoreTriageAI.Controllers;`)
- No XML doc comments on internal/private members
- Prefer expression-bodied members for simple properties and single-line actions
- Use `async`/`await` for all database calls (EF Core async APIs)
- Validate at the controller boundary via `ModelState.IsValid`; trust EF and framework internals below that
- No unnecessary null-checks on EF navigation properties that the ORM guarantees loaded
- Keep controllers thin — move logic to `Services/` if it grows beyond ~20 lines
- LLM response parsing lives in `OpenRouterService`; controllers only call the service and persist the result

---

## Hackathon Context

**This is a hackathon project.** 4 team members, 5–7 hour time limit. The goal is a working MVP prototype — not production-grade software. Every decision should favor shipping a demo-ready prototype over polish, abstractions, or future-proofing.

### Problem Statement — C-04: Complaint Triage & Intelligent Auto-Response Bot
> AI that classifies, prioritizes, and drafts professional responses to customer complaints instantly.

Banks receive thousands of customer complaints across email, web forms, mobile apps, and social media. Manually reading, classifying, and routing each complaint creates backlogs and delays resolution — damaging CSAT scores and regulatory standing.

**Objective:** Build a complaint triage tool that ingests mock customer complaints, classifies and prioritizes them using AI, routes them to the correct department, and auto-drafts a professional first response.

### Must-Deliver (MVP Core)
1. **Complaint Intake** — paste/type a complaint, or pick from a pre-loaded queue of 50 mock cases
2. **AI Classification** — Category (Fraud, Fees, Service Downtime, Card Issue, etc.), Priority (High/Medium/Low), Sentiment score
3. **Auto-Routing** — displays the correct handling team based on category
4. **Drafted Response** — professional, empathetic, personalized reply auto-generated and ready to send with one click
5. **Queue Dashboard** — all complaints sortable by priority, category, and status

### Example Routing (from problem brief)
| Complaint | Category | Priority | Team |
|---|---|---|---|
| "Charged twice for the same transaction" | Fraud/Billing | High | Billing Team |
| "Mobile app crashed during bill payment" | Service | Medium | Digital Banking Support |
| "Account blocked without notice" | Account Management | High | Relationship Management |
| "Waiting 3 weeks for my debit card" | Card Services | Medium | Card Operations |

### MVP-First Rules (hackathon mode)
- **Working > perfect.** A rough UI that works beats a polished UI that crashes.
- **Cut scope aggressively.** If a feature is not in the Must-Deliver list above, skip it.
- **No over-engineering.** No service abstractions, no repository patterns, no unit tests — controllers can be thicker than normal during the hackathon.
- **Hardcode when necessary.** Mock data, hardcoded lists, in-memory state — acceptable for demo purposes.
- **One LLM call per complaint** — classify + route + draft response in a single prompt to save time and API latency.
- **No auth, no multi-tenancy, no roles** — single shared view, no login required.
- **Pre-seed the database** with a few sample complaints so the dashboard looks populated on first load.
- **No migrations for schema changes** — drop and recreate the database if schema needs to change quickly.
- When in doubt, ask: *"Can we demo this in the next 10 minutes?"* If yes, ship it. If no, simplify.

---

## What to Avoid

- Do not add npm, webpack, Vite, or any JS build tooling unless asked
- Do not add Dapper or raw ADO.NET alongside EF Core — pick one
- Do not use `[ApiController]` or return `IActionResult` JSON from MVC controllers
- Do not scaffold Identity until asked; do not add `[Authorize]` gates prematurely
- Do not create `.md` documentation files unless explicitly requested
- Do not add comments that describe *what* the code does — only add a comment when the *why* is non-obvious
- Do not inline OpenRouter/LLM calls in controllers — keep them in `OpenRouterService`
