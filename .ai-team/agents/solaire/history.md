# Project Context

- **Owner:** LondoSpark (ridecar2@gmail.com)
- **Project:** SquadTUI — A terminal user interface built with Hex1b (.NET 10) for managing AI squads. Features include viewing squad activity, inspecting individual members, reading/editing charters, tracking sprint velocities, and more.
- **Stack:** C#, .NET 10, Hex1b TUI framework (https://hex1b.dev/)
- **Created:** 2026-02-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-02-16 — Project Bootstrap

- **Solution structure:** `SquadTUI.sln` at repo root, main project at `src/SquadTUI/`. Folders: `Models/`, `Services/`, `Screens/`.
- **Framework:** Hex1b 0.1.0 on .NET 10. Uses `Hex1bTerminal.CreateBuilder()` fluent API, not direct widget constructors. See `samples/InfoBarDemo/Program.cs` in mitchdenny/hex1b for the canonical pattern.
- **Naming convention:** `SquadTask` (not `Task`) to avoid `System.Threading.Tasks.Task` collision. `SquadTaskStatus` enum likewise.
- **Models:** All C# records with nullable reference types. Key files: `SquadMember.cs`, `SquadTask.cs`, `OrchestrationLogEntry.cs`, `DecisionEntry.cs`, `Skill.cs`, `TeamRoster.cs`, `DashboardData.cs`, `GitHubModels.cs`.
- **Services:** Interface + stub pattern. `ITeamService`, `IOrchestrationLogService`, `IDecisionService`, `ISkillService`, `ISquadDataProvider`. Implementations take `teamRootPath` string in constructor.
- **No DI container yet** — services wired manually. Add when complexity warrants it.
- **Architecture decision written to:** `.ai-team/decisions/inbox/solaire-project-architecture.md`

### 2026-02-16 — CI/CD Infrastructure

- **CI workflow:** `.github/workflows/ci.yml` — matrix builds across Windows/macOS/Linux × x64/ARM64 (6 combinations). Triggers on push to `develop`/`main` and PRs to those branches + `release/*`.
- **Release workflow:** `.github/workflows/release.yml` — triggers on push to `release/*` branches. Publishes self-contained binaries for all 6 RIDs, creates git tag from csproj version, creates GitHub release with binaries as assets (zip for Windows, tar.gz for Unix).
- **Version source:** `src/SquadTUI/SquadTUI.csproj` `<Version>` property (currently 0.2.0). Release workflow extracts this with `grep -oP`.
- **Binary packaging:** Windows gets `.zip`, macOS/Linux get `.tar.gz`. All archives include self-contained .NET runtime.
- **Standard .gitignore added:** Covers `bin/`, `obj/`, `dist/`, `packages/`, IDE folders (`.vs/`, `.idea/`, `.vscode/`), test results, NuGet artifacts.

### 2026-02-17 — Screenshot & Demo Infrastructure

- **hex1b CLI:** Installed globally via `dotnet tool install -g Hex1b.Tool` (v0.87.0). Provides `hex1b terminal`, `hex1b capture screenshot`, `hex1b capture recording`, `hex1b keys` commands.
- **Diagnostics:** Added `.WithDiagnostics()` to the terminal builder chain in `Program.cs`. This is a one-line change that enables the hex1b CLI to discover and interact with the running app via diagnostics socket.
- **Capture script:** `scripts/capture-screenshots.ps1` — automated PowerShell script that builds the app, starts it in a hex1b-hosted terminal, navigates screens via keystroke injection, captures SVG screenshots, and optionally records asciinema demos. Supports `-Record` and `-RecordDuration` parameters.
- **Documentation:** `docs/SCREENSHOTS.md` — comprehensive guide covering hex1b CLI installation, manual and automated screenshot capture, asciinema recording/upload, output formats, and troubleshooting.
- **README updated:** Screenshots section now references `.svg` format (hex1b native), added 🎬 Demo section with asciinema embed placeholder, added link to SCREENSHOTS.md for contribution workflow.
- **Pre-existing build errors:** The `SettingsScreen.cs` has a `ListItemActivatedEventArgs.SelectedIndex` error that predates this work — likely an API mismatch in the Hex1b version. Not my responsibility to fix.

📌 Team recast (2026-02-18): Squad recast from Ocean's Eleven to Dark Souls universe. Danny is now Solaire. Praise the sun! ☀️

### 2026-02-18 — SampleData Dark Souls Recast

- **SampleData.cs:** Replaced all Ocean's Eleven names (Danny, Linus, Rusty, Basher, Saul) with Dark Souls names (Solaire, Siegmeyer, Andre, Patches, Firekeeper) across Members, Decisions, LogEntries, Tasks, and GetCharterFor().
- **GetCharterFor():** Expanded from 3 entries to all 6 team members with Dark Souls-flavored charter descriptions.
- **MemberDetailScreen.cs / CharterScreen.cs:** Default member fallback changed from "Danny" to "Solaire".
- **E2E tests updated:** RosterScreenTests and DecisionsScreenTests now assert Dark Souls names.
- **Fixture-based tests untouched:** Unit/integration tests that parse fixture files (team.md, decisions.md, log files) still use fixture data — those test the parser, not SampleData.
- **Result:** 221 tests pass, build clean.

