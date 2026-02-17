using Hex1b;
using Hex1b.Input;
using Hex1b.Widgets;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

/// <summary>
/// Shared app layout using Hex1b TabPanel for main navigation.
/// Used by both Program.cs and TestAppBuilder to keep rendering in sync.
/// </summary>
public static class AppLayout
{
    private static readonly (string Label, string Icon, Screen Screen)[] TabScreens =
    [
        ("Dashboard", "🏠", Screen.Dashboard),
        ("Roster", "👥", Screen.Roster),
        ("Decisions", "📋", Screen.Decisions),
        ("Skills", "🔧", Screen.Skills),
        ("Log", "📊", Screen.ActivityLog),
        ("Metrics", "📈", Screen.Metrics),
    ];

    public static Hex1bWidget Build(
        RootContext ctx,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        // NoSquad screen — no tabs
        if (state.CurrentScreen == Screen.NoSquad)
        {
            return ctx.VStack(v =>
            [
                NoSquadScreen.Render(v, state, app)
            ]).WithInputBindings(keys => BindKeys(keys, state, app, options));
        }

        // Sub-screens that overlay tabs (MemberDetail, Charter, Help, Settings)
        if (state.CurrentScreen is Screen.MemberDetail or Screen.Charter or Screen.Help or Screen.Settings)
        {
            return ctx.VStack(v =>
            [
                v.TabPanel(tp =>
                    TabScreens.Select(tab =>
                        tp.Tab(tab.Label, _ => []).WithIcon(tab.Icon).Selected(false)
                    )
                )
                .OnSelectionChanged(e =>
                {
                    state.CurrentScreen = TabScreens[e.SelectedIndex].Screen;
                })
                .Compact(),

                (state.CurrentScreen switch
                {
                    Screen.MemberDetail => MemberDetailScreen.Render(v, state, app),
                    Screen.Charter => CharterScreen.Render(v, state, app),
                    Screen.Help => HelpScreen.Render(v, state, app),
                    Screen.Settings => SettingsScreen.Render(v, state, app, options),
                    _ => v.Text("")
                })
            ]).WithInputBindings(keys => BindKeys(keys, state, app, options));
        }

        // Main tabbed view
        return ctx.VStack(v =>
        [
            v.TabPanel(tp =>
                TabScreens.Select(tab =>
                    tp.Tab(tab.Label, t =>
                    [
                        tab.Screen switch
                        {
                            Screen.Dashboard => DashboardScreen.Render(t, state, app),
                            Screen.Roster => RosterScreen.Render(t, state, app),
                            Screen.Decisions => DecisionsScreen.Render(t, state, app),
                            Screen.Skills => SkillsScreen.Render(t, state, app),
                            Screen.ActivityLog => ActivityLogScreen.Render(t, state, app),
                            Screen.Metrics => MetricsScreen.Render(t, state, app),
                            _ => t.Text("")
                        }
                    ]).WithIcon(tab.Icon).Selected(state.CurrentScreen == tab.Screen)
                )
            )
            .OnSelectionChanged(e =>
            {
                state.CurrentScreen = TabScreens[e.SelectedIndex].Screen;
            })
            .Compact()
            .Fill()
        ]).WithInputBindings(keys => BindKeys(keys, state, app, options));
    }

    private static void BindKeys(
        InputBindingsBuilder keys,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        keys.Key(Hex1bKey.D1).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.Dashboard; }, "Dashboard");
        keys.Key(Hex1bKey.D2).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.Roster; }, "Roster");
        keys.Key(Hex1bKey.D3).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.Decisions; }, "Decisions");
        keys.Key(Hex1bKey.D4).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.Skills; }, "Skills");
        keys.Key(Hex1bKey.D5).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.ActivityLog; }, "Log");
        keys.Key(Hex1bKey.D6).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.Metrics; }, "Metrics");
        keys.Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");
        keys.Key(Hex1bKey.T).Action(() =>
        {
            state.SelectedThemeIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
            options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
        }, "Theme");
        keys.Key(Hex1bKey.S).Action(() => { if (state.CurrentScreen != Screen.NoSquad) state.CurrentScreen = Screen.Settings; }, "Settings");
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
            if (state.CurrentScreen == Screen.Roster)
                state.RosterSelectedIndex = Math.Min(state.RosterSelectedIndex + 1, (state.Members?.Count ?? 6) - 1);
            else if (state.CurrentScreen == Screen.Decisions)
                state.DecisionSelectedIndex = Math.Min(state.DecisionSelectedIndex + 1, (state.Decisions?.Count ?? 4) - 1);
            else if (state.CurrentScreen == Screen.ActivityLog)
                state.LogSelectedIndex = Math.Min(state.LogSelectedIndex + 1, (state.LogEntries?.Count ?? 3) - 1);
            else if (state.CurrentScreen == Screen.Skills)
                state.SkillSelectedIndex = Math.Min(state.SkillSelectedIndex + 1, (state.Skills?.Count ?? 5) - 1);
            else if (state.CurrentScreen == Screen.Settings)
                state.SettingsSelectedIndex = Math.Min(state.SettingsSelectedIndex + 1, 4);
        }, "Down");
        keys.Key(Hex1bKey.K).Action(() =>
        {
            if (state.CurrentScreen == Screen.Roster)
                state.RosterSelectedIndex = Math.Max(state.RosterSelectedIndex - 1, 0);
            else if (state.CurrentScreen == Screen.Decisions)
                state.DecisionSelectedIndex = Math.Max(state.DecisionSelectedIndex - 1, 0);
            else if (state.CurrentScreen == Screen.ActivityLog)
                state.LogSelectedIndex = Math.Max(state.LogSelectedIndex - 1, 0);
            else if (state.CurrentScreen == Screen.Skills)
                state.SkillSelectedIndex = Math.Max(state.SkillSelectedIndex - 1, 0);
            else if (state.CurrentScreen == Screen.Settings)
                state.SettingsSelectedIndex = Math.Max(state.SettingsSelectedIndex - 1, 0);
        }, "Up");
        keys.Key(Hex1bKey.H).Action(() =>
        {
            if (state.CurrentScreen == Screen.NoSquad) return;
            var screens = new[] { Screen.Dashboard, Screen.Roster, Screen.Decisions, Screen.Skills, Screen.ActivityLog, Screen.Metrics };
            var idx = Array.IndexOf(screens, state.CurrentScreen);
            if (idx > 0) state.CurrentScreen = screens[idx - 1];
        }, "Prev Screen");
        keys.Key(Hex1bKey.L).Action(() =>
        {
            if (state.CurrentScreen == Screen.NoSquad) return;
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
        keys.Key(Hex1bKey.F1).Action(() =>
        {
            if (state.CurrentScreen != Screen.Help)
            {
                state.PreviousScreen = state.CurrentScreen;
                state.CurrentScreen = Screen.Help;
            }
            else
            {
                if (state.PreviousScreen.HasValue)
                {
                    state.CurrentScreen = state.PreviousScreen.Value;
                    state.PreviousScreen = null;
                }
                else
                {
                    state.CurrentScreen = Screen.Dashboard;
                }
            }
        }, "Help");
    }
}
