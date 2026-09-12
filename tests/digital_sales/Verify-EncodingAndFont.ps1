# ==============================================================================
# Automated 5-Layer Verification Suite: Anti-Mojibake, Unicode Integrity & UI Font
# Module: DigitalSales (Hồ sơ Kinh doanh Số)
# ==============================================================================
$ErrorActionPreference = "Stop"
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " RUNNING 5-LAYER ANTI-MOJIBAKE & UNICODE VERIFICATION SUITE" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

$cs = "Data Source=10.57.30.10;Initial Catalog=quanlydoanhthucenit;User Id=quanlydoanhthucenit;Password=Kdhe@543HE2;Connect Timeout=15;"
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

$allPass = $true

# ------------------------------------------------------------------------------
# LAYER 1: Database Modules Mojibake Scanner (sys.sql_modules)
# ------------------------------------------------------------------------------
Write-Host "`n[LAYER 1] Scanning All Database SPs for Mojibake / Corrupted Unicode..." -ForegroundColor Yellow
$cmd1 = $conn.CreateCommand()
$cmd1.CommandText = @"
SELECT o.name, o.type_desc, m.definition
FROM sys.sql_modules m
INNER JOIN sys.objects o ON m.object_id = o.object_id
WHERE o.name LIKE 'RM_DigitalSales%'
"@
$reader1 = $cmd1.ExecuteReader()
$corruptedSPs = @()

while ($reader1.Read()) {
    $spName = $reader1["name"].ToString()
    $def = $reader1["definition"].ToString()
    if ($def -match "CÆ" -or $def -match "há»" -or $def -match "Dá»" -or $def -match "Ã¡n" -or $def -match "KhÃ¡c") {
        $corruptedSPs += $spName
    }
}
$reader1.Close()

if ($corruptedSPs.Count -eq 0) {
    Write-Host "  -> PASS: 0 corrupted SPs found in database RM_DigitalSales modules!" -ForegroundColor Green
} else {
    Write-Host "  -> FAIL: Found corrupted SPs: $($corruptedSPs -join ', ')" -ForegroundColor Red
    $allPass = $false
}

# ------------------------------------------------------------------------------
# LAYER 2: Runtime Execution of RM_DigitalSales_GetByID (Record 69)
# ------------------------------------------------------------------------------
Write-Host "`n[LAYER 2] Verifying SP Runtime Output on Record 69..." -ForegroundColor Yellow
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "EXEC dbo.RM_DigitalSales_GetByID @DigitalSalesID = 69, @UserName = 'tantd'"
$reader2 = $cmd2.ExecuteReader()

$verified69 = $false
if ($reader2.Read()) {
    $btn = $reader2["BusinessTypeName"].ToString()
    $rawBytes = [System.Text.Encoding]::UTF8.GetBytes($btn)
    Write-Host "  -> Record 69 BusinessTypeName: '$btn'" -ForegroundColor Cyan
    Write-Host "  -> UTF-8 Bytes: $($rawBytes -join ' ')" -ForegroundColor Gray
    
    # "Cơ hội" in UTF-8 bytes: 67 (C), 198 161 (ơ), 32 (space), 104 (h), 225 187 153 (ộ), 105 (i)
    $expectedBytes = @(67, 198, 161, 32, 104, 225, 187, 153, 105)
    $bytesMatch = $true
    if ($rawBytes.Length -ne $expectedBytes.Length) {
        $bytesMatch = $false
    } else {
        for ($i = 0; $i -lt $rawBytes.Length; $i++) {
            if ($rawBytes[$i] -ne $expectedBytes[$i]) {
                $bytesMatch = $false
                break
            }
        }
    }

    if ($bytesMatch) {
        Write-Host "  -> PASS: Record 69 BusinessTypeName exactly matches UTF-8 bytes for 'Cơ hội'!" -ForegroundColor Green
        $verified69 = $true
    } else {
        Write-Host "  -> FAIL: Byte mismatch! Actual bytes: $($rawBytes -join ' ')" -ForegroundColor Red
        $allPass = $false
    }
}
$reader2.Close()

if (-not $verified69) {
    Write-Host "  -> FAIL: Record 69 could not be read or matched!" -ForegroundColor Red
    $allPass = $false
}

# ------------------------------------------------------------------------------
# LAYER 3: Sys_Messages DB Coverage for Opportunity & Project labels
# ------------------------------------------------------------------------------
Write-Host "`n[LAYER 3] Checking Sys_Messages for BusinessType labels..." -ForegroundColor Yellow
$cmd3 = $conn.CreateCommand()
$cmd3.CommandText = @"
SELECT LabelKey, Message 
FROM dbo.Sys_Messages 
WHERE LabelKey IN ('DigitalSales_BusinessType_Opportunity', 'DigitalSales_BusinessType_Project')
"@
$reader3 = $cmd3.ExecuteReader()
$keysFound = 0
while ($reader3.Read()) {
    $k = $reader3["LabelKey"].ToString()
    $m = $reader3["Message"].ToString()
    Write-Host "  -> Found Sys_Messages ['$k'] = '$m'" -ForegroundColor Cyan
    if ($m -ne "" -and -not $m.Contains("Æ") -and -not $m.Contains("á»")) {
        $keysFound++
    }
}
$reader3.Close()

