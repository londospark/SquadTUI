using Hex1b;
using Hex1b.Automation;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ResponsiveLayoutTests
{
    [Theory]
    [InlineData(60, 30, "SquadTUI", null)]
    [InlineData(80, 30, "SquadTUI Dashboard", null)]
    [InlineData(100, 30, "SquadTUI Dashboard", null)]
    [InlineData(120, 30, "Roster", null)]
    [InlineData(160, 30, "Team Roster", "Sprint Metrics")]
    [InlineData(40, 30, "SquadTUI", null)]
    public async Task Dashboard_RendersCorrectLayout_AtWidth(int width, int height, string expectedText, string? additionalText)
    {
        await using var terminal = TestAppBuilder.Build(width: width, height: height);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText(expectedText),
            $"Expected '{expectedText}' at {width}x{height}");

        if (additionalText is not null)
            Assert.True(snapshot.ContainsText(additionalText),
                $"Expected '{additionalText}' at {width}x{height}");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
