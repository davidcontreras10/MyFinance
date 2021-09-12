
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpLoanDelete]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpLoanDelete]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	EXEC SpLoanDelete
	@pLoanRecordId = 12,
	@pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpLoanDelete]
@pLoanRecordId INT,
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountAffectedTableReturn Table(
	AccountId INT,
	Affected BIT
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================
DECLARE @AccountAffectedTable Table(
	AccountId INT,
	Affected BIT
);

DECLARE @IgnoreIdList IntArray;

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @SpendId INT;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.LoanRecord WHERE LoanRecordId = @pLoanRecordId)
	BEGIN
		SELECT * FROM @AccountAffectedTable;
		RETURN;
	END
	ELSE
	BEGIN
		BEGIN TRAN
	
		SELECT @SpendId = lr.SpendId
		FROM dbo.LoanRecord lr
		WHERE lr.LoanRecordId = @pLoanRecordId;
			
		INSERT INTO @IgnoreIdList (Value) VALUES (@SpendId);

		INSERT INTO @AccountAffectedTable(AccountId, Affected)
		EXEC SpSpendsDelete
		@pSpendId = @SpendId,
		@pUserId = @pUserId,
		@pIgnoreIdList = @IgnoreIdList,
		@pPreventLoan = 0

		DELETE FROM @IgnoreIdList;
		DELETE FROM LoanRecord WHERE LoanRecordId = @pLoanRecordId;

		INSERT INTO @AccountAffectedTable(AccountId, Affected)
		EXEC SpSpendsDelete
		@pSpendId = @SpendId,
		@pUserId = @pUserId,
		@pIgnoreIdList = @IgnoreIdList,
		@pPreventLoan = 0

		COMMIT TRAN

		INSERT INTO @AccountAffectedTableReturn (AccountId, Affected)
		SELECT DISTINCT accafft.AccountId, accafft.Affected
		FROM @AccountAffectedTable  accafft;

		SELECT * FROM @AccountAffectedTableReturn;
	END
END TRY
BEGIN CATCH
	
	IF @@TRANCOUNT > 0 ROLLBACK

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
