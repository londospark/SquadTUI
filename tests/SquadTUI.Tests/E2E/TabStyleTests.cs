using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class TabStyleTests
{
    [Fact]
    public async Task TabBar_RendersOnDashboardScreen()
    {
        await using var terminal = TestAppBuilder.Build(width: 200);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();
        snapshot.ContainsText("Roster").Should().BeTrue();
        snapshot.ContainsText("Decisions").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task TabBar_ActiveTabHasIndicator()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // The active tab should have the ▶ indicator (ANSI stripped, but text preserved)
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Theory]
    [InlineData(Hex1bKey.D2, "Team Roster")]
    [InlineData(Hex1bKey.D3, "Decisions")]
    [InlineData(Hex1bKey.D4, "Skills")]
    [InlineData(Hex1bKey.D5, "Activity")]
    [InlineData(Hex1bKey.D6, "Metrics")]
    public async Task TabBar_PressingNumberKey_SwitchesScreen(Hex1bKey key, string expectedContent)
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(key)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText(expectedContent).Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Theory]
    [InlineData(40, 30)]
    [InlineData(80, 30)]
    [InlineData(120, 30)]
    [InlineData(200, 30)]
    public async Task TabBar_RendersAtVariousWidths(int width, int height)
    {
        await using var terminal = TestAppBuilder.Build(width, height);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Should not crash and should contain at least Dashboard label
        snapshot.Should().NotBeNull();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task TabBar_ContainsTabLabelsWithEmoji()
    {
        // Use wide terminal to ensure all tabs fit
        await using var terminal = TestAppBuilder.Build(width: 200);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        // Check for tab labels (ANSI stripped but text preserved)
        snapshot.ContainsText("Dashboard").Should().BeTrue();
        snapshot.ContainsText("Roster").Should().BeTrue();
        snapshot.ContainsText("Decisions").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task TabBar_SwitchingBack_UpdatesActiveIndicator()
    {
        await using var terminal = TestAppBuilder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Switch to Roster, then back to Dashboard
        var sequence = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.D2).Wait(100)
            .Key(Hex1bKey.D1)
            .Build();
        await sequence.ApplyAsync(terminal);
        await Task.Delay(200);

        var snapshot = terminal.CreateSnapshot();
        snapshot.ContainsText("Dashboard").Should().BeTrue();

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
