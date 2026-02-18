using Hex1b;
using Hex1b.Input;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Services;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

/// <summary>
/// Shared app layout using stack-based navigation.
/// Dashboard is home. Enter drills in. Escape pops back.
/// </summary>
public static class AppLayout
{

    /// <summary>Renders a contextual status bar footer with screen-specific keybindings.</summary>
    public static Hex1bWidget RenderFooter(WidgetContext<VStackWidget> v, AppState state)
    {
        var D = PanelRenderer.Dim;
        var R = PanelRenderer.Reset;

        var keys = state.CurrentScreen switch
        {
            Screen.Dashboard => $"  {D}Tab/←→: Focus Panel  Enter: Open  T: Theme  S: Settings  Q: Quit  F1: Help{R}",
            Screen.Roster => $"  {D}j/k: Navigate  Enter: Detail  A: Add  D: Remove  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.MemberDetail => $"  {D}E: Edit Charter  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.Decisions => $"  {D}j/k: Navigate  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.Skills => $"  {D}j/k: Navigate  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.ActivityLog => $"  {D}j/k: Navigate  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.Metrics => $"  {D}V: Toggle Burndown  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.Charter => $"  {D}j/k: Scroll  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.Settings => $"  {D}j/k: Navigate  Enter: Toggle  Esc: Back  Q: Quit  F1: Help{R}",
            Screen.Help => $"  {D}F1/Esc: Dismiss  Q: Quit{R}",
            Screen.NoSquad => $"  {D}C: Create Squad  Q: Quit  F1: Help{R}",
            _ => $"  {D}Esc: Back  Q: Quit  F1: Help{R}",
        };

        return v.VStack(footer =>
        [
            footer.Text(""),
            footer.Text(keys),
        ]);
    }

    public static Hex1bWidget Build(
        RootContext ctx,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        var em = state.Settings.ShowEmoji;

        // Theme settings modal overlay — renders instead of normal content
        if (state.ShowSettingsOverlay)
        {
            return RenderThemeModal(ctx, state, app, options);
        }

        // NoSquad screen — no navigation
        if (state.CurrentScreen == Screen.NoSquad)
        {
            return ctx.VStack(v =>
            [
                NoSquadScreen.Render(v, state, app),
                RenderFooter(v, state)
            ]).WithInputBindings(keys => BindKeys(keys, state, app, options));
        }

        // Settings modal as centered overlay on top of current screen
        if (state.ShowSettingsModal)
        {
            return ctx.ZStack(z =>
            [
                // Layer 0: current screen underneath
                z.VStack(v =>
                [
                    RenderCurrentScreen(v, state, app, options),
                    RenderFooter(v, state)
                ]),
                // Layer 1: backdrop + centered settings modal
                z.Backdrop(
                    z.VStack(modal =>
                    {
                        return RenderSettingsModalContent(modal, state, app, options);
                    }).FixedWidth(56).FixedHeight(17)
                ).OnClickAway(() => { state.ShowSettingsModal = false; })
            ]).WithInputBindings(keys => BindSettingsModalKeys(keys, state, app, options));
        }

        // Main view — render current screen directly (no tabs)
        return ctx.VStack(v =>
        [
            RenderCurrentScreen(v, state, app, options),
            RenderFooter(v, state)
        ]).WithInputBindings(keys => BindKeys(keys, state, app, options));
    }

    private static Hex1bWidget RenderCurrentScreen(
        WidgetContext<VStackWidget> v,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        return state.CurrentScreen switch
        {
            Screen.Dashboard => DashboardScreen.Render(v, state, app),
            Screen.Roster => RosterScreen.Render(v, state, app),
            Screen.Decisions => DecisionsScreen.Render(v, state, app),
            Screen.Skills => SkillsScreen.Render(v, state, app),
            Screen.ActivityLog => ActivityLogScreen.Render(v, state, app),
            Screen.Metrics => MetricsScreen.Render(v, state, app),
            Screen.MemberDetail => MemberDetailScreen.Render(v, state, app),
            Screen.Charter => CharterScreen.Render(v, state, app),
            Screen.Help => HelpScreen.Render(v, state, app),
            Screen.Settings => SettingsScreen.Render(v, state, app, options),
            _ => v.Text("")
        };
    }

