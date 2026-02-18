# Architectural Vision — Sprint 18 & Beyond

**Author:** Solaire (Lead)
**Date:** 2025-01-27
**Status:** Proposal — for team review

---

## 1. What Users Actually Want

After reviewing the full codebase, all agent charters, the UX design doc, sprint directives, and the decisions log, here's what matters:

1. **Glanceable squad health** — open the TUI, see who's doing what in <2 seconds
2. **Drill-down without friction** — Dashboard → Member → Charter in 3 keystrokes
3. **Live updates that just work** — files change, the TUI reflects it, no manual refresh
4. **Settings that actually do something** — toggles that are wired, not cosmetic
5. **Confidence the tool is correct** — no phantom screens, no dead navigation paths

What they do NOT need right now: notifications, personal dashboards, kanban views, command palettes, or CI integrations. Those are Sprint 20+ at best. Scope discipline.

---

## 2. Current Architecture Assessment

### What's Working Well
- **Monadic data flow** — `Either<AppError, T>` throughout `AppState` is clean. Keep it.
- **`IFileLocationService`** — single source of truth for all paths. Well-designed.
- **Stack navigation** — `NavigationStack` in `AppState` is simple and correct.
- **Responsive layouts** — 3-tier responsive (120/80/narrow) is solid UX.
- **Hex1b fluent API** — widget composition is readable and composable.

### What Needs Work

| Issue | Severity | Sprint |
|-------|----------|--------|
| Settings toggles are cosmetic (VimBindings, Mouse) | High | 18 |
| Skills screen unreachable from navigation | High | 18 |
| Dashboard panels resize on focus change | Medium | 18 |
| Duplicate data-reload logic in 3 places (Program.cs) | Medium | 18-19 |
| No error recovery on service failures | Medium | 19 |
| `RedrawAfter(3000)` hardcoded in 2 screens | Low | 18 |
| No search/filter on any list screen | Low | 19-20 |

---

## 3. Architectural Proposals for Sprints 18–20

### 3.1 Extract a `RefreshService` (Sprint 18)

**Problem:** Data reload logic is copy-pasted three times in `Program.cs` — initial load (lines 26-41), FileWatcher callback (lines 47-64), and polling timer (lines 69-86). All three do the same `Task.WhenAll(members, tasks, decisions, logs)` dance.

**Proposal:** Extract an `IRefreshService` that owns the refresh lifecycle:

```csharp
public interface IRefreshService : IDisposable
{
    event Action? OnRefreshComplete;
    Task RefreshAllAsync(CancellationToken ct = default);
    void Start(RefreshMode mode, TimeSpan? interval = null);
    void Stop();
    RefreshMode CurrentMode { get; }
}

public enum RefreshMode { FileWatcher, Polling, Hybrid, Manual }
```

This unifies all three call sites, makes live-update strategy configurable, and eliminates the triple-copy problem. `Program.cs` drops to ~20 lines.

**Owner:** Andre (service layer) + Solaire (review)

### 3.2 Wire Settings to Runtime Behavior (Sprint 18)

**Problem:** `VimBindings` and `MouseEnabled` toggle in the UI and save to disk but have zero runtime effect:
- `BindKeys()` always registers j/k/h/l regardless of `VimBindings`
- `Program.cs` hardcodes `options.EnableMouse = true` regardless of `MouseEnabled`

**Proposal:** 
- `BindKeys()` checks `state.Settings.VimBindings` before registering j/k bindings
- `options.EnableMouse` reads from `state.Settings.MouseEnabled`
- Both take effect immediately on toggle (no restart required)

**Owner:** Siegmeyer (key bindings) + Patches (test the toggles)

### 3.3 Fix Navigation Graph (Sprint 18)

**Problem:** Skills screen exists but has no navigation path from Dashboard. The Dashboard panel-to-screen mapping is:
- Panel 0 → Roster ✓
- Panel 1 → ActivityLog ✓
- Panel 2 → Decisions ✓
- Panel 3 → Metrics ✓
- Skills → ??? (unreachable)

**Proposal:** Add a number-key scheme visible in the footer:
- `1` Dashboard, `2` Roster, `3` Decisions, `4` Skills, `5` Log, `6` Metrics

This matches the UX design doc's top nav spec exactly. Discoverable, fast, no collision with existing keys.

**Owner:** Firekeeper (design) + Siegmeyer (implementation) + Patches (audit)

### 3.4 Stabilize Panel Sizing (Sprint 18)

**Problem:** Dashboard panels resize when focus moves between them. The `PanelHeader` function changes formatting (highlight bg) based on focus, which may affect character width calculations.

