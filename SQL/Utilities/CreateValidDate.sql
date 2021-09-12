IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[CreateValidDate]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[CreateValidDate]
END
GO

CREATE FUNCTION [dbo].[CreateValidDate]
(
	@pYear INT,
	@pMonth INT,
	@pDay INT
)
RETURNS DATETIME
AS
BEGIN
	DECLARE
	@newDate DATETIME,
	@tempDay INT;
	SET @newDate = DATEFROMPARTS(@pYear,@pMonth,1);
	SET @newDate = DATEADD(MONTH,1,@newDate);
	SET @newDate = DATEADD(DAY,-1,@newDate);
	SET @tempDay = DATEPART(DAY,@newDate);
	IF @pDay > @tempDay
	BEGIN
		SET @pDay = @tempDay;
	END
	SET @newDate = DATEFROMPARTS(@pYear,@pMonth,@pDay);
    RETURN @newDate
END