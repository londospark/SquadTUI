using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class AppNavigationTests
{
    [Fact]
    public async Task App_StartsAndShowsDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Press2_NavigatesToRoster()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Press3_NavigatesToDecisions()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D3)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Decisions").Should().BeTrue();
        snapshot.ContainsText("Screen: Decisions").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Press4_NavigatesToSkills()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D4)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Skills").Should().BeTrue();
        snapshot.ContainsText("Screen: Skills").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Press5_NavigatesToActivityLog()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D5)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Activity Log").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Press6_NavigatesToMetrics()
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
        snapshot.ContainsText("Metrics").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Press1_BackToDashboard()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2)
            .Wait(100)
            .Key(Hex1bKey.D1)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();
        snapshot.ContainsText("Screen: Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task PressQ_AppExitsCleanly()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Q)
            .Build();
        await sequence.ApplyAsync(terminal);

        var completed = await Task.WhenAny(runTask, Task.Delay(2000));
        completed.Should().Be(runTask, "app should exit when Q is pressed");

        cts.Cancel();
    }
}
