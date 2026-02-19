# SquadTUI Usability Report

**Tester:** Patches  
**Date:** 2026-02-19  
**Method:** Code inspection, 790+ automated E2E/unit tests, headless terminal snapshots  
**Build:** v0.2.0, Hex1b 0.90.0, .NET 10

---

## Executive Summary

SquadTUI is a surprisingly capable terminal dashboard for its age. The responsive layout system works across three breakpoints and the keyboard-driven navigation model is coherent. However, there are real usability gaps — mostly around discoverability, data authenticity, and navigation edge cases — that would frustrate a new user within the first 60 seconds.

**Verdict:** Solid bones. Needs polish.

---

## What Works Well

### 1. Responsive Layout (✅ Excellent)
The three-tier responsive system (wide ≥120, medium ≥80, narrow <80) is the best thing about this TUI. Tested across 10+ terminal widths (40, 60, 80, 100, 120, 160, 200, 300 cols) and it never crashes, never overflows. The layout gracefully degrades: three-column dashboard collapses to two-column, then single column. This is rare in terminal apps.

### 2. Theme System (✅ Good)
Four themes (Ocean, Heist, Sunset, HighContrast) apply instantly with `T`. The themed panel backgrounds, accent colors, and divider rules create genuine visual distinction between themes. Settings persist across restarts via `SettingsService`. The theme picker modal with live preview is a nice touch.

### 3. Stack Navigation (✅ Good)
Dashboard panel focus → Enter to drill in → Escape to go back. Straightforward mental model. The navigation stack preserves state correctly (e.g., your Roster selection index survives a round-trip to Charter and back). Tested with rapid navigation sequences and deep chains — no crashes.

### 4. Keyboard Consistency (✅ Good)
Case-insensitive bindings (`q` and `Q` both quit, `s` and `S` both open settings). This is thoughtful — many TUI apps get bitten by caps lock. The Shift+Key duplication in AppLayout.cs is verbose but correct.

### 5. Settings Modal Overlay (✅ Good)
The ZStack-based settings modal renders as an overlay on top of the current screen with click-away dismissal. Clean separation. Toggle settings work (theme, vim bindings, mouse, emoji, markdown, refresh interval). Settings persist to disk immediately.

---

## What's Confusing

### 1. Help Screen Lies About `?` Key (🔴 Critical UX Bug)
The Help screen displays `? Toggle this help` as a documented keybinding, but **there is no `?` key binding anywhere in AppLayout.cs**. Only `F1` toggles help. A user reading the help text will press `?` and nothing will happen. This is the worst kind of UX bug — the app is teaching users the wrong thing.

**Recommendation:** Either bind `?` to toggle help, or change the help text to say `F1 Toggle this help`.

### 2. No Loading Indicator (🟡 Medium)
`state.IsLoading` and `state.ErrorMessage` are set in `Program.cs` during async data loading but **never read by any screen**. On a slow filesystem or network mount, the dashboard renders with empty data for a few hundred milliseconds. There's no spinner, no "Loading…" text, nothing. The user sees an empty dashboard and might think the app is broken.

**Recommendation:** Show a simple loading state in `DashboardScreen` when `state.IsLoading` is true.

### 3. Vim Bindings Are Optional But Not Obvious (🟡 Medium)
`j`/`k` navigation only works when `settings.VimBindings` is `true`. If a user reads the Help screen (which shows `j / k Move down / up`), presses `j`, and nothing happens because Vim bindings are off — that's confusing. The Help screen doesn't indicate these are conditional.

**Recommendation:** Show `(Vim mode)` next to j/k entries in Help, or grey them out when disabled.

### 4. No Screen Indicator / Breadcrumb (🟡 Medium)
When you drill into Roster, Decisions, or Metrics, there's no indicator of *where you are* in the navigation stack. The footer keybindings change (good), but there's no "Dashboard → Roster" breadcrumb or header showing the current screen name. At a glance, the user has to mentally track their position.

