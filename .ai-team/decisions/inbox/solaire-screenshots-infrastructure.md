# Decision: Screenshot & Demo Infrastructure

**By:** Solaire
**Date:** 2026-02-17
**Status:** Accepted

## What

Established screenshot and demo recording infrastructure using the hex1b CLI tool:

1. **hex1b CLI installed** (`dotnet tool install -g Hex1b.Tool` v0.87.0) — provides terminal hosting, screenshot capture (SVG/PNG/HTML/text), keystroke injection, and asciinema recording.
2. **`.WithDiagnostics()` added to Program.cs** — one-line addition to the terminal builder chain that enables the hex1b CLI to discover and control the running app via diagnostics socket.
3. **`scripts/capture-screenshots.ps1`** — automated capture script that builds, starts the app in a hex1b terminal, navigates all screens, and saves SVG screenshots to `docs/images/`. Supports optional asciinema recording with `-Record` flag.
4. **`docs/SCREENSHOTS.md`** — comprehensive guide for manual and automated screenshot capture, asciinema recording, upload, and embedding.
5. **README.md updated** — screenshot references changed from `.png` to `.svg`, added 🎬 Demo section with asciinema embed placeholder, added link to SCREENSHOTS.md.

## Why

The README referenced `.png` screenshots that don't exist. SVG is hex1b CLI's native high-quality output format — scalable, crisp, and version-control friendly. The automated script ensures screenshots can be regenerated consistently after UI changes. Asciinema recordings provide a more engaging demo than static images.

## Consequences

- Contributors can run `./scripts/capture-screenshots.ps1` to regenerate screenshots after UI changes.
- CI could be extended to auto-capture screenshots on release builds.
- The asciinema demo URL is a placeholder until a recording is uploaded.
- `.WithDiagnostics()` has no performance impact in production — it only activates when a hex1b CLI client connects.