    private static Hex1bWidget RenderThemeModal(
        RootContext ctx,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        var ti = state.SelectedThemeIndex;
        var acc = ThemeManager.GetAccentCode(ti);
        var sec = ThemeManager.GetSecondaryAccent(ti);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(ti);
        var em = state.Settings.ShowEmoji;

        var themeItems = ThemeManager.ThemeNames
            .Select((name, idx) => idx == ti ? $"  ► {name}" : $"    {name}")
            .ToList() as IReadOnlyList<string>;

        return ctx.VStack(v =>
        [
            v.Text(""),
            v.Text(""),
            new BackgroundPanelWidget(panelBg, v.VStack(modal =>
            [
                modal.Text($"  {B}{acc}{Icon("🎨", "◆", em)} Theme Selection{R}"),
                modal.Text($"  {sec}{new string('━', 36)}{R}"),
                modal.Text(""),
                modal.List(themeItems)
                    .OnSelectionChanged(e =>
                    {
                        state.SelectedThemeIndex = e.SelectedIndex;
                        options.Theme = ThemeManager.GetTheme(e.SelectedIndex);
                    })
                    .OnItemActivated(_ =>
                    {
                        // Enter confirms selection and closes modal
                        state.Settings.ThemeName = ThemeManager.ThemeNames[state.SelectedThemeIndex];
                        SettingsService.Save(state.Settings);
                        state.ShowSettingsOverlay = false;
                    })
                    .Fill(),
                modal.Text(""),
                modal.Text($"  {D}↑↓ Navigate  Enter Confirm  Esc Cancel{R}"),
                modal.Text(""),
            ]).FillWidth(1).FillHeight()),
        ]).WithInputBindings(keys => BindModalKeys(keys, state, app, options));
    }

    private static void BindModalKeys(
        InputBindingsBuilder keys,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        keys.Key(Hex1bKey.Escape).Action(() =>
        {
            // Revert to original theme
            state.SelectedThemeIndex = state.OriginalThemeIndex;
            options.Theme = ThemeManager.GetTheme(state.OriginalThemeIndex);
            state.ShowSettingsOverlay = false;
        }, "Cancel");
        keys.Key(Hex1bKey.Enter).Action(() =>
        {
            // Confirm selection and save
            state.Settings.ThemeName = ThemeManager.ThemeNames[state.SelectedThemeIndex];
            SettingsService.Save(state.Settings);
            state.ShowSettingsOverlay = false;
        }, "Confirm");
        keys.Key(Hex1bKey.J).Action(() =>
        {
            var newIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
            state.SelectedThemeIndex = newIndex;
            options.Theme = ThemeManager.GetTheme(newIndex);
        }, "Down");
        keys.Key(Hex1bKey.K).Action(() =>
        {
            var newIndex = (state.SelectedThemeIndex - 1 + ThemeManager.ThemeNames.Length) % ThemeManager.ThemeNames.Length;
            state.SelectedThemeIndex = newIndex;
            options.Theme = ThemeManager.GetTheme(newIndex);
        }, "Up");
        keys.Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");

        // Case-insensitive: Shift+Key for uppercase input
        keys.Shift().Key(Hex1bKey.J).Action(() =>
        {
            var newIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
            state.SelectedThemeIndex = newIndex;
            options.Theme = ThemeManager.GetTheme(newIndex);
        }, "Down");
        keys.Shift().Key(Hex1bKey.K).Action(() =>
        {
            var newIndex = (state.SelectedThemeIndex - 1 + ThemeManager.ThemeNames.Length) % ThemeManager.ThemeNames.Length;
            state.SelectedThemeIndex = newIndex;
            options.Theme = ThemeManager.GetTheme(newIndex);
        }, "Up");
        keys.Shift().Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");
    }

