---
name: "hex1b-tui-testing"
description: "Patterns for testing Hex1b terminal UIs with headless execution and snapshot assertions"
domain: "testing"
confidence: "medium"
source: "earned"
tools:
  - name: "powershell"
    description: "Run dotnet test commands"
    when: "Executing test suite or checking test results"
---

## Context

When testing .NET applications built with the Hex1b TUI framework, use headless terminal execution with snapshot-based assertions. This applies to any Hex1b-based TUI project, not just specific applications.

## Patterns

### Headless Terminal Setup

```csharp
var terminal = Hex1bTerminal.CreateBuilder()
    .WithHex1bApp((app, options) => ctx => /* your UI */)
    .WithHeadless()
    .WithDimensions(width, height)
    .Build();
```

Key points:
- Use `WithHeadless()` to run without actual terminal
- `WithDimensions(w, h)` sets terminal size for responsive layout testing
- Common sizes: 120x30 (default), 80x24 (narrow), 160x50 (wide)

### Test Lifecycle

```csharp
await using var terminal = TestAppBuilder.Build();
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var runTask = terminal.RunAsync(cts.Token);
await Task.Delay(200); // Allow initial render

// Test interactions here

cts.Cancel();
try { await runTask; } catch (OperationCanceledException) { }
```

Key points:
- Terminal is `IAsyncDisposable` — use `await using`
- `RunAsync(ct)` with timeout prevents hung tests
- `await Task.Delay(200)` after interactions for rendering to complete

### Input Sequences

```csharp
var sequence = new Hex1bTerminalInputSequenceBuilder()
    .Key(Hex1bKey.D2)
    .Wait(100)
    .Key(Hex1bKey.Enter)
    .Build();
await sequence.ApplyAsync(terminal);
await Task.Delay(200);
```

Use `Hex1bTerminalInputSequenceBuilder` (not `Hex1bInputSequenceBuilder`).

### Snapshot Assertions

```csharp
var snapshot = terminal.CreateSnapshot();
snapshot.ContainsText("Expected Text").Should().BeTrue();
var fullText = snapshot.GetText();
```

Available methods:
- `ContainsText(string)` — checks if text exists anywhere
- `GetText()` — returns full terminal content
- `GetLine(int)` — returns specific line
- `GetNonEmptyLines()` — returns non-blank lines

### E2E Test Organization

Mark E2E tests with `[Collection("E2E")]` to prevent parallel execution issues:

```csharp
[Collection("E2E")]
public class MyE2ETests
{
    [Fact]
    public async Task TestSomething()
    {
        // Test code
    }
}
```

### TestAppBuilder Pattern

Create a `TestAppBuilder` class that mirrors `Program.cs` structure:

```csharp
public static class TestAppBuilder
{
    public static Hex1bTerminal Build(int width = 120, int height = 30)
    {
        var state = new AppState();
        return Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) => ctx => /* replicate Program.cs layout */)
            .WithHeadless()
            .WithDimensions(width, height)
            .Build();
    }
}
```

This ensures E2E tests use the exact same UI structure as production.

## Examples

### Test Navigation

```csharp
[Fact]
public async Task Press2_NavigatesToRoster()
{
    await using var terminal = TestAppBuilder.Build();
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    var runTask = terminal.RunAsync(cts.Token);
    await Task.Delay(200);

    var sequence = new Hex1bTerminalInputSequenceBuilder()
        .Key(Hex1bKey.D2)
        .Build();
    await sequence.ApplyAsync(terminal);
    await Task.Delay(200);

    var snapshot = terminal.CreateSnapshot();
    snapshot.ContainsText("Team Roster").Should().BeTrue();

    cts.Cancel();
    try { await runTask; } catch (OperationCanceledException) { }
}
```

### Test Responsive Layout

```csharp
[Theory]
[InlineData(80, 24)]   // Narrow
[InlineData(120, 40)]  // Wide
public async Task DifferentSizes_RenderNavBar(int width, int height)
{
    await using var terminal = TestAppBuilder.Build(width, height);
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    var runTask = terminal.RunAsync(cts.Token);
    await Task.Delay(200);

    var snapshot = terminal.CreateSnapshot();
    snapshot.ContainsText("Dashboard").Should().BeTrue();

    cts.Cancel();
    try { await runTask; } catch (OperationCanceledException) { }
}
```

## Anti-Patterns

❌ **Don't** use `Hex1bInputSequenceBuilder` — it doesn't exist. Use `Hex1bTerminalInputSequenceBuilder`.

❌ **Don't** forget `await Task.Delay()` after input sequences — rendering is async.

❌ **Don't** run E2E tests in parallel — terminal state can interfere. Use `[Collection("E2E")]`.

❌ **Don't** use blocking `.Wait()` or `.Result` — Hex1b is fully async. Use `await` everywhere.

❌ **Don't** test rendering logic in unit tests if it requires `WidgetContext` — defer to E2E tests with snapshot assertions.

❌ **Don't** forget timeout on `RunAsync()` — tests can hang indefinitely without `CancellationToken`.
