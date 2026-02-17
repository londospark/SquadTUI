# Project Context

- **Owner:** LondoSpark (ridecar2@gmail.com)
- **Project:** SquadTUI — A terminal user interface built with Hex1b (.NET 10) for managing AI squads. Features include viewing squad activity, inspecting individual members, reading/editing charters, tracking sprint velocities, and more.
- **Stack:** C#, .NET 10, Hex1b TUI framework (https://hex1b.dev/)
- **Created:** 2026-02-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-02-16: Service layer implementation — file parsing patterns

**File parsing patterns used:**
- **Markdown table parsing:** Split rows on `|`, trim cells, skip header/separator rows by detecting `Name` or `---` patterns. Used for `team.md` Members and Coordinator tables.
- **Section-based state machine:** Track current `## ` heading to know which section we're in (Participants, Decisions, Outcomes, What Was Done). Flush accumulated data when a new heading appears.
- **YAML frontmatter parsing:** Detect `---` delimiters, extract `key: "value"` pairs with colon splitting and quote trimming. Used for `SKILL.md` files.
- **Bold-field extraction:** Parse `**Key:** value` and `- **Key:** value` patterns for decision metadata (Date, Author, By).
- **Filename-based metadata:** Extract date and topic from log filenames matching `{YYYY-MM-DD}-{topic}.md` pattern. Fallback regex for `YYYYMMDD` in inbox decision filenames.

**Key assumptions about file formats:**
- `team.md`: Members table has columns Name | Role | Charter | Status (4 cols); Coordinator table has Name | Role | Notes (3 cols). Status text may include emoji prefixes (✅, 📋, 🔄) that are stripped before matching.
- `decisions.md`: Each `## ` heading starts a new decision. May be empty (only has `# Decisions` header and blockquote).
- `decisions/inbox/*.md`: Files use `#` or `###` for titles, `**By:**` as an alternative to `**Author:**`. Date may be in filename rather than content.
- `SKILL.md`: Uses YAML frontmatter (`---` delimited) with `name`, `description`, `source`, `confidence` keys. Falls back to first `# ` heading for name if no frontmatter.
- `orchestration-log/` and `log/`: Directories may be empty — return empty lists gracefully. Log files use `## ` or `### ` section headings with bullet-list (`- `) items under each.
- Agent charters live at `.ai-team/agents/{lowercase-name}/charter.md` — existence checked with `File.Exists`.

### 2026-02-16: Service integration layer — bridging data to UI

**Service Provider pattern:**
- Created `ServiceProvider.cs` as a singleton that instantiates all services with discovered `.ai-team/` root path
- Path discovery strategy: (1) Try `git rev-parse --show-toplevel` first, (2) Walk up directory tree looking for `.ai-team/` folder, (3) Fall back to relative path from executable
- All services instantiated once and reused throughout app lifetime

**DataBridge abstraction:**
- Created `DataBridge.cs` to translate service calls into screen-friendly methods
- Methods: `LoadDashboardDataAsync()`, `LoadRosterDataAsync()`, `LoadDecisionsDataAsync()`, `LoadSkillsDataAsync()`, `LoadLogDataAsync()`, `LoadCharterContentAsync(memberName)`
- Wraps service layer to provide clean, purpose-specific API for UI layer

**AppState data loading:**
- Extended `AppState.cs` with nullable collections: `Members`, `Decisions`, `Skills`, `LogEntries`, `Dashboard`
- Added `IsLoading` flag and `ErrorMessage` for async state management
- All screens updated to use pattern: `state.Members ?? SampleData.Members` for graceful fallback
- Data loaded asynchronously in `Program.cs` startup using `Task.Run()` with parallel `Task.WhenAll()` for efficiency

**Key architectural decisions:**
- Services remain file-path-based; no in-memory caching at service level (stateless)
- UI layer (AppState) holds loaded data; single load on startup
- SampleData preserved as fallback — screens work even if `.ai-team/` files missing
- Error handling: catch exceptions during load, set `ErrorMessage` in state, set `IsLoading = false`

📌 Team update (2026-02-16): Patches created 110-test suite covering ThemeManager, DataBridge, MarkdownRenderer with E2E scenarios. Tests ready to run once MarkdownRenderer.cs compilation errors are fixed by Siegmeyer — decided by Patches

### 2026-02-17: SettingsScreen implementation — Issue #16

