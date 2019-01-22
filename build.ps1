[CmdletBinding()]
Param(
    [string]$Target,
	[switch]$MakeUnityPackage,
	[switch]$IgnoreLibraryBuild,
	[int]$BuildCount
)

function GatherArtifacts([string] $dest_root, [bool]$include_unity)
{
	$project = "DiscordRPC";
	$target = "Release"

	#Prepare the DLL and package names
	$manage_dll = "$project.dll";
	$unity_pack = "$project.unitypackage";

	#prepare the source folders
	$source_managed = "./$project/bin/$target";
	$source_unity = "./";

	#create the directories for the dest
	mkdir -Force $dest_root

	#copy the files
	copy -Force "$source_managed/$manage_dll"	"$dest_root/$manage_dll"
	
	Write-Host "Artifact: $dest_root/$manage_dll"
	
	if ($include_unity)
	{
		copy -Force "$source_unity/$unity_pack"	"$dest_root/$unity_pack";
		Write-Host "Artifact: $dest_root/$unity_pack";
	}
}

function BuildLibrary($buildcount)
{
	Write-Host "-buildCounter=$buildcount",'-buildType="Release"'
	.\build-lib.ps1 -ScriptArgs "-buildCounter=$buildcount",'-buildType="Release"'
	if ($LASTEXITCODE -ne 0) 
	{
		Throw "Failed to build library."
	}
}

function BuildUnity() 
{
	.\build-unity.ps1
	if ($LASTEXITCODE -ne 0) 
	{
		Throw "Failed to build unity package."
	}
}

#Build the library 
if (!($IgnoreLibraryBuild)) {
	Write-Host ">>> Building Library";
	BuildLibrary $BuildCount
	if ($LASTEXITCODE -ne 0)
	{
		throw "Error occured while building the project.";
	}
}

#Build the Unity package
if ($MakeUnityPackage)
{
	Write-Host ">>> Building Unity Package";
	BuildUnity
	if ($LASTEXITCODE -ne 0)
	{
		throw "Error occured while building the unity package.";
	}

	#Gather artifacts
	Write-Host ">>> Copying Packages";
	GatherArtifacts ./artifacts $True
}
else
{
	#Gather artifacts
	Write-Host ">>> Copying Packages";
	GatherArtifacts ./artifacts $False
}


Write-Host ">>> Build Complete"