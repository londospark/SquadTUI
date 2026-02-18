# Refresh Settings UI + R Key Binding

**Date:** 2026-02-19
**Author:** Siegmeyer
**Status:** Implemented

## Context

Andre is extracting an `IRefreshService` to consolidate refresh logic. The frontend needed corresponding UI changes to expose refresh interval configuration and manual refresh capability.

## Decisions

### 1. Refresh Interval Setting Added to Both Settings Surfaces

Added "Refresh Interval" as a cyclable setting (15s → 30s → 60s → 120s) to:
- **Settings Modal** (`AppLayout.cs`) — index 6 in `SettingsModalLabels`, rendered with 🔄 icon, cycles on Enter.
- **Settings Screen** (`SettingsScreen.cs`) — index 5 in `SettingLabels`, with detail pane description.

Modal fixed height bumped from 17 → 19 to accommodate the new item.

`AppSettings.RefreshIntervalSeconds` property added (default: 30) — Andre's `IRefreshService` can read this to set timer interval.

### 2. R Key for Manual Refresh

`R` key binding added to `BindKeys` (with Shift+R for case-insensitivity). Fires a parallel `Task.Run` that reloads all four data sources via `DataBridge` and updates `state.LastRefreshTime`.

Guarded behind `state.CurrentScreen != Screen.NoSquad` — no refresh when no squad is detected.

Dashboard footer updated: `R: Refresh` added to key hints.

### 3. RedrawAfter(3000) Kept

Initially considered removing `RedrawAfter(3000)` from DashboardScreen since the refresh service would handle redraws. However, Hex1b needs periodic redraw calls to pick up state changes from background tasks. Kept as-is until Andre's `IRefreshService` provides a mechanism to trigger redraws directly.

## Impact

- **Andre:** `AppSettings.RefreshIntervalSeconds` is available for `IRefreshService` to read. The R key binding creates a `DataBridge` directly — once `IRefreshService` is injectable into AppLayout, the R key handler can delegate to it instead.
- **Patches:** Settings modal now has 7 items (was 6). Any tests asserting settings count or modal height need updating.
