IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[CreateMonthPeriod]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[CreateMonthPeriod]
END
GO

CREATE FUNCTION [dbo].[CreateMonthPeriod]
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
	@dayOfMonth INT,
	@daysDifference INT,
	@dayOfCutting INT,
	@lastDayLastMonth INT;

    IF @pRepetition IS NULL OR @pRepetition = 0 
	BEGIN
		SET @pRepetition = 1;
	END
	
	SET @pDate=DATEFROMPARTS(DATEPART(YEAR,@pDate),DATEPART(MONTH,@pDate),DATEPART(DAY,@pDate));
	
	IF @pRepetition = 1
	BEGIN
		SET @dayOfMonth = datepart(DAY,@pDate);
	END 
	--LOGIC STARTS 
	SET @dayOfCutting = CAST(@pCuttingDate as int);
	IF @dayOfCutting IS NULL OR @dayOfCutting = 0
	BEGIN
		SET @dayOfCutting=1;
	END
	IF @dayOfMonth >= @dayOfCutting
	BEGIN
		SET @daysDifference=@dayOfMonth-@dayOfCutting;
	END
	ELSE
	BEGIN
		SET @lastDayLastMonth = DATEPART(DAY,DATEADD(MONTH, DATEDIFF(MONTH, -1, @pDate)-1, -1));
		SET	@daysDifference = @dayOfMonth+(@lastDayLastMonth-@dayOfCutting);
	END
	SET @daysDifference*=-1;
	SET @initialDate=@pDate;
	SET @initialDate = DATEADD(dd,@daysDifference,@pDate);
	SET @endDate=DATEADD(MONTH,1,@initialDate);
	
	INSERT INTO @TableWeek (InitialDate,EndDate) VALUES (@initialDate, @endDate);
    RETURN
END