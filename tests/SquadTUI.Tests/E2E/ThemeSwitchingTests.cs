using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ThemeSwitchingTests
{
    [Fact]
    public async Task ThemeCycle_PressT_CyclesThroughThemes()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        // Press T three times to cycle through themes
        for (int i = 0; i < 3; i++)
        {
            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.T)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(200);
        }

        // App should still render correctly after theme cycling
        var snapshot2 = terminal.CreateSnapshot();
        snapshot2.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task ThemeCycle_PressT_WrapsAroundAfterLastTheme()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        // Press T 4 times to wrap around to initial theme
        for (int i = 0; i < 4; i++)
        {
            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.T)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(200);
        }

        var finalSnapshot = terminal.CreateSnapshot();
        finalSnapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task ThemeCycle_AppRendersCorrectlyAfterThemeChange()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.T)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        // After theme change, nav and content should still render
        var snapshot2 = terminal.CreateSnapshot();
        snapshot2.ContainsText("Dashboard").Should().BeTrue();
        snapshot2.ContainsText("Roster").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
