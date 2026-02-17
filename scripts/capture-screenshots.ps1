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
    dotnet build src/SquadTUI/SquadTUI.csproj -c Release --quiet
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    # Start via hex1b terminal host (provides diagnostics socket)
    Write-Host "Starting SquadTUI via hex1b terminal..." -ForegroundColor Cyan
    $startOutput = hex1b terminal start "dotnet run --project src/SquadTUI -c Release" --json 2>&1 | ConvertFrom-Json
    $terminalId = $startOutput.id
    Write-Host "  Terminal ID: $terminalId" -ForegroundColor Gray

    # Wait for the app to render
    Write-Host "Waiting for app to become ready..." -ForegroundColor Cyan
    hex1b capture screenshot $terminalId --wait "SquadTUI" --timeout 15 --format text | Out-Null

    # --- Capture Dashboard ---
    Write-Host "Capturing Dashboard..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-dashboard.svg"

    # --- Navigate to Roster (key 2) and capture ---
    hex1b keys $terminalId "2"
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Roster..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-roster.svg"

    # --- Navigate to Decisions (key 3) and capture ---
    hex1b keys $terminalId "3"
    Start-Sleep -Milliseconds 500
    Write-Host "Capturing Decisions..." -ForegroundColor Green
    hex1b capture screenshot $terminalId --format svg --output "$OutputDir/screenshot-decisions.svg"

    # --- Navigate back to Dashboard ---
    hex1b keys $terminalId "1"
    Start-Sleep -Milliseconds 500

    # --- Optional: Record asciinema demo ---
    if ($Record) {
        Write-Host "Recording asciinema demo ($RecordDuration seconds)..." -ForegroundColor Magenta
        hex1b capture recording start $terminalId
        Start-Sleep -Seconds 2

        # Walk through screens for the demo
        $screens = @("1", "2", "3", "4", "5", "6")
        foreach ($key in $screens) {
            hex1b keys $terminalId $key
            Start-Sleep -Seconds ([math]::Floor($RecordDuration / $screens.Length))
        }

        hex1b capture recording stop $terminalId --output "$OutputDir/demo.cast"
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
