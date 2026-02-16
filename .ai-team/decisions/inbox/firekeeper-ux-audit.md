# UX Improvement Specification — Firekeeper

**Date:** 2026-02-18
**By:** Firekeeper (UX/Design)
**Status:** Active

## Executive Summary

LondoSpark flagged cramped layouts, missing explanations, unwanted borders, and overall visual quality. This spec audits every screen and recommends improvements using Hex1b's full widget catalog — many of which we aren't using yet.

---

## Hex1b Widget Audit — What We're Missing

### Currently Used
- `Text`, `HStack`, `VStack`, `List`, `Button`, `Responsive`, `BarChart`, `Border` (themed invisible), `Splitter`

### Available But Unused
| Widget | Value for SquadTUI |
|---|---|
| **Table** | Roster, Decisions list — proper columns, headers, row focus, virtualization |
| **TabPanel** | Dashboard tabs (Overview/Decisions/Activity/Metrics) — built-in tab bar with icons |
| **Progress** | Task completion, sprint progress — native themed progress bars instead of manual `█░` |
| **InfoBar** | Bottom status bar — mode, theme name, squad path, member count |
| **BreakdownChart** | Task status distribution — proportional segmented bar |
| **ColumnChart** | Velocity over sprints — vertical bars complement existing horizontal BarChart |
| **TimeSeriesChart** | Activity over time — braille-precision line charts |
| **Spinner** | Loading states during async data fetch |
| **Tree** | Squad hierarchy — agents under roles, skills under agents |
| **ToggleSwitch** | Settings toggles instead of text-based options |
| **Notifications** | Toast notifications for actions (theme changed, charter saved) |

### Recommended Adoption Priority
1. **InfoBar** — immediate value as a persistent status bar (low effort)
2. **Progress** — replace manual progress bars in Dashboard (low effort)
3. **Table** — Roster and Decisions list panels (medium effort, high impact)
4. **TabPanel** — Dashboard content organization (medium effort, high impact)
5. **BreakdownChart** — Metrics task distribution (low effort)
6. **Notifications** — Action feedback (medium effort)

---

## Per-Screen Assessment

### Dashboard (`DashboardScreen.cs`)

**Current state:** Rich 3-column responsive layout with manual progress bars and inline stats.

**What's wrong:**
- Manual `█▓░` progress bar should use native `Progress` widget
- No subtitle explaining what the dashboard shows
- Stats lack breathing room between metric groups

**Recommendations:**
- Replace manual progress bar with `v.Progress(completedPct).Fill()`
- Add descriptive subtitle under main header
- Consider `TabPanel` for Overview/Activity/Metrics sub-views
- Add `InfoBar` at the bottom showing squad path and theme

### Roster (`RosterScreen.cs`)

**Current state:** 2-column HStack with list + detail view. Good structure.

**What's wrong:**
- Section headers are bold+accent but lack visual weight — need reverse video bars
- Detail panel metadata lines cramped (no spacing between groups)
- No subtitle explaining what the roster shows

**Recommendations:**
- Use reverse video (`\x1b[7m`) for all section headers
- Add empty line after every `━━━` rule
- Add subtitle: "Your squad members and their current status"
- Consider `Table` widget for the member list (proper columns for name, role, status)

### Decisions (`DecisionsScreen.cs`) ✅ IMPROVED

**Current state:** 2-column with list + detail. Now has reverse-video headers, subtitle, and improved spacing.

**What's wrong (before fix):**
- Headers were plain bold — no visual weight
- No subtitle explaining what decisions are
- Content jumped right into the list with no breathing room
- Detail panel metadata fields too close together

**Applied fixes:**
- Reverse-video section headers
- Subtitle: "Team decisions and architectural choices"
- Empty lines between every section
- 4-space indent on metadata fields

### Skills (`SkillsScreen.cs`) ✅ IMPROVED

**Current state:** 2-column with list + detail. Now has reverse-video headers, subtitle, and improved spacing.

**What's wrong (before fix):**
- Same cramped header pattern as other screens
- No explanation of what skills are
- Related Members and Usage sections ran together

**Applied fixes:**
- Reverse-video section headers
- Subtitle: "Available capabilities for your squad"
- Empty lines between all section boundaries
- 4-space indent on detail content

### Activity Log (`ActivityLogScreen.cs`) ✅ IMPROVED

**Current state:** 2-column with list + detail. Now has reverse-video headers, subtitle, and improved spacing.

**What's wrong (before fix):**
- Cramped headers, no subtitle
- Summary text flush with section header
- Decisions/Outcomes sections lacked breathing room

**Applied fixes:**
- Reverse-video section headers
- Subtitle: "Chronological record of squad interactions"
- Empty lines after every rule and before every content block
- 4-space indent on content

### Help (`HelpScreen.cs`) ✅ IMPROVED

**Current state:** Responsive 2-column/1-column layout with keyboard reference. Now has proper spacing.

**What's wrong (before fix):**
- Key bindings were dense wall of text
- No subtitle explaining the screen
- Categories ran together without separation
- Section rules missing between categories

**Applied fixes:**
- Reverse-video main header
- Subtitle: "Quick reference for all keyboard shortcuts"
- Rule lines between categories
- Empty lines between every group
- 4-space indent on all key bindings

### Metrics (`MetricsScreen.cs`) — SIEGMEYER OWNS

**Current state:** BarChart + summary stats.

**Recommendations for Siegmeyer:**
- Add reverse-video header with subtitle explaining metrics
- Use `Progress` widget for completion percentage
- Add `BreakdownChart` for task status distribution
- Consider `ColumnChart` for velocity over time
- Add breathing room between chart and summary

### NoSquad (`NoSquadScreen.cs`) — SIEGMEYER OWNS

**Current state:** Centered dialog with Unicode box borders.

**Recommendations for Siegmeyer:**
- LondoSpark explicitly said "The borders are crap! NO borders, use shading"
- Replace box-drawing borders with background shading via `ThemePanel`
- Use reverse-video bars for section separators instead of `┌─┐│└─┘`
- More breathing room inside the dialog

---

## Design Language — Consistency Rules

All screens MUST follow these patterns:

1. **Main header:** Reverse video bar — `{B}{acc}{RV} emoji  Title {R}`
2. **Subtitle:** Dim text immediately below header — `{D}One-line explanation{R}`
3. **Section separator:** Empty line → secondary accent rule → empty line
4. **Section header:** Bold + accent (not reverse video) — `{B}{acc}Section Name{R}`
5. **Content indent:** 4 spaces for field labels, 4+ for nested content
6. **Rule lines:** Secondary accent color, NOT dim — `{sec}{━━━}{R}`
7. **Empty lines:** Between every logical section (header→content, content→next section)
8. **No borders:** Use shading/reverse-video for containment, never box-drawing chars

---

## Impact

- **Firekeeper:** Implemented spacing+header fixes on Decisions, Skills, ActivityLog, Help screens
- **Siegmeyer:** Should apply same patterns to Metrics and NoSquad screens
- **Future:** Table, TabPanel, InfoBar, Progress adoption tracked as separate work items
