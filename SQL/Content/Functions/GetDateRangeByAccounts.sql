IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[GetDateRangeByAccounts]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[GetDateRangeByAccounts]
END
GO
/*

BEGIN
	DECLARE @pAccounIds dbo.IntArray;
	INSERT INTO @pAccounIds VALUES (4)

	SELECT * FROM dbo.GetDateRangeByAccounts(null, @pAccounIds)
END


*/
CREATE FUNCTION [dbo].[GetDateRangeByAccounts]
(
	@pDate DATETIME = NULL,
	@pAccountIds IntArray READONLY
)
RETURNS @DateRangeTable TABLE
(
	MinDate DATETIME,
	MaxDate DATETIME,
	IsValid BIT,
	IsDateValid BIT,
	ActualDate DATETIME	
)
AS
BEGIN

	DECLARE
	@MinDate DATETIME = NULL,
	@MaxDate DATETIME = NULL,
	@IsValid BIT = 0,
	@IsDateValid BIT = NULL;

	DECLARE @AccountTempTable table( 
		AccountId int
	);


	DECLARE @AccountPeriodTempTable table( 
		AccountPeriodId int
	);

	INSERT INTO @AccountTempTable (AccountId)
	SELECT accids.Value 
	FROM @pAccountIds accids;

	IF EXISTS (SELECT * FROM @AccountTempTable)
	BEGIN		
		SELECT @MaxDate = MIN(tbl1.MaxDate) FROM 
			(SELECT accp.AccountId, accp.EndDate FROM
			@AccountTempTable acc
			JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId) tbl0
		JOIN
			(SELECT accp.AccountId, MAX(accp.EndDate) MaxDate FROM
			@AccountTempTable acc
			JOIN  dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
			GROUP BY accp.AccountId) tbl1 
		ON tbl1.AccountId = tbl0.AccountId
		WHERE tbl1.MaxDate = tbl0.EndDate

		SELECT @MinDate = MAX(tbl1.MinDate) FROM 
			(SELECT accp.AccountId, accp.InitialDate FROM
			@AccountTempTable acc
			JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId) tbl0
		JOIN
			(SELECT accp.AccountId, MIN(accp.InitialDate) MinDate FROM
			@AccountTempTable acc
			JOIN  dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
			GROUP BY accp.AccountId) tbl1 
		ON tbl1.AccountId = tbl0.AccountId
		WHERE tbl1.MinDate = tbl0.InitialDate

		SET @IsValid  = 1;

		IF @MaxDate IS NULL
		BEGIN
			SET @IsValid = 0
		END
		IF @MinDate IS NULL
		BEGIN
			SET @IsValid = 0
		END
	END

	SET @IsDateValid = NULL;
	IF @pDate IS NOT NULL
	BEGIN
		SET @IsDateValid = 1;
		IF @IsValid = 1 AND @pDate >= @MaxDate 
		BEGIN
			SET @IsDateValid = 0;
		END
		IF @IsValid = 1 AND @pDate < @MinDate 
		BEGIN
			SET @IsDateValid = 0;
		END
	END
	IF @IsDateValid = 0
	BEGIN
		SET @pDate = @MinDate; 
	END
	SET @MaxDate = DATEADD(SECOND, -1, @MaxDate);
	INSERT INTO @DateRangeTable (MinDate,	MaxDate,	IsValid, IsDateValid, ActualDate)
	VALUES						(@MinDate,	@MaxDate,	@IsValid, @IsDateValid, @pDate);


	RETURN;
END