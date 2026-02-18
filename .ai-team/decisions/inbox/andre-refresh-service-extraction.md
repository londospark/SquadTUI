# Decision: Extract IRefreshService — Option A Hybrid Implementation

**Date:** 2026-02-19
**By:** Andre
**Status:** Implemented

## What

Extracted a unified `IRefreshService` / `RefreshService` that consolidates the three independent refresh mechanisms in `Program.cs` into a single owner:

1. **`IRefreshService`** (`src/SquadTUI/Services/IRefreshService.cs`) — interface exposing `Start`, `Stop`, `RefreshNowAsync`, `SetPollingInterval`, `OnDataRefreshed` event, and `IsActive`/`LastRefreshTime` properties.
2. **`RefreshService`** (`src/SquadTUI/Services/RefreshService.cs`) — sealed implementation that owns both a `FileWatcherService` (reactive) and a `System.Threading.Timer` (polling fallback). Smart polling: if the watcher already fired since the last poll tick, the poll is skipped to avoid redundant reloads. A `SemaphoreSlim` prevents concurrent reloads. All data reload goes through a single `ReloadAllAsync()` method.
3. **`AppSettings.RefreshIntervalSeconds`** — new property (default 30) to make the polling interval configurable. Valid values: 15, 30, 60, 120.
4. **`Program.cs` simplified** — replaced ~40 lines (inline FileWatcher handler + inline Timer) with 3 lines instantiating and starting `RefreshService`. Initial startup load (`Task.Run`) is preserved as-is (startup ≠ refresh).

## Why

Three independent mechanisms all duplicated the same 4-call reload pattern (`LoadRosterDataAsync`, `LoadTasksFromRosterAsync`, `LoadDecisionsDataAsync`, `LoadLogDataAsync`). Any change to the reload set required editing 3 places. The new service provides:

- **Single reload path** — one method, one place to change
- **Smart polling** — watcher-aware timer avoids double-reloads
- **Concurrency safety** — `SemaphoreSlim` prevents overlapping reloads
- **Configurability** — polling interval driven by `AppSettings.RefreshIntervalSeconds`
- **Manual refresh** — `RefreshNowAsync()` ready for R-key binding
- **Clean disposal** — `IDisposable` tears down both watcher and timer

## Impact

- **Program.cs**: Net reduction of ~35 lines. Simpler startup flow.
- **AppSettings**: One new property. No breaking changes.
- **FileWatcherService**: Unchanged — still works the same, just owned by RefreshService instead of Program.cs.
- **Tests**: 699 pass, 0 fail. No test regressions from this change.
- **Next steps**: Siegmeyer can wire `refreshService.RefreshNowAsync()` to the R key handler. SettingsScreen can call `refreshService.SetPollingInterval()` when the user cycles through interval values.
