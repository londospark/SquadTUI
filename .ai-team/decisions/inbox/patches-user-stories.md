# User Stories — Patches (Tester)

> From the perspective of a Tester who lives in the terminal, assumes every input is malformed, and wants confidence that nothing ships broken.

---

## US-1: Test Coverage Dashboard Panel

**As a** tester reviewing SquadTUI's quality,
**I want** a dashboard panel or screen that shows test coverage metrics (total tests, pass/fail/skip counts, coverage % by file),
**so that** I can see at a glance which parts of the codebase are under-tested and prioritize new test work.

**Acceptance criteria:**
- Shows total test count, pass/fail/skip breakdown
- Shows file-level or namespace-level coverage percentages (from coverlet output)
- Highlights files with <50% coverage in a warning color
- Updates when test data changes (file watcher or manual refresh)
- Works at all responsive breakpoints (narrow/medium/wide)

---

## US-2: Regression Detection via Build Health Indicator

**As a** tester monitoring CI health,
**I want** SquadTUI to display the latest CI build status (pass/fail/running) on the Dashboard,
**so that** regressions are immediately visible without leaving the terminal to check GitHub Actions.

**Acceptance criteria:**
- Dashboard shows a build health badge (✅ Passing / ❌ Failing / 🔄 Running)
- Clicking/entering the badge navigates to a detail screen showing which jobs failed
- Failed test names are listed with truncated error messages
- Data refreshes on the polling timer alongside other dashboard data
- Gracefully shows "Unknown" if no CI data is available

---

## US-3: Automated Quality Gate Before Squad Operations

**As a** tester who wants to prevent broken changes from being committed,
**I want** SquadTUI to run `dotnet test` as a quality gate before destructive operations (adding/removing members, migrating),
**so that** the squad directory is never modified while the build is broken.

**Acceptance criteria:**
- Before `A` (Add Member) or `D` (Remove Member) executes, a pre-flight check runs `dotnet test --no-build`
- If tests fail, the operation is blocked and an error message is shown
- User can bypass with a confirmation ("Tests failing — proceed anyway? Y/N")
- Pre-flight check has a configurable timeout (default 60s)
- Quality gate can be disabled in Settings

---

## US-4: Error Detail Screen with Stack Traces

**As a** tester debugging service failures,
**I want** an error detail screen that shows the full exception chain when data loading fails,
**so that** I can diagnose issues without re-running the app with `--verbose` or checking logs externally.

**Acceptance criteria:**
- When `state.ErrorMessage` is set, a clickable/navigable error indicator appears on the Dashboard
- Entering the error indicator opens an Error Detail screen showing: error type, message, stack trace, and inner exceptions
- Multiple errors are listed chronologically
- Error screen supports j/k scrolling for long stack traces
- Escape returns to the previous screen
- Errors are also visible in the `AppState` for E2E test assertions

---

## US-5: E2E Test Recording Playback

**As a** tester writing and debugging E2E tests,
**I want** SquadTUI to support a `--replay` mode that replays a sequence of Hex1b input events from a file,
**so that** I can reproduce complex navigation scenarios deterministically without writing new test code each time.

**Acceptance criteria:**
- `SquadTUI --replay events.json` reads a JSON file of `{key, wait_ms}` tuples and replays them
- Terminal snapshots are captured at each step and saved to a `snapshots/` directory
- Replay mode exits automatically after the last event
- Invalid event files produce a clear error message, not a crash
- Replay files can be generated from E2E test runs with a `--record` flag on TestAppBuilder
