#!/usr/bin/env pwsh
# record-demo.ps1
# Drives SquadTUI through all screens for recording.
# Run this INSIDE an acast recording session.

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot

$exe = "src\SquadTUI\bin\Release\net10.0\SquadTUI.exe"
if (-not (Test-Path $exe)) {
    Write-Host "Building SquadTUI..." -ForegroundColor Cyan
    dotnet build src/SquadTUI/SquadTUI.csproj -c Release -v minimal
}

# Start the app as a process with redirected stdin so we can send keystrokes
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
        "Tab"    { $stdin.Write([char]9) }
        "Right"  { $stdin.Write("$([char]27)[C") }
        "Left"   { $stdin.Write("$([char]27)[D") }
        "Up"     { $stdin.Write("$([char]27)[A") }
        "Down"   { $stdin.Write("$([char]27)[B") }
        "F1"     { $stdin.Write("$([char]27)OP") }
        default  { $stdin.Write($key) }
    }
    $stdin.Flush()
}

function Pause-Sec([int]$sec) { Start-Sleep -Seconds $sec }

# Wait for app to start
Pause-Sec 3

# Dashboard is showing
Pause-Sec 3

# Navigate to Roster (panel 0 → Enter)
Send-Key "Enter"
Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"
Pause-Sec 2

# Activity (Right → Enter, panel 1)
Send-Key "Right"
Send-Key "Enter"
Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"
Pause-Sec 2

# Decisions (Right×2 → Enter, panel 2)
Send-Key "Right"
Send-Key "Right"
Send-Key "Enter"
Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"
Pause-Sec 2

# Metrics (Right×3 → Enter, panel 3)
Send-Key "Right"
Send-Key "Right"
Send-Key "Right"
Send-Key "Enter"
Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"
Pause-Sec 2

# Skills (Right×4 → Enter, panel 4)
Send-Key "Right"
Send-Key "Right"
Send-Key "Right"
Send-Key "Right"
Send-Key "Enter"
Pause-Sec 3

# Back to Dashboard
Send-Key "Escape"
Pause-Sec 2

# Settings (S opens modal)
Send-Key "S"
Pause-Sec 3

# Close settings
Send-Key "Escape"
Pause-Sec 2

# Help (F1)
Send-Key "F1"
Pause-Sec 3

# Close help, back to Dashboard
Send-Key "Escape"
Pause-Sec 2

# Quit
Send-Key "q"
Pause-Sec 1

if (-not $proc.HasExited) {
    $proc.Kill()
}

Pop-Location
