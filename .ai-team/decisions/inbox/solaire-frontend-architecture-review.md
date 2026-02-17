### 2026-02-17: Frontend Architecture Review

**By:** Solaire (with input from Firekeeper & Siegmeyer perspectives)

**What:** Comprehensive audit of Hex1b widget usage, theming approach, and panel backgrounds across all screens. Identified 11 unused Hex1b widgets, confirmed ANSI escape code misuse for backgrounds, and clarified panel background strategy.

## Widget Audit: What We Use vs. What We Should Use

### Currently Used Widgets ✅
- **Layout:** VStack, HStack, Responsive, Fill
- **Interactive:** List (Roster, Decisions, ActivityLog, Skills)
- **Display:** Text, BarChart (MetricsScreen)
- **Input:** InputBindings (all screens)

### Available But UNUSED Widgets (by category):

#### 🎯 **HIGH PRIORITY — Should Use**
1. **TabPanel** — We're building tab-like navigation manually with emoji labels. AppLayout.cs lines 14-22 define tab metadata that could be fed directly to TabPanel. **Impact:** Cleaner tab rendering, built-in tab selection styling, consistent active/inactive states.
   
2. **Border** — Currently using ANSI escape rules (`━━━`) everywhere. Border widget provides proper separators with theme integration. **Impact:** Consistent dividers, proper theme color inheritance.

3. **Scroll** — Long content in DecisionsScreen detail pane (lines 50-52 markdown content) and RosterScreen charter excerpts (lines 71-72) have no scrolling. **Impact:** User can see full content beyond viewport.

4. **Progress** — Manual ASCII progress bars in DashboardScreen (line 78) and MetricsScreen (lines 30-35). **Impact:** Theme-aware, semantically correct progress indicators.

#### 📊 **MEDIUM PRIORITY — Consider Using**
5. **Table** — RosterScreen member list (line 36 List widget) and Skills list could benefit from multi-column tabular format (Name | Role | Status | Task). **Impact:** Better information density.

6. **Splitter** — HStack layouts with manual FillWidth ratios (e.g., RosterScreen line 38 `FillWidth(2)`, line 85 `FillWidth(3)`) could use Splitter for user-resizable panes. **Impact:** User-adjustable layout.

7. **Align** — NoSquadScreen (lines 18-74) manually centers content with nested HStack/VStack. Align widget does this idiomatically. **Impact:** Simpler centering code.

8. **InfoBar** — Removed in Sprint 6 (decision says "Removed InfoBar entirely"). Could be reintroduced for transient notifications (e.g., "Theme changed to Sunset"). **Impact:** Contextual feedback without modal dialogs.

#### 🛠️ **LOW PRIORITY — Future Enhancements**
9. **Charts:** ColumnChart, BreakdownChart, TimeSeriesChart, ScatterChart — MetricsScreen only uses BarChart. Sprint velocity over time = TimeSeriesChart. Task breakdown by status = BreakdownChart. **Impact:** Richer data visualization.

10. **Tree** — Charter content (currently flat markdown) could use Tree for hierarchical sections (Identity → Role → Expertise → Boundaries). **Impact:** Collapsible charter sections.

11. **ToggleSwitch** — SettingsScreen could use ToggleSwitch for boolean preferences instead of List selection. **Impact:** More intuitive boolean controls.

12. **Notifications** — Toast-style notifications for ephemeral messages (errors, confirmations). **Impact:** Non-blocking user feedback.

13. **Spinner** — Loading indicator while DataBridge loads async data at startup. **Impact:** Visual feedback during I/O.

14. **Picker** — Theme selection in SettingsScreen could use Picker instead of List. **Impact:** Cleaner single-value selection UI.

15. **Hyperlink** — NoSquadScreen line 62 has a plain URL string. Hyperlink widget makes it clickable (if terminal supports OSC 8). **Impact:** Better UX in modern terminals.

16. **QrCode** — Could generate QR code for squad URLs or asciinema demo links in Help screen. **Impact:** Mobile device integration.

## Theming Audit: ANSI Escape Codes vs. Hex1b Theme System

### ❌ **INCORRECT: Manual ANSI Background Codes**
**Problem identified:** We're using `ThemeManager.GetPanelHeaderBg()` which returns raw ANSI background escape sequences (e.g., `\x1b[48;2;22;28;38m`). This is used in:
- DashboardScreen lines 27, 45, 64, 94, 102 (5 occurrences)
- RosterScreen lines 28, 34, 45, 70 (4 occurrences)
- DecisionsScreen lines 22, 28, 41, 50 (4 occurrences)
- MetricsScreen lines 20, 49, 84, 91 (4 occurrences)
- SkillsScreen lines 27, 33, 48, 57, 70 (5 occurrences)
- ActivityLogScreen lines 23, 29, 44, 53, 63, 74 (6 occurrences)
- HelpScreen lines 19, 26, 36, 43, 51, 58, 82, 90, 97, 108 (10 occurrences)
- NoSquadScreen line 26 (reverse-video, but still manual ANSI)

