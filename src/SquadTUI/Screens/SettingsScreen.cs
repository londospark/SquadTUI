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
        "Markdown Rendering"
    ];

    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app, dynamic options)
    {
        var settings = state.Settings;
        var ti = state.SelectedThemeIndex;
        var acc = ThemeManager.GetAccentCode(ti);
        var sec = ThemeManager.GetSecondaryAccent(ti);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var em = settings.ShowEmoji;

        var selectedIdx = Math.Clamp(state.SettingsSelectedIndex, 0, SettingLabels.Length - 1);
        var panelBg = ThemeManager.GetPanelBgColor(ti);
        var detailBg = ThemeManager.GetPanelDetailBgColor(ti);

        var listItems = new List<string>
        {
            $"  {Icon("🎨", "◆", em)} Theme            {FormatThemeValue(settings.ThemeName)}",
            $"  {Icon("⌨️", "◆", em)}  Vim Keybindings  {FormatToggle(settings.VimBindings)}",
            $"  {Icon("🖱️", "◆", em)}  Mouse Support    {FormatToggle(settings.MouseEnabled)}",
            $"  {Icon("😀", "◆", em)} Emoji Display    {FormatToggle(settings.ShowEmoji)}",
            $"  {Icon("📝", "▪", em)} Markdown Render  {FormatToggle(settings.MarkdownRendering)}"
        } as IReadOnlyList<string>;

        return v.HStack(h =>
        [
            new BackgroundPanelWidget(panelBg, h.VStack(left =>
            [
                left.Text($"  {B}{acc}{Icon("⚙️", "◆", em)}  Settings{R}"),
                left.Text($"  {sec}{new string('━', 30)}{R}"),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.SettingsSelectedIndex = e.SelectedIndex; })
                    .OnItemActivated(e =>
                    {
                        ToggleSetting(state, e.ActivatedIndex, options);
                    })
                    .Fill()
            ]).FillWidth(1).FillHeight()),

            new BackgroundPanelWidget(detailBg, h.VStack(detail =>
            {
                var (label, description, currentValue) = GetSettingDetail(selectedIdx, settings, em);
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"  {B}{acc}{Icon("📋", "▪", em)} Setting Details{R}"),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {B}{acc}{label}{R}"),
                    detail.Text($"  {D}{description}{R}"),
                    detail.Text(""),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {D}Current:{R}  {B}{currentValue}{R}"),
                    detail.Text(""),
                    detail.Text($"  {D}Press Enter to change{R}"),
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
        _ => ("", "", "")
    };

    private static void ToggleSetting(AppState state, int index, dynamic options)
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
        }
        SettingsService.Save(settings);
    }
}
