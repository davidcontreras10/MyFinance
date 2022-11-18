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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpExecutedTaskList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpExecutedTaskList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	BEGIN
		EXEC SpExecutedTaskList @pAutomaticTaskId = '6F841D21-0201-486D-9F9F-F7FE06A2BF31'
	END
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpExecutedTaskList]
@pAutomaticTaskId UNIQUEIDENTIFIER
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

	SELECT TOP 5 * FROM dbo.ExecutedTask et WHERE et.AutomaticTaskId = @pAutomaticTaskId
		ORDER BY et.ExecuteDatetime DESC

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