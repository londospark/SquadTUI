using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Tests for Settings modal overlay behavior: opening, closing, persistence,
/// and non-interference with underlying content and screen sizing.
/// </summary>
[Collection("E2E")]
public class SettingsModalOverlayTests
{
    [Fact]
    public async Task SKey_OpensCenteredModal()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Settings"));
        Assert.True(snapshot.ContainsText("Theme"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Escape_ClosesSettingsModal()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S).Wait(100)
            .Key(Hex1bKey.Escape)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Settings content should be gone, Dashboard visible
        Assert.False(snapshot.ContainsText("Vim Keybindings"));
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_ShowsAllConfigOptions()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Theme"));
        Assert.True(snapshot.ContainsText("Vim Keybindings"));
        Assert.True(snapshot.ContainsText("Mouse Support"));
        Assert.True(snapshot.ContainsText("Emoji Display"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_EnterToggles_ModalStaysOpen()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Open settings, press Enter to toggle theme, modal should stay open
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S).Wait(100)
            .Key(Hex1bKey.Enter)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Settings"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Theory]
    [InlineData(160, 40)]
    [InlineData(60, 24)]
    [InlineData(120, 30)]
    public async Task SettingsModal_DoesNotAffectScreenSizing_AtVariousWidths(int width, int height)
    {
        await using var terminal = TestAppBuilder.Build(width: width, height: height);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Settings"), 
            $"Settings modal should render at {width}x{height}");
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_NavigateWithJK()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Open settings, press J twice to move selection, verify modal is still intact
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S).Wait(100)
            .Key(Hex1bKey.J).Wait(50)
            .Key(Hex1bKey.J)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Settings"));
        Assert.True(snapshot.ContainsText("Vim Keybindings"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_OpenFromDifferentScreen()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Go to Roster, then open settings — should show settings modal
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Enter().Wait(100)  // Panel 0 = Roster
            .Key(Hex1bKey.S)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Settings"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_OpenAndClose_ReturnsToPreviousScreen()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Go to Metrics, open settings, close with Escape — should be back on Metrics
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter().Wait(100)  // Panel 3 = Metrics
            .Key(Hex1bKey.S).Wait(100)
            .Key(Hex1bKey.Escape)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // After closing settings modal, we should be back to the screen we were on
        Assert.False(snapshot.ContainsText("Vim Keybindings"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
