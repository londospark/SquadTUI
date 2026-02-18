---
name: "acast-terminal-recording"
description: "Terminal session recording, editing, and GIF conversion using acast"
domain: "documentation"
confidence: "low"
source: "manual"
tools:
  - name: "powershell"
    description: "Run acast commands for recording, editing, and converting terminal sessions"
    when: "Creating demo recordings, editing cast files, or generating GIF assets"
---

## Context

When the team needs to create terminal demos, documentation assets, or shareable recordings of CLI workflows, use `acast` — a cross-platform terminal session recorder written in Go. It supports native recording, playback, editing, and GIF conversion. This is useful for README demos, PR documentation, and feature showcases.

## Patterns

### Executable Location

The acast binary is at `%USERPROFILE%\go\bin\acast.exe`. Use the full path or ensure `%USERPROFILE%\go\bin` is on the system PATH.

### Recording a Session

```powershell
& "$env:USERPROFILE\go\bin\acast.exe" record demo.cast
# Stop recording with: exit or Ctrl+D
```

### Playing a Recording

```powershell
& "$env:USERPROFILE\go\bin\acast.exe" play demo.cast
```

### Converting to GIF

```powershell
& "$env:USERPROFILE\go\bin\acast.exe" convert-to-gif demo.cast demo.gif
```

### Editing Recordings

**Cut out a time range** (remove mistakes, loading screens, sensitive info):
```powershell
& "$env:USERPROFILE\go\bin\acast.exe" cut --start=5.0 --end=10.5 input.cast output.cast
```

**Adjust playback speed** of a segment:
```powershell
& "$env:USERPROFILE\go\bin\acast.exe" speed --start=0.0 --end=2.9 --factor=0.7 input.cast output.cast
```

**Quantize idle time** (trim long pauses):
```powershell
& "$env:USERPROFILE\go\bin\acast.exe" quantize --ranges=1.0,5.0 input.cast output.cast
```

### Cloud Sharing

```powershell
# Link to asciinema.org account
& "$env:USERPROFILE\go\bin\acast.exe" auth

# Upload a recording
& "$env:USERPROFILE\go\bin\acast.exe" upload demo.cast
```

## Examples

### Typical Documentation Workflow

1. Record the demo: `acast record demo.cast`
2. Cut out mistakes: `acast cut --start=12.0 --end=18.5 demo.cast clean.cast`
3. Remove idle time: `acast quantize --ranges=1.0,3.0 clean.cast final.cast`
4. Convert to GIF: `acast convert-to-gif final.cast docs/demo.gif`

### Quick Version Check

```powershell
& "$env:USERPROFILE\go\bin\acast.exe" version
```

## Anti-Patterns

- Don't record sessions that contain secrets, tokens, or credentials without cutting them out before sharing.
- Don't skip the quantize step for long recordings — idle pauses make demos tedious.
- Don't use `record` in non-interactive (headless) contexts — it requires a TTY.
- Don't forget to use the `&` call operator in PowerShell when invoking the full path.
