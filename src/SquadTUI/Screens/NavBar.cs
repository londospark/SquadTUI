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
                    ? $" ▶ {item.Emoji} [{item.Key}]{item.Label} "
                    : $" {item.Emoji} [{item.Key}]{item.Label} ";
                
                widgets.Add(h.Text(label));
            }
            widgets.Add(h.Text("  [Q]Quit "));
            return widgets.ToArray();
        });
    }
}
