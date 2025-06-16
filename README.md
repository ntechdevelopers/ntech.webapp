# Ntech Web Application

## Overview
This repository contains a .NET 6 solution for a web application platform with user, role, and permission management, database migrations, and Docker-based deployment. The solution is modular, supporting PostgreSQL by default, and is ready for extension.

---

## Project Structure
- `Ntech.WebApp/` — Main ASP.NET Core MVC web application
- `Ntech.Migration/` — Database migration tool using DbUp
- `Ntech.Libraries/` — Shared utility libraries
- `docker-compose.yml` — Multi-container orchestration for the app and database

---

## Features
- User, Role, and Permission management (with SuperAdmin role)
- Dashboard, Applications, Assets, Devices management (controllers scaffolded)
- Database seeding for default roles and users
- Session, authentication, and email sending support
- Dockerized for easy deployment

---

## Getting Started

### Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)
- PostgreSQL (default, but can be switched to MySQL/SQL Server)

### Build and Run (Development)
```sh
dotnet build Ntech.WebApp.sln
dotnet run --project Ntech.WebApp/Ntech.WebApp.csproj
```

### Database Migration
```sh
dotnet run --project Ntech.Migration/Ntech.Migration.csproj ntech-database
```

### Run with Docker Compose
```sh
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build
```

### Configuration
- Update connection strings in `Ntech.WebApp/appsettings.json` and `Ntech.Migration/appsettings.json`.
- Default PostgreSQL connection:
  ```json
  "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd;"
  ```
- MailJet API keys and other settings are also in `appsettings.json`.

---

## Main Endpoints
- `/Dashboard` — Main dashboard (requires authentication)
- `/Users` — User management (requires authentication)
- `/Roles` — Role management (SuperAdmin only)
- `/UserRoles` — Assign roles to users (SuperAdmin only)
- `/Permission` — Manage role permissions (SuperAdmin only)
- `/Applications`, `/Assets`, `/Devices` — Scaffolded controllers for future expansion

---

## Docker Images
- `ntech-web-app` — Built from `Ntech.WebApp/Dockerfile`
- `ntech-web-migration` — Built from `Ntech.Migration/Dockerfile`
- `postgresql` — Official Postgres image

---

## Notes
- The solution is ready for extension with more features and endpoints.
- For production, update secrets and connection strings accordingly.