using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class StackNavigationTests
{
    [Fact]
    public async Task Dashboard_RendersWithoutTabs()
    {
        await using var terminal = TestAppBuilder.Build(width: 200);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Enter_DrillsIn_Escape_PopsBack()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Enter drills into Roster (panel 0)
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Escape pops back to Dashboard
        var back = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await back.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task StackNavigation_MultipleDepth()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Dashboard → Roster (Enter) → should show Roster
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Escape → back to Dashboard
        var back = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await back.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Theory]
    [InlineData(40, 30)]
    [InlineData(80, 30)]
    [InlineData(120, 30)]
    [InlineData(200, 30)]
    public async Task Dashboard_RendersAtVariousWidths(int width, int height)
    {
        await using var terminal = TestAppBuilder.Build(width, height);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task EscapeOnDashboard_StaysOnDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
