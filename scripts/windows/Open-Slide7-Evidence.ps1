# LIVE Slide-7 evidence: app, Selenium, JMeter, security build, then open docs.
# Finds the repo by walking up from this script until ItAssetCheckout.sln exists.
# Reads DEMO_ADMIN_PASSWORD from the gitignored .env. This file has no password.
# ASCII-only for Windows PowerShell 5.1.
$ErrorActionPreference = 'Stop'

$TrelloUrl = 'https://trello.com/b/beKd2EVy/it-asset-checkout'
$AppUrl = 'http://127.0.0.1:5080'
$script:AppJob = $null
$script:StartedAppHere = $false

function Write-Step([string]$msg) {
  Write-Host ''
  Write-Host ('==> ' + $msg) -ForegroundColor Cyan
}

function Pause-Step([string]$msg) {
  Write-Host ''
  Write-Host $msg -ForegroundColor Yellow
  Write-Host 'Press Enter for next step...'
  [void][System.Console]::ReadLine()
}

function Resolve-RepoRoot {
  $dir = $PSScriptRoot
  if ([string]::IsNullOrWhiteSpace($dir)) {
    throw 'PSScriptRoot is empty. Run this script with powershell.exe -File.'
  }
  while (-not [string]::IsNullOrWhiteSpace($dir)) {
    $sln = Join-Path $dir 'ItAssetCheckout.sln'
    if (Test-Path -LiteralPath $sln) {
      return $dir
    }
    $parent = Split-Path -Parent $dir
    if ([string]::IsNullOrWhiteSpace($parent) -or ($parent -eq $dir)) {
      break
    }
    $dir = $parent
  }
  throw 'Could not find ItAssetCheckout.sln above this script.'
}

function Import-RepoDotEnv([string]$RepoRoot) {
  $envFile = Join-Path $RepoRoot '.env'
  $envExample = Join-Path $RepoRoot '.env.example'
  if (-not (Test-Path -LiteralPath $envFile)) {
    if (Test-Path -LiteralPath $envExample) {
      Copy-Item -LiteralPath $envExample -Destination $envFile
      Write-Host 'Created .env from .env.example.' -ForegroundColor Yellow
    }
    throw 'Set DEMO_ADMIN_PASSWORD in the gitignored .env file (see .env.example), then run again.'
  }

  $utf8 = New-Object System.Text.UTF8Encoding $false
  # @() keeps a one-line file as an array under Windows PowerShell 5.1.
  $lines = @([System.IO.File]::ReadAllLines($envFile, $utf8))
  if ($lines.Length -gt 0 -and $lines[0].StartsWith([char]0xFEFF)) {
    $lines[0] = $lines[0].TrimStart([char]0xFEFF)
  }
  $lines | ForEach-Object {
    $line = $_.Trim()
    if ($line -eq '' -or $line.StartsWith('#')) { return }
    $eq = $line.IndexOf('=')
    if ($eq -lt 1) { return }
    $name = $line.Substring(0, $eq).Trim()
    $value = $line.Substring($eq + 1).Trim()
    if ($value.Length -ge 2) {
      $quote = $value.Substring(0, 1)
      if (($quote -eq '"' -or $quote -eq "'") -and $value.EndsWith($quote)) {
        $value = $value.Substring(1, $value.Length - 2)
      }
    }
    if ($name -notmatch '^[A-Za-z_][A-Za-z0-9_]*$') { return }
    Set-Item -Path ('Env:' + $name) -Value $value
  }

  $placeholder = 'CHANGE_ME_DEMO_ONLY'
  if ([string]::IsNullOrWhiteSpace($env:DEMO_ADMIN_PASSWORD) -or $env:DEMO_ADMIN_PASSWORD -eq $placeholder) {
    throw 'DEMO_ADMIN_PASSWORD is missing or still the placeholder in .env. Set it locally. Do not commit .env.'
  }
  if ([string]::IsNullOrWhiteSpace($env:DEMO_ADMIN_USER)) {
    $env:DEMO_ADMIN_USER = 'admin'
  }
  $env:ASPNETCORE_ENVIRONMENT = 'Development'
}

function Resolve-MarkText {
  if ([string]::IsNullOrWhiteSpace($env:LOCALAPPDATA)) { return $null }
  $candidate = Join-Path $env:LOCALAPPDATA 'Programs\marktext\MarkText.exe'
  if (Test-Path -LiteralPath $candidate) { return $candidate }
  return $null
}

