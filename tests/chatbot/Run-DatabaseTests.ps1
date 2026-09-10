$ErrorActionPreference = 'Stop'
$crmRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$webRoot = Join-Path $crmRoot 'CenIT.Solution.TOC.WebApp'
$crmBin = Join-Path $crmRoot 'Modules.Cate\bin'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$csc = & $vswhere -latest -products '*' -find 'MSBuild\Current\Bin\Roslyn\csc.exe' | Select-Object -First 1
if (-not $csc) { throw 'Roslyn compiler not found.' }
$testDir = Join-Path ([IO.Path]::GetTempPath()) ('crm-db-tests-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($testDir) | Out-Null
$testExe = Join-Path $testDir 'DatabaseTests.exe'
$procDir = Join-Path $webRoot 'Libraries\Procedures'
$authDir = Join-Path $webRoot 'Libraries\Authorities'
$refs = @('Modules.Cate.dll','Core.Cate.dll','TSFramework.Libs.dll','Newtonsoft.Json.dll','System.Web.Http.dll','TiSun.dll') | ForEach-Object { '/reference:' + (Join-Path $crmBin $_) }
$refs += '/reference:' + (Join-Path $procDir 'Plugable.SQLProcedureProcessor.dll')
$refs += '/reference:' + (Join-Path $authDir 'Plugable.SQLProcedureAuthority.dll')
& $csc /nologo /target:exe "/out:$testExe" /reference:System.Web.dll /reference:System.Net.Http.dll /reference:System.Configuration.dll /reference:System.Data.dll $refs (Join-Path $PSScriptRoot 'DatabaseTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Database test compilation failed.' }
# Copy only configuration sections needed for the real SQL provider. Never print credentials.
[xml]$web = Get-Content (Join-Path $webRoot 'Web.config') -Raw
$config = New-Object Xml.XmlDocument
$root = $config.CreateElement('configuration'); [void]$config.AppendChild($root)
foreach ($name in @('configSections','connectionStrings','TiSunService','runtime')) {
    $node = $web.configuration.SelectSingleNode($name)
    if ($node) { [void]$root.AppendChild($config.ImportNode($node, $true)) }
}
$configPath = $testExe + '.config'
try {
    $config.Save($configPath)
    & $testExe $webRoot $crmBin $procDir $authDir
    if ($LASTEXITCODE -ne 0) { throw 'Database checks failed; see sanitized diagnostics above.' }
} finally {
    # Remove only the exact temporary configuration containing connection secrets.
    if (Test-Path -LiteralPath $configPath) { Remove-Item -LiteralPath $configPath -Force }
}
