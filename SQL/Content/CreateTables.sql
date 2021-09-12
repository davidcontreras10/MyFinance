BEGIN
IF OBJECT_ID('SpendOnPeriod', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'SpendOnPeriod';
IF OBJECT_ID('AccountPeriod', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'AccountPeriod';
IF OBJECT_ID('AccountInclude', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'AccountInclude';
IF OBJECT_ID('Account', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'Account';
IF OBJECT_ID('PeriodDefinition', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'PeriodDefinition';
IF OBJECT_ID('PeriodType', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'PeriodType';
IF OBJECT_ID('AppUser', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'AppUser';
IF OBJECT_ID('Spend', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'Spend';
IF OBJECT_ID('SpendType', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'SpendType';
IF OBJECT_ID('CurrencyConverterMethod', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'CurrencyConverterMethod';
IF OBJECT_ID('CurrencyConverter', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'CurrencyConverter';
IF OBJECT_ID('Currency', 'U') IS NOT NULL
	EXEC dbo.SpDropTable @ptable = 'Currency';
END
GO
BEGIN

CREATE TABLE [dbo].[ApplicationResource](
	[ApplicationResourceId] [INT] NOT NULL,
	[ApplicationResourceName] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_ApplicationResource] PRIMARY KEY CLUSTERED 
(
	[ApplicationResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[ResourceAction](
	[ResourceActionId] [INT] NOT NULL,
	[ResourceActionName] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_ResourceAction] PRIMARY KEY CLUSTERED 
(
	[ResourceActionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[ResourceAccessLevel](
	[ResourceAccessLevelId] [INT] NOT NULL,
	[ResourceAccessLevelName] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_ResourceAccessLevel] PRIMARY KEY CLUSTERED 
(
	[ResourceAccessLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[ResourceRequiredAccess](
	[ResourceActionId] [INT] NOT NULL,
	[ApplicationResourceId] [INT] NOT NULL,
	[ResourceAccessLevelId] [INT] NOT NULL
)

ALTER TABLE [dbo].[ResourceRequiredAccess]  WITH NOCHECK ADD  CONSTRAINT [ResourceRequiredAccess_FK_ResourceActionId] 
FOREIGN KEY([ResourceActionId]) REFERENCES [dbo].[ResourceAction] ([ResourceActionId])

ALTER TABLE [dbo].[ResourceRequiredAccess]  WITH NOCHECK ADD  CONSTRAINT [ResourceRequiredAccess_FK_ApplicationResourceId] 
FOREIGN KEY([ApplicationResourceId]) REFERENCES [dbo].[ApplicationResource] ([ApplicationResourceId])

ALTER TABLE [dbo].[ResourceRequiredAccess]  WITH NOCHECK ADD  CONSTRAINT [ResourceRequiredAccess_FK_ResourceAccessLevelId] 
FOREIGN KEY([ResourceAccessLevelId]) REFERENCES [dbo].[ResourceAccessLevel] ([ResourceAccessLevelId])

CREATE TABLE dbo.AccountType(
	AccountTypeId INT IDENTITY(1,1) NOT NULL,
	AccountTypeName Varchar(500) NOT NULL,
	CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED 
(
	AccountTypeId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[FinancialEntity](
	[FinancialEntityId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_FinancialEntity] PRIMARY KEY CLUSTERED 
(
	[FinancialEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[Currency](
	[CurrencyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Symbol] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[CurrencyConverter](
	[CurrencyConverterId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyIdOne] [int] NOT NULL,
	[CurrencyIdTwo] [int] NOT NULL
 CONSTRAINT [PK_CurrencyConverter] PRIMARY KEY CLUSTERED 
(
	[CurrencyConverterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)


CREATE TABLE [dbo].[CurrencyConverterMethod](
	[CurrencyConverterMethodId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyConverterId] [int] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[FinancialEntityId] [int],
	[IsDefault] [BIT]
 CONSTRAINT [PK_CurrencyConverterMethod] PRIMARY KEY CLUSTERED 
(
	[CurrencyConverterMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[CurrencyConverterMethod]  WITH NOCHECK ADD  CONSTRAINT [CurrencyConverterMethod_FK_CurrencyIdOne] FOREIGN KEY([CurrencyConverterId])
REFERENCES [dbo].[CurrencyConverter] ([CurrencyConverterId])

ALTER TABLE [dbo].[CurrencyConverterMethod]  WITH NOCHECK ADD  CONSTRAINT [CurrencyConverterMethod_FK_FinancialEntityId] FOREIGN KEY([FinancialEntityId])
REFERENCES [dbo].[FinancialEntity] ([FinancialEntityId])


CREATE TABLE [dbo].[SpendType](
	[SpendTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
 CONSTRAINT [PK_SpendType] PRIMARY KEY CLUSTERED 
(
	[SpendTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[AppUser](
	[UserId] [UNIQUEIDENTIFIER] NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Password] [nvarchar](500) NOT NULL
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE dbo.AppUser ADD CONSTRAINT AppUser_Unq_Username UNIQUE ([Username]);

CREATE TABLE dbo.AccountGroup(
	AccountGroupId INT IDENTITY(1,1) NOT NULL,
	AccountGroupName Varchar(500) NOT NULL,
	DisplayValue Varchar(500),
	AccountGroupPosition INT,
	DisplayDefault BIT,
	UserId UNIQUEIDENTIFIER,
	CONSTRAINT [PK_AccountGroup] PRIMARY KEY CLUSTERED 
(
	AccountGroupId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[AccountGroup]  WITH NOCHECK ADD  CONSTRAINT [AccountGroup_FK_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AppUser] ([UserId])

--UPDATE NOT NULL CUTTING DATE
CREATE TABLE [dbo].[PeriodType](
	[PeriodTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL
 CONSTRAINT [PK_PeriodType] PRIMARY KEY CLUSTERED 
(
	[PeriodTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[PeriodDefinition](
	[PeriodDefinitionId] [int] IDENTITY(1,1) NOT NULL,
	[PeriodTypeId] [int] NOT NULL,		
	[CuttingDate] [nvarchar](500),
	[Repetition] [INT]
 CONSTRAINT [PK_PeriodDefinition] PRIMARY KEY CLUSTERED 
(
	[PeriodDefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[PeriodDefinition]  WITH NOCHECK ADD  CONSTRAINT [PeriodDefinition_FK_PeriodTypeId] FOREIGN KEY([PeriodTypeId])
REFERENCES [dbo].[PeriodType] ([PeriodTypeId])


CREATE TABLE [dbo].[Account](
	[AccountId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [UNIQUEIDENTIFIER] NOT NULL,
	[PeriodDefinitionId] [int] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[CurrencyId] [int],
	[BaseBudget] [Float],
	[Position] [int],
	[HeaderColor] [nvarchar](500),
	[AccountTypeId] [INT],
	[DefaultSpendTypeId] [INT],
	[FinancialEntityId] [INT],
	[AccountGroupId] [INT] NOT NULL DEFAULT 1
CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AppUser] ([UserId])

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_PeriodDefinitionId] FOREIGN KEY([PeriodDefinitionId])
REFERENCES [dbo].[PeriodDefinition] ([PeriodDefinitionId])

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_DefaultSpendTypeId] FOREIGN KEY([DefaultSpendTypeId])
REFERENCES [dbo].[SpendType] ([SpendTypeId])

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_FinancialEntityId] FOREIGN KEY([FinancialEntityId])
REFERENCES [dbo].[FinancialEntity] ([FinancialEntityId])

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_AccountTypeId] FOREIGN KEY([AccountTypeId])
REFERENCES [dbo].[AccountType] ([AccountTypeId])

ALTER TABLE [dbo].[Account]  WITH NOCHECK ADD  CONSTRAINT [Account_FK_AccountGroupId] FOREIGN KEY([AccountGroupId])
REFERENCES [dbo].[AccountGroup] ([AccountGroupId])

CREATE TABLE [dbo].[AccountInclude](
	[AccountId] [int],
	[AccountIncludeId] [int],
	[CurrencyConverterMethodId] [int]
	CONSTRAINT [PK_AccountInclude] PRIMARY KEY 
	(
		[AccountId],
		[AccountIncludeId]
	)   ON [PRIMARY]
)

ALTER TABLE [dbo].[AccountInclude]  WITH NOCHECK ADD CONSTRAINT [AccountInclude_FK_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([AccountId])

ALTER TABLE [dbo].[AccountInclude]  WITH NOCHECK ADD CONSTRAINT [AccountInclude_FK_AccountIncludeId] FOREIGN KEY([AccountIncludeId])
REFERENCES [dbo].[Account] ([AccountId])

ALTER TABLE [dbo].[AccountInclude]  WITH NOCHECK ADD CONSTRAINT [AccountInclude_FK_CurrencyConverterMethodId] FOREIGN KEY([CurrencyConverterMethodId])
REFERENCES [dbo].[CurrencyConverterMethod] ([CurrencyConverterMethodId])


CREATE TABLE [dbo].[AmountType](
	[AmountTypeId] [int] IDENTITY(1,1) NOT NULL,
	[AmountTypeName] [nvarchar](100) NOT NULL,
	[AmountSign] [INT] NOT NULL
 CONSTRAINT [PK_AmountType] PRIMARY KEY CLUSTERED 
(
	[AmountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 

alter table AmountType add constraint AmountTypeName_unique unique(AmountTypeName);

CREATE TABLE [dbo].[Spend](
	[SpendId] [int] IDENTITY(1,1) NOT NULL,
	[OriginalAmount] [Float],
	[SpendDate] [DateTime],
	[SpendTypeId] [int],
	[Description] [nvarchar](500) NOT NULL,
	[AmountCurrencyId] [int],
	[AmountTypeId] [INT],
	[Numerator] [FLOAT] DEFAULT 1,
	[Denominator] [FLOAT] DEFAULT 1,
 CONSTRAINT [PK_Spend] PRIMARY KEY CLUSTERED 
(
	[SpendId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 

ALTER TABLE [dbo].[Spend]  WITH NOCHECK ADD  CONSTRAINT [Spend_FK_SpendTypeId] FOREIGN KEY([SpendTypeId])
REFERENCES [dbo].[SpendType] ([SpendTypeId])

ALTER TABLE [dbo].[Spend]  WITH NOCHECK ADD  CONSTRAINT [Spend_FK_AmountCurrencyId] FOREIGN KEY([AmountCurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])

ALTER TABLE [dbo].[Spend]  WITH NOCHECK ADD  CONSTRAINT [Spend_FK_AmountTypeId] FOREIGN KEY([AmountTypeId])
REFERENCES [dbo].[AmountType] ([AmountTypeId])

CREATE TABLE [dbo].[AccountPeriod](
	AccountPeriodId [int] IDENTITY(1,1) NOT NULL,
	AccountId [int],
	Budget [Float],
	InitialDate [DateTime],
	EndDate [DateTime],
	[CurrencyId] [int]
 CONSTRAINT [PK_AccountPeriod] PRIMARY KEY CLUSTERED 
(
	[AccountPeriodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 

ALTER TABLE [dbo].[AccountPeriod]  WITH NOCHECK ADD  CONSTRAINT [AccountPeriod_FK_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([AccountId])

ALTER TABLE [dbo].[AccountPeriod]  WITH NOCHECK ADD  CONSTRAINT [AccountPeriod_FK_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])


CREATE TABLE [dbo].[SpendOnPeriod](
	[SpendId] [int] NOT NULL,
	[AccountPeriodId] [int] NOT NULL,
	[Numerator] [FLOAT],
	[Denominator] [FLOAT],
	[PendingUpdate] [BIT],
	[CurrencyConverterMedbothodId] [INT],
	[IsOriginal] [BIT]
 CONSTRAINT [PK_SpendOnPeriod] PRIMARY KEY CLUSTERED 
(
	[SpendId],
	[AccountPeriodId]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 


ALTER TABLE [dbo].[SpendOnPeriod]  WITH NOCHECK ADD  CONSTRAINT [SpendOnPeriod_FK_SpendId] FOREIGN KEY([SpendId])
REFERENCES [dbo].[Spend] ([SpendId])

ALTER TABLE [dbo].[SpendOnPeriod]  WITH NOCHECK ADD  CONSTRAINT [SpendOnPeriod_FK_AccountPeriodId] FOREIGN KEY([AccountPeriodId])
REFERENCES [dbo].[AccountPeriod] ([AccountPeriodId])

ALTER TABLE [dbo].[SpendOnPeriod]  WITH NOCHECK ADD CONSTRAINT [SpendOnPeriod_FK_CurrencyConverterMethodId] FOREIGN KEY([CurrencyConverterMethodId])
REFERENCES [dbo].[CurrencyConverterMethod] ([CurrencyConverterMethodId])

CREATE TABLE [dbo].[TransferRecord](
	[TransferRecordId] [INT] NOT NULL,
	[SpendId] [INT] NOT NULL
)

ALTER TABLE [dbo].[TransferRecord]  WITH NOCHECK ADD  CONSTRAINT [TransferRecord_FK_SpendId] FOREIGN KEY([SpendId])
REFERENCES [dbo].[Spend] ([SpendId])

END

CREATE TABLE [dbo].[UserAssignedAccess](
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[ResourceActionId] [INT] NOT NULL,
	[ApplicationResourceId] [INT] NOT NULL,
	[ResourceAccessLevelId] [INT] NOT NULL,
	[OnResourceActionLevelId] [INT] NOT NULL
)

ALTER TABLE [dbo].[UserAssignedAccess]  WITH NOCHECK ADD  CONSTRAINT [UserAssignedAccess_FK_UserId] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[AppUser] ([UserId])

ALTER TABLE [dbo].[UserAssignedAccess]  WITH NOCHECK ADD  CONSTRAINT [UserAssignedAccess_FK_ResourceActionId] 
FOREIGN KEY([ResourceActionId]) REFERENCES [dbo].[ResourceAction] ([ResourceActionId])

ALTER TABLE [dbo].[UserAssignedAccess]  WITH NOCHECK ADD  CONSTRAINT [UserAssignedAccess_FK_ApplicationResourceId] 
FOREIGN KEY([ApplicationResourceId]) REFERENCES [dbo].[ApplicationResource] ([ApplicationResourceId])

ALTER TABLE [dbo].[UserAssignedAccess]  WITH NOCHECK ADD  CONSTRAINT [UserAssignedAccess_FK_ResourceAccessLevelId] 
FOREIGN KEY([ResourceAccessLevelId]) REFERENCES [dbo].[ResourceAccessLevel] ([ResourceAccessLevelId])

ALTER TABLE [dbo].[UserAssignedAccess]  WITH NOCHECK ADD  CONSTRAINT [UserAssignedAccess_FK_OnResourceActionLevelId] 
FOREIGN KEY([OnResourceActionLevelId]) REFERENCES [dbo].[OnResourceActionLevel] ([OnResourceActionLevelId])

CREATE TABLE [dbo].[AppUserOwner](
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[OwnerUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [not_null_AppUserOwner] check 
	([UserId] <> [OwnerUserId])
);

ALTER TABLE [dbo].[AppUserOwner] WITH NOCHECK ADD  CONSTRAINT [AppUserOwner_FK_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AppUser] ([UserId]);

ALTER TABLE [dbo].[AppUserOwner] WITH NOCHECK ADD  CONSTRAINT [AppUserOwner_FK_OwnerUserId] FOREIGN KEY([OwnerUserId])
REFERENCES [dbo].[AppUser] ([UserId]);

CREATE TABLE dbo.LoanRecord(
	LoanRecordId INT IDENTITY(1,1) NOT NULL,
	LoanRecordName VARCHAR(100),
	SpendId INT NOT NULL,
 CONSTRAINT PK_LoanRecord PRIMARY KEY CLUSTERED 
(
	LoanRecordId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 

ALTER TABLE dbo.LoanRecord WITH NOCHECK ADD  CONSTRAINT [LoanRecord_FK_SpendId] FOREIGN KEY(SpendId)
REFERENCES dbo.Spend (SpendId);

CREATE TABLE dbo.LoanSpend(
	LoanRecordId INT NOT NULL,
	SpendId INT NOT NULL,
 CONSTRAINT PK_LoanSpend UNIQUE CLUSTERED 
(
	LoanRecordId, SpendId
)
)

ALTER TABLE dbo.LoanSpend WITH NOCHECK ADD  CONSTRAINT [LoanSpend_FK_LoanRecordId] FOREIGN KEY(LoanRecordId)
REFERENCES dbo.LoanRecord (LoanRecordId);

ALTER TABLE dbo.LoanSpend WITH NOCHECK ADD  CONSTRAINT [LoanSpend_FK_SpendId] FOREIGN KEY(SpendId)
REFERENCES dbo.Spend (SpendId);

 