function Resolve-JMeterBat {
  if (-not [string]::IsNullOrWhiteSpace($env:JMETER_HOME)) {
    $fromEnv = Join-Path $env:JMETER_HOME 'bin\jmeter.bat'
    if (Test-Path -LiteralPath $fromEnv) { return $fromEnv }
  }
  $fallback = 'C:\Tools\apache-jmeter-5.6.3\bin\jmeter.bat'
  if (Test-Path -LiteralPath $fallback) { return $fallback }
  throw 'JMeter not found. Set JMETER_HOME to the Apache JMeter folder, or install 5.6.3 at C:\Tools\apache-jmeter-5.6.3.'
}

function Test-AppUp {
  try {
    $r = Invoke-WebRequest -Uri $AppUrl -UseBasicParsing -TimeoutSec 2
    return ($r.StatusCode -ge 200 -and $r.StatusCode -lt 500)
  } catch {
    return $false
  }
}

function Ensure-AppRunning([string]$RepoRoot) {
  if (Test-AppUp) {
    Write-Host ('App already up at ' + $AppUrl) -ForegroundColor Green
    return
  }
  Write-Step ('Starting app at ' + $AppUrl)
  $proj = Join-Path $RepoRoot 'src\ItAssetCheckout\ItAssetCheckout.csproj'
  $script:AppJob = Start-Job -ScriptBlock {
    param($repo, $projPath, $url, $user, $pass)
    Set-Location $repo
    $env:DEMO_ADMIN_USER = $user
    $env:DEMO_ADMIN_PASSWORD = $pass
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    & dotnet run --project $projPath --urls $url --no-launch-profile 2>&1
  } -ArgumentList $RepoRoot, $proj, $AppUrl, $env:DEMO_ADMIN_USER, $env:DEMO_ADMIN_PASSWORD
  $script:StartedAppHere = $true
  $deadline = (Get-Date).AddSeconds(90)
  while ((Get-Date) -lt $deadline) {
    if (Test-AppUp) {
      Write-Host 'App is ready.' -ForegroundColor Green
      return
    }
    if ($script:AppJob.State -eq 'Failed' -or $script:AppJob.State -eq 'Completed') {
      $out = Receive-Job $script:AppJob -ErrorAction SilentlyContinue | Out-String
      if (-not [string]::IsNullOrEmpty($env:DEMO_ADMIN_PASSWORD)) {
        $out = $out.Replace($env:DEMO_ADMIN_PASSWORD, '***')
      }
      throw ('App failed to start. Job state=' + $script:AppJob.State + "`n" + $out)
    }
    Start-Sleep -Seconds 2
  }
  throw 'App did not become ready within 90 seconds.'
}

function Stop-CheckoutHosts {
  Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" -ErrorAction SilentlyContinue |
    Where-Object { $_.CommandLine -and $_.CommandLine -like '*ItAssetCheckout*' -and $_.CommandLine -notlike '*test*' } |
    ForEach-Object {
      Write-Host ('Stopping PID ' + $_.ProcessId + ' holding build outputs') -ForegroundColor DarkGray
      try { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue } catch {}
    }
  Get-Process -Name 'ItAssetCheckout' -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host ('Stopping PID ' + $_.Id + ' (ItAssetCheckout host)') -ForegroundColor DarkGray
    try { Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue } catch {}
  }
}

function Stop-AppIfStartedHere {
  if (-not $script:StartedAppHere) { return }
  Write-Step 'Stopping app started by this script (needed for clean build)'
  if ($script:AppJob) {
    Stop-Job $script:AppJob -ErrorAction SilentlyContinue
    Remove-Job $script:AppJob -Force -ErrorAction SilentlyContinue
    $script:AppJob = $null
  }
  Stop-CheckoutHosts
  Start-Sleep -Seconds 2
  $script:StartedAppHere = $false
}

