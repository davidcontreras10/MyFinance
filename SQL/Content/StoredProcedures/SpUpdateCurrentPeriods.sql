
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpUpdateCurrentPeriods]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpUpdateCurrentPeriods]
END
GO
--==============================================================================================================================================
--	Name:		 				
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
	
	EXEC SpUpdateCurrentPeriods
	@pUserId = 'test'
	,@pDate = '2015-10-30'
	,@pDate = '2015-09-1' 
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpUpdateCurrentPeriods]
@pUserId UNIQUEIDENTIFIER,
@pDate DATETIME = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================
DECLARE
@periodDate DATETIME,
@TmpAccountId INT,
@TmpPeriodTypeId INT,
@TmpPeriodDefinitionId INT,
@TmpBaseBudget FLOAT,
@TmpCuttingDate NVARCHAR (500),
@TmpRepetition INT,
@initialDate DATETIME,
@endDate DATETIME;

DECLARE @AccountsCursor CURSOR;

DECLARE @AccountItems TABLE
(
	AccountId INT,
	UserId UNIQUEIDENTIFIER,
	AccountName NVARCHAR (500),
	CurrentPeriodId INT
);

DECLARE @InvalidAccountItems TABLE
(
	AccountId INT,
	PeriodTypeId INT,
	PeriodDefinitionId INT,
	BaseBudget FLOAT,
	CuttingDate NVARCHAR (500),
	Repetition INT
);



--==============================================================================================================================================
--	DECLARE VARIABLE CONSTANTS
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================




--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	IF @pDate IS NULL
	BEGIN
		SET @periodDate = GETDATE();
	END
	ELSE
	BEGIN
		SET @periodDate = @pDate;
	END

	INSERT INTO @AccountItems (AccountId, UserId, AccountName, CurrentPeriodId)
	SELECT acc.AccountId, acc.UserId, acc.Name, ap.AccountPeriodId
	FROM [dbo].[Account] acc
	JOIN dbo.AccountPeriod ap ON ap.AccountId=acc.AccountId
	WHERE 
	acc.UserId=@pUserId AND
	@periodDate >= ap.InitialDate AND @periodDate < ap.EndDate;


	IF EXISTS (SELECT * FROM @AccountItems)
	BEGIN
		INSERT INTO @AccountItems (AccountId, UserId, AccountName, CurrentPeriodId)
		SELECT acc.AccountId, acc.UserId, acc.Name, NULL FROM dbo.Account acc
		LEFT JOIN @AccountItems aci ON aci.AccountId = acc.AccountId
		WHERE
		aci.AccountId IS NULL AND
		acc.UserId=@pUserId;
	END 
	ELSE
	BEGIN
		INSERT INTO @AccountItems (AccountId, UserId, AccountName, CurrentPeriodId)
		SELECT acc.AccountId, acc.UserId, acc.Name, NULL FROM dbo.Account acc
		WHERE
		acc.UserId=@pUserId;
	END

	INSERT INTO @InvalidAccountItems (AccountId, PeriodTypeId, PeriodDefinitionId, BaseBudget, CuttingDate, Repetition)
	SELECT acc.AccountId, pt.PeriodTypeId, pd.PeriodDefinitionId, acc.BaseBudget, pd.CuttingDate, pd.Repetition 
	FROM @AccountItems tmp_acc
	JOIN dbo.Account acc ON acc.AccountId = tmp_acc.AccountId
	JOIN dbo.PeriodDefinition pd ON pd.PeriodDefinitionId = acc.PeriodDefinitionId
	JOIN dbo.PeriodType pt ON pt.PeriodTypeId=pd.PeriodTypeId
	WHERE tmp_acc.CurrentPeriodId IS NULL;

	--SELECT * FROM @InvalidAccountItems;
	SET @AccountsCursor = CURSOR FOR
	SELECT * FROM @InvalidAccountItems;

	OPEN @AccountsCursor;
	FETCH NEXT FROM @AccountsCursor INTO
	@TmpAccountId,
	@TmpPeriodTypeId,
	@TmpPeriodDefinitionId,
	@TmpBaseBudget,
	@TmpCuttingDate,
	@TmpRepetition;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @initialDate = NULL;
		SET @endDate = NULL;
		IF @TmpPeriodTypeId = 1 
		BEGIN
			SELECT TOP 1 @initialDate=res.InitialDate, @endDate = res.EndDate
			FROM dbo.CreateWeekPeriod(@periodDate, NULL, @TmpCuttingDate, @TmpRepetition) res;
		END
		IF @TmpPeriodTypeId = 2 
		BEGIN
			SELECT TOP 1 @initialDate=res.InitialDate, @endDate = res.EndDate
			FROM dbo.CreateMonthPeriod(@periodDate, NULL, @TmpCuttingDate, @TmpRepetition) res;
		END
		IF @TmpPeriodTypeId = 3 
		BEGIN
			SELECT TOP 1 @initialDate=res.InitialDate, @endDate = res.EndDate
			FROM dbo.CreateFortnightPeriod(@periodDate, NULL, @TmpCuttingDate, @TmpRepetition) res;
		END
		IF @TmpPeriodTypeId = 5
		BEGIN
			SELECT TOP 1 @initialDate=res.InitialDate, @endDate = res.EndDate
			FROM dbo.CreateYearPeriod(@periodDate, NULL, @TmpCuttingDate, @TmpRepetition) res;
		END
		IF @initialDate IS NOT NULL AND @endDate IS NOT NULL
		BEGIN 
			INSERT INTO dbo.AccountPeriod (AccountId,		Budget,			InitialDate,	EndDate)
			VALUES						  (@TmpAccountId,	@TmpBaseBudget,	@initialDate,	@endDate);
		END
		FETCH NEXT FROM @AccountsCursor INTO
		@TmpAccountId,
		@TmpPeriodTypeId,
		@TmpPeriodDefinitionId,
		@TmpBaseBudget,
		@TmpCuttingDate,
		@TmpRepetition;
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
