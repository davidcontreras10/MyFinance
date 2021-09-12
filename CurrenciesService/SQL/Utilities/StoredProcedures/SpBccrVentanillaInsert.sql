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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpBccrVentanillaInsert]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpBccrVentanillaInsert]
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
	
	EXEC SpBccrVentanillaInsert
	@pUsername = 'test' 
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpBccrVentanillaInsert]
@pValuesTable BccrVentanillaObject READONLY
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--==============================================================================================================================================

DECLARE @CurrentDate DATETIME;
DECLARE @InsertAttemptCount INT;
DECLARE @InsertRealCount INT;

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================
	
DECLARE @InsertValuesTemp TABLE(
EntityName VARCHAR(500) NOT NULL,
Purchase FLOAT NOT NULL,
Sell FLOAT NOT NULL,
LastUpdate DATETIME NOT NULL
);

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY
	
	INSERT INTO @InsertValuesTemp
	SELECT * FROM @pValuesTable pvt
	WHERE NOT EXISTS (
		SELECT * 
		FROM dbo.BccrVentanilla bccrv
		WHERE bccrv.LastUpdate = pvt.LastUpdate AND bccrv.EntityName = pvt.EntityName
	)

	INSERT INTO dbo.BccrVentanilla 
	SELECT * FROM @InsertValuesTemp;

	SET @CurrentDate = GETDATE();
	SET @InsertRealCount = (SELECT COUNT(*) FROM @InsertValuesTemp);
	SET @InsertAttemptCount = (SELECT COUNT(*) FROM @pValuesTable);

	INSERT INTO dbo.BccrVentanillaInsertLog (InsertDate, InsertAttemptCount, InsertRealCount)
	VALUES (@CurrentDate, @InsertAttemptCount, @InsertRealCount);

	SELECT * FROM @InsertValuesTemp;
			
END TRY
BEGIN CATCH

    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH	
--==============================================================================================================================================
--	TRAP Errors
--	Set return code and error id output values
--==============================================================================================================================================
ERRORFinish:

SET NOCOUNT OFF
RETURN
GO
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
