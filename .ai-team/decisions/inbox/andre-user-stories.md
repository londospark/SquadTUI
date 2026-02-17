# User Stories — Backend Dev Perspective

**By:** Andre
**Date:** 2026-02-18

These user stories reflect backend/data-layer needs that would make SquadTUI more reliable, performant, and useful for real squad management.

---

## US-1: File Change Debouncing for Live Dashboard

**As a** squad lead monitoring my team in SquadTUI,
**I want** the file watcher to debounce rapid bursts of file changes (e.g., during a `git pull` or squad CLI run),
**So that** the dashboard doesn't flicker or reload data 15 times in 2 seconds when multiple `.ai-team/` files are written simultaneously.

**Acceptance Criteria:**
- FileWatcherService batches changes within a 500ms window before triggering reload
- Only one data refresh fires per batch, not one per file
- UI shows a brief "Refreshing..." indicator during reload
- No stale data displayed — final state after batch is always consistent

---

## US-2: Data Export to JSON/CSV

**As a** developer integrating SquadTUI data into other tools (dashboards, reports, CI pipelines),
**I want** to export squad data (roster, decisions, metrics, activity log) to JSON or CSV format from the command line,
**So that** I can feed squad performance data into external reporting tools without parsing markdown manually.

**Acceptance Criteria:**
- `squadtui export --format json --output squad-data.json` exports all data
- `squadtui export --section roster --format csv` exports a specific section
- Export includes computed fields (completion rate, velocity, utilization)
- JSON output follows a documented schema for programmatic consumption

---

## US-3: Cached Data Layer with Incremental Updates

**As a** user running SquadTUI on a large squad directory (50+ files),
**I want** parsed data to be cached in memory with incremental updates when individual files change,
**So that** the app starts fast and doesn't re-parse every file on every refresh.

**Acceptance Criteria:**
- First load parses all files and caches results in a `DataCache` service
- FileWatcher triggers re-parse only for changed files, merging into cache
- Cache invalidation is file-path-based (change to `agents/andre/charter.md` only re-parses that charter)
- Startup time for a 50-file squad directory is under 500ms after first load

---

## US-4: External Tool Integration — Squad CLI Sync

**As a** developer using both `squad` CLI and SquadTUI,
**I want** SquadTUI to detect when the `squad` CLI modifies files and auto-refresh,
**So that** I don't have to restart SquadTUI after running `squad run` or `squad add-agent`.

**Acceptance Criteria:**
- FileWatcher monitors the entire `.squad/` (or `.ai-team/`) directory tree recursively
- New files (e.g., a new agent directory) are detected and loaded without restart
- Deleted files (e.g., removed decision) are reflected in the UI
- A notification appears briefly: "Squad files updated — data refreshed"

---

## US-5: Offline-First with Git-Aware Conflict Detection

**As a** team member working offline and syncing later,
**I want** SquadTUI to detect when `.ai-team/` files have git merge conflicts after a `git pull`,
**So that** I see a clear warning instead of corrupted data from half-parsed conflict markers.

**Acceptance Criteria:**
- Service layer detects `<<<<<<<`, `=======`, `>>>>>>>` markers in parsed files
- Affected items show a ⚠️ conflict badge in the UI instead of garbled content
- A summary notification shows "3 files have merge conflicts — resolve before editing"
- Conflict-free files still display normally
