/****** Script for SelectTopNRows command from SSMS  ******/


	INSERT INTO [dbo].[PeriodType]	(Name)		
	VALUES							('Weekly'),	
									('Monthly'),
									('Fortnightly'),
									('Manual')


	INSERT INTO [dbo].[AppUser] (Username, Name, Password) VALUES ('test','test','test')

	INSERT INTO [dbo].[PeriodDefinition] (PeriodTypeId, CuttingDate, Repetition)
	VALUES								 (1,			 '2',         1),
										 (2,			 '1',         1), 
										 (3,			 '15,30',     1),
										 (3,			 '8,23',      1)
	
	INSERT INTO [dbo].[Currency] (CurrencyId,	Name)
	VALUES						 (1,			'Colones'),
								 (2,			'Dolares'),
								 (3,			'Euros')
				

	INSERT INTO [dbo].[Account]	(Name,					BaseBudget,		PeriodDefinitionId,		Username,		Position,	CurrencyId)
	VALUES						('Weekly',				50000,			1,						'test',			1,			1		),
								('Monthly',				720,			2,						'test',			2,			2		),
								('Combustible',			95000,			2,						'test',			3,			1		),	
								('Cable & Internet',	35000,			2,						'test',			4,			1		),
								('Teléfonos',			30000,			2,						'test',			5,			1		)


	------------------------------------------------------------------------------------------------------------------------------------

		INSERT INTO [dbo].[PeriodType]	(Name)		
	VALUES							('Weekly'),	
									('Monthly'),
									('Fortnightly'),
									('Manual')


	INSERT INTO [dbo].[AppUser] (Username, Name, Password) VALUES ('test','test','test')

	INSERT INTO [dbo].[PeriodDefinition] (PeriodTypeId, CuttingDate, Repetition)
	VALUES								 (1,			 '2',         1),
										 (2,			 '1',         1), 
										 (3,			 '15,30',     1),
										 (3,			 '8,23',      1)
	
	INSERT INTO [dbo].[Currency] (Name)
	VALUES						 ('Colones'),
								 ('Dolares'),
								 ('Euros')
				
	INSERT INTO [dbo].[CurrencyConverter] (CurrencyIdOne, CurrencyIdTwo)
	VALUES								  (1,			  2),--1
										  (2,			  1),--2
										  (1,			  1),--3
										  (2,			  2),--4
										  (1,			  3),--5
										  (3,			  1),--5
										  (3,			  3)--6

	INSERT INTO [dbo].[CurrencyConverterMethod] (CurrencyConverterId, Name)
	VALUES										(1,					  'Bac SJ Col-Dol'),
												(2,					  'Bac SJ Dol-Col'),
												(3,					  'Colones default'),
												(4,					  'Dolares default'),
												(5,					  'Bac SJ Col-Eur'),
												(6,					  'Bac SJ Eur-Col'),
												(7,					  'Euros default'),
												(1,					  'BCCR Col-Dol'),
												(2,					  'BCCR Dol-Col')
	
	INSERT INTO [dbo].[Account]	(Name,					BaseBudget,		PeriodDefinitionId,		Username,		Position,	CurrencyId)
	VALUES						('Weekly',				50000,			1,						'test',			1,			1		),
								('Monthly',				720,			2,						'test',			2,			2		),
								('Combustible',			95000,			2,						'test',			3,			1		),	
								('Cable & Internet',	35000,			2,						'test',			4,			1		),
								('Teléfonos',			30000,			2,						'test',			5,			1		)

	
	INSERT INTO [dbo].[AccountInclude] (AccountId, AccountIncludeId, CurrencyConverterMethodId)
	VALUES						       (1,		   2,				 1),
									   (3,		   2,				 1),
									   (4,		   2,				 1),
									   (5,		   2,				 1)		

	INSERT INTO dbo.SpendType	 (Name,		Description)
	VALUES						 ('Other',	'Other'),
								 ('Food',	'Other'),
								 ('Sports',	'Other'),
								 ('Personal',''),
								 ('ATM',''),
								 ('Cable/Internet Bill',''),
								 ('Phone Bill',''),
								 ('Fuel','');

								 	INSERT INTO [dbo].[Account]	(Name,					BaseBudget,		PeriodDefinitionId,		Username,		Position,	CurrencyId, AccountType)
	VALUES						('AHORRO MENSUAL',		0,				2,						'dcontre10',	10,			2,			2),
								('Ahorro Libre',		0,				2,						'dcontre10',	11,			2,			2),
								('Ahorro Objetivo',		0,				2,						'dcontre10',	12,			2,			2),
								('Ahorro Sobras',		0,				2,						'dcontre10',	13,			2,			2)


								INSERT INTO AccountInclude (AccountId,	AccountIncludeId,	CurrencyConverterMethodId)
			VALUES		   (2004,		2003,				4),
						   (2005,		2003,				4),
						   (2006,		2003,				4)
