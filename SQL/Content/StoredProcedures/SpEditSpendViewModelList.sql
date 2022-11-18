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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpEditSpendViewModelList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpEditSpendViewModelList]
END
GO
--==============================================================================================================================================
--	Name:		 				[dbo].[SpEditSpendViewModelList]
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

	exec SpEditSpendViewModelList @pSpendId=5084, @pUserId=N'test', @pAccountPeriodId = 8026
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpEditSpendViewModelList]
@pSpendId INT,
@pAccountPeriodId INT,
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES	
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE 
@cAccountId INT,
@cAccountPeriodId INT,
@cSpendId INT,
@AccountIdsTemp INT,
@SpendDateTemp DATETIME;

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================

DECLARE @AccountCurrencyTemp TABLE(
	AccountId INT,
	CurrencyId INT,
	CurrencyConverterMethodId INT
);

DECLARE @AccountIncludeMethodTemp TABLE(
	AccountId INT,
	AccountIncludeId INT,
	CurrencyConverterMethodId INT,
	IsDefault BIT
);


DECLARE @SupportedCurrencyTemp TABLE(
	CurrencyId INT,
	CurrencyName VARCHAR(100),
	CurrencySymbol VARCHAR(10)
);

DECLARE @MethodDataTemp TABLE(
	CurrencyConverterMethodId INT,
	CurrencyConverterMethodName VARCHAR(100),
	IsDefault BIT
);

DECLARE @SupportedAccountTemp TABLE(
	AccountId INT,
	AccountName VARCHAR(100)
);

DECLARE @AccountIncludeTemp TABLE(
	AccountId INT,
	AccountIncludeId INT,
	CurrencyConverterMethodId INT
);

DECLARE @AccountDataTemp TABLE(
	AccountId INT,
	AccountName VARCHAR(100),
	AccountPeriodId INT,
	CurrencyId INT,
	InitialDate DATETIME,
	EndDate DATETIME
);

DECLARE @SpendTypeData TABLE(
	SpendTypeId INT,
	SpendTypeName VARCHAR(500)
);

DECLARE @SpendTypeDefault TABLE(
	AccountId INT,
	SpendTypeId INT
);

DECLARE @SpendDataTemp TABLE (
	AccountId INT,
	SpendId INT,
	AmountCurrencyId INT,
	SpendDate DATETIME,
	SpendTypeId INT,
	OriginalAmount FLOAT,
	Numerator INT,
	Denominator INT,
	SpendDescription NVARCHAR(500),
	CurrencyConverterMethodId INT,
	SetPaymentDate DATETIME,
	IsPending BIT
);

DECLARE @DateRangeTemp Table(
	AccountId INT,
	MinDate DATETIME,
	MaxDate DATETIME,
	IsValid BIT,
	IsDateValid BIT,
	ActualDate DATETIME
);

