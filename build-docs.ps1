#powershell -ExecutionPolicy ByPass -File
Write-Host "=========== Building Docs ===========" -ForegroundColor Green

choco install docfx -y
docfx .\docfx_project\docfx.json -o docs\
