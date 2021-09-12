CREATE TABLE [dbo].[SpendType](
	[SpendTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
 CONSTRAINT [PK_SpendType] PRIMARY KEY CLUSTERED 
(
	[SpendTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
--

CREATE TABLE [dbo].[User](
	[Username] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Password] [nvarchar](500) NOT NULL
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Username] 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

CREATE TABLE [dbo].[Spend](
	[SpendId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Amount] [Float],
	[SpendDate] [DateTime],
	[SpendTypeId] [int]
 CONSTRAINT [PK_Spend] PRIMARY KEY CLUSTERED 
(
	[SpendId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 

ALTER TABLE [dbo].[Spend]  WITH NOCHECK ADD  CONSTRAINT [Spend_FK_SpendTypeId] FOREIGN KEY([SpendTypeId])
REFERENCES [dbo].[SpendType] ([SpendTypeId])

ALTER TABLE [dbo].[Spend]  WITH NOCHECK ADD  CONSTRAINT [Spend_FK_Username] FOREIGN KEY([Username])
REFERENCES [dbo].[User] ([Username])