# Decision: SampleData Isolation from Production Code

**Date:** 2026-02-17
**By:** Solaire (Architecture Review Ceremony)
**Participants:** Firekeeper, Andre

## Context

`SampleData.cs` in `src/SquadTUI/Screens/` is referenced by 7 production screens as fallback data via `state.X ?? SampleData.X` pattern. This means fake data ships in the production binary and can silently mask data loading failures.

### Full SampleData Reference Audit

| File | References | Pattern |
|------|-----------|---------|
| `DashboardScreen.cs` | 4 | `state.Members ?? SampleData.Members`, `.Tasks`, `.LogEntries`, `.Decisions` |
| `RosterScreen.cs` | 4 | `state.Members`, `SampleData.GetCharterFor()`, `.Tasks`, `.LogEntries` |
| `MemberDetailScreen.cs` | 4 | `state.Members`, `SampleData.GetCharterFor()`, `.Tasks`, `.LogEntries` |
| `CharterScreen.cs` | 1 | `SampleData.GetCharterFor()` — **hardcoded, no service fallback** |
| `MetricsScreen.cs` | 9 | `SampleData.SprintHistory` (hardcoded), `.Tasks`, `.Members`, `.OverallCompletionRate`, `.AverageVelocity`, `.VelocityTrend` — **heaviest user** |
| `DecisionsScreen.cs` | 1 | `state.Decisions ?? SampleData.Decisions` |
| `ActivityLogScreen.cs` | 1 | `state.LogEntries ?? SampleData.LogEntries` |
| `SkillsScreen.cs` | 2 | `state.Skills`, `state.Members` with SampleData fallback |

### Critical Findings

1. **MetricsScreen is 100% SampleData for sprint history.** `SampleData.SprintHistory`, `.OverallCompletionRate`, `.AverageVelocity`, `.VelocityTrend` are used directly — no service loads sprint data at all. No `state.SprintHistory` property exists in `AppState`.
2. **CharterScreen and charter rendering always use `SampleData.GetCharterFor()`** even though `DataBridge.LoadCharterContentAsync()` exists and reads real charter files. The service result is never consumed by any screen.
3. **SampleData names are Sonic-themed** (Sonic, Tails, Knuckles, Amy, Shadow, Eggman) which will look bizarre to end users if data loading fails silently.

## Decision

**Move SampleData.cs entirely to the test project.** Replace all fallback patterns with explicit empty-state handling.

### Architecture

1. **Add to `AppState`:**
   - `IReadOnlyList<SprintMetrics>? SprintHistory` — loaded from a new `ISprintService` or from DataBridge
   - `string? CharterContent` — loaded via `DataBridge.LoadCharterContentAsync()`

2. **Replace `state.X ?? SampleData.X` with `state.X ?? []`:**
   - All collection fallbacks become empty list: `state.Members ?? []`
   - Screens must gracefully handle empty collections (show "No data loaded" placeholder)

3. **Replace `SampleData.GetCharterFor()` calls with `state.CharterContent`:**
   - DataBridge already has `LoadCharterContentAsync()` — wire it through AppState
   - Charter loads on member selection (lazy), not at startup

4. **MetricsScreen sprint data:**
   - Andre creates `ISprintService` to compute sprint metrics from log entries and tasks
   - Alternatively, compute derived metrics in DataBridge from existing data
   - Until sprint data is real, MetricsScreen shows "No sprint data available" instead of fake graphs

5. **SampleData.cs moves to `tests/SquadTUI.Tests/Fixtures/SampleData.cs`:**
   - Tests continue to use it for fixture data
   - No production code may reference it

### Empty State UX (Firekeeper)

Each screen gets a consistent empty-state widget when data is null/empty:
- Centered message: `"No {data type} found. Ensure your .squad/ directory contains the expected files."`
- Dim styling, non-alarming
- Dashboard shows skeleton panels with placeholder text during loading

## Consequences

- **Andre:** Create `ISprintService`, add `SprintHistory` and `CharterContent` to AppState, wire DataBridge, move SampleData to tests
- **Siegmeyer:** Update all 7 screens to use `?? []` pattern with empty-state widgets
- **Firekeeper:** Design the empty-state widget pattern
- **Patches:** Update tests that reference `SquadTUI.Screens.SampleData` namespace

## Risks

- MetricsScreen will be empty until real sprint data service exists — acceptable, fake data is worse
- Charter content loading adds async latency on member selection — mitigate with loading indicator
