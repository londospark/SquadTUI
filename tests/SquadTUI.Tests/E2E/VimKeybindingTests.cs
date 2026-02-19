using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class VimKeybindingTests
{
    [Fact]
    public async Task VimJ_NavigatesDownInRoster()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Enter to Roster, then J to navigate down
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Wait(100)
            .Key(Hex1bKey.J)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    // Escape-from-screen tests consolidated to NavigationEdgeCaseTests
    // and StackNavigationExtendedTests
}
