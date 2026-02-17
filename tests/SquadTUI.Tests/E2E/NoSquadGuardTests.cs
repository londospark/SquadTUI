using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class NoSquadGuardTests
{
    [Theory]
    [InlineData(Hex1bKey.Enter)]
    [InlineData(Hex1bKey.RightArrow)]
    [InlineData(Hex1bKey.LeftArrow)]
    public async Task NoSquadScreen_NavigationKeys_DoNotNavigateAway(Hex1bKey key)
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
        Assert.True(snapshot.ContainsText("Welcome to SquadTUI"),
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
        Assert.True(snapshot.ContainsText("Welcome to SquadTUI"),
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
            Assert.True(snapshot1.ContainsText("Welcome to SquadTUI"));

            // Press C to create squad structure
            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.C)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(300);

            // Should now be on Dashboard
            var snapshot2 = terminal.CreateSnapshot();
            Assert.True(snapshot2.ContainsText("Dashboard"),
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

            // Verify .squad structure was created (new directory name)
            Assert.True(Directory.Exists(Path.Combine(tempDir, ".squad")));
            Assert.True(Directory.Exists(Path.Combine(tempDir, ".squad", "agents")));
            Assert.True(File.Exists(Path.Combine(tempDir, ".squad", "team.md")));
            Assert.True(File.Exists(Path.Combine(tempDir, ".squad", "decisions.md")));

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
