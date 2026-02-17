using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Tests for the live dashboard update timer and refresh behavior.
/// Validates LastRefreshTime display, RedrawAfter timer, and
/// non-interference with file watcher.
/// </summary>
[Collection("E2E")]
public class UpdateTimerTests
{
    [Fact]
    public async Task Dashboard_ShowsRefreshTimestamp()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Dashboard should show "Updated:" timestamp
        Assert.True(snapshot.ContainsText("Updated:"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_ShowsLiveIndicator()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // IsLiveEnabled is true by default — should show LIVE indicator
        Assert.True(snapshot.ContainsText("LIVE"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_RedrawAfter_TimerDoesNotCrash()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
        var runTask = terminal.RunAsync(cts.Token);

        // Wait longer than the RedrawAfter(3000) timer to ensure
        // the automatic redraw fires at least once without crashing
        await Task.Delay(4000);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));
        Assert.True(snapshot.ContainsText("Updated:"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_TimerDoesNotInterfereWithNavigation()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate to Roster, wait 4 seconds (past redraw timer), verify still on Roster
        var seq = new Hex1bTerminalInputSequenceBuilder()
            .Enter()  // Panel 0 = Roster
            .Build();
        await seq.ApplyAsync(terminal);
        await Task.Delay(4000);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}

/// <summary>
/// Tests for AppState timer and refresh state management.
/// </summary>
public class UpdateTimerStateTests
{
    [Fact]
    public void LastRefreshTime_DefaultsToNow()
    {
        var before = DateTime.Now.AddSeconds(-1);
        var state = new SquadTUI.Screens.AppState();
        var after = DateTime.Now.AddSeconds(1);

        Assert.InRange(state.LastRefreshTime, before, after);
    }

    [Fact]
    public void IsLiveEnabled_DefaultsToTrue()
    {
        var state = new SquadTUI.Screens.AppState();
        Assert.True(state.IsLiveEnabled);
    }

    [Fact]
    public void HasPendingRefresh_DefaultsToFalse()
    {
        var state = new SquadTUI.Screens.AppState();
        Assert.False(state.HasPendingRefresh);
    }

    [Fact]
    public void PendingRefresh_ClearedAfterConsumption()
    {
        var state = new SquadTUI.Screens.AppState();
        state.HasPendingRefresh = true;

        // Simulate what DashboardScreen.Render does
        if (state.HasPendingRefresh)
        {
            state.HasPendingRefresh = false;
            state.LastRefreshTime = DateTime.Now;
        }

        Assert.False(state.HasPendingRefresh);
    }
}
