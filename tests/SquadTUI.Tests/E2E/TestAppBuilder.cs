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
    public static Hex1bTerminal Build(int width = 120, int height = 30)
    {
        var state = new AppState();

        var terminal = Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                options.EnableMouse = true;

                return ctx =>
                {
                    return ctx.VStack(v =>
                    [
                        NavBar.Render(v, state),

                        (state.CurrentScreen switch
                        {
                            Screen.Dashboard => DashboardScreen.Render(v, state, app),
                            Screen.Roster => RosterScreen.Render(v, state, app),
                            Screen.MemberDetail => MemberDetailScreen.Render(v, state, app),
                            Screen.Decisions => DecisionsScreen.Render(v, state, app),
                            Screen.Skills => SkillsScreen.Render(v, state, app),
                            Screen.ActivityLog => ActivityLogScreen.Render(v, state, app),
                            Screen.Metrics => MetricsScreen.Render(v, state, app),
                            Screen.Charter => CharterScreen.Render(v, state, app),
                            _ => v.Text("Unknown screen")
                        }),

                        v.InfoBar(s =>
                        [
                            s.Section("SquadTUI v0.2.0"),
                            s.Spacer(),
                            s.Section($"🎨 {ThemeManager.ThemeNames[state.SelectedThemeIndex % ThemeManager.ThemeNames.Length]}"),
                            s.Spacer(),
                            s.Section($"Screen: {state.CurrentScreen}"),
                            s.Spacer(),
                            s.Section("T:Theme  Q:Quit")
                        ])
                    ]).WithInputBindings(keys =>
                    {
                        keys.Key(Hex1bKey.D1).Action(() => { state.CurrentScreen = Screen.Dashboard; }, "Dashboard");
                        keys.Key(Hex1bKey.D2).Action(() => { state.CurrentScreen = Screen.Roster; }, "Roster");
                        keys.Key(Hex1bKey.D3).Action(() => { state.CurrentScreen = Screen.Decisions; }, "Decisions");
                        keys.Key(Hex1bKey.D4).Action(() => { state.CurrentScreen = Screen.Skills; }, "Skills");
                        keys.Key(Hex1bKey.D5).Action(() => { state.CurrentScreen = Screen.ActivityLog; }, "Log");
                        keys.Key(Hex1bKey.D6).Action(() => { state.CurrentScreen = Screen.Metrics; }, "Metrics");
                        keys.Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");
                        keys.Key(Hex1bKey.T).Action(() =>
                        {
                            state.SelectedThemeIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
                            options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                        }, "Theme");
                        keys.Key(Hex1bKey.B).Action(() =>
                        {
                            if (state.CurrentScreen == Screen.MemberDetail)
                                state.CurrentScreen = Screen.Roster;
                            else if (state.CurrentScreen == Screen.Charter)
                                state.CurrentScreen = Screen.MemberDetail;
                        }, "Back");
                        keys.Key(Hex1bKey.E).Action(() =>
                        {
                            if (state.CurrentScreen == Screen.MemberDetail)
                                state.CurrentScreen = Screen.Charter;
                        }, "Edit Charter");
                    });
                };
            })
            .WithHeadless()
            .WithDimensions(width, height)
            .Build();

        return terminal;
    }
}
