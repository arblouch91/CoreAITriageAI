# CoreTriageAI

An AI-powered customer complaint triage system for banking institutions. Customers submit complaints through a web form, and the system automatically classifies them, assigns priority, routes them to the appropriate department, analyzes sentiment, and drafts a professional response — all in a single AI call.

---

## Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [API Keys](#api-keys)
- [Database Setup](#database-setup)
- [Running the Project](#running-the-project)
- [How It Works](#how-it-works)
- [AI Response Format](#ai-response-format)
- [Screenshots](#screenshots)

---

## Overview

Banks receive thousands of customer complaints across multiple channels, creating backlogs and delaying resolution. CoreTriageAI solves this by automatically triaging complaints using a Large Language Model (LLM) via [OpenRouter](https://openrouter.ai), then storing and displaying them in a staff-facing dashboard.

**Built as a hackathon MVP** (4 team members, 5–7 hour sprint) — production polish intentionally deferred in favour of working functionality.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC (.NET 10) |
| Language | C# |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (Express or Standard) |
| AI Provider | OpenRouter API |
| Frontend | Bootstrap 5.3, Lucide Icons, Poppins (Google Fonts) |
| Styling | Custom CSS with `.sw-` design system |

No frontend build pipeline — all external libraries load from CDN.

---

## Features

- **Complaint Submission** — customer form collects name, email, phone, and complaint text
- **AI Triage** — single LLM call classifies, routes, prioritises, and scores sentiment
- **Auto-Drafted Response** — LLM generates a personalised, empathetic reply to the customer
- **Staff Dashboard** — filterable, sortable complaint queue with pagination
- **Statistics** — total complaints, high-priority count, average sentiment score
- **Status Tracking** — Open → In Progress → Resolved

**Classification options used by the AI:**

| Field | Options |
|-------|---------|
| Category | Fraud, Fees, Service Downtime, Card Issue |
| Department | Fraud / Billing Team, Digital Banking Support, Relationship Management, Card Operations |
| Priority | Low, Medium, High |
| Sentiment | Extremely Negative, Negative, Neutral, Positive |

---

## Prerequisites

Before running the project you need:

1. **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** — the runtime and CLI tools
2. **SQL Server** — Express edition is sufficient. Download [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) if you don't have it
3. **SQL Server Management Studio (SSMS)** *(optional)* — useful for inspecting the database
4. **An OpenRouter API Key** — see [API Keys](#api-keys) below
5. **Git** — to clone the repository

---

## Project Structure

```
CoreAITriageAI/
└── CoreTriageAI/                  # Main ASP.NET Core project
    ├── Controllers/
    │   ├── ComplainController.cs  # Complaint submission and list endpoints
    │   └── HomeController.cs      # Dashboard
    ├── Data/
    │   └── AppDbContext.cs        # EF Core DbContext
    ├── Models/
    │   ├── Complain.cs            # Core domain entity
    │   └── ComplainListViewModel.cs
    ├── Services/
    │   └── OpenRouterService.cs   # LLM API integration
    ├── Views/
    │   ├── Complain/              # Submission form and list view
    │   ├── Home/                  # Dashboard
    │   └── Shared/                # Layout, error pages
    ├── Migrations/                # EF Core auto-generated migrations
    ├── wwwroot/                   # Static assets (CSS, JS, libraries)
    ├── appsettings.json           # Main configuration file
    ├── appsettings.Development.json
    └── Program.cs                 # App startup and DI registration
```

---

## Configuration

All configuration lives in [CoreTriageAI/appsettings.json](CoreTriageAI/appsettings.json).

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=CoreTriageAI;Integrated Security=True;TrustServerCertificate=True;Application Name=CoreTriageAI"
  },
  "OpenRouterApiKey": "YOUR_OPENROUTER_API_KEY_HERE",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Connection String

The default connection string targets a local SQL Server Express instance named `SQLEXPRESS01` using Windows Integrated Security (trusted connection — no username/password needed).

**If your SQL Server instance has a different name**, update the `Data Source` value:

| Instance | `Data Source` value |
|----------|---------------------|
| SQL Server Express (default) | `localhost\SQLEXPRESS` |
| SQL Server Express with custom name | `localhost\SQLEXPRESS01` |
| Full SQL Server (default instance) | `localhost` |
| Named instance | `localhost\INSTANCE_NAME` |

**If you need SQL authentication** (username + password instead of Windows auth), replace the connection string with:

```
Data Source=YOUR_SERVER;Initial Catalog=CoreTriageAI;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;
```

---

## API Keys

### OpenRouter API Key

The project uses [OpenRouter](https://openrouter.ai) as its LLM provider. OpenRouter is an aggregator that gives access to dozens of AI models (GPT-4, Claude, Llama, Mistral, etc.) through a single API.

#### How to get your API Key

1. Go to [https://openrouter.ai](https://openrouter.ai)
2. Click **Sign Up** and create a free account (GitHub or Google login supported)
3. After logging in, navigate to **[Keys](https://openrouter.ai/keys)** in your account settings
4. Click **Create Key**, give it a name (e.g. `CoreTriageAI`), and click **Create**
5. Copy the generated key — it starts with `sk-or-v1-`

> **Note:** OpenRouter offers a free tier with limited credits. For heavier usage, add credits in the [Credits](https://openrouter.ai/credits) section.

#### How to add the key to the project

Open [CoreTriageAI/appsettings.json](CoreTriageAI/appsettings.json) and replace the value for `OpenRouterApiKey`:

```json
{
  "OpenRouterApiKey": "sk-or-v1-YOUR_ACTUAL_KEY_HERE"
}
```

#### Model used

The project sends requests to:

```
POST https://openrouter.ai/api/v1/chat/completions
Model: openrouter/auto
```

`openrouter/auto` lets OpenRouter automatically select the best available model for the prompt. You can change this to a specific model (e.g. `anthropic/claude-3-haiku`, `openai/gpt-4o-mini`) by editing [CoreTriageAI/Services/OpenRouterService.cs](CoreTriageAI/Services/OpenRouterService.cs).

> **Security:** Never commit your actual API key to version control. For production use, store the key in an environment variable or a secrets manager and reference it from `appsettings.json` using environment variable substitution.

---

## Database Setup

The project uses **Entity Framework Core code-first migrations**. The database and all tables are created automatically when the application starts — no manual SQL scripts required.

### Automatic (default behaviour)

`Program.cs` calls `db.Database.Migrate()` on startup, which:
1. Creates the `CoreTriageAI` database if it doesn't exist
2. Applies all pending migrations in order

Just ensure your SQL Server instance is running and the connection string is correct.

### Manual migration commands

If you need to run migrations manually from the terminal:

```bash
# Apply all pending migrations
dotnet ef database update --project CoreTriageAI

# Create a new migration (after changing a model)
dotnet ef migrations add <MigrationName> --project CoreTriageAI

# Roll back to a specific migration
dotnet ef database update <MigrationName> --project CoreTriageAI
```

### Database schema

**Table: `Complains`**

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| `Id` | int | Yes | Primary key, auto-increment |
| `Name` | nvarchar(max) | Yes | Customer name |
| `Email` | nvarchar(max) | Yes | Customer email |
| `PhoneNumber` | nvarchar(max) | Yes | Customer phone number |
| `ComplainText` | nvarchar(max) | Yes | Full complaint text |
| `Department` | nvarchar(max) | Yes | AI-assigned department |
| `Priority` | nvarchar(max) | Yes | AI-assigned priority (low/medium/high) |
| `Category` | nvarchar(max) | No | AI-assigned category |
| `SentimentScore` | decimal(5,2) | No | Sentiment score 0.00–1.00 |
| `SentimentLabel` | nvarchar(max) | No | Sentiment description |
| `AIDraftedResponse` | nvarchar(max) | No | Auto-generated customer response |
| `Status` | nvarchar(max) | Yes | Open / In Progress / Resolved (default: Open) |
| `CreatedAt` | datetime2 | Yes | UTC timestamp, set by SQL Server default |

---

## Running the Project

### 1. Clone the repository

```bash
git clone <repository-url>
cd CoreAITriageAI
```

### 2. Configure the application

Edit [CoreTriageAI/appsettings.json](CoreTriageAI/appsettings.json):
- Set `OpenRouterApiKey` to your key from OpenRouter
- Update `ConnectionStrings.DefaultConnection` if your SQL Server instance name differs from `SQLEXPRESS01`

### 3. Start the application

```bash
dotnet watch run --project CoreTriageAI
```

`dotnet watch` enables hot reload so changes to `.cs` and `.cshtml` files take effect without restarting.

Alternatively, without hot reload:

```bash
dotnet run --project CoreTriageAI
```

### 4. Open in browser

The app runs on:

| Protocol | URL |
|----------|-----|
| HTTP | http://localhost:5070 |
| HTTPS | https://localhost:7010 |

The browser should open automatically. The default route lands on the **Complaint Submission** form at `/Complain`.

---

## How It Works

### Complaint submission flow

```
Customer fills form
        ↓
POST /Complain/Submit
        ↓
OpenRouterService.AnalyzeAsync()
  - Builds prompt with complaint text + available categories/departments
  - POSTs to OpenRouter API
  - Parses JSON response
        ↓
Save to SQL Server (Complains table)
        ↓
Return JSON result to browser (AJAX)
        ↓
Show success alert with AI analysis summary
```

### Dashboard flow

```
GET /Home/Index  (or /Complain/List)
        ↓
Query Complains table with filters:
  Department, Priority, Category, Status
        ↓
Sort by: Date, Priority, Category, Status
        ↓
Paginate (10 per page)
        ↓
Render dashboard with stats + complaint list
```

---

## AI Response Format

The LLM is instructed to return a strict JSON object. The `OpenRouterService` strips any markdown code fences before deserialising:

```json
{
  "category": "Fraud | Fees | Service Downtime | Card Issue",
  "department": "Fraud / Billing Team | Digital Banking Support | Relationship Management | Card Operations",
  "priority": "low | medium | high",
  "sentiments_score": 0.00,
  "sentiments_label": "Extremely Negative | Negative | Neutral | Positive",
  "ai_drafted_response": "Dear [Customer Name], ..."
}
```

`sentiments_score` is a float between `0.00` (most negative) and `1.00` (most positive).

---

## Development Notes

- **No authentication** is implemented — any user can access all complaints. Add ASP.NET Core Identity if deploying beyond a local/trusted network.
- **Auto-migrations on startup** — suitable for development. For production, run migrations as a separate deployment step.
- **Hardcoded categories and departments** — defined in the AI prompt inside `OpenRouterService.cs`. Update that prompt to add/remove options.
- **Port numbers** are defined in [CoreTriageAI/Properties/launchSettings.json](CoreTriageAI/Properties/launchSettings.json).
