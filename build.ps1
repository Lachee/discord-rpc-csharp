[CmdletBinding()]
Param(
    [string]$Target,
	[switch]$MakeUnityPackage,
	[switch]$MakeNugetPackage,
	[switch]$IgnoreLibraryBuild,
	[int]$BuildCount,
	[string]$BuildTag,
    [string]$Certificate
)

function GatherArtifacts([string] $dest_root, [bool]$include_unity, [bool]$include_nuget)
{
	$project = "DiscordRPC";
	$target = "Release"

	#Prepare the DLL and package names
	$manage_dll = "$project.dll";
	$unity_pack = "$project.unitypackage";

	#prepare the source folders
	$source_managed = "./$project/bin/$target";
	$source_unity = "./";
	$source_nuget = "./nupkg"

	#create the directories for the dest
	mkdir -Force $dest_root

	#copy the files
	copy -Force "$source_managed/$manage_dll"	"$dest_root/$manage_dll"
	
	Write-Host "Artifact: $dest_root/$manage_dll"
	
	if ($include_unity)
	{
		Move-Item -Force "$source_unity/$unity_pack"	"$dest_root/$unity_pack";
		Write-Host "Artifact: $dest_root/$unity_pack";
	}

	if ($include_nuget)
	{
		Move-Item -Force "$source_nuget/*" "$dest_root/"
		Write-Host "Artifact: NUGET"
	}
}

function BuildLibrary($buildCount, $buildTag, [bool]$makeNuget)
{
	Write-Host "-buildCounter=$buildCount",'-buildType="Release"'
	$args = "-buildCounter=$buildCount",'-buildType="Release"'
	if (![string]::IsNullOrEmpty($buildTag)) { $args += "-buildTag=$buildTag" }
    if (![string]::IsNullOrEmpty($Certificate))
    {
        $args += "-signCertificate=$Certificate"
        $args += "-signPassword=$env:CERTIFICATE_PASSWORD"
    }
    
	
	if ($makeNuget) 
	{
		.\build-lib.ps1 -Target "NugetBuild" -ScriptArgs $args
	}
	else
	{
		.\build-lib.ps1 -Target "Default" -ScriptArgs $args
	}

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
	BuildLibrary $BuildCount $BuildTag $MakeNugetPackage
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
}

#Gather artifacts
Write-Host ">>> Copying Packages";
GatherArtifacts ./artifacts $MakeUnityPackage $MakeNugetPackage

Write-Host ">>> Build Complete"