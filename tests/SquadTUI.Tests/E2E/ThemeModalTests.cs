using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ThemeModalTests
{
    // PressS_OpensSettingsModal consolidated to AppNavigationTests

    [Fact]
    public async Task SettingsModal_ShowsSettingsList()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Ocean"));
        Assert.True(snapshot.ContainsText("Vim Keybindings"));
        Assert.True(snapshot.ContainsText("Emoji Display"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_EscapeClosesModal()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Wait(100)
            .Key(Hex1bKey.Escape)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.False(snapshot.ContainsText("Vim Keybindings"));
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_ShowsNavigationHints()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Navigate"));
        Assert.True(snapshot.ContainsText("Toggle"));
        Assert.True(snapshot.ContainsText("Close"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_EnterTogglesAndStaysOpen()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Wait(100)
            .Key(Hex1bKey.Enter)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Settings modal stays open after toggling
        Assert.True(snapshot.ContainsText("Settings"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task TKey_StillCyclesThemes()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // T should cycle themes without opening modal
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.T)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));
        Assert.False(snapshot.ContainsText("Theme Selection"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
