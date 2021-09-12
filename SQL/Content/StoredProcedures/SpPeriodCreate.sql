
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpPeriodCreate]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpPeriodCreate]
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

	exec SpPeriodCreate @pUserId=N'test',@pStartDate='2015-08-13 14:12:30', @pEndDate='2015-09-13 14:12:30', @pAccountId=1, @pBudget=5000
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpPeriodCreate]
@pUserId UNIQUEIDENTIFIER,
@pStartDate DATETIME,
@pEndDate DATETIME,
@pBudget FLOAT,
@pAccountId INT
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES	
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

Declare
@newSpendId INT;

--==============================================================================================================================================
--	DECLARE CONSTANTS
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--	The following Table Variables will be used to store temporary datasets within this Stored Procedure.
--==============================================================================================================================================

DECLARE @PeriodNextValuesItems TABLE(
	AccountId INT,
	EndDate DATETIME,
	Budget FLOAT
);
	
--==============================================================================================================================================
--	DECLARE VARIABLES 
--	Use this section to declare variables and set valuse for any variable constants.
--==============================================================================================================================================

DECLARE @lastDate DATETIME	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	INSERT INTO @PeriodNextValuesItems
	EXEC SpAccountPeriodNextValues @pUserId = @pUserId, @pAccountId=@pAccountId
	
	IF EXISTS (SELECT * FROM @PeriodNextValuesItems) 
	BEGIN
		SET @lastDate = (SELECT TOP 1 t.EndDate FROM @PeriodNextValuesItems t); 
	END

	SET @pStartDate= DATEFROMPARTS(DATEPART(YEAR,@pStartDate),DATEPART(MONTH,@pStartDate),DATEPART(DAY,@pStartDate));
	SET @pEndDate= DATEFROMPARTS(DATEPART(YEAR,@pEndDate),DATEPART(MONTH,@pEndDate),DATEPART(DAY,@pStartDate));
	SET @pEndDate= DATEADD(DAY,1,@pEndDate);

	IF @lastDate IS NOT NULL
	BEGIN
		SET @pStartDate = @lastDate;
	END
	
	IF @pBudget > 0 AND @pEndDate > @pStartDate 
		AND NOT EXISTS (
		SELECT * FROM dbo.AccountPeriod tmp_per 
		JOIN dbo.Account tmp_acc ON tmp_acc.AccountId = tmp_per.AccountId
		WHERE 
		tmp_per.EndDate > @pStartDate AND
		tmp_acc.AccountId = @pAccountId )
	BEGIN
		SET @pUserId = @pUserId;
		--INSERT INTO dbo.AccountPeriod (AccountId,	Budget,		InitialDate,	EndDate)
		--VALUES						  (@pAccountId,	@pBudget,	@pStartDate,	@pEndDate)
	END

			
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
GO
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
