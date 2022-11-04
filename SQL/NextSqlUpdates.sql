/*
EXEC dbo.SpDropTable @ptable = 'ExecutedTask';
EXEC dbo.SpDropTable @ptable = 'SpDaysTrxTaskFreq';
EXEC dbo.SpDropTable @ptable = 'TransferTrxDef';
EXEC dbo.SpDropTable @ptable = 'SpInTrxDef';
EXEC dbo.SpDropTable @ptable = 'AutomaticTask';
*/

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[AutomaticTask](
	[AutomaticTaskId] [UNIQUEIDENTIFIER] NOT NULL,
	[Amount] [Float] NOT NULL,
	[SpendTypeId] [INT] NOT NULL,
	[CurrencyId] [INT] NOT NULL,
	[TaskDescription] [nvarchar](400),
	[AccountId] [INT] NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[PeriodTypeId] [INT] NOT NULL,
	[Days] [NVARCHAR] NOT NULL
 CONSTRAINT [PK_AutomaticTask] PRIMARY KEY CLUSTERED 
(
	[AutomaticTaskId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[AutomaticTask] ADD CONSTRAINT [AutomaticTask_Unq_AutomaticTaskId] UNIQUE ([AutomaticTaskId]);

ALTER TABLE [dbo].[AutomaticTask]  WITH NOCHECK ADD CONSTRAINT [AutomaticTask_FK_UserId] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[AppUser] ([UserId])

ALTER TABLE [dbo].[AutomaticTask] WITH NOCHECK ADD CONSTRAINT [AutomaticTask_FK_SpendTypeId] FOREIGN KEY([SpendTypeId])
REFERENCES [dbo].[SpendType] ([SpendTypeId])

ALTER TABLE [dbo].[AutomaticTask] WITH NOCHECK ADD CONSTRAINT [AutomaticTask_FK_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([AccountId])

ALTER TABLE [dbo].[AutomaticTask]  WITH NOCHECK ADD  CONSTRAINT [AutomaticTask_FK_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[SpInTrxDef](
	[SpInTrxDefId] [UNIQUEIDENTIFIER] NOT NULL,
	[IsSpendTrx] [BIT] NOT NULL,
 CONSTRAINT [PK_SpInTrxDef] PRIMARY KEY CLUSTERED 
(
	[SpInTrxDefId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[SpInTrxDef] ADD CONSTRAINT [SpInTrxDef_Unq_SpInTrxDefId] UNIQUE ([SpInTrxDefId]);

ALTER TABLE [dbo].[SpInTrxDef] WITH NOCHECK ADD CONSTRAINT [SpInTrxDef_FK_AutomaticTaskId] FOREIGN KEY([SpInTrxDefId])
REFERENCES [dbo].[AutomaticTask] ([AutomaticTaskId])

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[TransferTrxDef](
	[TransferTrxDefId] [UNIQUEIDENTIFIER] NOT NULL,
	[ToAccountId] [INT] NOT NULL
 CONSTRAINT [PK_TransferTrxDef] PRIMARY KEY CLUSTERED 
(
	[TransferTrxDefId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[TransferTrxDef] ADD CONSTRAINT [TransferTrxDef_Unq_TransferTrxDefId] UNIQUE ([TransferTrxDefId]);

ALTER TABLE [dbo].[TransferTrxDef] WITH NOCHECK ADD CONSTRAINT [TransferTrxDef_FK_AutomaticTaskId] FOREIGN KEY([TransferTrxDefId])
REFERENCES [dbo].[AutomaticTask] ([AutomaticTaskId])

ALTER TABLE [dbo].[TransferTrxDef] WITH NOCHECK ADD CONSTRAINT [TransferTrxDef_FK_ToAccountId] FOREIGN KEY([ToAccountId])
REFERENCES [dbo].[Account] ([AccountId])

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[ExecutedTask](
	[ExecutedTaskId] INT IDENTITY(1,1) NOT NULL,
	[AutomaticTaskId] UNIQUEIDENTIFIER NOT NULL,
	[ExecutedByUserId] UNIQUEIDENTIFIER NOT NULL,
	[ExecuteDatetime] DATETIME NOT NULL,
	[ExecutionStatus] INT NOT NULL,
	[ExecutionMsg] VARCHAR(500)
 CONSTRAINT [PK_ExecutedTask] PRIMARY KEY CLUSTERED 
(
	[ExecutedTaskId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[ExecutedTask] ADD CONSTRAINT [ExecutedTask_Unq_ExecutedTaskId] UNIQUE ([ExecutedTaskId]);

ALTER TABLE [dbo].[ExecutedTask] WITH NOCHECK ADD CONSTRAINT [ExecutedTask_FK_AutomaticTaskId] FOREIGN KEY([AutomaticTaskId])
REFERENCES [dbo].[AutomaticTask] ([AutomaticTaskId])

ALTER TABLE [dbo].[ExecutedTask]  WITH NOCHECK ADD CONSTRAINT [ExecutedTask_FK_UserId] FOREIGN KEY([ExecutedByUserId]) 
REFERENCES [dbo].[AppUser] ([UserId])
