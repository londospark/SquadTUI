using FluentAssertions;
using Hex1b;
using Hex1b.Automation;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class ResponsiveLayoutTests
{
    [Fact]
    public async Task Dashboard_At60Cols_ShowsNarrowLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 60, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Narrow layout shows SquadTUI header and Recent/Decisions sections
        snapshot.ContainsText("SquadTUI").Should().BeTrue();
        snapshot.ContainsText("Recent").Should().BeTrue();
        snapshot.ContainsText("Decisions").Should().BeTrue();
        // Wide layout panels should NOT appear
        snapshot.ContainsText("Team Roster").Should().BeFalse();
        snapshot.ContainsText("Sprint Metrics").Should().BeFalse();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At80Cols_ShowsMediumLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 80, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Medium layout shows Dashboard header and Team section
        snapshot.ContainsText("SquadTUI Dashboard").Should().BeTrue();
        snapshot.ContainsText("Team").Should().BeTrue();
        snapshot.ContainsText("Decisions").Should().BeTrue();
        // Wide layout full titles should NOT appear
        snapshot.ContainsText("Team Roster").Should().BeFalse();
        snapshot.ContainsText("Sprint Metrics").Should().BeFalse();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At100Cols_ShowsMediumLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 100, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("SquadTUI Dashboard").Should().BeTrue();
        snapshot.ContainsText("Team").Should().BeTrue();
        // Wide layout titles should NOT appear
        snapshot.ContainsText("Team Roster").Should().BeFalse();
        snapshot.ContainsText("Sprint Metrics").Should().BeFalse();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At120Cols_ShowsWideLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 120, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();
        snapshot.ContainsText("Activity").Should().BeTrue();
        snapshot.ContainsText("Decisions").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At160Cols_ShowsWideLayout()
    {
        await using var terminal = TestAppBuilder.Build(width: 160, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Team Roster").Should().BeTrue();
        snapshot.ContainsText("Activity").Should().BeTrue();
        snapshot.ContainsText("Sprint Metrics").Should().BeTrue();
        snapshot.ContainsText("Velocity:").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task Dashboard_At40Cols_RendersWithoutCrashing()
    {
        await using var terminal = TestAppBuilder.Build(width: 40, height: 30);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("SquadTUI").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