**Total:** 41+ manual ANSI background escape sequences across 7 screens.

**Why this is wrong:**
1. **Breaks Hex1b theme abstraction** — Hex1b's `GlobalTheme.BackgroundColor` exists specifically for this. We're bypassing it.
2. **No dynamic theme updates** — If user switches theme with T key, these ANSI codes don't update without full re-render.
3. **Inconsistent with Hex1b philosophy** — Hex1b provides `ThemePanel` for scoped theme mutations. We're reinventing it badly.

### ✅ **CORRECT APPROACH: ThemePanel + Theme Properties**

**What we SHOULD do:**
```csharp
// Instead of:
var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
detail.Text($"  {hBg}{B}{acc}👤 {selected.Name}{R}"),

// Do this:
detail.ThemePanel(tp => [
    tp.Text($"  {B}{acc}👤 {selected.Name}{R}"),
], theme => theme
    .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(22, 28, 38))
)
```

**How ThemePanel works:**
- Takes a lambda that builds child widgets and a lambda that mutates the theme.
- All descendants inherit the mutated theme properties.
- Changes apply *within that subtree only* — no global pollution.
- Respects parent theme if only partial properties mutated.

**Alternative for simple backgrounds:**
```csharp
// For solid background panels:
detail.ThemePanel(tp => [
    tp.Text("  Content here"),
], theme => theme.Set(GlobalTheme.BackgroundColor, accentColor))
```

## Panel Backgrounds: The BackdropWidget Misconception

**User request:** "The user wants nice backgrounds on panels. BackdropWidget doesn't exist."

**Analysis:**
1. **BackdropWidget does NOT exist in Hex1b 0.87.0.** ✅ Correctly identified.
2. **The right approach:** Use `ThemePanel` for theme-based backgrounds, or `EffectPanel` for advanced effects.

### Strategy 1: ThemePanel for Solid Backgrounds
```csharp
// Panel with distinct background color
h.ThemePanel(panel => [
    panel.Text("  Header"),
    panel.Text("  Content"),
], theme => theme
    .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(20, 25, 30))
    .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(200, 210, 220))
)
```

**Use for:**
- Section headers (DashboardScreen columns, RosterScreen detail pane)
- NoSquadScreen centered welcome panel
- Help screen sections

### Strategy 2: EffectPanel for Custom Effects
```csharp
// EffectPanel applies post-processing to rendered cells
h.EffectPanel(panel => [
    panel.Text("  Content"),
], effect: (cell, x, y) => {
    // Modify cell background/foreground per-cell
    if (y % 2 == 0) cell.Background = Hex1bColor.FromRgb(18, 22, 28); // Zebra stripe
    return cell;
})
```

**Use for:**
- Gradient backgrounds
- Zebra-striped tables
- Highlight bands
- Custom visual effects

### Strategy 3: Surface for Direct Rendering (ADVANCED)
```csharp
// Surface provides raw cell buffer access
h.Surface((surface, width, height) => {
    // Direct cell manipulation — lowest level
    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            surface[x, y] = new Cell(' ', fg: ..., bg: ...);
})
```

**Use for:**
- Complex custom rendering
- Performance-critical scenarios
- When widget composition doesn't suffice

## Specific Screen Recommendations

### DashboardScreen
**Current issues:**
- 5 manual `hBg` background codes (lines 27, 45, 64, 94, 102)
- Manual progress bar rendering (line 78)
- No scrolling in 3-column layout

**Recommended changes:**
1. Wrap each column in `ThemePanel` with distinct `BackgroundColor` for visual separation.
2. Replace ASCII progress bar (line 78) with `Progress` widget.
3. Wrap content in `Scroll` widget for long lists.
4. Consider `Border` widget for column separators instead of `━━━` rules.

**Priority:** HIGH — Dashboard is first screen users see.

### RosterScreen
**Current issues:**
- 4 manual `hBg` codes (lines 28, 34, 45, 70)
- Charter excerpt (lines 71-72) not scrollable
- List widget (line 36) could be Table for multi-column

**Recommended changes:**
1. Wrap detail pane (right side) in `ThemePanel` with subtle background.
2. Wrap charter section in `Scroll` widget.
3. Consider `Table` widget for roster list (columns: Status | Name | Role | Task).

**Priority:** HIGH — Second most-used screen.

### DecisionsScreen
**Current issues:**
- 4 manual `hBg` codes (lines 22, 28, 41, 50)
- Markdown content (line 52) not scrollable
- Could use `Border` for separators

**Recommended changes:**
1. Wrap decision detail (right pane) in `ThemePanel`.
2. Wrap markdown content in `Scroll` widget — long decisions get cut off.
3. Replace `━━━` rules with `Border` widget.

**Priority:** MEDIUM — Users read but don't edit in-app.

### MetricsScreen
**Current issues:**
- 4 manual `hBg` codes (lines 20, 49, 84, 91)
- Manual progress bar (lines 30-35)
- Only uses BarChart — no TimeSeriesChart for velocity over time