**What was built:**
- Created `SettingsScreen.cs` — a new screen for viewing and changing app settings
- Settings exposed: Theme (cycles through 4 themes), Vim Keybindings, Mouse Support, Emoji Display, Markdown Rendering
- Two-panel layout: left panel has a selectable list of settings with current values, right panel shows detail/description for the highlighted setting
- Changes are saved immediately via `SettingsService.Save()` and theme changes apply in real-time via `options.Theme`
- Added `Screen.Settings` enum value and `SettingsSelectedIndex` property to `AppState.cs`
- Added minimal wiring in `Program.cs`: screen switch case, S key binding, J/K navigation for settings list

**API learnings:**
- `ListItemActivatedEventArgs` uses `ActivatedIndex` property (not `SelectedIndex`) — discovered via binary inspection of Hex1b 0.87.0 DLL
- `SettingsScreen.Render()` takes a `dynamic options` parameter to allow runtime theme switching (same pattern used by the T key in Program.cs)
- Pre-existing build errors in `HelpScreen.cs` (Hex1bKey not found) — not our problem, that's Siegmeyer's territory

📌 Team recast (2026-02-18): Squad recast from Ocean's Eleven to Dark Souls universe. Rusty is now Andre. Praise the sun! ☀️

### 2026-02-18: Self-reflection — Metrics data gap

**Why wasn't the data layer providing meaningful metrics?**
SampleData was built as a quick fallback scaffold — flat lists of tasks and members with no temporal dimension. There was no sprint history, no velocity trend, no completion rates. The data existed to prove screens rendered, not to tell a story. I focused on wiring real `.ai-team/` file parsing and didn't circle back to make the sample data presentable. That was a miss.

**Why weren't issues verified before closing?**
Honest answer: I was heads-down on service integration and trusted that "it compiles, ship it" was good enough. I didn't verify that the data actually made sense from a metrics perspective. Closing issues without checking whether the output was useful to a human — not just whether it compiled — is sloppy.

**What will I do differently?**
1. Every data structure gets a "can this be presented?" check before it's marked done. If a stakeholder can't look at it and understand the story, it's not done.
2. Added `SprintMetrics` model with computed properties (`CompletionRate`, `Velocity`, `Utilization`) so metrics are derived from data, not hardcoded.
3. SampleData now carries 3 sprints of history (Undead Burg, Sen's Fortress, Anor Londo) with per-member contributions, so the metrics screen can show trends and identify bottlenecks.
4. Helper properties (`OverallCompletionRate`, `AverageVelocity`, `VelocityTrend`, `TeamUtilization`) provide ready-to-display values for any screen that needs them.

### 2026-02-18: Dual-path support — .squad/ and .ai-team/ directory rename

**Context:** Upstream `squad` CLI is renaming `.ai-team/` to `.squad/` (bradygaster/squad#69, #70). SquadTUI needs to support both during the transition.

**Architecture:**
- `SquadPathResolver` (new static utility): Central resolution logic. `Resolve()` checks `.squad/` first, falls back to `.ai-team/`, defaults to `.squad/` for new installs. `NeedsMigration()` returns true when only `.ai-team/` exists.
- `MigrationService` (new): Handles `.ai-team/` → `.squad/` rename. Tries `git mv` first (preserves git history), falls back to `Directory.Move()`. Returns a `MigrationResult` record.
- Service parameter contract changed: Services now take the fully resolved squad directory path (e.g., `/project/.squad`) instead of the project root. `ServiceProvider` does the resolution once in its constructor.
- `ServiceProvider.Reset()` added for post-migration re-initialization of all services.

**UI changes:**
- Deprecation banner in `AppLayout` when `state.NeedsMigration` is true (yellow warning box)
- C-key creates `.squad/` (not `.ai-team/`) for new projects
- M-key triggers migration from `.ai-team/` to `.squad/`
- `NoSquadScreen` mentions both directory names

**Lessons learned:**
- When working on a feature branch, always check `git status` before starting — uncommitted changes from other branches (cross-branch contamination) can cause confusing build errors
- `git stash pop` after editing files can revert your changes if there are merge conflicts — better to commit WIP before stashing
- Changing service constructor contracts requires updating ALL callers: unit tests, integration tests, E2E tests, and the ServiceProvider. Miss one and the build breaks.