**Proposal:** Ensure `FillWidth(n)` ratios are consistent across focused/unfocused states. The issue is likely ANSI escape codes in focused headers adding invisible characters that affect text measurement. Fix: use fixed-width containers or normalize padding.

**Owner:** Siegmeyer (widget layer) + Firekeeper (verify UX)

### 3.5 Introduce Search/Filter (Sprint 19-20)

**Problem:** No way to find a specific member, decision, or skill in growing lists.

**Proposal:** A `/` key opens a filter bar at the top of any list screen. Type to filter, Escape to clear. Incremental — no full search index needed.

```
State addition:
public Option<string> FilterText { get; set; } = None;
```

Screens apply `.Where(m => FilterText.Match(f => m.Name.Contains(f, OrdinalIgnoreCase), () => true))` inline. Zero new services. Pure UI concern.

**Owner:** Firekeeper (design) → Siegmeyer (implementation)
**Sprint:** 19 earliest. Not Sprint 18.

### 3.6 Error Recovery and Retry (Sprint 19)

**Problem:** When `DataBridge` catches an exception, it stores `Left<AppError>` in state. The UI shows nothing — no retry, no "press R to refresh" prompt. The user is stuck until the next polling cycle.

**Proposal:** Add an `ErrorBanner` component that screens render when their data source is `Left`. Include a `R` key binding for manual refresh. Small, self-contained.

**Owner:** Siegmeyer (banner widget) + Andre (retry in DataBridge)

---

## 4. Priority Matrix

### Sprint 18 — Polish (Current)

| # | Task | Value | Effort | Owner |
|---|------|-------|--------|-------|
| 1 | Wire VimBindings/Mouse toggles | High | Low | Siegmeyer + Patches |
| 2 | Fix navigation — Skills reachable | High | Low | Firekeeper + Siegmeyer |
| 3 | Fix panel sizing on focus | Medium | Medium | Siegmeyer |
| 4 | Extract RefreshService | Medium | Medium | Andre |
| 5 | Live update strategy decision | Medium | Low | Solaire (present options) |

### Sprint 19 — Reliability

| # | Task | Value | Effort | Owner |
|---|------|-------|--------|-------|
| 1 | Error recovery + retry | High | Medium | Andre + Siegmeyer |
| 2 | Search/filter on list screens | Medium | Medium | Firekeeper + Siegmeyer |
| 3 | Configurable refresh interval | Low | Low | Andre |

### Sprint 20 — Features

| # | Task | Value | Effort | Owner |
|---|------|-------|--------|-------|
| 1 | Command palette (Ctrl+P) | Medium | High | Firekeeper + Siegmeyer |
| 2 | Notification feed | Low | High | Deferred |
| 3 | Personal dashboard | Low | High | Deferred |

---

## 5. Abstractions We're Missing

1. **`IRefreshService`** — owns the entire data-refresh lifecycle. Eliminates triple-copy in Program.cs. Makes live-update strategy swappable.

2. **`INavigationService`** — the current `NavigationStack` in `AppState` works but has no validation. A service could enforce "Skills requires SquadDetected", "Charter requires SelectedMemberName set", etc. Sprint 19 candidate, not urgent.

3. **`FilterState`** — when we add search, it should be a first-class state object, not ad-hoc string fields scattered across AppState.

4. **`RefreshInterval` in `AppSettings`** — currently hardcoded at 30s in Program.cs and 3000ms in `RedrawAfter()`. Should be configurable. Trivial to add to the settings model.

---

## 6. What I'm NOT Proposing

- **DI container** — Not yet. `ServiceProvider` singleton works fine for this scale.
- **Plugin system** — Over-engineering for a single-purpose TUI.
- **Multi-squad support** — Interesting but not now. One squad at a time is the use case.
- **Kanban/board view** — The dashboard already shows task status. A board is a different mental model that adds complexity without clear user demand.
- **CI/CD integration** — Git hooks for live updates is a trap (see live-update options doc). Keep it simple.

---

## 7. Code Style Reminders

These apply to all Sprint 18+ work:

- `Either<AppError, T>` / `Option<T>` over exceptions — we're already doing this, keep it
- Expression-bodied members where the body is a single expression
- `IFileLocationService` for ALL path resolution — no `Path.Combine(squadRoot, ".ai-team")` in new code
- Query expressions (`from x in xs where ...`) preferred over method chains for complex LINQ
- No hardcoded data in the main app — `SampleData` fallback only in tests or demo mode

---

*Praise the sun. Let's ship a solid Sprint 18 and not go hollow.*
