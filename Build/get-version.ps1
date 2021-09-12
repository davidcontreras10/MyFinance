$windir = [System.Environment]::ExpandEnvironmentVariables("%WINDIR%");
$baseProjectsDir = split-path -parent $PSScriptRoot
. $baseProjectsDir\Build\utilities.ps1

$configFilePath = $baseProjectsDir + "\project.json"

if (([string]::IsNullOrEmpty($build_count)))
{
    $build_count = $args[0]
}

$configData = Get-Project-Config -configPath $configFilePath
$currentVersion = [version]$configData.version
$branchName = $configData.branchInfo.name

$myFinanceApplicationDir = $baseProjectsDir + '\MyFinanceApplication\MyFinanceWebApp\Properties\AssemblyInfo.cs'
$myFinanceWebApiDir = $baseProjectsDir + '\WebApiSolution\MyFinanceWebApi\MyFinanceWebApi\Properties\AssemblyInfo.cs'

$newVersion = "{0}.{1}.{2}.{3}" -f $currentVersion.Major, $currentVersion.Minor, $build_count, $currentVersion.Revision

$assemblyFiles = @($myFinanceApplicationDir, $myFinanceWebApiDir)

foreach($assemblyFile in $assemblyFiles){
	Set-Assembly-Version $assemblyFile ([version]$newVersion)
}

$build_number_id = $newVersion + "-" + $branchName
Write-Host "##teamcity[buildNumber '$build_number_id']"

# $version = Get-Next-Version -build_number "756"
# write-output $version

#$version = Get-Current-Version
#$versionString = Version-To-String $version
#write-output $versionString