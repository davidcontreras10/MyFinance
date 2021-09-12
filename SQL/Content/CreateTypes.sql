BEGIN
IF OBJECT_ID('AddSpendAccountsTable', 'U') IS NULL
	BEGIN
		CREATE TYPE [dbo].[AddSpendAccountsTable] AS TABLE
		(
			AccountId INT NOT NULL,
			Numerator FLOAT NOT NULL,
			Denominator FLOAT NOT NULL,
			PendingUpdate BIT NOT NULL,
			CurrencyConverterMethodId INT,
			IsOriginal BIT
		);
	END
END

BEGIN
IF OBJECT_ID('ClientAddSpendCurrencies', 'U') IS NULL
	BEGIN
		CREATE TYPE [dbo].[ClientAddSpendCurrencies] AS TABLE
		(
			AmountCurrencyId INT,
			AccountId INT,
			CurrencyConverterMethodId INT
		);
	END
END

BEGIN
IF OBJECT_ID('ClientAddSpendAccountInclude', 'U') IS NULL
	BEGIN
		CREATE TYPE [dbo].[ClientAddSpendAccountInclude] AS TABLE
		(
			AccountId INT,
			AmountCurrencyId INT,
			AccountIncludeId INT,
			CurrencyConverterMethodId INT
		);
	END
END

BEGIN
IF OBJECT_ID('IntArray', 'U') IS NULL
	BEGIN
		CREATE TYPE [dbo].[IntArray] AS TABLE
		(
			Value INT
		);
	END
END

BEGIN
IF OBJECT_ID('AccountOptionParametersTable', 'U') IS NULL
	BEGIN
		CREATE TYPE [dbo].[AccountOptionParametersTable] AS TABLE
		(
			AccountId INT NOT NULL,
			PendingSpends BIT NOT NULL DEFAULT 1,
			LoanSpends BIT NOT NULL DEFAULT 1
		);
	END
END