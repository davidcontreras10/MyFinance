
CREATE TABLE [dbo].[AutomaticTask](
	[AutomaticTaskId] [UNIQUEIDENTIFIER] NOT NULL,
	[TaskDescription] [nvarchar](400),
	[TrxTypeId] [INT] NOT NULL,
	[FreqTypeId] [INT] NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL
 CONSTRAINT [PK_AutomaticTask] PRIMARY KEY CLUSTERED 
(
	[AutomaticTaskId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[AutomaticTask] ADD CONSTRAINT [AutomaticTask_Unq_AutomaticTaskId] UNIQUE ([AutomaticTaskId]);

ALTER TABLE [dbo].[AutomaticTask]  WITH NOCHECK ADD CONSTRAINT [AutomaticTask_FK_UserId] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[AppUser] ([UserId])

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[SpInTrxDef](
	[SpInTrxDefId] [UNIQUEIDENTIFIER] NOT NULL,
	[TrxAmount] [Float] NOT NULL,
	[SpendTypeId] [INT] NOT NULL,
	[AmountCurrencyId] [INT] NOT NULL,
	[IsSpendTrx] [BIT] NOT NULL,
	[AccountId] [INT]
 CONSTRAINT [PK_SpInTrxDef] PRIMARY KEY CLUSTERED 
(
	[SpInTrxDefId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[SpInTrxDef] ADD CONSTRAINT [SpInTrxDef_Unq_SpInTrxDefId] UNIQUE ([SpInTrxDefId]);

ALTER TABLE [dbo].[SpInTrxDef] WITH NOCHECK ADD CONSTRAINT [SpInTrxDef_FK_SpendTypeId] FOREIGN KEY([SpendTypeId])
REFERENCES [dbo].[SpendType] ([SpendTypeId])

ALTER TABLE [dbo].[SpInTrxDef] WITH NOCHECK ADD CONSTRAINT [SpInTrxDef_FK_AutomaticTaskId] FOREIGN KEY([SpInTrxDefId])
REFERENCES [dbo].[AutomaticTask] ([AutomaticTaskId])

ALTER TABLE [dbo].[SpInTrxDef] WITH NOCHECK ADD CONSTRAINT [SpInTrxDef_FK_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Account] ([AccountId])

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[TransferTrxDef](
	[TransferTrxDefId] [UNIQUEIDENTIFIER] NOT NULL,
	[TrxAmount] [Float] NOT NULL,
	[SpendTypeId] [INT] NOT NULL,
	[AmountCurrencyId] [INT] NOT NULL,
	[FromAccountId] [INT] NOT NULL,
	[ToAccountId] [INT] NOT NULL
 CONSTRAINT [PK_TransferTrxDef] PRIMARY KEY CLUSTERED 
(
	[TransferTrxDefId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[TransferTrxDef] ADD CONSTRAINT [TransferTrxDef_Unq_TransferTrxDefId] UNIQUE ([TransferTrxDefId]);

ALTER TABLE [dbo].[TransferTrxDef] WITH NOCHECK ADD CONSTRAINT [TransferTrxDef_FK_SpendTypeId] FOREIGN KEY([SpendTypeId])
REFERENCES [dbo].[SpendType] ([SpendTypeId])

ALTER TABLE [dbo].[TransferTrxDef] WITH NOCHECK ADD CONSTRAINT [TransferTrxDef_FK_AutomaticTaskId] FOREIGN KEY([TransferTrxDefId])
REFERENCES [dbo].[AutomaticTask] ([AutomaticTaskId])

ALTER TABLE [dbo].[TransferTrxDef] WITH NOCHECK ADD CONSTRAINT [TransferTrxDef_FK_FromAccountId] FOREIGN KEY([FromAccountId])
REFERENCES [dbo].[Account] ([AccountId])

ALTER TABLE [dbo].[TransferTrxDef] WITH NOCHECK ADD CONSTRAINT [TransferTrxDef_FK_ToAccountId] FOREIGN KEY([ToAccountId])
REFERENCES [dbo].[Account] ([AccountId])

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[SpDaysTrxTaskFreq](
	[SpDaysTrxTaskFreqId] [UNIQUEIDENTIFIER] NOT NULL,
	[PeriodTypeId] [INT] NOT NULL,
	[Days] [NVARCHAR] NOT NULL
 CONSTRAINT [PK_SpDaysTrxTaskFreq] PRIMARY KEY CLUSTERED 
(
	[SpDaysTrxTaskFreqId] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[SpDaysTrxTaskFreq] ADD CONSTRAINT [SpDaysTrxTaskFreq_Unq_SpDaysTrxTaskFreqId] UNIQUE ([SpDaysTrxTaskFreqId]);

ALTER TABLE [dbo].[SpDaysTrxTaskFreq] WITH NOCHECK ADD CONSTRAINT [SpDaysTrxTaskFreq_FK_AutomaticTaskId] FOREIGN KEY([SpDaysTrxTaskFreqId])
REFERENCES [dbo].[AutomaticTask] ([AutomaticTaskId])

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
