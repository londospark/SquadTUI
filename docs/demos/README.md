# SquadTUI Demo Recordings

Scripted demos for recording terminal sessions using [acast](https://github.com/asciinema/acast).

## Prerequisites

- **acast** installed: `go install github.com/asciinema/acast@latest`
  - Binary location: `%USERPROFILE%\go\bin\acast.exe`
- **SquadTUI** built: `dotnet build src/SquadTUI/SquadTUI.csproj -c Release`
- Run from the **repository root** (`X:\Code\SquadTUI` or equivalent)

## Available Demos

| Demo | Script | Duration | Shows |
|------|--------|----------|-------|
| Dashboard Tour | `demo-dashboard-tour.ps1` | ~25s | Panel focus cycling, drill-in to Roster, back navigation |
| Navigation Flow | `demo-navigation-flow.ps1` | ~45s | Stack navigation through all screens, j/k list nav, burndown toggle |
| Theme & Settings | `demo-theme-settings.ps1` | ~30s | Theme cycling (4 themes), settings modal, help screen |

## Recording a Demo

```powershell
# 1. Start recording in a new terminal
& "$env:USERPROFILE\go\bin\acast.exe" record docs/demos/dashboard-tour.cast

# 2. In the SAME terminal (acast is recording), run the demo script
.\docs\demos\demo-dashboard-tour.ps1

# 3. Type 'exit' to stop recording
exit
```

## Post-Processing

```powershell
$acast = "$env:USERPROFILE\go\bin\acast.exe"

# Remove idle pauses (keeps recording snappy)
& $acast quantize --ranges=1.0,3.0 dashboard-tour.cast dashboard-tour-clean.cast

# Convert to GIF for README/docs
& $acast convert-to-gif dashboard-tour-clean.cast dashboard-tour.gif

# Upload to asciinema.org for sharing
& $acast upload dashboard-tour-clean.cast
```

## Full App Tour

For a comprehensive walkthrough of all screens, use the existing script:

```powershell
& "$env:USERPROFILE\go\bin\acast.exe" record docs/demos/full-tour.cast
.\scripts\record-demo.ps1
exit
```

## Notes

- **acast requires a TTY** — run in a real terminal, not headless/piped.
- Demo scripts use redirected stdin to drive the app programmatically.
- Each script builds the app automatically if the Release binary is missing.
- All demos run against the **real `.ai-team/` data** in the repo.
