
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpLoanRecordDetailList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpLoanRecordDetailList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpLoanRecordDetailList
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

		EXEC SpLoanRecordDetailList @pLoanRecordIds=@pLoanRecordIds
	END

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpLoanRecordDetailList]
@pLoanRecordIds dbo.IntArray readonly
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @LoansRtn TABLE
(
	LoanRecordId INT, --loan
	LoanRecordName VARCHAR(500),
	AccountId INT, 
	AccountName VARCHAR(500),
	PaymentSumary FLOAT,

	LoanSpendId INT, --loan spend 
	LoanSpendDate DATETIME,
	LoanSetPaymentDate DATETIME,
	LoanSpendTypeId INT,
	LoanSpendTypeName VARCHAR(500),
	LoanCurrencyId INT,
	LoanCurrencyName VARCHAR(500),
	LoanCurrencySymbol VARCHAR(10),
	LoanNumerator FLOAT,
	LoanDenominator FLOAT,
	LoanOriginalAmount FLOAT,
	LoanDescription VARCHAR(500),
	LoanAmountTypeId INT,
	LoanIsPending BIT,

	SpendId INT, --spends
	SpendDate DATETIME,
	SetPaymentDate DATETIME,
	SpendTypeId INT,
	SpendTypeName VARCHAR(500),
	CurrencyId INT,
	CurrencyName VARCHAR(500),
	CurrencySymbol VARCHAR(10),
	Numerator FLOAT,
	Denominator FLOAT,
	OriginalAmount FLOAT,
	Description VARCHAR(500),
	AmountTypeId INT,
	IsPending BIT
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @LoanIdsTbl TABLE
(
	LoanRecordId INT
)

--==============================================================================================================================================
--	DECLARE SP VARIABLES
--==============================================================================================================================================

DECLARE @AccountIncludes dbo.IntArray;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @LoanIdsTbl
	SELECT Value 
	FROM @pLoanRecordIds; 

	INSERT INTO @LoansRtn
	SELECT 
	lr.LoanRecordId, lr.LoanRecordName, accp1.AccountId AccountId, 
	acc1.Name AccountName, lr_summ.lr_total PaymentSumary,

	sp1.SpendId, sp1.SpendDate, sp1.SetPaymentDate,
	sp1.SpendTypeId, spt1.Name, c1.CurrencyId, c1.Name, c1.Symbol,
	sop1.Numerator, sop1.Denominator, 
	(sp1.OriginalAmount*sp1.Numerator)/sp1.Denominator, 
	sp1.Description,
	sp1.AmountTypeId, sp1.IsPending,

	sp.SpendId, sp.SpendDate, sp.SetPaymentDate,
	sp.SpendTypeId, spt.Name, c.CurrencyId, c.Name, c.Symbol,
	sop.Numerator, sop.Denominator, 
	(sp.OriginalAmount*sp.Numerator)/sp.Denominator, 
	sp.Description,
	sp.AmountTypeId, sp.IsPending

	FROM dbo.LoanRecord lr
	JOIN dbo.Spend sp1 ON sp1.SpendId = lr.SpendId
	JOIN dbo.SpendType spt1 ON spt1.SpendTypeId = sp1.SpendTypeId
	JOIN dbo.SpendOnPeriod sop1 ON sop1.SpendId = sp1.SpendId
	JOIN dbo.AccountPeriod accp1 ON accp1.AccountPeriodId = sop1.AccountPeriodId
	JOIN dbo.Account acc1 ON acc1.AccountId = accp1.AccountId
	JOIN dbo.Currency c1 ON c1.CurrencyId = acc1.CurrencyId
	LEFT JOIN dbo.LoanSpend lp ON lp.LoanRecordId = lr.LoanRecordId
	LEFT JOIN dbo.Spend sp ON sp.SpendId = lp.SpendId
	LEFT JOIN dbo.SpendType spt ON spt.SpendTypeId = sp.SpendTypeId
	LEFT JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
	LEFT JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
	LEFT JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
	LEFT JOIN dbo.Currency c ON c.CurrencyId = acc.CurrencyId
	CROSS APPLY(
		SELECT 
		SUM(((((s_sp.OriginalAmount*s_sp.Numerator)/s_sp.Denominator)*s_sop.Numerator)/s_sop.Denominator)) AS lr_total
		FROM dbo.LoanSpend s_ls
		JOIN dbo.Spend s_sp ON s_sp.SpendId = s_ls.SpendId
		JOIN dbo.SpendOnPeriod s_sop ON s_sop.SpendId = s_sp.SpendId
		WHERE s_ls.LoanRecordId = lr.LoanRecordId
		AND s_sop.IsOriginal = 1
	) AS lr_summ
	WHERE lr.LoanRecordId IN (SELECT Value FROM @pLoanRecordIds)
	AND (sop.IsOriginal = 1 OR sop.IsOriginal IS NULL)
	AND (sop1.IsOriginal = 1)

	SELECT * 
	FROM @LoansRtn;

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
