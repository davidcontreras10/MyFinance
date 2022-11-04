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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAutoTrxBasicInsert]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAutoTrxBasicInsert]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*

	exec SpAutoTrxBasicInsert @pUserId=N'017844B8-A92A-44B0-9FAF-E4E7230959B1'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAutoTrxBasicInsert]
@pUserId UNIQUEIDENTIFIER,
@pAmount FLOAT,
@pSpendTypeId INT,
@pCurrencyId INT,
@pTaskDescription [nvarchar](400),
@pFreqTypeId INT,
@pAccountId INT,
@pIsSpendTrx BIT,
@pPeriodTypeId INT,
@pDays NVARCHAR
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @CreatedTask TABLE(
	[AutomaticTaskId] [UNIQUEIDENTIFIER] NOT NULL,
	[Amount] [Float] NOT NULL,
	[SpendTypeId] [INT] NOT NULL,
	[CurrencyId] [INT] NOT NULL,
	[TaskDescription] [nvarchar](400),
	[AccountId] [INT] NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[IsSpendTrx] [BIT] NOT NULL,
	[PeriodTypeId] [INT] NOT NULL,
	[Days] [NVARCHAR] NOT NULL
);

--==============================================================================================================================================
--	DECLARE SP TABLES
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE VARIBLES
--==============================================================================================================================================

DECLARE 
@trxId uniqueidentifier = NEWID();

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	INSERT INTO dbo.AutomaticTask(AutomaticTaskId, Amount, SpendTypeId, CurrencyId, TaskDescription, AccountId, UserId, PeriodTypeId, Days) VALUES
								 (@trxId, @pAmount, @pSpendTypeId, @pSpendTypeId, @pCurrencyId, @pAccountId, @pUserId, @pPeriodTypeId, @pDays);
	INSERT INTO dbo.SpInTrxDef(SpInTrxDefId, IsSpendTrx)
	VALUES					  (@trxId, @pIsSpendTrx);

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