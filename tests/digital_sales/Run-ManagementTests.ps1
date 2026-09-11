$ErrorActionPreference = 'Stop'

$crmRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$crmBin = Join-Path $crmRoot 'Modules.Cate\bin'
if (-not (Test-Path (Join-Path $crmBin 'Core.Cate.dll'))) {
    $crmBin = Join-Path $crmRoot 'publish_source\bin'
}
$csc = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
if (-not (Test-Path $csc)) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $csc = & $vswhere -latest -products '*' -find 'MSBuild\Current\Bin\Roslyn\csc.exe' | Select-Object -First 1
    }
}

if (-not $csc -or -not (Test-Path $csc)) {
    throw 'Visual Studio Roslyn compiler (csc.exe) was not found.'
}

$connStr = "Data Source=10.57.30.10;Initial Catalog=quanlydoanhthucenit;Persist Security Info=True;User Id=quanlydoanhthucenit;Password=Kdhe@543HE2;Connect Timeout=30;"

$testDir = Join-Path ([IO.Path]::GetTempPath()) ('crm-mgmt-tests-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($testDir) | Out-Null
$testExe = Join-Path $testDir 'DigitalSalesManagementTests.exe'

$refs = @('Core.Cate.dll', 'Modules.Cate.dll', 'TSFramework.Libs.dll') |
    ForEach-Object { '/reference:' + (Join-Path $crmBin $_) }

Write-Host "Biên dịch bài test DigitalSalesManagementTests..." -ForegroundColor Cyan
& $csc /nologo /codepage:65001 /utf8output /target:exe "/out:$testExe" /reference:System.dll /reference:System.Data.dll /reference:System.Xml.dll /reference:System.Core.dll $refs (Join-Path $PSScriptRoot 'DigitalSalesManagementTests.cs')

if ($LASTEXITCODE -ne 0) {
    throw 'Test compilation failed.'
}

Write-Host "Khởi chạy bài test..." -ForegroundColor Cyan
& $testExe $crmBin $connStr

$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    throw "Management tests failed with exit code $exitCode"
}

Remove-Item -Recurse -Force $testDir -ErrorAction SilentlyContinue
Write-Host "HOÀN TẤT KIỂM THỬ: 100% PASS!" -ForegroundColor Green
