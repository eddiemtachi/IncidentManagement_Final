# Build and security notes

- The active solution excludes the MAUI project.
- The API uses MVC controllers, JWT token issuance/validation, and `IncidentDbContext`.
- Passwords are hashed with ASP.NET Core Identity `PasswordHasher`; login verifies the encoded hash.
- Ordinary active registered users can log in. The JWT contains the user's role from `dbo.Roles`.
- `dbo.Roles` is the role/privilege table in the supplied database. There is no separate `Admin` table in the supplied SQL script.
- User profile listing and activation/deactivation endpoints require an active Admin account. User status is stored in `dbo.Users.isActive`.
- The incident page calls the authenticated `api/Incident/my` endpoint. The API derives the reporter ID from the JWT and filters by `Incidents.ReporterId`.
- The API checks `isActive` at login and on incident creation/listing. Disabled accounts cannot continue to use those endpoints with an old token.
- Run `DATABASE_SETUP_ADMIN_ROLES.sql` after replacing its administrator email placeholder.
- Verify the SQL Server connection string, JWT key, issuer, and audience before running.
