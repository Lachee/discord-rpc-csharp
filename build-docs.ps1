#powershell -ExecutionPolicy ByPass -File
Write-Host "=========== Building Docs ===========" -ForegroundColor Green
docfx .\docfx_project\docfx.json -o docs\
