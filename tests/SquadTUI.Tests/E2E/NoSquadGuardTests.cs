using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class NoSquadGuardTests
{
    [Theory]
    [InlineData(Hex1bKey.D1)]
    [InlineData(Hex1bKey.D2)]
    [InlineData(Hex1bKey.D3)]
    [InlineData(Hex1bKey.D4)]
    [InlineData(Hex1bKey.D5)]
    [InlineData(Hex1bKey.D6)]
    public async Task NoSquadScreen_NumberKeys_DoNotNavigateAway(Hex1bKey key)
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(key)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Should still be on NoSquad screen — look for welcome text
        snapshot.ContainsText("Welcome to SquadTUI").Should().BeTrue(
            $"pressing {key} should not navigate away from NoSquad screen");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadScreen_PressS_DoesNotGoToSettings()
    {
        await using var terminal = TestAppBuilder.Build(squadDetected: false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Welcome to SquadTUI").Should().BeTrue(
            "pressing S should not navigate away from NoSquad screen");

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task NoSquadScreen_PressC_NavigatesToDashboard()
    {
        // Use a temp directory so squad creation doesn't pollute the project
        var tempDir = Path.Combine(Path.GetTempPath(), $"squadtui-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var originalDir = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(tempDir);

            await using var terminal = TestAppBuilder.Build(squadDetected: false);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var runTask = terminal.RunAsync(cts.Token);
            await Task.Delay(200);

            // Verify we start on NoSquad
            var snapshot1 = terminal.CreateSnapshot();
            snapshot1.ContainsText("Welcome to SquadTUI").Should().BeTrue();

            // Press C to create squad structure
            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.C)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(300);

            // Should now be on Dashboard
            var snapshot2 = terminal.CreateSnapshot();
            snapshot2.ContainsText("Dashboard").Should().BeTrue(
                "pressing C should create squad and navigate to Dashboard");

            cts.Cancel();
            try { await runTask; } catch (OperationCanceledException) { }
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public async Task NoSquadScreen_PressC_CreatesAiTeamDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squadtui-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var originalDir = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(tempDir);

            await using var terminal = TestAppBuilder.Build(squadDetected: false);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var runTask = terminal.RunAsync(cts.Token);
            await Task.Delay(200);

            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.C)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(300);

            // Verify .ai-team structure was created
            Directory.Exists(Path.Combine(tempDir, ".ai-team")).Should().BeTrue();
            Directory.Exists(Path.Combine(tempDir, ".ai-team", "agents")).Should().BeTrue();
            File.Exists(Path.Combine(tempDir, ".ai-team", "team.md")).Should().BeTrue();
            File.Exists(Path.Combine(tempDir, ".ai-team", "decisions.md")).Should().BeTrue();

            cts.Cancel();
            try { await runTask; } catch (OperationCanceledException) { }
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }
}
