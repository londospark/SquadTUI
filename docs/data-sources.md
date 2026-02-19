# Data Sources

> Complete catalog of all data sources consumed by SquadTUI, how changes are detected, and known gaps.
>
> Last updated: 2026-02-18 by Solaire

## Overview

SquadTUI reads from the `.ai-team/` (or `.squad/`) directory structure created by the [Squad CLI](https://github.com/csharpfritz/SquadUI). Data is loaded at startup via `DataBridge` and kept fresh through a hybrid refresh system: `FileSystemWatcher` for reactive updates + a configurable polling timer as fallback.

## Data Sources — Currently Integrated

### Team Roster (`team.md`)

- **Path:** `.ai-team/team.md`
- **Format:** Markdown with tables (Coordinator table, Members table, Project Context section)
- **Service:** `TeamService.GetRosterAsync()`
- **Provides:** Member names, roles, status badges, charter file paths, project description
- **Refresh:** FileWatcher + polling (included in `ReloadAllAsync()`)

### Agent Charters (`agents/*/charter.md`)

- **Path:** `.ai-team/agents/{name}/charter.md`
- **Format:** Freeform Markdown
- **Service:** `DataBridge.LoadCharterContentAsync(memberName)` — raw file read
- **Provides:** Agent identity, role description, expertise, working style
- **Refresh:** On-demand when navigating to Charter screen

### Agent Histories (`agents/*/history.md`)

- **Path:** `.ai-team/agents/{name}/history.md`
- **Format:** Markdown with `### date — topic` headings under `## Learnings`
- **Service:** `TeamService.GetCurrentTasksAsync()` → `ExtractLatestTaskFromHistory()`
- **Provides:** Latest task/learning entry per agent (infers "current work")
- **Refresh:** FileWatcher + polling (via roster reload)

### Decisions (`decisions.md` + `decisions/inbox/`)

- **Path:** `.ai-team/decisions.md` + `.ai-team/decisions/inbox/*.md`
- **Format:** Markdown with `### date: title`, `**By:**`, `**What:**`, `**Why:**` fields
- **Service:** `DecisionService.GetDecisionsAsync()`
- **Provides:** Decision title, date, author, what/why content, source file path + line number
- **Refresh:** FileWatcher + polling (included in `ReloadAllAsync()`)

### Session Logs (`log/`)

- **Path:** `.ai-team/log/*.md`
- **Format:** Markdown with date-prefixed filenames, structured sections
- **Service:** `OrchestrationLogService.GetEntriesAsync()`
- **Provides:** Timestamp, topic, participants, summary, decisions, outcomes, work description
- **Refresh:** FileWatcher + polling (included in `ReloadAllAsync()`)

### Orchestration Logs (`orchestration-log/`)

- **Path:** `.ai-team/orchestration-log/*.md`
- **Format:** Same as session logs
- **Service:** `OrchestrationLogService.GetEntriesAsync()` (merged with `log/`)
- **Provides:** Same as session logs
- **Refresh:** FileWatcher + polling

### Skills (`skills/*/SKILL.md`)

- **Path:** `.ai-team/skills/{slug}/SKILL.md`
- **Format:** Markdown with YAML frontmatter (`name`, `description`, `source`, `confidence`)
- **Service:** `SkillService.GetSkillsAsync()`
- **Provides:** Skill name, description, source, confidence level, full body content
- **Refresh:** FileWatcher + polling (included in `ReloadAllAsync()`)

### User Settings

- **Path:** `~/.config/squadtui/settings.json`
- **Format:** JSON
- **Service:** `SettingsService.Load()` / `.Save()`
- **Provides:** Theme, vim bindings, mouse, emoji, markdown rendering, default screen, refresh interval
- **Refresh:** Loaded once at startup, saved on user action

## Data Sources — Present but Not Parsed

These files exist in the `.ai-team/` directory and are covered by the FileWatcher, but **no service reads them**.

### Ceremonies (`ceremonies.md`)

- **Path:** `.ai-team/ceremonies.md`
- **Format:** Markdown with per-ceremony tables (trigger, when, condition, facilitator, participants, time budget, enabled)
- **Potential:** Ceremony schedule display, upcoming review indicators, retro cadence tracking

### Routing (`routing.md`)

- **Path:** `.ai-team/routing.md`
- **Format:** Markdown with routing table and rules
- **Potential:** Show work routing rules in UI, help users understand which agent handles what

### Casting State (`casting/`)

- **Path:** `.ai-team/casting/registry.json`, `policy.json`, `history.json`
- **Format:** JSON
- **Potential:** Agent-to-universe mapping, casting history, "team identity" panel, universe theme display

### GitHub Models (code only)

- **Path:** `src/SquadTUI/Models/GitHubModels.cs`
- **Status:** C# record types (`GitHubIssue`, `GitHubMilestone`, `GitHubLabel`) defined but no service implementation

## Change Detection Architecture

```
┌─────────────────────────────────────────┐
│            RefreshService               │
│  ┌────────────────┐  ┌───────────────┐  │
│  │ FileWatcher    │  │ Polling Timer │  │
│  │ (reactive,     │  │ (30s default, │  │
│  │  500ms debounce)│  │  smart-skip)  │  │
│  └───────┬────────┘  └──────┬────────┘  │
│          │                  │            │
│          └──────┬───────────┘            │
│                 ▼                        │
│         ReloadAllAsync()                 │
│  ┌──────────────────────────────┐       │
│  │ Members ✓  Tasks ✓           │       │
│  │ Decisions ✓  LogEntries ✓    │       │
│  │ Skills ✓  Charters (on-demand) │       │
│  └──────────────────────────────┘       │
│                 │                        │
│                 ▼                        │
│         AppState updated                 │
│         LastRefreshTime set              │
└─────────────────────────────────────────┘
```

**Note:** Charter content loads on-demand when the user navigates to the Charter screen. All other data sources refresh automatically via FileWatcher and polling.

## Known Gaps

### Data We Cannot Get from `.ai-team/` Files

| Gap | Description | Best Available Proxy |
|-----|-------------|---------------------|
| **Real-time agent activity** | Is an agent currently spawned/working in a Copilot session? | File mtime on `history.md` / inbox writes |
| **Session state** | Is a Copilot coding session active? Duration? Token usage? | No proxy available today |
| **Git activity** | Recent commits, branch state, uncommitted changes | `git log`, `git branch`, `git status` via CLI |
| **GitHub state** | Open PRs, issues, CI status, code review queue | `gh` CLI or GitHub REST API |
| **Sprint boundaries** | When did sprints start/end? What's the current sprint? | Not tracked anywhere currently |
| **Task completion time** | How long did a task take from start to finish? | Not tracked — only final state is recorded |

### Recommended External Integrations

1. **Git CLI** (P1) — Create `GitService` wrapping `git log`, `git branch`, `git status`. Already have process-spawning patterns. Graceful degradation if git unavailable.
2. **File mtime heuristics** (P1) — Use `File.GetLastWriteTimeUtc()` on `history.md` and inbox files to infer recent agent activity. Low effort, high value.
3. **GitHub CLI** (P2) — Create `GitHubService` wrapping `gh issue list`, `gh pr list`, `gh run list`. Graceful degradation if `gh` unavailable.
4. **Copilot SDK** (Deferred) — No public API exists for querying Copilot session state. Monitor for future availability.

## Service Architecture Reference

All data flows through this chain:

```
.ai-team/ files
    → IFileLocationService (path resolution)
    → I{Domain}Service (parsing)
    → DataBridge (UI-friendly wrappers, Either<AppError, T>)
    → AppState (screen-accessible state)
    → Screen rendering
```

New services should follow this pattern. See `src/SquadTUI/Services/` for implementations.