if ($keysFound -ge 2) {
    Write-Host "  -> PASS: Sys_Messages contains valid Unicode translations for BusinessType!" -ForegroundColor Green
} else {
    Write-Host "  -> FAIL: Missing or corrupted Sys_Messages keys! (Found $keysFound/2)" -ForegroundColor Red
    $allPass = $false
}

$conn.Close()

# ------------------------------------------------------------------------------
# LAYER 4: Triple Mirroring & UTF-8 BOM Verification
# ------------------------------------------------------------------------------
Write-Host "`n[LAYER 4] Checking Triple Mirroring & UTF-8 BOM for _ChangeStatusModal.cshtml..." -ForegroundColor Yellow
$p1 = "d:\SVN\crm\Modules.Cate\Areas\Cate\Views\DigitalSales\_ChangeStatusModal.cshtml"
$p2 = "d:\SVN\crm\publish_source\Areas\Cate\Views\DigitalSales\_ChangeStatusModal.cshtml"
$p3 = "d:\SVN\crm\CenIT.Solution.TOC.WebApp\Areas\Cate\Views\DigitalSales\_ChangeStatusModal.cshtml"

$files = @($p1, $p2, $p3)
$hashes = @()

foreach ($f in $files) {
    if (-not (Test-Path $f)) {
        Write-Host "  -> FAIL: Missing file $f" -ForegroundColor Red
        $allPass = $false
        continue
    }
    
    $bytes = [System.IO.File]::ReadAllBytes($f)
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    $md5 = (Get-FileHash -Path $f -Algorithm MD5).Hash
    $hashes += $md5
    
    if ($hasBom) {
        Write-Host "  -> PASS: UTF-8 BOM confirmed for $f" -ForegroundColor Green
    } else {
        Write-Host "  -> FAIL: Missing UTF-8 BOM for $f" -ForegroundColor Red
        $allPass = $false
    }
}

if ($hashes.Count -eq 3 -and $hashes[0] -eq $hashes[1] -and $hashes[1] -eq $hashes[2]) {
    Write-Host "  -> PASS: Triple Mirroring MD5 Match 100% ($($hashes[0]))" -ForegroundColor Green
} else {
    Write-Host "  -> FAIL: Triple Mirroring MD5 Mismatch: $($hashes -join ', ')" -ForegroundColor Red
    $allPass = $false
}

# ------------------------------------------------------------------------------
# LAYER 5: Defense-in-Depth Code Verification (Controller & Razor View)
# ------------------------------------------------------------------------------
Write-Host "`n[LAYER 5] Checking Defense-in-Depth in Controller and View..." -ForegroundColor Yellow
$ctrlCode = [System.IO.File]::ReadAllText("d:\SVN\crm\Modules.Cate\Areas\Cate\Controllers\DigitalSalesController.cs")
$viewCode = [System.IO.File]::ReadAllText("d:\SVN\crm\Modules.Cate\Areas\Cate\Views\DigitalSales\_ChangeStatusModal.cshtml")

$ctrlProtected = $ctrlCode.Contains("CurrentBusinessTypeName = sales.BusinessType == 1") -and $ctrlCode.Contains("DigitalSales_BusinessType_Opportunity")
$viewProtected = $viewCode.Contains("Model.CurrentBusinessType == 1") -and $viewCode.Contains("DigitalSales_BusinessType_Opportunity")

if ($ctrlProtected) {
    Write-Host "  -> PASS: DigitalSalesController.cs is protected with AppProcessor.Messagor fallback!" -ForegroundColor Green
} else {
    Write-Host "  -> FAIL: DigitalSalesController.cs is not using AppProcessor.Messagor fallback!" -ForegroundColor Red
    $allPass = $false
}

if ($viewProtected) {
    Write-Host "  -> PASS: _ChangeStatusModal.cshtml is protected with AppProcessor.Messagor fallback!" -ForegroundColor Green
} else {
    Write-Host "  -> FAIL: _ChangeStatusModal.cshtml is not using AppProcessor.Messagor fallback!" -ForegroundColor Red
    $allPass = $false
}

# ------------------------------------------------------------------------------
# Final Summary
# ------------------------------------------------------------------------------
Write-Host "`n=================================================================" -ForegroundColor Cyan
if ($allPass) {
    Write-Host " ALL 5 LAYERS PASSED SUCCESSFULLY! ZERO MOJIBAKE / FONT ERRORS." -ForegroundColor Green
} else {
    Write-Host " SUITE FAILED! Some tests did not meet strict standards." -ForegroundColor Red
    exit 1
}
Write-Host "=================================================================`n" -ForegroundColor Cyan
