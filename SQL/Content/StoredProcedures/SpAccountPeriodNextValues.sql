--ADAPTED
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountPeriodNextValues]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountPeriodNextValues]
END
GO

/****** Object:  StoredProcedure [dbo].[SpAccountPeriodNextValues]    Script Date: 9/25/2015 9:36:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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

	exec [dbo].[SpAccountPeriodNextValues] @pUserId='test', @pAccountId = 1
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountPeriodNextValues]
@pUserId UNIQUEIDENTIFIER,
@pAccountId INT
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


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--	REMOVE THIS AFTER STORED PROCEDURE CREATION
--  If the stored procedure returns a table, the final Table name must be @tblResultSet, if it returns other thing 
--  like a Proficy Result Set, include it as a comment, starting with the word @tblResultSet :
--==============================================================================================================================================

DECLARE @AccountItems TABLE (
	AccountId INT,
	EndDate DATETIME,
	Budget FLOAT,
	HasPeriods BIT,
	IsValid BIT
);
	
--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF EXISTS (
	SELECT * FROM dbo.Account acc
	JOIN dbo.PeriodDefinition pd ON pd.PeriodDefinitionId = acc.PeriodDefinitionId
	WHERE 
	pd.PeriodTypeId = 4
	) 
	BEGIN
		IF EXISTS (SELECT * FROM AccountPeriod accp WHERE accp.AccountId = @pAccountId)
		BEGIN
			INSERT INTO @AccountItems
			(AccountId, EndDate, Budget, HasPeriods, IsValid)
			SELECT TOP 1 acc.AccountId, acc_p.EndDate, acc.BaseBudget,1,1 FROM
			dbo.AccountPeriod acc_p
			JOIN dbo.Account acc ON acc.AccountId = acc_p.AccountId
			WHERE	
			acc_p.AccountId = @pAccountId
			ORDER BY
			acc_p.EndDate DESC
		END
		ELSE
		BEGIN
			INSERT INTO @AccountItems
			(AccountId, EndDate, Budget, HasPeriods, IsValid)
			SELECT TOP 1 acc.AccountId, NULL, acc.BaseBudget, 0, 1
			FROM dbo.Account acc WHERE acc.AccountId = @pAccountId;
		END
	END
	ELSE
	BEGIN
			INSERT INTO @AccountItems
			(AccountId,		EndDate, Budget, HasPeriods,  IsValid) VALUES
			(@pAccountId,	NULL,	 NULL,	 0,			  0);	
	END
	SELECT * FROM @AccountItems;
	

END TRY
BEGIN CATCH
	DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
	;
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
