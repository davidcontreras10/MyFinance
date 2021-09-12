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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpUserAssignedAccessList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpUserAssignedAccessList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpUserAssignedAccessList
--	Type:						Stored Procedure
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
	SELECT usaa.UserId, ra.ResourceActionId, ra.ResourceActionName, 
	ar.ApplicationResourceId, ar.ApplicationResourceName, ral.ResourceAccessLevelId, ral.ResourceAccessLevelName 
	FROM dbo.UserAssignedAccess usaa
	JOIN dbo.ResourceAction ra ON ra.ResourceActionId = usaa.ResourceActionId
	JOIN dbo.ApplicationResource ar ON ar.ApplicationResourceId = usaa.ApplicationResourceId
	JOIN dbo.ResourceAccessLevel ral ON ral.ResourceAccessLevelId = usaa.ResourceAccessLevelId

	
	EXEC SpUserAssignedAccessList
	@pUsername = 'test' 
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpUserAssignedAccessList]
@pUserId UNIQUEIDENTIFIER,
@pApplicationResourceId INT = NULL,
@pResourceActionId INT = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE VARIABLE CONSTANTS
--	The following variables will be used as internal constants to this Stored Procedure.

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	RETURN TABLES
--==============================================================================================================================================

DECLARE @UserAssignedAccessTemp TABLE
(
	UserId UNIQUEIDENTIFIER,
	ResourceActionId INT,
	ApplicationResourceId INT,
	ResourceAccessLevelId INT
);


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	INSERT INTO @UserAssignedAccessTemp (UserId, ApplicationResourceId, ResourceActionId, ResourceAccessLevelId)
	SELECT usasg.UserId, usasg.ApplicationResourceId, usasg.ResourceActionId, usasg.ResourceAccessLevelId FROM
	dbo.UserAssignedAccess usasg WHERE 
	usasg.UserId = @pUserId AND (@pApplicationResourceId IS NULL OR usasg.ApplicationResourceId = @pApplicationResourceId)
	AND (@pResourceActionId IS NULL OR usasg.ResourceActionId = @pResourceActionId);

	SELECT * FROM @UserAssignedAccessTemp;

END TRY
BEGIN CATCH

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
