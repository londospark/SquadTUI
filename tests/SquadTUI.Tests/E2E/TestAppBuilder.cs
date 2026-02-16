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
                            Screen.NoSquad => NoSquadScreen.Render(v, state, app),
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
                            s.Section("Esc:Back  j/k:Nav  h/l:Screen  T:Theme  Q:Quit")
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
                        keys.Key(Hex1bKey.Escape).Action(() =>
                        {
                            if (state.CurrentScreen == Screen.MemberDetail)
                                state.CurrentScreen = Screen.Roster;
                            else if (state.CurrentScreen == Screen.Charter)
                                state.CurrentScreen = Screen.MemberDetail;
                            else if (state.CurrentScreen != Screen.Dashboard)
                                state.CurrentScreen = Screen.Dashboard;
                        }, "Back");
                        keys.Key(Hex1bKey.E).Action(() =>
                        {
                            if (state.CurrentScreen == Screen.MemberDetail)
                                state.CurrentScreen = Screen.Charter;
                        }, "Edit Charter");
                        keys.Key(Hex1bKey.J).Action(() =>
                        {
                            // Move selection down in current list
                            if (state.CurrentScreen == Screen.Roster)
                                state.RosterSelectedIndex = Math.Min(state.RosterSelectedIndex + 1, (state.Members?.Count ?? 6) - 1);
                            else if (state.CurrentScreen == Screen.Decisions)
                                state.DecisionSelectedIndex = Math.Min(state.DecisionSelectedIndex + 1, (state.Decisions?.Count ?? 4) - 1);
                            else if (state.CurrentScreen == Screen.ActivityLog)
                                state.LogSelectedIndex = Math.Min(state.LogSelectedIndex + 1, (state.LogEntries?.Count ?? 3) - 1);
                            else if (state.CurrentScreen == Screen.Skills)
                                state.SkillSelectedIndex = Math.Min(state.SkillSelectedIndex + 1, (state.Skills?.Count ?? 5) - 1);
                        }, "Down");
                        keys.Key(Hex1bKey.K).Action(() =>
                        {
                            // Move selection up in current list
                            if (state.CurrentScreen == Screen.Roster)
                                state.RosterSelectedIndex = Math.Max(state.RosterSelectedIndex - 1, 0);
                            else if (state.CurrentScreen == Screen.Decisions)
                                state.DecisionSelectedIndex = Math.Max(state.DecisionSelectedIndex - 1, 0);
                            else if (state.CurrentScreen == Screen.ActivityLog)
                                state.LogSelectedIndex = Math.Max(state.LogSelectedIndex - 1, 0);
                            else if (state.CurrentScreen == Screen.Skills)
                                state.SkillSelectedIndex = Math.Max(state.SkillSelectedIndex - 1, 0);
                        }, "Down");
                        keys.Key(Hex1bKey.H).Action(() =>
                        {
                            // Previous screen
                            var screens = new[] { Screen.Dashboard, Screen.Roster, Screen.Decisions, Screen.Skills, Screen.ActivityLog, Screen.Metrics };
                            var idx = Array.IndexOf(screens, state.CurrentScreen);
                            if (idx > 0) state.CurrentScreen = screens[idx - 1];
                        }, "Prev Screen");
                        keys.Key(Hex1bKey.L).Action(() =>
                        {
                            // Next screen
                            var screens = new[] { Screen.Dashboard, Screen.Roster, Screen.Decisions, Screen.Skills, Screen.ActivityLog, Screen.Metrics };
                            var idx = Array.IndexOf(screens, state.CurrentScreen);
                            if (idx >= 0 && idx < screens.Length - 1) state.CurrentScreen = screens[idx + 1];
                        }, "Next Screen");
                        keys.Key(Hex1bKey.C).Action(() =>
                        {
                            if (state.CurrentScreen == Screen.NoSquad)
                            {
                                var root = Directory.GetCurrentDirectory();
                                var aiTeamDir = Path.Combine(root, ".ai-team");
                                var agentsDir = Path.Combine(aiTeamDir, "agents");
                                Directory.CreateDirectory(agentsDir);
                                File.WriteAllText(Path.Combine(aiTeamDir, "team.md"), "# Team Roster\n\n*Created by SquadTUI*\n");
                                File.WriteAllText(Path.Combine(aiTeamDir, "decisions.md"), "# Decisions\n\n*No decisions yet.*\n");
                                state.SquadDetected = true;
                                state.SquadRootPath = root;
                                state.CurrentScreen = Screen.Dashboard;
                            }
                        }, "Create Squad");
                    });
                };
            })
            .WithHeadless()
            .WithDimensions(width, height)
            .Build();

        return terminal;
    }
}