### 2026-02-18 — CI Fix: Removed Self-Referential Tests

- **Problem:** `DotnetTest_Passes` in CIPipelineTests.cs ran `dotnet test` as a subprocess, which re-invoked itself infinitely. This broke CI on all 6 platforms. `DotnetBuild_Succeeds` similarly spawned a child build process — unnecessary and slow.
- **Fix:** Deleted both process-spawning tests. Added `--filter` to ci.yml as defense-in-depth. 326 tests run, 319 pass (7 pre-existing E2E failures unrelated to this change).
- **Decision written to:** `.ai-team/decisions/inbox/solaire-ci-fix.md`

#### Self-Reflection: Process Gaps

**Why did CI break and nobody caught it?**
The recursive tests were introduced in Sprint 5 test coverage expansion. I reviewed and approved the CIPipelineTests concept without catching that `DotnetTest_Passes` would recurse. The tests passed locally in some configurations (timeouts, process isolation) but failed consistently in CI's matrix builds. Nobody was monitoring CI results after merges — we merged and moved on.

**What process gaps allowed closed issues without CI verification?**
Our Definition of Done says "tests pass" but we weren't enforcing "tests pass *in CI*" as a gate. Issues were closed based on local test runs. The CI workflow existed but was treated as informational, not blocking. No branch protection rules required CI to pass before merge.

**What will I do differently going forward?**
1. **No test may spawn `dotnet build` or `dotnet test`.** This is now a hard rule. CI tests validate file structure and configuration only.
2. **CI must be green before any issue is closed.** I will enforce this in reviews.
3. **I will check CI status after every merge**, not assume it passes because local tests passed.
4. **Branch protection:** Recommend to LondoSpark that we enable required status checks on develop/main so CI failures block merges.

### 2026-02-17 — Architecture Review Ceremony: SampleData, Monadic Types, Navigation

- **SampleData audit complete:** 7 screens reference SampleData with 26 total references. Worst offender: MetricsScreen (9 refs including hardcoded `SprintHistory` with no service backing). CharterScreen and charter rendering bypass `DataBridge.LoadCharterContentAsync()` entirely.
- **Decision: SampleData → test project.** Replace `state.X ?? SampleData.X` with `state.X ?? []` plus empty-state UI widgets. No fake data in prod binary.
- **Decision: LanguageExt for monadic types.** `DataBridge` returns `Either<AppError, T>`, screens use `.Match()`. Chose LanguageExt over OneOf (no LINQ) and roll-our-own (maintenance burden). `AppError` is abstract record hierarchy.
- **Decision: Dashboard panel focus.** Tab cycles panels, Enter drills in, Escape backs out. New `DashboardFocusedPanel` on AppState. Risk: Tab may conflict with Hex1b TabPanel.
- **Decision: Settings modal overlay.** SettingsScreen becomes overlay, not screen replacement. Theme preview applies live. T key preserved for quick cycling. Prepare for 5+ additional themes.
- **Pattern established: Empty-state widgets.** Every screen must handle null/empty data gracefully with a centered dim message instead of crashing or showing fake data.
- **Pattern established: `Either<AppError, T>` at service boundaries.** Services don't throw. Screens use `.Match()`. No try/catch in rendering pipeline.
- **Observation: AppLayout.BindKeys() is 120+ lines.** Consider splitting per-screen binding methods after dashboard navigation lands.
- **Decisions written to inbox:** `solaire-arch-review-sampledata-isolation.md`, `solaire-arch-review-monadic-types.md`, `solaire-arch-review-dashboard-navigation.md`, `solaire-arch-review-theme-menu.md`

### 2026-02-19 — P0 Fix: Polling Timer + LastRefreshTime

- **Root cause:** `state.LastRefreshTime` was set once at initialization and never updated. No periodic polling existed — the only refresh mechanism was `FileWatcherService.OnFilesChanged` which itself never updated the timestamp.
- **Fix (Program.cs):** (1) Added `System.Threading.Timer` polling every 30 seconds that reloads all data via `bridge.Load*Async()` and updates `state.LastRefreshTime` + clears `HasPendingRefresh`. (2) Updated the existing file watcher handler to also set `LastRefreshTime = DateTime.Now` and `HasPendingRefresh = false` after data reload completes.
- **Fix (DashboardScreen.cs):** Removed the render-side `HasPendingRefresh` check that was attempting to set `LastRefreshTime` during rendering — this was a workaround for the missing data-side updates and could race with the actual data loading.
- **Timer disposal:** `using var pollingTimer` ensures cleanup when the app exits.
- **DashboardScreen already had `RedrawAfter(3000)`** which triggers re-render every 3 seconds — this was already working but the timestamp it displayed was stale because nothing was updating it.
- **User stories written to:** `.ai-team/decisions/inbox/solaire-user-stories.md` — 5 stories covering architecture observability, decision search, sprint velocity, code review queue, and decision drafting from TUI.
