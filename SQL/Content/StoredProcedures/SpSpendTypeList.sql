
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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpSpendTypeList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpSpendTypeList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpSpendTypeList
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
	
	EXEC [dbo].[SpSpendTypeList] @pUserId = '71722361-99FF-493F-AF02-2BD0ED7CE676', @pIncludeAll = 1
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpSpendTypeList]
@pUserId UNIQUEIDENTIFIER,
@pIncludeAll BIT
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--	The following variables will be used as internal variables to this Stored Procedure.
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE VARIABLE CONSTANTS
--	The following variables will be used as internal constants to this Stored Procedure.
--==============================================================================================================================================

DECLARE @SpendTypesItems TABLE
(
	SpendTypeId INT,
	SpendTypeName NVARCHAR(500),
	SpendTypeDescription NVARCHAR(500),
	IsDefault BIT
);


--==============================================================================================================================================
--	INITIALIZE VARIABLES and VARIABLE CONSTANTS
--	Use this section to initialize variables and set valuse for any variable constants.
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	IF @pIncludeAll = 1
	BEGIN
		INSERT INTO @SpendTypesItems(
			SpendTypeId,
			SpendTypeName,
			SpendTypeDescription
		)
		SELECT spendtypes.SpendTypeId, spendtypes.Name, spendtypes.Description
		FROM [dbo].[SpendType] spendtypes;

		UPDATE @SpendTypesItems SET IsDefault = 1
		FROM dbo.UserSpendType usrst 
		JOIN @SpendTypesItems spti ON spti.SpendTypeId = usrst.SpendTypeId
		WHERE usrst.UserId = @pUserId;
	END
	ELSE
	BEGIN
		INSERT INTO @SpendTypesItems(
			SpendTypeId,
			SpendTypeName,
			SpendTypeDescription
		)
		SELECT spendtypes.SpendTypeId, spendtypes.Name, spendtypes.Description
		FROM dbo.UserSpendType usrst
		JOIN dbo.SpendType spendtypes ON spendtypes.SpendTypeId = usrst.SpendTypeId
		WHERE usrst.UserId = @pUserId;
	END
	SELECT * FROM @SpendTypesItems;
			
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