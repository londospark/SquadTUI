using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Stack navigation tests. Basic panel drill-in/escape and Escape-at-Dashboard
/// tests live in AppNavigationTests and NavigationEdgeCaseTests respectively.
/// </summary>
[Collection("E2E")]
public class StackNavigationTests
{
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
        Assert.True(snapshot.ContainsText("SquadTUI"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
