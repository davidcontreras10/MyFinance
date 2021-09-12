--Adapted
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpLoginAttempt]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpLoginAttempt]
END
GO
--==============================================================================================================================================
--	Name:		 				Dashboard.spLocal_DS_UI_Dashboard_DashboardsList
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
	
	EXEC SpLoginAttempt
	@pUsername = 'dcontre10', 
	@pPassword = 'dcontre10'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpLoginAttempt]
@pUsername NVARCHAR (100),
@pPassword NVARCHAR (500)
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE 
@UsersResultCount INT,
@UserPassword NVARCHAR (500),

@IsAuthenticated BIT,
@ResultCode INT,
@ResultMessage NVARCHAR (500),
@ResetPassword BIT

--==============================================================================================================================================
--	DECLARE VARIABLE CONSTANTS
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================



--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--	REMOVE THIS AFTER STORED PROCEDURE CREATION
--  If the stored procedure returns a table, the final Table name must be @tblResultSet, if it returns other thing 
--  like a Proficy Result Set, include it as a comment, starting with the word @tblResultSet :
--==============================================================================================================================================

DECLARE @LoginResult TABLE
(
	IsAuthenticated BIT,
	ResetPassword BIT,
	ResultCode INT,
	ResultMessage NVARCHAR (500)
)
	
--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================
	
	SET @ResultCode = 0;
	SET @IsAuthenticated = 0;
	SET @ResultMessage = 'No action performed';
	SET @ResetPassword = 0

--====================================================================================================================
--	LOGIC RESULT

--  CODE	MESSAGE
--	-1		Error during Login
--	0		No action performed
--	1		Login Succesfully
--	2		Password Incorrect
--	3		User Incorrect
--==============================================================================================================================================

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	SET @UsersResultCount = 
	(SELECT COUNT(*) 
	FROM [dbo].[AppUser] temp_users
	WHERE temp_users.Username = @pUsername);

	IF @UsersResultCount > 0
	BEGIN
		SET @UserPassword = 
		(SELECT TOP 1 temp_users.Password
		FROM [dbo].[AppUser] temp_users
		WHERE temp_users.Username = @pUsername)

		IF @UserPassword = @pPassword
		BEGIN
			--USER AND PASS MATCH
			SET @ResultCode = 1;
			SET @IsAuthenticated = 1;
			SET @ResultMessage = 'Login Succesfully';
		END
		ELSE
		BEGIN
			--USER AND PASS DON'T MATCH
			SET @ResultCode = 2;
			SET @IsAuthenticated = 0;
			SET @ResultMessage = 'Password Incorrect';
		END
	END
	ELSE
	BEGIN
		--USER DOES'T NOT EXIST
		SET @ResultCode = 3;
		SET @IsAuthenticated = 0;
		SET @ResultMessage = 'User Incorrect';
	END

	--RETURNS DATA BASED ON LOGIC
	INSERT INTO @LoginResult	(IsAuthenticated,  ResetPassword,	ResultCode,		ResultMessage)
	VALUES						(@IsAuthenticated, @ResetPassword,	@ResultCode,	@ResultMessage);
	SELECT * FROM @LoginResult;

	--RETURNS USER DATA IF PAASWORD MATCHED
	IF @IsAuthenticated = 1
	BEGIN
		SELECT urs.UserId, urs.Username, urs.Name 
		FROM [dbo].[AppUser] urs 
		WHERE urs.Username=@pUsername;
	END

END TRY
BEGIN CATCH



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
