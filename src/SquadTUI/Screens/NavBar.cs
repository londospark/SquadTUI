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
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;

        var items = new (string Key, string Label, string Emoji, Screen Screen)[]
        {
            ("1", "Dashboard", "🏠", Screen.Dashboard),
            ("2", "Roster", "👥", Screen.Roster),
            ("3", "Decisions", "📋", Screen.Decisions),
            ("4", "Skills", "🔧", Screen.Skills),
            ("5", "Log", "📊", Screen.ActivityLog),
            ("6", "Metrics", "📈", Screen.Metrics),
        };

        return v.VStack(outer =>
        {
            // Tab row
            var tabRow = outer.HStack(h =>
            {
                var widgets = new List<Hex1bWidget>();
                foreach (var item in items)
                {
                    var isActive = state.CurrentScreen == item.Screen;
                    // Active tab: reverse video with accent color for a filled tab effect
                    // Inactive tab: dim but visible with secondary accent
                    var label = isActive
                        ? $"  {B}{acc}\x1b[7m █ {item.Emoji} {item.Label} █ {R}  "
                        : $"  {D}{sec} {item.Emoji} {item.Label} {R}  ";

                    var screen = item.Screen;
                    widgets.Add(h.Button(label).OnClick(_ => { state.CurrentScreen = screen; }));
                }
                return widgets.ToArray();
            });

            // Bottom border line under tabs
            var borderLine = outer.Text($"  {sec}{new string('━', 120)}{R}");

            return new Hex1bWidget[] { tabRow, borderLine };
        });
    }
}
