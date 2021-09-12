IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[GetAccountIncludeHierarchy]')
	AND		type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )
)
BEGIN
	DROP FUNCTION [dbo].[GetAccountIncludeHierarchy]
END
GO

CREATE FUNCTION [dbo].[GetAccountIncludeHierarchy]
(
	@pAccountId INT
)
RETURNS @AccountIncludeTable TABLE
(
	AccountId INT
)
AS
BEGIN

	DECLARE @NextIncludesTable TABLE
	(
		AccountId INT
	)

	INSERT INTO @AccountIncludeTable
	SELECT acci.AccountIncludeId 
	FROM dbo.AccountInclude acci
	WHERE acci.AccountId = @pAccountId
	AND acci.AccountIncludeId NOT IN 
		(SELECT AccountId 
		FROM @AccountIncludeTable)

	INSERT INTO @NextIncludesTable
	SELECT accit.AccountId 
	FROM @AccountIncludeTable accit

	INSERT INTO @AccountIncludeTable
	SELECT gdra.AccountId
	FROM @NextIncludesTable nit
	CROSS APPLY GetAccountIncludeHierarchy(nit.AccountId) gdra
	WHERE gdra.AccountId NOT IN 
		(SELECT AccountId
		FROM @AccountIncludeTable)

	RETURN;
END
GO