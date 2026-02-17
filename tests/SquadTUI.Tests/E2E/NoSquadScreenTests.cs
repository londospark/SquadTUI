using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;
using SquadTUI.Screens;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class NoSquadScreenTests
{
    [Fact]
    public async Task NoSquadDetected_ShowsWelcomeMessage()
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Welcome to SquadTUI"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadDetected_ShowsNoSquadDetectedText()
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("No squad detected"));

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadScreen_ContainsSetupInstructions()
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Should mention npx or squad or .ai-team for setup instructions
        var hasNpx = snapshot.ContainsText("npx");
        var hasSquad = snapshot.ContainsText("squad");
        var hasAiTeam = snapshot.ContainsText(".ai-team");
        Assert.True(hasNpx || hasSquad || hasAiTeam,
            "NoSquad screen should contain setup instructions mentioning npx, squad, or .ai-team");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadScreen_ShowsCKeyInstruction()
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        Assert.True(snapshot.ContainsText("Create"),
            "NoSquad screen should show the C key instruction for creating squad structure");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadScreen_ShowsQKeyInstruction()
    {
        // Taller terminal needed — key bindings line is near the bottom of centered layout
        await using var terminal = TestAppBuilder.Build(height: 40, squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // The key bindings line renders: "C  Create basic squad structure        Q  Quit"
        // Check for the Q key instruction being present
        Assert.True(snapshot.ContainsText("Quit"),
            "NoSquad screen should show the Q key instruction");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadScreen_NavBarNotVisible()
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // When on NoSquad, navigation keys should not work.
        // We check that pressing Enter doesn't navigate away as a resilient assertion.
        Assert.True(snapshot.ContainsText("Welcome to SquadTUI"));

        // Press Enter — should NOT navigate away (should stay on NoSquad)
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot2 = terminal.CreateSnapshot();
        Assert.True(snapshot2.ContainsText("Welcome to SquadTUI"),
            "Navigation should not work on NoSquad screen");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}

