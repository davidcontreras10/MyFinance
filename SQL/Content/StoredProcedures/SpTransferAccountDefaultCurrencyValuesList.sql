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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpTransferAccountDefaultCurrencyValuesList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpTransferAccountDefaultCurrencyValuesList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpTransferAccountDefaultCurrencyValuesList
--	Type:						Stored Procedure
--	Editor Tab Spacing:	4	
--==============================================================================================================================================
--	DESCRIPTION: 
--	The first result set will be used to return the parent menu option. 
--  The second result set will return the list of menu options with parent id equals to the one provided to the stored procedure.
--	The third result set will return the list of parameters for all menu options.
--==============================================================================================================================================
--	BUSINESS RULES:
--	Enter the business rules in this section...
--	1.	Declare Variables 
--	2.	Declare Tables 
--	3.	Initialize Variables 
--	4.	Validate Input Parameters
--	5.	Retrieve Menu Items data
--	6.	Retrieve Menu Item Parameters data
--	7.	Trap Errors
--==============================================================================================================================================
--	EDIT HISTORY:
------------------------------------------------------------------------------------------------------------------------------------------------
--	Revision	Date			Who						What
--	========	====			===						====
--	1.0			2013-04-19		David Contreras			Initial Development

--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

	exec SpTransferAccountDefaultCurrencyValuesList @pAccountPeriodId=1, @pUserId=N'test', @pCurrencyId = 1
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpTransferAccountDefaultCurrencyValuesList]
@pAccountId INT = NULL,
@pAccountPeriodId INT = NULL,
@pUserId UNIQUEIDENTIFIER,
@pCurrencyId INT
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountIncludeDataRt TABLE(
	AccountId INT,
	CurrencyConverterMethodId INT
);

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================

DECLARE @SupportedAccountsTmp TABLE(
	AccountId INT,
	CurrencyId INT,
	FinancialEntityId INT
);

--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

DECLARE 
@supportedAccountsCurs CURSOR,
@currentAccountId INT,
@currentCurrencyId INT,
@currentFinancialEntityId INT,
@inputAccountId INT,
@currencyConverterMethodId INT;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF @pAccountId IS NULL
	BEGIN
		IF @pAccountPeriodId IS NULL
		BEGIN
			raiserror ('@pAccountPeriodId cannot be null when @pAccountId is null', 20, -1) with log;
		END
		SELECT @inputAccountId = accp.AccountId FROM dbo.AccountPeriod accp WHERE accp.AccountPeriodId = @pAccountPeriodId;
	END
	ELSE
	BEGIN
		SET @inputAccountId = @pAccountId;
	END

	INSERT INTO @SupportedAccountsTmp (AccountId, CurrencyId, FinancialEntityId) 
	SELECT acc.AccountId, acc.CurrencyId, acc.FinancialEntityId FROM dbo.Account acc 
	WHERE acc.AccountId = @inputAccountId;
	
	INSERT INTO @SupportedAccountsTmp (AccountId, CurrencyId, FinancialEntityId)
	SELECT acci.AccountIncludeId, acc.CurrencyId, acc.FinancialEntityId FROM
		dbo.AccountInclude acci
	JOIN dbo.Account acc ON acc.AccountId = acci.AccountIncludeId 
	WHERE
		acci.AccountId = @inputAccountId;

	--SELECT * FROM @SupportedAccountsTmp;

	SET @supportedAccountsCurs = CURSOR FOR SELECT AccountId, CurrencyId, FinancialEntityId FROM @SupportedAccountsTmp;
	OPEN @supportedAccountsCurs;

	FETCH NEXT FROM @supportedAccountsCurs INTO @currentAccountId, @currentCurrencyId, @currentFinancialEntityId;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @currencyConverterMethodId = NULL;
		IF @pCurrencyId = @currentCurrencyId
		BEGIN
			SET @currencyConverterMethodId = NULL;
			SELECT TOP 1 @currencyConverterMethodId = ccm.CurrencyConverterMethodId FROM 
			dbo.CurrencyConverterMethod ccm
			JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
			WHERE cc.CurrencyIdOne = @pCurrencyId AND cc.CurrencyIdTwo = @pCurrencyId;
			IF @currencyConverterMethodId IS NULL
			BEGIN
				raiserror ('@currencyConverterMethodId not found 1', 20, -1) with log
			END
		END
		ELSE
		BEGIN
			SET @currencyConverterMethodId = NULL;
			SELECT @currencyConverterMethodId = acci.CurrencyConverterMethodId FROM 
			dbo.AccountInclude acci
			JOIN dbo.CurrencyConverterMethod ccm ON ccm.CurrencyConverterMethodId = acci.CurrencyConverterMethodId
			JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterMethodId
			WHERE acci.AccountId = @inputAccountId AND acci.AccountIncludeId = @currentAccountId AND
			cc.CurrencyIdOne = @pCurrencyId AND cc.CurrencyIdTwo = @currentCurrencyId
			IF @currencyConverterMethodId IS NULL
			BEGIN
				SELECT TOP 1 @currencyConverterMethodId = ccm.CurrencyConverterMethodId 
				FROM dbo.CurrencyConverterMethod ccm
				JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
				WHERE cc.CurrencyIdOne = @pCurrencyId AND cc.CurrencyIdTwo = @currentCurrencyId AND
				ccm.FinancialEntityId = @currentFinancialEntityId;
				IF @currencyConverterMethodId IS NULL
				BEGIN
					raiserror ('@currencyConverterMethodId not found 2', 20, -1) with log;
				END
			END
		END
		INSERT INTO @AccountIncludeDataRt (AccountId, CurrencyConverterMethodId)
		VALUES							  (@currentAccountId, @currencyConverterMethodId);
		FETCH NEXT FROM @supportedAccountsCurs INTO @currentAccountId, @currentCurrencyId, @currentFinancialEntityId;
	END
	
	IF EXISTS (SELECT * FROM @AccountIncludeDataRt rt 
		WHERE rt.AccountId = 0 OR rt.AccountId IS NULL OR rt.CurrencyConverterMethodId = 0 OR rt.CurrencyConverterMethodId IS NULL)
	BEGIN
		raiserror ('@AccountIncludeDataRt contains invalid values', -20, -1);
	END

	SELECT * FROM @AccountIncludeDataRt;

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