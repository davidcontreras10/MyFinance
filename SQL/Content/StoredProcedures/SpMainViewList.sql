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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpMainViewList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpMainViewList]
END
GO
--==============================================================================================================================================
--	EXEC Statement:
------------------------------------------------------------------------------------------------------------------------------------------------
--	The DECLARE, SELECT, and EXEC statements in the following example should match the stored procedure input
--	parameters.
/*
	
	EXEC SpMainViewList
	@pUserId = 'test'
	,@pDate = '2015-10-06'
	@pDate = '2015-09-1' 
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpMainViewList]
@pUserId UNIQUEIDENTIFIER,
@pDate DATETIME = NULL
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	RETURN TABLES
--==============================================================================================================================================


DECLARE @AccountItems TABLE
(
	AccountId INT,
	AccountName NVARCHAR (500),
	CurrencytId INT,
	CurrencyName NVARCHAR (500),
	AccountPeriodId INT,
	CurrentPeriodId INT,
	AccountPeriodInitialDate DATETIME,
	AccountPeriodEndDate DATETIME,
	Position INT,
	AccountTypeId INT,
	HeaderColor NVARCHAR (500),
	AccountGroupId INT,
	AccountGroupName NVARCHAR (500),
	AccountGroupDisplayValue NVARCHAR(500),
	AccountGroupPosition INT,
	AccountGroupDisplayDefault BIT
);



--==============================================================================================================================================
--	DECLARE VARIABLE 
--	The following variables will be used as internally to this Stored Procedure.
--==============================================================================================================================================

DECLARE
@periodDate DATETIME;

DECLARE
@CurrentPeriodTemp TABLE
(
	AccountId INT,
	AccountPeriodId INT,
	Budget FLOAT,
	InitialDate DATETIME,
	EndDate DATETIME
);


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

	EXEC dbo.SpUpdateCurrentPeriods @pUserId = @pUserId, @pDate = @periodDate;
	
	INSERT INTO @CurrentPeriodTemp
	EXEC dbo.SpCurrentAccountPeriodList @pUserId = @pUserId, @pDate = @periodDate;

	INSERT INTO @AccountItems
	(AccountId, AccountName, AccountPeriodId, CurrentPeriodId, CurrencyName, 
	CurrencytId, AccountPeriodEndDate, AccountPeriodInitialDate, Position, AccountTypeId, 
	HeaderColor, AccountGroupId, AccountGroupName, AccountGroupDisplayValue, AccountGroupPosition, AccountGroupDisplayDefault)
	SELECT acc.AccountId, acc.Name, accp.AccountPeriodId, NULL, curr.Name, curr.CurrencyId, accp.EndDate, 
		accp.InitialDate, acc.Position, acc.AccountTypeId, acc.HeaderColor, acc.AccountGroupId, accg.AccountGroupName, 
		accg.DisplayValue, accg.AccountGroupPosition, accg.DisplayDefault
	FROM dbo.AccountPeriod accp
	JOIN dbo.Account acc ON accp.AccountId = acc.AccountId
	--JOIN @CurrentPeriodTemp caccp ON accp.AccountId = acc.AccountId
	JOIN dbo.Currency curr ON curr.CurrencyId = acc.CurrencyId
	JOIN dbo.AccountGroup accg ON accg.AccountGroupId = acc.AccountGroupId
	WHERE acc.UserId = @pUserId;

	UPDATE @AccountItems SET CurrentPeriodId = ctp.AccountPeriodId
	FROM 
	@CurrentPeriodTemp ctp
	JOIN @AccountItems acci ON acci.AccountId = ctp.AccountId;

	SELECT * FROM @AccountItems acc ORDER BY acc.Position;
	--SELECT * FROM @CurrentPeriodTemp;
	

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
