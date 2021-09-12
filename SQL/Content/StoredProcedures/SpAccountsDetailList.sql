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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountsDetailList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountsDetailList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpAccountsDetailList
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

	exec SpAccountsDetailList @pAccountIds='1', @pUserId='71722361-99FF-493F-AF02-2BD0ED7CE676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountsDetailList]
@pAccountIds VARCHAR(50),
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
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
	BaseBudget FLOAT,
	AccountGroupId INT
);

DECLARE @RtnSpendTypeAccountTable TABLE(
	AccountId INT,
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

DECLARE @AccountIdsTmp TABLE(
	AccountId INT
);

--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	INSERT INTO @AccountIdsTmp (AccountId)
	SELECT CAST(accPer.part as int) 
	FROM dbo.SDF_SplitString(@pAccountIds,',') accPer;

	INSERT INTO @RtnAccountDetailTable (AccountId, AccountName, AccountPosition, AccountHeaderColor, 
	PeriodDefinitionId, CurrencyId, FinancialEntityId, AccountTypeId, SpendTypeId, BaseBudget, AccountGroupId)
	SELECT acc.AccountId, acc.Name, acc.Position, acc.HeaderColor, acc.PeriodDefinitionId, 	acc.CurrencyId, 
	acc.FinancialEntityId, acc.AccountTypeId, acc.DefaultSpendTypeId, acc.BaseBudget, acc.AccountGroupId FROM 
	dbo.Account acc WHERE acc.AccountId IN (SELECT AccountId FROM @AccountIdsTmp)
	
	INSERT INTO @RtnSpendTypeAccountTable (AccountId, SpendTypeId)
	SELECT acc.AccountId, spt.SpendTypeId FROM
	dbo.Account acc, dbo.SpendType spt
	JOIN dbo.UserSpendType uspt ON uspt.SpendTypeId = spt.SpendTypeId
	WHERE acc.AccountId IN (SELECT AccountId FROM @AccountIdsTmp)
	AND uspt.UserId = @pUserId
	ORDER BY acc.AccountId;

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

	INSERT INTO @RtAccountIncludeInfoTable (AccountId, AccountIncludeId, AccountIncludeName, CurrencyConverterMethodId, CurrencyConverterMethodName, FinancialEntityId, FinancialEntityName, IsDefault)
	SELECT acc.AccountId, acc2.AccountId, acc2.Name, ccm.CurrencyConverterMethodId, ccm.Name, ccm.FinancialEntityId, fnett.Name, ccm.IsDefault
	FROM dbo.Account acc, dbo.Account acc2, dbo.CurrencyConverterMethod ccm
	JOIN dbo.CurrencyConverter cc ON cc.CurrencyConverterId = ccm.CurrencyConverterId
	JOIN dbo.FinancialEntity fnett ON fnett.FinancialEntityId = ccm.FinancialEntityId
	WHERE acc.AccountId IN (SELECT AccountId FROM @AccountIdsTmp) AND acc2.UserId = @pUserId AND acc2.AccountId <> acc.AccountId 
	AND cc.CurrencyIdOne = acc.CurrencyId AND cc.CurrencyIdTwo = acc2.CurrencyId

	UPDATE @RtAccountIncludeInfoTable SET IsSelected = 1
	FROM dbo.AccountInclude acci, @RtAccountIncludeInfoTable rtn
	WHERE acci.AccountId = rtn.AccountId 
		AND acci.AccountIncludeId = rtn.AccountIncludeId 
		AND rtn.CurrencyConverterMethodId = acci.CurrencyConverterMethodId;
	
	INSERT INTO @RtnAccountGroupInfoTable (AccountGroupId, AccountGroupName)
	SELECT accg.AccountGroupId, accg.AccountGroupName 
	FROM dbo.AccountGroup accg WHERE accg.UserId = @pUserId;

	SELECT * FROM @RtnAccountDetailTable;
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