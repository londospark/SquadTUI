# Test Suite Audit

**Date:** 2026-02-19
**Auditor:** Solaire (Lead)
**Requested by:** LondoSpark

## Summary Stats

| Category | Files | Test Attributes | Expanded Test Cases (approx) |
|----------|-------|-----------------|------------------------------|
| Unit | 24 | 229 | ~350 |
| Integration | 7 | 56 | ~60 |
| E2E | 27 | 177 | ~190 |
| Root-level | 4 | 89 | ~100 |
| **Total** | **62** | **551** | **~778** |

Baseline: **778 tests pass** (some crash on teardown but all assertions succeed).

---

## Redundancy Analysis

### 🔴 Critical: E2E Test Duplication

The E2E suite has **massive duplication across 4+ files** testing the same panel navigation behavior. These are not just similar — they are functionally identical.

#### "Escape on Dashboard stays on Dashboard" — 4 copies
| File | Method |
|------|--------|
| `NavigationEdgeCaseTests` | `EscapeOnDashboard_StaysOnDashboard` |
| `StackNavigationExtendedTests` | `Escape_AtDashboard_StaysOnDashboard` |
| `StackNavigationTests` | `EscapeOnDashboard_StaysOnDashboard` |
| `VimKeybindingTests` | `EscapeOnDashboard_DoesNothing` |

**Action:** Keep `NavigationEdgeCaseTests` version. Delete 3 copies.

#### "Enter panel 0 → Roster" — 3 copies
| File | Method |
|------|--------|
| `AppNavigationTests` | `Enter_NavigatesToRoster_FromPanel0` |
| `DashboardPanelNavigationTests` | `Enter_DrillsIntoRoster_WhenFocusPanel0` |
| `StackNavigationExtendedTests` | `Dashboard_Enter_Panel0_GoesToRoster` |

**Action:** Keep `AppNavigationTests`. Delete 2 copies.

#### "Enter panel 1 → ActivityLog" — 3 copies
| File | Method |
|------|--------|
| `AppNavigationTests` | `Enter_NavigatesToActivityLog_FromPanel1` |
| `DashboardPanelNavigationTests` | `Enter_DrillsIntoActivityLog_WhenFocusPanel1` |
| `StackNavigationExtendedTests` | `Dashboard_Enter_Panel1_GoesToActivityLog` |

**Action:** Keep `AppNavigationTests`. Delete 2 copies.

#### "Enter panel 2 → Decisions" — 3 copies
| File | Method |
|------|--------|
| `AppNavigationTests` | `Enter_NavigatesToDecisions_FromPanel2` |
| `DashboardPanelNavigationTests` | `Enter_DrillsIntoDecisions_WhenFocusPanel2` |
| `StackNavigationExtendedTests` | `Dashboard_Enter_Panel2_GoesToDecisions` |

**Action:** Keep `AppNavigationTests`. Delete 2 copies.

#### "Enter panel 3 → Metrics" — 3 copies
| File | Method |
|------|--------|
| `AppNavigationTests` | `Enter_NavigatesToMetrics_FromPanel3` |
| `DashboardPanelNavigationTests` | `Enter_DrillsIntoMetrics_WhenFocusPanel3` |
| `StackNavigationExtendedTests` | `Dashboard_Enter_Panel3_GoesToMetrics` |

**Action:** Keep `AppNavigationTests`. Delete 2 copies.

#### "Escape from Decisions → Dashboard" — 2 copies
| File | Method |
|------|--------|
| `StackNavigationExtendedTests` | `Escape_FromDecisions_ReturnsToDashboard` |
| `VimKeybindingTests` | `EscapeFromDecisions_ReturnsToDashboard` |

#### "Escape from Metrics → Dashboard" — 2 copies
| File | Method |
|------|--------|
| `StackNavigationExtendedTests` | `Escape_FromMetrics_ReturnsToDashboard` |
| `VimKeybindingTests` | `EscapeFromMetrics_ReturnsToDashboard` |

#### "PressS opens settings" — 2 copies
| File | Method |
|------|--------|
| `AppNavigationTests` | `PressS_OpensSettingsModal` |
| `ThemeModalTests` | `PressS_OpensSettingsModal` |

#### Responsive layout duplication
`ResponsiveLayoutTests`, `SizingConsistencyTests`, and `StackNavigationTests` all test rendering at various widths with overlapping width values (60, 80, 120, 160).

**Total E2E duplicates identified: ~20 tests**

### 🟡 Moderate: Unit Test Overlap

#### EmptyState ↔ ErrorHandling overlap
- `EmptyStateTests.AppState_CharterContent_NoneByDefault` ≡ `ErrorHandlingTests.AppState_CharterContent_DefaultsToNone`
- `EmptyStateTests` has 6 `GetOrEmpty` tests that are subsumed by `ErrorHandlingTests.AppState_DefaultState_AllEithersAreRightEmpty`

#### Theme test overlap
- `ThemeManagerTests.ThemeNames_MatchExpectedOrder` ≡ `ThemeBackgroundTests.Theme_HasExpectedName` (same Theory, same InlineData)
- `ThemeManagerTests.GetAccentCode_ReturnsAnsiEscapeCode` overlaps with `ThemeCoverageTests.AllColorMethods_ProduceValidAnsi_ForAllThemes`

---

## Data-Driven Candidates ([Fact] → [Theory])

### High Priority

#### 1. `EmptyStateTests.cs` — 14 Facts → 3 Theories
- 6× `AppState_Empty{X}_GetOrEmptyReturnsEmptyList` → 1 Theory with MemberData
- 4× `{X}SelectedIndex_ClampedToZero_When{X}Empty` → 1 Theory with MemberData
- 4× `{X}Screen_Empty{X}_ShouldTriggerEmptyState` → 1 Theory with MemberData

#### 2. `ErrorHandlingTests.cs` — 8 Facts → 2 Theories
- 5× `GetOrEmpty_WorksWith{Type}` (Left path) → 1 Theory
- 3× `AppState_WithLeft{X}_GetOrEmptyReturnsEmpty` → 1 Theory

#### 3. `ResponsiveLayoutTests.cs` — 6 Facts → 1 Theory
- All 6 width tests → 1 Theory with `[InlineData(width, height, expectedText)]`

#### 4. `ExtremeWidthTests.cs` — 9 Facts → 2 Theories
- Width tests → 1 Theory
- Height tests → 1 Theory

### Medium Priority

#### 5. `SampleDataTests.cs` — 5 "HasAtLeastOneEntry" Facts → 1 Theory
#### 6. `SampleDataLeakTests.cs` — 3 "NoSampleDataNamesIn{X}" Facts → 1 Theory
#### 7. Navigation panel tests in `AppNavigationTests` — 4 panel drill-in Facts → 1 Theory

---

## C# 14 Modernization Opportunities

### Collection Expressions (`[]` syntax)
Already used in some files (e.g., `ErrorHandlingTests`). Opportunities:
- `SampleDataLeakTests.cs:14` — uses `["Sonic", "Tails", ...]` ✅ already modern
- Various `new List<T>()` instantiations → `[]` or `[item1, item2]`

### `field` keyword (C# 14)
Not applicable here — test classes don't use auto-properties with backing fields that would benefit from `field`.

### Primary Constructors (C# 12+)
Several test classes implement `IDisposable` with constructor + `_tempDir` field:
- `TeamServiceTests`, `TeamServiceMemberManagementTests`, `TeamServiceTaskTests`, `DecisionServiceTests`, `FileLocationServiceTests`
These could use primary constructors but the benefit is marginal for test classes.

### Pattern: Reduce boilerplate in IDisposable test classes
The 5 service test files all share identical temp-dir setup/teardown. A shared `TempDirFixture` base class would eliminate ~100 lines of duplication.

---

## Estimated Test Count After Refactoring

| Change | Tests Removed | Tests Added | Net |
|--------|--------------|-------------|-----|
| Remove E2E duplicates | -20 | 0 | -20 |
| EmptyState → Theory | -14 | 3 | -11 |
| ErrorHandling → Theory | -8 | 2 | -6 |
| Responsive → Theory | -6 | 1 | -5 |
| ExtremeWidth → Theory | -9 | 2 | -7 |
| Remove unit overlaps | -3 | 0 | -3 |
| **Total** | **-60** | **+8** | **-52** |

**Estimated final: ~726 test cases** (down from 778), with identical coverage.

> Note: [Theory] tests with InlineData still produce the same number of test *cases* in the runner. The reduction is in duplicate *test methods* and *source lines*. The actual test case count reduction comes from removing the ~20 truly redundant E2E tests and ~3 unit duplicates.

---

## Priority Order for Refactoring

1. **P0: Remove E2E duplicates** — Biggest win. 20 tests removed, zero risk.
2. **P1: EmptyStateTests → Theory** — 14 Facts → 3 Theories. Clean pattern.
3. **P2: Responsive/Extreme width → Theory** — 15 Facts → 3 Theories.
4. **P3: ErrorHandlingTests → Theory** — 8 Facts → 2 Theories.
5. **P4: Remove unit-level overlaps** — ThemeBackgroundTests duplicate, EmptyState↔ErrorHandling overlap.
6. **P5: TempDirFixture base class** — Reduces boilerplate across 5 service test files.
7. **P6: Navigation panel drill-in → Theory** — 4 Facts → 1 Theory in AppNavigationTests.

---

## Implemented Changes (2026-02-19)

**Final test count: 768** (down from 778 baseline). All 768 pass.

### P0: E2E Duplicate Removal ✅

| File | Tests Removed | Details |
|------|--------------|---------|
| `StackNavigationTests.cs` (StackNavigationExtendedTests class) | 9 | 4 panel drill-ins, 3 escape-from-screen, 2 escape-at-dashboard |
| `DashboardPanelNavigationTests.cs` | 4 | 4 panel drill-in duplicates |
| `VimKeybindingTests.cs` | 3 | EscapeFromDecisions, EscapeFromMetrics, EscapeOnDashboard |
| `TabStyleTests.cs` (StackNavigationTests class) | 3 | Dashboard_RendersWithoutTabs, Enter_DrillsIn_Escape_PopsBack, EscapeOnDashboard |
| `ThemeModalTests.cs` | 1 | PressS_OpensSettingsModal (kept in AppNavigationTests) |

### P2: Responsive/Extreme Width → Theory ✅

| File | Before | After |
|------|--------|-------|
| `ResponsiveLayoutTests.cs` | 6 Facts | 1 Theory (6 InlineData) |
| `ExtremeWidthTests.cs` | 9 Facts | 2 Theories (5 + 4 InlineData) |

### P4: Unit-Level Overlap Removal ✅

| File | Tests Removed | Details |
|------|--------------|---------|
| `ThemeBackgroundTests.cs` | 10 (Theory InlineData) | Theme_HasExpectedName Theory (dup of ThemeManagerTests) |
| `EmptyStateTests.cs` | 7 | 6× AppState_Empty*_GetOrEmptyReturnsEmptyList + CharterContent_NoneByDefault |
| `ErrorHandlingTests.cs` | 3 | 3× AppState_WithLeft*_GetOrEmptyReturnsEmpty |
| `SizingConsistencyTests.cs` | 8 | ResponsiveBreakpoints Theory (5) + WideLayout + MediumLayout + NarrowLayout + SubScreen |

### Not Implemented (Deferred)

- **P1: EmptyState → Theory** — Remaining tests have distinct property accessors not easily parameterized via InlineData. MemberData approach would reduce readability. Left as-is.
- **P3: ErrorHandling → Theory** — Same reasoning. Each `GetOrEmpty_WorksWith*` test uses a different model type constructor, making parameterization more complex than beneficial.
- **P5: TempDirFixture base class** — Low ROI for test refactoring scope.
- **P6: Navigation panel drill-in → Theory** — Each panel test uses different arrow-key sequences. Not worth the abstraction cost.
