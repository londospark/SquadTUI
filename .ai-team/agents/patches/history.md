# Project Context

- **Owner:** LondoSpark (ridecar2@gmail.com)
- **Project:** SquadTUI — A terminal user interface built with Hex1b (.NET 10) for managing AI squads. Features include viewing squad activity, inspecting individual members, reading/editing charters, tracking sprint velocities, and more.
- **Stack:** C#, .NET 10, Hex1b TUI framework (https://hex1b.dev/)
- **Created:** 2026-02-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-02-17: Test Suite Created — 110 tests (Unit + Integration + E2E)

**Testing patterns that worked:**
- **Unit tests** use `IDisposable` with temp directories for service isolation. Each test creates a fresh temp dir with `.ai-team/` structure, writes specific markdown files, then runs service methods against them. This keeps tests fast, independent, and deterministic.
- **Integration tests** use fixture files copied to output directory via `<Content Include="Fixtures\**\*" CopyToOutputDirectory="PreserveNewest" />` in the csproj. The `AppContext.BaseDirectory` resolves the fixtures path at runtime.
- **NSubstitute** mocks for `SquadDataProvider` tests — substituting `ITeamService`, `IDecisionService`, etc. lets us test aggregation logic without file I/O.
- **E2E tests** recreate the full app layout from `Program.cs` in a `TestAppBuilder` helper, using the Hex1b headless terminal.

**Hex1b Testing API discoveries:**
- The correct builder class is `Hex1bTerminalInputSequenceBuilder` (in `Hex1b.Automation` namespace), NOT `Hex1bInputSequenceBuilder`.
- Terminal inspection uses `terminal.CreateSnapshot()` which returns a `Hex1bTerminalSnapshot`. Extension methods like `ContainsText()`, `GetText()`, `GetLine()`, `GetNonEmptyLines()` are on `IHex1bTerminalRegion` via `Hex1bTerminalRegionExtensions`.
- Headless mode: `Hex1bTerminal.CreateBuilder().WithHex1bApp(...).WithHeadless().WithDimensions(w, h).Build()`.
- App lifecycle: `RunAsync(ct)` + `CancellationTokenSource` for clean shutdown. `await Task.Delay(200)` is sufficient for rendering between input steps.
- `WaitUntil(predicate, timeout)` is available on the builder for waiting for async rendering.
- The terminal is `IAsyncDisposable` — use `await using`.

**Package versions used:**
- xunit 2.9.3
- xunit.runner.visualstudio 3.1.4
- Microsoft.NET.Test.Sdk 17.14.1
- coverlet.collector 6.0.4
- FluentAssertions 8.8.0
- NSubstitute 5.3.0
- Hex1b 0.87.0

### 2026-02-17: Test Suite Expanded for New Features

**New test files created:**
- `tests/SquadTUI.Tests/Unit/ThemeTests.cs` — Tests for ThemeManager (3 themes: Ocean, Heist, Daylight). Validates all themes have required properties, index clamping works correctly, and theme retrieval by index.
- `tests/SquadTUI.Tests/Unit/MarkdownRendererTests.cs` — Placeholder tests for MarkdownRenderer. Actual rendering tests deferred to E2E since MarkdownRenderer.Render() requires WidgetContext from Hex1b render pipeline.
- `tests/SquadTUI.Tests/Unit/DataBridgeTests.cs` — Tests for DataBridge service aggregation layer. Uses NSubstitute to mock ServiceProvider and underlying services. Tests all data loading methods and charter content loading with error handling.
- `tests/SquadTUI.Tests/E2E/ThemeSwitchingTests.cs` — E2E tests for theme cycling with T key.
- `tests/SquadTUI.Tests/E2E/ResponsiveLayoutTests.cs` — Tests responsive behavior at different terminal dimensions (wide: 120x40, narrow: 80x24, small: 60x20).
- `tests/SquadTUI.Tests/E2E/MarkdownRenderingTests.cs` — E2E tests verifying markdown renders with bullet points, headings, etc. in charter and decisions screens.
- `tests/SquadTUI.Tests/Integration/CIPipelineTests.cs` — CI validation tests that check workflow YAML exists, is valid, contains build/test steps, and that `dotnet build` and `dotnet test` succeed.

**Implementation discoveries:**
- **ThemeManager** uses index-based theme selection (0-2), not string-based keys. Three themes: Ocean (default), Heist, Daylight.
- **ThemeManager.GetTheme(index)** clamps out-of-bounds indices to valid range, making it safe to cycle indefinitely.
- **Hex1bTheme** objects from Hex1b.Theming namespace include color properties (DefaultForeground/Background, Primary, Secondary, Accent, Border, InfoBar) plus ListTheme, ScrollTheme, SplitterTheme.
- **DataBridge** depends on ServiceProvider singleton, which aggregates ITeamService, IDecisionService, ISkillService, IOrchestrationLogService, ISquadDataProvider.
- **ServiceProvider** is a singleton that auto-discovers `.ai-team` directory via git root or directory walk-up.
- **MarkdownRenderer** is a static class with `Render(WidgetContext ctx, string markdown, int availableWidth)` that directly returns Hex1bWidget arrays — not an intermediate parsed structure. Testing requires full Hex1b context, so E2E tests cover markdown rendering behavior.

