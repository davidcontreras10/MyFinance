
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpBasicAccountPeriodList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpBasicAccountPeriodList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpBasicAccountPeriodList
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
		DECLARE @pAccountPeriodIds dbo.IntArray;
		INSERT INTO @pAccountPeriodIds VALUES (22241)

		EXEC SpBasicAccountPeriodList @pAccountPeriodIds=@pAccountPeriodIds
	END
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpBasicAccountPeriodList]
@pAccountPeriodIds dbo.IntArray readonly
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountPeriodRtn TABLE
(
	AccountPeriodId INT,
	AccountId INT,
	AccountName VARCHAR(500),
	InitialDate DATETIME,
	EndDate DATETIME
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE 
@AccountIds IntArray,
@AccountPeriods CURSOR,
@AccountPeriodId INT,
@AccountId INT;

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @AccountIncludes dbo.IntArray;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	SET @AccountPeriods = CURSOR FOR SELECT PaccpIds.Value FROM @pAccountPeriodIds PaccpIds
	OPEN @AccountPeriods;
	FETCH NEXT FROM @AccountPeriods INTO @AccountPeriodId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT TOP 1 @AccountId = AccountId
		FROM dbo.AccountPeriod 
		WHERE AccountPeriodId = @AccountPeriodId;

		DELETE FROM @AccountIncludes;
		
		INSERT INTO @AccountIncludes
		SELECT acci.AccountId 
		FROM dbo.GetAccountIncludeHierarchy(@AccountId) acci
		
		INSERT INTO @AccountIncludes (Value) VALUES (@AccountId);

		INSERT INTO @AccountPeriodRtn 
			(AccountId, AccountPeriodId, AccountName, InitialDate, EndDate)
		SELECT TOP 1 accp.AccountId, accp.AccountPeriodId, acc.Name, dr.MinDate, dr.MaxDate
		FROM dbo.GetDateRangeByAccounts(null,@AccountIncludes) dr, dbo.AccountPeriod accp
		JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
		WHERE accp.AccountPeriodId = @AccountPeriodId

		FETCH NEXT FROM @AccountPeriods INTO @AccountPeriodId
	END

	SELECT * FROM @AccountPeriodRtn;

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