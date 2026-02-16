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

📌 Team update (2026-02-16): Basher created 110-test suite covering ThemeManager, DataBridge, MarkdownRenderer with E2E scenarios. Tests ready to run once MarkdownRenderer.cs compilation errors are fixed by Linus — decided by Basher
