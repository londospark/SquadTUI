# Sprint 6 UI Cleanup — Decision Record

**Author:** Siegmeyer (Frontend Dev)
**Date:** 2026-02-17
**Issues:** #11, #12, #13, #14, #15, #17, #19

## Context

Multiple UI polish issues were filed for SquadTUI to improve visual consistency, remove clutter, and add modern interaction patterns. All changes were implemented together as a cohesive sprint.

## Decisions

### 1. Remove all ANSI background escape codes (#11, #12)
**Decision:** Strip all `\x1b[48;2;R;G;Bm` background codes from Text() content across all screens.
**Rationale:** Hex1b's `GlobalTheme.BackgroundColor` fills the entire terminal uniformly. ANSI bg codes in text strings only color the character cells, creating jarring rectangles against the theme background. Foreground-only ANSI codes (bold, dim, italic, colors) work correctly.
**Impact:** `PanelColors` record simplified from 4 fields to 1 (`Accent` only). `PanelRenderer` simplified significantly.

### 2. Keep J/K as state-updating bindings (#13)
**Decision:** J/K update AppState indices rather than programmatically moving ListNode selection.
**Rationale:** `ListNode.MoveDown()`/`MoveUp()` are documented in Hex1b XML but are internal/inaccessible. The next best approach is updating state, which the detail panes read from. Arrow keys drive the List widget's visual highlight natively.
**Trade-off:** J/K and arrow keys are not perfectly synced — J/K updates the detail pane, arrows update the list highlight. Acceptable until Hex1b exposes public list navigation API.

### 3. Use Button widgets for clickable NavBar (#15)
**Decision:** Replace `h.Text(label)` with `h.Button(label).OnClick(...)` in NavBar.
**Rationale:** Button is the only Hex1b widget with click handler support. Enables mouse-driven navigation.

### 4. Remove InfoBar entirely (#17)
**Decision:** Remove the InfoBar from both Program.cs and TestAppBuilder.cs. Theme name no longer displayed.
**Rationale:** InfoBar consumed vertical space and showed redundant info (screen name visible from content, theme name rarely needed).

### 5. Use background-color-only selection highlighting (#14)
**Decision:** Set `SelectedIndicator` to `"  "` (two spaces) for all themes. Selection shown via `SelectedBackgroundColor` only.
**Rationale:** Arrow indicators (`▶`) combined with highlight color was visually redundant and noisy.

### 6. Remove bracket notation from NavBar (#19)
**Decision:** Change labels from `[1]Dashboard` to `🏠 Dashboard`. Remove `[Q]Quit` entry.
**Rationale:** Brackets add visual clutter. Key bindings are discoverable via help screen. Quit via Q key still works.

## Files Changed
- `src/SquadTUI/Rendering/PanelRenderer.cs`
- `src/SquadTUI/Themes/ThemeManager.cs`
- `src/SquadTUI/Screens/NavBar.cs`
- `src/SquadTUI/Screens/DashboardScreen.cs`
- `src/SquadTUI/Screens/RosterScreen.cs`
- `src/SquadTUI/Screens/DecisionsScreen.cs`
- `src/SquadTUI/Screens/SkillsScreen.cs`
- `src/SquadTUI/Screens/ActivityLogScreen.cs`
- `src/SquadTUI/Screens/MetricsScreen.cs`
- `src/SquadTUI/Screens/MemberDetailScreen.cs`
- `src/SquadTUI/Screens/CharterScreen.cs`
- `src/SquadTUI/Program.cs`
- `tests/SquadTUI.Tests/E2E/TestAppBuilder.cs`
- `tests/SquadTUI.Tests/E2E/ThemeSwitchingTests.cs`
- `tests/SquadTUI.Tests/E2E/AppNavigationTests.cs`
- `tests/SquadTUI.Tests/E2E/VimKeybindingTests.cs`
- `tests/SquadTUI.Tests/E2E/RosterScreenTests.cs`
- `tests/SquadTUI.Tests/E2E/ResponsiveLayoutTests.cs`
- `tests/SquadTUI.Tests/E2E/CharterNavigationTests.cs`
- `tests/SquadTUI.Tests/Unit/Rendering/PanelRendererTests.cs`
