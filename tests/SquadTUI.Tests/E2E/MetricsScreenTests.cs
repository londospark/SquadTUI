using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class MetricsScreenTests
{
    [Fact]
    public async Task Metrics_ShowsSprintOverview()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D6)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Sprint Metrics").Should().BeTrue();
        snapshot.ContainsText("Sprint Overview").Should().BeTrue();
        snapshot.ContainsText("Completion Rate:").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Metrics_ShowsVelocityViewByDefault()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D6)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Velocity View").Should().BeTrue();
        snapshot.ContainsText("Avg Velocity:").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Metrics_ShowsSprintContributions()
    {
        await using var terminal = TestAppBuilder.Build(width: 160, height: 50);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D6)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team size:").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Metrics_VKeyTogglesBurndownView()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var navSequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D6)
            .Build();
        await navSequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var toggleSequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.V)
            .Build();
        await toggleSequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Burndown View").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Metrics_VKeyTogglesBackToVelocityView()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var navSequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D6)
            .Build();
        await navSequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var toggleSequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.V)
            .Build();
        await toggleSequence.ApplyAsync(terminal);
        await Task.Delay(200);
        await toggleSequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Velocity View").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
