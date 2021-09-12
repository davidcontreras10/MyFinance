
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAddSpendViewModelList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAddSpendViewModelList]
END
GO
--==============================================================================================================================================
--	Name:		 				[dbo].[SpAddSpendViewModelList]
--	Type:						Stored Procedure
--	Editor Tab Spacing:	4	
--==============================================================================================================================================
--	DESCRIPTION: 
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

	exec SpAddSpendViewModelList @pAccountPeriodIds=N'17225',@pUserId=N'71722361-99ff-493f-af02-2bd0ed7ce676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAddSpendViewModelList]
@pAccountPeriodIds NVARCHAR (100),
@pUserId UNIQUEIDENTIFIER
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

DECLARE @AccountCurrencyTemp TABLE(
	AccountId INT,
	CurrencyId INT,
	CurrencyConverterMethodId INT,
	IsSuggested BIT
);

DECLARE @AccountIncludeMethodTemp TABLE(
	AccountId INT,
	AccountIncludeId INT,
	CurrencyConverterMethodId INT,
	IsDefault BIT
);

DECLARE @AccountIncludeTemp TABLE(
	AccountId INT,
	AccountIncludeId INT,
	CurrencyConverterMethodId INT
);

DECLARE @SupportedAccountTemp TABLE(
	AccountId INT,
	AccountName VARCHAR(100)
);

DECLARE @MethodDataTemp TABLE(
	CurrencyConverterMethodId INT,
	CurrencyConverterMethodName VARCHAR(100),
	IsDefault BIT
);

DECLARE @SupportedCurrencyTemp TABLE(
	CurrencyId INT,
	CurrencyName VARCHAR(100),
	CurrencySymbol VARCHAR(10)
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
--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--==============================================================================================================================================

DECLARE @AccountPeriodsTemp TABLE(
AccountPeriodId INT,
AccountId INT
);



--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
			
	INSERT INTO @AccountPeriodsTemp (AccountPeriodId)
	SELECT CAST(accPer.part as int) 
	FROM dbo.SDF_SplitString(@pAccountPeriodIds,',') accPer;

	UPDATE @AccountPeriodsTemp SET AccountId = accp.AccountId
	FROM 
	dbo.AccountPeriod accp
	JOIN @AccountPeriodsTemp apt ON apt.AccountPeriodId = accp.AccountPeriodId;
		
	INSERT INTO @AccountCurrencyTemp (AccountId, CurrencyId, CurrencyConverterMethodId)
	SELECT acc.AccountId, crrc.CurrencyIdOne, crrcm.CurrencyConverterMethodId
	FROM dbo.Account acc
	JOIN dbo.CurrencyConverter crrc ON crrc.CurrencyIdTwo = acc.CurrencyId
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterId = crrc.CurrencyConverterId
	WHERE acc.AccountId IN (SELECT AccountId FROM @AccountPeriodsTemp);

	UPDATE @AccountCurrencyTemp SET IsSuggested = 1
	FROM 
	dbo.Account acc 
	JOIN @AccountCurrencyTemp accct ON accct.AccountId = acc.AccountId
	JOIN dbo.CurrencyConverterMethod ccm ON ccm.CurrencyConverterMethodId = accct.CurrencyConverterMethodId
	WHERE acc.FinancialEntityId = ccm.FinancialEntityId

	INSERT INTO @AccountIncludeMethodTemp (AccountId, AccountIncludeId, CurrencyConverterMethodId)
	SELECT acc1.AccountId, acc2.AccountId, crrcm.CurrencyConverterMethodId
	FROM dbo.Account acc1, dbo.Account acc2, dbo.CurrencyConverter crrc
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterId = crrc.CurrencyConverterId
	WHERE acc1.AccountId IN (SELECT AccountId FROM @AccountCurrencyTemp) AND acc2.UserId = @pUserId
	AND crrc.CurrencyIdOne = acc1.CurrencyId AND crrc.CurrencyIdTwo = acc2.CurrencyId
	AND acc1.AccountId <> acc2.AccountId
	ORDER BY acc1.AccountId;
	
	INSERT INTO @AccountIncludeTemp (AccountId, AccountIncludeId,CurrencyConverterMethodId)
	SELECT acc.AccountId, acci.AccountIncludeId, acci.CurrencyConverterMethodId
	FROM dbo.Account acc
	JOIN dbo.AccountInclude acci ON acci.AccountId = acc.AccountId
	WHERE acc.UserId = @pUserId;
	
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

	INSERT INTO @SpendTypeData(SpendTypeId, SpendTypeName)
	SELECT spt.SpendTypeId, spt.Name 
	FROM dbo.SpendType spt
	JOIN dbo.UserSpendType uspt ON uspt.SpendTypeId = spt.SpendTypeId
	WHERE uspt.UserId = @pUserId;

	INSERT INTO @SpendTypeDefault (AccountId, SpendTypeId)
	SELECT acc.AccountId, acc.DefaultSpendTypeId FROM dbo.Account acc
	WHERE acc.AccountId IN (SELECT AccountId FROM @AccountPeriodsTemp) 
	AND acc.DefaultSpendTypeId IS NOT NULL;

	SELECT * FROM @AccountCurrencyTemp;
	SELECT * FROM @AccountIncludeMethodTemp;
	SELECT * FROM @AccountIncludeTemp;
	SELECT * FROM @SupportedAccountTemp;
	SELECT * FROM @MethodDataTemp;
	SELECT * FROM @SupportedCurrencyTemp;
	SELECT * FROM @AccountDataTemp;
	SELECT * FROM @SpendTypeData;
	SELECT * FROM @SpendTypeDefault;

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
