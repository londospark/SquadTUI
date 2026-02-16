using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class CharterNavigationTests
{
    [Fact]
    public async Task FullNavigation_Roster_MemberDetail_Charter_Back()
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

        // Enter to go to Member Detail
        var toDetail = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await toDetail.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Screen: MemberDetail").Should().BeTrue();

        // E to go to Charter
        var toCharter = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.E)
            .Build();
        await toCharter.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Charter").Should().BeTrue();
        snapshot.ContainsText("Screen: Charter").Should().BeTrue();

        // B to go back to Member Detail
        var backToDetail = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.B)
            .Build();
        await backToDetail.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Screen: MemberDetail").Should().BeTrue();

        // B to go back to Roster
        var backToRoster = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.B)
            .Build();
        await backToRoster.ApplyAsync(terminal);
        await Task.Delay(200);

        snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();
        snapshot.ContainsText("Screen: Roster").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
