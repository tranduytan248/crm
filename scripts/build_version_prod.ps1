# ==============================================================================
# Script: build_version_prod.ps1
# Muc dich:
# 1. Tu dong bien dich WebApp o che do Release.
# 2. Ket noi FTP Production (lay thong tin tu FTP_SERVER_PROD, FTP_USERNAME_PROD, FTP_PASSWORD_PROD).
# 3. So sanh cac file trong ban build voi FTP Production:
#    - Loc ra file CHUA CO tren FTP Prod (NEW).
#    - Loc ra file CO VERSION CAP NHAT / THAY DOI tren FTP Prod (MODIFIED).
# 4. Sao chep cac file nay vao thu muc version/<ten version>/ (giu nguyen cau truc thu muc).
# 5. Xuat file manifest.json va manifest.txt tong ket.
# ==============================================================================

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0, HelpMessage = "Ten version (vi du: v1.0.0, 2026.09.08)")]
    [string]$VersionName,

    [string]$FtpServer = $env:FTP_SERVER_PROD,
    [int]$FtpPort = 21,
    [string]$FtpUser = $env:FTP_USERNAME_PROD,
    [string]$FtpPassword = $env:FTP_PASSWORD_PROD,
    [string]$FtpRemotePath = "/",

    [string]$Notes = "",
    [switch]$SkipBuild,
    [switch]$ForceAll
)

$ErrorActionPreference = "Stop"

# Chuan hoa ten version
$cleanVersion = $VersionName.Trim()
if ($cleanVersion.StartsWith("build ", [System.StringComparison]::OrdinalIgnoreCase)) {
    $cleanVersion = $cleanVersion.Substring(6).Trim()
}
$cleanVersion = $cleanVersion -replace '[\\/:*?"<>|]', '_'

# Fallback thong tin FTP neu chua cau hinh trong bien moi truong
if ([string]::IsNullOrWhiteSpace($FtpServer)) {
    $FtpServer = "10.57.47.3"
}
if ([string]::IsNullOrWhiteSpace($FtpUser)) {
    $FtpUser = "crm"
}
if ([string]::IsNullOrWhiteSpace($FtpPassword)) {
    $FtpPassword = "Kh@2026"
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir
$targetVersionDir = Join-Path $rootDir "version\$cleanVersion"

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "       TAO GOI PHAT HANH PRODUCTION: $cleanVersion       " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "Version Name       : $cleanVersion" -ForegroundColor Gray
Write-Host "Thu muc dich       : $targetVersionDir" -ForegroundColor Gray
Write-Host "FTP Prod Server    : $FtpServer`:$FtpPort" -ForegroundColor Gray
Write-Host "FTP User           : $FtpUser" -ForegroundColor Gray
Write-Host ""

# ------------------------------------------------------------------------------
# 1. BIEN DICH DU AN (NEU KHONG SKIP)
# ------------------------------------------------------------------------------
$webAppProj = Join-Path $rootDir "CenIT.Solution.TOC.WebApp\CenIT.Solution.TOC.WebApp.csproj"
$packageTmpDir = Join-Path $rootDir "CenIT.Solution.TOC.WebApp\obj\Release\Package\PackageTmp"
$publishSourceDir = Join-Path $rootDir "publish_source"

if (-not $SkipBuild) {
    Write-Host "[1/4] Dang tim kiem MSBuild va bien dich WebApp Release..." -ForegroundColor Yellow
    
    $msbuildCandidates = @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    )

    $msbuild = $null
    foreach ($path in $msbuildCandidates) {
        if (Test-Path $path) {
            $msbuild = $path
            break
        }
    }

    if (-not $msbuild) {
        $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
        if (Test-Path $vswhere) {
            $msbuild = & $vswhere -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
        }
    }

    if (-not $msbuild) {
        throw "Khong tim thay cong cu MSBuild tren may tinh nay!"
    }

    $slnPath = Join-Path $rootDir "CenIT.Solution.TOC.sln"
    Write-Host "   => Dang bien dich Solution: $slnPath" -ForegroundColor DarkGray
    & $msbuild $slnPath /p:Configuration=Release /v:m
    if ($LASTEXITCODE -ne 0) {
        throw "Qua trinh bien dich Solution bi loi (Exit code: $LASTEXITCODE)."
    }

    Write-Host "   => Dang dong goi Package WebApp..." -ForegroundColor DarkGray
    & $msbuild $webAppProj /target:Package /p:Configuration=Release /p:SolutionDir="$rootDir\" /v:m
    if ($LASTEXITCODE -ne 0) {
        throw "Qua trinh dong goi WebApp bi loi (Exit code: $LASTEXITCODE)."
    }
    Write-Host "   [OK] Bien dich va dong goi hoan tat!" -ForegroundColor Green
} else {
    Write-Host "[1/4] Bo qua buoc bien dich (-SkipBuild)." -ForegroundColor Yellow
}

