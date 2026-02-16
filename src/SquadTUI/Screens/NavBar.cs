using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class NavBar
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state)
    {
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

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
                    ? $"\x1b[1m{acc} ▶ {item.Emoji} {item.Label} {R}"
                    : $"\x1b[90m {item.Emoji} {item.Label} {R}";

                var screen = item.Screen;
                widgets.Add(h.Button(label).OnClick(_ => { state.CurrentScreen = screen; }));
            }
            return widgets.ToArray();
        });
    }
}