    private static readonly string[] SettingsModalLabels =
    [
        "Theme",
        "Vim Keybindings",
        "Mouse Support",
        "Emoji Display",
        "Markdown Rendering",
        "Default Screen"
    ];

    private static Hex1bWidget[] RenderSettingsModalContent(
        WidgetContext<VStackWidget> modal,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        var ti = state.SelectedThemeIndex;
        var acc = ThemeManager.GetAccentCode(ti);
        var sec = ThemeManager.GetSecondaryAccent(ti);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(ti);
        var em = state.Settings.ShowEmoji;
        var sel = Math.Clamp(state.SettingsModalSelectedIndex, 0, SettingsModalLabels.Length - 1);

        var screenNames = new[] { "Dashboard", "Roster", "Decisions", "Skills", "Log", "Metrics" };

        string FormatToggle(bool v) => v ? "\x1b[32m● ON\x1b[0m" : "\x1b[90m○ OFF\x1b[0m";
        var listItems = new List<string>
        {
            $"  {Icon("🎨", "◆", em)} Theme            {B}{state.Settings.ThemeName}{R}",
            $"  {Icon("⌨️", "◆", em)}  Vim Keybindings  {FormatToggle(state.Settings.VimBindings)}",
            $"  {Icon("🖱️", "◆", em)}  Mouse Support    {FormatToggle(state.Settings.MouseEnabled)}",
            $"  {Icon("😀", "◆", em)} Emoji Display    {FormatToggle(state.Settings.ShowEmoji)}",
            $"  {Icon("📝", "▪", em)} Markdown Render  {FormatToggle(state.Settings.MarkdownRendering)}",
            $"  {Icon("🏠", "▸", em)} Default Screen   {B}{state.Settings.DefaultScreen}{R}"
        } as IReadOnlyList<string>;

        return
        [
            new BackgroundPanelWidget(panelBg, modal.VStack(inner =>
            [
                inner.Text($"  {B}{acc}{Icon("⚙️", "◆", em)}  Settings{R}"),
                inner.Text($"  {sec}{new string('━', 36)}{R}"),
                inner.Text(""),
                inner.List(listItems)
                    .OnSelectionChanged(e =>
                    {
                        state.SettingsModalSelectedIndex = e.SelectedIndex;
                    })
                    .OnItemActivated(e =>
                    {
                        ToggleSettingsModalItem(state, e.ActivatedIndex, options);
                    })
                    .Fill(),
                inner.Text(""),
                inner.Text($"  {D}j/k Navigate  Enter Toggle  Esc Close{R}"),
                inner.Text(""),
            ]).FillWidth(1).FillHeight()),
        ];
    }

    private static void ToggleSettingsModalItem(AppState state, int index, Hex1bAppOptions options)
    {
        var settings = state.Settings;
        switch (index)
        {
            case 0: // Theme — cycle
                var currentIdx = Array.IndexOf(ThemeManager.ThemeNames, settings.ThemeName);
                if (currentIdx < 0) currentIdx = 0;
                var nextIdx = (currentIdx + 1) % ThemeManager.ThemeNames.Length;
                settings.ThemeName = ThemeManager.ThemeNames[nextIdx];
                state.SelectedThemeIndex = nextIdx;
                options.Theme = ThemeManager.GetTheme(nextIdx);
                break;
            case 1:
                settings.VimBindings = !settings.VimBindings;
                break;
            case 2:
                settings.MouseEnabled = !settings.MouseEnabled;
                options.EnableMouse = settings.MouseEnabled;
                break;
            case 3:
                settings.ShowEmoji = !settings.ShowEmoji;
                break;
            case 4:
                settings.MarkdownRendering = !settings.MarkdownRendering;
                break;
            case 5: // Default Screen — cycle
                var screenNames = new[] { "Dashboard", "Roster", "Decisions", "Skills", "Log", "Metrics" };
                var curScreenIdx = Array.IndexOf(screenNames, settings.DefaultScreen);
                if (curScreenIdx < 0) curScreenIdx = 0;
                settings.DefaultScreen = screenNames[(curScreenIdx + 1) % screenNames.Length];
                break;
        }
        SettingsService.Save(settings);
    }

