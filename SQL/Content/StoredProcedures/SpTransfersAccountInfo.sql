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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpTransfersAccountInfo]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpTransfersAccountInfo]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpTransfersAccountInfo
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

	exec SpTransfersAccountInfo @pAccountPeriodId=1, @pUserId=N'test'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpTransfersAccountInfo]
@pAccountPeriodId INT,
@pUserId UNIQUEIDENTIFIER,
@pDate DATETIME = NULL
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountFinanceRtTemp TABLE(
	AccountId INT,
	AccountPeriodId INT,
	AccountCurrencyId INT,
	AccountCurrencySymbol VARCHAR,
	AccountGeneralBalance FLOAT,
	AccountGeneralBalanceToday FLOAT,
	AccountPeriodBalance FLOAT,
	AccountPeriodSpent FLOAT,
	InitialDate DATETIME,
	EndDate DATETIME,
	Budget FLOAT,
	SpendId INT,
	SpendAmount FLOAT,
	Numerator FLOAT,
	Denominator FLOAT,
	SpendDate DATETIME,
	SpendTypeName VARCHAR(100),
	SpendCurrencyName VARCHAR(100),
	SpendCurrencySymbol VARCHAR(100),
	AmountType INT,
	AccountName VARCHAR(100),
	IsPending BIT,
	SetPaymentDate DATETIME,
	IsValid BIT,
	IsLoan BIT
);

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================



--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

DECLARE @accountId INT;
DECLARE @accountPeriodIds VARCHAR(100);
DECLARE @accountPeriodValues AccountPeriodOptionParametersTable;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF @pDate IS NULL
	BEGIN
		SET @pDate = GETDATE();
	END

	INSERT INTO @accountPeriodValues (AccountPeriodId,PendingSpends, LoanSpends) 
	VALUES (@pAccountPeriodId, 1, 0);

	EXEC dbo.SpFinanceSpendByAccountsList 
		@pUserId = @pUserId, @pAccountPeriodTable = @accountPeriodValues, @pAvoidQuery = 1;
	INSERT INTO @AccountFinanceRtTemp
	SELECT * FROM dbo.SpFinanceSpendByAccountsListTable;
	DELETE FROM dbo.SpFinanceSpendByAccountsListTable;

	WITH CTE AS(
	   SELECT AccountId, RN = ROW_NUMBER()OVER(PARTITION BY AccountId ORDER BY AccountId)
	   FROM @AccountFinanceRtTemp
	)
	DELETE FROM CTE WHERE RN > 1

	SELECT * FROM @AccountFinanceRtTemp;

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