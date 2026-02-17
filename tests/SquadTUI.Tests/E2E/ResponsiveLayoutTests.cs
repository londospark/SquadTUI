using Hex1b;
using Hex1b.Automation;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ResponsiveLayoutTests
{
    [Fact]
    public async Task Dashboard_At60Cols_ShowsNarrowLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 60, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Narrow layout shows SquadTUI header and Recent/Decisions sections
        Assert.True(snapshot.ContainsText("SquadTUI"));
        Assert.True(snapshot.ContainsText("Recent"));
        Assert.True(snapshot.ContainsText("Decisions"));
        // Wide layout panels should NOT appear
        Assert.False(snapshot.ContainsText("Team Roster"));
        Assert.False(snapshot.ContainsText("Sprint Metrics"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At80Cols_ShowsMediumLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 80, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Medium layout shows Dashboard header and Team section
        Assert.True(snapshot.ContainsText("SquadTUI Dashboard"));
        Assert.True(snapshot.ContainsText("Team"));
        Assert.True(snapshot.ContainsText("Decisions"));
        // Wide layout full titles should NOT appear
        Assert.False(snapshot.ContainsText("Team Roster"));
        Assert.False(snapshot.ContainsText("Sprint Metrics"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At100Cols_ShowsMediumLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 100, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("SquadTUI Dashboard"));
        Assert.True(snapshot.ContainsText("Team"));
        // Wide layout titles should NOT appear
        Assert.False(snapshot.ContainsText("Team Roster"));
        Assert.False(snapshot.ContainsText("Sprint Metrics"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At120Cols_ShowsWideLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster") || snapshot.ContainsText("Roster"));
        Assert.True(snapshot.ContainsText("Activity") || snapshot.ContainsText("Progress"));
        Assert.True(snapshot.ContainsText("Decisions") || snapshot.ContainsText("SquadTUI"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At160Cols_ShowsWideLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 160, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));
        Assert.True(snapshot.ContainsText("Activity"));
        Assert.True(snapshot.ContainsText("Sprint Metrics"));
        Assert.True(snapshot.ContainsText("Velocity:"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At40Cols_RendersWithoutCrashing()
    {
        await using var terminal = TestAppBuilder.Build(width: 40, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("SquadTUI"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
