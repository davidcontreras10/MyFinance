
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpAddSpendAccountIncludeList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpAddSpendAccountIncludeList]
END
GO
--==============================================================================================================================================
--	Name:		 				[dbo].[SpAddSpendAccountIncludeList]
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

declare @p2 dbo.ClientAddSpendAccountInclude
insert into @p2 values(N'3',N'2',N'4',N'1002')
insert into @p2 values(N'3',N'2',N'2003',N'1002')

exec SpAddSpendAccountIncludeList @pUserId=N'71722361-99ff-493f-af02-2bd0ed7ce676',@pAccountIncludeDataTable=@p2

*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpAddSpendAccountIncludeList]
@pAccountIncludeDataTable ClientAddSpendAccountInclude READONLY,
@pUserId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES	
--==============================================================================================================================================


--==============================================================================================================================================
--	DECLARE RETURN TABLES
--==============================================================================================================================================

DECLARE @SupportedAccountTemp TABLE(
	AccountId INT,
	AccountName VARCHAR(100)
);

DECLARE @MethodDataTemp TABLE(
	CurrencyConverterMethodId INT,
	CurrencyConverterMethodName VARCHAR(100),
	IsDefault BIT
);

DECLARE @AccountIncludeMethodTemp TABLE(
	AccountId INT,
	AccountIncludeId INT,
	CurrencyConverterMethodId INT,
	IsDefault BIT,
	IsCurrentSelection BIT
);

DECLARE @AccountTemp TABLE(
	AccountId INT
);

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================

DECLARE @AccountCurrencyTemp TABLE(
	AccountId INT,
	CurrencyId INT,
	CurrencyConverterMethodId INT
);

DECLARE @AccountIncludeTemp TABLE(
	AccountId INT,
	AccountIncludeId INT,
	CurrencyConverterMethodId INT
);

DECLARE @AccountAux TABLE(
	AccountId INT,
	CurrencyId INT
);

--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--==============================================================================================================================================
	

--==============================================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
			
	INSERT INTO @AccountTemp (AccountId)
	SELECT DISTINCT pacci.AccountId FROM @pAccountIncludeDataTable pacci;
	
	INSERT INTO @AccountAux (AccountId, CurrencyId)
	SELECT acc.AccountId, acc.CurrencyId
	FROM dbo.Account acc
	WHERE acc.UserId = @pUserId;

	UPDATE @AccountAux SET CurrencyId = paccidt.AmountCurrencyId
	FROM @pAccountIncludeDataTable paccidt 
	JOIN @AccountAux accx ON accx.AccountId = paccidt.AccountId;

	INSERT INTO @AccountCurrencyTemp (AccountId, CurrencyId, CurrencyConverterMethodId)
	SELECT acc.AccountId, crrc.CurrencyIdOne, crrcm.CurrencyConverterMethodId
	FROM dbo.Account acc
	JOIN dbo.CurrencyConverter crrc ON crrc.CurrencyIdTwo = acc.CurrencyId
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterId = crrc.CurrencyConverterId
	WHERE acc.AccountId IN (SELECT AccountId FROM @AccountTemp);

	INSERT INTO @AccountIncludeMethodTemp (AccountId, AccountIncludeId, CurrencyConverterMethodId)
	SELECT acc1.AccountId, acc2.AccountId, crrcm.CurrencyConverterMethodId
	FROM @AccountAux acc1, dbo.Account acc2, dbo.CurrencyConverter crrc
	JOIN dbo.CurrencyConverterMethod crrcm ON crrcm.CurrencyConverterId = crrc.CurrencyConverterId
	WHERE acc1.AccountId IN (SELECT AccountId FROM @AccountCurrencyTemp) AND acc2.UserId = @pUserId
	AND crrc.CurrencyIdOne = acc1.CurrencyId AND crrc.CurrencyIdTwo = acc2.CurrencyId
	AND acc1.AccountId <> acc2.AccountId
	ORDER BY acc1.AccountId;
	
	UPDATE @AccountIncludeMethodTemp SET IsCurrentSelection = 1
	FROM @pAccountIncludeDataTable paccidt 
	JOIN @AccountIncludeMethodTemp accimt ON accimt.AccountId = paccidt.AccountId
	AND accimt.AccountIncludeId = paccidt.AccountIncludeId;

	UPDATE @AccountIncludeMethodTemp SET IsDefault = 1
	FROM @pAccountIncludeDataTable paccidt 
	JOIN @AccountIncludeMethodTemp accimt ON accimt.AccountId = paccidt.AccountId
	AND accimt.AccountIncludeId = paccidt.AccountIncludeId AND
	accimt.CurrencyConverterMethodId = paccidt.CurrencyConverterMethodId;

	INSERT INTO @AccountIncludeTemp (AccountId, AccountIncludeId,CurrencyConverterMethodId)
	SELECT acc.AccountId, acci.AccountIncludeId, acci.CurrencyConverterMethodId
	FROM dbo.Account acc
	JOIN dbo.AccountInclude acci ON acci.AccountId = acc.AccountId
	WHERE acc.UserId = @pUserId;
	
	UPDATE @AccountIncludeMethodTemp SET IsDefault = 1
	FROM @AccountIncludeTemp accit
	JOIN @AccountIncludeMethodTemp accimt 
	ON accimt.AccountId = accit.AccountId AND accimt.AccountIncludeId = accit.AccountIncludeId 
		AND accimt.CurrencyConverterMethodId = accit.CurrencyConverterMethodId;

	UPDATE accimt SET accimt.IsDefault = 1
	FROM @AccountIncludeMethodTemp accimt
	JOIN dbo.Account acc_from ON acc_from.AccountId = accimt.AccountId
	JOIN dbo.CurrencyConverterMethod ccm ON ccm.CurrencyConverterMethodId = accimt.CurrencyConverterMethodId
	WHERE acc_from.FinancialEntityId = ccm.FinancialEntityId

	INSERT INTO @SupportedAccountTemp (AccountId,AccountName)
	SELECT acc.AccountId, acc.Name
	FROM dbo.Account acc
	WHERE acc.AccountId IN (SELECT AccountIncludeId FROM @AccountIncludeMethodTemp)

	INSERT INTO @MethodDataTemp (CurrencyConverterMethodId, CurrencyConverterMethodName, IsDefault)
	SELECT crrcm.CurrencyConverterMethodId, crrcm.Name, crrcm.IsDefault
	FROM dbo.CurrencyConverterMethod crrcm
	WHERE crrcm.CurrencyConverterMethodId IN (SELECT CurrencyConverterMethodId FROM @AccountCurrencyTemp) 
	OR crrcm.CurrencyConverterMethodId IN (SELECT CurrencyConverterMethodId FROM @AccountIncludeMethodTemp) 
	OR crrcm.CurrencyConverterMethodId IN (SELECT CurrencyConverterMethodId FROM @AccountIncludeTemp) 
	
	--SELECT * FROM @AccountAux;	
		
	SELECT * FROM @SupportedAccountTemp;
	SELECT * FROM @MethodDataTemp;
	SELECT * FROM @AccountIncludeMethodTemp;
	SELECT * FROM @AccountTemp;

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
