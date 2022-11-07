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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAutoTrxList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAutoTrxList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	BEGIN
		EXEC SpAutoTrxList @pUserId=N'017844b8-a92a-44b0-9faf-e4e7230959b1'
	END
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAutoTrxList]
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	SELECT 
		spInTasks.*, 
		acc.Name AS AccountName, 
		curr.Symbol AS CurrencySymbol,
		et.ExecutionStatus,
		et.ExecutionMsg
	FROM
	(SELECT * FROM
	dbo.SpInTrxDef spIn
	JOIN dbo.AutomaticTask aut ON aut.AutomaticTaskId = spIn.SpInTrxDefId) AS spInTasks
	JOIN dbo.Currency curr ON curr.CurrencyId = spInTasks.CurrencyId
	JOIN dbo.Account acc ON acc.AccountId = spInTasks.AccountId
	LEFT JOIN dbo.ExecutedTask et ON et.AutomaticTaskId = spInTasks.AutomaticTaskId OR et.AutomaticTaskId IS NULL
	WHERE spInTasks.UserId = @pUserId AND (et.AutomaticTaskId IS NULL OR EXISTS (SELECT 1 
		FROM dbo.ExecutedTask et2 
		WHERE et2.AutomaticTaskId = et.AutomaticTaskId 
		GROUP BY et2.AutomaticTaskId
		HAVING et.ExecuteDatetime = MAX(et2.ExecuteDatetime)))

	SELECT 
		transferTasks.*, 
		acc.Name AS AccountName, 
		curr.Symbol AS CurrencySymbol,
		toAcc.AccountId ToAccountId,
		toAcc.Name AS ToAccountName,
		et.ExecutionStatus,
		et.ExecutionMsg
	FROM
	(SELECT * FROM
	dbo.TransferTrxDef trnsf
	JOIN dbo.AutomaticTask aut ON aut.AutomaticTaskId = trnsf.TransferTrxDefId) AS transferTasks
	JOIN dbo.Currency curr ON curr.CurrencyId = transferTasks.CurrencyId
	JOIN dbo.Account acc ON acc.AccountId = transferTasks.AccountId
	JOIN dbo.Account toAcc ON toAcc.AccountId = transferTasks.ToAccountId
	LEFT JOIN dbo.ExecutedTask et ON et.AutomaticTaskId = transferTasks.AutomaticTaskId OR et.AutomaticTaskId IS NULL
	WHERE transferTasks.UserId = @pUserId AND (et.AutomaticTaskId IS NULL OR EXISTS (SELECT 1 
		FROM dbo.ExecutedTask et2 
		WHERE et2.AutomaticTaskId = et.AutomaticTaskId 
		GROUP BY et2.AutomaticTaskId
		HAVING et.ExecuteDatetime = MAX(et2.ExecuteDatetime)))

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