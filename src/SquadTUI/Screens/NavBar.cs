using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;

namespace SquadTUI.Screens;

public static class NavBar
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state)
    {
        var colors = Themes.ThemeManager.GetPanelColors(state.SelectedThemeIndex);
        var navBg = colors.PanelBg;

        var items = new (string Key, string Label, string Emoji, Screen Screen)[]
        {
            ("1", "Dashboard", "🏠", Screen.Dashboard),
            ("2", "Roster", "👥", Screen.Roster),
            ("3", "Decisions", "📋", Screen.Decisions),
            ("4", "Skills", "🔧", Screen.Skills),
            ("5", "Log", "📊", Screen.ActivityLog),
            ("6", "Metrics", "📈", Screen.Metrics),
        };

        return v.HStack(h =>
        {
            var widgets = new List<Hex1bWidget>();
            foreach (var item in items)
            {
                var isActive = state.CurrentScreen == item.Screen;
                var label = isActive
                    ? $"{colors.NestedBg}\x1b[1m{colors.Accent} ▶ {item.Emoji} [{item.Key}]{item.Label} {PanelRenderer.Reset}"
                    : $"{navBg}\x1b[90m {item.Emoji} [{item.Key}]{item.Label} {PanelRenderer.Reset}";

                widgets.Add(h.Text(label));
            }
            widgets.Add(h.Text($"{navBg}\x1b[90m  [Q]Quit {PanelRenderer.Reset}"));
            return widgets.ToArray();
        });
    }
}
