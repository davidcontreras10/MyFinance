
--==============================================================================================================================================
-- Script Name: 		Create FinanceApp Schema
-- Type:				Script
-- Editor Tab Spacing: 	4	
--==============================================================================================================================================
-- DESCRIPTION: 
-- This script creates the required database Schemas
--==============================================================================================================================================
-- EDIT HISTORY:
------------------------------------------------------------------------------------------------------------------------------------------------
--	Revision	Date			Who						What
--	========	====			===						====
--	1.0			2014-01-24		David Contreras			Initial Development


--==============================================================================================================================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
SET NOCOUNT ON

--==============================================================================================================================================
--	Create the database Schemas in MYFNDB database
--==============================================================================================================================================

IF SCHEMA_ID('FinanceApp') IS NULL
BEGIN
	EXECUTE('CREATE SCHEMA [FinanceApp]') -- EXEC because 'CREATE SCHEMA' must be the first statement in a query batch.
END

ENDScript:
GO
SET NOCOUNT OFF
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO


