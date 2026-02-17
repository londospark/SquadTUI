# SampleData Leak Audit — Patches

**Date:** 2026-02-19
**By:** Patches (Tester)
**Status:** Audit complete — critical leaks identified

---

## Summary

Every screen in `src/SquadTUI/Screens/` references `SampleData` as a fallback when `AppState` data is null. The design intent (per Andre's decision) is: "All screens updated to use real data with SampleData fallback." However, this creates a **silent data leak** — if the async data load in `Program.cs` fails or hasn't completed yet, users see fabricated Sonic/Tails/Knuckles data with no indication it's fake. There are also **hardcoded SampleData references** with no fallback path — data that *always* comes from SampleData regardless of real data availability.

---

## File-by-File SampleData References (Production Code Only)

### 1. `DashboardScreen.cs` (lines 19-20, 26-27) — FALLBACK
```csharp
var members = state.Members ?? SampleData.Members;
var tasks = state.Tasks ?? SampleData.Tasks;
var logEntries = state.LogEntries ?? SampleData.LogEntries;
var decisions = state.Decisions ?? SampleData.Decisions;
```
**Severity:** Medium. Falls back silently — no visual indicator that sample data is shown.

### 2. `RosterScreen.cs` (lines 12, 23, 25, 27) — FALLBACK + HARDCODED
```csharp
var members = state.Members ?? SampleData.Members;          // fallback
var charter = SampleData.GetCharterFor(selected.Name);      // ALWAYS sample
var allTasks = state.Tasks ?? SampleData.Tasks;             // fallback
var logs = state.LogEntries ?? SampleData.LogEntries;       // fallback
```
**Severity:** HIGH. `GetCharterFor()` always uses SampleData — never reads real charter files. This means even when real data loads, the charter excerpt in the roster detail panel is always fabricated Sonic-universe text.

### 3. `MemberDetailScreen.cs` (lines 13, 16-17, 19) — FALLBACK + HARDCODED
```csharp
var members = state.Members ?? SampleData.Members;          // fallback
var charter = SampleData.GetCharterFor(member.Name);        // ALWAYS sample
var allTasks = state.Tasks ?? SampleData.Tasks;             // fallback
var logs = state.LogEntries ?? SampleData.LogEntries;       // fallback
```
**Severity:** HIGH. Same issue — `GetCharterFor()` always returns Sonic-themed hardcoded strings. Real charters from `.ai-team/agents/*/charter.md` are never displayed.

### 4. `CharterScreen.cs` (line 13) — HARDCODED
```csharp
var charter = SampleData.GetCharterFor(memberName);
```
**Severity:** CRITICAL. The entire Charter screen only ever shows SampleData. `DataBridge.LoadCharterContentAsync()` exists and reads real files, but is never called by any screen.

### 5. `DecisionsScreen.cs` (line 12) — FALLBACK
```csharp
var decisions = state.Decisions ?? SampleData.Decisions;
```
**Severity:** Medium.

### 6. `ActivityLogScreen.cs` (line 12) — FALLBACK
```csharp
var logs = state.LogEntries ?? SampleData.LogEntries;
```
**Severity:** Medium.

### 7. `SkillsScreen.cs` (lines 12, 23) — FALLBACK
```csharp
var skills = state.Skills ?? SampleData.Skills;
var members = state.Members ?? SampleData.Members;
```
**Severity:** Medium.

### 8. `MetricsScreen.cs` (lines 20-22, 54, 72-74, 130-132, 160-161) — HARDCODED + FALLBACK
```csharp
var sprints = SampleData.SprintHistory;                     // ALWAYS sample
var tasks = state.Tasks ?? SampleData.Tasks;                // fallback
var members = state.Members ?? SampleData.Members;          // fallback
// ... plus 9 more direct references to SampleData.OverallCompletionRate,
// SampleData.AverageVelocity, SampleData.VelocityTrend across all 3 layouts
```
**Severity:** CRITICAL. `SprintHistory` is hardcoded — there is no service that provides real sprint data. All computed metrics (completion rate, velocity, trend) are derived from fake data. The Metrics screen is 100% fabricated.

---

## Classification

### Always-SampleData (no real data path exists)
| Item | Files | Impact |
|------|-------|--------|
| `SampleData.GetCharterFor()` | RosterScreen, MemberDetailScreen, CharterScreen | Charter content is always fake |
| `SampleData.SprintHistory` | MetricsScreen | Sprint metrics are always fake |
| `SampleData.OverallCompletionRate` | MetricsScreen (×3 layouts) | Always fake |
| `SampleData.AverageVelocity` | MetricsScreen (×3 layouts) | Always fake |
| `SampleData.VelocityTrend` | MetricsScreen (×4 refs) | Always fake |

### Silent Fallback (shows fake data when real data hasn't loaded)
| Item | Files |
|------|-------|
| `state.Members ?? SampleData.Members` | Dashboard, Roster, MemberDetail, Metrics, Skills |
| `state.Tasks ?? SampleData.Tasks` | Dashboard, Roster, MemberDetail, Metrics |
| `state.Decisions ?? SampleData.Decisions` | Dashboard, Decisions |
| `state.LogEntries ?? SampleData.LogEntries` | Dashboard, ActivityLog, Roster, MemberDetail |
| `state.Skills ?? SampleData.Skills` | Skills |

---

## Recommendations

1. **Add loading indicator.** When `state.IsLoading == true`, show a spinner or "Loading…" instead of silently falling back to SampleData. The `IsLoading` flag exists but is never checked by any screen.

2. **Wire up real charter loading.** `DataBridge.LoadCharterContentAsync(memberName)` exists but is never called. CharterScreen, MemberDetailScreen, and RosterScreen should use it.

3. **Mark MetricsScreen as preview/demo.** Sprint velocity data has no real data source — no service parses sprint history from files. Until a `SprintMetricsService` exists, MetricsScreen should show a "Demo Data" badge.

4. **Add SampleData leak detection tests.** E2E tests should verify that when real data loads, no Sonic/Tails/Knuckles names appear. When AppState has data populated, SampleData should not be reachable.

5. **Default member name fallback.** `MemberDetailScreen` line 12: `state.SelectedMemberName ?? "Sonic"` — if no member is selected, it defaults to "Sonic" which is a SampleData character, not a real team member. Should default to first member from `state.Members`.

---

## UX_DESIGN.md Reference (line 665)
`src/SquadTUI/Screens/UX_DESIGN.md` also references SampleData:
> `src/SquadTUI/Screens/SampleData.cs` — Mock team, task, decision data

This is documentation, not a leak — but should be updated when SampleData is properly isolated.