function Open-File([string]$path) {
  if (-not (Test-Path -LiteralPath $path)) {
    Write-Host ('Skip (missing): ' + $path) -ForegroundColor DarkYellow
    return
  }
  Write-Host ('Open: ' + $path) -ForegroundColor Cyan
  $ext = [System.IO.Path]::GetExtension($path).ToLowerInvariant()
  if ($ext -eq '.md' -or $ext -eq '.markdown') {
    $markText = Resolve-MarkText
    if ($markText) {
      Start-Process -FilePath $markText -ArgumentList ('"' + $path + '"')
      return
    }
  }
  if ($ext -eq '.png' -or $ext -eq '.jpg' -or $ext -eq '.jpeg' -or $ext -eq '.gif' -or $ext -eq '.bmp' -or $ext -eq '.webp') {
    $photos = $null
    if (-not [string]::IsNullOrWhiteSpace($env:LOCALAPPDATA)) {
      $photos = Join-Path $env:LOCALAPPDATA 'OpenWithPhotos.cmd'
    }
    if ($photos -and (Test-Path -LiteralPath $photos)) {
      Start-Process -FilePath $photos -ArgumentList ('"' + $path + '"')
      return
    }
    $wpsPhoto = 'D:\download\WPS Office\12.1.0.28022\office6\photolaunch.exe'
    if (Test-Path -LiteralPath $wpsPhoto) {
      Start-Process -FilePath $wpsPhoto -ArgumentList @('/photo', '/view', $path)
      return
    }
    Start-Process -FilePath 'explorer.exe' -ArgumentList ('"' + $path + '"')
    return
  }
  if ($ext -eq '.html' -or $ext -eq '.htm') {
    Start-Process $path
    return
  }
  Start-Process $path
}

function Open-Folder([string]$path) {
  if (-not (Test-Path -LiteralPath $path)) {
    Write-Host ('Skip (missing): ' + $path) -ForegroundColor DarkYellow
    return
  }
  Write-Host ('Folder: ' + $path) -ForegroundColor Cyan
  Start-Process explorer.exe $path
}

function Run-Selenium([string]$RepoRoot) {
  Write-Step 'LIVE Selenium (RUN_SELENIUM=1)'
  $env:RUN_SELENIUM = '1'
  $env:SELENIUM_SHOT_DIR = Join-Path $RepoRoot 'docs\testing\selenium\screenshots'
  New-Item -ItemType Directory -Force -Path $env:SELENIUM_SHOT_DIR | Out-Null
  & dotnet test (Join-Path $RepoRoot 'tests\ItAssetCheckout.Selenium\ItAssetCheckout.Selenium.csproj') --nologo
  if ($LASTEXITCODE -ne 0) {
    throw ('Selenium failed with exit code ' + $LASTEXITCODE)
  }
  Write-Host 'Selenium finished (expect 4 Passed).' -ForegroundColor Green
}

function Run-JMeter([string]$RepoRoot) {
  Write-Step 'LIVE JMeter (60s, 20 threads)'
  $jmeterBat = Resolve-JMeterBat
  $perf = Join-Path $RepoRoot 'docs\performance'
  $jmx = Join-Path $perf 'it-asset-checkout.jmx'
  $jtl = Join-Path $perf 'results.jtl'
  $report = Join-Path $perf 'jmeter-report'
  if (-not (Test-Path -LiteralPath $jmx)) { throw ('Missing ' + $jmx) }
  # Local refresh so JMeter can write a new report. Do not commit this over the archived 2026-11-03 artifacts.
  if (Test-Path -LiteralPath $jtl) { Remove-Item -LiteralPath $jtl -Force }
  if (Test-Path -LiteralPath $report) { Remove-Item -LiteralPath $report -Recurse -Force }
  $passArg = '-JdemoPassword=' + $env:DEMO_ADMIN_PASSWORD
  & $jmeterBat -n -t $jmx $passArg -l $jtl -e -o $report
  if ($LASTEXITCODE -ne 0) {
    throw ('JMeter failed with exit code ' + $LASTEXITCODE)
  }
  Write-Host 'JMeter finished.' -ForegroundColor Green
}

function Run-SecurityBuild([string]$RepoRoot) {
  Write-Step 'LIVE security build (dotnet clean + build; look for SCS0018, 0 Errors)'
  Stop-AppIfStartedHere
  Stop-CheckoutHosts
  Start-Sleep -Seconds 2
  $proj = Join-Path $RepoRoot 'src\ItAssetCheckout\ItAssetCheckout.csproj'
  & dotnet clean $proj --nologo
  & dotnet build $proj --nologo
  if ($LASTEXITCODE -ne 0) {
    throw ('dotnet build failed with exit code ' + $LASTEXITCODE)
  }
  Write-Host 'Build finished (SCS0018 warnings are expected; Errors should be 0).' -ForegroundColor Green
}

