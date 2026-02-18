# Decision: Navigation & Settings Audit — Sprint 18 Polish

**Author:** Patches (Tester)
**Date:** 2026-02-18
**Status:** Implemented

## Context

After migrating from TabPanel to stack-based navigation, the Skills screen became unreachable — no dashboard panel mapped to it, and no hotkey navigated there. Additionally, the Settings modal's Vim Keybindings and Mouse Support toggles had no runtime effect: j/k keys were always bound regardless of the VimBindings setting, and toggling Mouse Support never updated `options.EnableMouse`.

## Findings

### Task 1 — Skills Screen Unreachable

**Root cause:** The dashboard had 4 panels (Roster, Activity, Decisions, Metrics) mapped to panels 0–3. The Enter handler only mapped those 4 panels to screens. Skills had no panel and no navigation path from the Dashboard.

**Fix:** Added a Skills summary section as panel 4 in the dashboard's right column (wide and medium layouts). Updated all panel-count modular arithmetic from `% 4` to `% 5` across Tab, Shift+Tab, RightArrow, LeftArrow handlers. Added `4 => Screen.Skills` to the Enter handler's switch expression.

### Task 2 — Broken Settings Toggles

**Problem 1 — Vim Keybindings:** `BindKeys()` unconditionally bound j/k keys for list navigation. Since `BindKeys` is re-invoked each render cycle, wrapping the j/k bindings in `if (state.Settings.VimBindings)` makes the toggle effective immediately.

**Problem 2 — Mouse Support:** `options.EnableMouse` was hardcoded to `true` in `Program.cs` and never updated. Added `options.EnableMouse = settings.MouseEnabled` after toggling in both `ToggleSettingsModalItem` (AppLayout.cs) and `ToggleSetting` (SettingsScreen.cs).

## Changes

| File | Change |
|------|--------|
| `src/SquadTUI/Screens/DashboardScreen.cs` | Added Skills panel (panel 4) to wide and medium layouts |
| `src/SquadTUI/Screens/AppLayout.cs` | Panel count 4→5 in Tab/Arrow handlers; panel 4→Skills in Enter; j/k conditional on VimBindings; mouse toggle updates `options.EnableMouse` |
| `src/SquadTUI/Screens/SettingsScreen.cs` | Mouse toggle updates `options.EnableMouse` |
| `tests/SquadTUI.Tests/E2E/SkillsNavigationTests.cs` | 4 tests: drill into Skills, dashboard shows Skills, escape back, 5-panel wrap |
| `tests/SquadTUI.Tests/E2E/VimToggleTests.cs` | 2 tests: j/k inactive when VimBindings=false, active when true |
| `tests/SquadTUI.Tests/E2E/MouseToggleTests.cs` | 1 test: toggling mouse in settings modal updates `options.EnableMouse` |

## Test Results

- **Before:** 790 tests passing
- **After:** 797 tests passing (7 new, 0 regressions)

## Risks

- The narrow layout (< 80 cols) does not display a Skills panel due to space constraints. Users on very narrow terminals must use the medium/wide layout to access Skills. This is consistent with how Metrics is also absent from the narrow layout.
- Settings modal j/k keys remain unconditional (they are in `BindSettingsModalKeys`, separate from `BindKeys`). This is intentional — the modal always needs j/k for navigation regardless of vim mode.
