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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpUserBankSummaryAccountPeriodList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpUserBankSummaryAccountPeriodList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

	exec SpUserBankSummaryAccountPeriodList @pUserId=N'017844B8-A92A-44B0-9FAF-E4E7230959B1'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpUserBankSummaryAccountPeriodList]
@pUserId UNIQUEIDENTIFIER,
@pDate DATETIME = NULL
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE
@BankAccounts TABLE
(
	AccountId INT,
	AccountPeriodId INT
);

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================

DECLARE
@CurrentPeriodTemp TABLE
(
	AccountId INT,
	AccountPeriodId INT,
	Budget FLOAT,
	InitialDate DATETIME,
	EndDate DATETIME
);

--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF @pDate IS NULL
	BEGIN
		SET @pDate = GETDATE();
	END

	INSERT INTO @CurrentPeriodTemp
	EXEC dbo.SpCurrentAccountPeriodList @pUserId = @pUserId, @pDate = @pDate;

	SELECT ubsa.AccountId AccountId, cpt.AccountPeriodId AccountPeriodId 
	FROM dbo.UserBankSummaryAccount ubsa
	JOIN @CurrentPeriodTemp cpt ON cpt.AccountId = ubsa.AccountId
	WHERE ubsa.UserId = @pUserId

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