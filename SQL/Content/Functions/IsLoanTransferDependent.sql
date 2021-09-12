IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[IsLoanSpendDependent]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[IsLoanSpendDependent]
END
GO

CREATE FUNCTION [dbo].[IsLoanSpendDependent]
(
	@pSpendId INT
)
RETURNS BIT
AS
BEGIN
	DECLARE @Restrictions IntArray;
	DECLARE @SpendIdsEvaluate IntArray;
	INSERT INTO @SpendIdsEvaluate (Value) VALUES (@pSpendId);
	INSERT INTO @SpendIdsEvaluate (Value)
	SELECT r.SpendId FROM dbo.GetSpendDependencies(@pSpendId, @Restrictions) r;
	IF EXISTS (SELECT TOP 1 * FROM @SpendIdsEvaluate sp WHERE dbo.IsLoanSpendId(sp.Value) = 1)
		RETURN 1;
	RETURN 0;
END