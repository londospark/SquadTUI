# History Archive — Patches (Tester)

## Entries from Feb 17 and Earlier

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

### 2026-02-19: Manual Testing Audit + Comprehensive Test Plan

**What was done:**
- Full audit of all 10 screen files in `src/SquadTUI/Screens/` for SampleData references, UI inconsistencies, and edge cases.
- Counted 180 tests across Unit/Integration/E2E categories via `dotnet test --list-tests`.
- Identified 2 CRITICAL SampleData leaks (CharterScreen and MetricsScreen always use hardcoded sample data with no real data path), 2 HIGH severity leaks (RosterScreen and MemberDetailScreen charter always from SampleData), and silent fallback patterns in all other screens.
- Found HelpScreen documents `?` key for toggling help but no `?` key binding exists — only F1 works.
- Found MemberDetailScreen shows member name twice (header + status line).
- Created comprehensive test plan covering 10 test areas with ~70+ specific test cases.
- Identified 9 untested production code files (DataBridge charter loading, SettingsService persistence, MigrationService, MarkdownRenderer.ExtractHeadings, etc.).
- Documented 8 user stories from tester's perspective.

**Key findings:**
1. `CharterScreen`, `RosterScreen`, `MemberDetailScreen` all call `SampleData.GetCharterFor()` — always returns hardcoded Sonic-universe text. `DataBridge.LoadCharterContentAsync()` exists but is never wired to any screen.
2. `MetricsScreen` uses `SampleData.SprintHistory` directly (not through AppState) — no service provides real sprint data. All velocity/completion/trend metrics are fabricated.
3. `state.IsLoading` and `state.ErrorMessage` are set in `Program.cs` but never read by any screen — no loading indicator, no error display.
4. `MemberDetailScreen` defaults `SelectedMemberName` to `"Sonic"` when null — a SampleData character name.
5. `SkillsScreen.GetConfidenceLevel()` returns hardcoded progress bars for 5 known SampleData skills only.

**Deliverables:**
- `.ai-team/decisions/inbox/patches-sampledata-audit.md` — Full file-by-file SampleData reference audit with severity classifications
- `.ai-team/decisions/inbox/patches-test-plan.md` — Comprehensive test plan with 10 sections covering high-risk areas, leak detection, responsive layouts, tab navigation, markdown rendering, migration, settings persistence, and future monadic types
