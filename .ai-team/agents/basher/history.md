# Project Context

- **Owner:** LondoSpark (ridecar2@gmail.com)
- **Project:** SquadTUI — A terminal user interface built with Hex1b (.NET 10) for managing AI squads. Features include viewing squad activity, inspecting individual members, reading/editing charters, tracking sprint velocities, and more.
- **Stack:** C#, .NET 10, Hex1b TUI framework (https://hex1b.dev/)
- **Created:** 2026-02-16

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-02-17: Test Suite Created — 110 tests (Unit + Integration + E2E)

**Testing patterns that worked:**
- **Unit tests** use `IDisposable` with temp directories for service isolation. Each test creates a fresh temp dir with `.ai-team/` structure, writes specific markdown files, then runs service methods against them. This keeps tests fast, independent, and deterministic.
- **Integration tests** use fixture files copied to output directory via `<Content Include="Fixtures\**\*" CopyToOutputDirectory="PreserveNewest" />` in the csproj. The `AppContext.BaseDirectory` resolves the fixtures path at runtime.
- **NSubstitute** mocks for `SquadDataProvider` tests — substituting `ITeamService`, `IDecisionService`, etc. lets us test aggregation logic without file I/O.
- **E2E tests** recreate the full app layout from `Program.cs` in a `TestAppBuilder` helper, using the Hex1b headless terminal.

**Hex1b Testing API discoveries:**
- The correct builder class is `Hex1bTerminalInputSequenceBuilder` (in `Hex1b.Automation` namespace), NOT `Hex1bInputSequenceBuilder`.
- Terminal inspection uses `terminal.CreateSnapshot()` which returns a `Hex1bTerminalSnapshot`. Extension methods like `ContainsText()`, `GetText()`, `GetLine()`, `GetNonEmptyLines()` are on `IHex1bTerminalRegion` via `Hex1bTerminalRegionExtensions`.
- Headless mode: `Hex1bTerminal.CreateBuilder().WithHex1bApp(...).WithHeadless().WithDimensions(w, h).Build()`.
- App lifecycle: `RunAsync(ct)` + `CancellationTokenSource` for clean shutdown. `await Task.Delay(200)` is sufficient for rendering between input steps.
- `WaitUntil(predicate, timeout)` is available on the builder for waiting for async rendering.
- The terminal is `IAsyncDisposable` — use `await using`.

**Package versions used:**
- xunit 2.9.3
- xunit.runner.visualstudio 3.1.4
- Microsoft.NET.Test.Sdk 17.14.1
- coverlet.collector 6.0.4
- FluentAssertions 8.8.0
- NSubstitute 5.3.0
- Hex1b 0.87.0
