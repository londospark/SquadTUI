using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class CharterNavigationTests
{
    [Fact]
    public async Task EscapeNavigation_BackFromScreensToDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate to Roster via Enter (panel 0)
        var toRoster = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await toRoster.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Escape from Roster should go to Dashboard
        var backToDashboard = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await backToDashboard.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        // Navigate to Decisions via panel 2 (Right x2, Enter)
        var toDecisions = new Hex1bTerminalInputSequenceBuilder()
            .Right().Wait(50)
            .Right().Wait(50)
            .Enter()
            .Build();
        await toDecisions.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Decisions"));

        // Escape from Decisions should go to Dashboard
        var backAgain = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await backAgain.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Team Roster"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
