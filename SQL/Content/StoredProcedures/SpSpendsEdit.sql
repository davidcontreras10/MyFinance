
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpSpendsEdit]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpSpendsEdit]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpSpendsEdit
--	Type:						Stored Procedure
--	Editor Tab Spacing:	4	
--==============================================================================================================================================
--	DESCRIPTION: 
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
	EXEC SpSpendsEdit
	@pSpendId = 12
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpSpendsEdit]
@pSpendId INT,
@pUserId UNIQUEIDENTIFIER,
@pModifyList VARCHAR(100) = NULL,
@pSpendDate DATETIME = NULL,
@pAmount FLOAT = NULL,
@pAccountsTable [dbo].[AddSpendAccountsTable] READONLY,
@pDescription NVARCHAR(100) = NULL,
@pSpendTypeId INT = NULL,
@pCurrencyId INT = NULL,
@pAmountTypeName VARCHAR(100) = NULL
AS
SET NOCOUNT ON

--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

DECLARE
@AmountTypeId INT;

--==============================================================================================================================================
--	RETURN TABLES
--	
--==============================================================================================================================================

DECLARE @ModifiedAccount table( 
	AccountId INT,
	Affected BIT,
	SpendId INT
);

--==============================================================================================================================================
--	STORED PROCEDURE TABLES
--	
--==============================================================================================================================================
DECLARE @ModifyField table( 
	Field int
);
--@pSpendDate 1
--@pAmount 2
--@pAccountsTable 3
--@pDescription 4
--@pSpendType 5
--@pCurrencyId 6


DECLARE @AccountTempTable table( 
	AccountId int
);


DECLARE @AccountPeriodTempTable table( 
	AccountPeriodId INT,
	AccountId INT
);


--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF EXISTS (SELECT * FROM dbo.TransferRecord tr WHERE tr.SpendId = @pSpendId)
	BEGIN
		raiserror ('Cannnot modified transfer record', 20, -1) WITH LOG;
	END

	INSERT INTO @ModifyField (Field)
	SELECT CAST(accPer.part as int) 
	FROM dbo.SDF_SplitString(@pModifyList,',') accPer;
	
	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 1) --@pSpendDate 1
	BEGIN	
		UPDATE dbo.Spend SET SpendDate = @pSpendDate WHERE SpendId = @pSpendId;

		INSERT INTO @AccountTempTable (AccountId)
		SELECT pacct.AccountId
		FROM @pAccountsTable pacct

		INSERT INTO @AccountPeriodTempTable (AccountPeriodId, AccountId)
		SELECT accp.AccountPeriodId, acc.AccountId FROM
		dbo.Account acc
		JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
		WHERE acc.AccountId IN (SELECT * FROM @AccountTempTable)
		AND @pSpendDate>=accp.InitialDate AND @pSpendDate < accp.EndDate AND acc.UserId = @pUserId;
		
		DELETE FROM dbo.SpendOnPeriod WHERE SpendId = @pSpendId;

		IF EXISTS (SELECT * FROM @AccountPeriodTempTable)
		BEGIN		
			INSERT INTO DBO.SpendOnPeriod (AccountPeriodId, SpendId, Denominator, Numerator, PendingUpdate)
			SELECT valids.AccountPeriodId, @pSpendId, pacct.Denominator, pacct.Numerator, pacct.PendingUpdate
			FROM @AccountPeriodTempTable valids
			JOIN @pAccountsTable pacct ON pacct.AccountId = valids.AccountPeriodId
		END

		DELETE FROM @ModifyField WHERE Field = 3;
	END

	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 2)
	BEGIN
		UPDATE dbo.Spend SET OriginalAmount = @pAmount WHERE SpendId = @pSpendId;
	END 

	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 3) --@Accounts include
	BEGIN
		IF EXISTS (SELECT * FROM @AccountTempTable) 
		BEGIN
			
			SELECT TOP 1 @pSpendDate = sp.SpendDate FROM dbo.Spend sp WHERE sp.SpendId = @pSpendId;
			
			INSERT INTO @AccountTempTable (AccountId)
			SELECT pacct.AccountId
			FROM @pAccountsTable pacct

			INSERT INTO @AccountPeriodTempTable (AccountPeriodId)
			SELECT accp.AccountPeriodId FROM
			dbo.Account acc
			JOIN dbo.AccountPeriod accp ON accp.AccountId = acc.AccountId
			WHERE acc.AccountId IN (SELECT * FROM @AccountTempTable)
			AND @pSpendDate>=accp.InitialDate AND @pSpendDate < accp.EndDate AND acc.UserId = @pUserId;

			DELETE FROM dbo.SpendOnPeriod WHERE SpendId = @pSpendId;

			IF EXISTS (SELECT * FROM @AccountPeriodTempTable)
			BEGIN		
				INSERT INTO DBO.SpendOnPeriod (AccountPeriodId, SpendId)
				SELECT valids.AccountPeriodId, @pSpendId FROM @AccountPeriodTempTable valids
			END
		END
	END

	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 4) --@pDescription
	BEGIN
		UPDATE dbo.Spend SET Description = @pDescription WHERE SpendId = @pSpendId;
	END 

	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 5) --SpendTypeId
	BEGIN
		UPDATE dbo.Spend SET SpendTypeId = @pSpendTypeId WHERE SpendId = @pSpendId;
	END 

	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 6) --Currency
	BEGIN
		UPDATE dbo.Spend SET AmountCurrencyId = @pCurrencyId WHERE SpendId = @pSpendId;
	END
	
	IF EXISTS (SELECT * FROM @ModifyField WHERE Field = 7) --Amount Type
	BEGIN
		SELECT TOP 1 @AmountTypeId = amt.AmountTypeId FROM AmountType amt 
			WHERE AmountTypeName = @pAmountTypeName COLLATE SQL_Latin1_General_CP1_CI_AS;
		UPDATE dbo.Spend SET AmountTypeId = @AmountTypeId WHERE SpendId = @pSpendId;
	END
	
	INSERT INTO @ModifiedAccount (AccountId, Affected, SpendId)
	SELECT accp.AccountId, 1, sp.SpendId FROM dbo.Spend sp
	JOIN dbo.SpendOnPeriod sop ON sop.SpendId = sp.SpendId
	JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
	WHERE sp.SpendId = @pSpendId;

	SELECT * FROM @ModifiedAccount;

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
