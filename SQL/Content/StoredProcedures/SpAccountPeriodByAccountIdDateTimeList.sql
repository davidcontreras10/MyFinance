
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAccountPeriodByAccountIdDateTimeList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAccountPeriodByAccountIdDateTimeList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	EXEC SpAccountPeriodByAccountIdDateTimeList
	@pSpendId = 12,
	@pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAccountPeriodByAccountIdDateTimeList]
@pDateTime DATETIME,
@pAccountId INT
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountPeriodRtn TABLE
(
	AccountPeriodId INT,
	AccountId INT,
	AccountName VARCHAR(500),
	InitialDate DATETIME,
	EndDate DATETIME,
	UserId UNIQUEIDENTIFIER
);

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

	INSERT INTO @AccountPeriodRtn(AccountId, AccountName, AccountPeriodId, InitialDate, EndDate, UserId)
	SELECT accp.AccountId, acc.Name, accp.AccountPeriodId, accp.InitialDate, accp.EndDate, acc.UserId
	FROM dbo.AccountPeriod accp
	JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
	WHERE @pDateTime >= accp.InitialDate AND @pDateTime < accp.EndDate
	AND accp.AccountId = @pAccountId;

	SELECT * FROM @AccountPeriodRtn;

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
