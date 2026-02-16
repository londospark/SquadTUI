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
- **Architecture decision written to:** `.ai-team/decisions/inbox/danny-project-architecture.md`

### 2026-02-16 — CI/CD Infrastructure

- **CI workflow:** `.github/workflows/ci.yml` — matrix builds across Windows/macOS/Linux × x64/ARM64 (6 combinations). Triggers on push to `develop`/`main` and PRs to those branches + `release/*`.
- **Release workflow:** `.github/workflows/release.yml` — triggers on push to `release/*` branches. Publishes self-contained binaries for all 6 RIDs, creates git tag from csproj version, creates GitHub release with binaries as assets (zip for Windows, tar.gz for Unix).
- **Version source:** `src/SquadTUI/SquadTUI.csproj` `<Version>` property (currently 0.2.0). Release workflow extracts this with `grep -oP`.
- **Binary packaging:** Windows gets `.zip`, macOS/Linux get `.tar.gz`. All archives include self-contained .NET runtime.
- **Standard .gitignore added:** Covers `bin/`, `obj/`, `dist/`, `packages/`, IDE folders (`.vs/`, `.idea/`, `.vscode/`), test results, NuGet artifacts.
