$windir = [System.Environment]::ExpandEnvironmentVariables("%WINDIR%");

$vs17dir = 'C:\Program Files (x86)\Microsoft Visual Studio\2017'
$vs17communityDir = $vs17dir + '\Community\MSBuild\15.0\Bin'
$vs17enterpriseDir = $vs17dir + '\Enterprise\MSBuild\15.0\Bin'
$vs17professionalDir = $vs17dir + '\Professional\MSBuild\15.0\Bin'

if(Test-Path -Path $vs17communityDir){
	$msBuild17Dir = $vs17communityDir
}

if(Test-Path -Path $vs17enterpriseDir){
	$msBuild17Dir = $vs17enterpriseDir
}

if(Test-Path -Path $vs17enterpriseDir){
	$msBuild17Dir = $vs17enterpriseDir
}

$msBuild11Dir=$windir+ '\Microsoft.NET\Framework\v4.0.30319'

$msBuild14Dir = 'C:\Program Files (x86)\MSBuild\14.0\Bin'
$baseProjectsDir = split-path -parent $PSScriptRoot
$solutionDir = '..\MyFinanceApplication'
$utilitiesDir = $baseProjectsDir + '\Utilities\Utilities'
$dataAccessDir = $baseProjectsDir + '\DataAccessSolution'
$myFinanceApplicationDir = $baseProjectsDir + '\MyFinanceApplication'
$myFinanceWebApiDir = $baseProjectsDir + '\WebApiSolution\MyFinanceWebApi'
$currencyServiceApiDir = $baseProjectsDir + '\CurrenciesService\CurrencyService'
$currencyUpdateServiceApiDir = $baseProjectsDir + '\CurrenciesService\CurrencyUpdateService'
$mobileApplication = $baseProjectsDir + '\MyFinanceMobile'
$logFolder = 'C:\LogFiles\MyFinanceApplicationBuildLog'
$nugetFolder = split-path -parent $PSScriptRoot
$nugetFolder = $nugetFolder + '\Tools\Nuget'
$nugetRestoreCommand = $nugetFolder + '\NuGet.exe restore ' 

if(!(Test-Path -Path $logFolder)){
	New-Item -ItemType directory -Path $logFolder
}

$nugetRestoreDataAccess = $nugetRestoreCommand + $dataAccessDir + '\DataAccessSolution.sln'
$nugetRestoreMyFinanceApplication = $nugetRestoreCommand + $myFinanceApplicationDir + '\MyFinanceApplication.sln'
$nugetRestoreMyFinanceWebApi = $nugetRestoreCommand + $myFinanceWebApiDir + '\MyFinanceWebApi.sln'
$nugetRestoreCurrencyService = $nugetRestoreCommand + $currencyServiceApiDir + '\CurrencyService.sln'
$nugetRestoreCurrencyUpdateService = $nugetRestoreCommand + $currencyUpdateServiceApiDir + '\CurrencyUpdateService.sln'
$nugetRestoreMyFinanceMobile = $nugetRestoreCommand + $mobileApplication + '\MyFinanceMobile.sln'

Invoke-Expression $nugetRestoreDataAccess
Invoke-Expression $nugetRestoreMyFinanceApplication
Invoke-Expression $nugetRestoreMyFinanceWebApi
Invoke-Expression $nugetRestoreCurrencyService
# Invoke-Expression $nugetRestoreCurrencyUpdateService
Invoke-Expression $nugetRestoreMyFinanceMobile

$buildUtilities = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $utilitiesDir + '\Utilities.csproj /t:Rebuild /fl /flp:logfile="' + $logFolder + '\Utilities.log"'
$buildDataAccess = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $dataAccessDir + '\DataAccessSolution.sln /fl /flp:logfile="' + $logFolder + '\DataAccessSolution.log"'
$buildyFinanceApplication = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $myFinanceApplicationDir + '\MyFinanceApplication.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\MyFinanceApplication.log"'
$buildMyFinanceWebApi = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $myFinanceWebApiDir + '\MyFinanceWebApi.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\MyFinanceWebApi.log"'
$buildCurrencyService = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $currencyServiceApiDir + '\CurrencyService.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\CurrencyService.log"'
$buildCurrencyUpdateService = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $currencyUpdateServiceApiDir + '\CurrencyUpdateService.csproj /t:Rebuild /fl /flp:logfile="' + $logFolder + '\CurrencyUpdateService.log"'
$buildMyFinanceMobile = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $mobileApplication + '\MyFinanceMobile.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\MyFinanceMobile.log"'

Invoke-Expression $buildUtilities
# Write-Output $Success
if(!$LastExitCode){
	Invoke-Expression $buildDataAccess	
}

if(!$LastExitCode){
	Invoke-Expression $buildyFinanceApplication
}

if(!$LastExitCode){
	Invoke-Expression $buildMyFinanceWebApi
}

if(!$LastExitCode){
	Invoke-Expression $buildCurrencyService
}

exit $LastExitCode
# Write-Output $LastExitCode