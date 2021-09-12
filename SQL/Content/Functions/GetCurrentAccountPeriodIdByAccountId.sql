IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[GetCurrentAccountPeriodIdByAccountId]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[GetCurrentAccountPeriodIdByAccountId]
END
GO

CREATE FUNCTION [dbo].[GetCurrentAccountPeriodIdByAccountId]
(
	@pAccountId INT,
	@pDate DATETIME 
)
RETURNS INT
AS
BEGIN
	DECLARE @AccountPeriodId INT;

	IF @pDate IS NULL
		SET @pDate = GETDATE();
	
	SELECT @AccountPeriodId = ap.AccountPeriodId FROM AccountPeriod ap 
	JOIN dbo.Account acc ON acc.AccountId = ap.AccountId
	WHERE
		ap.AccountId in (Select max(ap2.AccountId) FROM AccountPeriod ap2 GROUP BY ap2.AccountPeriodId) AND 
		@pDate >= ap.InitialDate AND @pDate < ap.EndDate AND
		acc.AccountId=@pAccountId;

	RETURN @AccountPeriodId;
END