
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpSpendsDelete]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpSpendsDelete]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	EXEC SpSpendsDelete
	@pSpendId = 12,
	@pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676'
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpSpendsDelete]
@pSpendId INT,
@pUserId UNIQUEIDENTIFIER,
@pPreventLoan BIT = 1,
@pIgnoreIdList IntArray READONLY,
@pLoanRecordId INT = NULL
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @AccountAffectedTable Table(
	AccountId int,
	Affected bit
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @spendDeleteTmp TABLE(
	SpendId INT
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @transferId INT;

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	BEGIN TRAN

	IF @pPreventLoan = 1 
		AND
		(EXISTS (SELECT TOP 1 * FROM dbo.LoanRecord lr WHERE lr.SpendId = @pSpendId) 
		OR 
		EXISTS (SELECT TOP 1 * 
		FROM dbo.SpendDependencies spd 
		JOIN dbo.LoanRecord lr ON lr.SpendId = spd.DependencySpendId
		WHERE spd.SpendId = @pSpendId))
	BEGIN
		RAISERROR ('Not allowed to delete loan record spend', 20, -1);
	END

	IF EXISTS (SELECT * FROM dbo.TransferRecord tr
		WHERE tr.SpendId = @pSpendId)
	BEGIN
		SELECT TOP 1 @transferId = tr.TransferRecordId FROM dbo.TransferRecord tr
		WHERE tr.SpendId = @pSpendId

		INSERT INTO @spendDeleteTmp (SpendId)
		SELECT DISTINCT tr.SpendId FROM dbo.TransferRecord tr
		WHERE tr.TransferRecordId = @transferId;
	END
	ELSE
	BEGIN
		INSERT INTO @spendDeleteTmp (SpendId) VALUES (@pSpendId)
	END

	DECLARE @Restrictions IntArray;
	INSERT INTO @Restrictions (Value)
	SELECT spt.SpendId
	FROM @spendDeleteTmp spt

	INSERT INTO @spendDeleteTmp (SpendId)
	SELECT r.SpendId
	FROM dbo.GetSpendDependencies(@pSpendId,@Restrictions) r;

	INSERT INTO @AccountAffectedTable (AccountId, Affected)
	SELECT accp.AccountId, 1
	FROM dbo.SpendOnPeriod sop
	JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
	WHERE SpendId IN (SELECT tmp.SpendId FROM @spendDeleteTmp tmp);
	
	DELETE FROM @spendDeleteTmp WHERE SpendId IN (SELECT value FROM @pIgnoreIdList);

	DELETE dbo.LoanSpend WHERE SpendId IN (SELECT tmp.SpendId FROM @spendDeleteTmp tmp);
	DELETE dbo.SpendDependencies WHERE SpendId IN (SELECT tmp.SpendId FROM @spendDeleteTmp tmp);
	DELETE dbo.TransferRecord WHERE SpendId IN (SELECT tmp.SpendId FROM @spendDeleteTmp tmp);
	DELETE dbo.SpendOnPeriod WHERE SpendId IN (SELECT tmp.SpendId FROM @spendDeleteTmp tmp);
	DELETE dbo.Spend WHERE SpendId IN (SELECT tmp.SpendId FROM @spendDeleteTmp tmp);

	COMMIT TRAN

	SELECT * FROM @AccountAffectedTable;
END TRY
BEGIN CATCH
	
	declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();

	IF @@TRANCOUNT > 0 ROLLBACK

	--rethrows exception
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
