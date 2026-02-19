#!/usr/bin/env pwsh
# demo-dashboard-tour.ps1
# Quick tour of the Dashboard: panel focus, drill-in, and back.
# Usage: Start acast recording first, then run this script.
#   acast record docs/demos/dashboard-tour.cast
#   .\docs\demos\demo-dashboard-tour.ps1
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
        "Enter"  { $stdin.Write([char]13) }
        "Escape" { $stdin.Write([char]27) }
        "Right"  { $stdin.Write("$([char]27)[C") }
        "Left"   { $stdin.Write("$([char]27)[D") }
        default  { $stdin.Write($key) }
    }
    $stdin.Flush()
}

function Pause-Sec([int]$sec) { Start-Sleep -Seconds $sec }

# Let dashboard load
Pause-Sec 3

# Focus each dashboard panel: Team Roster → Activity → Decisions → Metrics → Skills
Send-Key "Right"; Pause-Sec 2
Send-Key "Right"; Pause-Sec 2
Send-Key "Right"; Pause-Sec 2
Send-Key "Right"; Pause-Sec 2

# Drill into Roster
Send-Key "Left"; Pause-Sec 1
Send-Key "Left"; Pause-Sec 1
Send-Key "Left"; Pause-Sec 1
Send-Key "Left"; Pause-Sec 1
Send-Key "Enter"; Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"; Pause-Sec 2

# Quit
Send-Key "q"; Pause-Sec 1

if (-not $proc.HasExited) { $proc.Kill() }
Pop-Location
