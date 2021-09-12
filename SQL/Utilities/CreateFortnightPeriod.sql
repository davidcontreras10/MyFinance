IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[CreateFortnightPeriod]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[CreateFortnightPeriod]
END
GO

CREATE FUNCTION [dbo].[CreateFortnightPeriod]
(
	@pDate DATETIME,
	@pLastDate DATETIME,
    @pCuttingDate NVARCHAR(500),
    @pRepetition INT = NULL
)
RETURNS @TableWeek TABLE ( InitialDate DATETIME, EndDate DATETIME )
AS
BEGIN
	DECLARE 
	@initialDate DATETIME,
	@endDate DATETIME,
	@dateTemp DATETIME,
	@cuttingDay1 INT,
	@cuttingDay2 INT,
	@lastDayLastMonth INT;

	DECLARE @daysOfCuttingTable TABLE (DayIndex INT IDENTITY(1,1),CuttingDay INT);

	INSERT INTO @daysOfCuttingTable (CuttingDay)
	SELECT CAST(parts.part as int) FROM dbo.SDF_SplitString(@pCuttingDate,',') parts
	ORDER BY CAST(parts.part as int);
	
	SELECT @cuttingDay1=t.CuttingDay FROM @daysOfCuttingTable t WHERE t.DayIndex=1;
	SELECT @cuttingDay2=t.CuttingDay FROM @daysOfCuttingTable t WHERE t.DayIndex=2;
	--DECLARE @v XML = (SELECT * FROM @daysOfCuttingTable FOR XML AUTO)

    IF @pRepetition IS NULL OR @pRepetition = 0 
	BEGIN
		SET @pRepetition = 1;
	END
	
	SET @pDate=DATEFROMPARTS(DATEPART(YEAR,@pDate),DATEPART(MONTH,@pDate),DATEPART(DAY,@pDate));
	
	IF @pRepetition = 1

	--LOGIC STARTS 
	SET @dateTemp=DATEADD(MONTH,-1,@pDate);
	SET @initialDate = DBO.CreateValidDate(DATEPART(YEAR,@dateTemp),DATEPART(MONTH,@dateTemp),@cuttingDay2);
	SET @initialDate = dbo.GetBusinessDay(@initialDate, NULL);

	SET @dateTemp=@pDate;
	SET @endDate = DBO.CreateValidDate(DATEPART(YEAR,@dateTemp),DATEPART(MONTH,@dateTemp),@cuttingDay1);
	SET @endDate = dbo.GetBusinessDay(@endDate, NULL);

	IF @pDate >= @initialDate AND @pDate < @endDate
	BEGIN
		INSERT INTO @TableWeek (InitialDate,EndDate) VALUES (@initialDate, @endDate);
	END
	ELSE
	BEGIN
		SET @dateTemp=@pDate;
		SET @initialDate = DBO.CreateValidDate(DATEPART(YEAR,@dateTemp),DATEPART(MONTH,@dateTemp),@cuttingDay1);
		SET @initialDate = dbo.GetBusinessDay(@initialDate, NULL);

		SET @dateTemp=@pDate;
		SET @endDate = DBO.CreateValidDate(DATEPART(YEAR,@dateTemp),DATEPART(MONTH,@dateTemp),@cuttingDay2);
		SET @endDate = dbo.GetBusinessDay(@endDate, NULL);

		IF @pDate >= @initialDate AND @pDate < @endDate
		BEGIN
			INSERT INTO @TableWeek (InitialDate,EndDate) VALUES (@initialDate, @endDate);
		END
		ELSE
		BEGIN
			SET @dateTemp=@pDate;
			SET @initialDate = DBO.CreateValidDate(DATEPART(YEAR,@dateTemp),DATEPART(MONTH,@dateTemp),@cuttingDay2);
			SET @initialDate = dbo.GetBusinessDay(@initialDate, NULL);

			SET @dateTemp=DATEADD(MONTH,1,@pDate);
			SET @endDate = DBO.CreateValidDate(DATEPART(YEAR,@dateTemp),DATEPART(MONTH,@dateTemp),@cuttingDay1);
			SET @endDate = dbo.GetBusinessDay(@endDate, NULL);
			IF @pDate >= @initialDate AND @pDate < @endDate
			BEGIN
				INSERT INTO @TableWeek (InitialDate,EndDate) VALUES (@initialDate, @endDate);
			END
		END
	END

    RETURN
END