
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpLoanIdList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpLoanIdList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpLoanIdList
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
		DECLARE @pLoanRecordIds dbo.IntArray;
		INSERT INTO @pLoanRecordIds VALUES (1)

		EXEC SpLoanIdList @pLoanRecordIds=@pLoanRecordIds
	END

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpLoanIdList]
@pUserId UNIQUEIDENTIFIER,
@pLoanRecordStatusId INT,
@pAccountPeriodIds dbo.IntArray readonly,
@pAccountIds dbo.IntArray readonly,
@pCriteriaId INT
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @LoanIdsTbl TABLE
(
	LoanRecordId INT
)

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE SP VARIABLES
--==============================================================================================================================================


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF @pCriteriaId = 1
	BEGIN
		INSERT INTO @LoanIdsTbl
		SELECT lr.LoanRecordId
		FROM dbo.LoanRecord lr
		JOIN dbo.Spend sp ON sp.SpendId = lr.SpendId
		JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
		JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId  = sop.AccountPeriodId
		JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
		WHERE acc.UserId = @pUserId AND sop.IsOriginal = 1
		AND lr.LoanRecordStatusId = @pLoanRecordStatusId;
	END
	ELSE IF @pCriteriaId = 2
	BEGIN
		INSERT INTO @LoanIdsTbl
		SELECT lr.LoanRecordId
		FROM dbo.LoanRecord lr
		JOIN dbo.Spend sp ON sp.SpendId = lr.SpendId
		JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
		JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId  = sop.AccountPeriodId
		JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
		WHERE acc.AccountId IN (SELECT Value FROM @pAccountIds) 
		AND sop.IsOriginal = 1
		AND lr.LoanRecordStatusId = @pLoanRecordStatusId;
	END
	ELSE IF @pCriteriaId = 3
	BEGIN
		INSERT INTO @LoanIdsTbl
		SELECT lr.LoanRecordId
		FROM dbo.LoanRecord lr
		JOIN dbo.Spend sp ON sp.SpendId = lr.SpendId
		JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
		JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId  = sop.AccountPeriodId
		JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
		WHERE acc.AccountId IN 
			(SELECT accp.AccountId 
			FROM @pAccountPeriodIds pAccp
			JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = pAccp.Value) 
		AND sop.IsOriginal = 1
		AND lr.LoanRecordStatusId = @pLoanRecordStatusId;
	END
	ELSE
	BEGIN
		RAISERROR ('Invalid search criteria Id', 20, -1) WITH LOG;
	END

	SELECT * FROM @LoanIdsTbl;

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
