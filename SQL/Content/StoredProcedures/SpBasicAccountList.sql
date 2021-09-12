	
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpBasicAccountList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpBasicAccountList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpBasicAccountList
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
	
	BEGIN
		DECLARE @pAccountIds dbo.IntArray;
		INSERT INTO @pAccountIds VALUES (22241)

		EXEC SpBasicAccountList @pAccountIds=@pAccountIds
	END
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpBasicAccountList]
@pAccountIds dbo.IntArray readonly
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountRtn TABLE
(
	AccountId INT,
	AccountName VARCHAR(500),
	MinDate DATETIME,
	MaxDate DATETIME
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE 
@Accounts CURSOR,
@AccountId INT;

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @AccountIncludes dbo.IntArray;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	SET @Accounts = CURSOR FOR SELECT PaccIds.Value FROM @pAccountIds PaccIds
	OPEN @Accounts;
	FETCH NEXT FROM @Accounts INTO @AccountId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DELETE FROM @AccountIncludes;
		
		INSERT INTO @AccountIncludes
		SELECT acci.AccountId 
		FROM dbo.GetAccountIncludeHierarchy(@AccountId) acci

		INSERT INTO @AccountIncludes (Value) VALUES (@AccountId);

		INSERT INTO @AccountRtn 
			(AccountId, AccountName, MinDate, MaxDate)
		SELECT TOP 1 acc.AccountId, acc.Name, dr.MinDate, dr.MaxDate
		FROM dbo.GetDateRangeByAccounts(null,@AccountIncludes) dr, dbo.Account acc
		WHERE acc.AccountId = @AccountId

		FETCH NEXT FROM @Accounts INTO @AccountId
	END

	SELECT * FROM @AccountRtn;

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