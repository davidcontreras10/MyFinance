CREATE LOGIN FinanceAppUserDev WITH PASSWORD = 'FinanceAppUserDev'
GO

DENY VIEW ANY DATABASE TO [FinanceAppUserDev]
GO

Use [MYFNDB_DEV];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'FinanceAppUserDev')
BEGIN
    CREATE USER [FinanceAppUserDev] FOR LOGIN [FinanceAppUserDev]
    EXEC sp_addrolemember N'db_owner', N'FinanceAppUserDev'
END;
GO