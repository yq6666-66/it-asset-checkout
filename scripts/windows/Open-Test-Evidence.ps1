# Opens existing SD6503 test evidence. Does not re-run Selenium, JMeter, or the build.
# Finds the repo by walking up from this script until ItAssetCheckout.sln exists.
# ASCII-only for Windows PowerShell 5.1.
$ErrorActionPreference = 'Stop'

$AppUrl = 'http://127.0.0.1:5080'

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

function Resolve-MarkText {
  if ([string]::IsNullOrWhiteSpace($env:LOCALAPPDATA)) { return $null }
  $candidate = Join-Path $env:LOCALAPPDATA 'Programs\marktext\MarkText.exe'
  if (Test-Path -LiteralPath $candidate) { return $candidate }
  return $null
}

function Open-Folder([string]$path) {
  if (-not (Test-Path -LiteralPath $path)) {
    Write-Host ('Skip (missing): ' + $path) -ForegroundColor DarkYellow
    return
  }
  Write-Host ('Folder: ' + $path) -ForegroundColor Cyan
  Start-Process explorer.exe $path
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
  Start-Process $path
}

try {
  if ($env:OS -ne 'Windows_NT') {
    throw 'This opener is for Windows PowerShell 5.1.'
  }
  # PS 5.1 draws a progress bar that makes Invoke-WebRequest miss the short timeout.
  $ProgressPreference = 'SilentlyContinue'

  Write-Host 'Opening test evidence...' -ForegroundColor Green
  $RepoRoot = Resolve-RepoRoot
  $testing = Join-Path $RepoRoot 'docs\testing'
  if (-not (Test-Path -LiteralPath $testing)) {
    throw ('Could not find ' + $testing)
  }

  $perf = Join-Path $RepoRoot 'docs\performance'
  $shots = Join-Path $testing 'selenium\screenshots'
  $security = Join-Path $RepoRoot 'docs\security'

  Write-Host '==> Folders' -ForegroundColor Cyan
  Open-Folder $testing
  Start-Sleep -Milliseconds 400
  Open-Folder $shots
  Start-Sleep -Milliseconds 400
  Open-Folder $perf
  if (Test-Path -LiteralPath $security) {
    Start-Sleep -Milliseconds 400
    Open-Folder $security
  }

  Write-Host '==> Key files' -ForegroundColor Cyan
  Open-File (Join-Path $testing 'unit-tests.md')
  Start-Sleep -Milliseconds 200
  Open-File (Join-Path $testing 'selenium-run.md')
  Start-Sleep -Milliseconds 200
  Open-File (Join-Path $testing 'black-box.md')
  Start-Sleep -Milliseconds 200
  Open-File (Join-Path $testing 'white-box.md')
  Start-Sleep -Milliseconds 200
  Open-File (Join-Path $perf 'results-summary.md')
  Start-Sleep -Milliseconds 200
  Open-File (Join-Path $perf 'jmeter-dashboard.png')

  $testUrl = $AppUrl + '/Test'
  try {
    $r = Invoke-WebRequest -Uri $testUrl -UseBasicParsing -TimeoutSec 2
    if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 500) {
      Write-Host '==> App is up; opening Test page' -ForegroundColor Cyan
      Start-Process $testUrl
    }
  } catch {
    Write-Host 'App not running; skipped Test page. Start scripts\windows\Start-IT-Asset-Checkout.bat if needed.' -ForegroundColor DarkGray
  }

  Write-Host ''
  Write-Host 'Opened existing docs and screenshots. Nothing was re-run.' -ForegroundColor Green
  Write-Host 'Press Enter to close...'
  [void][System.Console]::ReadLine()
}
catch {
  Write-Host ('ERROR: ' + $_.Exception.Message) -ForegroundColor Red
  Write-Host 'Press Enter to close...'
  [void][System.Console]::ReadLine()
  exit 1
}
