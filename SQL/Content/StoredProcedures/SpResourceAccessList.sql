
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpResourceAccessList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpResourceAccessList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	EXEC SpResourceAccessList @pApplicationModuleId = 5
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpResourceAccessList]
@pApplicationResourceId INT = NULL,
@pApplicationModuleId INT = NULL,
@pResourceActionId INT = NULL,
@pResourceAccessLevelId INT = NULL
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AttributesReturn TABLE(
	ApplicationResourceId INT,
	ApplicationResourceName VARCHAR(100),
	ApplicationModuleId INT,
	ApplicationModuleName VARCHAR(100),
	ResourceActionId INT,
	ResourceActionName VARCHAR(100),
	ResourceAccessLevelId INT,
	ResourceAccessLevelName VARCHAR(100)
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE VARIABLES
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @AttributesReturn
	SELECT 
		ar.ApplicationResourceId, ar.ApplicationResourceName, 
		appm.ApplicationModuleId, appm.ApplicationModuleName, 
		ra.ResourceActionId, ra.ResourceActionName, 
		ral.ResourceAccessLevelId, ral.ResourceAccessLevelName 
	FROM dbo.ResourceRequiredAccess rra
	JOIN dbo.ResourceAction ra ON ra.ResourceActionId = rra.ResourceActionId
	JOIN dbo.ApplicationResource ar ON ar.ApplicationResourceId = rra.ApplicationResourceId
	JOIN dbo.ResourceAccessLevel ral ON ral.ResourceAccessLevelId = rra.ResourceAccessLevelId
	JOIN dbo.ApplicationModule appm ON appm.ApplicationModuleId = rra.ApplicationModuleId
	WHERE 
	(@pApplicationResourceId IS NULL OR @pApplicationResourceId = ar.ApplicationResourceId)
	AND (@pApplicationModuleId IS NULL OR @pApplicationModuleId = appm.ApplicationModuleId)
	AND (@pResourceAccessLevelId IS NULL OR @pResourceAccessLevelId = ral.ResourceAccessLevelId)
	AND (@pResourceActionId IS NULL OR @pResourceActionId = ra.ResourceActionId)

	SELECT * FROM @AttributesReturn;

END TRY
BEGIN CATCH
	
	declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

	IF @@TRANCOUNT > 0 ROLLBACK

	--rethrows exception
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
