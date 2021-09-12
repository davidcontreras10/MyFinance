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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpDateRangeByAccounts]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpDateRangeByAccounts]
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
	
BEGIN
	DECLARE @pAccountPeriodIds dbo.IntArray;
	INSERT INTO @pAccountPeriodIds VALUES (13785)

	EXEC SpBasicAccountPeriodList @pAccountPeriodIds=@pAccountPeriodIds
END
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpDateRangeByAccounts]
@pDate DATETIME = NULL,
@pAccountIds NVARCHAR(100)
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE
@MinDate DATETIME = NULL,
@MaxDate DATETIME = NULL,
@IsValid BIT = 0,
@IsDateValid BIT = NULL;

--==============================================================================================================================================
--	RETURN TABLES
--	
--==============================================================================================================================================

DECLARE @DateRangeTable TABLE(
MinDate DATETIME,
MaxDate DATETIME,
IsValid BIT,
IsDateValid BIT,
ActualDate DATETIME
);
	
--==============================================================================================================================================
--	STORED PROCEDURE TABLES
--	
--==============================================================================================================================================

DECLARE @AccountTempTable table( 
	AccountId int
);


DECLARE @AccountPeriodTempTable table( 
	AccountPeriodId int
);


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY


	INSERT INTO @AccountTempTable (AccountId)
	SELECT CAST(accPer.part as int) 
	FROM dbo.SDF_SplitString(@pAccountIds,',') accPer;


	IF EXISTS (SELECT * FROM @AccountTempTable)
	BEGIN		
		SELECT @MaxDate = MIN(tbl1.MaxDate) FROM 
			(SELECT accp.AccountId, accp.EndDate FROM
			@AccountTempTable acc
			JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId) tbl0
		JOIN
			(SELECT accp.AccountId, MAX(accp.EndDate) MaxDate FROM
			@AccountTempTable acc
			JOIN  dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
			GROUP BY accp.AccountId) tbl1 
		ON tbl1.AccountId = tbl0.AccountId
		WHERE tbl1.MaxDate = tbl0.EndDate

		SELECT @MinDate = MAX(tbl1.MinDate) FROM 
			(SELECT accp.AccountId, accp.InitialDate FROM
			@AccountTempTable acc
			JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId) tbl0
		JOIN
			(SELECT accp.AccountId, MIN(accp.InitialDate) MinDate FROM
			@AccountTempTable acc
			JOIN  dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
			GROUP BY accp.AccountId) tbl1 
		ON tbl1.AccountId = tbl0.AccountId
		WHERE tbl1.MinDate = tbl0.InitialDate

		SET @IsValid  = 1;

		IF @MaxDate IS NULL
		BEGIN
			SET @IsValid = 0
		END
		IF @MinDate IS NULL
		BEGIN
			SET @IsValid = 0
		END
	END

	SET @IsDateValid = NULL;
	IF @pDate IS NOT NULL
	BEGIN
		SET @IsDateValid = 1;
		IF @IsValid = 1 AND @pDate >= @MaxDate 
		BEGIN
			SET @IsDateValid = 0;
		END
		IF @IsValid = 1 AND @pDate < @MinDate 
		BEGIN
			SET @IsDateValid = 0;
		END
	END
	IF @IsDateValid = 0
	BEGIN
		SET @pDate = @MinDate; 
	END
	SET @MaxDate = DATEADD(SECOND, -1, @MaxDate);
	INSERT INTO @DateRangeTable (MinDate,	MaxDate,	IsValid, IsDateValid, ActualDate)
	VALUES						(@MinDate,	@MaxDate,	@IsValid, @IsDateValid, @pDate);

	SELECT * FROM @DateRangeTable

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