# Xac dinh thu muc nguon ban build
$sourceDir = $null
if (Test-Path $packageTmpDir) {
    $sourceDir = $packageTmpDir
} elseif (Test-Path $publishSourceDir) {
    $sourceDir = $publishSourceDir
} else {
    throw "Khong tim thay thu muc ban build ($packageTmpDir hoac $publishSourceDir). Hay chay khong co co -SkipBuild."
}
Write-Host "Thu muc nguon build: $sourceDir" -ForegroundColor DarkGray
Write-Host ""

# ------------------------------------------------------------------------------
# 2. QUET DANH SACH FILE TRONG BAN BUILD (LOCAL)
# ------------------------------------------------------------------------------
Write-Host "[2/4] Dang quet danh sach file trong ban build..." -ForegroundColor Yellow
$localFiles = Get-ChildItem -Path $sourceDir -Recurse -File
$localMap = @{}

foreach ($f in $localFiles) {
    $rel = $f.FullName.Substring($sourceDir.Length).TrimStart('\', '/').Replace('\', '/')
    $localMap[$rel.ToLower()] = @{
        RelativePath = $rel
        FullName = $f.FullName
        Length = $f.Length
        LastWriteTimeUtc = $f.LastWriteTimeUtc
    }
}
Write-Host "   => Tim thay $($localFiles.Count) files trong ban build." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------------------------
# 3. KET NOI VA QUET DANH SACH FILE TREN FTP PRODUCTION
# ------------------------------------------------------------------------------
Write-Host "[3/4] Dang ket noi va quet danh sach file tren FTP Production..." -ForegroundColor Yellow

$ftpFiles = @{}
$ftpConnected = $false

function Test-FtpConnection {
    param([string]$Server, [int]$Port)
    try {
        $tcp = New-Object System.Net.Sockets.TcpClient
        $iar = $tcp.BeginConnect($Server, $Port, $null, $null)
        $wait = $iar.AsyncWaitHandle.WaitOne(3000, $false)
        if (-not $wait) {
            $tcp.Close()
            return $false
        }
        $tcp.EndConnect($iar)
        $tcp.Close()
        return $true
    } catch {
        return $false
    }
}

function Get-FtpDirectoryListing {
    param(
        [string]$Server,
        [int]$Port,
        [string]$SubPath,
        [System.Net.NetworkCredential]$Cred,
        [hashtable]$FtpMap
    )

    $normalizedSub = $SubPath.Trim('/')
    if ($normalizedSub) {
        $targetUri = "ftp://$Server`:$Port/$normalizedSub/"
    } else {
        $targetUri = "ftp://$Server`:$Port/"
    }

    try {
        $req = [System.Net.FtpWebRequest]::Create($targetUri)
        $req.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectoryDetails
        $req.Credentials = $Cred
        $req.UseBinary = $true
        $req.UsePassive = $true
        $req.KeepAlive = $false
        $req.Timeout = 8000

        $res = $req.GetResponse()
        $reader = New-Object System.IO.StreamReader($res.GetResponseStream())
        $listing = $reader.ReadToEnd()
        $reader.Close()
        $res.Close()

        $lines = $listing -split "`r`n|`r|`n"
        foreach ($line in $lines) {
            if ([string]::IsNullOrWhiteSpace($line)) { continue }
            
            $isDir = $false
            $name = $null
            $size = 0

            # Unix style check
            if ($line.StartsWith("d") -or $line.StartsWith("-")) {
                $parts = -split $line
                if ($parts.Length -ge 9) {
                    $isDir = $line.StartsWith("d")
                    [int64]::TryParse($parts[4], [ref]$size) | Out-Null
                    $name = ($parts[8..($parts.Length - 1)]) -join " "
                }
            }
            # Windows/DOS style check
            elseif ($line -match "<\s*DIR\s*>") {
                $isDir = $true
                $idx = $line.IndexOf(">")
                if ($idx -gt 0) {
                    $name = $line.Substring($idx + 1).Trim()
                }
            } else {
                # Windows file: date time size name
                $parts = -split $line
                if ($parts.Length -ge 4) {
                    [int64]::TryParse($parts[2], [ref]$size) | Out-Null
                    $name = ($parts[3..($parts.Length - 1)]) -join " "
                }
            }

            if (-not $name -or $name -eq "." -or $name -eq "..") { continue }

            $itemRelPath = if ($normalizedSub) { "$normalizedSub/$name" } else { $name }

            if ($isDir) {
                Get-FtpDirectoryListing -Server $Server -Port $Port -SubPath $itemRelPath -Cred $Cred -FtpMap $FtpMap
            } else {
                $FtpMap[$itemRelPath.ToLower()] = @{
                    RelativePath = $itemRelPath
                    Size = $size
                }
            }
        }
    } catch {
        Write-Verbose "Khong the duyet thu muc FTP: $targetUri"
    }
}

$canReachFtp = Test-FtpConnection -Server $FtpServer -Port $FtpPort
$cred = New-Object System.Net.NetworkCredential($FtpUser, $FtpPassword)

if ($canReachFtp) {
    Write-Host "   => Ket noi FTP Production thanh cong. Dang quet de quy cac tep..." -ForegroundColor Green
    try {
        Get-FtpDirectoryListing -Server $FtpServer -Port $FtpPort -SubPath "" -Cred $cred -FtpMap $ftpFiles
        $ftpConnected = $true
        Write-Host "   => Da quet duoc $($ftpFiles.Count) files tren FTP Production." -ForegroundColor Green
    } catch {
        Write-Warning "Quet FTP gap loi: $($_.Exception.Message)"
    }
} else {
    Write-Warning "Tuong lua / Mang may tram hien tai khong mo cong ket noi truc tiep toi FTP Production ($FtpServer`:$FtpPort)."
    Write-Warning "He thong se chuyen sang che do tao goi version phat hanh tu toan bo ban build."
}

# ------------------------------------------------------------------------------
# 4. SO SANH (DIFF) FILE DE LOC FILE MOI VA FILE CAP NHAT
# ------------------------------------------------------------------------------
Write-Host ""
Write-Host "[4/4] So sanh va loc file dua vao thu muc version/$cleanVersion..." -ForegroundColor Yellow

$itemsToCopy = @()
$newFilesList = @()
$modifiedFilesList = @()

foreach ($key in $localMap.Keys) {
    $loc = $localMap[$key]
    $rel = $loc.RelativePath

    if ($ForceAll -or (-not $ftpConnected -and $ftpFiles.Count -eq 0)) {
        $status = "NEW"
        $reason = if ($ForceAll) { "ForceAll flag" } else { "Chua co tren FTP hoac chay che do phat hanh day du" }
        $itemsToCopy += [PSCustomObject]@{
            Path = $rel
            Status = $status
            Size = $loc.Length
            Reason = $reason
            LocalFullPath = $loc.FullName
        }
        $newFilesList += $rel
        continue
    }

    if (-not $ftpFiles.ContainsKey($key)) {
        $itemsToCopy += [PSCustomObject]@{
            Path = $rel
            Status = "NEW"
            Size = $loc.Length
            Reason = "Tep chua co tren FTP Production"
            LocalFullPath = $loc.FullName
        }
        $newFilesList += $rel
    } else {
        $remote = $ftpFiles[$key]
        $isDifferent = $false
        $diffReason = ""

        if ($loc.Length -ne $remote.Size) {
            $isDifferent = $true
            $diffReason = "Kich thuoc khac nhau (Build: $($loc.Length) bytes vs FTP: $($remote.Size) bytes)"
        }

        if ($isDifferent) {
            $itemsToCopy += [PSCustomObject]@{
                Path = $rel
                Status = "MODIFIED"
                Size = $loc.Length
                Reason = $diffReason
                LocalFullPath = $loc.FullName
            }
            $modifiedFilesList += $rel
        }
    }
}

# Tao thu muc version/<ten version>
if (-not (Test-Path $targetVersionDir)) {
    New-Item -ItemType Directory -Path $targetVersionDir -Force | Out-Null
}

# Sao chep cac file duoc loc vao thu muc version
$copiedCount = 0
foreach ($item in $itemsToCopy) {
    $destFile = Join-Path $targetVersionDir ($item.Path.Replace('/', '\'))
    $destSubDir = Split-Path -Parent $destFile
    if (-not (Test-Path $destSubDir)) {
        New-Item -ItemType Directory -Path $destSubDir -Force | Out-Null
    }
    Copy-Item -Path $item.LocalFullPath -Destination $destFile -Force
    $copiedCount++
}

# ------------------------------------------------------------------------------
# 5. XUAT MANIFEST VA BAO CAO
# ------------------------------------------------------------------------------
$manifest = [PSCustomObject]@{
    version = $cleanVersion
    created_at = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    ftp_server = $FtpServer
    ftp_connected = $ftpConnected
    total_build_files = $localMap.Count
    total_ftp_files = $ftpFiles.Count
    new_files_count = $newFilesList.Count
    modified_files_count = $modifiedFilesList.Count
    total_version_files = $itemsToCopy.Count
    files = $itemsToCopy | Select-Object Path, Status, Size, Reason
}

$manifestJsonPath = Join-Path $targetVersionDir "manifest.json"
$manifestTxtPath = Join-Path $targetVersionDir "manifest.txt"

$manifest | ConvertTo-Json -Depth 5 | Set-Content -Path $manifestJsonPath -Encoding UTF8

$ftpStatusText = if ($ftpConnected) { "KET NOI THANH CONG" } else { "KHONG KET NOI (Chan tuong lua / Offline)" }

$txtLines = @()
$txtLines += "================================================================================"
$txtLines += "               BAO CAO TONG KET GOI PHAT HANH: $cleanVersion"
$txtLines += "================================================================================"
$txtLines += "Thoi gian tao           : $((Get-Date).ToString('dd/MM/yyyy HH:mm:ss'))"
$txtLines += "May chu FTP Production  : $FtpServer`:$FtpPort"
$txtLines += "Trang thai ket noi FTP  : $ftpStatusText"
$txtLines += "Tong so file ban build  : $($localMap.Count)"
$txtLines += "Tong so file tren FTP   : $($ftpFiles.Count)"
$txtLines += "--------------------------------------------------------------------------------"
$txtLines += "THONG KE PHAN LOAI VERSION"
$txtLines += "--------------------------------------------------------------------------------"
$txtLines += "1. File moi (NEW)       : $($newFilesList.Count) tep"
$txtLines += "2. File cap nhat (MOD)  : $($modifiedFilesList.Count) tep"
$txtLines += "=> TONG CONG DA COPY    : $($itemsToCopy.Count) tep"
$txtLines += "--------------------------------------------------------------------------------"
$txtLines += "DANH SACH CHI TIET TEP DA DUA VAO VERSION:"
foreach ($f in $itemsToCopy) {
    $txtLines += "[$($f.Status)] $($f.Path) ($($f.Size) bytes) - $($f.Reason)"
}
$txtLines | Set-Content -Path $manifestTxtPath -Encoding UTF8

# ------------------------------------------------------------------------------
# 5b. XUAT FILE GHI CHU CAP NHAT (UPDATE_NOTES.md)
# ------------------------------------------------------------------------------
$updateNotesPath = Join-Path $targetVersionDir "UPDATE_NOTES.md"

$gitLogLines = @()
try {
    $rawLog = & git log -n 5 --pretty=format:"* **%h** (%ad): %s" --date=format:"%d/%m/%Y" 2>$null
    if ($rawLog) {
        $gitLogLines = $rawLog
    }
} catch {
    # Bo qua neu loi git
}

$nowFormatted = (Get-Date).ToString("dd/MM/yyyy HH:mm:ss")
$nowHeader = (Get-Date).ToString("HH:mm:ss dd/MM/yyyy")

$mdLines = @()
$mdLines += "# GHI CHU PHAT HANH - PHIEN BAN $cleanVersion"
$mdLines += ""
$mdLines += "> Goi cap nhat duoc tao tu dong vao luc **$nowHeader**"
$mdLines += ""
$mdLines += "## Thong Tin Tong Quan"
$mdLines += "- **Phien ban (Version)**: $cleanVersion"
$mdLines += "- **Thoi gian tao**: $nowFormatted"
$mdLines += "- **Nguoi thuc hien**: $env:USERNAME"
$mdLines += "- **May chu dich (FTP Prod)**: $FtpServer`:$FtpPort"
$mdLines += "- **Trang thai ket noi FTP**: $ftpStatusText"
$mdLines += "- **Tong so tep trong ban build**: $($localMap.Count)"
$mdLines += "- **So tep cap nhat (MODIFIED)**: **$($modifiedFilesList.Count)** tep"
$mdLines += "- **So tep moi (NEW)**: **$($newFilesList.Count)** tep"
$mdLines += "- **Tong tep dua vao goi**: **$($itemsToCopy.Count)** tep"
$mdLines += ""

if (-not [string]::IsNullOrWhiteSpace($Notes)) {
    $mdLines += "## Noi Dung Thay Doi / Ghi Chu Tinh Nang"
    $mdLines += "$Notes"
    $mdLines += ""
}

if ($gitLogLines.Count -gt 0) {
    $mdLines += "## Lich Su Cam Ket Gan Nhat (Git Commits)"
    foreach ($gl in $gitLogLines) {
        $mdLines += $gl
    }
    $mdLines += ""
}

$mdLines += "## Danh Sach Tep Duoc Cap Nhat (MODIFIED FILES)"
$modItems = @($itemsToCopy | Where-Object { $_.Status -eq "MODIFIED" })
if ($modItems.Count -gt 0) {
    $mdLines += "| STT | Duong Dan Tep | Kich Thuoc | Ly Do Cap Nhat |"
    $mdLines += "| :---: | :--- | :--- | :--- |"
    $idx = 1
    foreach ($m in $modItems) {
        $sizeKb = [math]::Round($m.Size / 1KB, 2)
        $mdLines += "| $idx | ``$($m.Path)`` | $sizeKb KB | $($m.Reason) |"
        $idx++
    }
} else {
    $mdLines += "_Khong co tep nao bi sua doi so voi FTP Prod (toan bo la tep moi hoac giu nguyen)._"
}
$mdLines += ""

$mdLines += "## Danh Sach Tep Them Moi (NEW FILES)"
$newItems = @($itemsToCopy | Where-Object { $_.Status -eq "NEW" })
if ($newItems.Count -gt 0) {
    $binFiles = @($newItems | Where-Object { $_.Path -like "bin/*" -or $_.Path -like "Libraries/*" })
    $viewFiles = @($newItems | Where-Object { $_.Path -like "Views/*" -or $_.Path -like "Areas/*" })
    $configFiles = @($newItems | Where-Object { $_.Path -like "Configs/*" -or $_.Path -eq "Web.config" -or $_.Path -eq "Global.asax" })
    $contentFiles = @($newItems | Where-Object { $_.Path -like "Contents/*" })
    $otherFiles = @($newItems | Where-Object { $_.Path -notlike "bin/*" -and $_.Path -notlike "Libraries/*" -and $_.Path -notlike "Views/*" -and $_.Path -notlike "Areas/*" -and $_.Path -notlike "Configs/*" -and $_.Path -ne "Web.config" -and $_.Path -ne "Global.asax" -and $_.Path -notlike "Contents/*" })

    if ($binFiles.Count -gt 0) {
        $mdLines += "### Thu Vien & Ma Thuc Thi (Assemblies / DLLs: $($binFiles.Count) tep)"
        foreach ($bf in $binFiles) {
            $mdLines += "- ``$($bf.Path)`` ($([math]::Round($bf.Size / 1KB, 2)) KB)"
        }
        $mdLines += ""
    }

    if ($viewFiles.Count -gt 0) {
        $mdLines += "### Giao Dien (Views / Razor Pages: $($viewFiles.Count) tep)"
        foreach ($vf in $viewFiles) {
            $mdLines += "- ``$($vf.Path)``"
        }
        $mdLines += ""
    }

    if ($configFiles.Count -gt 0) {
        $mdLines += "### Cau Hinh (Configs: $($configFiles.Count) tep)"
        foreach ($cf in $configFiles) {
            $mdLines += "- ``$($cf.Path)``"
        }
        $mdLines += ""
    }

    if ($contentFiles.Count -gt 0) {
        $mdLines += "### Tai Nguyen Tinh (Contents / Assets: $($contentFiles.Count) tep)"
        $mdLines += "- _Gom $($contentFiles.Count) tep hinh anh, CSS, JS, plugin._"
        $mdLines += ""
    }

    if ($otherFiles.Count -gt 0) {
        $mdLines += "### Tep Khac ($($otherFiles.Count) tep)"
        foreach ($of in $otherFiles) {
            $mdLines += "- ``$($of.Path)``"
        }
        $mdLines += ""
    }
} else {
    $mdLines += "_Khong co tep moi._"
}
$mdLines += ""

$mdLines += "## Huong Dan Trien Khai (Deployment Guide)"
$mdLines += "1. **Sao luu**: Sao luu ma nguon hien tai tren Production truoc khi cap nhat."
$mdLines += "2. **Ghi de**: Tai toan bo noi dung trong thu muc nay len thu muc goc cua WebApp tren Production."
$mdLines += "3. **Tai khoi dong IIS**: Neu co cap nhat tep trong thu muc ``bin/`` hoac tep ``Web.config``, hay Recycle App Pool tren IIS de nap dll moi."

$mdLines | Set-Content -Path $updateNotesPath -Encoding UTF8

Write-Host ""
Write-Host "=======================================================" -ForegroundColor Green
Write-Host " [HOAN TAT] Goi version '$cleanVersion' da duoc tao! " -ForegroundColor Green
Write-Host "=======================================================" -ForegroundColor Green
Write-Host "Thu muc dich         : $targetVersionDir" -ForegroundColor Cyan
Write-Host "Tep moi (NEW)        : $($newFilesList.Count) tep" -ForegroundColor Yellow
Write-Host "Tep cap nhat (MOD)   : $($modifiedFilesList.Count) tep" -ForegroundColor Yellow
Write-Host "Tong tep version     : $($itemsToCopy.Count) tep" -ForegroundColor Green
Write-Host "Manifest JSON        : $manifestJsonPath" -ForegroundColor DarkGray
Write-Host "Manifest TXT         : $manifestTxtPath" -ForegroundColor DarkGray
Write-Host "Update Notes MD      : $updateNotesPath" -ForegroundColor DarkGray
Write-Host ""
