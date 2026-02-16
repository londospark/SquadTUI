using FluentAssertions;
using Hex1b;
using Hex1b.Automation;
using SquadTUI.Screens;

namespace SquadTUI.Tests.E2E;

[Collection("E2E")]
public class NoSquadScreenTests
{
    [Fact]
    public async Task NoSquadDetected_ShowsNoSquadScreen()
    {
        // Build a terminal with SquadDetected = false by using a custom builder
        var state = new AppState { SquadDetected = false, CurrentScreen = Screen.NoSquad };

        var terminal = Hex1b.Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = SquadTUI.Themes.ThemeManager.GetTheme(0);
                return ctx =>
                {
                    return ctx.VStack(v =>
                    [
                        NavBar.Render(v, state),
                        NoSquadScreen.Render(v, state, app)
                    ]);
                };
            })
            .WithHeadless()
            .WithDimensions(120, 30)
            .Build();

        await using (terminal)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var runTask = terminal.RunAsync(cts.Token);
            await Task.Delay(200);

            var snapshot = terminal.CreateSnapshot();
            snapshot.ContainsText("Welcome to SquadTUI").Should().BeTrue();
            snapshot.ContainsText("No squad detected").Should().BeTrue();

            cts.Cancel();
            try { await runTask; } catch (OperationCanceledException) { }
        }
    }
}
