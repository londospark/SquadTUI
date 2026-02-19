#!/usr/bin/env pwsh
# demo-theme-settings.ps1
# Demonstrates theme cycling with T key and the Settings modal.
# Usage: Start acast recording first, then run this script.
#   acast record docs/demos/theme-settings.cast
#   .\docs\demos\demo-theme-settings.ps1
#   exit  (stops recording)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Push-Location $repoRoot

$exe = "src\SquadTUI\bin\Release\net10.0\SquadTUI.exe"
if (-not (Test-Path $exe)) {
    Write-Host "Building SquadTUI..." -ForegroundColor Cyan
    dotnet build src/SquadTUI/SquadTUI.csproj -c Release -v minimal
}

$psi = [System.Diagnostics.ProcessStartInfo]::new()
$psi.FileName = (Resolve-Path $exe).Path
$psi.UseShellExecute = $false
$psi.RedirectStandardInput = $true
$psi.WorkingDirectory = $repoRoot

$proc = [System.Diagnostics.Process]::Start($psi)
$stdin = $proc.StandardInput

function Send-Key([string]$key) {
    switch ($key) {
        "Escape" { $stdin.Write([char]27) }
        default  { $stdin.Write($key) }
    }
    $stdin.Flush()
}

function Pause-Sec([int]$sec) { Start-Sleep -Seconds $sec }

# Dashboard loads with Ocean theme
Pause-Sec 3

# Cycle themes: Ocean → Heist → Sunset → HighContrast → Ocean
Send-Key "T"; Pause-Sec 2  # Heist
Send-Key "T"; Pause-Sec 2  # Sunset
Send-Key "T"; Pause-Sec 2  # HighContrast
Send-Key "T"; Pause-Sec 2  # Back to Ocean

# Open Settings modal
Send-Key "S"; Pause-Sec 3

# Navigate settings items
Send-Key "j"; Pause-Sec 1
Send-Key "j"; Pause-Sec 1
Send-Key "j"; Pause-Sec 1

# Close settings
Send-Key "Escape"; Pause-Sec 2

# Open help (F1)
Send-Key "$([char]27)OP"; Pause-Sec 3

# Close help
Send-Key "Escape"; Pause-Sec 2

# Quit
Send-Key "q"; Pause-Sec 1

if (-not $proc.HasExited) { $proc.Kill() }
Pop-Location
