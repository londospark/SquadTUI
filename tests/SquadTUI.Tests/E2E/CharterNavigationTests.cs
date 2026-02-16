using FluentAssertions;
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

        // Navigate to Roster
        var toRoster = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Build();
        await toRoster.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();

        // Escape from Roster should go to Dashboard
        var backToDashboard = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await backToDashboard.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Screen: Dashboard").Should().BeTrue();

        // Navigate to Decisions
        var toDecisions = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D3)
            .Build();
        await toDecisions.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Decisions").Should().BeTrue();

        // Escape from Decisions should go to Dashboard
        var backAgain = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Escape)
            .Build();
        await backAgain.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Screen: Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
