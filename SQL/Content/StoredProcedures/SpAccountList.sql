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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpAccountList
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
	
	EXEC SpAccountList
	@pUserId='71722361-99FF-493F-AF02-2BD0ED7CE676'

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountList]
@pUserId UNIQUEIDENTIFIER,
@pAccountGroupId INT = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE
@Username VARCHAR(500)

--==============================================================================================================================================
--	RETURN TABLES
--	
--==============================================================================================================================================

DECLARE @RtnAccountDetailTable TABLE(
	AccountId INT,
	AccountName VARCHAR(500),
	AccountPosition INT,
	AccountHeaderColor VARCHAR(500),
	PeriodDefinitionId INT,
	CurrencyId INT,
	FinancialEntityId INT,
	AccountTypeId INT,
	SpendTypeId INT DEFAULT 0,
	AccountGroupId INT
);

DECLARE @RtnAccountGroupTable TABLE(
	AccountGroupId INT,
	AccountGroupName VARCHAR(500),
	AccountGroupPosition INT,
	IsSelected BIT
);

--==============================================================================================================================================
--	STORED PROCEDURE TABLES
--	
--==============================================================================================================================================


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF (@pAccountGroupId < 0 OR NOT EXISTS (SELECT * FROM dbo.AccountGroup WHERE AccountGroupId = @pAccountGroupId)) AND
		@pAccountGroupId IS NOT NULL
	BEGIN
		SELECT TOP 1 @pAccountGroupId = AccountGroupId 
			FROM dbo.AccountGroup WHERE UserId = @pUserId
			ORDER BY AccountGroupPosition ASC
	END

	INSERT INTO @RtnAccountDetailTable (AccountId, AccountName, AccountPosition, AccountHeaderColor, 
	PeriodDefinitionId, CurrencyId, FinancialEntityId, AccountTypeId, SpendTypeId, AccountGroupId)
	SELECT acc.AccountId, acc.Name, acc.Position, acc.HeaderColor, acc.PeriodDefinitionId, 
		acc.CurrencyId, acc.FinancialEntityId, acc.AccountTypeId, acc.DefaultSpendTypeId, acc.AccountGroupId FROM 
	dbo.Account acc WHERE acc.UserId = @pUserId 
	AND (@pAccountGroupId IS NULL OR @pAccountGroupId = 0 OR acc.AccountGroupId = @pAccountGroupId);
	
	INSERT INTO @RtnAccountGroupTable (AccountGroupId, AccountGroupName, AccountGroupPosition)
	SELECT AccountGroupId, AccountGroupName, AccountGroupPosition 
	FROM dbo.AccountGroup WHERE UserId = @pUserId;

	IF @pAccountGroupId > 0
	BEGIN 
		UPDATE @RtnAccountGroupTable SET IsSelected = 1 WHERE AccountGroupId = @pAccountGroupId;
	END

	SELECT * FROM @RtnAccountDetailTable;
	SELECT * FROM @RtnAccountGroupTable ORDER BY AccountGroupPosition ASC;
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
