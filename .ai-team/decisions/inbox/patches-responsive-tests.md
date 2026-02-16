# Responsive Layout and Theme Switching E2E Tests

**By:** Patches  
**Date:** 2026-02-17  
**Status:** ✅ Implemented

## What

Created comprehensive E2E test coverage for responsive layout breakpoints and theme switching functionality:

### ResponsiveLayoutTests.cs
Tests validating the dashboard renders correctly at all responsive breakpoints:

1. **60 cols (narrow)** — Single-column layout with "🏠 Dashboard" panel showing all content stacked
2. **80 cols (medium)** — Two-column layout with "🏠 Dashboard" + "📋 Recent" panels
3. **100 cols (medium range)** — Still two-column layout (validates range behavior)
4. **120 cols (wide)** — Three-column layout with "🏠 Team" + "📊 Activity" + "📈 Summary" panels
5. **160 cols (extra-wide)** — Still three-column layout (validates upper range)
6. **40 cols (extremely narrow)** — Validates app doesn't crash at very small widths

Each test uses `TestAppBuilder.Build(width: X, height: 30)` to create headless terminal at specific width, then asserts:
- Correct panel titles appear for that breakpoint
- Incorrect panel titles (from other breakpoints) are absent
- Layout-specific content is present

### ThemeSwitchingTests.cs
Tests validating theme cycling with the T key:

1. **ThemeCycle_PressT_CyclesThroughThemes** — Validates pressing T cycles Ocean → Heist → Sunset
2. **ThemeCycle_PressT_WrapsAroundAfterLastTheme** — Validates cycling wraps back to Ocean after 4 presses
3. **InfoBar_AlwaysShowsCurrentTheme** — Validates info bar displays current theme name with 🎨 emoji

Tests use `Hex1bTerminalInputSequenceBuilder` with `.Key(Hex1bKey.T)` to simulate keypresses, then inspect snapshots for theme name changes.

## Why

**Test coverage gap:** Previously only tested at default 120-col width. Dashboard has 3 responsive breakpoints (wide ≥120, medium ≥80, narrow <80) that were untested. User explicitly requested E2E tests at all breakpoints to validate responsive behavior.

**Theme switching untested:** T key cycles through 4 themes (Ocean, Heist, Sunset, HighContrast) with wrapping. This UX feature had no automated validation.

**Prevents regressions:** Responsive layouts are notoriously fragile. E2E tests catch breakpoints breaking when screen logic changes. Theme cycling logic is simple but critical for user experience — tests ensure it works and wraps correctly.

## Bugs Fixed

**DecisionService.cs compilation errors:** Fixed duplicate variable name `content` in two scopes causing CS0136 errors. Renamed nested variables to `builtContent` and `finalContent` to avoid shadowing the outer `content` variable from line 17/26.

## Test Results

All 29 tests pass (26 existing + 3 new theme tests + 6 new responsive tests):
- ResponsiveLayoutTests: 6/6 ✅
- ThemeSwitchingTests: 3/3 ✅

## Consequences

- **Responsive breakpoints validated** — CI will catch if any breakpoint breaks
- **Theme switching validated** — CI ensures theme cycling and wrapping works
- **Regression prevention** — Layout changes that break responsive behavior will fail tests immediately
- **Documentation by test** — Tests serve as executable spec for responsive behavior at each width

## Technical Notes

**Hex1b API patterns discovered:**
- Use `.Key(Hex1bKey.X)` not `.Press()` on `Hex1bTerminalInputSequenceBuilder`
- Use `await sequence.ApplyAsync(terminal)` followed by `await Task.Delay(200)` for rendering
- Use `snapshot.ContainsText("🏠 Team")` not just `"Team"` — need full panel title with emoji to avoid false positives from content like "Team Members:"

**Test isolation:** Each test creates its own headless terminal with specific dimensions, runs app, takes snapshot, asserts, then cancels cleanly. No shared state between tests.
