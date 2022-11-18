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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountDelete]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountDelete]
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

EXEC SpAccountDelete @pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676', @pAccountId = 10000
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountDelete]
@pUserId UNIQUEIDENTIFIER,
@pAccountId INT
AS
SET NOCOUNT ON

--====================================================================================================================
--	TEMP TABLES
--====================================================================================================================

DECLARE @AutoTrxIds TABLE(
	Id UNIQUEIDENTIFIER
);

--====================================================================================================================
--	BEGIN LOGIC
--====================================================================================================================
BEGIN TRY

	INSERT INTO @AutoTrxIds
	SELECT aut.AutomaticTaskId
	FROM dbo.AutomaticTask aut
	WHERE aut.AccountId = @pAccountId;

	DELETE FROM dbo.ExecutedTask WHERE AutomaticTaskId IN (SELECT Id FROM @AutoTrxIds);
	DELETE FROM dbo.SpInTrxDef WHERE SpInTrxDefId IN (SELECT Id FROM @AutoTrxIds);
	DELETE FROM dbo.TransferTrxDef WHERE TransferTrxDefId IN (SELECT Id FROM @AutoTrxIds);
	DELETE FROM dbo.AutomaticTask WHERE AutomaticTaskId IN (SELECT Id FROM @AutoTrxIds);

	DELETE FROM dbo.UserBankSummaryAccount WHERE AccountId = @pAccountId;

	DELETE FROM dbo.SpendOnPeriod 
	WHERE AccountPeriodId IN (
	SELECT accp.AccountPeriodId FROM dbo.Account acc 
	JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
	WHERE acc.AccountId = @pAccountId);

	DELETE FROM dbo.AccountPeriod
	WHERE AccountId = @pAccountId;

	DELETE FROM dbo.AccountInclude
	WHERE AccountId = @pAccountId OR AccountIncludeId = @pAccountId;

	DELETE FROM dbo.Account 
	WHERE AccountId = @pAccountId;

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