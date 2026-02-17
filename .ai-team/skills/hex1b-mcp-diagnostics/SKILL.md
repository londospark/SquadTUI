# Hex1b MCP Diagnostics & Testing Skill

> Source: `mitchdenny/hex1b` — `src/Hex1b.McpServer/SKILL.md`

## Hex1bTerminal Builder Pattern

The modern way to create Hex1b apps uses the builder pattern:

```csharp
await using var terminal = Hex1bTerminal.CreateBuilder()
    .WithHex1bApp((app, options) =>
    {
        options.Theme = myTheme;
        options.EnableMouse = true;
        return ctx => BuildWidget(ctx);
    })
    .WithDiagnostics()
    .Build();

await terminal.RunAsync();
```

### Builder Methods
- `.WithHex1bApp((app, options) => ctx => widget)` — Configure app with sync widget builder
- `.WithHex1bApp((app, options) => async ctx => widget)` — Async widget builder
- `.WithMouse(bool enable = true)` — Enable mouse support
- `.WithDiagnostics(appName?, forceEnable?)` — Enable MCP diagnostics (auto-disabled in Release)
- `.WithHeadless()` — Run without real terminal (for testing)
- `.WithHeadless(TerminalCapabilities caps)` — Headless with custom capabilities
- `.WithDimensions(width, height)` — Set terminal dimensions
- `.WithScrollback(capacity)` — Enable scrollback buffer
- `.WithReflow()` — Enable terminal reflow on resize
- `.WithAsciinemaRecording(filePath)` — Record session to asciinema
- `.WithAsciinemaPlayback(filePath)` — Play back asciinema recording
- `.WithMetrics()` — Enable performance metrics

## Testing with Headless Mode

```csharp
[Fact]
public async Task MyWidget_Click_PerformsAction()
{
    await using var terminal = Hex1bTerminal.CreateBuilder()
        .WithHeadless(80, 24)
        .WithDiagnostics()
        .WithHex1bApp((app, options) => ctx =>
            ctx.Button("Click me", e => { /* handler */ }))
        .Build();

    await terminal.StartAsync();
    terminal.SendMouseClick(MouseButton.Left, 5, 0);
    await terminal.ProcessEventsAsync();

    var snapshot = terminal.CreateSnapshot();
    Assert.Contains("expected text", snapshot.GetText());
}
```

### Key Testing APIs
1. `WithHeadless(width, height)` — No real terminal needed
2. `terminal.SendMouseClick(button, x, y)` — Inject mouse input
3. `terminal.SendKey(key)` — Inject keyboard input
4. `terminal.ProcessEventsAsync()` — Process pending events
5. `terminal.CreateSnapshot()` — Capture screen for assertions
6. `WithDiagnostics()` — Enable for debugging test failures

## Widget/Node Architecture
- **Widgets** (`*Widget`): Immutable records describing what to render
- **Nodes** (`*Node`): Mutable classes managing state and rendering
- Reconciliation diffs widgets against nodes to preserve state

## Common Node Types
| Node | Purpose |
|------|---------|
| `ZStackNode` | Popup host, layers children on Z-axis |
| `VStackNode` / `HStackNode` | Vertical/horizontal layout |
| `ButtonNode` | Clickable button |
| `TextBlockNode` | Text display |
| `TableNode` | Data table with scrolling |
| `PickerNode` | Dropdown selection |
| `BackdropNode` | Modal backdrop with click-away |
| `AnchoredNode` | Positions popup relative to anchor |
| `ThemePanelNode` | Scoped theme mutations |
| `EffectPanelNode` | Post-processing effects |
