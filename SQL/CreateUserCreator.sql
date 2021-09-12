CREATE LOGIN FinanceAppUserCreator WITH PASSWORD = 'FinanceAppUserCreator'
GO

Use [MYFNDB];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'FinanceAppUserCreator')
BEGIN
    CREATE USER [FinanceAppUserCreator] FOR LOGIN [FinanceAppUserCreator]
    EXEC sp_addrolemember N'db_owner', N'FinanceAppUserCreator'
END;
GO

Use [MYFNDB];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'FinanceAppUserCreator')
BEGIN
    CREATE USER [FinanceAppUserCreator] FOR LOGIN [FinanceAppUserCreator]
    EXEC sp_addrolemember N'db_owner', N'FinanceAppUserCreator'
END;
GO