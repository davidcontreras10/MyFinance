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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountGroupDelete]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountGroupDelete]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpAccountGroupDelete
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
	
	EXEC SpAccountGroupDelete
	@pAccountGroupIds = '1'
	,@pUserId='71722361-99FF-493F-AF02-2BD0ED7CE676'

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountGroupDelete]
@pUserId UNIQUEIDENTIFIER,
@pAccountGroupId INT
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE
@currentPosition INT;

--==============================================================================================================================================
--	RETURN TABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	STORED PROCEDURE TABLES
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF EXISTS (SELECT * FROM dbo.Account WHERE AccountGroupId = @pAccountGroupId)
	BEGIN 
		RAISERROR ('Accounts still associated to this group', 20, -1);
	END
	ELSE
	BEGIN
		SELECT @currentPosition = AccountGroupPosition FROM dbo.AccountGroup 
			WHERE AccountGroupId = @pAccountGroupId;
		DELETE FROM dbo.AccountGroup WHERE AccountGroupId = @pAccountGroupId;
		IF @currentPosition IS NOT NULL
		BEGIN
			EXEC SpAccountGroupPositionFix @pUserId = @pUserId;
		END
	END

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
