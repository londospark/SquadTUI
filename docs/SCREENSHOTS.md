# 📸 Capturing Screenshots & Demos

This guide explains how to capture SquadTUI screenshots and record terminal demos using the [hex1b CLI](https://hex1b.dev/).

## Prerequisites

Install the hex1b CLI tool:

```bash
dotnet tool install -g Hex1b.Tool
```

Verify the installation:

```bash
hex1b --version
```

## How It Works

SquadTUI is built with [Hex1b](https://hex1b.dev/) and has `.WithDiagnostics()` enabled in `Program.cs`. This exposes a diagnostics socket that the hex1b CLI uses to discover, inspect, and capture the running terminal app.

## Automated Capture

Run the capture script from the repo root:

```powershell
# Capture screenshots only
./scripts/capture-screenshots.ps1

# Capture screenshots + record an asciinema demo
./scripts/capture-screenshots.ps1 -Record

# Custom recording duration (default: 30s)
./scripts/capture-screenshots.ps1 -Record -RecordDuration 60
```

The script will:
1. Build SquadTUI in Release mode
2. Start it inside a hex1b-hosted terminal
3. Navigate through screens and capture SVG screenshots
4. Optionally record an asciinema `.cast` file
5. Save everything to `docs/images/`

## Manual Capture

### Step 1: Start the app in a hex1b terminal

```bash
hex1b terminal start "dotnet run --project src/SquadTUI -c Release"
```

Note the terminal ID from the output.

### Step 2: List running terminals

```bash
hex1b terminal list
```

### Step 3: Capture a screenshot

```bash
# SVG format (recommended for docs/README)
hex1b capture screenshot <terminal-id> --format svg --output docs/images/screenshot-dashboard.svg

# PNG format
hex1b capture screenshot <terminal-id> --format png --output docs/images/screenshot-dashboard.png

# HTML format
hex1b capture screenshot <terminal-id> --format html --output docs/images/screenshot-dashboard.html
```

### Step 4: Navigate between screens

Use `hex1b keys` to send keystrokes:

```bash
# Navigate to Roster
hex1b keys <terminal-id> "2"

# Navigate to Decisions
hex1b keys <terminal-id> "3"

# Cycle theme
hex1b keys <terminal-id> "T"
```

### Step 5: Stop the terminal

```bash
hex1b terminal stop <terminal-id>
```

## Recording Asciinema Demos

### Start a recording

```bash
hex1b capture recording start <terminal-id>
```

### Interact with the app

Navigate screens, scroll lists, switch themes — everything is recorded.

### Stop and save

```bash
hex1b capture recording stop <terminal-id> --output docs/images/demo.cast
```

### Upload to asciinema.org

```bash
# Install asciinema (if needed)
pip install asciinema

# Upload
asciinema upload docs/images/demo.cast
```

The upload returns a URL you can embed in the README.

### Embed in Markdown

Use an asciinema player embed:

```markdown
[![asciicast](https://asciinema.org/a/YOUR_CAST_ID.svg)](https://asciinema.org/a/YOUR_CAST_ID)
```

## Output Formats

| Format | Extension | Best For |
|--------|-----------|----------|
| SVG    | `.svg`    | README embeds, docs (scalable, crisp) |
| PNG    | `.png`    | Social media, thumbnails |
| HTML   | `.html`   | Interactive previews |
| Text   | `.txt`    | CI assertions, diffing |
| ANSI   | `.ans`    | Terminal playback |

## Troubleshooting

- **"No terminals found"** — Make sure the app was started with `hex1b terminal start`, not just `dotnet run`. The diagnostics socket requires the hex1b host.
- **Screenshots look wrong** — Try resizing the terminal first: `hex1b terminal resize <id> --cols 120 --rows 40`
- **Recording is empty** — Ensure you interact with the app between `recording start` and `recording stop`.
