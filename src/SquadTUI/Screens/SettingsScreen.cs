using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Services;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class SettingsScreen
{
    private static readonly string[] SettingLabels =
    [
        "Theme",
        "Vim Keybindings",
        "Mouse Support",
        "Emoji Display",
        "Markdown Rendering",
        "Refresh Interval"
    ];

    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app, dynamic options)
    {
        var settings = state.Settings;
        var t = new ThemeContext(state.SelectedThemeIndex, settings.ShowEmoji);

        var selectedIdx = Math.Clamp(state.SettingsSelectedIndex, 0, SettingLabels.Length - 1);

        var listItems = new List<string>
        {
            $"  {Icon("🎨", "◆", t.Em)} Theme            {FormatThemeValue(settings.ThemeName)}",
            $"  {Icon("⌨️", "◆", t.Em)}  Vim Keybindings  {FormatToggle(settings.VimBindings)}",
            $"  {Icon("🖱️", "◆", t.Em)}  Mouse Support    {FormatToggle(settings.MouseEnabled)}",
            $"  {Icon("😀", "◆", t.Em)} Emoji Display    {FormatToggle(settings.ShowEmoji)}",
            $"  {Icon("📝", "▪", t.Em)} Markdown Render  {FormatToggle(settings.MarkdownRendering)}",
            $"  {Icon("🔄", "▸", t.Em)} Refresh Interval {t.B}{settings.RefreshIntervalSeconds}s{t.R}"
        } as IReadOnlyList<string>;

        return v.HStack(h =>
        [
            new BackgroundPanelWidget(t.PanelBg, h.VStack(left =>
            [
                left.Text($"  {t.B}{t.Acc}{Icon("⚙️", "◆", t.Em)}  Settings{t.R}"),
                left.Text(t.Separator(30)),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.SettingsSelectedIndex = e.SelectedIndex; })
                    .OnItemActivated(e =>
                    {
                        ToggleSetting(state, e.ActivatedIndex, options);
                    })
                    .Fill()
            ]).FillWidth(1).FillHeight()),

            new BackgroundPanelWidget(t.DetailBg, h.VStack(detail =>
            {
                var (label, description, currentValue) = GetSettingDetail(selectedIdx, settings, t.Em);
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"  {t.B}{t.Acc}{Icon("📋", "▪", t.Em)} Setting Details{t.R}"),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text($"  {t.B}{t.Acc}{label}{t.R}"),
                    detail.Text($"  {t.D}{description}{t.R}"),
                    detail.Text(""),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text($"  {t.D}Current:{t.R}  {t.B}{currentValue}{t.R}"),
                    detail.Text(""),
                    detail.Text($"  {t.D}Press Enter to change{t.R}"),
                };

                return widgets.ToArray();
            }).FillWidth(1).FillHeight()),
        ]).Fill();
    }

    private static string FormatToggle(bool value) =>
        value ? "\x1b[32m● ON\x1b[0m" : "\x1b[90m○ OFF\x1b[0m";

    private static string FormatThemeValue(string themeName) =>
        $"\x1b[1m{themeName}\x1b[0m";

    private static (string Label, string Description, string Value) GetSettingDetail(int index, Models.AppSettings settings, bool em) => index switch
    {
        0 => ($"{Icon("🎨", "◆", em)} Theme",
              "Visual theme for the application. Cycles through Ocean, Heist, Sunset, and HighContrast.",
              settings.ThemeName),
        1 => ($"{Icon("⌨️", "◆", em)}  Vim Keybindings",
              "Enable j/k navigation and other vim-style keys.",
              settings.VimBindings ? "Enabled" : "Disabled"),
        2 => ($"{Icon("🖱️", "◆", em)}  Mouse Support",
              "Enable mouse click and scroll interactions.",
              settings.MouseEnabled ? "Enabled" : "Disabled"),
        3 => ($"{Icon("😀", "◆", em)} Emoji Display",
              "Show emoji icons throughout the interface.",
              settings.ShowEmoji ? "Enabled" : "Disabled"),
        4 => ($"{Icon("📝", "▪", em)} Markdown Rendering",
              "Render markdown formatting in charter and log views.",
              settings.MarkdownRendering ? "Enabled" : "Disabled"),
        5 => ($"{Icon("🔄", "▸", em)} Refresh Interval",
              "How often data is automatically refreshed. Cycles through 15s, 30s, 60s, 120s.",
              $"{settings.RefreshIntervalSeconds}s"),
        _ => ("", "", "")
    };

    public static void ToggleSetting(AppState state, int index, dynamic options)
    {
        var settings = state.Settings;
        switch (index)
        {
            case 0: // Theme — cycle through available themes
                var currentThemeIdx = Array.IndexOf(ThemeManager.ThemeNames, settings.ThemeName);
                if (currentThemeIdx < 0) currentThemeIdx = 0;
                var nextIdx = (currentThemeIdx + 1) % ThemeManager.ThemeNames.Length;
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
            case 5: // Refresh Interval — cycle
                var intervals = new[] { 15, 30, 60, 120 };
                var curIdx = Array.IndexOf(intervals, settings.RefreshIntervalSeconds);
                if (curIdx < 0) curIdx = 1; // default to 30s
                settings.RefreshIntervalSeconds = intervals[(curIdx + 1) % intervals.Length];
                break;
        }
        SettingsService.Save(settings);
    }
}