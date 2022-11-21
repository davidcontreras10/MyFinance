$windir = [System.Environment]::ExpandEnvironmentVariables("%WINDIR%");

$vs17dir = 'C:\Program Files (x86)\Microsoft Visual Studio\2017'
$vs19dir = 'C:\Program Files (x86)\Microsoft Visual Studio\2019'
$vs22dir = 'C:\Program Files\Microsoft Visual Studio\2022'
$vs17communityDir = $vs17dir + '\Community\MSBuild\15.0\Bin'
$vs17enterpriseDir = $vs17dir + '\Enterprise\MSBuild\15.0\Bin'
$vs17professionalDir = $vs17dir + '\Professional\MSBuild\15.0\Bin'
$vs17genericDir = $vs17dir + '\BuildTools\MSBuild\15.0\Bin'
$vs19enterprise = $vs19dir + '\Enterprise\MSBuild\Current\Bin'
$vs22communityDir = $vs22dir + '\Community\MSBuild\Current\Bin'

# if(Test-Path -Path $vs17communityDir){
	# $msBuildDir = $vs17communityDir
# }

# if(Test-Path -Path $vs17enterpriseDir){
	# $msBuildDir = $vs17enterpriseDir
# }

# if(Test-Path -Path $vs17enterpriseDir){
	# $msBuildDir = $vs17enterpriseDir
# }

$msBuildDir = $vs22communityDir
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
# $nugetRestoreCurrencyUpdateService = $nugetRestoreCommand + $currencyUpdateServiceApiDir + '\CurrencyUpdateService.sln'
$nugetRestoreMyFinanceMobile = $nugetRestoreCommand + $mobileApplication + '\MyFinanceMobile.sln'

Invoke-Expression $nugetRestoreDataAccess
Invoke-Expression $nugetRestoreMyFinanceApplication
Invoke-Expression $nugetRestoreMyFinanceWebApi
Invoke-Expression $nugetRestoreCurrencyService
# Invoke-Expression $nugetRestoreCurrencyUpdateService
# Invoke-Expression $nugetRestoreMyFinanceMobile

$buildUtilities = '& "' + $msBuildDir + '\msbuild.exe" ' + $utilitiesDir + '\Utilities.csproj /t:Rebuild /fl /flp:logfile="' + $logFolder + '\Utilities.log"'
$buildDataAccess = '& "' + $msBuildDir + '\msbuild.exe" ' + $dataAccessDir + '\DataAccessSolution.sln /fl /flp:logfile="' + $logFolder + '\DataAccessSolution.log"'
$buildyFinanceApplication = '& "' + $msBuildDir + '\msbuild.exe" ' + $myFinanceApplicationDir + '\MyFinanceApplication.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\MyFinanceApplication.log"'
$buildMyFinanceWebApi = '& "' + $msBuildDir + '\msbuild.exe" ' + $myFinanceWebApiDir + '\MyFinanceWebApi.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\MyFinanceWebApi.log"'
$buildCurrencyService = '& "' + $msBuildDir + '\msbuild.exe" ' + $currencyServiceApiDir + '\CurrencyService.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\CurrencyService.log"'
# $buildCurrencyUpdateService = '& "' + $msBuildDir + '\msbuild.exe" ' + $currencyUpdateServiceApiDir + '\CurrencyUpdateService.csproj /t:Rebuild /fl /flp:logfile="' + $logFolder + '\CurrencyUpdateService.log"'
# $buildMyFinanceMobile = '& "' + $msBuildDir + '\msbuild.exe" ' + $mobileApplication + '\MyFinanceMobile.sln /t:Rebuild /fl /flp:logfile="' + $logFolder + '\MyFinanceMobile.log"'

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