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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpTransfersPossibleDestinationAccountList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpTransfersPossibleDestinationAccountList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpTransfersPossibleDestinationAccountList
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

	exec SpTransfersPossibleDestinationAccountList @pAccountPeriodId=1, @pUserId=N'test', @pCurrencyId = 1
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpTransfersPossibleDestinationAccountList]
@pAccountPeriodId INT,
@pCurrencyId INT,
@pUserId UNIQUEIDENTIFIER,
@pDate DATETIME = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES	
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE @strAccountId VARCHAR(100);
DECLARE @curCurrencies CURSOR;
DECLARE @accountId INT;
DECLARE @currentCurrencyId INT;

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountRtTemp TABLE(
	AccountId INT,
	AccountName VARCHAR(500)
);


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @DateRangeTemp TABLE(
	MinDate DATETIME,
	MaxDate DATETIME,
	IsValid BIT,
	IsDateValid BIT,
	ActualDate DATETIME
);

DECLARE @CurrencyTemp TABLE(
	AccountId INT,
	CurrencyId INT,
	CurrencyName VARCHAR(500),
	CurrencySymbol VARCHAR(10),
	IsDefault BIT
);

DECLARE @AccountTemp TABLE(
	AccountId INT
)

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF @pDate IS NULL
	BEGIN
		SET @pDate = GETDATE();
	END

	SELECT @accountId = AccountId FROM dbo.AccountPeriod WHERE AccountPeriodId = @pAccountPeriodId;
	
	SET @curCurrencies = CURSOR FOR SELECT CurrencyId FROM dbo.Currency WHERE CurrencyId = @pCurrencyId;
	OPEN @curCurrencies;
	FETCH NEXT FROM @curCurrencies INTO @currentCurrencyId;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @strAccountId = CAST(@accountId AS VARCHAR(100))
		DELETE FROM @DateRangeTemp;
		INSERT INTO @DateRangeTemp (MinDate,MaxDate,IsValid,IsDateValid,ActualDate)
			EXEC dbo.SpDateRangeByAccounts @pAccountIds = @strAccountId, @pDate = @pDate;
		INSERT INTO @AccountTemp (AccountId)
		SELECT acc.AccountId FROM 
		dbo.Account acc, dbo.Account accOr
		WHERE 
		accOr.AccountId = @accountId AND
		acc.AccountId NOT IN (SELECT AccountId FROM @AccountTemp) AND
		acc.UserId = @pUserId AND acc.AccountId <> @accountId AND
		(acc.CurrencyId = @currentCurrencyId OR 
		EXISTS (
		SELECT * FROM dbo.AccountInclude acci
		WHERE acci.AccountId = @accountId AND acci.AccountIncludeId = acc.AccountId AND
			acci.CurrencyConverterMethodId IS NOT NULL)
		OR
		EXISTS (
			SELECT * FROM dbo.CurrencyConverterMethod ccm
			JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
			WHERE cc.CurrencyIdOne = @currentCurrencyId AND cc.CurrencyIdTwo = acc.CurrencyId AND
			ccm.FinancialEntityId IS NOT NULL AND ccm.FinancialEntityId = accOr.FinancialEntityId
			 ));
		FETCH NEXT FROM @curCurrencies INTO @currentCurrencyId;
	END

	INSERT INTO @AccountRtTemp (AccountId, AccountName)
	SELECT acc.AccountId, acc.Name 
	FROM @AccountTemp acctmp
	JOIN dbo.Account acc ON acc.AccountId = acctmp.AccountId

	SELECT * FROM @AccountRtTemp;

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