IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[CreateWeekPeriod]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[CreateWeekPeriod]
END
GO

CREATE FUNCTION [dbo].[CreateWeekPeriod]
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
	@dayOfWeek INT,
	@daysDifference INT,
	@dayOfCutting INT;

    IF @pRepetition IS NULL OR @pRepetition = 0 
	BEGIN
		SET @pRepetition = 1
	END
	
	SET @pDate=DATEFROMPARTS(DATEPART(YEAR,@pDate),DATEPART(MONTH,@pDate),DATEPART(DAY,@pDate));
	
	--IF @pRepetition = 1
	--BEGIN
	--	SET @dayOfWeek = datepart(dw,@pDate);
	--END 
	SET @dayOfWeek = datepart(dw,@pDate);

	SET @dayOfCutting = CAST(@pCuttingDate as int);
	IF @dayOfCutting IS NULL OR @dayOfCutting = 0
	BEGIN
		SET @dayOfCutting=1;
	END
	IF @dayOfWeek >= @dayOfCutting
	BEGIN
		SET @daysDifference = @dayOfWeek-@dayOfCutting;
	END
	ELSE
	BEGIN
		SET @daysDifference = 7-(@dayOfCutting-@dayOfWeek);
	END
		SET @daysDifference*=-1;
	
	SET @initialDate=@pDate;
	SET @initialDate = DATEADD(dd,@daysDifference,@pDate);
	SET @endDate=DATEADD(WEEK,@pRepetition,@initialDate);
	
	INSERT INTO @TableWeek (InitialDate,EndDate) VALUES (@initialDate, @endDate);
    RETURN
END