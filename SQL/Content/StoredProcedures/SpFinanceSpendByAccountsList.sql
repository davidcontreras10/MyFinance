
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpFinanceSpendByAccountsList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpFinanceSpendByAccountsList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpFinanceSpendByAccountsList
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

declare @p1 dbo.AccountPeriodOptionParametersTable
insert into @p1 values(N'21258',N'True',N'True',0)
insert into @p1 values(N'21251',N'True',N'True',0)
insert into @p1 values(N'21259',N'True',N'True',0)
insert into @p1 values(N'21252',N'True',N'True',0)
insert into @p1 values(N'21253',N'True',N'True',0)
insert into @p1 values(N'21260',N'True',N'True',0)
insert into @p1 values(N'21257',N'True',N'True',0)
insert into @p1 values(N'21254',N'True',N'True',0)
insert into @p1 values(N'21255',N'True',N'True',0)
insert into @p1 values(N'21256',N'True',N'True',0)

exec SpFinanceSpendByAccountsList @pAccountPeriodTable=@p1,@pUserId=N'71722361-99ff-493f-af02-2bd0ed7ce676'

*/

--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpFinanceSpendByAccountsList]
@pAccountPeriodTable AccountPeriodOptionParametersTable Readonly,
@pUserId UNIQUEIDENTIFIER,
@pAvoidQuery BIT = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES	
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE RETURN TABLES
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================

