if (([string]::IsNullOrEmpty($build_number))) {
    $build_number = $args[0]
}

if (([string]::IsNullOrEmpty($file_changed))) {
    $file_changed = $args[1]
}

$vs17dir = 'C:\Program Files (x86)\Microsoft Visual Studio\2017'
$vs17communityDir = $vs17dir + '\Community\MSBuild\15.0\Bin'
$vs17enterpriseDir = $vs17dir + '\Enterprise\MSBuild\15.0\Bin'
$vs17professionalDir = $vs17dir + '\Professional\MSBuild\15.0\Bin'

if (Test-Path -Path $vs17communityDir) {
    $msBuild17Dir = $vs17communityDir
}

if (Test-Path -Path $vs17enterpriseDir) {
    $msBuild17Dir = $vs17enterpriseDir
}

if (Test-Path -Path $vs17enterpriseDir) {
    $msBuild17Dir = $vs17enterpriseDir
}

if (Test-Path -Path $vs17professionalDir) {
    $msBuild17Dir = $vs17professionalDir
}

$baseProjectsDir = split-path -parent $PSScriptRoot
$sqlExportFiles = $baseProjectsDir + '\SQL' 
$myFinanceApplicationDir = $baseProjectsDir + '\MyFinanceApplication\MyFinanceWebApp'
$myFinanceWebApiDir = $baseProjectsDir + '\WebApiSolution\MyFinanceWebApi\MyFinanceWebApi'
$logFolder = 'C:\LogFiles\MyFinancePublish'

if (!(Test-Path -Path $logFolder)) {
    New-Item -ItemType directory -Path $logFolder
}

. $baseProjectsDir\Build\utilities.ps1

$versionFileHasChanged = File-Contains $file_changed "project.json"
write-output $versionFileHasChanged
if ($versionFileHasChanged -Eq $false) {
    write-output "Deploy skipped. No version changed"
    exit 0
    return 0
}
else {
    write-output "File version changed"
}

$new_version_project = $build_number + '_'
$defaultPublishFileLocation = '\Properties\PublishProfiles\'

$MyFinanceWebProj = @{
	'appBaseDir' = $myFinanceApplicationDir
    'publishFileLocation' = $defaultPublishFileLocation
    'csprojFile' = '\MyFinanceWebApp.csproj'
    'publishFileLogSufix' = 'MyFinanceWeb.log'
}

$MyFinanceWebApiProj = @{
	'appBaseDir' = $myFinanceWebApiDir
    'publishFileLocation' = $defaultPublishFileLocation
    'csprojFile' = '\MyFinanceWebApi.csproj'
    'publishFileLogSufix' = 'MyFinanceWebApi.log'
}

$projectsToDeploy = @($MyFinanceWebProj, $MyFinanceWebApiProj)

foreach($project in $projectsToDeploy){

    $publishFileLogSufix = $project.publishFileLogSufix
    $csprojFile = $project.csprojFile
    $appBaseDir = $project.appBaseDir
    $publishFileLocation = $project.publishFileLocation

    $publishDevFile = $appBaseDir + $publishFileLocation + 'DevTest.pubxml'
    $devPublishCommand = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $appBaseDir + $csprojFile + ' /p:DeployOnBuild=true /p:PublishProfile="' + $publishDevFile + '" /p:c_version="' + $new_version_project + '" /flp:logfile="' + $logFolder + '\Dev' + $publishFileLogSufix + '"'
    $devPublishPath = Get-Publish-File-Deploy-Path $publishDevFile $new_version_project

    $publishPreProdFile = $appBaseDir + $publishFileLocation + 'PreProd.pubxml'
    $preProdPublishCommand = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $appBaseDir + $csprojFile + ' /p:DeployOnBuild=true /p:PublishProfile="' + $publishPreProdFile + '" /p:c_version="' + $new_version_project + '" /flp:logfile="' + $logFolder + '\PreProd' + $publishFileLogSufix + '"'
    $PreProdPublishPath = Get-Publish-File-Deploy-Path $publishPreProdFile $new_version_project

    $publishProdFile = $appBaseDir + $publishFileLocation + 'Prod.pubxml'
    $prodPublishCommand = '& "' + $msBuild17Dir + '\msbuild.exe" ' + $appBaseDir + $csprojFile + ' /p:DeployOnBuild=true /p:PublishProfile="' + $publishProdFile + '" /p:c_version="' + $new_version_project + '" /flp:logfile="' + $logFolder + '\Prod' + $publishFileLogSufix + '"'
    $ProdPublishPath = Get-Publish-File-Deploy-Path $publishProdFile $new_version_project

    if (!$LastExitCode) {
        Invoke-Expression $devPublishCommand
    }
    else{
        exit $LastExitCode
    }

    if (!$LastExitCode) {
        Invoke-Expression $preProdPublishCommand
    }
    else{
        exit $LastExitCode
    }

    if (!$LastExitCode) {
        Invoke-Expression $prodPublishCommand
    }
    else{
        exit $LastExitCode
    }
}

$sqlDevPublishPath = $devPublishPath + "\SQL"
Copy-Item -Path $sqlExportFiles -Destination $sqlDevPublishPath -recurse -Force

$sqlPreProdPublishPath = $PreProdPublishPath + "\SQL"
Copy-Item -Path $sqlExportFiles -Destination $sqlPreProdPublishPath -recurse -Force

$sqlProdPublishPath = $ProdPublishPath + "\SQL"
Copy-Item -Path $sqlExportFiles -Destination $sqlProdPublishPath -recurse -Force

exit $LastExitCode