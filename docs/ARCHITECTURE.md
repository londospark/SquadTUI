# 🏗️ Architecture Overview

A concise guide to SquadTUI's project structure, patterns, and design decisions.

## Project Structure

```
SquadTUI/
├── src/SquadTUI/              # Main application
│   ├── Program.cs             # Entry point — builds Hex1b terminal, wires keybindings
│   ├── Models/                # Immutable record types for domain data
│   ├── Services/              # Data loading, file I/O, squad detection
│   ├── Screens/               # Screen components and navigation state
│   ├── Rendering/             # ANSI text styling helpers (PanelRenderer)
│   └── Themes/                # ThemeManager — 4 themes with accent colors
├── tests/SquadTUI.Tests/      # Test suite
│   ├── Unit/                  # Unit tests for models, services, rendering
│   ├── Integration/           # Integration tests (file I/O, CI pipeline)
│   └── E2E/                   # End-to-end tests using Hex1b headless mode
├── docs/                      # Documentation and screenshots
├── scripts/                   # Build and capture scripts
└── SquadTUI.slnx              # Solution file
```

## Key Patterns

### Screen System

SquadTUI uses a single `AppState` object as the source of truth. Each screen is a **static class** with a `Render` method that takes a Hex1b `WidgetContext`, the `AppState`, and the `Hex1bApp` instance:

```csharp
public static class DashboardScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        // Build and return widget tree
    }
}
```

**`AppState`** tracks:
- `CurrentScreen` — which screen enum is active
- `PreviousScreen` — for back navigation (e.g., Help → return)
- Selection indices per list screen (`RosterSelectedIndex`, etc.)
- Loaded data (`Members`, `Decisions`, `Skills`, `LogEntries`, `Dashboard`)
- Theme index, settings, loading/error state

**Navigation** is handled via global `WithInputBindings` in `Program.cs`. Keys like `1`–`6` set `CurrentScreen` directly; `Escape` walks back through the screen hierarchy.

### Widget Composition (Hex1b)

Screens compose layouts using Hex1b's declarative API:

- **`VStack`** — vertical layout (main content flow)
- **`HStack`** — horizontal layout (side-by-side panels, nav bar)
- **`List`** — scrollable list with selection highlighting
- **`Button`** — clickable element (used in NavBar tabs)
- **`Text`** — styled text with ANSI escape sequences

Example from NavBar:
```csharp
return v.HStack(h =>
{
    foreach (var item in items)
        widgets.Add(h.Button(label).OnClick(_ => { state.CurrentScreen = screen; }));
    return widgets.ToArray();
});
```

### ThemeManager

Four themes defined in `Themes/ThemeManager.cs`:

| Theme | Background | Accent |
|-------|-----------|--------|
| Ocean | Deep navy (`#0D1117`) | Cyan |
| Heist | Dark charcoal (`#1A1A2E`) | Yellow |
| Sunset | Warm dark (`#1A1A1A`) | Magenta |
| HighContrast | Pure black | White |

Each theme configures Hex1b's `Hex1bTheme` with `GlobalTheme`, `BorderTheme`, `ListTheme`, `ScrollTheme`, and `SplitterTheme` tokens. Borders are set to space characters (`WithModernBorders`) creating a clean, borderless look.

Accent colors are provided as raw ANSI foreground codes via `GetAccentCode()` for inline text styling through `PanelRenderer`.

### Services

Services live in `Services/` and follow an interface + implementation pattern:

| Service | Purpose |
|---------|---------|
| `TeamService` / `ITeamService` | Load squad member data from `.ai-team/` |
| `DecisionService` / `IDecisionService` | Parse decision entries from markdown |
| `SkillService` / `ISkillService` | Aggregate skills from member charters |
| `OrchestrationLogService` / `IOrchestrationLogService` | Read orchestration log entries |
| `SquadDataProvider` / `ISquadDataProvider` | Unified data access layer |
| `SquadDetector` | Detect `.ai-team/` directory presence |
| `SettingsService` | Load/save user preferences (JSON) |
| `ServiceProvider` | Simple service locator (singleton) |
| `DataBridge` | Async data loading coordinator |

### Models

All models are immutable C# `record` types in `Models/`:

- `SquadMember` — name, role, status, skills, charter path
- `DecisionEntry` — title, date, context, rationale
- `Skill` — name, category, members who have it
- `OrchestrationLogEntry` — timestamp, type, description
- `DashboardData` — aggregated metrics for the dashboard
- `SquadTask` / `SquadTaskStatus` — task tracking (avoids `Task` name collision)
- `AppSettings` — persisted user preferences
- `MemberStatus` — enum (Active, Idle, Working, Offline)

## Test Infrastructure

Tests use **xUnit** and are organized by scope:

### Unit Tests (`tests/SquadTUI.Tests/Unit/`)
Standard xUnit tests for models, services, and rendering helpers. No external dependencies.

### Integration Tests (`tests/SquadTUI.Tests/Integration/`)
Test file I/O, data parsing from real `.ai-team/` structures, and CI pipeline validation.

### E2E Tests (`tests/SquadTUI.Tests/E2E/`)
Use Hex1b's **headless testing** mode via `TestAppBuilder`:

```csharp
public static class TestAppBuilder
{
    public static Hex1bTerminal Build(int width = 120, int height = 30)
    {
        // Creates a headless Hex1b terminal with full app wired up
        // Uses .WithHeadless() and .WithDimensions() for deterministic testing
    }
}
```

This enables testing screen rendering, keyboard navigation, theme switching, and layout behavior without a real terminal.

### Running Tests

```bash
# Run all tests (excluding CI meta tests)
dotnet test --filter "FullyQualifiedName!~DotnetBuild&FullyQualifiedName!~DotnetTest"
```

## Stack

- **.NET 10** / C# 13
- **Hex1b 0.87.0** — declarative terminal UI framework
- **xUnit** — test framework
- **6-platform CI** — Windows/macOS/Linux × x64/ARM64
