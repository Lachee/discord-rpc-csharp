[CmdletBinding()]
Param(
    [string]$Target,
    [string[]]$ScriptArgs
)

#MD5 Hasing Algo required for NuGet
[Reflection.Assembly]::LoadWithPartialName("System.Security") | Out-Null
function MD5HashFile([string] $filePath) {
    if ([string]::IsNullOrEmpty($filePath) -or !(Test-Path $filePath -PathType Leaf))
    {
        return $null
    }

    [System.IO.Stream] $file = $null;
    [System.Security.Cryptography.MD5] $md5 = $null;
    try
    {
        $md5 = [System.Security.Cryptography.MD5]::Create()
        $file = [System.IO.File]::OpenRead($filePath)
        return [System.BitConverter]::ToString($md5.ComputeHash($file))
    }
    finally
    {
        if ($file -ne $null)
        {
            $file.Dispose()
        }
    }
}
function GetProxyEnabledWebClient {
    $wc = New-Object System.Net.WebClient
    $proxy = [System.Net.WebRequest]::GetSystemWebProxy()
    $proxy.Credentials = [System.Net.CredentialCache]::DefaultCredentials        
    $wc.Proxy = $proxy
    return $wc
}

function RestoreTools([string] $toolpath, [string] $nugetpath, [string] $cakepath) {
    
    #prepare some variables
    $packageconf = Join-Path $toolpath "packages.config"
    $packageconf_md5 = Join-Path $toolpath "packages.config.md5sum"

    # Try find NuGet.exe in path if not exists
    if (!(Test-Path $nugetpath)) {
        Write-Host "Trying to find nuget.exe in PATH..."
        $existingPaths = $Env:Path -Split ';' | Where-Object { (![string]::IsNullOrEmpty($_)) -and (Test-Path $_ -PathType Container) }
        $NUGET_EXE_IN_PATH = Get-ChildItem -Path $existingPaths -Filter "nuget.exe" | Select -First 1
        if ($NUGET_EXE_IN_PATH -ne $null -and (Test-Path $NUGET_EXE_IN_PATH.FullName)) {
            Write-Verbose -Message "Found in PATH at $($NUGET_EXE_IN_PATH.FullName)."
            $nugetpath = $NUGET_EXE_IN_PATH.FullName
        }
    }

    # Make sure tools folder exists
    if (!(Test-Path $toolpath)) 
    {
        Write-Host "Creating tools directory..."
        New-Item -Path $toolpath -Type directory | out-null
    }
    

    # Make sure that packages.config exist.
    if (!(Test-Path $packageconf)) {
        Write-Host "Downloading packages.config..."    
        try {        
            $wc = GetProxyEnabledWebClient
            $wc.DownloadFile("https://cakebuild.net/download/bootstrapper/packages", $packageconf) } catch {
            Throw "Could not download packages.config."
        }
    }

    # Try download NuGet.exe if not exists
    if (!(Test-Path $nugetpath)) {
        Write-Verbose -Message "Downloading NuGet.exe..."
        try {
            $wc = GetProxyEnabledWebClient
            $wc.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", $nugetpath)
        } catch {
            Throw "Could not download NuGet.exe."
        }
    }
    
    # Save nuget.exe path to environment to be available to child processed
    $ENV:NUGET_EXE = $NUGET_EXE

    # Restore tools from NuGet?
    if(-Not $SkipToolPackageRestore.IsPresent) {
        Push-Location
        Set-Location $toolpath

        # Check for changes in packages.config and remove installed tools if true.
        [string] $md5Hash = MD5HashFile($packageconf)
        if((!(Test-Path $packageconf_md5)) -Or
          ($md5Hash -ne (Get-Content $packageconf_md5 ))) {
            Write-Host "Missing or changed package.config hash..."
            Get-ChildItem -Exclude packages.config,nuget.exe,Cake.Bakery |
            Remove-Item -Recurse
        }

        Write-Host "Restoring tools from NuGet..."
        $NuGetOutput = Invoke-Expression "&`"$nugetpath`" install -ExcludeVersion -OutputDirectory `"$toolpath`""

        if ($LASTEXITCODE -ne 0) 
        {
            Throw "An error occurred while restoring NuGet tools."
        }
        else
        {
            $md5Hash | Out-File $packageconf_md5 -Encoding "ASCII"
        }
        Write-Verbose -Message ($NuGetOutput | out-string)

        Pop-Location
    }



}

if(!$PSScriptRoot){
    $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

#prepare teh tool paths
$tool_dir = Join-Path $PSScriptRoot "tools"
$nuget_exe = Join-Path $tool_dir "nuget.exe"
$cake_exe = Join-Path $tool_dir "Cake/cake.exe"

#restore the tools
RestoreTools $tool_dir $nuget_exe $cake_exe

# Make sure that Cake has been installed.
if (!(Test-Path $cake_exe)) {
    Throw "Could not find Cake.exe at $cake_exe"
}

# Run the cake build script
Write-Host "Running build script..."


# Build Cake arguments
$cakeArguments = @("$Script") + "--settings_skipverification=true";
if ($Target) { $cakeArguments += "-target=$Target" }
if ($Configuration) { $cakeArguments += "-configuration=$Configuration" }
if ($Verbosity) { $cakeArguments += "-verbosity=$Verbosity" }
if ($ShowDescription) { $cakeArguments += "-showdescription" }
if ($DryRun) { $cakeArguments += "-dryrun" }
if ($Experimental) { $cakeArguments += "-experimental" }
if ($Mono) { $cakeArguments += "-mono" }
$cakeArguments += $ScriptArgs

&$cake_exe $cakeArguments
exit $LASTEXITCODE;
