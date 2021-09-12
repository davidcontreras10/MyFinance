CREATE LOGIN ExchCrrnAppUserDev WITH PASSWORD = 'ExchCrrnAppUserDev'
GO

--DENY VIEW ANY DATABASE TO [FinanceAppUserDev]
--GO

Use [EXCHCRRN_DEV];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'ExchCrrnAppUserDev')
BEGIN
    CREATE USER [ExchCrrnAppUserDev] FOR LOGIN [ExchCrrnAppUserDev]
    EXEC sp_addrolemember N'db_owner', N'ExchCrrnAppUserDev'
END;
GO