IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[IsSpendIdValidByConditions]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[IsSpendIdValidByConditions]
END
GO

CREATE FUNCTION [dbo].[IsSpendIdValidByConditions]
(
	@pSpendId INT,
	@pAccountPeriodId INT,
	@pConditions AccountPeriodOptionParametersTable READONLY
)
RETURNS BIT
AS
BEGIN
	DECLARE 
	@LoanSpends BIT,
	@PendingSpends BIT,
	@AmountTypeId INT;

	SELECT TOP 1
		@LoanSpends = c.LoanSpends,
		@PendingSpends = c.PendingSpends,
		@AmountTypeId = c.AmountTypeId
	FROM @pConditions c
	WHERE c.AccountPeriodId = @pAccountPeriodId;
	
	IF EXISTS (
		SELECT TOP 1 * 
		FROM dbo.Spend sp
		WHERE 
			sp.SpendId = @pSpendId
			AND (@AmountTypeId = 0 OR sp.AmountTypeId = @AmountTypeId)
			AND (@PendingSpends = 1 OR sp.IsPending = 0)
			AND (@LoanSpends = 1 OR dbo.IsLoanSpendId(sp.SpendId) = 0))
	BEGIN
		RETURN 1;
	END

	RETURN 0;
END