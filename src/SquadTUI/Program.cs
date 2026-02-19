using Hex1b;
using Hex1b.Input;
using SquadTUI.Models;
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
    state.NeedsMigration = SquadPathResolver.NeedsMigration(squadRoot);
}

// Load initial data asynchronously
var bridge = new DataBridge(ServiceProvider.Instance);
_ = Task.Run(async () =>
{
    state.IsLoading = true;
    var membersTask = bridge.LoadRosterDataAsync();
    var decisionsTask = bridge.LoadDecisionsDataAsync();
    var skillsTask = bridge.LoadSkillsDataAsync();
    var logsTask = bridge.LoadLogDataAsync();
    var tasksTask = bridge.LoadTasksFromRosterAsync();
    await Task.WhenAll(membersTask, decisionsTask, skillsTask, logsTask, tasksTask);
    state.Members = await membersTask;
    state.Decisions = await decisionsTask;
    state.Skills = await skillsTask;
    state.LogEntries = await logsTask;
    state.Tasks = await tasksTask;
    // Load charter for first member
    var loadedMembers = state.Members.GetOrEmpty();
    if (loadedMembers.Count > 0)
    {
        state.CharterContent = await bridge.LoadCharterContentAsync(loadedMembers[0].Name);
    }
    state.IsLoading = false;
});

// Hybrid refresh: FileWatcher (reactive) + configurable polling (fallback)
using var refreshService = new RefreshService(bridge, state);
if (state.SquadRootPath != null)
    refreshService.Start(state.SquadRootPath);

await using var terminal= Hex1bTerminal.CreateBuilder()
    .WithHex1bApp((app, options) =>
    {
        options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
        options.EnableMouse = true;

        return ctx => AppLayout.Build(ctx, state, app, options);
    })
    .WithDiagnostics()
    .Build();

await terminal.RunAsync();
