CREATE LOGIN ExchCrrnAppUser WITH PASSWORD = 'ExchCrrnAppUser'
GO

Use [EXCHCRRN];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'ExchCrrnAppUser')
BEGIN
    CREATE USER [ExchCrrnAppUser] FOR LOGIN [ExchCrrnAppUser]
    EXEC sp_addrolemember N'db_owner', N'ExchCrrnAppUser'
END;
GO