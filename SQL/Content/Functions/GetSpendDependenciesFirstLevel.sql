IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[GetSpendDependenciesFirstLevel]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[GetSpendDependenciesFirstLevel]
END
GO

CREATE FUNCTION [dbo].[GetSpendDependenciesFirstLevel]
(
	@pSpendId INT,
	@pRestrictions IntArray READONLY
)
RETURNS @DependenciesSpendIds TABLE
(
	SpendId INT
)
AS
BEGIN
	
	DECLARE @Restriction IntArray;
	INSERT INTO @Restriction
	SELECT Value FROM @pRestrictions;

	INSERT INTO @Restriction (Value) VALUES (@pSpendId);

	IF EXISTS (SELECT TOP 1 * FROM dbo.LoanRecord lr WHERE lr.SpendId = @pSpendId)
	BEGIN
		INSERT INTO @DependenciesSpendIds
		SELECT ls.SpendId 
		FROM dbo.LoanRecord lr 
		JOIN dbo.LoanSpend ls ON ls.LoanRecordId = lr.LoanRecordId
		WHERE lr.SpendId = @pSpendId 
		AND ls.SpendId NOT IN (SELECT SpendId FROM @DependenciesSpendIds)
		AND ls.SpendId NOT IN (SELECT Value FROM @Restriction)
	END

	INSERT INTO @DependenciesSpendIds
	SELECT sd.DependencySpendId
	FROM dbo.SpendDependencies sd
	WHERE sd.SpendId = @pSpendId
	AND sd.DependencySpendId NOT IN (SELECT SpendId FROM @DependenciesSpendIds)
	AND sd.DependencySpendId NOT IN (SELECT Value FROM @Restriction);

	RETURN;

END