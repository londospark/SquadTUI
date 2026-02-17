using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;
using SquadTUI.Screens;
using SquadTUI.Themes;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Builds a headless Hex1b terminal running the SquadTUI app for E2E testing.
/// </summary>
public static class TestAppBuilder
{
    public static Hex1bTerminal Build(int width = 120, int height = 30, bool squadDetected = true)
    {
        var state = new AppState();
        if (!squadDetected)
        {
            state.SquadDetected = false;
            state.CurrentScreen = Screen.NoSquad;
        }

        var terminal = Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                options.EnableMouse = true;

                return ctx => AppLayout.Build(ctx, state, app, options);
            })
            .WithHeadless()
            .WithDimensions(width, height)
            .Build();

        return terminal;
    }
}
