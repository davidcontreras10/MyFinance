IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[GetBusinessDay]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[GetBusinessDay]
END
GO

CREATE FUNCTION [dbo].[GetBusinessDay]
(
	@pDate DATETIME,
	@pInvalidDates VARCHAR(500) = NULL
)
RETURNS DATETIME
AS
BEGIN
	DECLARE 
	@currentDay INT,
	@newDate DATETIME;
	SET @newDate=DATEFROMPARTS(DATEPART(YEAR,@pDate),DATEPART(MONTH,@pDate),DATEPART(DAY,@pDate));
	SET @currentDay = DATEPART(DW,@newDate);
	WHILE @currentDay = 1 OR @currentDay = 7
	BEGIN
		SET @newDate=DATEADD(DW,-1,@newDate);
		SET @currentDay = DATEPART(DW,@newDate);
	END
	RETURN @newDate;
END