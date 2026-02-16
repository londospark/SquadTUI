using FluentAssertions;
using Hex1b;
using Hex1b.Automation;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class DashboardScreenTests
{
    [Fact]
    public async Task Dashboard_RendersTeamMemberCount()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Members:").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_RendersTaskSummary()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Tasks:").Should().BeTrue();
        snapshot.ContainsText("in progress").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_RendersRecentActivity()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Recent Activity").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_RendersRecentDecisions()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Recent Decisions").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
