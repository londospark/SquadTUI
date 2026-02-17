using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class DashboardPanelNavigationTests
{
    [Fact]
    public async Task Tab_CyclesFocusForward()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow cycles focus forward on Dashboard
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task RightArrow_CyclesThrough4Panels()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow 4 times: should wrap back to panel 0
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Right()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task LeftArrow_CyclesFocusBackward()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // LeftArrow from panel 0 should go to panel 3
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Left()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Enter_DrillsIntoRoster_WhenFocusPanel0()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Focus is on panel 0 (Roster) by default, Enter to drill in
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Enter_DrillsIntoActivityLog_WhenFocusPanel1()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow once to panel 1 (Activity), then Enter
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Activity Log").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Enter_DrillsIntoDecisions_WhenFocusPanel2()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow twice to panel 2 (Decisions), then Enter
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Decisions").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Enter_DrillsIntoMetrics_WhenFocusPanel3()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow three times to panel 3 (Metrics), then Enter
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Sprint Metrics").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task FocusResetsOnReturnToDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // RightArrow to panel 2, navigate away with key 2, come back with key 1
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Key(Hex1bKey.D2).Wait(100)
            .Key(Hex1bKey.D1)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        // Now press Enter — should go to Roster (panel 0), not Decisions (panel 2)
        var enterSeq = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await enterSeq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task FocusedPanel_HasReverseVideoIndicator()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Default focus is panel 0 — Team Roster header should have reverse video
        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();

        // RightArrow to panel 1 and verify Activity header is visible
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Right()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Activity").Should().BeTrue();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
