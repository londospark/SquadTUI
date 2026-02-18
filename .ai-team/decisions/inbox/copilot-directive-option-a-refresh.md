### 2026-02-18: Live update strategy — Option A (Hybrid) selected
**By:** LondoSpark (via Copilot)
**What:** User chose Option A: Hybrid FileWatcher + Configurable Polling. Extract IRefreshService, make polling interval configurable in Settings, add R key for manual refresh, eliminate triple-copy reload logic in Program.cs.
**Why:** User decision — best balance of responsiveness (FileWatcher for instant) and reliability (polling as fallback). User explicitly requested service extraction.
