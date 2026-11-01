# Simplified Incident Management System

## Projects
- `IncidentManagement.WebClient`: Blazor WebAssembly UI; existing pages are retained.
- `IncidentManagement.WebApi`: MVC API controllers, JWT issuance/validation, and direct EF Core `IncidentDbContext` access.
- `IncidentManagement.Domain`: existing database entities.
- `IncidentManagement.Infrastructure`: `IncidentDbContext` and existing SQL Server entity mappings.
- `IncidentManagement.Application`: existing request/response DTOs used by the UI and API.
- `IncidentManagement.Shared`: existing Blazor shared models and authentication state.

The MAUI project is intentionally excluded from the solution. Controllers now query/update `IncidentDbContext` directly; repository, Unit of Work, and application-service registrations are removed from the API startup. Existing Blazor pages and client service classes remain.

## Before running
1. Open `IncidentManagement.slnx` in Visual Studio with the .NET 10 SDK installed.
2. Confirm the SQL Server instance in `IncidentManagement.WebApi/appsettings.json` matches your machine and that the `IncidentManagement` database/tables already exist.
3. Replace the development JWT key before deployment. For local development, use user-secrets instead of committing the key. Example from the WebAPI project directory:
   `dotnet user-secrets set "Jwt:Key" "<a-random-secret-at-least-32-bytes-long>"`
4. Ensure the WebAPI runs at `https://localhost:7226` and Blazor at `https://localhost:7220`. Trust the local HTTPS development certificate if prompted.
5. Run the WebAPI and WebClient projects together.

## Authentication and authorization
Login checks the submitted password against the stored ASP.NET Identity password hash and issues a signed JWT. No admin-only login or role-based authorization checks are configured, as requested. **API endpoints are therefore not restricted by user identity/role; do not expose this configuration publicly without adding appropriate access controls.**

## Limitations
The uploaded archive contains no usable .NET SDK in this environment, so this source could not be compiled or run here. Build and integration-test it in Visual Studio, particularly against your actual SQL Server schema. The existing SignUp Blazor page still uses its in-memory `UserService`; the API registration endpoint is available at `POST /api/RegisterUser/register` and requires `Username`, `Email`, `Password`, optional phone numbers, and a valid `PropertyId`.


## Login flow and running locally

1. Open `IncidentManagement.slnx` in Visual Studio with the .NET 10 SDK installed.
2. In `IncidentManagement.WebApi/appsettings.json`, set `ConnectionStrings:IncidentDbConnection` to your SQL Server database and configure a private JWT key of at least 32 UTF-8 bytes. Keep the same `Jwt:Issuer` and `Jwt:Audience` values for the API. Do not use the sample key in production.
3. Start `IncidentManagement.WebApi` first using the HTTPS profile (`https://localhost:7226`).
4. Start `IncidentManagement.WebClient` using its HTTPS profile (`https://localhost:7220`). Trust the local ASP.NET development certificate if prompted.
5. Register an ordinary user at `/signup`, then log in at `/login`. The Blazor client posts to `POST https://localhost:7226/api/Auth/login`; the API returns a signed JWT and the client adds it as a Bearer token to later requests in the current client session.

The login endpoint accepts email or username. No admin-only login or role-based authorization policy is configured. This means the API endpoints are not restricted by roles; add endpoint authorization before deploying a real system.

Password hashes created by this project are ASP.NET Identity encoded hash strings. The `User.PasswordHash` entity property therefore uses `string`, matching a SQL Server text column. If the existing database column is `varbinary`, change the database schema or mapping to match before running; do not cast or decrypt the password hash.

If the previous `ArgumentOutOfRangeException` occurs while System.Text.Json reads the request, close Visual Studio, delete `bin` and `obj` folders for all projects, then restore and rebuild the solution. The project files use aligned .NET 10 package versions to reduce assembly-version mismatches.
