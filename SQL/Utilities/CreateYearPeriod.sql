IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[CreateYearPeriod]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[CreateYearPeriod]
END
GO

CREATE FUNCTION [dbo].[CreateYearPeriod]
(
	@pDate DATETIME,
	@pLastDate DATETIME,
    @pCuttingDate NVARCHAR(500) = NULL,
    @pRepetition INT = NULL
)
RETURNS @TableWeek TABLE ( InitialDate DATETIME, EndDate DATETIME )
AS
BEGIN
	DECLARE 
	@initialDate DATETIME,
	@endDate DATETIME,
	@dayOfCutting INT,
	@monthOfCutting INT,
	@monthOfDate INT,
	@dayOfDate INT,
	@yearOfDate INT,
	@lessOrEqual BIT;

	DECLARE @CuttingDate TABLE(
		Id INT IDENTITY(1,1) NOT NULL,
		DateValue INT
	);

    IF @pRepetition IS NULL OR @pRepetition = 0 
	BEGIN
		SET @pRepetition = 1;
	END
	
	SET @pDate=DATEFROMPARTS(DATEPART(YEAR,@pDate),DATEPART(MONTH,@pDate),DATEPART(DAY,@pDate));
	SET @dayOfDate = DATEPART(DAY, @pDate);
	SET @monthOfDate = DATEPART(MONTH, @pDate);
	SET @yearOfDate = DATEPART(YEAR, @pDate);

	IF @pCuttingDate IS NULL OR @pCuttingDate = ''
	BEGIN
		SET @pCuttingDate='1-1';
	END

	INSERT INTO @CuttingDate (dateValue)
	SELECT CAST(dateVals.part as int) 
	FROM dbo.SDF_SplitString(@pCuttingDate,'-') dateVals;

	SELECT @monthOfCutting = DateValue
	FROM @CuttingDate 
	WHERE Id = 1;

	SELECT @dayOfCutting = DateValue
	FROM @CuttingDate 
	WHERE Id = 2;

	IF @monthOfCutting = NULL 
	BEGIN
		SET @monthOfCutting = 1;
	END
	IF @dayOfCutting = NULL
	BEGIN
		SET @dayOfCutting = 1;
	END

	--LOGIC STARTS 
	
	IF @monthOfDate > @monthOfCutting 
	BEGIN
		SET @lessOrEqual = 0;
	END
	ELSE
	BEGIN
		IF @monthOfDate = @monthOfCutting
		BEGIN
			IF @dayOfDate <= @dayOfCutting 
			BEGIN
				SET @lessOrEqual = 1;
			END
			ELSE
			BEGIN
				SET @lessOrEqual = 0;
			END
		END
		ELSE
		BEGIN
			SET @lessOrEqual = 0;
		END
	END

	IF @lessOrEqual = 0
	BEGIN
		SET @initialDate = DATEFROMPARTS(@yearOfDate, @monthOfCutting, @dayOfCutting);
		SET @endDate = DATEFROMPARTS(@yearOfDate+1, @monthOfCutting, @dayOfCutting);
	END
	ELSE
	BEGIN
		SET @initialDate = DATEFROMPARTS(@yearOfDate-1, @monthOfCutting, @dayOfCutting);
		SET @endDate = DATEFROMPARTS(@yearOfDate, @monthOfCutting, @dayOfCutting);
	END

	INSERT INTO @TableWeek (InitialDate, EndDate) VALUES (@initialDate, @endDate);
    RETURN
END