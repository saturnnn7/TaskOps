# TaskOps API

> Production-ready Task & Project Management REST API built with ASP.NET Core 8.

[![CI](https://github.com/saturnnn7/TaskOps/actions/workflows/ci.yml/badge.svg)](https://github.com/saturnnn7/TaskOps/actions/workflows/ci.yml)

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core |
| Database | PostgreSQL 16 + Entity Framework Core 8 |
| Cache | Redis 7 |
| Authentication | JWT RS256 + Refresh Tokens |
| Password Hashing | Argon2id |
| Validation | FluentValidation 11 |
| Documentation | Swagger / OpenAPI |
| Containerization | Docker + Docker Compose |
| CI | GitHub Actions |

## Architecture

Clean layered architecture with strict unidirectional dependencies:

### src

- **TaskOps.Domain**
  - Entities
  - Enums
  - Interfaces
  - Domain errors

- **TaskOps.Application**
  - Services (business logic)
  - DTOs
  - Validators
  - Use cases

- **TaskOps.Infrastructure**
  - EF Core (database)
  - Redis (cache)
  - JWT (auth)
  - Password hashing

- **TaskOps.API**
  - Controllers
  - Middleware
  - Dependency Injection
  - Filters

**Key patterns:**
- `Result<T>` — explicit success/failure instead of exceptions
- `ApiResponse<T>` — unified response envelope for all endpoints
- `PagedResponse<T>` — consistent pagination across all list endpoints
- Repository + Unit of Work — abstracted data access layer
- Resource-based authorization — roles scoped per project (Owner/Member/Viewer)

## API Endpoints

### Auth
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/auth/register` | Register new account | ❌ |
| POST | `/api/v1/auth/login` | Login and get tokens | ❌ |
| POST | `/api/v1/auth/refresh` | Refresh access token | ❌ |
| POST | `/api/v1/auth/logout` | Revoke refresh token | ✅ |

### Users
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/users/me` | Get current user profile | ✅ |
| PATCH | `/api/v1/users/me` | Update display name | ✅ |
| POST | `/api/v1/users/me/change-password` | Change password | ✅ |

### Projects
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/projects` | List user's projects | ✅ |
| GET | `/api/v1/projects/{id}` | Get project by ID | ✅ |
| POST | `/api/v1/projects` | Create project | ✅ |
| PATCH | `/api/v1/projects/{id}` | Update project | ✅ |
| DELETE | `/api/v1/projects/{id}` | Archive project | ✅ |
| GET | `/api/v1/projects/{id}/members` | List members | ✅ |
| POST | `/api/v1/projects/{id}/members` | Add member | ✅ |
| DELETE | `/api/v1/projects/{id}/members/{userId}` | Remove member | ✅ |

### Tasks
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/projects/{id}/tasks` | List project tasks | ✅ |
| GET | `/api/v1/tasks/{id}` | Get task by ID | ✅ |
| POST | `/api/v1/projects/{id}/tasks` | Create task | ✅ |
| PATCH | `/api/v1/tasks/{id}` | Update task | ✅ |
| DELETE | `/api/v1/tasks/{id}` | Delete task | ✅ |

### Comments
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/tasks/{id}/comments` | List task comments | ✅ |
| POST | `/api/v1/tasks/{id}/comments` | Create comment | ✅ |
| PATCH | `/api/v1/comments/{id}` | Update comment | ✅ |
| DELETE | `/api/v1/comments/{id}` | Soft-delete comment | ✅ |

## Authorization Model

Roles are scoped per project — a user can be Owner in one project and Viewer in another.

| Action | Viewer | Member | Owner |
|---|---|---|---|
| View project & tasks | ✅ | ✅ | ✅ |
| Create / update tasks | ❌ | ✅ | ✅ |
| Delete own tasks | ❌ | ✅ | ✅ |
| Delete any task | ❌ | ❌ | ✅ |
| Add / remove members | ❌ | ❌ | ✅ |
| Archive project | ❌ | ❌ | ✅ |
| Comment on tasks | ✅ | ✅ | ✅ |
| Edit own comments | ✅ | ✅ | ✅ |
| Delete any comment | ❌ | ❌ | ✅ |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run locally

**1. Clone the repository:**
```bash
git clone https://github.com/YOUR_USERNAME/TaskOps.git
cd TaskOps
```

**2. Start infrastructure (PostgreSQL + Redis):**
```bash
cd backend
docker-compose -f docker-compose.dev.yml up -d
```

**3. Run the API:**
```bash
dotnet run --project src/TaskOps.API
```

**4. Open Swagger UI:**

http://localhost:5182/swagger

Migrations are applied automatically on startup.

### Run with Docker

```bash
cd backend
docker-compose up --build
```

API available at `http://localhost:8080/swagger`

## Response Format

All endpoints return a consistent envelope:

**Success:**
```json
{
  "success": true,
  "data": { ... },
  "error": null,
  "timestamp": "2026-01-01T00:00:00Z"
}
```

**Error:**
```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "Project.NotFound",
    "message": "Project with ID '...' was not found.",
    "details": null
  },
  "timestamp": "2026-01-01T00:00:00Z"
}
```

## Project Status

✅ Authentication (JWT RS256 + Argon2id + Redis refresh tokens)  
✅ Project management with role-based access  
✅ Task management with status/priority/assignee  
✅ Comments with soft delete  
✅ User profile management  
✅ Automatic database migrations  
✅ Docker support  
✅ CI pipeline (GitHub Actions)
