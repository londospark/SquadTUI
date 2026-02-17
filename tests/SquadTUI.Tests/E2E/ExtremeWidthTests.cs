using Hex1b;
using Hex1b.Automation;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ExtremeWidthTests
{
    [Fact]
    public async Task Dashboard_At10Cols_HandlesGracefully()
    {
        await using var terminal = TestAppBuilder.Build(width: 10, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(300);

        // Should not crash — just verify the terminal renders something
        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At20Cols_MinimumViable()
    {
        await using var terminal = TestAppBuilder.Build(width: 20, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(300);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At30Cols_NarrowLayoutRenders()
    {
        await using var terminal = TestAppBuilder.Build(width: 30, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(300);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);
        // Narrow layout shows SquadTUI header
        Assert.True(snapshot.ContainsText("SquadTUI"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At200Cols_WideFillsSpace()
    {
        await using var terminal = TestAppBuilder.Build(width: 200, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Wide layout should show team roster and activity panels
        Assert.True(snapshot.ContainsText("Team Roster"));
        Assert.True(snapshot.ContainsText("Activity"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At300Cols_UltraWideDoesNotBreak()
    {
        await using var terminal = TestAppBuilder.Build(width: 300, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));
        Assert.True(snapshot.ContainsText("Activity"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At5Rows_ShortTerminal()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 5);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(300);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At10Rows_SmallHeight()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 10);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At50Rows_TallTerminal()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 50);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At100Rows_VeryTallTerminal()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 100);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