    private static void BindSettingsModalKeys(
        InputBindingsBuilder keys,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        keys.Key(Hex1bKey.Escape).Action(() =>
        {
            state.ShowSettingsModal = false;
        }, "Close");
        keys.Key(Hex1bKey.Enter).Action(() =>
        {
            ToggleSettingsModalItem(state, state.SettingsModalSelectedIndex, options);
        }, "Toggle");
        keys.Key(Hex1bKey.J).Action(() =>
        {
            state.SettingsModalSelectedIndex = (state.SettingsModalSelectedIndex + 1) % SettingsModalLabels.Length;
        }, "Down");
        keys.Key(Hex1bKey.K).Action(() =>
        {
            state.SettingsModalSelectedIndex = (state.SettingsModalSelectedIndex - 1 + SettingsModalLabels.Length) % SettingsModalLabels.Length;
        }, "Up");
        keys.Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");

        // Case-insensitive: Shift+Key for uppercase input
        keys.Shift().Key(Hex1bKey.J).Action(() =>
        {
            state.SettingsModalSelectedIndex = (state.SettingsModalSelectedIndex + 1) % SettingsModalLabels.Length;
        }, "Down");
        keys.Shift().Key(Hex1bKey.K).Action(() =>
        {
            state.SettingsModalSelectedIndex = (state.SettingsModalSelectedIndex - 1 + SettingsModalLabels.Length) % SettingsModalLabels.Length;
        }, "Up");
        keys.Shift().Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");
    }

