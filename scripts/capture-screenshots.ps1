#!/usr/bin/env pwsh
# capture-screenshots.ps1
# Capture SquadTUI screenshots and demo recordings using hex1b CLI.
# Requires: dotnet tool install -g Hex1b.Tool
# The app must be built with .WithDiagnostics() in Program.cs.

param(
    [switch]$Record,           # Also record an asciinema demo
    [int]$RecordDuration = 30, # Seconds to record
    [string]$Theme = "Ocean",  # Theme name for screenshots
    [string]$OutputDir = "docs/images"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot
try {
    # Ensure output directory exists
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

    # Build
    Write-Host "Building SquadTUI..." -ForegroundColor Cyan
    dotnet build src/SquadTUI/SquadTUI.csproj -c Release -v minimal
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    # Start via hex1b terminal host (provides diagnostics socket)
    Write-Host "Starting SquadTUI via hex1b terminal..." -ForegroundColor Cyan
    $tmpJson = [System.IO.Path]::GetTempFileName()
    try {
        $proc = Start-Process -FilePath hex1b -ArgumentList "terminal","start","--json","--","dotnet","run","--project","src/SquadTUI","-c","Release" -RedirectStandardOutput $tmpJson -PassThru -NoNewWindow
        $proc.WaitForExit(15000) | Out-Null
        $startOutput = Get-Content $tmpJson -Raw | ConvertFrom-Json
    } finally {
        Remove-Item $tmpJson -ErrorAction SilentlyContinue
    }
    $terminalId = $startOutput.id
    Write-Host "  Terminal ID: $terminalId" -ForegroundColor Gray

    # Wait for the app to render
    Write-Host "Waiting for app to become ready..." -ForegroundColor Cyan
    hex1b capture screenshot $terminalId --wait "SquadTUI" --timeout 15 --format text | Out-Null

    # Navigation uses stack-based model:
    #   Dashboard → Enter drills into focused panel (Right/Left cycle panels 0-4)
    #   Panel mapping: 0=Roster, 1=ActivityLog, 2=Decisions, 3=Metrics, 4=Skills
    #   Escape returns to Dashboard (resets focus to panel 0)
    #   S opens Settings modal, Escape closes it
    #   F1 toggles Help screen

    # --- Capture Dashboard ---
    Write-Host "Capturing Dashboard..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-dashboard.svg"

    # --- Navigate to Roster (panel 0 → Enter) and capture ---
    hex1b keys $terminalId --key Enter
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Roster..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-roster.svg"

    # --- Navigate to Decisions (Escape → Right×2 → Enter, panel 2) ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 300
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Enter
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Decisions..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-decisions.svg"

    # --- Navigate to Skills (Escape → Right×2 → Enter, panel 4) ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 300
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Enter
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Skills..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-skills.svg"

    # --- Navigate to Activity (Escape back to Dashboard resets to panel 0, Right → panel 1, Enter) ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 300
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Enter
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Activity Log..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-activity.svg"

    # --- Navigate to Metrics (Escape → Right×3 → Enter, panel 3) ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 300
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Right
    hex1b keys $terminalId --key Enter
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Metrics..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-metrics.svg"

    # --- Navigate to Settings (Escape → S opens modal) ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 300
    hex1b keys $terminalId --text "S"
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Settings..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-settings.svg"

    # --- Navigate to Help (Escape closes modal, F1 opens Help) ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 300
    hex1b keys $terminalId --key F1
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Help..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-help.svg"

    # --- Navigate back to Dashboard ---
    hex1b keys $terminalId --key Escape
    Start-Sleep -Milliseconds 500

    # --- Optional: Record asciinema demo ---
    if ($Record) {
        Write-Host "Recording asciinema demo ($RecordDuration seconds)..." -ForegroundColor Magenta
        hex1b capture recording start $terminalId --output "$OutputDir/demo.cast" --title "SquadTUI Demo" --idle-limit 2
        Start-Sleep -Seconds 2

        # Walk through screens using stack navigation
        $pauseSec = [math]::Max(2, [math]::Floor($RecordDuration / 8))

        # Dashboard (already here)
        Start-Sleep -Seconds $pauseSec

        # Roster (panel 0 → Enter)
        hex1b keys $terminalId --key Enter
        Start-Sleep -Seconds $pauseSec

        # Back → Decisions (panel 2)
        hex1b keys $terminalId --key Escape
        Start-Sleep -Milliseconds 500
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Enter
        Start-Sleep -Seconds $pauseSec

        # Back → Skills (panel 4)
        hex1b keys $terminalId --key Escape
        Start-Sleep -Milliseconds 500
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Enter
        Start-Sleep -Seconds $pauseSec

        # Back → Activity (panel 1)
        hex1b keys $terminalId --key Escape
        Start-Sleep -Milliseconds 500
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Enter
        Start-Sleep -Seconds $pauseSec

        # Back → Metrics (panel 3)
        hex1b keys $terminalId --key Escape
        Start-Sleep -Milliseconds 500
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Right
        hex1b keys $terminalId --key Enter
        Start-Sleep -Seconds $pauseSec

        # Back → Settings modal
        hex1b keys $terminalId --key Escape
        Start-Sleep -Milliseconds 500
        hex1b keys $terminalId --text "S"
        Start-Sleep -Seconds $pauseSec

        # Close modal → Help
        hex1b keys $terminalId --key Escape
        Start-Sleep -Milliseconds 500
        hex1b keys $terminalId --key F1
        Start-Sleep -Seconds $pauseSec

        # Back to Dashboard
        hex1b keys $terminalId --key Escape
        Start-Sleep -Seconds 2

        hex1b capture recording stop $terminalId
        Write-Host "Recording saved to $OutputDir/demo.cast" -ForegroundColor Green
    }

    # Stop the terminal
    Write-Host "Stopping terminal..." -ForegroundColor Cyan
    hex1b terminal stop $terminalId

    Write-Host ""
    Write-Host "Screenshots saved to $OutputDir/" -ForegroundColor Green
    Get-ChildItem "$OutputDir/screenshot-*.svg" | ForEach-Object { Write-Host "  $_" }

    if ($Record) {
        Write-Host ""
        Write-Host "Demo recording: $OutputDir/demo.cast" -ForegroundColor Magenta
        Write-Host "Upload with: asciinema upload $OutputDir/demo.cast" -ForegroundColor Gray
    }
}
finally {
    Pop-Location
}
