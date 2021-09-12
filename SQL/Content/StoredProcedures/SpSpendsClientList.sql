
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpSpendsClientList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpSpendsClientList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpSpendsClientList
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
	EXEC SpSpendsClientList
	@pSpendId = 19014
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpSpendsClientList]
@pSpendId INT
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE VARIABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	RETURN TABLES
--	
--==============================================================================================================================================

DECLARE @RtnSpend TABLE(
	SpendId INT,
	Amount FLOAT,
	AmountNumerator FLOAT,
	AmountDenominator FLOAT,
	SpendDate DATETIME,
	CurrencyId INT,
	AmountTypeId INT,
	AccountId INT,
	CurrencyConverterMethodId INT,
	IsOriginal BIT,
	UserId UNIQUEIDENTIFIER,
	IsPending BIT
);

--==============================================================================================================================================
--	STORED PROCEDURE TABLES
--==============================================================================================================================================

DECLARE @SpendIdsTmp TABLE(
	SpendId INT
);

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @SpendIdsTmp (SpendId) VALUES (@pSpendId); 

	INSERT INTO @SpendIdsTmp (SpendId)
	SELECT SpendId FROM TransferRecord tr1 
	WHERE tr1.SpendId <> @pSpendId AND tr1.TransferRecordId IN 
	(SELECT TOP 1 tr2.TransferRecordId FROM TransferRecord tr2 
		WHERE tr2.SpendId = @pSpendId)

	INSERT INTO @RtnSpend(SpendId, Amount, AmountNumerator, AmountDenominator, SpendDate, CurrencyId, AmountTypeId,
		AccountId, CurrencyConverterMethodId, IsOriginal, UserId, IsPending)
	SELECT sp.SpendId, sp.OriginalAmount, sp.Numerator, sp.Denominator, sp.SpendDate, sp.AmountCurrencyId, 
	sp.AmountTypeId, accp.AccountId, sop.CurrencyConverterMethodId, sop.IsOriginal, acc.UserId, sp.IsPending
	FROM dbo.Spend sp
	JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
	JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
	JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
	WHERE sp.SpendId IN (SELECT SpendId FROM @SpendIdsTmp)
	
	SELECT * FROM @RtnSpend;

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
