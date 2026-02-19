using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Extended tests for stack-based navigation: deep chains, edge cases,
/// and panel drill-in from Dashboard.
/// </summary>
[Collection("E2E")]
public class StackNavigationExtendedTests
{
    // ── Deep navigation chain ──
    // Panel drill-in/escape tests consolidated to AppNavigationTests

    [Fact]
    public async Task DeepNav_Dashboard_Roster_MemberDetail_Escape2x_ToDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Dashboard → Roster (Enter on panel 0)
        var toRoster = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await toRoster.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Roster → MemberDetail (Enter on selected member)
        var toMember = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await toMember.ApplyAsync(terminal);
        await Task.Delay(200);

        // Escape from MemberDetail → Roster
        var escapeOnce = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await escapeOnce.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Escape from Roster → Dashboard
        await escapeOnce.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task DeepNav_Dashboard_Roster_MemberDetail_Charter_Escape3x_ToDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Dashboard → Roster
        var toRoster = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await toRoster.ApplyAsync(terminal);
        await Task.Delay(200);

        // Roster → MemberDetail (Enter)
        var enter = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await enter.ApplyAsync(terminal);
        await Task.Delay(200);

        // MemberDetail → Charter (E key)
        var toCharter = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.E)
            .Build();
        await toCharter.ApplyAsync(terminal);
        await Task.Delay(200);

        var escape = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();

        // Escape from Charter → MemberDetail
        await escape.ApplyAsync(terminal);
        await Task.Delay(200);

        // Escape from MemberDetail → Roster
        await escape.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Escape from Roster → Dashboard
        await escape.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    // Escape-at-Dashboard tests consolidated to NavigationEdgeCaseTests

    // ── Stack doesn't grow unboundedly ──

    [Fact]
    public async Task RepeatedNavigation_DoesNotCrash()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate in and out 10 times — stack should not grow
        for (int i = 0; i < 10; i++)
        {
            var forward = new Hex1bTerminalInputSequenceBuilder()
                .Enter()  // Panel 0 = Roster
                .Build();
            await forward.ApplyAsync(terminal);
            await Task.Delay(100);

            var back = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.Escape)
                .Build();
            await back.ApplyAsync(terminal);
            await Task.Delay(100);
        }

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    // ── RightArrow panel cycling on Dashboard preserves state ──

    [Fact]
    public async Task RightArrow_CyclesDashboardPanels_ThenEnterDrillsIn()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter()
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Sprint Metrics"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    // ── Enter drill-in and Escape round-trip ──

    [Fact]
    public async Task EnterAndEscape_RoundTrip()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Enter Roster, Escape back, Enter Metrics via panel 3, Escape back
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Enter().Wait(100)
            .Key(Hex1bKey.Escape).Wait(100)
            .Right().Wait(50)
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter().Wait(100)
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
}
