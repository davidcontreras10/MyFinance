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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountEdit]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountEdit]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpAccountEdit
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

EXEC SpAccountEdit @pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676'

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountEdit]
@pUserId UNIQUEIDENTIFIER,
@pAccountId INT,
@pAccountName VARCHAR(500) = NULL,  --1
--@pPeriodDefinitionId INT = NULL,  --2
@pCurrencyId INT = NULL,  --3
@pBaseBudget FLOAT = NULL,  --4
@pHeaderColor VARCHAR(500) = NULL,  --5
@pAccountTypeId INT = NULL,  --6
@pSpendTypeId INT = NULL,  --7
@pFinancialEntityId INT = NULL,  --8,
@pAccountIncludeData VARCHAR(MAX) = NULL, --9
@pEditFields VARCHAR(MAX),
@pAccountGroupId INT = 1 --10
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================

DECLARE @EditFieldsTempTable TABLE(
	EditField INT
);

DECLARE @AccountIncludeTempTable TABLE(
	AccountIncludeId INT,
	CurrencyConverterMethodId INT
);

--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE CONSTANTS
--==============================================================================================================================================

DECLARE 
@ACCOUNT_NAME_FIELD INT = 1,
--@PERIOD_DEFINITION_ID_FIELD INT = 2,
--@CURRENCY_ID_FIELD INT = 3,
@BASE_BUDGET_FIELD INT = 4,
@HEADER_COLOR INT = 5,
@ACCOUNT_TYPE_ID_FIELD INT = 6,
@SPEND_TYPE_ID_FIELD INT = 7,
@FINANCIAL_ENTITY_ID_FIELD INT = 8,
@ACCOUNT_INCLUDE__FIELD INT = 9,
@ACCOUNT_GROUP__FIELD INT = 10;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @EditFieldsTempTable (EditField)
	SELECT CAST(accPer.part as int) 
	FROM dbo.SDF_SplitString(@pEditFields,',') accPer;

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @ACCOUNT_NAME_FIELD)
	BEGIN
		UPDATE dbo.Account SET Name = @pAccountName WHERE AccountId = @pAccountId;
	END

	--IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @PERIOD_DEFINITION_ID_FIELD)
	--BEGIN
	--	UPDATE dbo.Account SET PeriodDefinitionId = @pPeriodDefinitionId WHERE AccountId = @pAccountId;
	--END

	--IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @CURRENCY_ID_FIELD)
	--BEGIN
	--	UPDATE dbo.Account SET CurrencyId = @pCurrencyId WHERE AccountId = @pAccountId;
	--END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @BASE_BUDGET_FIELD)
	BEGIN
		UPDATE dbo.Account SET BaseBudget = @pBaseBudget WHERE AccountId = @pAccountId;
		DECLARE @CurrentAccountPeriod INT = dbo.GetCurrentAccountPeriodIdByAccountId(@pAccountId,NULL);
		UPDATE dbo.AccountPeriod SET Budget = @pBaseBudget WHERE AccountPeriodId = @CurrentAccountPeriod;
	END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @HEADER_COLOR)
	BEGIN
		UPDATE dbo.Account SET HeaderColor = @pHeaderColor WHERE AccountId = @pAccountId;
	END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @ACCOUNT_TYPE_ID_FIELD)
	BEGIN
		UPDATE dbo.Account SET AccountTypeId = @pAccountTypeId WHERE AccountId = @pAccountId;
	END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @SPEND_TYPE_ID_FIELD)
	BEGIN
		UPDATE dbo.Account SET DefaultSpendTypeId = @pSpendTypeId WHERE AccountId = @pAccountId;
	END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @FINANCIAL_ENTITY_ID_FIELD)
	BEGIN
		UPDATE acci SET acci.CurrencyConverterMethodId = ccm.CurrencyConverterMethodId
		FROM
		dbo.AccountInclude acci
		JOIN dbo.Account acc_from ON acc_from.AccountId = acci.AccountId
		JOIN dbo.Account acc_to ON acc_to.AccountId = acci.AccountIncludeId
		JOIN dbo.CurrencyConverter cc ON cc.CurrencyIdOne = acc_from.CurrencyId AND cc.CurrencyIdTwo = acc_to.CurrencyId
		JOIN dbo.CurrencyConverterMethod ccm ON cc.CurrencyConverterId = ccm.CurrencyConverterId
		WHERE acc_from.AccountId = @pAccountId AND ccm.FinancialEntityId = @pFinancialEntityId;

		UPDATE dbo.Account SET FinancialEntityId = @pFinancialEntityId WHERE AccountId = @pAccountId;
	END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @ACCOUNT_INCLUDE__FIELD)
	BEGIN
		
		IF @pAccountIncludeData IS NOT NULL AND @pAccountIncludeData <> ''
		BEGIN
			INSERT INTO @AccountIncludeTempTable (AccountIncludeId, CurrencyConverterMethodId)
			Select
			   max(case when name='AccountIncludeId' then convert(int,StringValue) else '' end),
			   max(case when name='CurrencyConverterMethodId' then convert(int,StringValue) else '' end)
			From parseJSON(@pAccountIncludeData)
			where ValueType = 'int'
			group by parent_ID;
		END

		DELETE FROM dbo.AccountInclude WHERE AccountId = @pAccountId;

		INSERT INTO dbo.AccountInclude (AccountId,AccountIncludeId,CurrencyConverterMethodId)
		SELECT @pAccountId, AccountIncludeId, CurrencyConverterMethodId
		FROM @AccountIncludeTempTable;
	END

	IF EXISTS (SELECT * FROM  @EditFieldsTempTable WHERE EditField = @ACCOUNT_GROUP__FIELD)
	BEGIN
		UPDATE dbo.Account SET AccountGroupId = @pAccountGroupId WHERE AccountId = @pAccountId;
	END

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