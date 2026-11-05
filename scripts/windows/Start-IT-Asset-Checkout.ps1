# One-click launcher for SD6503 IT Asset Checkout (local demo only).
# Finds the repo by walking up from this script until ItAssetCheckout.sln exists.
# Reads DEMO_ADMIN_PASSWORD from the gitignored .env. This file has no password.
$ErrorActionPreference = 'Stop'

$AppUrl = 'http://127.0.0.1:5080'

function Write-Step([string]$msg) {
  Write-Host ''
  Write-Host ('==> ' + $msg) -ForegroundColor Cyan
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

try {
  if ($env:OS -ne 'Windows_NT') {
    throw 'This launcher is for Windows PowerShell 5.1.'
  }
  if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw '.NET SDK not found. Install the .NET 8 SDK first.'
  }

  $RepoRoot = Resolve-RepoRoot
  $proj = Join-Path $RepoRoot 'src\ItAssetCheckout\ItAssetCheckout.csproj'
  if (-not (Test-Path -LiteralPath $proj)) {
    throw ('Could not find ' + $proj)
  }

  Write-Host 'IT Asset Checkout launcher' -ForegroundColor Green
  Write-Host ('Repo: ' + $RepoRoot)

  Set-Location -LiteralPath $RepoRoot
  Import-RepoDotEnv -RepoRoot $RepoRoot

  Write-Step 'dotnet restore'
  & dotnet restore
  if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }

  # Open the browser after Kestrel has had a moment to listen.
  Start-Job -ScriptBlock {
    param($u)
    Start-Sleep -Seconds 8
    Start-Process $u
  } -ArgumentList $AppUrl | Out-Null

  Write-Step ('Starting app -> ' + $AppUrl)
  Write-Host ('Login: ' + $env:DEMO_ADMIN_USER + ' / (password from .env)') -ForegroundColor Yellow
  Write-Host 'Close this window to stop the server.' -ForegroundColor DarkGray
  Write-Host ''

  & dotnet run --project $proj --urls $AppUrl
  if ($LASTEXITCODE -ne 0) { throw 'dotnet run ended with a non-zero exit code' }
}
catch {
  Write-Host ''
  Write-Host ('ERROR: ' + $_.Exception.Message) -ForegroundColor Red
  Write-Host 'Press Enter to close...'
  [void][System.Console]::ReadLine()
  exit 1
}
