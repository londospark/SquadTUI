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
        snapshot.ContainsText("🎨 Ocean").Should().BeTrue();

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.T)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot2 = terminal.CreateSnapshot();
        snapshot2.ContainsText("🎨 Heist").Should().BeTrue();

        var sequence2 = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.T)
            .Build();
        await sequence2.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot3 = terminal.CreateSnapshot();
        snapshot3.ContainsText("🎨 Sunset").Should().BeTrue();

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
        snapshot.ContainsText("🎨 Ocean").Should().BeTrue();

        for (int i = 0; i < 4; i++)
        {
            var sequence = new Hex1bTerminalInputSequenceBuilder()
                .Key(Hex1bKey.T)
                .Build();
            await sequence.ApplyAsync(terminal);
            await Task.Delay(200);
        }

        var finalSnapshot = terminal.CreateSnapshot();
        finalSnapshot.ContainsText("🎨 Ocean").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task InfoBar_AlwaysShowsCurrentTheme()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("🎨 Ocean").Should().BeTrue();
        snapshot.ContainsText("T:Theme").Should().BeTrue();

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.T)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot2 = terminal.CreateSnapshot();
        snapshot2.ContainsText("🎨 Heist").Should().BeTrue();
        snapshot2.ContainsText("T:Theme").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
