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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountDefaultCurrencyValuesList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountDefaultCurrencyValuesList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpAccountDefaultCurrencyValuesList
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

	exec SpAccountDefaultCurrencyValuesList @pAccountId=1, @pUserId=N'test', @pCurrencyId = 1
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountDefaultCurrencyValuesList]
@pAccountId INT,
@pUserId UNIQUEIDENTIFIER,
@pAmountCurrencyId INT,
@pDestinationCurrencyId INT
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


--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

DECLARE 
@financialInstitutionId INT,
@currencyConverterMethodId INT;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	SELECT @financialInstitutionId = FinancialEntityId FROM dbo.Account WHERE AccountId = @pAccountId;
	IF @pAmountCurrencyId = @pDestinationCurrencyId
	BEGIN
		SELECT TOP 1 @currencyConverterMethodId = ccm.CurrencyConverterMethodId 
		FROM dbo.CurrencyConverterMethod ccm
		JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
		WHERE cc.CurrencyIdOne = @pAmountCurrencyId AND cc.CurrencyIdTwo = @pAmountCurrencyId;
	END
	ELSE
	BEGIN
		SELECT TOP 1 @currencyConverterMethodId = ccm.CurrencyConverterMethodId 
		FROM dbo.CurrencyConverterMethod ccm
		JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
		WHERE 
		cc.CurrencyIdOne = @pAmountCurrencyId AND 
		cc.CurrencyIdTwo = @pDestinationCurrencyId AND
		ccm.FinancialEntityId = @financialInstitutionId;
	END

	INSERT INTO @AccountIncludeDataRt (AccountId,	CurrencyConverterMethodId) 
	VALUES							  (@pAccountId,	@currencyConverterMethodId);

	IF EXISTS (SELECT * FROM @AccountIncludeDataRt rt 
		WHERE rt.AccountId = 0 OR rt.AccountId IS NULL OR rt.CurrencyConverterMethodId = 0 OR rt.CurrencyConverterMethodId IS NULL)
	BEGIN
		raiserror ('@AccountIncludeDataRt contains invalid values', 20, -1); 
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