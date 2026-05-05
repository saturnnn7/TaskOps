# TaskOps API

> Task & Project Management REST API

## Tech Stack

- **Runtime:** .NET 8 / ASP.NET Core
- **Database:** PostgreSQL 16 + Entity Framework Core 8
- **Cache:** Redis 7
- **Auth:** JWT (RS256) + Refresh Tokens
- **Docs:** Swagger / Scalar
- **Containerization:** Docker + Docker Compose

## Architecture

Clean layered architecture:
src/
├── TaskOps.API            # Controllers, Middleware, DI composition
├── TaskOps.Application    # Business logic, Services, DTOs
├── TaskOps.Domain         # Entities, Enums, Interfaces
└── TaskOps.Infrastructure # EF Core, Repositories, Redis, Email

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose

### Run with Docker
```bash
docker-compose up --build
```

### Run locally
```bash
cd backend
dotnet restore
dotnet run --project src/TaskOps.API
```

## API Documentation
Swagger UI available at: `http://localhost:5000/swagger`

## Project Status
🚧 In active development