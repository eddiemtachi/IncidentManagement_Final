# IncidentManagement changes

## Included changes
- Preserved the existing Blazor WebAssembly + ASP.NET Core Web API + EF Core project layout.
- Kept JWT login and ASP.NET Core Identity `PasswordHasher` password hashing.
- Login remains open to ordinary active registered users; the API issues a JWT containing the actual role from `dbo.Roles`.
- Profile Management now loads users from the database and enables/disables accounts using `Users.isActive`.
- User administration API endpoints require the `Admin` role. An administrator cannot deactivate their own account.
- Incident page loads `GET api/Incident/my`; the API filters by the authenticated JWT user ID.
- Incident creation derives the reporter and property from the authenticated database user. The client cannot spoof another reporter ID.
- Registration validates email addresses and rejects usernames made only of zeroes.
- Maps the entity's `Incident.UserId` property to the SQL column `Incidents.ReporterId`.
- Maps `Users.RoleId` to the existing `dbo.Roles` table.

## Important database setup
The supplied SQL script has `dbo.Roles` and `Users.RoleId`; it does not contain a separate `Admin` table. The implementation uses the existing Roles table as the role/privilege source. Run `DATABASE_SETUP_ADMIN_ROLES.sql` after changing `@AdminEmail` to the email of an already registered account. That account becomes an administrator. New accounts receive the `Resident` role.

Ensure `dbo.Roles` contains `Admin` and `Resident`. The SQL setup script inserts either role if missing.

## Configuration
The Web API still reads:
- `ConnectionStrings:IncidentDbConnection`
- `Jwt:Key` (at least 32 UTF-8 bytes)
- `Jwt:Issuer`
- `Jwt:Audience`

For deployment, store the JWT signing key in User Secrets or an environment variable rather than committing a production secret in `appsettings.json`.

## Run
1. Open `IncidentManagement.slnx` in Visual Studio.
2. Set `IncidentManagement.WebApi` and `IncidentManagement.WebClient` as the startup projects.
3. Confirm the API URL in `IncidentManagement.WebClient/Program.cs` matches the API HTTPS URL in `IncidentManagement.WebApi/Properties/launchSettings.json`.
4. Run the database setup SQL, then run the solution.
5. Sign in using the account assigned the Admin role to manage users. Ordinary users can sign in and see only incidents they reported.

## Build verification limitation
This environment did not have the .NET SDK installed, so a `dotnet build` or live SQL Server integration test could not be run here. Open and build the solution in Visual Studio on a machine with the .NET 10 SDK and restore NuGet packages.
