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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountWithPeriodList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountWithPeriodList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	EXEC SpAccountWithPeriodList
	@pUserId=N'017844b8-a92a-44b0-9faf-e4e7230959b1'

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountWithPeriodList]
@pUserId UNIQUEIDENTIFIER,
@pDate DATETIME = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE
@Username VARCHAR(500)

--==============================================================================================================================================
--	RETURN TABLES
--	
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
	AccountGroupId INT,
	AccountPeriodId INT
);


--==============================================================================================================================================
--	STORED PROCEDURE TABLES
--	
--==============================================================================================================================================


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @RtnAccountDetailTable (AccountId, AccountName, AccountPosition, AccountHeaderColor, 
	PeriodDefinitionId, CurrencyId, FinancialEntityId, AccountTypeId, SpendTypeId, AccountGroupId, AccountPeriodId)
	SELECT acc.AccountId, acc.Name, acc.Position, acc.HeaderColor, acc.PeriodDefinitionId, 
		acc.CurrencyId, acc.FinancialEntityId, acc.AccountTypeId, acc.DefaultSpendTypeId, acc.AccountGroupId, accp.AccountPeriodId FROM 
	dbo.Account acc
	JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
	WHERE acc.UserId = @pUserId AND @pDate >= accp.InitialDate AND @pDate < accp.EndDate;
	
	SELECT * FROM @RtnAccountDetailTable;
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
