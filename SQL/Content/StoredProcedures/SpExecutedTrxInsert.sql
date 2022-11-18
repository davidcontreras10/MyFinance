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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpExecutedTrxInsert]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpExecutedTrxInsert]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

	exec SpExecutedTrxInsert @pUserId=N'017844b8-a92a-44b0-9faf-e4e7230959b1',@pAmount=55,@pSpendTypeId=3,@pCurrencyId=1,@pDescription=N'des_transf',@pAccountId=5039,@pToAccount=9042,@pPeriodTypeId=2,@pDays=N'0'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpExecutedTrxInsert]
@pUserId UNIQUEIDENTIFIER,
@pAutomaticTaskId UNIQUEIDENTIFIER,
@pExecutedDatetime DATETIME,
@pExecutedStatus INT,
@pExecutionMsg VARCHAR(500) = null
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

DECLARE 
@trxId uniqueidentifier = NEWID();

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	INSERT INTO dbo.ExecutedTask (AutomaticTaskId, ExecuteDatetime, ExecutedByUserId, ExecutionMsg, ExecutionStatus)
	VALUES (@pAutomaticTaskId, @pExecutedDatetime, @pUserId, @pExecutionMsg, @pExecutedStatus)

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

SET NOCOUNT OFF
RETURN
GO
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO