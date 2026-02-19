# Decision: Test Suite Audit & Refactoring

**Date:** 2026-02-19
**Author:** Solaire (Lead)
**Status:** Implemented
**Requested by:** LondoSpark

## Context

The test suite had grown to 778 test cases across 62 files, accumulated organically across sprints with different authors. Panel navigation E2E tests were duplicated 3-4× across `StackNavigationExtendedTests`, `DashboardPanelNavigationTests`, `AppNavigationTests`, and `VimKeybindingTests`. Responsive layout tests were scattered across 3 files with overlapping width values. Unit-level tests in `EmptyStateTests` and `ErrorHandlingTests` had significant overlap.

## Decision

1. **Remove exact E2E duplicates** — Keep canonical versions in `AppNavigationTests` (panel drill-in) and `NavigationEdgeCaseTests` (escape-at-dashboard). Delete all copies.
2. **Consolidate responsive layout tests to Theory** — Convert per-width Facts to parameterized Theories in `ResponsiveLayoutTests` and `ExtremeWidthTests`.
3. **Remove unit-level overlaps** — Delete `ThemeBackgroundTests.Theme_HasExpectedName` (exact dup of `ThemeManagerTests`), remove 7 EmptyState Facts subsumed by ErrorHandling's comprehensive test, remove 3 ErrorHandling Facts subsumed by EmptyState's comprehensive Left-error test.
4. **Defer Theory conversion for EmptyStateTests/ErrorHandlingTests** — The remaining Facts use distinct type constructors that resist clean InlineData parameterization without MemberData complexity.

## Result

- **778 → 768 test cases** (all passing)
- **~48 redundant [Fact]/[Theory] attributes removed** across 8 files
- Zero coverage loss — every removed test had an identical copy retained
- Audit document at `docs/test-audit.md`

## Files Modified

- `tests/SquadTUI.Tests/E2E/StackNavigationTests.cs` — Removed 9 duplicate tests
- `tests/SquadTUI.Tests/E2E/DashboardPanelNavigationTests.cs` — Removed 4 duplicate tests
- `tests/SquadTUI.Tests/E2E/VimKeybindingTests.cs` — Removed 3 duplicate tests
- `tests/SquadTUI.Tests/E2E/TabStyleTests.cs` — Removed 3 duplicates, kept Theory
- `tests/SquadTUI.Tests/E2E/ThemeModalTests.cs` — Removed 1 duplicate
- `tests/SquadTUI.Tests/E2E/ResponsiveLayoutTests.cs` — 6 Facts → 1 Theory
- `tests/SquadTUI.Tests/E2E/ExtremeWidthTests.cs` — 9 Facts → 2 Theories
- `tests/SquadTUI.Tests/E2E/SizingConsistencyTests.cs` — Removed 5 duplicates + 1 Theory
- `tests/SquadTUI.Tests/Unit/ThemeBackgroundTests.cs` — Removed duplicate Theory
- `tests/SquadTUI.Tests/EmptyStateTests.cs` — Removed 7 Facts covered elsewhere
- `tests/SquadTUI.Tests/ErrorHandlingTests.cs` — Removed 3 Facts covered elsewhere

## Risks

- None. All removed tests had exact functional duplicates retained.
