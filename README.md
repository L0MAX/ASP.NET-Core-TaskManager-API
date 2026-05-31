# Task Manager API

Production-ready task management REST API built with **ASP.NET Core 9** and **Clean Architecture**.

## Tech stack

| Layer | Technologies |
|-------|----------------|
| API | ASP.NET Core 9 Web API, Swagger, JWT Bearer, health checks |
| Application | FluentValidation, AutoMapper, application services |
| Domain | Entities, enums, domain exceptions |
| Infrastructure | Entity Framework Core 9, SQL Server, migrations |

## Solution structure

```
csharp-taskmanager-api/
├── .env                    # Local secrets (gitignored) — copy from .env.example
├── .env.example            # Committed template for all environment variables
├── TaskManager.sln
├── global.json
└── src/
    ├── Api/                # HTTP host, controllers, middleware, Swagger
    ├── Application/        # DTOs, validators, services, mapping
    ├── Domain/             # DDD entities, value objects, aggregates (see Domain/README.md)
    └── Infrastructure/     # EF Core, SQL Server, JWT, migrations
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance, Docker, or Azure SQL)
- Optional: [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool restore` in repo root)

## Quick start

1. **Clone and configure environment**

   ```bash
   cp .env.example .env
   ```

   Edit `.env` with your SQL Server credentials and a JWT secret (minimum 32 characters).

2. **Restore and build**

   ```bash
   dotnet tool restore
   dotnet build TaskManager.sln
   ```

3. **Apply database migrations**

   Migrations run automatically when `ASPNETCORE_ENVIRONMENT=Development`. To apply manually:

   ```bash
   dotnet ef database update \
     --project src/Infrastructure \
     --startup-project src/Api
   ```

4. **Run the API**

   ```bash
   dotnet run --project src/Api
   ```

5. **Open Swagger**

   - https://localhost:5001/swagger  
   - Register → copy `token` → **Authorize** → `Bearer {token}` → call task endpoints

## Environment variables

Configuration is loaded in this order (later wins):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Root `.env` file (via [DotNetEnv](https://github.com/tonerdo/dotnet-env), loaded at startup)
4. System environment variables
5. User secrets (optional, `dotnet user-secrets` in `src/Api`)

| Variable | Required | Description |
|----------|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Yes | `Development`, `Staging`, or `Production` |
| `ASPNETCORE_URLS` | No | Listen URLs (default in launchSettings) |
| `CONNECTIONSTRINGS__DEFAULTCONNECTION` | Yes | SQL Server connection string |
| `JWT__SECRET` | Yes | Signing key (≥ 32 characters) |
| `JWT__ISSUER` | Yes | Token issuer |
| `JWT__AUDIENCE` | Yes | Token audience |
| `JWT__EXPIRATIONHOURS` | No | Token lifetime in hours (default: `24`) |

See [.env.example](.env.example) for the full template with comments.

> **Production:** Do not rely on `.env` files. Use Azure Key Vault, AWS Secrets Manager, or platform environment variables. Never commit `.env`.

## API reference

### Authentication (public)

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/auth/register` | Create account, returns JWT |
| `POST` | `/api/auth/login` | Sign in, returns JWT |

**Register body example:**

```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "Jane",
  "lastName": "Doe"
}
```

### Projects (requires `Authorization: Bearer {token}`)

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/projects` | Paginated list of projects owned by the user |
| `GET` | `/api/projects/{id}` | Get one project |
| `POST` | `/api/projects` | Create project |
| `PUT` | `/api/projects/{id}` | Update project |
| `DELETE` | `/api/projects/{id}` | Delete project (cascades tasks and comments) |

### Tasks (requires `Authorization: Bearer {token}`)

Tasks live under a project. The domain type is `ProjectTask` (table `Tasks`).

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/projects/{projectId}/tasks` | Paginated list (`pageNumber`, `pageSize`, `status`, `priority`) |
| `GET` | `/api/projects/{projectId}/tasks/{taskId}` | Get one task |
| `POST` | `/api/projects/{projectId}/tasks` | Create task (optional `assigneeId`) |
| `PUT` | `/api/projects/{projectId}/tasks/{taskId}` | Update task |
| `DELETE` | `/api/projects/{projectId}/tasks/{taskId}` | Delete task |

**Task status:** `Pending`, `InProgress`, `Completed`, `Cancelled`  
**Priority:** `Low`, `Medium`, `High`, `Critical`

## Domain model

See [src/Domain/README.md](src/Domain/README.md) for aggregate boundaries and relationships.

| Concept | Entity | Aggregate root |
|---------|--------|------------------|
| User | `User` | Yes |
| Project | `Project` | Yes |
| Task | `ProjectTask` | No (child of `Project`) |
| Comment | `Comment` | No (child of `ProjectTask`) |
| Role | `Role` | Reference data |
| Refresh token | `RefreshToken` | No (child of `User`) |

**Relationships:** User owns Projects → Project contains Tasks → Task has Comments; Task optionally assigned to User (`AssigneeId`); User ↔ Role via `UserRole`; User has RefreshTokens.

### Health

| Method | Path | Auth |
|--------|------|------|
| `GET` | `/health` | None |

## Cross-cutting concerns

| Concern | Location |
|---------|----------|
| Dependency injection | `Application/DependencyInjection.cs`, `Infrastructure/DependencyInjection.cs` |
| Exception handling | `Api/Middleware/ExceptionHandlingMiddleware.cs` |
| Request validation | `Api/Filters/ValidationFilter.cs` + FluentValidation in Application |
| Logging | Console + Debug providers; levels via config / `.env` |
| EF Core | `Infrastructure/Data/ApplicationDbContext.cs` |
| Migrations | `Infrastructure/Migrations/` |

## EF Core commands

```bash
# Add migration after model changes
dotnet ef migrations add <MigrationName> \
  --project src/Infrastructure \
  --startup-project src/Api \
  --output-dir Migrations

# Update database
dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/Api
```

## Documentation maintenance

This repository keeps **README.md** and **.env.example** in sync with the codebase.

When you add or change features, configuration, endpoints, or project structure, update:

- **README.md** — setup steps, env table, API reference, architecture notes
- **.env.example** — any new or renamed environment variables (with comments)

A Cursor rule (`.cursor/rules/documentation.mdc`) reminds contributors and AI assistants to apply these updates on every refactor or feature.

## License

MIT (adjust as needed for your organization).