**Recommendation:** Add a minimal breadcrumb or highlight the current screen in a nav header.

### 5. Dashboard Focus Not Visually Obvious Enough (🟡 Medium)
The focused panel on the Dashboard gets a highlighted header (inverse colors), but if the user doesn't notice the subtle color shift, they won't understand which panel Enter will drill into. At narrow widths (below 80 cols) where panels stack vertically, the focus concept is harder to perceive.

**Recommendation:** Add a visible focus indicator like `►` prefix or a brighter border/glow on the focused panel.

---

## What Could Be Improved

### 1. Footer Keybindings Are Dense (Low)
The footer shows all available keys in a single line: `Tab/←→: Focus Panel  Enter: Open  R: Refresh  T: Theme  S: Settings  Q: Quit  F1: Help`. At narrow widths, this truncates. At any width, it's a lot to parse.

**Recommendation:** Show only the 3-4 most relevant keys in the footer. Use F1/Help for the rest.

### 2. Add Member (A Key) Creates Generic Names (Low)
Pressing `A` on the Roster creates `Member{N}` with role "Team Member". There's no input prompt for a real name. This feels like a placeholder.

**Recommendation:** Either implement a text input dialog or remove the A-key binding until it's useful.

### 3. Metrics Screen Needs Sprint Data to Be Useful (Low)
Without sprint history data in `.ai-team/`, the Metrics screen shows "No sprint data available." The bar charts and breakdown charts from Hex1b look great when data exists, but there's no guidance on how to create sprint data.

**Recommendation:** Add a hint: "Create sprint data in `.ai-team/sprints/` to see metrics."

### 4. `D` Key Clash: Dashboard Panel Focus vs. Delete Member (Low)
`D` on the Dashboard is handled by arrow key panel navigation (no conflict), but on the Roster screen, `D` triggers member removal confirmation. If a user is used to pressing `D` on the dashboard and switches to Roster, they'll accidentally initiate a delete flow. The `Y/N` confirmation guards against data loss, but the initial surprise is jarring.

**Recommendation:** Use a less common key for destructive actions. `X` or `Del` would be safer.

### 5. NoSquad Screen is Informative (✅ Actually Good)
The no-squad onboarding screen is well-designed: clear explanation of what SquadTUI is, what a squad is, setup instructions with the `npx` command, and a `C` key to bootstrap a basic structure. This is the best first-run experience in the app.

---

## Data Integrity Concerns

These are not usability issues per se, but they affect trust:

1. **SampleData leaks:** `SkillsScreen.GetConfidenceLevel()` returns hardcoded progress bars for 5 known sample skills. If your real skill names don't match, confidence bars won't display.
2. **`state.ErrorMessage` is silently swallowed.** If data loading fails, the user gets empty screens with no error feedback.
3. **`MemberDetailScreen` defaults to "Sonic"** if `SelectedMemberName` is null — a character from the old sample data universe. This should default to the first actual member or show an empty state.

---

## acast Recording Notes

- **acast is installed** at `%USERPROFILE%\go\bin\acast.exe` and functional.
- **acast requires a TTY** — cannot be driven from headless/non-interactive sessions.
- Demo scripts are in `docs/demos/` and use redirected stdin to drive the TUI programmatically within an acast recording session.
- The existing `scripts/record-demo.ps1` provides a full app tour; the new scripts are focused demos for specific features.
- Post-processing workflow: `record → cut → quantize → convert-to-gif`. The quantize step is essential for removing idle pauses from scripted demos.

---

## Test Coverage Confidence

This report is backed by **790 automated tests** (787 passing, 3 pre-existing ANSI text-splitting failures). The E2E test suite validates every screen at multiple terminal widths, navigation edge cases (rapid switching, deep stack chains, escape on root), theme cycling, and settings persistence. The assertions I'm making about keyboard behavior, layout breakpoints, and navigation flow are empirically verified, not guesses.

---

*— Patches, Tester. Trust nothing. Verify everything.*
