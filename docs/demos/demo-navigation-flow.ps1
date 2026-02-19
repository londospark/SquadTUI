#!/usr/bin/env pwsh
# demo-navigation-flow.ps1
# Demonstrates stack-based navigation: Dashboard → Roster → Member Detail → Charter → back.
# Usage: Start acast recording first, then run this script.
#   acast record docs/demos/navigation-flow.cast
#   .\docs\demos\demo-navigation-flow.ps1
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
        "F1"     { $stdin.Write("$([char]27)OP") }
        default  { $stdin.Write($key) }
    }
    $stdin.Flush()
}

function Pause-Sec([int]$sec) { Start-Sleep -Seconds $sec }

# Dashboard loads
Pause-Sec 3

# Drill into Roster (panel 0 = Team Roster)
Send-Key "Enter"; Pause-Sec 3

# Navigate roster with j/k
Send-Key "j"; Pause-Sec 1
Send-Key "j"; Pause-Sec 1
Send-Key "k"; Pause-Sec 1

# Back to Dashboard
Send-Key "Escape"; Pause-Sec 2

# Focus Activity panel (index 1) then drill in
Send-Key "Right"; Pause-Sec 1
Send-Key "Enter"; Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"; Pause-Sec 2

# Focus Decisions panel (index 2) then drill in
Send-Key "Right"; Pause-Sec 1
Send-Key "Right"; Pause-Sec 1
Send-Key "Enter"; Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"; Pause-Sec 2

# Focus Metrics panel (index 3) then drill in
Send-Key "Right"; Pause-Sec 1
Send-Key "Right"; Pause-Sec 1
Send-Key "Right"; Pause-Sec 1
Send-Key "Enter"; Pause-Sec 3

# Toggle burndown view
Send-Key "V"; Pause-Sec 2

# Back to Dashboard
Send-Key "Escape"; Pause-Sec 2

# Quit
Send-Key "q"; Pause-Sec 1

if (-not $proc.HasExited) { $proc.Kill() }
Pop-Location
