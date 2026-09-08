# ==============================================================================
# Script: build_version_prod.ps1
# Muc dich:
# 1. Bien dich WebApp Release va dong goi sang thu muc publish_source (neu khong -SkipBuild).
# 2. So sanh toan bo ma nguon trong publish_source vs Source_Prod:
#    - Loc ra file CHUA CO trong Source_Prod (NEW).
#    - Loc ra file CO CAP NHAT / THAY DOI so voi Source_Prod (MODIFIED: khac size hoac hash).
# 3. Sao chep cac file NEW va MODIFIED vao thu muc version/<ten version>/ (giu nguyen cau truc thu muc).
# 4. Xuat file UPDATE_NOTES.md, manifest.txt, manifest.json tong ket chi tiet.
# ==============================================================================

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0, HelpMessage = "Ten version (vi du: v1.0.0, 2026.09.08)")]
    [string]$VersionName,

    [string]$SourcePublishDir = "publish_source",
    [string]$SourceProdDir = "Source_Prod",
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

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir

$publishPath = Join-Path $rootDir $SourcePublishDir
$prodPath = Join-Path $rootDir $SourceProdDir
$targetVersionDir = Join-Path $rootDir "version\$cleanVersion"

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "    TAO GOI VERSION PRODUCTION: $cleanVersion          " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "• Version Name       : $cleanVersion" -ForegroundColor Gray
Write-Host "• Nguon moi (Build)  : $publishPath" -ForegroundColor Gray
Write-Host "• Nguon chuan (Prod) : $prodPath" -ForegroundColor Gray
Write-Host "• Thu muc dich       : $targetVersionDir" -ForegroundColor Gray
Write-Host ""

# ------------------------------------------------------------------------------
# 1. BIEN DICH VA XUAT BAN SANG publish_source (NEU KHONG -SkipBuild)
# ------------------------------------------------------------------------------
if (-not $SkipBuild) {
    Write-Host "[1/4] Dang tim kiem MSBuild va bien dich Solution..." -ForegroundColor Yellow

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

    Write-Host "   => MSBuild: $msbuild" -ForegroundColor DarkGray

    $slnPath = Join-Path $rootDir "CenIT.Solution.TOC.sln"
    Write-Host "   => Bien dich Solution Release..." -ForegroundColor DarkGray
    & $msbuild $slnPath /p:Configuration=Release /v:m
    if ($LASTEXITCODE -ne 0) {
        throw "Qua trinh bien dich Solution bi loi (Exit code: $LASTEXITCODE)."
    }

    $webAppProj = Join-Path $rootDir "CenIT.Solution.TOC.WebApp\CenIT.Solution.TOC.WebApp.csproj"
    Write-Host "   => Dong goi Package WebApp..." -ForegroundColor DarkGray
    & $msbuild $webAppProj /target:Package /p:Configuration=Release /p:SolutionDir="$rootDir\" /v:m
    if ($LASTEXITCODE -ne 0) {
        throw "Qua trinh dong goi WebApp bi loi (Exit code: $LASTEXITCODE)."
    }

    # Dong bo vao publish_source
    $packageTmpDir = Join-Path $rootDir "CenIT.Solution.TOC.WebApp\obj\Release\Package\PackageTmp"
    if (-not (Test-Path $publishPath)) {
        New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
    }
    Write-Host "   => Dong bo Package sang $SourcePublishDir..." -ForegroundColor DarkGray
    robocopy $packageTmpDir $publishPath /MIR /NJH /NJS /NDL /NC /NS | Out-Null
    Write-Host "   [OK] Bien dich va cap nhat $SourcePublishDir thanh cong!" -ForegroundColor Green
} else {
    Write-Host "[1/4] Bo qua buoc bien dich (-SkipBuild)." -ForegroundColor Yellow
}

if (-not (Test-Path $publishPath)) {
    throw "Thu muc $publishPath khong ton tai! Hay chay lai ma khong dung co -SkipBuild."
}

# ------------------------------------------------------------------------------
# 2. QUET DANH SACH FILE TRONG publish_source VA Source_Prod
# ------------------------------------------------------------------------------
Write-Host ""
Write-Host "[2/4] Dang quet danh sach tep trong publish_source..." -ForegroundColor Yellow
$pubFiles = Get-ChildItem -Path $publishPath -Recurse -File
$pubMap = @{}

