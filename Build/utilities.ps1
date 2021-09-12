#=================== Library file ==========================
function Get-Current-Version($assemblyFilePath){
	$pattern = '\[assembly: AssemblyVersion\("(.*)"\)\]'
	(Get-Content $assemblyFilePath) | ForEach-Object{
		if($_ -match $pattern){
			# We have found the matching line
			# Edit the version number and put back.
			$fileVersion = [version]$matches[1]
			#$newVersion = "{0}.{1}.{2}.{3}" -f $fileVersion.Major, $fileVersion.Minor, $fileVersion.Build, ($fileVersion.Revision + 1)
			#$currentVersion = "{0}.{1}.{2}.{3}" -f $fileVersion.Major, $fileVersion.Minor, $fileVersion.Build, $fileVersion.Revision
			return $fileVersion
			# '[assembly: AssemblyVersion("{0}")]' -f $newVersion
		} else {
			# Output line as is
			#$_
		}
	}
}

function Set-Assembly-Version($assemblyFilePath, $nextVersion){
	$assemblyEntries = @("AssemblyVersion", "AssemblyFileVersion")
	foreach($assemblyEntry in $assemblyEntries){
		$pattern = '\[assembly: {0}\("(.*)"\)\]' -f $assemblyEntry
		(Get-Content $assemblyFilePath) | ForEach-Object{
			if($_ -match $pattern){
				# We have found the matching line
				# Edit the version number and put back.
				#$fileVersion = [version]$matches[1]
				$newVersion = "{0}.{1}.{2}.{3}" -f $nextVersion.Major, $nextVersion.Minor, $nextVersion.Build, $nextVersion.Revision
				'[assembly: {0}("{1}")]' -f $assemblyEntry, $newVersion
			} else {
				# Output line as is
				$_
			}
		} | Set-Content $assemblyFilePath
	}
}

function Version-To-String($version){
	$stringVersion = "{0}.{1}.{2}.{3}" -f $version.Major, $version.Minor, $version.Build, $version.Revision
	return $stringVersion
}

function Get-Next-Version($build_number, $assemblyFilePath){
  $current_version = Get-Current-Version -assemblyFilePath $assemblyFilePath
  $newVersion = "{0}.{1}.{2}.{3}" -f $current_version.Major, $current_version.Minor, $build_number, $current_version.Revision
  return $newVersion;
}

function Get-Project-Config($configPath){
	$json = Get-Content $configPath | Out-String | ConvertFrom-Json	
	return $json;
}

function File-Contains($filePath, $evalContent){
	$fileContent = Get-Content $filePath
	$isString = $fileContent.GetType().Name -Eq "String"
	if($isString){
		$isMatched = $fileContent -Match $evalContent
		return $isMatched
	}
	for ($i = 0; $i -lt $fileContent.Count ; $i++){
		$contentLine = $fileContent[$i]
		if($contentLine -Match $evalContent){
			return $true
		}
	}
	
	return $false
}

function Get-Publish-File-Deploy-Path($filePath, $new_version_project){
	[System.Xml.XmlDocument]$fileContent = Get-Content $filePath
	$url = $fileContent.Project.PropertyGroup.publishUrl
	$url = $url.replace('$(c_version)', $new_version_project)
	return Remove-Last-Folder-From-Path $url
}

function Remove-Last-Folder-From-Path($path){
	$path = $path -replace "\\[^\\]*(?:\\)?$"
	return $path;
} 

#=================== End Library file ==========================