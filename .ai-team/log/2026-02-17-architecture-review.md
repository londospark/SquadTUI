# Architecture Review — 2026-02-17

**Facilitator:** Solaire
**Participants:** Firekeeper, Andre
**Context:** SampleData isolation, monadic types, dashboard navigation, theme menu

## Decisions

1. **SampleData Isolation** — Move `SampleData.cs` entirely to the test project. Replace all `state.X ?? SampleData.X` fallback patterns with `state.X ?? []` plus explicit empty-state UI widgets. MetricsScreen's hardcoded sprint data gets a real `ISprintService`. Charter content loads from `DataBridge.LoadCharterContentAsync()` via AppState instead of `SampleData.GetCharterFor()`. See `solaire-arch-review-sampledata-isolation.md`.

2. **Monadic Error Handling** — Adopt `LanguageExt` NuGet package. `DataBridge` returns `Either<AppError, T>`. `AppState` stores `Either` results. Screens use `.Match(Right: render, Left: showError)` instead of null-coalescing. `AppError` is an abstract record hierarchy (`FileNotFound`, `ParseError`, `ServiceError`). Charter uses `Option<string>`. See `solaire-arch-review-monadic-types.md`.

3. **Dashboard Panel Navigation** — Tab cycles focus between dashboard panels (Roster → Activity → Decisions → Metrics). Enter drills into the focused panel's full screen. Escape returns to dashboard. Focused panel gets a visually distinct border/background. New `DashboardFocusedPanel` property on AppState. See `solaire-arch-review-dashboard-navigation.md`.

4. **Theme Menu with Preview** — SettingsScreen becomes a modal overlay (not a full screen replacement). Theme selection shows live preview — moving through themes applies them immediately behind the modal. Enter confirms, Escape reverts. T key still works for quick cycling. Prepare architecture for 5+ additional themes. See `solaire-arch-review-theme-menu.md`.

## Action Items

| Owner | Action |
|-------|--------|
| Andre | Add `LanguageExt` to csproj, create `AppError` hierarchy, update `DataBridge` to return `Either<AppError, T>` |
| Andre | Create `ISprintService` to compute sprint metrics from real log/task data |
| Andre | Add `SprintHistory`, `CharterContent` properties to `AppState`, wire through `DataBridge` |
| Andre | Update `Program.cs` startup to remove try/catch, use `Either` results directly |
| Siegmeyer | Update all 7 screens to replace `?? SampleData.X` with `?? []` and add empty-state widgets |
| Siegmeyer | Update screens to use `.Match()` pattern once Andre delivers `Either`-based AppState |
| Siegmeyer | Implement Tab/Enter/Escape panel focus in `AppLayout.BindKeys()` for Dashboard |
| Siegmeyer | Implement `ModalOverlayWidget` and refactor SettingsScreen to overlay mode |
| Firekeeper | Design empty-state widget pattern (centered, dim, informative) |
| Firekeeper | Design focused vs unfocused panel visual states for dashboard |
| Firekeeper | Design modal overlay layout and theme preview strip |
| Patches | Move `SampleData.cs` to test project, update test namespace references |
| Patches | Update tests for `Either<AppError, T>` return types from DataBridge |
| Solaire | Review all PRs for these changes; ensure no SampleData leaks back into prod |

## User Stories

### Essential — What makes SquadTUI indispensable

1. **As a squad lead, I want to see real sprint velocity trends** so I can identify when my AI team is slowing down and why — not fake Sonic-themed sample data.

2. **As a developer running SquadTUI for the first time in a new repo**, I want a graceful empty state that guides me to create a `.squad/` directory, not a screen full of fictional characters.

3. **As a power user, I want to drill from the dashboard into any panel** so I can investigate a problem (blocked task, stale decision) without hunting through tabs.

4. **As someone who lives in the terminal, I want the app to never crash** — monadic error handling means every failure is visible in the UI, never a stack trace in my terminal.

5. **As a team using SquadTUI across multiple projects**, I want to quickly switch themes to visually distinguish which project I'm looking at.

### High Value — Next sprint candidates

6. **As a squad lead, I want to see a diff of what changed since my last session** — new decisions, new log entries, task status changes highlighted.

7. **As a developer, I want to edit charters and decisions inline** from within the TUI, not by opening a text editor separately.

8. **As a CI/CD pipeline, I want SquadTUI to exit with a non-zero code** if the squad has critical issues (e.g., blocked tasks > 50%, no activity in 7 days) — headless mode for health checks.

9. **As a squad member, I want to see my personal dashboard** — my tasks, my recent activity, my charter — without scrolling through everyone else.

10. **As a project manager, I want to export a sprint report** as markdown from the Metrics screen — shareable, archivable.

### Nice to Have — Differentiation

11. **As a user, I want SquadTUI to integrate with GitHub Issues** — see my squad's issues, link tasks to PRs, show CI status per member.

12. **As a team, I want real-time collaboration indicators** — see who else is running SquadTUI against the same repo right now.

## Notes

### Risks

- **Tab key conflict:** Tab is used by Hex1b's TabPanel for screen navigation AND we want it for panel focus on Dashboard. Need to test whether we can intercept Tab per-screen or if it always propagates to TabPanel. Fallback: use a different key (e.g., `Ctrl+Tab` or `F2`) for panel cycling.

- **LanguageExt binary size:** Adds ~2MB. Acceptable for a self-contained .NET 10 binary that's already 60MB+. If LondoSpark objects, we can roll a minimal `Result<T>` instead.

- **MetricsScreen will be empty after SampleData removal** until `ISprintService` is built. This is intentional — shipping fake data is worse than shipping an honest "no data" state. Andre should prioritize `ISprintService` alongside the SampleData removal work.

- **Modal overlay rendering in Hex1b:** Unknown whether Hex1b supports z-index/overlay rendering. If not, the settings "modal" will actually be a conditional full-screen render (current behavior) styled to look like a modal. Functionally identical, visually similar.

### Observations

- SampleData.cs has been recast 3 times (Ocean's Eleven → Dark Souls → Sonic) — a clear signal it doesn't belong in production. Every recast touches production files unnecessarily.

- DataBridge already has `LoadCharterContentAsync()` but no screen uses it — the wiring was started but never completed. This is the lowest-hanging fruit in the SampleData isolation work.

- The `AppLayout.BindKeys()` method is already 120+ lines and growing. After adding panel focus bindings, consider splitting into `DashboardBindings`, `RosterBindings`, etc. as separate methods or a strategy pattern.