DECLARE @AccountFinanceTemp TABLE(
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
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--==============================================================================================================================================

DECLARE @AccountPeriodTable TABLE(
	AccountPeriodId INT,
	AccountId INT,
	PendingSpends BIT,
	LoanSpends BIT
);

DECLARE @AccountDataTemp TABLE(
	AccountId INT,
	AccountPeriodId INT,
	TotalAccountBudget FLOAT,
	TotalAccountSpent FLOAT
);

DECLARE @AccountPeriodDataTemp TABLE(
	AccountPeriodId INT,
	TotalAccountPeriodBalance FLOAT
);
	
DECLARE @AccountTotalSpendTemp TABLE(
	AccountId INT,
	TotalAccountSpent FLOAT,
	TotalAccountBudget FLOAT,
	TotalAccountSpentToday FLOAT,
	TotalAccountBudgetToday FLOAT
);

DECLARE @CurrentAccountPeriodTemp TABLE
(
	AccountId INT,
	AccountPeriodId INT,
	Budget FLOAT,
	InitialDate DATETIME,
	EndDate DATETIME
);

DECLARE @UserSpendType TABLE
(
	SpendTypeName VARCHAR(100)
);

--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @AccountPeriodTable (AccountPeriodId, AccountId, LoanSpends, PendingSpends)
	SELECT pAccptt.AccountPeriodId, accp.AccountId, pAccptt.LoanSpends, pAccptt.PendingSpends 
	FROM @pAccountPeriodTable pAccptt
	JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = pAccptt.AccountPeriodId;
	
	INSERT INTO @CurrentAccountPeriodTemp
	EXEC dbo.SpCurrentPeriodList @pUserId = @pUserId;

	INSERT INTO @AccountFinanceTemp
		(AccountId, AccountPeriodId, AccountCurrencyId, AccountCurrencySymbol, InitialDate,
		EndDate, Budget, SpendId, Numerator, Denominator, AccountName, IsValid)
	SELECT acc.AccountId, ap.AccountPeriodId, acc.CurrencyId, crrAc.Symbol, ap.InitialDate,
		ap.EndDate, ap.Budget, sop.SpendId, sop.Numerator, sop.Denominator, acc.Name, 0
	FROM dbo.AccountPeriod ap
	LEFT JOIN dbo.SpendOnPeriod sop ON sop.AccountPeriodId = ap.AccountPeriodId
	--LEFT JOIN dbo.Spend sp ON sp.SpendId = sop.SpendId
	JOIN dbo.Account acc ON acc.AccountId = ap.AccountId
	JOIN dbo.Currency crr ON crr.CurrencyId = acc.CurrencyId
	JOIN dbo.Currency crrAc ON crrac.CurrencyId = acc.CurrencyId
	--LEFT JOIN dbo.SpendType spt ON spt.SpendTypeId = sp.SpendTypeId
	--LEFT JOIN dbo.Currency crrSp ON crrSp.CurrencyId = sp.AmountCurrencyId
	WHERE 
		ap.AccountPeriodId IN (SELECT aptt.AccountPeriodId FROM @AccountPeriodTable aptt);

	UPDATE accfntmp SET 
		accfntmp.SpendAmount = ((sp.OriginalAmount*sp.Numerator)/sp.Denominator),
		accfntmp.SpendDate = sp.SpendDate, 
		accfntmp.SpendTypeName = spt.Name, accfntmp.SpendCurrencyName = crrSp.Name,
		accfntmp.SpendCurrencySymbol = crrSp.Symbol, accfntmp.AmountType = sp.AmountTypeId,
		accfntmp.IsPending = sp.IsPending, accfntmp.IsLoan = dbo.IsLoanSpendId(sp.SpendId),
		accfntmp.SetPaymentDate = sp.SetPaymentDate, accfntmp.IsValid = 1
	FROM @AccountFinanceTemp accfntmp
	JOIN @AccountPeriodTable accptt ON accptt.AccountPeriodId = accfntmp.AccountPeriodId
	JOIN dbo.Spend sp ON sp.SpendId = accfntmp.SpendId
	JOIN dbo.SpendType spt ON spt.SpendTypeId = sp.SpendTypeId
	JOIN dbo.Currency crrSp ON crrSp.CurrencyId = sp.AmountCurrencyId
	WHERE dbo.IsSpendIdValidByConditions(sp.SpendId, accptt.AccountPeriodId, @pAccountPeriodTable) = 1;
	--WHERE (accptt.PendingSpends = 1 OR sp.IsPending = 0) 
	--	AND (accptt.LoanSpends = 1 OR dbo.IsLoanSpendId(sp.SpendId) = 0);

	UPDATE @AccountFinanceTemp SET SpendId = NULL
	WHERE IsValid <> 1

	INSERT INTO @AccountDataTemp (AccountId, AccountPeriodId)
	SELECT ap.AccountId, ap.AccountPeriodId
	FROM @AccountPeriodTable acptt
	JOIN dbo.AccountPeriod ap ON ap.AccountPeriodId = acptt.AccountPeriodId
	GROUP BY
	ap.AccountPeriodId, ap.AccountId, ap.Budget;

	INSERT INTO @AccountTotalSpendTemp (AccountId, TotalAccountSpent)
	SELECT acc.AccountId, SUM(((((sp.OriginalAmount*sp.Numerator)/sp.Denominator)*amt.AmountSign)*sop.Numerator)/sop.Denominator)
	FROM dbo.Account acc
	JOIN @AccountPeriodTable accpt ON accpt.AccountId = acc.AccountId
	JOIN dbo.AccountPeriod ap ON ap.AccountId=acc.AccountId
	JOIN dbo.SpendOnPeriod sop ON sop.AccountPeriodId = ap.AccountPeriodId
	JOIN dbo.Spend sp ON sp.SpendId=sop.SpendId
	JOIN dbo.AmountType amt ON amt.AmountTypeId = sp.AmountTypeId 
	WHERE 
	acc.AccountId IN (SELECT AccountId FROM @AccountFinanceTemp) AND
	ap.AccountPeriodId NOT IN (SELECT AccountPeriodId FROM @CurrentAccountPeriodTemp)
	AND dbo.IsSpendIdValidByConditions(sp.SpendId, accpt.AccountPeriodId, @pAccountPeriodTable) = 1
	--AND (accpt.PendingSpends = 1 OR sp.IsPending = 0)
	--AND (accpt.LoanSpends = 1 OR dbo.IsLoanSpendId(sp.SpendId) = 0)
	GROUP BY
	acc.AccountId;

	UPDATE @AccountTotalSpendTemp SET TotalAccountBudget = tb1.budgetSum
	FROM 
	(	SELECT acc.AccountId accountId, SUM(ap.Budget) budgetSum
		FROM dbo.Account acc 
		JOIN dbo.AccountPeriod ap ON ap.AccountId=acc.AccountId
		WHERE 
		acc.AccountId IN (SELECT AccountId FROM @AccountFinanceTemp) 
		AND ap.AccountPeriodId NOT IN (SELECT AccountPeriodId FROM @CurrentAccountPeriodTemp)
		GROUP BY
		acc.AccountId
	) tb1
	JOIN @AccountTotalSpendTemp acctpt ON acctpt.AccountId = tb1.accountId;

	UPDATE @AccountTotalSpendTemp SET TotalAccountBudgetToday = tb1.budgetSum
	FROM 
	(	SELECT acc.AccountId accountId, SUM(ap.Budget) budgetSum
		FROM dbo.Account acc 
		JOIN dbo.AccountPeriod ap ON ap.AccountId=acc.AccountId
		WHERE 
		acc.AccountId IN (SELECT AccountId FROM @AccountFinanceTemp) 
		GROUP BY
		acc.AccountId
	) tb1
	JOIN @AccountTotalSpendTemp acctpt ON acctpt.AccountId = tb1.accountId;

	UPDATE @AccountTotalSpendTemp SET TotalAccountSpentToday = tb1.todaySum
	FROM 
	(	
		SELECT acc.AccountId, 
		SUM(((((sp.OriginalAmount*sp.Numerator)/sp.Denominator)*amt.AmountSign)*sop.Numerator)/sop.Denominator) todaySum
		FROM dbo.Account acc 
		JOIN @AccountPeriodTable accpt ON accpt.AccountId = acc.AccountId
		JOIN dbo.AccountPeriod ap ON ap.AccountId=acc.AccountId
		JOIN dbo.SpendOnPeriod sop ON sop.AccountPeriodId = ap.AccountPeriodId
		JOIN dbo.Spend sp ON sp.SpendId=sop.SpendId
		JOIN dbo.AmountType amt ON amt.AmountTypeId = sp.AmountTypeId 
		WHERE 
		acc.AccountId IN (SELECT AccountId FROM @AccountFinanceTemp)
		AND dbo.IsSpendIdValidByConditions(sp.SpendId, accpt.AccountPeriodId, @pAccountPeriodTable) = 1
		--AND (accpt.PendingSpends = 1 OR sp.IsPending = 0)
		--AND (accpt.LoanSpends = 1 OR dbo.IsLoanSpendId(sp.SpendId) = 0)
		GROUP BY
		acc.AccountId
	) tb1
	JOIN @AccountTotalSpendTemp acctpt ON acctpt.AccountId = tb1.accountId;

	--UPDATE @AccountDataTemp SET TotalAccountSpent = atsp.TotalAccountSpent 
	--FROM @AccountTotalSpendTemp atsp
	--JOIN @AccountDataTemp apft ON apft.AccountId = atsp.AccountId;

	INSERT INTO @AccountPeriodDataTemp (AccountPeriodId, TotalAccountPeriodBalance)
	SELECT aft.AccountPeriodId, SUM(((((sp.OriginalAmount*sp.Numerator)/sp.Denominator)*amt.AmountSign)*sop.Numerator)/sop.Denominator) 
	FROM @AccountDataTemp aft
	JOIN @AccountPeriodTable accpt ON accpt.AccountPeriodId = aft.AccountPeriodId
	JOIN dbo.SpendOnPeriod sop ON sop.AccountPeriodId = aft.AccountPeriodId
	JOIN dbo.Spend sp ON sp.SpendId = sop.SpendId
	JOIN dbo.AmountType amt ON amt.AmountTypeId = sp.AmountTypeId
	WHERE dbo.IsSpendIdValidByConditions(sp.SpendId, accpt.AccountPeriodId, @pAccountPeriodTable) = 1
	--WHERE (accpt.PendingSpends = 1 OR sp.IsPending = 0) 
	--AND (accpt.LoanSpends = 1 OR dbo.IsLoanSpendId(sp.SpendId) = 0)
	GROUP BY aft.AccountPeriodId

	UPDATE @AccountFinanceTemp SET AccountGeneralBalance = (adt.TotalAccountBudget - adt.TotalAccountSpent), 
		AccountGeneralBalanceToday = (adt.TotalAccountBudgetToday - adt.TotalAccountSpentToday)
	FROM @AccountTotalSpendTemp adt
	JOIN @AccountFinanceTemp aft ON aft.AccountId = adt.AccountId;

	UPDATE @AccountFinanceTemp SET AccountPeriodBalance = (Budget - apdt.TotalAccountPeriodBalance), 
		AccountPeriodSpent = apdt.TotalAccountPeriodBalance
	FROM @AccountPeriodDataTemp apdt
	JOIN @AccountFinanceTemp aft ON aft.AccountPeriodId = apdt.AccountPeriodId;

	IF @pAvoidQuery = 1
	BEGIN
		DELETE FROM dbo.SpFinanceSpendByAccountsListTable;
		INSERT INTO dbo.SpFinanceSpendByAccountsListTable
		SELECT * FROM @AccountFinanceTemp;
	END
	ELSE
	BEGIN
		SELECT * FROM @AccountFinanceTemp;
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
