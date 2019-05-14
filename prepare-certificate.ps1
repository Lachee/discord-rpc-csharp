[CmdletBinding()]
Param(
    [string]$FileName='certificate.pfx',
    [switch]$Encode
)

if ($Encode)
{
    $path = Resolve-Path $FileName
    $Base64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes($path))
    [IO.File]::WriteAllText("$path" + ".txt", $Base64);
    Write-Host ('Certificate Encoded');
}
else
{
    Add-Content $FileName "TMP"
    $path = Resolve-Path $FileName
    [IO.File]::WriteAllBytes($path, [Convert]::FromBase64String($env:CERTIFICATE))
    Write-Host ("Certificate Decoded");
}

