using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class RosterScreenTests
{
    [Fact]
    public async Task Roster_ShowsMemberNames()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var navSequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Build();
        await navSequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Danny").Should().BeTrue();
        snapshot.ContainsText("Linus").Should().BeTrue();
        snapshot.ContainsText("Rusty").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Roster_ShowsPreviewPanel()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var navSequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Build();
        await navSequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Preview").Should().BeTrue();
        snapshot.ContainsText("Name:").Should().BeTrue();
        snapshot.ContainsText("Role:").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Roster_EnterNavigatesToMemberDetail()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Wait(100)
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Member").Should().BeTrue();
        snapshot.ContainsText("Screen: MemberDetail").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task MemberDetail_BackButtonReturnsToRoster()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Wait(100)
            .Enter()
            .Wait(100)
            .Key(Hex1bKey.B)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();
        snapshot.ContainsText("Screen: Roster").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
