USE [IncidentManagement];
GO

-- This project uses the existing dbo.Roles table for privileges.
-- It does not need a separate Admin table.
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Admin')
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Resident')
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Resident');

-- IMPORTANT: Replace the email below with the email address of an already
-- registered account that should be the administrator.
DECLARE @AdminEmail NVARCHAR(200) = N'REPLACE_WITH_YOUR_ADMIN_EMAIL';
DECLARE @AdminRoleId INT = (SELECT TOP (1) RoleId FROM dbo.Roles WHERE RoleName = N'Admin');

IF @AdminEmail = N'REPLACE_WITH_YOUR_ADMIN_EMAIL'
BEGIN
    PRINT 'Set @AdminEmail to your registered administrator account before running the UPDATE.';
END
ELSE IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @AdminEmail)
BEGIN
    PRINT 'No registered user was found with that email. Register the account first.';
END
ELSE
BEGIN
    UPDATE dbo.Users
    SET RoleId = @AdminRoleId
    WHERE Email = @AdminEmail;
    PRINT 'Administrator role assigned.';
END;

-- Newly registered accounts are assigned the Resident role by the API.
GO
