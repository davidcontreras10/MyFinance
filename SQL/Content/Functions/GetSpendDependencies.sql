IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[GetSpendDependencies]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[GetSpendDependencies]
END
GO

CREATE FUNCTION [dbo].[GetSpendDependencies]
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
	
	DECLARE @LoopCount INT = 1;
	DECLARE @AnalyseCount INT;
	DECLARE @CurrentSpendId INT;
	DECLARE @AnalyseTable TABLE
	(
		Id INT IDENTITY (1,1) NOT NULL,
		SpendId INT
	);

	DECLARE @TempDependenciesSpendIds TABLE
	(
		SpendId INT
	);

	DECLARE @Restriction IntArray;
	INSERT INTO @Restriction
	SELECT Value FROM @pRestrictions;

	INSERT INTO @Restriction (Value) VALUES (@pSpendId);

	INSERT INTO @DependenciesSpendIds
	SELECT r.SpendId 
	FROM dbo.GetSpendDependenciesFirstLevel(@pSpendId, @Restriction) r
	
	INSERT INTO @Restriction
	SELECT d.SpendId
	FROM @DependenciesSpendIds d;

	INSERT INTO @AnalyseTable (SpendId)
	SELECT spd.SpendId 
	FROM @DependenciesSpendIds spd

	SELECT @AnalyseCount = COUNT(*) 
	FROM @AnalyseTable at;

	WHILE @LoopCount <= @AnalyseCount
	BEGIN
		SELECT @CurrentSpendId = at.SpendId
		FROM @AnalyseTable at
		WHERE at.Id = @LoopCount;

		DELETE FROM @TempDependenciesSpendIds;
		INSERT INTO @TempDependenciesSpendIds (SpendId)
		SELECT r.SpendId 
		FROM dbo.GetSpendDependenciesFirstLevel(@CurrentSpendId, @Restriction) r;

		INSERT INTO @AnalyseTable (SpendId)
		SELECT tmp.SpendId
		FROM @TempDependenciesSpendIds tmp;

		SELECT @AnalyseCount = COUNT(*) 
		FROM @AnalyseTable at;
		SET @LoopCount = @LoopCount + 1;

		INSERT INTO @DependenciesSpendIds (SpendId)
		SELECT tmp.SpendId
		FROM @TempDependenciesSpendIds tmp;

		INSERT INTO @Restriction (Value)
		SELECT tmp.SpendId
		FROM @TempDependenciesSpendIds tmp;
	END

	RETURN;

END