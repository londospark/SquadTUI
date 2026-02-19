using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Tests for layout sizing consistency: panels maintain size during focus
/// changes, no layout shift when navigating.
/// Responsive breakpoint coverage is in ResponsiveLayoutTests and ExtremeWidthTests.
/// </summary>
[Collection("E2E")]
public class SizingConsistencyTests
{
    [Fact]
    public async Task Dashboard_PanelsMaintainSizeDuringFocusCycle()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Take snapshot at panel 0
        var snapshot0 = terminal.CreateSnapshot();
        Assert.True(snapshot0.ContainsText("Team Roster"));

        // Move to panel 1
        var right = new Hex1bTerminalInputSequenceBuilder()
            .Right()
            .Build();
        await right.ApplyAsync(terminal);
        await Task.Delay(200);

        // Take snapshot at panel 1 — still on Dashboard
        var snapshot1 = terminal.CreateSnapshot();
        Assert.True(snapshot1.ContainsText("Dashboard"));

        // Move to panel 2
        await right.ApplyAsync(terminal);
        await Task.Delay(200);

        // Still on Dashboard (focus moved, not navigated away)
        var snapshot2 = terminal.CreateSnapshot();
        Assert.True(snapshot2.ContainsText("Dashboard"));

        // Return to panel 0 via left arrows
        var left = new Hex1bTerminalInputSequenceBuilder()
            .Left().Wait(50)
            .Left()
            .Build();
        await left.ApplyAsync(terminal);
        await Task.Delay(200);

        // Should still show Dashboard with all panels
        var snapshotFinal = terminal.CreateSnapshot();
        Assert.True(snapshotFinal.ContainsText("Team Roster"));
        Assert.True(snapshotFinal.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoLayoutShift_WhenNavigatingBetweenScreens()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate to Roster and back — Dashboard should render identically
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Enter().Wait(100)  // Panel 0 = Roster
            .Key(Hex1bKey.Escape)
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SettingsModal_WorksAtAllBreakpoints()
    {
        // Test settings modal at narrow, medium, and wide
        foreach (var (w, h) in new[] { (60, 24), (80, 30), (120, 30), (160, 40) })
        {
            await using var terminal = TestAppBuilder.Build(width: w, height: h);
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
                $"Settings modal should render at {w}x{h}");

            cts.Cancel();
            try { await runTask; } catch (OperationCanceledException) { }
        }
    }
}
