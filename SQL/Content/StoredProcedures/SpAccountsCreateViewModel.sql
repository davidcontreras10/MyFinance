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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountsCreateViewModel]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountsCreateViewModel]
END
GO

--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

	exec SpAccountsCreateViewModel @pUserId='71722361-99FF-493F-AF02-2BD0ED7CE676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountsCreateViewModel]
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @RtnSpendTypeAccountTable TABLE(
	SpendTypeId INT
);

DECLARE @RtAccountInfoTable TABLE(
	AccountId INT,
	AccountName VARCHAR(500)
);

DECLARE @RtnSpendTypeInfoTable TABLE(
	SpendTypeId INT,
	SpendTypeName VARCHAR(500)
);

DECLARE @RtnAccountTypeInfoTable TABLE(
	AccountTypeId INT,
	AccountTypeName VARCHAR(500)
);

DECLARE @RtnPeriodTypeInfoTable TABLE(
	PeriodDefinitionId INT,
	PeriodTypeId INT,
	PeriodTypeName VARCHAR(500),
	CuttingDate VARCHAR(500),
	Repetition INT
);

DECLARE @RtnFinancialEntityInfoTable TABLE(
	FinancialEntityId INT,
	FinancialEntityName VARCHAR(500)
);

DECLARE @RtnCurrencyInfoTable TABLE(
	CurrencyId INT,
	CurrencyName VARCHAR(500)
);

DECLARE @RtAccountIncludeInfoTable TABLE(
	AccountId INT,
	AccountIncludeId INT,
	AccountIncludeName VARCHAR(500),
	CurrencyConverterMethodId INT,
	CurrencyConverterMethodName VARCHAR(500),
	FinancialEntityId INT,
	FinancialEntityName VARCHAR(500),
	IsDefault BIT,
	IsSelected BIT
);

DECLARE @RtnAccountGroupInfoTable TABLE(
	AccountGroupId INT,
	AccountGroupName VARCHAR(500)
);

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	
	INSERT INTO @RtnSpendTypeAccountTable (SpendTypeId)
	SELECT spt.SpendTypeId 
	FROM dbo.SpendType spt
	JOIN dbo.UserSpendType uspt ON uspt.SpendTypeId = spt.SpendTypeId
	WHERE uspt.UserId = @pUserId;

	INSERT INTO @RtAccountInfoTable (AccountId, AccountName)
	SELECT acc.AccountId, acc.Name FROM
	dbo.Account acc
	WHERE acc.UserId = @pUserId;

	INSERT INTO @RtnSpendTypeInfoTable
	SELECT spt.SpendTypeId, spt.Name FROM
	dbo.SpendType spt;

	INSERT INTO @RtnAccountTypeInfoTable (AccountTypeId, AccountTypeName)
	SELECT acct.AccountTypeId, AccountTypeName FROM 
	dbo.AccountType acct;

	INSERT INTO @RtnPeriodTypeInfoTable (PeriodDefinitionId, PeriodTypeId, PeriodTypeName, CuttingDate, Repetition)
	SELECT pd.PeriodDefinitionId, pt.PeriodTypeId, pt.Name, pd.CuttingDate, pd.Repetition FROM dbo.PeriodDefinition pd
		JOIN dbo.PeriodType pt ON pt.PeriodTypeId = pd.PeriodTypeId ORDER BY pt.Name;

	INSERT INTO @RtnFinancialEntityInfoTable (FinancialEntityId, FinancialEntityName)
	SELECT fne.FinancialEntityId, fne.Name FROM
	dbo.FinancialEntity fne
	WHERE fne.Name NOT LIKE '%default%';

	INSERT INTO @RtnCurrencyInfoTable(CurrencyId, CurrencyName)
	SELECT crrcy.CurrencyId, crrcy.Name FROM 
	dbo.Currency crrcy;

	INSERT INTO @RtAccountIncludeInfoTable (AccountId,AccountIncludeId, AccountIncludeName, CurrencyConverterMethodId, CurrencyConverterMethodName, FinancialEntityId, FinancialEntityName, IsDefault)
	SELECT 1, acc.AccountId, acc.Name, ccm.CurrencyConverterMethodId, ccm.Name, ccm.FinancialEntityId, fnett.Name, ccm.IsDefault
	FROM dbo.Account acc, dbo.CurrencyConverterMethod ccm
	JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
	JOIN dbo.FinancialEntity fnett ON fnett.FinancialEntityId = ccm.FinancialEntityId
	WHERE cc.CurrencyIdOne = 1 AND cc.CurrencyIdTwo = acc.CurrencyId AND acc.UserId = @pUserId;

	--UPDATE @RtAccountIncludeInfoTable SET IsSelected = 1
	--FROM dbo.AccountInclude acci, @RtAccountIncludeInfoTable rtn
	--WHERE acci.AccountId = rtn.AccountId AND acci.AccountIncludeId = rtn.AccountIncludeId;

	INSERT INTO @RtnAccountGroupInfoTable (AccountGroupId, AccountGroupName)
	SELECT accg.AccountGroupId, accg.AccountGroupName 
	FROM dbo.AccountGroup accg WHERE accg.UserId = @pUserId;

	SELECT * FROM @RtnSpendTypeAccountTable;
	SELECT * FROM @RtAccountInfoTable;
	SELECT * FROM @RtnSpendTypeInfoTable;
	SELECT * FROM @RtnAccountTypeInfoTable;
	SELECT * FROM @RtnPeriodTypeInfoTable;
	SELECT * FROM @RtnFinancialEntityInfoTable;
	SELECT * FROM @RtnCurrencyInfoTable;
	SELECT * FROM @RtAccountIncludeInfoTable;
	SELECT * FROM @RtnAccountGroupInfoTable;

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