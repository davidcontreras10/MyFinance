BEGIN
IF OBJECT_ID('BccrVentanilla', 'U') IS NOT NULL
BEGIN
	EXEC dbo.SpDropTable @ptable = 'BccrVentanilla';
END
IF OBJECT_ID('BccrVentanillaInsertLog', 'U') IS NOT NULL
BEGIN
	EXEC dbo.SpDropTable @ptable = 'BccrVentanillaInsertLog';
END
IF OBJECT_ID('EntitiesSupported', 'U') IS NOT NULL
BEGIN
	EXEC dbo.SpDropTable @ptable = 'EntitiesSupported';
END
END
GO

BEGIN
CREATE TABLE dbo.BccrVentanilla(
	Entity VARCHAR(500) NOT NULL,
	Purchase FLOAT NOT NULL,
	Sell FLOAT NOT NULL,
	LastUpdate DATETIME NOT NULL
);

CREATE TABLE dbo.BccrVentanillaInsertLog(
	InsertDate DATETIME NOT NULL,
	InsertAttemptCount INT NOT NULL,
	InsertRealCount INT NOT NULL
);

CREATE TABLE dbo.EntitiesSupported(
	EntitiesSupportedId INT IDENTITY(1,1) NOT NULL,
	EntityName VARCHAR(500),
	EntitySearchKey VARCHAR(500) NOT NULL,
	 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
	(
		EntitiesSupportedId ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE dbo.MethodsSupported(
EntitiesSupportedId INT NOT NULL,
MethodId INT NOT NULL,
Colones BIT
)

ALTER TABLE [dbo].[MethodsSupported]  WITH NOCHECK ADD CONSTRAINT [MethodsSupported_FK_EntitiesSupportedId] FOREIGN KEY([EntitiesSupportedId])
REFERENCES [dbo].[EntitiesSupported] ([EntitiesSupportedId])

CREATE TABLE dbo.BccrWebServiceIndicator(
EntityName VARCHAR(500) NOT NULL,
SellCode VARCHAR(500) NOT NULL,
PurchaseCode VARCHAR(500) NOT NULL
)

END

