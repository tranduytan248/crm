# Local direct upload script for Demo FTP (fallback / direct execution)
[CmdletBinding()]
param(
    [string]$Server = $env:FTP_SERVER_DEMO,
    [int]$Port = 21,
    [string]$User = $env:FTP_USERNAME_DEMO,
    [string]$Password = $env:FTP_PASSWORD_DEMO,
    [string]$SourceDir = "publish_source"
)

if ([string]::IsNullOrWhiteSpace($Server)) { $Server = "10.57.30.10" }
if ([string]::IsNullOrWhiteSpace($User)) { $User = "quanlydoanhthucenit" }
if ([string]::IsNullOrWhiteSpace($Password)) { $Password = "fsJD37sH@23" }

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir
$localPublishDir = Join-Path $rootDir $SourceDir

if (-not (Test-Path $localPublishDir)) {
    throw "Directory $localPublishDir does not exist! Please run build_publish.ps1 first."
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

$files = Get-ChildItem -Path $localPublishDir -Recurse -File
$total = $files.Count
$current = 0

Write-Host "Starting upload of $total files..." -ForegroundColor Yellow

# Ensure directories first
$dirs = Get-ChildItem -Path $localPublishDir -Recurse -Directory | Sort-Object FullName
foreach ($d in $dirs) {
    $rel = $d.FullName.Substring($localPublishDir.Length).TrimStart('\', '/').Replace('\', '/')
    Ensure-FtpDirectory -remoteUri "$baseUri$rel/" -cred $cred
}

foreach ($f in $files) {
    $current++
    $rel = $f.FullName.Substring($localPublishDir.Length).TrimStart('\', '/').Replace('\', '/')
    $targetUri = "$baseUri$rel"
    Write-Progress -Activity "Uploading to FTP Demo" -Status "$current/$total: $rel" -PercentComplete (($current / $total) * 100)
    try {
        Upload-FtpFile -localFile $f.FullName -remoteUri $targetUri -cred $cred
    } catch {
        Write-Warning "Failed to upload $rel : $($_.Exception.Message)"
    }
}

Write-Host "`n[SUCCESS] Direct upload to FTP Demo completed!" -ForegroundColor Green
