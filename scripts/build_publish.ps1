# Build and publish script for CenIT CRM Solution
[CmdletBinding()]
param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   CRM Build & Publish to publish_source  " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# 1. Locate MSBuild
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = ""
if (Test-Path $vswhere) {
    $msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
}

if (-not $msbuild -or -not (Test-Path $msbuild)) {
    $defaultPaths = @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    )
    foreach ($p in $defaultPaths) {
        if (Test-Path $p) {
            $msbuild = $p
            break
        }
    }
}

if (-not $msbuild) {
    throw "MSBuild.exe not found! Please check Visual Studio installation."
}

Write-Host "Using MSBuild: $msbuild" -ForegroundColor Green

# 2. Build Solution
Write-Host "`n[1/3] Building Solution ($Configuration)..." -ForegroundColor Yellow
$slnPath = Join-Path $rootDir "CenIT.Solution.TOC.sln"
& $msbuild $slnPath /p:Configuration=$Configuration /v:m
if ($LASTEXITCODE -ne 0) {
    throw "Failed to build solution $slnPath (Exit code: $LASTEXITCODE)"
}

# 3. Create Web Package
Write-Host "`n[2/3] Packaging CenIT.Solution.TOC.WebApp..." -ForegroundColor Yellow
$webAppCsproj = Join-Path $rootDir "CenIT.Solution.TOC.WebApp\CenIT.Solution.TOC.WebApp.csproj"
& $msbuild $webAppCsproj /target:Package /p:Configuration=$Configuration /p:SolutionDir="$rootDir\" /v:m
if ($LASTEXITCODE -ne 0) {
    throw "Failed to package WebApp (Exit code: $LASTEXITCODE)"
}

# 4. Copy to publish_source
Write-Host "`n[3/3] Synchronizing package to publish_source..." -ForegroundColor Yellow
$packageTmpDir = Join-Path $rootDir "CenIT.Solution.TOC.WebApp\obj\$Configuration\Package\PackageTmp"
$publishSourceDir = Join-Path $rootDir "publish_source"

if (-not (Test-Path $publishSourceDir)) {
    New-Item -ItemType Directory -Path $publishSourceDir -Force | Out-Null
}

robocopy $packageTmpDir $publishSourceDir /MIR /NJH /NJS /NDL /NC /NS
$roboExit = $LASTEXITCODE
if ($roboExit -ge 8) {
    throw "Robocopy failed with exit code: $roboExit"
}

$itemCount = (Get-ChildItem $publishSourceDir).Count
Write-Host "`n[SUCCESS] Successfully published $itemCount items to $publishSourceDir!" -ForegroundColor Green
