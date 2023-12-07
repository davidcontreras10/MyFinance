SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--==============================================================================================================================================
--	CHECK to see if stored procedure already exists in the database
--==============================================================================================================================================
IF	EXISTS	
(
	SELECT	* 
	FROM	dbo.SYSOBJECTS 
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountPossibleSpendCurrenciesList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountPossibleSpendCurrenciesList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

	exec SpAccountPossibleSpendCurrenciesList @pAccountId=1, @pUserId='71722361-99FF-493F-AF02-2BD0ED7CE676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountPossibleSpendCurrenciesList]
@pAccountId INT,
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON




--==============================================================================================================================================
--	DECLARE VARIABLES	
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE @includeCount INT;
DECLARE @alternateCurrencyId INT;
DECLARE @accountCurrencyId INT;

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================

DECLARE @CurrencyRtTemp TABLE(
	AccountId INT,
	CurrencyId INT,
	CurrencyName VARCHAR(500),
	CurrencySymbol VARCHAR(10),
	IsDefault BIT
);


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--==============================================================================================================================================

DECLARE @ValidCurrencies TABLE(
CurrencyId INT
);


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF NOT EXISTS (
	SELECT * FROM dbo.AccountInclude 
	WHERE AccountId = @pAccountId AND CurrencyConverterMethodId IS NULL)
	BEGIN
		INSERT INTO @ValidCurrencies (CurrencyId)
			SELECT CurrencyId FROM dbo.Account 
			WHERE AccountId = @pAccountId;
	END

	SELECT @accountCurrencyId = CurrencyId FROM dbo.Account WHERE AccountId = @pAccountId;

	SELECT @includeCount = COUNT(DISTINCT acc.CurrencyId)
	FROM dbo.AccountInclude acci 
	JOIN dbo.Account acc ON acc.AccountId = acci.AccountIncludeId
	WHERE acci.AccountId = @pAccountId;

	IF @includeCount = 1 
	BEGIN
		SELECT TOP 1 @alternateCurrencyId = acc.CurrencyId
		FROM dbo.AccountInclude acci 
		JOIN dbo.Account acc ON acc.AccountId = acci.AccountIncludeId
		WHERE acci.AccountId = @pAccountId
		IF EXISTS (
			SELECT * 
			FROM dbo.CurrencyConverterMethod ccm
			JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
			WHERE cc.CurrencyIdOne = @alternateCurrencyId AND cc.CurrencyIdTwo = @accountCurrencyId
			AND ccm.FinancialEntityId IN (SELECT FinancialEntityId FROM dbo.Account WHERE AccountId = @pAccountId)
		)
		BEGIN
			INSERT INTO @ValidCurrencies (CurrencyId) VALUES (@alternateCurrencyId);
		END
	END	

	INSERT INTO @ValidCurrencies
	SELECT DISTINCT (cc.CurrencyIdTwo) FROM dbo.Account acc, dbo.CurrencyConverterMethod ccm
	JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
	WHERE acc.AccountId = @pAccountId 
		AND cc.CurrencyIdOne = acc.CurrencyId 
		AND acc.FinancialEntityId = ccm.FinancialEntityId
		AND cc.CurrencyIdTwo NOT IN (SELECT CurrencyId FROM @ValidCurrencies);

	INSERT INTO @CurrencyRtTemp (AccountId,CurrencyId, CurrencyName, CurrencySymbol)
	SELECT @pAccountId, crry.CurrencyId, crry.Name, crry.Symbol 
	FROM dbo.Currency crry
	WHERE crry.CurrencyId IN (SELECT CurrencyId FROM @ValidCurrencies);

	UPDATE @CurrencyRtTemp SET IsDefault = 1
	FROM dbo.Account acc
	JOIN @CurrencyRtTemp ctmp ON ctmp.AccountId = acc.AccountId
	WHERE ctmp.CurrencyId = acc.CurrencyId;

	SELECT * FROM @CurrencyRtTemp;

END TRY
BEGIN CATCH
	--rethrows exception
    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH	
--==============================================================================================================================================
--	TRAP Errors
--	Set return code and error id output values
--==============================================================================================================================================
ERRORFinish:

--IF	EXISTS	
--(
--	SELECT	Error_Id
--	FROM	dbo.Local_SSI_ErrorLogHeader	WITH(NOLOCK)
--	WHERE	Error_Id = @op_ErrorGUID
--	AND		Primary_Object_Name	= @ObjectName
--)
--BEGIN
--	SELECT	@ReturnCode	= MIN(Error_Severity_Level)
--	FROM	dbo.Local_SSI_ErrorLogDetail	WITH(NOLOCK)
--	WHERE	Error_Id		= @op_ErrorGUID
--	AND		[Object_Name]	= @ObjectName
--	RETURN	@ReturnCode
--END
--ELSE
--BEGIN
--	IF	@Primary	=	1
--	BEGIN
--		SET @op_ErrorGUID = NULL
--	END
--    RETURN @ERROR_NONE
--END

--==============================================================================================================================================
--	RETURN CODE
--==============================================================================================================================================
SET NOCOUNT OFF
RETURN
GO
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO