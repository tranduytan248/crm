# Direct upload to Demo FTP from a workstation inside the VNPT network.
[CmdletBinding()]
param(
    [string]$Server = $(if ($env:FTP_SERVER_DEMO) { $env:FTP_SERVER_DEMO } else { "10.57.30.10" }),
    [int]$Port = 21,
    [string]$User = $(if ($env:FTP_USERNAME_DEMO) { $env:FTP_USERNAME_DEMO } else { "quanlydoanhthucenit" }),
    [string]$Password = $env:FTP_PASSWORD_DEMO,
    [string]$SourceDir = "publish_source",
    [string]$CredentialPath = ".secrets/ftp-demo.credential.xml"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir
$localPublishDir = Join-Path $rootDir $SourceDir
$localCredentialPath = Join-Path $rootDir $CredentialPath

if (-not (Test-Path $localPublishDir)) {
    throw "Directory $localPublishDir does not exist! Please run build_publish.ps1 first."
}

$mainDll = Join-Path $localPublishDir "bin/CenIT.Solution.TOC.WebApp.dll"
if (-not (Test-Path -LiteralPath $mainDll -PathType Leaf)) {
    throw "Published application DLL is missing: $mainDll"
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    if (-not (Test-Path -LiteralPath $localCredentialPath -PathType Leaf)) {
        throw "FTP credential is missing. Set FTP_PASSWORD_DEMO or create $localCredentialPath with Export-Clixml."
    }

    $storedCredential = Import-Clixml -LiteralPath $localCredentialPath
    $User = $storedCredential.UserName
    $Password = $storedCredential.GetNetworkCredential().Password
}

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   Direct Upload to Demo FTP ($Server)   " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

function Ensure-FtpDirectory {
    param([string]$remoteUri, [System.Net.NetworkCredential]$cred)
    try {
        $req = [System.Net.FtpWebRequest]::Create($remoteUri)
        $req.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
        $req.Credentials = $cred
        $req.UsePassive = $true
        $req.GetResponse().Close()
    } catch {
        # Directory might already exist
    }
}

function Upload-FtpFile {
    param([string]$localFile, [string]$remoteUri, [System.Net.NetworkCredential]$cred)
    $req = [System.Net.FtpWebRequest]::Create($remoteUri)
    $req.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
    $req.Credentials = $cred
    $req.UseBinary = $true
    $req.UsePassive = $true
    
    $fileBytes = [System.IO.File]::ReadAllBytes($localFile)
    $req.ContentLength = $fileBytes.Length
    $stream = $req.GetRequestStream()
    $stream.Write($fileBytes, 0, $fileBytes.Length)
    $stream.Close()
    $req.GetResponse().Close()
}

$cred = New-Object System.Net.NetworkCredential($User, $Password)
$baseUri = "ftp://$Server`:$Port/"

function Get-RelativeFtpPath {
    param([string]$FullName)
    return $FullName.Substring($localPublishDir.Length).TrimStart('\', '/').Replace('\', '/')
}

$excludedRelativePaths = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
[void]$excludedRelativePaths.Add("Web.config")
[void]$excludedRelativePaths.Add("Configs/AppSettings.config")

$allFiles = @(Get-ChildItem -Path $localPublishDir -Recurse -File)
$files = @($allFiles | Where-Object {
    -not $excludedRelativePaths.Contains((Get-RelativeFtpPath -FullName $_.FullName))
})
$skippedFiles = @($allFiles | Where-Object {
    $excludedRelativePaths.Contains((Get-RelativeFtpPath -FullName $_.FullName))
})
$total = $files.Count
$current = 0
$uploaded = 0
$failed = [System.Collections.Generic.List[string]]::new()

Write-Host "Starting upload of $total files; skipping $($skippedFiles.Count) environment config file(s)..." -ForegroundColor Yellow
foreach ($skippedFile in $skippedFiles) {
    Write-Host "[SKIP] $(Get-RelativeFtpPath -FullName $skippedFile.FullName)" -ForegroundColor DarkYellow
}

# Ensure directories first
$dirs = Get-ChildItem -Path $localPublishDir -Recurse -Directory | Sort-Object FullName
foreach ($d in $dirs) {
    $rel = Get-RelativeFtpPath -FullName $d.FullName
    Ensure-FtpDirectory -remoteUri "$baseUri$rel/" -cred $cred
}

foreach ($f in $files) {
    $current++
    $rel = Get-RelativeFtpPath -FullName $f.FullName
    $targetUri = "$baseUri$rel"
    Write-Progress -Activity "Uploading to FTP Demo" -Status "$current/${total}: $rel" -PercentComplete (($current / $total) * 100)
    try {
        Upload-FtpFile -localFile $f.FullName -remoteUri $targetUri -cred $cred
        $uploaded++
    } catch {
        Write-Warning "Failed to upload $rel : $($_.Exception.Message)"
        $failed.Add($rel)
    }
}

Write-Progress -Activity "Uploading to FTP Demo" -Completed
if ($failed.Count -gt 0) {
    Write-Error "FTP upload failed: $($failed.Count)/$total file(s) failed; $uploaded uploaded successfully."
    exit 1
}

Write-Host "`n[SUCCESS] Uploaded $uploaded/$total files to FTP Demo; skipped $($skippedFiles.Count) environment config file(s)." -ForegroundColor Green
exit 0