**Test strategy notes:**
- Unit tests for ThemeManager cover all three themes, index clamping, and property validation.
- DataBridge tests mock ServiceProvider with NSubstitute since it's a sealed class with internal constructor — mock the interfaces instead.
- MarkdownRenderer has minimal unit tests (class/method existence checks) since rendering requires WidgetContext. Behavior validated via E2E tests that inspect terminal snapshots for bullet points, headings, etc.
- E2E tests updated TestAppBuilder with comments showing where theme and notification support will be integrated when those features are complete.
- CI pipeline tests validate GitHub Actions workflow structure and can run `dotnet build` and `dotnet test` as sanity checks.

**Current blocker:**
- MarkdownRenderer.cs has 5 compilation errors (WidgetContext generic type issue, Hex1bColor not found). This blocks `dotnet build` and prevents running any tests. These errors are in Siegmeyer's implementation and need fixing before test suite can execute. Once fixed, all new tests should pass.

📌 Team update (2026-02-16): Siegmeyer completed Phase 1 UI modernization adding emoji icons to all screens (🏠 🧑‍💼 📋 🔧 📊 📈), status badges, and task indicators. Full theme system, responsive layouts, TabPanel, VScroll, and markdown rendering deferred pending Hex1b API docs — decided by Siegmeyer

### 2026-02-17: Responsive Layout and Theme Switching E2E Tests

**What was added:**
- `tests/SquadTUI.Tests/E2E/ResponsiveLayoutTests.cs` — 6 tests validating dashboard responsive behavior at all breakpoints (40, 60, 80, 100, 120, 160 cols)
- `tests/SquadTUI.Tests/E2E/ThemeSwitchingTests.cs` — 3 tests validating theme cycling with T key (Ocean → Heist → Sunset → HighContrast → Ocean)

**Responsive breakpoints tested:**
- **Narrow (<80 cols)**: Single-column layout with "🏠 Dashboard" panel — tested at 40 and 60 cols
- **Medium (≥80 cols)**: Two-column layout with "🏠 Dashboard" + "📋 Recent" — tested at 80 and 100 cols
- **Wide (≥120 cols)**: Three-column layout with "🏠 Team" + "📊 Activity" + "📈 Summary" — tested at 120 and 160 cols

**Test patterns:**
- Each responsive test creates headless terminal at specific width using `TestAppBuilder.Build(width: X, height: 30)`
- Tests assert correct panel titles appear for that breakpoint
- Tests assert incorrect panel titles (from other breakpoints) are absent
- Must check full panel titles like "🏠 Team" not just "Team" to avoid false positives from content text like "Team Members:"

**Theme switching patterns:**
- Use `Hex1bTerminalInputSequenceBuilder().Key(Hex1bKey.T).Build()` to create input sequence
- Apply sequence with `await sequence.ApplyAsync(terminal)` followed by `await Task.Delay(200)`
- Inspect snapshot for theme name in info bar like "🎨 Ocean", "🎨 Heist", etc.

**Bug fixed:**
- DecisionService.cs had duplicate `content` variable names in nested scopes causing CS0136 compilation errors. Renamed to `builtContent` and `finalContent` to avoid shadowing.

**Current test count:** 29 tests total (26 existing + 6 responsive + 3 theme switching) — all passing ✅

📌 Team recast (2026-02-18): Squad recast from Ocean's Eleven to Dark Souls universe. Basher is now Patches. Praise the sun! ☀️

### 2026-02-18: Corner Case & Regression Test Suite — 281 tests total

**What was added (60 new tests):**

**New test files:**
- `tests/SquadTUI.Tests/E2E/ExtremeWidthTests.cs` — 9 E2E tests for extreme terminal dimensions: impossibly narrow (10 cols), minimum viable (20 cols), narrow (30 cols), wide (200 cols), ultra-wide (300 cols), short (5 rows), small (10 rows), tall (50 rows), very tall (100 rows). Validates app doesn't crash at any extreme size.
- `tests/SquadTUI.Tests/E2E/NavigationEdgeCaseTests.cs` — 10 E2E tests: Escape on Dashboard stays put, rapid tab switching (1→2→3→4→5→6), H on first screen no-op, L on last screen no-op, J/K on Dashboard and Metrics no-op, E on Dashboard/Roster no-op, rapid back-and-forth switching.
- `tests/SquadTUI.Tests/E2E/NoSquadScreenTests.cs` — 1 E2E test rendering NoSquad screen with SquadDetected=false, verifying welcome message and "No squad detected" text.
- `tests/SquadTUI.Tests/Unit/SampleDataTests.cs` — 16 unit tests: all Task.Assignee values match a Member.Name, all LogEntry participants exist in Members, GetCharterFor returns non-empty for each member, no null member names/roles, tasks have unique IDs, decisions have dates and authors, all collections non-empty, skills have names/descriptions, unknown member gets fallback charter.

