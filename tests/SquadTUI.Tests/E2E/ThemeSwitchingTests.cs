using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ThemeSwitchingTests
{
    [Theory]
    [InlineData(3, "cycle through themes")]
    [InlineData(4, "wrap around after last theme")]
    [InlineData(1, "single theme change")]
    public async Task ThemeCycle_PressT_CyclesCorrectly(int pressCount, string scenario)
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Dashboard"));

        // Press T specified number of times
        for (int i = 0; i < pressCount; i++)
        {
            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.T)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(200);
        }

        // App should still render correctly after theme cycling
        var finalSnapshot = terminal.CreateSnapshot();
        Assert.True(finalSnapshot.ContainsText("Dashboard"), 
            $"Dashboard should render after {scenario}");
        Assert.True(finalSnapshot.ContainsText("Roster"),
            $"Roster should be visible after {scenario}");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
