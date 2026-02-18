using Hex1b;
using Hex1b.Input;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class HelpScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);

        return v.Responsive(r =>
        [
            // Wide: two-column help layout
            r.WhenMinWidth(100, r => r.VStack(outer =>
            [
                outer.Text($"  {t.HBg}{t.B}{t.Acc} {Icon("❓", "?", t.Em)}  Help & Keybindings {t.R}"),
                outer.Text($"  {t.D}Quick reference for all keyboard shortcuts{t.R}"),
                outer.Text(""),
                outer.Text(t.Separator(44)),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(t.PanelBg, h.VStack(left =>
                    [
                        left.Text(t.SectionHeader("NAVIGATION")),
                        left.Text(""),
                        left.Text($"    {t.D}1-6{t.R}          Jump to screen"),
                        left.Text($"    {t.D}h / l{t.R}        Previous / next screen"),
                        left.Text($"    {t.D}Tab / →{t.R}     Next panel (Dashboard)"),
                        left.Text($"    {t.D}Shift+Tab / ←{t.R} Prev panel (Dashboard)"),
                        left.Text($"    {t.D}Escape{t.R}       Go back"),
                        left.Text(""),
                        left.Text(t.Separator(30)),
                        left.Text(""),
                        left.Text(t.SectionHeader("LIST NAVIGATION")),
                        left.Text(""),
                        left.Text($"    {t.D}j / k{t.R}        Move down / up"),
                        left.Text($"    {t.D}↑ / ↓{t.R}        Arrow keys"),
                        left.Text(""),
                        left.Text(t.Separator(30)),
                        left.Text(""),
                        left.Text(t.SectionHeader("HELP")),
                        left.Text(""),
                        left.Text($"    {t.D}?{t.R}            Toggle this help"),
                    ]).FillWidth(1).FillHeight()),

                    new BackgroundPanelWidget(t.DetailBg, h.VStack(right =>
                    [
                        right.Text(t.SectionHeader("ACTIONS")),
                        right.Text(""),
                        right.Text($"    {t.D}Enter{t.R}        Drill into panel (Dashboard)"),
                        right.Text($"    {t.D}T{t.R}            Toggle theme"),
                        right.Text($"    {t.D}S{t.R}            Settings"),
                        right.Text($"    {t.D}E{t.R}            Edit charter"),
                        right.Text($"    {t.D}C{t.R}            Create squad"),
                        right.Text($"    {t.D}Q{t.R}            Quit SquadTUI"),
                    ]).FillWidth(1).FillHeight()),
                ]).Fill(),

                outer.Text(""),
                outer.Text(t.Separator(44)),
                outer.Text($"  {t.D}Press ? or Escape to dismiss{t.R}"),
            ])),

            // Narrow: single column
            r.Otherwise(r => new BackgroundPanelWidget(t.PanelBg, r.VStack(stack =>
            [
                stack.Text($"  {t.HBg}{t.B}{t.Acc} {Icon("❓", "?", t.Em)}  Help & Keybindings {t.R}"),
                stack.Text($"  {t.D}Quick reference for all keyboard shortcuts{t.R}"),
                stack.Text(""),
                stack.Text(t.Separator(32)),
                stack.Text(""),
                stack.Text(t.SectionHeader("NAVIGATION")),
                stack.Text(""),
                stack.Text($"    {t.D}1-6{t.R}          Jump to screen"),
                stack.Text($"    {t.D}h / l{t.R}        Previous / next screen"),
                stack.Text($"    {t.D}Tab / →{t.R}     Next panel (Dashboard)"),
                stack.Text($"    {t.D}Shift+Tab / ←{t.R} Prev panel (Dashboard)"),
                stack.Text($"    {t.D}Escape{t.R}       Go back"),
                stack.Text(""),
                stack.Text(t.Separator(32)),
                stack.Text(""),
                stack.Text(t.SectionHeader("LIST NAVIGATION")),
                stack.Text(""),
                stack.Text($"    {t.D}j / k{t.R}        Move down / up"),
                stack.Text($"    {t.D}↑ / ↓{t.R}        Arrow keys"),
                stack.Text(""),
                stack.Text(t.Separator(32)),
                stack.Text(""),
                stack.Text(t.SectionHeader("ACTIONS")),
                stack.Text(""),
                stack.Text($"    {t.D}Enter{t.R}        Drill into panel (Dashboard)"),
                stack.Text($"    {t.D}T{t.R}            Toggle theme"),
                stack.Text($"    {t.D}S{t.R}            Settings"),
                stack.Text($"    {t.D}E{t.R}            Edit charter"),
                stack.Text($"    {t.D}C{t.R}            Create squad"),
                stack.Text($"    {t.D}Q{t.R}            Quit SquadTUI"),
                stack.Text(""),
                stack.Text(t.Separator(32)),
                stack.Text(""),
                stack.Text(t.SectionHeader("HELP")),
                stack.Text(""),
                stack.Text($"    {t.D}?{t.R}            Toggle this help"),
                stack.Text(""),
                stack.Text(t.Separator(32)),
                stack.Text($"  {t.D}Press ? or Escape to dismiss{t.R}"),
            ]))),
        ]).WithInputBindings(keys =>
        {
            keys.Key(Hex1bKey.F1).Action(() =>
            {
                if (state.PreviousScreen.HasValue)
                {
                    state.CurrentScreen = state.PreviousScreen.Value;
                    state.PreviousScreen = null;
                }
                else
                {
                    state.CurrentScreen = Screen.Dashboard;
                }
            }, "Close Help");
            keys.Key(Hex1bKey.Escape).Action(() =>
            {
                if (state.PreviousScreen.HasValue)
                {
                    state.CurrentScreen = state.PreviousScreen.Value;
                    state.PreviousScreen = null;
                }
                else
                {
                    state.CurrentScreen = Screen.Dashboard;
                }
            }, "Close Help");
        });
    }
}