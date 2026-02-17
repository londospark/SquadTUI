# IFileLocationService — Centralized Path Resolution

- **Date:** 2025-07-24
- **Author:** Solaire
- **Status:** Implemented

## What

Introduced `IFileLocationService` as a single interface for ALL file path resolution in the application. A concrete `FileLocationService` implementation handles `.squad/` vs `.ai-team/` transparently via `SquadPathResolver`.

All services (`TeamService`, `DecisionService`, `SkillService`, `OrchestrationLogService`) now accept `IFileLocationService` instead of constructing paths with raw `Path.Combine` calls. `ServiceProvider` creates and exposes the `IFileLocationService` instance. `SettingsService`, `FileWatcherService`, and `SquadDetector` also use it where applicable.

### Interface Methods

| Method | Returns |
|--------|---------|
| `ProjectRoot` | The git repo / project root directory |
| `SquadDirectory` | Resolved `.squad/` or `.ai-team/` path |
| `GetRosterPath()` | `{squad}/team.md` |
| `GetAgentsDirectory()` | `{squad}/agents/` |
| `GetCharterPath(agentName)` | `{squad}/agents/{name}/charter.md` |
| `GetDecisionsFilePath()` | `{squad}/decisions.md` |
| `GetDecisionsInboxPath()` | `{squad}/decisions/inbox/` |
| `GetSkillsDirectory()` | `{squad}/skills/` |
| `GetSkillFilePath(slug)` | `{squad}/skills/{slug}/SKILL.md` |
| `GetOrchestrationLogDirectory()` | `{squad}/orchestration-log/` |
| `GetLogDirectory()` | `{squad}/log/` |
| `GetSettingsDirectory()` | `~/.config/squadtui/` |
| `GetSettingsFilePath()` | `~/.config/squadtui/settings.json` |
| `ActiveDirectoryName` | `.squad` or `.ai-team` |
| `NeedsMigration` | `true` if legacy `.ai-team/` needs migration |
| `HasSquadDirectory` | `true` if either directory exists |

### Backward Compatibility

Services retain a legacy `string squadDirPath` constructor that delegates to `FileLocationService.FromSquadDirectory()` — this keeps all existing tests passing without modification. The `FromSquadDirectory` factory method treats the passed string as an already-resolved squad directory path rather than a project root.

## Why

- **Single source of truth**: Before this change, every service independently constructed paths with `Path.Combine(squadDir, "agents", ...)`, `Path.Combine(squadDir, "decisions.md")`, etc. If directory structure changed, every service needed updating.
- **Testability**: `IFileLocationService` can be mocked in tests to control path resolution without touching the filesystem.
- **Future extensibility**: Adding new paths (e.g., templates, archives, backups) requires adding one method to the interface — all consumers get it automatically.
- **Transparent migration**: `.squad/` vs `.ai-team/` resolution is handled in one place. When we eventually drop `.ai-team/` support, only `FileLocationService` changes.

## Files Changed

- `IFileLocationService.cs` — New interface
- `FileLocationService.cs` — New concrete implementation
- `TeamService.cs` — Uses `IFileLocationService` for roster + charter paths
- `DecisionService.cs` — Uses `IFileLocationService` for decisions + inbox paths
- `SkillService.cs` — Uses `IFileLocationService` for skills directory + file paths
- `OrchestrationLogService.cs` — Uses `IFileLocationService` for log directories
- `ServiceProvider.cs` — Creates and exposes `IFileLocationService`
- `SettingsService.cs` — Accepts optional `IFileLocationService` for settings paths
- `FileWatcherService.cs` — Added `Start(IFileLocationService)` overload
- `SquadDetector.cs` — Uses `FileLocationService` for path resolution
- `AppLayout.cs` — Uses `FileLocationService` for squad creation paths