try {
  if ($env:OS -ne 'Windows_NT') {
    throw 'This walkthrough is for Windows PowerShell 5.1.'
  }
  # PS 5.1 draws a progress bar that makes Invoke-WebRequest miss the short timeout.
  $ProgressPreference = 'SilentlyContinue'

  Write-Host 'Slide 7 LIVE evidence walkthrough (video order)' -ForegroundColor Green
  $RepoRoot = Resolve-RepoRoot
  $proj = Join-Path $RepoRoot 'src\ItAssetCheckout\ItAssetCheckout.csproj'
  if (-not (Test-Path -LiteralPath $proj)) {
    throw ('Could not find ' + $proj)
  }
  Set-Location -LiteralPath $RepoRoot
  Import-RepoDotEnv -RepoRoot $RepoRoot

  if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw '.NET SDK not found. Install the .NET 8 SDK first.'
  }

  $testing = Join-Path $RepoRoot 'docs\testing'
  $shots = Join-Path $testing 'selenium\screenshots'
  $perf = Join-Path $RepoRoot 'docs\performance'
  $sec = Join-Path $RepoRoot 'docs\security'
  $agile = Join-Path $RepoRoot 'docs\agile'

  Pause-Step 'STEP 1/7 - Ensure site up, then LIVE Selenium (Chrome + 4 Passed)'
  Ensure-AppRunning -RepoRoot $RepoRoot
  Run-Selenium -RepoRoot $RepoRoot
  Open-File (Join-Path $testing 'selenium-run.md')
  Start-Sleep -Milliseconds 300
  Open-Folder $shots

  Pause-Step 'STEP 2/7 - LIVE JMeter (~60s), then open dashboard'
  if (-not (Test-AppUp)) { Ensure-AppRunning -RepoRoot $RepoRoot }
  Run-JMeter -RepoRoot $RepoRoot
  Open-File (Join-Path $perf 'results-summary.md')
  Start-Sleep -Milliseconds 300
  Open-File (Join-Path $perf 'jmeter-dashboard.png')
  Start-Sleep -Milliseconds 300
  Open-File (Join-Path $perf 'jmeter-report\index.html')

  Pause-Step 'STEP 3/7 - LIVE dotnet clean/build (SCS0018), then security-code-scan.md'
  Run-SecurityBuild -RepoRoot $RepoRoot
  Open-File (Join-Path $sec 'security-code-scan.md')

  Pause-Step 'STEP 4/7 - Trello Done column (US-01..US-08)'
  Start-Process $TrelloUrl

  Pause-Step 'STEP 5/7 - OWASP mapping (scroll to SCS0018 note)'
  Open-File (Join-Path $sec 'owasp-top-10.md')

  Pause-Step 'STEP 6/7 - User stories + MoSCoW'
  Open-File (Join-Path $agile 'user-stories.md')
  Start-Sleep -Milliseconds 300
  Open-File (Join-Path $agile 'moscow.md')

  Pause-Step 'STEP 7/7 - Sprint 1 notes'
  Open-File (Join-Path $agile 'sprint-1.md')

  try {
    if (-not (Test-AppUp)) {
      Write-Step 'Restarting app for Test page'
      Ensure-AppRunning -RepoRoot $RepoRoot
    }
    Write-Host 'Opening Test page for UT list.' -ForegroundColor Cyan
    Start-Process ($AppUrl + '/Test')
  } catch {
    Write-Host 'Could not open Test page (app not up).' -ForegroundColor DarkGray
  }

  Write-Host ''
  Write-Host 'Done. Live runs finished; evidence windows are open.' -ForegroundColor Green
  Write-Host 'If this script started the app, that process ends when this window closes.' -ForegroundColor DarkGray
  Write-Host 'Use Start-IT-Asset-Checkout.bat when the site should stay up.' -ForegroundColor DarkGray
  Write-Host 'Press Enter to exit...'
  [void][System.Console]::ReadLine()
}
catch {
  Write-Host ('ERROR: ' + $_.Exception.Message) -ForegroundColor Red
  Write-Host 'Press Enter to close...'
  [void][System.Console]::ReadLine()
  exit 1
}
