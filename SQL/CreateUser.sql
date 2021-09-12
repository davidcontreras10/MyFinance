CREATE LOGIN FinanceAppUser WITH PASSWORD = 'FinanceAppUser'
GO

Use [MYFNDB];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'FinanceAppUser')
BEGIN
    CREATE USER [FinanceAppUser] FOR LOGIN [FinanceAppUser]
    EXEC sp_addrolemember N'db_owner', N'FinanceAppUser'
END;
GO