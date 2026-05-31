# Domain model

## Aggregate boundaries

| Aggregate root | Entities / value objects | Invariants enforced via |
|----------------|--------------------------|-------------------------|
| **User** | `UserRole`, `RefreshToken`, owned `Project` references | `User.Register`, `CreateProject`, `AssignRole`, `IssueRefreshToken` |
| **Project** | `ProjectTask`, `Comment` (via task) | `Project.AddTask`, `GetTask`, `RemoveTask`, `UpdateDetails` |
| **Role** | — | `Role.Create` (reference data) |

Cross-aggregate references use **IDs only** (`OwnerId`, `AssigneeId`, `AuthorId`) except where EF navigation is required for persistence.

## Entity map (concept → type)

| Concept | CLR type | Notes |
|---------|----------|--------|
| Task | `ProjectTask` | Avoids clash with `System.Threading.Tasks.Task` |
| User | `User` | |
| Project | `Project` | |
| Comment | `Comment` | |
| Role | `Role` | |
| Refresh token | `RefreshToken` | |

## Relationships

```
User 1──* Project (owner)
Project 1──* ProjectTask
ProjectTask *──1 User (assignee, optional)
ProjectTask 1──* Comment
Comment *──1 User (author)
User *──* Role (via UserRole)
User 1──* RefreshToken
```

## Task fields

`Id`, `Title`, `Description`, `Status`, `Priority`, `DueDate`, `CreatedAt`, `UpdatedAt` — on `BaseEntity` + `ProjectTask`.

## Project fields

`Id`, `Name`, `Description`, `CreatedAt`, `UpdatedAt` — on `BaseEntity` + `Project`.
