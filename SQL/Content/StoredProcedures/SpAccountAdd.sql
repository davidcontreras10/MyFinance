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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountAdd]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountAdd]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpAccountAdd
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

EXEC SpAccountAdd @pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676'

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountAdd]
@pUserId UNIQUEIDENTIFIER,
@pAccountName VARCHAR(500),  --1
@pPeriodDefinitionId INT,  --2
@pCurrencyId INT,  --3
@pBaseBudget FLOAT,  --4
@pHeaderColor VARCHAR(500),  --5
@pAccountTypeId INT,  --6
@pSpendTypeId INT = NULL,  --7
@pFinancialEntityId INT = NULL,  --8,
@pAccountIncludeData VARCHAR(MAX) = NULL, --9
@pAccountGroupId INT = 1 --10
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================

DECLARE @CreatedAccount TABLE(
	AccountId INT
);

DECLARE @AccountIncludeTempTable TABLE(
	AccountIncludeId INT,
	CurrencyConverterMethodId INT
);

--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================
DECLARE 
@newAccountId INT;

--==============================================================================================================================================
--	DECLARE CONSTANTS
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO dbo.Account (Name, PeriodDefinitionId, CurrencyId, BaseBudget, HeaderColor, AccountTypeId, 
		DefaultSpendTypeId, FinancialEntityId, UserId, AccountGroupId)
	OUTPUT INSERTED.AccountId
	INTO @CreatedAccount
	VALUES (@pAccountName, @pPeriodDefinitionId, @pCurrencyId, @pBaseBudget, @pHeaderColor, @pAccountTypeId,
		@pSpendTypeId, @pFinancialEntityId, @pUserId, @pAccountGroupId);

	SELECT TOP 1 @newAccountId = AccountId FROM @CreatedAccount;

	IF @pAccountIncludeData IS NOT NULL AND @pAccountIncludeData <> ''
	BEGIN
		INSERT INTO @AccountIncludeTempTable (AccountIncludeId, CurrencyConverterMethodId)
		Select
			max(case when name='AccountIncludeId' then convert(int,StringValue) else '' end),
			max(case when name='CurrencyConverterMethodId' then convert(int,StringValue) else '' end)
		From parseJSON(@pAccountIncludeData)
		where ValueType = 'int'
		group by parent_ID;

		INSERT INTO dbo.AccountInclude (AccountId, AccountIncludeId, CurrencyConverterMethodId)
		SELECT @newAccountId, AccountIncludeId, CurrencyConverterMethodId
		FROM @AccountIncludeTempTable;
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