foreach ($f in $pubFiles) {
    $rel = $f.FullName.Substring($publishPath.Length).TrimStart('\', '/').Replace('\', '/')
    $pubMap[$rel.ToLower()] = @{
        RelativePath = $rel
        FullName = $f.FullName
        Length = $f.Length
        LastWriteTimeUtc = $f.LastWriteTimeUtc
    }
}
Write-Host "   => publish_source co $($pubFiles.Count) files." -ForegroundColor Green

Write-Host "Dang quet danh sach tep trong Source_Prod..." -ForegroundColor Yellow
$prodMap = @{}
$hasProdFiles = $false

if (Test-Path $prodPath) {
    $prodFiles = Get-ChildItem -Path $prodPath -Recurse -File | Where-Object { $_.Name -ne "readme.txt" }
    foreach ($f in $prodFiles) {
        $rel = $f.FullName.Substring($prodPath.Length).TrimStart('\', '/').Replace('\', '/')
        $prodMap[$rel.ToLower()] = @{
            RelativePath = $rel
            FullName = $f.FullName
            Length = $f.Length
            LastWriteTimeUtc = $f.LastWriteTimeUtc
        }
    }
    if ($prodMap.Count -gt 0) {
        $hasProdFiles = $true
        Write-Host "   => Source_Prod co $($prodMap.Count) files thuc te." -ForegroundColor Green
    } else {
        Write-Warning "Thu muc Source_Prod hien tai chua co file ma nguon (hoac chi co readme.txt)."
        Write-Warning "Tat ca tep trong publish_source se duoc coi la tep moi (NEW)."
    }
} else {
    New-Item -ItemType Directory -Path $prodPath -Force | Out-Null
    Write-Warning "Thu muc Source_Prod chua ton tai. Da tu dong tao thu muc rong."
}

# Ham tinh Hash MD5 de so sanh noi dung chinh xac khi can
function Get-FileMD5Hash {
    param([string]$FilePath)
    try {
        $md5 = [System.Security.Cryptography.MD5]::Create()
        $stream = [System.IO.File]::OpenRead($FilePath)
        $hashBytes = $md5.ComputeHash($stream)
        $stream.Close()
        return [System.BitConverter]::ToString($hashBytes) -replace '-'
    } catch {
        return $null
    }
}

# ------------------------------------------------------------------------------
# 3. SO SANH (DIFF) GIUA publish_source VA Source_Prod
# ------------------------------------------------------------------------------
Write-Host ""
Write-Host "[3/4] Dang so sanh publish_source vs Source_Prod..." -ForegroundColor Yellow

$itemsToCopy = @()
$newFilesList = @()
$modifiedFilesList = @()

foreach ($key in $pubMap.Keys) {
    $pubItem = $pubMap[$key]
    $rel = $pubItem.RelativePath

    if ($ForceAll -or (-not $hasProdFiles)) {
        # Neu Source_Prod rong hoac bat co ForceAll: tat ca la file NEW
        $reason = if ($ForceAll) { "ForceAll flag" } else { "Chua co trong Source_Prod" }
        $itemsToCopy += [PSCustomObject]@{
            Path = $rel
            Status = "NEW"
            Size = $pubItem.Length
            Reason = $reason
            LocalFullPath = $pubItem.FullName
        }
        $newFilesList += $rel
        continue
    }

    if (-not $prodMap.ContainsKey($key)) {
        # File CHUA CO trong Source_Prod
        $itemsToCopy += [PSCustomObject]@{
            Path = $rel
            Status = "NEW"
            Size = $pubItem.Length
            Reason = "Tep chua co trong Source_Prod"
            LocalFullPath = $pubItem.FullName
        }
        $newFilesList += $rel
    } else {
        # File DA CO trong Source_Prod -> So sanh size & hash
        $prodItem = $prodMap[$key]
        $isDifferent = $false
        $diffReason = ""

        if ($pubItem.Length -ne $prodItem.Length) {
            $isDifferent = $true
            $sizeDiff = $pubItem.Length - $prodItem.Length
            $diffReason = "Kich thuoc khac nhau (Moi: $($pubItem.Length) bytes vs Prod: $($prodItem.Length) bytes)"
        } else {
            # Kich thuoc bang nhau -> Kiem tra them MD5 neu can
            $pubHash = Get-FileMD5Hash -FilePath $pubItem.FullName
            $prodHash = Get-FileMD5Hash -FilePath $prodItem.FullName
            if ($pubHash -and $prodHash -and ($pubHash -ne $prodHash)) {
                $isDifferent = $true
                $diffReason = "Noi dung tep thay doi (MD5 hash khac biet)"
            }
        }

        if ($isDifferent) {
            $itemsToCopy += [PSCustomObject]@{
                Path = $rel
                Status = "MODIFIED"
                Size = $pubItem.Length
                Reason = $diffReason
                LocalFullPath = $pubItem.FullName
            }
            $modifiedFilesList += $rel
        }
    }
}

# ------------------------------------------------------------------------------
# 4. SAO CHEP CAC TEP VAO THU MUC version/<ten version>
# ------------------------------------------------------------------------------
Write-Host ""
Write-Host "[4/4] Sao chep $($itemsToCopy.Count) tep vao version\$cleanVersion..." -ForegroundColor Yellow

if (-not (Test-Path $targetVersionDir)) {
    New-Item -ItemType Directory -Path $targetVersionDir -Force | Out-Null
}

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
# 5. XUAT MANIFEST VA UPDATE_NOTES.md
# ------------------------------------------------------------------------------
$manifest = [PSCustomObject]@{
    version = $cleanVersion
    created_at = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    compare_mode = "publish_source vs Source_Prod"
    total_publish_files = $pubMap.Count
    total_prod_files = $prodMap.Count
    new_files_count = $newFilesList.Count
    modified_files_count = $modifiedFilesList.Count
    total_version_files = $itemsToCopy.Count
    files = $itemsToCopy | Select-Object Path, Status, Size, Reason
}

$manifestJsonPath = Join-Path $targetVersionDir "manifest.json"
$manifestTxtPath = Join-Path $targetVersionDir "manifest.txt"
$updateNotesPath = Join-Path $targetVersionDir "UPDATE_NOTES.md"

$manifest | ConvertTo-Json -Depth 5 | Set-Content -Path $manifestJsonPath -Encoding UTF8

$nowFormatted = (Get-Date).ToString("dd/MM/yyyy HH:mm:ss")
$nowHeader = (Get-Date).ToString("HH:mm:ss dd/MM/yyyy")

# Tao manifest.txt
$txtLines = @()
$txtLines += "================================================================================"
$txtLines += "               BAO CAO TONG KET GOI PHAT HANH: $cleanVersion"
$txtLines += "================================================================================"
$txtLines += "Thoi gian tao           : $nowFormatted"
$txtLines += "Che do so sanh          : publish_source vs Source_Prod"
$txtLines += "Tong so tep trong build : $($pubMap.Count)"
$txtLines += "Tong so tep Source_Prod : $($prodMap.Count)"
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

# Tao UPDATE_NOTES.md
$gitLogLines = @()
try {
    $rawLog = & git log -n 5 --pretty=format:"* **%h** (%ad): %s" --date=format:"%d/%m/%Y" 2>$null
    if ($rawLog) {
        $gitLogLines = $rawLog
    }
} catch {
    # Bo qua loi git
}

$mdLines = @()
$mdLines += "# GHI CHU PHAT HANH - PHIEN BAN $cleanVersion"
$mdLines += ""
$mdLines += "> Goi cap nhat duoc tao tu dong vao luc **$nowHeader**"
$mdLines += ""
$mdLines += "## Thong Tin Tong Quan"
$mdLines += "- **Phien ban (Version)**: $cleanVersion"
$mdLines += "- **Thoi gian tao**: $nowFormatted"
$mdLines += "- **Nguoi thuc hien**: $env:USERNAME"
$mdLines += "- **Che do so sanh**: publish_source vs Source_Prod"
$mdLines += "- **Tong so tep trong publish_source**: $($pubMap.Count)"
$mdLines += "- **Tong so tep trong Source_Prod**: $($prodMap.Count)"
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
    $mdLines += "_Khong co tep nao bi sua doi so voi Source_Prod (toan bo la tep moi hoac giu nguyen)._"
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
$mdLines += "2. **Ghi de**: Copy toan bo noi dung trong thu muc nay de len thu muc goc cua WebApp tren Production."
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