DECLARE @ConvertedAmountInfoTemp TABLE(
	AccountId INT,
	AccountPeriodId INT,
	AccountIncludeId INT,
	ConvertedAmount FLOAT,
	IsSelected BIT,
	CurrencyId INT,
	CurrencyName VARCHAR(100),
	CurrencySymbol VARCHAR(10)
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--==============================================================================================================================================

DECLARE @AccountPeriodsTemp TABLE(
AccountPeriodId INT,
AccountId INT,
SpendId INT
);

DECLARE @cDateRangeTemp Table(
	MinDate DATETIME,
	MaxDate DATETIME,
	IsValid BIT,
	IsDateValid BIT,
	ActualDate DATETIME
);

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
		
	INSERT INTO @AccountPeriodsTemp (AccountPeriodId, SpendId)
	SELECT sop.AccountPeriodId, sop.SpendId FROM
	dbo.SpendOnPeriod sop
	WHERE sop.SpendId = @pSpendId AND sop.IsOriginal = 1

	IF NOT EXISTS (SELECT * FROM @AccountPeriodsTemp) 
	BEGIN
		raiserror ('Not able to get edit data', 20, -1);
	END

	UPDATE @AccountPeriodsTemp SET AccountId = accp.AccountId
	FROM 
	dbo.AccountPeriod accp
	JOIN @AccountPeriodsTemp apt ON apt.AccountPeriodId = accp.AccountPeriodId;

	-- Monedas que soporta con su metodo de conversion.
	INSERT INTO @AccountCurrencyTemp (AccountId, CurrencyId, CurrencyConverterMethodId)
	SELECT acc.AccountId, crrc.CurrencyIdOne, crrcm.CurrencyConverterMethodId
	FROM dbo.Account acc
	JOIN dbo.CurrencyConverter crrc ON crrc.CurrencyIdTwo = acc.CurrencyId
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterId = crrc.CurrencyConverterId
	WHERE acc.AccountId IN (SELECT AccountId FROM @AccountPeriodsTemp);

	-- Cuentas en las que se puede incluir basados en los metodos de conversion
	INSERT INTO @AccountIncludeMethodTemp (AccountId, AccountIncludeId, CurrencyConverterMethodId)
	SELECT acc1.AccountId, acc2.AccountId, crrcm.CurrencyConverterMethodId
	FROM dbo.Account acc1, dbo.Account acc2, dbo.Spend sp, dbo.CurrencyConverter crrc
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterId = crrc.CurrencyConverterId
	WHERE sp.SpendId = @pSpendId AND acc1.AccountId IN (SELECT AccountId FROM @AccountCurrencyTemp) AND acc2.UserId = @pUserId
	AND crrc.CurrencyIdOne = sp.AmountCurrencyId AND crrc.CurrencyIdTwo = acc2.CurrencyId
	AND acc1.AccountId <> acc2.AccountId
	ORDER BY acc1.AccountId;
	
	-- Data de account include que peretenecen al usuario actual
	INSERT INTO @AccountIncludeTemp (AccountId, AccountIncludeId,CurrencyConverterMethodId)
	SELECT acc1.AccountId AccountId, acc2.AccountId AccountIncludeId, sop2.CurrencyConverterMethodId
	FROM dbo.SpendOnPeriod sop
	JOIN @AccountPeriodsTemp tmp1 ON tmp1.AccountPeriodId = sop.AccountPeriodId AND tmp1.SpendId = sop.SpendId
	JOIN dbo.AccountPeriod accp1 ON accp1.AccountPeriodId = tmp1.AccountPeriodId
	JOIN dbo.Account acc1 ON acc1.AccountId = accp1.AccountId
	JOIN dbo.SpendOnPeriod sop2 ON sop2.SpendId = sop.SpendId AND sop2.AccountPeriodId <> sop.AccountPeriodId
	JOIN dbo.AccountPeriod accp2 ON accp2.AccountPeriodId = sop2.AccountPeriodId
	JOIN dbo.Account acc2 ON acc2.AccountId = accp2.AccountId
	
	--INSERT INTO @AccountIncludeTemp (AccountId, AccountIncludeId,CurrencyConverterMethodId)
	--SELECT acc.AccountId, acci.AccountIncludeId, acci.CurrencyConverterMethodId
	--FROM dbo.Account acc
	--JOIN dbo.AccountInclude acci ON acci.AccountId = acc.AccountId
	--WHERE acc.UserId = @pUserId;

	UPDATE @AccountIncludeMethodTemp SET IsDefault = 1
	FROM @AccountIncludeTemp accit
	JOIN @AccountIncludeMethodTemp accimt 
	ON accimt.AccountId = accit.AccountId AND accimt.AccountIncludeId = accit.AccountIncludeId 
		AND accimt.CurrencyConverterMethodId = accit.CurrencyConverterMethodId;

	INSERT INTO @SupportedAccountTemp (AccountId,AccountName)
	SELECT acc.AccountId, acc.Name
	FROM dbo.Account acc
	WHERE acc.AccountId IN (SELECT AccountIncludeId FROM @AccountIncludeMethodTemp)

	INSERT INTO @MethodDataTemp (CurrencyConverterMethodId, CurrencyConverterMethodName, IsDefault)
	SELECT crrcm.CurrencyConverterMethodId, crrcm.Name, crrcm.IsDefault
	FROM dbo.CurrencyConverterMethod crrcm
	WHERE crrcm.CurrencyConverterMethodId IN (SELECT CurrencyConverterMethodId FROM @AccountCurrencyTemp) 
	OR crrcm.CurrencyConverterMethodId IN (SELECT CurrencyConverterMethodId FROM @AccountIncludeMethodTemp) 
	OR crrcm.CurrencyConverterMethodId IN (SELECT CurrencyConverterMethodId FROM @AccountIncludeTemp) 
		
	INSERT INTO @AccountDataTemp(AccountId, AccountName, AccountPeriodId, CurrencyId,InitialDate,EndDate)
	SELECT acc.AccountId, acc.Name, accpt.AccountPeriodId, acc.CurrencyId, accp.InitialDate, accp.EndDate 
	FROM @AccountPeriodsTemp accpt
	JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = accpt.AccountPeriodId
	JOIN dbo.Account acc ON acc.AccountId = accp.AccountId;

	INSERT INTO @SupportedCurrencyTemp (CurrencyId, CurrencyName, CurrencySymbol)
	SELECT crr.CurrencyId, crr.Name, crr.Symbol 
	FROM dbo.Currency crr
	WHERE crr.CurrencyId IN (SELECT CurrencyId FROM @AccountDataTemp) OR
		  crr.CurrencyId IN (SELECT CurrencyId FROM @AccountCurrencyTemp);

	INSERT INTO @SpendDataTemp 
		(AccountId, SpendId, SpendTypeId, AmountCurrencyId, SpendDate, SpendDescription, OriginalAmount,
		 Numerator, Denominator, CurrencyConverterMethodId, SetPaymentDate, IsPending)
	SELECT 
		tmpAccpt.AccountId, sp.SpendId, sp.SpendTypeId, sp.AmountCurrencyId, sp.SpendDate, sp.Description, 
		sp.OriginalAmount, sp.Numerator, sp.Denominator, sop.CurrencyConverterMethodId, sp.SetPaymentDate, sp.IsPending	
	FROM dbo.Spend sp
	JOIN @AccountPeriodsTemp tmpAccpt ON tmpAccpt.SpendId = sp.SpendId
	JOIN dbo.SpendOnPeriod sop ON sop.AccountPeriodId = tmpAccpt.AccountPeriodId AND sop.SpendId = tmpAccpt.SpendId
	WHERE sp.SpendId IN (SELECT SpendId FROM @AccountPeriodsTemp);

	DECLARE AccountsCursor CURSOR FOR SELECT SpendId, AccountPeriodId, AccountId FROM @AccountPeriodsTemp;
	OPEN AccountsCursor;
	FETCH NEXT FROM AccountsCursor INTO @cSpendId, @cAccountPeriodId, @cAccountId;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @AccountIdsTemp = STUFF((SELECT ',' + cast(accp.AccountId as varchar)
						 FROM dbo.SpendOnPeriod sop
						 JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
						 WHERE sop.AccountPeriodId = @cAccountPeriodId AND sop.SpendId = @cSpendId
                         FOR XML PATH('')), 
                         1, 1, '');
		SELECT TOP 1 @SpendDateTemp = SpendDate FROM Spend WHERE SpendId = @cSpendId

		DELETE FROM @cDateRangeTemp;
		INSERT INTO @cDateRangeTemp
		EXEC dbo.SpDateRangeByAccounts @pDate = @SpendDateTemp, @pAccountIds = @AccountIdsTemp;
		
		INSERT INTO @DateRangeTemp (AccountId, MinDate, MaxDate, IsValid, IsDateValid,	ActualDate)
		SELECT @cAccountId, t.MinDate, t.MaxDate, t.IsValid, t.IsDateValid, t.ActualDate 
		FROM @cDateRangeTemp t;

		FETCH NEXT FROM AccountsCursor INTO @cSpendId, @cAccountPeriodId, @cAccountId;
	END

	INSERT INTO @SpendTypeData(SpendTypeId, SpendTypeName)
	SELECT spt.SpendTypeId, spt.Name 
	FROM dbo.SpendType spt
	JOIN dbo.UserSpendType uspt ON uspt.SpendTypeId = spt.SpendTypeId
	WHERE uspt.UserId = @pUserId;

	INSERT INTO @SpendTypeDefault (AccountId, SpendTypeId)
	SELECT accpt.AccountId, sp.SpendTypeId FROM @AccountPeriodsTemp accpt 
	JOIN dbo.Spend sp ON sp.SpendId = accpt.SpendId

	INSERT INTO @ConvertedAmountInfoTemp (AccountId, AccountPeriodId, AccountIncludeId, ConvertedAmount, 
		CurrencyId, CurrencyName, CurrencySymbol)
	SELECT accptmp.AccountId, accp.AccountPeriodId, accp.AccountId,
		((((sp.OriginalAmount*sp.Numerator)/sp.Denominator)*sop.Numerator)/sop.Denominator),
		crry_acc_in.CurrencyId, crry_acc_in.Name, crry_acc_in.Symbol
	FROM dbo.Spend sp
	JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
	JOIN dbo.AmountType amt ON amt.AmountTypeId = sp.AmountTypeId
	JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
	JOIN @AccountPeriodsTemp accptmp ON accptmp.SpendId = sp.SpendId
	JOIN dbo.Account acc_in ON acc_in.AccountId = accp.AccountId
	JOIN dbo.Currency crry_acc_in ON crry_acc_in.CurrencyId = acc_in.CurrencyId
	WHERE sp.SpendId = @pSpendId AND sop.AccountPeriodId NOT IN (SELECT AccountPeriodId FROM @AccountPeriodsTemp);

	UPDATE @ConvertedAmountInfoTemp SET IsSelected = 1
	WHERE AccountPeriodId = @pAccountPeriodId

	SELECT * FROM @AccountCurrencyTemp;
	SELECT * FROM @AccountIncludeMethodTemp;
	SELECT * FROM @AccountIncludeTemp;
	SELECT * FROM @SupportedAccountTemp;
	SELECT * FROM @MethodDataTemp;
	SELECT * FROM @SupportedCurrencyTemp;
	SELECT * FROM @AccountDataTemp;
	SELECT * FROM @SpendTypeData;
	SELECT * FROM @SpendTypeDefault;
	SELECT * FROM @SpendDataTemp;
	SELECT * FROM @DateRangeTemp;
	SELECT * FROM @ConvertedAmountInfoTemp;

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