**Recommended changes:**
1. Replace manual progress bar with `Progress` widget.
2. Add `TimeSeriesChart` for sprint velocity over time.
3. Add `BreakdownChart` for task status breakdown (done/active/pending/blocked).
4. Wrap chart panels in `ThemePanel` for visual separation.

**Priority:** MEDIUM — Data-heavy screen benefits from richer widgets.

### SkillsScreen
**Current issues:**
- 5 manual `hBg` codes (lines 27, 33, 48, 57, 70)
- Related members list (lines 62-65) could use `Tree` for hierarchical relationships

**Recommended changes:**
1. Wrap sections in `ThemePanel`.
2. Consider `Tree` widget for skill → member relationships.
3. Progress bar for confidence (line 97) could use `Progress` widget.

**Priority:** LOW — Less frequently used.

### ActivityLogScreen
**Current issues:**
- 6 manual `hBg` codes (lines 23, 29, 44, 53, 63, 74)
- Log detail content not scrollable

**Recommended changes:**
1. Wrap log detail in `ThemePanel` + `Scroll`.
2. Replace `━━━` rules with `Border`.

**Priority:** MEDIUM — Long log entries need scrolling.

### NoSquadScreen
**Current issues:**
- Manual centering with nested HStack/VStack (lines 18-74)
- Reverse-video title (line 26) uses manual ANSI `\x1b[7m`
- Plain URL string (line 62) not clickable

**Recommended changes:**
1. Use `Align` widget for centering instead of manual stacking.
2. Wrap centered content in `ThemePanel` with distinct background to create a "card" effect.
3. Replace URL string with `Hyperlink` widget (if terminal supports OSC 8).
4. Replace reverse-video ANSI code with `ThemePanel` background mutation.

**Priority:** MEDIUM — First-run experience should feel polished.

### HelpScreen
**Current issues:**
- 10 manual `hBg` codes (lines 19, 26, 36, 43, 51, 58, 82, 90, 97, 108)
- Two-column layout could use `Border` between columns

**Recommended changes:**
1. Wrap sections in `ThemePanel` for visual grouping.
2. Add vertical `Border` between left/right columns.

**Priority:** LOW — Help is transient, users focus on content.

## Recommendations Summary

### For Siegmeyer (Implementation)
**Priority 1 (Critical):**
1. **Remove all manual ANSI background codes.** Replace with `ThemePanel` wrapping sections that need distinct backgrounds. Target: DashboardScreen, RosterScreen, DecisionsScreen.
2. **Add Scroll widgets** to long-content areas: DecisionsScreen markdown, RosterScreen charter, ActivityLogScreen detail.
3. **Replace manual progress bars** with `Progress` widget in DashboardScreen and MetricsScreen.

**Priority 2 (High Value):**
4. **Add Border widgets** to replace `━━━` separator lines across all screens.
5. **Use TabPanel** in AppLayout instead of manual tab rendering (lines 44-90).
6. **Add Align widget** to NoSquadScreen for idiomatic centering.

**Priority 3 (Nice to Have):**
7. Add `Table` widget to RosterScreen for multi-column roster display.
8. Add `TimeSeriesChart` and `BreakdownChart` to MetricsScreen.
9. Add `Hyperlink` widget to NoSquadScreen URL.
10. Add `Notifications` widget for transient feedback (theme changes, errors).

### For Firekeeper (UX Spec Updates)
**Action items:**
1. **Document ThemePanel usage patterns** — create a style guide for when to use ThemePanel vs. manual theming.
2. **Define panel background color palette** — specify exact RGB values for header backgrounds, detail panes, etc. per theme.
3. **Chart integration spec** — design TimeSeriesChart and BreakdownChart layouts for MetricsScreen.
4. **Table layout spec** — design multi-column roster table (columns, widths, sort indicators).
5. **Scrolling UX** — define scroll indicators (scrollbar style, page-down hints).
6. **Loading states** — design Spinner placement during async data loads.

## Why This Matters

**Consistency:** Manual ANSI codes fragment theme logic across 7 files. Centralizing in ThemePanel means theme changes propagate correctly.

**Maintainability:** 41+ manual escape sequences are brittle. Adding a new theme means updating 7 files. ThemePanel means updating ThemeManager.cs only.

**User Experience:** Missing widgets (Scroll, Progress, Border) cause UX gaps — cut-off content, crude progress bars, inconsistent separators.

**Framework Alignment:** We're paying for Hex1b but bypassing its abstractions. Using ThemePanel/EffectPanel/Border/Progress/Scroll aligns with framework intent and gets us better rendering, theming, and maintainability.

## Decision

**Immediate action (Sprint 7):**
1. Siegmeyer implements Priority 1 (ANSI removal, Scroll, Progress) on Dashboard, Roster, Decisions screens.
2. Firekeeper writes ThemePanel style guide and panel background palette.

**Next sprint (Sprint 8):**
3. Siegmeyer implements Priority 2 (Border, TabPanel, Align).
4. Firekeeper specs Chart and Table layouts.

**Future (Sprint 9+):**
5. Siegmeyer implements Priority 3 (Table, Charts, Hyperlink, Notifications).
