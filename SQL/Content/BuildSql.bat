@ECHO OFF

set /p Server=Server?(y/n) %=%
set /p Database=Database name?(y/n) %=%
set /p User=Username?(y/n) %=%
set /p Password=Password?(y/n) %=%

:while1
set /p aConfirmReCreate=Do you want to re-create the tables?(y/n) %=%
if "%aConfirmReCreate%" == "y" (goto confirmYes) else (goto confirmNo)
:confirmYes
set /p aConfirmReCreate2=Confirm re-create tables. This will remove all the data. Do you want to continue?(y/n) %=%
if "%aConfirmReCreate2%" == "y" (set cWhile1=0) else (set cWhile1=1)
goto continue1
:confirmNo
if "%aConfirmReCreate%" == "n" (set cWhile1=0) else (set cWhile1=1)
:continue1
if "%cWhile1%" == "1" (goto while1)

set storedProcedures=StoredProcedures\*.sql
set appFunctions=Functions\*.sql
set functions=..\Utilities\*.sql
set createTable=CreateTables.sql
set createTypes=CreateTypes.sql

ECHO Running Create custom types
call :RunScript %createTypes%

ECHO Running Functions
FOR %%i IN (%functions%) DO call :RunScript %%i

if "%aConfirmReCreate%" == "y" (
ECHO Running Create tables script
call :RunScript %createTable%
)

ECHO Running app functions
FOR %%i IN (%appFunctions%) DO call :RunScript %%i

ECHO Running Stored Procedures
FOR %%i IN (%storedProcedures%) DO call :RunScript %%i

ECHO Finished Sql function!
GOTO :END 
 
:RunScript 
Echo Executing Script: %1 
SQLCMD /S %Server% /d %Database% -U %User% -P %Password% -i %1
Echo Completed Script: %1  
:END
