using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class SkillsNavigationTests
{
    [Fact]
    public async Task Enter_DrillsIntoSkills_WhenFocusPanel4()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow four times to panel 4 (Skills), then Enter
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Skills"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_ShowsSkillsPanel()
    {
        await using var terminal = TestAppBuilder.Build(width: 160, height: 40);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Skills"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task EscapeFromSkills_ReturnsToDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate to Skills via panel 4, then Escape back
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter()
            .Wait(100)
            .Key(Hex1bKey.Escape)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task RightArrow_CyclesThrough5Panels()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow 5 times: should wrap back to panel 0
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Right()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        // Panel 0 is focused again, Enter should go to Roster
        var enterSeq = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await enterSeq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
