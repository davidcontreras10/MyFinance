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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpUsersEdit]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpUsersEdit]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpUsersEdit
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
	
	EXEC SpUsersEdit
	@pUsername = 'test' 
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpUsersEdit]
@opModified BIT OUTPUT,
@pUsername NVARCHAR (100) = NULL,
@pName NVARCHAR(500) = NULL,
@pPrimaryEmail NVARCHAR(500) = NULL,
@pUserId UNIQUEIDENTIFIER
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

DECLARE @UserItems TABLE
(
	UserId UNIQUEIDENTIFIER,
	Username nvarchar(100),
	Name nvarchar(500),
	Password nvarchar(500),
	PrimaryEmail VARCHAR(500)
)

--DECLARE @SpendTypeItems TABLE
--(
--	SpendTypeId int,
--	Name nvarchar(500),
--	Description nvarchar(500)
--)


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE
@updateCount INT = 0;
	
--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================



--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF @pName IS NOT NULL
	BEGIN
		UPDATE dbo.AppUser SET Name = @pName WHERE UserId = @pUserId;
		SET @updateCount = @updateCount + 1;
	END

	IF @pPrimaryEmail IS NOT NULL
	BEGIN
		UPDATE dbo.AppUser SET PrimaryEmail = @pPrimaryEmail WHERE UserId = @pUserId;
		SET @updateCount = @updateCount + 1;
	END

	IF @pUsername IS NOT NULL
	BEGIN
		UPDATE dbo.AppUser SET Username = @pUsername WHERE UserId = @pUserId;
		SET @updateCount = @updateCount + 1;
	END

	SET @opModified = 0;
	IF EXISTS (SELECT * FROM dbo.AppUser WHERE UserId = @pUserId) AND @updateCount > 0
	BEGIN
		SET @opModified = 1;
	END

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
