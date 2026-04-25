# SpendWise — Claude Project Context

## Project Overview
**SpendWise** is a personal expense tracker web application.

- **Framework**: ASP.NET Core MVC on .NET 10
- **Language**: C# with nullable reference types and implicit usings enabled
- **Database**: SQL Server (SQLEXPRESS) via Entity Framework Core 10
- **Frontend**: Bootstrap 5.3.3 (CDN/local), jQuery 3.7.1, vanilla JS — no npm, no build pipeline
- **Auth**: Not yet configured (planned: ASP.NET Core Identity)

---

## Architecture

```
spendwise/
├── Controllers/       # MVC controllers (one per feature domain)
├── Data/              # EF Core DbContext (AppDbContext)
├── Models/            # Domain entities + ViewModels
├── Views/             # Razor views, organized by controller name
│   └── Shared/        # _Layout.cshtml, partials, error page
├── Migrations/        # EF Core migrations (never edit manually)
└── wwwroot/           # Static assets (css/, js/, lib/)
```

The app follows standard ASP.NET Core MVC conventions with no custom layers beyond what the framework provides.

---

## Tech Stack Details

### Backend
- ASP.NET Core MVC (not Razor Pages, not Web API)
- Entity Framework Core 10 — code-first, migrations-based
- `AppDbContext` in `spendwise.Data` namespace
- Connection string lives in `appsettings.Development.json` under `ConnectionStrings:DefaultConnection`
- SQL Server Integrated Security (Windows auth) on `localhost\SQLEXPRESS01`, database `spendwise`

### Frontend
- Bootstrap 5.3.3 — utility classes preferred, custom CSS only when Bootstrap can't do it
- Custom styles go in `wwwroot/css/site.css`
- Custom scripts go in `wwwroot/js/site.js`
- jQuery unobtrusive validation is wired up via `_ValidationScriptsPartial.cshtml`
- No TypeScript, no Webpack, no npm — keep it that way unless explicitly asked to change


## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Namespace | Match folder path | `spendwise.Controllers` |
| Classes | PascalCase | `ExpenseController` |
| Methods (actions) | PascalCase | `Index`, `Create` |
| Properties | PascalCase | `CreatedAt`, `Amount` |
| EF DbSet | Plural noun | `Categories`, `Expenses` |
| Migrations | Descriptive snake-case label | `AddExpenseTable` |
| Partial views | Underscore prefix | `_ExpenseRow.cshtml` |
| ViewModels | `<Feature>ViewModel` | `ExpenseSummaryViewModel` |

---

## Development Workflow

### Running the app
```bash
dotnet watch run --project spendwise
HTTP:  http://localhost:5108
```

Use `dotnet watch run` (not `dotnet run`). It enables .NET Hot Reload for C# changes:
- Method body edits apply instantly with no restart
- New controllers / classes trigger an automatic rebuild + restart — no manual stop needed
- Razor Runtime Compilation (`AddRazorRuntimeCompilation`) handles `.cshtml` changes live on top of this

### EF Core migrations
```bash
dotnet ef migrations add <MigrationName> --project spendwise
dotnet ef database update --project spendwise
```

Never edit generated migration files after they have been applied to any database.

### Runtime Razor compilation
`Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` is enabled — view changes are picked up without restart in development.

---

## Code Style

- File-scoped namespaces (`namespace spendwise.Controllers;`)
- No XML doc comments on internal/private members
- Prefer expression-bodied members for simple properties and single-line actions
- Use `async`/`await` for all database calls (EF Core async APIs)
- Validate at the controller boundary via `ModelState.IsValid`; trust EF and framework internals below that
- No unnecessary null-checks on EF navigation properties that the ORM guarantees loaded
- Keep controllers thin — move query logic to the DbContext or a dedicated service class if it grows beyond ~20 lines

---

## What to Avoid

- Do not add npm, webpack, Vite, or any JS build tooling unless asked
- Do not add Dapper or raw ADO.NET alongside EF Core — pick one
- Do not use `[ApiController]` or return `IActionResult` JSON from MVC controllers; keep MVC and API concerns separate
- Do not scaffold Identity until the user asks; do not add `[Authorize]` gates prematurely
- Do not create `.md` documentation files unless explicitly requested
- Do not add comments that describe *what* the code does — only add a comment when the *why* is non-obvious

---

## Reuse Notes (for other ASP.NET Core projects)

When adapting this file for a new project:
1. Update **Project Overview** (name, purpose, auth status)
2. Update **Design Language** (colors, aesthetic)
3. Update the connection string details under **Tech Stack Details**
4. Keep all conventions, workflow commands, and code style rules — they apply to any ASP.NET Core MVC project on this machine