**Updated test files:**
- `AppStateTests.cs` — +8 tests for index bounds: RosterSelectedIndex clamps to 0 when empty, DecisionSelectedIndex doesn't go negative, SkillSelectedIndex clamps at boundary, SettingsSelectedIndex stays 0–4, LogSelectedIndex clamps both ends, all defaults zero, full range navigation.
- `ThemeManagerTests.cs` — +8 tests for edge cases: negative index doesn't crash, very large index (100/1000/MaxValue) wraps correctly, PanelColors valid for all indices, large index PanelColors wraps to equivalent, CreateSunsetTheme/CreateHighContrastTheme return non-null, all theme names unique.

**Fixed 18 pre-existing test regressions:**
Dashboard was redesigned (by Siegmeyer) to use new panel titles ("👥 Team Roster", "📊 Activity & Progress", "📈 Sprint Metrics") and responsive layout. Old tests asserted `ContainsText("Members:")` which no longer appears at default 120-col width. Fixed all 18 tests across 8 files to use resilient assertions matching current dashboard output.

**Test patterns used:**
- For "no crash" tests: assert `snapshot.Should().NotBeNull()` — validates terminal renders without throwing.
- For "stays on same screen" tests: assert presence of current screen's unique text after input.
- For resilient assertions: use generic text like "Dashboard", "Team Roster" instead of specific member names.
- NoSquadScreen test builds custom AppState with `SquadDetected = false` and `CurrentScreen = Screen.NoSquad`.
- ThemeManager negative index test uses `Should().NotThrow()` since C# modulo can return negative.

**Current test count:** 281 tests total (221 existing + 60 new) — all passing ✅

### 2026-02-18: Tests for Issues #25, #26, #27 — 326 tests total

**What was added (45 new tests):**

**New test files:**
- `tests/SquadTUI.Tests/E2E/TabStyleTests.cs` — 12 E2E tests: tab bar renders with labels at 200 cols, active tab has indicator, pressing D1-D6 switches screens (5 parametrized), tab bar renders at various widths (4 parametrized: 40/80/120/200), tab labels include text, switching back updates indicator.
- `tests/SquadTUI.Tests/Unit/ThemeBackgroundTests.cs` — 18 unit tests: each theme has background color (4), all themes have distinct backgrounds, each theme's divider differs from background (4), each theme's divider is not pure black (4), all themes have distinct dividers, theme names match expected (4).
- `tests/SquadTUI.Tests/E2E/NoSquadGuardTests.cs` — 10 E2E tests: pressing D1-D6 on NoSquad screen doesn't navigate away (6 parametrized), pressing S doesn't go to Settings, pressing C navigates to Dashboard, pressing C creates .ai-team directory structure. Uses temp directories for file creation tests.

**Updated test files:**
- `tests/SquadTUI.Tests/E2E/NoSquadScreenTests.cs` — Rewritten from 1 test to 6 tests: shows welcome message, shows "No squad detected" text, contains setup instructions (npx/squad/.ai-team), shows C key instruction, shows Q key instruction, NavBar not visible on NoSquad (verified by pressing D1).
- `tests/SquadTUI.Tests/E2E/TestAppBuilder.cs` — Added `bool squadDetected = true` parameter to `Build()`. When `false`, sets `state.SquadDetected = false` and `state.CurrentScreen = Screen.NoSquad`. Default `true` preserves backward compat with all existing tests.

**Source file fixes (needed for compilation):**
- `src/SquadTUI/Screens/NavBar.cs` — Fixed CS0826 by explicit `Hex1bWidget[]` array type (Firekeeper's in-progress change had implicit array).
- `src/SquadTUI/Program.cs` — Fixed `...` (triple-dot) to `..` (double-dot spread operator) for C# collection expression syntax.

**Testing patterns:**
- Tab tests use wider terminals (200 cols) when asserting all tab labels, since styled tabs with padding may truncate at 120 cols.
- NoSquad guard tests use `TestAppBuilder.Build(squadDetected: false)` to start on NoSquad screen.
- File creation tests (C key) use temp directories with `Guid.NewGuid()` and restore `Directory.GetCurrentDirectory()` in finally blocks.
- Theme background tests use `Hex1bTheme.Get(GlobalTheme.BackgroundColor)` and `Hex1bColor.ToBackgroundAnsi()` to compare theme colors as ANSI strings.

**Current test count:** 326 tests total (281 existing + 45 new) — all passing ✅

