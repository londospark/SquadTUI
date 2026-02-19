using Hex1b;
using Hex1b.Automation;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ExtremeWidthTests
{
    [Theory]
    [InlineData(10, 30)]
    [InlineData(20, 30)]
    [InlineData(30, 30)]
    [InlineData(200, 30)]
    [InlineData(300, 30)]
    public async Task Dashboard_HandlesExtremeWidths(int width, int height)
    {
        await using var terminal = TestAppBuilder.Build(width: width, height: height);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(300);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Dashboard_HandlesExtremeHeights(int height)
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: height);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(300);

        var snapshot = terminal.CreateSnapshot();
        Assert.NotNull(snapshot);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
