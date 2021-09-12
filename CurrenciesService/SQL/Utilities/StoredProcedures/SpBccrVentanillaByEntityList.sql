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
	WHERE	Id = OBJECT_ID(N'[dbo].[SpBccrVentanillaByEntityList]')
	AND		OBJECTPROPERTY(Id, N'ISPROCEDURE') = 1
)
BEGIN
	DROP PROCEDURE [dbo].[SpBccrVentanillaByEntityList]
END
GO
--==============================================================================================================================================
--	Name:		 				dbo.SpBccrVentanillaByEntityList
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
	
	EXEC SpBccrVentanillaByEntityList
	@pEntityNameMatch = '%Banco Bac%' 
*/
--==============================================================================================================================================
CREATE PROCEDURE [dbo].[SpBccrVentanillaByEntityList]
@pEntityNameMatch VARCHAR(500)
AS
SET NOCOUNT ON



--==============================================================================================================================================
--	DECLARE VARIABLES
--==============================================================================================================================================

--==============================================================================================================================================
--	DECLARE TABLE VARIABLES
--==============================================================================================================================================
	

--====================================================================================================================
--	BEGIN LOGIC
--==============================================================================================================================================
BEGIN TRY

	SELECT * FROM dbo.BccrVentanilla bccrv
	WHERE bccrv.EntityName LIKE @pEntityNameMatch;
			
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