    private static void BindKeys(
        InputBindingsBuilder keys,
        AppState state,
        Hex1bApp app,
        Hex1bAppOptions options)
    {
        keys.Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");
        keys.Key(Hex1bKey.T).Action(() =>
        {
            state.SelectedThemeIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
            options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
        }, "Theme");
        keys.Key(Hex1bKey.S).Action(() =>
        {
            if (state.CurrentScreen != Screen.NoSquad)
            {
                state.SettingsModalSelectedIndex = 0;
                state.ShowSettingsModal = true;
            }
        }, "Settings");
        keys.Key(Hex1bKey.Escape).Action(() =>
        {
            if (state.ConfirmingRemove)
            {
                state.ConfirmingRemove = false;
            }
            else
            {
                state.NavigateBack();
            }
        }, "Back");
        keys.Key(Hex1bKey.E).Action(() =>
        {
            if (state.CurrentScreen == Screen.MemberDetail)
                state.NavigateTo(Screen.Charter);
        }, "Edit Charter");
        keys.Key(Hex1bKey.A).Action(() =>
        {
            if (state.CurrentScreen == Screen.Roster && !state.ConfirmingRemove)
            {
                var members = state.Members.GetOrEmpty();
                var newName = $"Member{members.Count + 1}";
                var newRole = "Team Member";
                state.AddMemberMessage = $"Adding {newName}...";
                var bridge = new DataBridge(ServiceProvider.Instance);
                _ = Task.Run(async () =>
                {
                    await bridge.AddMemberAsync(newName, newRole);
                    state.Members = await bridge.LoadRosterDataAsync();
                    state.Tasks = await bridge.LoadTasksFromRosterAsync();
                    state.AddMemberMessage = null;
                });
            }
        }, "Add Member");
        keys.Key(Hex1bKey.D).Action(() =>
        {
            if (state.CurrentScreen == Screen.Roster && !state.ConfirmingRemove)
            {
                state.ConfirmingRemove = true;
            }
        }, "Remove Member");
        keys.Key(Hex1bKey.Y).Action(() =>
        {
            if (state.ConfirmingRemove && state.CurrentScreen == Screen.Roster)
            {
                var members = state.Members.GetOrEmpty();
                if (members.Count > 0)
                {
                    var selectedIdx = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
                    var memberName = members[selectedIdx].Name;
                    state.ConfirmingRemove = false;
                    var bridge = new DataBridge(ServiceProvider.Instance);
                    _ = Task.Run(async () =>
                    {
                        await bridge.RemoveMemberAsync(memberName);
                        state.Members = await bridge.LoadRosterDataAsync();
                        state.Tasks = await bridge.LoadTasksFromRosterAsync();
                        state.RosterSelectedIndex = Math.Max(0, state.RosterSelectedIndex - 1);
                    });
                }
            }
        }, "Confirm Remove");
        keys.Key(Hex1bKey.N).Action(() =>
        {
            if (state.ConfirmingRemove)
                state.ConfirmingRemove = false;
        }, "Cancel Remove");
        if (state.Settings.VimBindings)
        {
            keys.Key(Hex1bKey.J).Action(() =>
            {
                if (state.CurrentScreen == Screen.Roster)
                    state.RosterSelectedIndex = Math.Min(state.RosterSelectedIndex + 1, (state.Members.GetOrEmpty().Count is var mc && mc > 0 ? mc : 6) - 1);
                else if (state.CurrentScreen == Screen.Decisions)
                    state.DecisionSelectedIndex = Math.Min(state.DecisionSelectedIndex + 1, (state.Decisions.GetOrEmpty().Count is var dc && dc > 0 ? dc : 4) - 1);
                else if (state.CurrentScreen == Screen.ActivityLog)
                    state.LogSelectedIndex = Math.Min(state.LogSelectedIndex + 1, (state.LogEntries.GetOrEmpty().Count is var lc && lc > 0 ? lc : 3) - 1);
                else if (state.CurrentScreen == Screen.Skills)
                    state.SkillSelectedIndex = Math.Min(state.SkillSelectedIndex + 1, (state.Skills.GetOrEmpty().Count is var sc && sc > 0 ? sc : 5) - 1);
                else if (state.CurrentScreen == Screen.Settings)
                    state.SettingsSelectedIndex = Math.Min(state.SettingsSelectedIndex + 1, 4);
                else if (state.CurrentScreen == Screen.Charter)
                    state.CharterScrollOffset++;
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
                else if (state.CurrentScreen == Screen.Charter)
                    state.CharterScrollOffset = Math.Max(state.CharterScrollOffset - 1, 0);
            }, "Up");
        }
        keys.Key(Hex1bKey.C).Action(() =>
        {
            if (state.CurrentScreen == Screen.NoSquad)
            {
                var root = Directory.GetCurrentDirectory();
                var fileLocations = new FileLocationService(root);
                Directory.CreateDirectory(fileLocations.GetAgentsDirectory());
                File.WriteAllText(fileLocations.GetRosterPath(), "# Team Roster\n\n*Created by SquadTUI*\n");
                File.WriteAllText(fileLocations.GetDecisionsFilePath(), "# Decisions\n\n*No decisions yet.*\n");
                state.SquadDetected = true;
                state.SquadRootPath = root;
                state.CurrentScreen = Screen.Dashboard;
            }
        }, "Create Squad");
        keys.Key(Hex1bKey.M).Action(() =>
        {
            if (state.NeedsMigration && state.SquadRootPath != null)
            {
                var result = MigrationService.Migrate(state.SquadRootPath);
                state.MigrationMessage = result.Message;
                if (result.Success)
                {
                    state.NeedsMigration = false;
                    ServiceProvider.Reset();
                }
            }
        }, "Migrate");
        keys.Key(Hex1bKey.F1).Action(() =>
        {
            if (state.CurrentScreen != Screen.Help)
            {
                state.NavigateTo(Screen.Help);
            }
            else
            {
                state.NavigateBack();
            }
        }, "Help");
        keys.Key(Hex1bKey.V).Action(() =>
        {
            if (state.CurrentScreen == Screen.Metrics)
                state.ShowBurndown = !state.ShowBurndown;
        }, "Toggle Burndown");

        // Case-insensitive bindings: Shift+Key handles uppercase letter input
        keys.Shift().Key(Hex1bKey.Q).Action(() => { app.RequestStop(); }, "Quit");
        keys.Shift().Key(Hex1bKey.T).Action(() =>
        {
            state.SelectedThemeIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
            options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
        }, "Theme");
        keys.Shift().Key(Hex1bKey.S).Action(() =>
        {
            if (state.CurrentScreen != Screen.NoSquad)
            {
                state.SettingsModalSelectedIndex = 0;
                state.ShowSettingsModal = true;
            }
        }, "Settings");
        keys.Shift().Key(Hex1bKey.E).Action(() =>
        {
            if (state.CurrentScreen == Screen.MemberDetail)
                state.NavigateTo(Screen.Charter);
        }, "Edit Charter");
        keys.Shift().Key(Hex1bKey.A).Action(() =>
        {
            if (state.CurrentScreen == Screen.Roster && !state.ConfirmingRemove)
            {
                var members = state.Members.GetOrEmpty();
                var newName = $"Member{members.Count + 1}";
                var newRole = "Team Member";
                state.AddMemberMessage = $"Adding {newName}...";
                var bridge = new DataBridge(ServiceProvider.Instance);
                _ = Task.Run(async () =>
                {
                    await bridge.AddMemberAsync(newName, newRole);
                    state.Members = await bridge.LoadRosterDataAsync();
                    state.Tasks = await bridge.LoadTasksFromRosterAsync();
                    state.AddMemberMessage = null;
                });
            }
        }, "Add Member");
        keys.Shift().Key(Hex1bKey.D).Action(() =>
        {
            if (state.CurrentScreen == Screen.Roster && !state.ConfirmingRemove)
            {
                state.ConfirmingRemove = true;
            }
        }, "Remove Member");
        keys.Shift().Key(Hex1bKey.Y).Action(() =>
        {
            if (state.ConfirmingRemove && state.CurrentScreen == Screen.Roster)
            {
                var members = state.Members.GetOrEmpty();
                if (members.Count > 0)
                {
                    var selectedIdx = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
                    var memberName = members[selectedIdx].Name;
                    state.ConfirmingRemove = false;
                    var bridge = new DataBridge(ServiceProvider.Instance);
                    _ = Task.Run(async () =>
                    {
                        await bridge.RemoveMemberAsync(memberName);
                        state.Members = await bridge.LoadRosterDataAsync();
                        state.Tasks = await bridge.LoadTasksFromRosterAsync();
                        state.RosterSelectedIndex = Math.Max(0, state.RosterSelectedIndex - 1);
                    });
                }
            }
        }, "Confirm Remove");
        keys.Shift().Key(Hex1bKey.N).Action(() =>
        {
            if (state.ConfirmingRemove)
                state.ConfirmingRemove = false;
        }, "Cancel Remove");
        if (state.Settings.VimBindings)
        {
            keys.Shift().Key(Hex1bKey.J).Action(() =>
            {
                if (state.CurrentScreen == Screen.Roster)
                    state.RosterSelectedIndex = Math.Min(state.RosterSelectedIndex + 1, (state.Members.GetOrEmpty().Count is var mc && mc > 0 ? mc : 6) - 1);
                else if (state.CurrentScreen == Screen.Decisions)
                    state.DecisionSelectedIndex = Math.Min(state.DecisionSelectedIndex + 1, (state.Decisions.GetOrEmpty().Count is var dc && dc > 0 ? dc : 4) - 1);
                else if (state.CurrentScreen == Screen.ActivityLog)
                    state.LogSelectedIndex = Math.Min(state.LogSelectedIndex + 1, (state.LogEntries.GetOrEmpty().Count is var lc && lc > 0 ? lc : 3) - 1);
                else if (state.CurrentScreen == Screen.Skills)
                    state.SkillSelectedIndex = Math.Min(state.SkillSelectedIndex + 1, (state.Skills.GetOrEmpty().Count is var sc && sc > 0 ? sc : 5) - 1);
                else if (state.CurrentScreen == Screen.Settings)
                    state.SettingsSelectedIndex = Math.Min(state.SettingsSelectedIndex + 1, 4);
                else if (state.CurrentScreen == Screen.Charter)
                    state.CharterScrollOffset++;
            }, "Down");
            keys.Shift().Key(Hex1bKey.K).Action(() =>
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
                else if (state.CurrentScreen == Screen.Charter)
                    state.CharterScrollOffset = Math.Max(state.CharterScrollOffset - 1, 0);
            }, "Up");
        }
        keys.Shift().Key(Hex1bKey.C).Action(() =>
        {
            if (state.CurrentScreen == Screen.NoSquad)
            {
                var root = Directory.GetCurrentDirectory();
                var fileLocations = new FileLocationService(root);
                Directory.CreateDirectory(fileLocations.GetAgentsDirectory());
                File.WriteAllText(fileLocations.GetRosterPath(), "# Team Roster\n\n*Created by SquadTUI*\n");
                File.WriteAllText(fileLocations.GetDecisionsFilePath(), "# Decisions\n\n*No decisions yet.*\n");
                state.SquadDetected = true;
                state.SquadRootPath = root;
                state.CurrentScreen = Screen.Dashboard;
            }
        }, "Create Squad");
        keys.Shift().Key(Hex1bKey.M).Action(() =>
        {
            if (state.NeedsMigration && state.SquadRootPath != null)
            {
                var result = MigrationService.Migrate(state.SquadRootPath);
                state.MigrationMessage = result.Message;
                if (result.Success)
                {
                    state.NeedsMigration = false;
                    ServiceProvider.Reset();
                }
            }
        }, "Migrate");
        keys.Shift().Key(Hex1bKey.V).Action(() =>
        {
            if (state.CurrentScreen == Screen.Metrics)
                state.ShowBurndown = !state.ShowBurndown;
        }, "Toggle Burndown");
        keys.Key(Hex1bKey.Tab).OverridesCapture().Action(() =>
        {
            if (state.CurrentScreen == Screen.Dashboard)
                state.DashboardFocusedPanel = (state.DashboardFocusedPanel + 1) % 5;
        }, "Next Panel");
        keys.Shift().Key(Hex1bKey.Tab).OverridesCapture().Action(() =>
        {
            if (state.CurrentScreen == Screen.Dashboard)
                state.DashboardFocusedPanel = (state.DashboardFocusedPanel + 4) % 5;
        }, "Prev Panel");
        keys.Key(Hex1bKey.RightArrow).Action(() =>
        {
            if (state.CurrentScreen == Screen.Dashboard)
                state.DashboardFocusedPanel = (state.DashboardFocusedPanel + 1) % 5;
        }, "Next Panel");
        keys.Key(Hex1bKey.LeftArrow).Action(() =>
        {
            if (state.CurrentScreen == Screen.Dashboard)
                state.DashboardFocusedPanel = (state.DashboardFocusedPanel + 4) % 5;
        }, "Prev Panel");
        keys.Key(Hex1bKey.Enter).Action(() =>
        {
            if (state.CurrentScreen == Screen.Dashboard)
            {
                var target = state.DashboardFocusedPanel switch
                {
                    0 => Screen.Roster,
                    1 => Screen.ActivityLog,
                    2 => Screen.Decisions,
                    3 => Screen.Metrics,
                    4 => Screen.Skills,
                    _ => (Screen?)null
                };
                if (target.HasValue)
                {
                    state.NavigateTo(target.Value);
                    state.DashboardFocusedPanel = 0;
                }
            }
        }, "Drill In");
    }
}
