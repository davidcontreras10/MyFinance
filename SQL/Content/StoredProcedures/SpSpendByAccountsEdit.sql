
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpSpendByAccountsEdit]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpSpendByAccountsEdit]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpSpendByAccountsEdit
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

declare @p6 dbo.AddSpendAccountsTable
insert into @p6 values(N'1',N'1',N'1',N'False',N'3')
insert into @p6 values(N'2',N'1',N'549',N'False',N'1')

exec SpSpendByAccountsEdit @pUserId=N'test',@pSpendTypeId=1,@pAmount=2000,@pSpendDate='2016-12-30 01:15:19',@pCurrencyId=1,@pAccountsTable=@p6,@pAmountTypeName=N'Spend'

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpSpendByAccountsEdit]
@pUserId UNIQUEIDENTIFIER,
@pSpendId INT,
@pAmount FLOAT,
@pAmountNumerator FLOAT = 1,
@pAmountDenominator FLOAT = 1,
@pSpendDate DATETIME,
@pSetPaymentDate DATETIME = NULL,
@pCurrencyId INT,
@pAccountsTable [dbo].[AddSpendAccountsTable] READONLY,
@pAllAccountsValid BIT = 1,
@pIsPending BIT = 0
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE VARIABLES	
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

Declare
@allAccountsValid BIT = 1
--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountAffectedTable Table(
	AccountId INT,
	Affected bit,
	SpendId INT
);


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @AccountTempTable table( 
	AccountId int
);

DECLARE @SpendTempTable table( 
	SpendId int
);

DECLARE @AccountPeriodTempTable table(
	AccountId INT, 
	AccountPeriodId INT,
	Numerator FLOAT,
	Denominator FLOAT,
	PendingUpdate BIT,
	CurrencyConverterMethodId INT,
	IsOriginal BIT
);

	
--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
			
	INSERT INTO @AccountTempTable (AccountId)
	SELECT pacct.AccountId
	FROM @pAccountsTable pacct;

	INSERT INTO @AccountAffectedTable (AccountId, Affected)
	SELECT pacct.AccountId, 0
	FROM @pAccountsTable pacct;

	IF @pAmount IS NOT NULL 
	BEGIN
		--DECLARE @v2 XML = (SELECT * FROM @AccountTempTable FOR XML AUTO)
		--SELECT * FROM @AccountTempTable;

		--validates correct account periods
		INSERT INTO @AccountPeriodTempTable (AccountPeriodId, AccountId)
		SELECT accp.AccountPeriodId, acc.AccountId FROM
		dbo.Account acc
		JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
		WHERE acc.AccountId IN (SELECT * FROM @AccountTempTable)
		AND @pSpendDate >= accp.InitialDate AND @pSpendDate < accp.EndDate AND acc.UserId = @pUserId;

		--DECLARE @v3 XML = (SELECT * FROM @AccountPeriodTempTable FOR XML AUTO)
		--SELECT * FROM @AccountPeriodTempTable;

		UPDATE @AccountAffectedTable SET Affected = 1
		FROM
		@AccountAffectedTable accft
		JOIN @AccountPeriodTempTable accpt ON accft.AccountId = accpt.AccountId;

		IF EXISTS (SELECT * FROM @AccountAffectedTable WHERE Affected = 0)
		BEGIN
			SET @allAccountsValid = 0
		END

		IF ( EXISTS (SELECT * FROM @AccountPeriodTempTable) ) AND (@pAllAccountsValid = 0 OR @allAccountsValid = 1)
		BEGIN
			
			UPDATE dbo.Spend SET SetPaymentDate = @pSetPaymentDate, SpendDate = @pSpendDate, IsPending = @pIsPending
			WHERE SpendId = @pSpendId;

			DELETE FROM dbo.SpendOnPeriod WHERE SpendId = @pSpendId;
			UPDATE @AccountPeriodTempTable SET Numerator = pacct.Numerator, Denominator = pacct.Denominator, 
			PendingUpdate = pacct.PendingUpdate, CurrencyConverterMethodId = pacct.CurrencyConverterMethodId,
			IsOriginal = pacct.IsOriginal
			FROM @pAccountsTable pacct
			JOIN @AccountPeriodTempTable accpt ON accpt.AccountId = pacct.AccountId
			
			INSERT INTO DBO.SpendOnPeriod (AccountPeriodId, SpendId, Numerator, Denominator, PendingUpdate, 
				CurrencyConverterMethodId, IsOriginal)
			SELECT valids.AccountPeriodId, @pSpendId, valids.Numerator, valids.Denominator, valids.PendingUpdate, 
				valids.CurrencyConverterMethodId, valids.IsOriginal
			FROM @AccountPeriodTempTable valids

			UPDATE @AccountAffectedTable SET SpendId = @pSpendId;
		END
		--DECLARE @v3 XML = (SELECT * FROM @InvalidAccountPeriodTempTable FOR XML AUTO
	END
	SELECT * FROM @AccountAffectedTable;
			
END TRY
BEGIN CATCH
	DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
	;
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
