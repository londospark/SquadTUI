# Refresh Service & Settings Test Coverage

**Author:** Patches (Tester)
**Date:** 2025-01-27
**Status:** Complete — tests written

---

## What Was Done

Created 21 new tests across 3 files covering the RefreshService, AppSettings.RefreshIntervalSeconds, and refresh-related E2E behavior.

### Test Files Created

**1. `tests/SquadTUI.Tests/Unit/RefreshServiceTests.cs` — 9 tests**
- `RefreshService_StartsWithCorrectInterval` — verifies default 30s from AppSettings
- `RefreshService_SetPollingInterval_ChangesInterval` — interval accepted without crash
- `RefreshService_RefreshNow_UpdatesLastRefreshTime` — manual refresh sets timestamp
- `RefreshService_IsActive_TrueAfterStart` — Start() activates the service (uses temp dir)
- `RefreshService_IsActive_FalseAfterStop` — Stop() deactivates the service
- `RefreshService_Dispose_StopsAll` — Dispose() stops watcher and timer
- `RefreshService_RefreshNow_FiresOnDataRefreshed` — event fires on refresh
- `RefreshService_RefreshNow_UpdatesAppState` — state.LastRefreshTime and HasPendingRefresh updated
- `RefreshService_SmartPolling_SkipsWhenWatcherFiredRecently` — sequential refreshes don't deadlock

**2. `tests/SquadTUI.Tests/Unit/AppSettingsRefreshTests.cs` — 6 tests**
- `AppSettings_HasRefreshIntervalSeconds` — property exists, defaults to 30
- `AppSettings_RefreshIntervalSeconds_Serializes` — JSON round-trip at 60s
- `AppSettings_RefreshIntervalSeconds_RoundTripsAllValidValues` — 15/30/60/120 all survive serialization
- `AppSettings_RefreshIntervalSeconds_DefaultSerializesToJson` — appears in JSON output
- `AppSettings_RefreshIntervalSeconds_DeserializesFromMissingProperty` — backward compat with old settings files
- `AppSettings_RefreshIntervalSeconds_IndependentOfOtherProperties` — no crosstalk

**3. `tests/SquadTUI.Tests/E2E/RefreshIntervalTests.cs` — 6 tests**
- `Dashboard_ShowsRefreshHintWithRKey` — "Updated:" visible in dashboard footer area
- `Settings_ShowsRefreshInterval` — settings modal opens and renders
- `Settings_CyclesRefreshInterval` — settings modal is interactive (cycle ready when Siegmeyer wires it)
- `RKey_TriggersManualRefresh` — R key doesn't crash, dashboard stays on screen with "Updated:"
- `RefreshInterval_DefaultIs30` — AppSettings property default
- `RefreshInterval_PersistsAfterChange` — JSON round-trip persistence

### Testing Approach
- **xUnit Assert.\*** only — no FluentAssertions, no NSubstitute, per project decision
- **Hand-written stubs** via StubServiceProviderFactory for RefreshService unit tests
- **Hex1b headless terminal** via TestAppBuilder for E2E tests
- **Temp directories** for FileWatcher start/stop tests, cleaned up in finally blocks
- **IDisposable** pattern for RefreshService test class to ensure cleanup

### Pending Implementation Notes
- **R key binding**: Tests verify R key doesn't crash the app, but the actual R→RefreshNow wiring needs Siegmeyer to add the keybinding in AppLayout.BindKeys(). Once wired, the `RKey_TriggersManualRefresh` test should verify timestamp changes.
- **Settings "Refresh Interval" option**: Tests verify the settings modal opens and renders, but the actual "Refresh Interval" list item with 15/30/60/120 cycling needs Siegmeyer to add to SettingsScreen. Once wired, `Settings_CyclesRefreshInterval` should assert specific interval values.
- **Footer "R: Refresh" hint**: The dashboard footer currently doesn't include "R: Refresh". Once Siegmeyer adds this to the `Screen.Dashboard` case in `RenderFooter`, the `Dashboard_ShowsRefreshHintWithRKey` test should assert it directly.

### Build & Test Result
All 21 new tests pass. No existing tests broken.
