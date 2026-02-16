using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class NavBar
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state)
    {
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
                    ? $"\x1b[1m\x1b[36m ▶ {item.Emoji} [{item.Key}]{item.Label} \x1b[0m"
                    : $"\x1b[90m {item.Emoji} [{item.Key}]{item.Label} \x1b[0m";
                
                widgets.Add(h.Text(label));
            }
            widgets.Add(h.Text("\x1b[90m  [Q]Quit \x1b[0m"));
            return widgets.ToArray();
        });
    }
}
