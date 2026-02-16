using Hex1b;
using Hex1b.Input;
using SquadTUI.Screens;
using SquadTUI.Services;
using SquadTUI.Themes;

var state = new AppState();

var settings = SettingsService.Load();
state.Settings = settings;

var squadRoot = SquadDetector.FindSquadRoot();
if (squadRoot == null || !SquadDetector.HasValidSquad(squadRoot))
{
    state.SquadDetected = false;
    state.CurrentScreen = Screen.NoSquad;
}
else
{
    state.SquadRootPath = squadRoot;
}

// Load initial data asynchronously
var bridge = new DataBridge(ServiceProvider.Instance);
_ = Task.Run(async () =>
{
    try
    {
        state.IsLoading = true;
        var membersTask = bridge.LoadRosterDataAsync();
        var decisionsTask = bridge.LoadDecisionsDataAsync();
        var skillsTask = bridge.LoadSkillsDataAsync();
        var logsTask = bridge.LoadLogDataAsync();
        await Task.WhenAll(membersTask, decisionsTask, skillsTask, logsTask);
        state.Members = await membersTask;
        state.Decisions = await decisionsTask;
        state.Skills = await skillsTask;
        state.LogEntries = await logsTask;
        state.IsLoading = false;
    }
    catch (Exception ex)
    {
        state.ErrorMessage = $"Failed to load data: {ex.Message}";
        state.IsLoading = false;
    }
});

await using var terminal = Hex1bTerminal.CreateBuilder()
    .WithHex1bApp((app, options) =>
    {
        options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
        options.EnableMouse = true;

        return ctx =>
        {
            return ctx.VStack(v =>
            [
                state.CurrentScreen != Screen.NoSquad ? NavBar.Render(v, state) : v.Text(""),

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
                    Screen.Help => HelpScreen.Render(v, state, app),
                    Screen.NoSquad => NoSquadScreen.Render(v, state, app),
                    Screen.Settings => SettingsScreen.Render(v, state, app, options),
                    _ => v.Text("Unknown screen")
                })
            ]).WithInputBindings(keys =>
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
                    // Previous screen
                    var screens = new[] { Screen.Dashboard, Screen.Roster, Screen.Decisions, Screen.Skills, Screen.ActivityLog, Screen.Metrics };
                    var idx = Array.IndexOf(screens, state.CurrentScreen);
                    if (idx > 0) state.CurrentScreen = screens[idx - 1];
                }, "Prev Screen");
                keys.Key(Hex1bKey.L).Action(() =>
                {
                    if (state.CurrentScreen == Screen.NoSquad) return;
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
            });
        };
    })
    .WithDiagnostics()
    .Build();

await terminal.RunAsync();
