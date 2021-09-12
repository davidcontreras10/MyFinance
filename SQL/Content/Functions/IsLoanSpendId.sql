IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[IsLoanSpendId]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[IsLoanSpendId]
END
GO

CREATE FUNCTION [dbo].[IsLoanSpendId]
(
	@pSpendId INT
)
RETURNS BIT
AS
BEGIN
	IF EXISTS (SELECT TOP 1 * FROM dbo.LoanRecord lr WHERE lr.SpendId = @pSpendId) 
		OR EXISTS (SELECT TOP 1 * FROM dbo.LoanSpend ls WHERE ls.SpendId = @pSpendId)
	BEGIN
		RETURN 1;
	END
	RETURN 0;
END