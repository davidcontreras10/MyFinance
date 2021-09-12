
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAddSpendCurrencyValidate]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAddSpendCurrencyValidate]
END
GO
--==============================================================================================================================================
--	Name:		 				Dashboard.spLocal_DS_UI_Dashboard_DashboardsList
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
  DECLARE
	  @addValues dbo.ClientAddSpendCurrencies;
	  INSERT INTO @addValues VALUES (1,2,1);
	  EXEC dbo.SpAddSpendCurrencyValidate @pCurrencyDataTable = @addValues;

  END
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAddSpendCurrencyValidate]
@pCurrencyDataTable [dbo].[ClientAddSpendCurrencies] READONLY
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES	
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE RETURN TABLES
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================


DECLARE @AddSpendCurrencyData TABLE(
	AccountId INT,
	AccountName VARCHAR(100),
	AmountCurrencyId INT,
	ConvertCurrencyId INT,
	CurrencyIdOne INT,
	CurrencyIdTwo INT,
	IsSuccess BIT
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================


--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @AddSpendCurrencyData(AccountId, AccountName, AmountCurrencyId, ConvertCurrencyId, CurrencyIdOne, CurrencyIdTwo, IsSuccess)
	SELECT pcdt.AccountId, acc.Name, pcdt.AmountCurrencyId, acc.CurrencyId, crrc.CurrencyIdOne, crrc.CurrencyIdTwo, 0
	FROM @pCurrencyDataTable pcdt
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterMethodId = pcdt.CurrencyConverterMethodId
	JOIN dbo.CurrencyConverter crrc ON crrc.CurrencyConverterId = crrcm.CurrencyConverterId
	JOIN dbo.Account acc ON acc.AccountId = pcdt.AccountId

	UPDATE @AddSpendCurrencyData SET IsSuccess = 1 WHERE AmountCurrencyId = CurrencyIdOne AND ConvertCurrencyId = CurrencyIdTwo;

	SELECT * FROM @AddSpendCurrencyData;

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
