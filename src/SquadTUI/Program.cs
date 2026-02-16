using Hex1b;
using Hex1b.Input;
using SquadTUI.Screens;
using SquadTUI.Services;
using SquadTUI.Themes;

var state = new AppState();

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
                    s.Section($"\x1b[1mSquadTUI\x1b[0m \x1b[90mv0.2.0\x1b[0m"),
                    s.Spacer(),
                    s.Section($"\x1b[36m🎨 {ThemeManager.ThemeNames[state.SelectedThemeIndex % ThemeManager.ThemeNames.Length]}\x1b[0m"),
                    s.Spacer(),
                    s.Section($"\x1b[90mScreen:\x1b[0m \x1b[1m{state.CurrentScreen}\x1b[0m"),
                    s.Spacer(),
                    s.Section("\x1b[90mT:Theme  Q:Quit\x1b[0m")
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
    .Build();

await terminal.RunAsync();
