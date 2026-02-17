using Hex1b;
using Hex1b.Input;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class HelpScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var D = PanelRenderer.Dim;
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;

        var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var detailBg = ThemeManager.GetPanelDetailBgColor(state.SelectedThemeIndex);

        return v.Responsive(r =>
        [
            // Wide: two-column help layout
            r.WhenMinWidth(100, r => r.VStack(outer =>
            [
                outer.Text($"  {hBg}{B}{acc} ❓  Help & Keybindings {R}"),
                outer.Text($"  {D}Quick reference for all keyboard shortcuts{R}"),
                outer.Text(""),
                outer.Text($"  {sec}{new string('━', 44)}{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    [
                        left.Text($"  {hBg}{B}{acc}NAVIGATION{R}"),
                        left.Text(""),
                        left.Text($"    {D}1-6{R}          Jump to screen"),
                        left.Text($"    {D}h / l{R}        Previous / next screen"),
                        left.Text($"    {D}Tab / →{R}     Next panel (Dashboard)"),
                        left.Text($"    {D}Shift+Tab / ←{R} Prev panel (Dashboard)"),
                        left.Text($"    {D}Escape{R}       Go back"),
                        left.Text(""),
                        left.Text($"  {sec}{new string('━', 30)}{R}"),
                        left.Text(""),
                        left.Text($"  {hBg}{B}{acc}LIST NAVIGATION{R}"),
                        left.Text(""),
                        left.Text($"    {D}j / k{R}        Move down / up"),
                        left.Text($"    {D}↑ / ↓{R}        Arrow keys"),
                        left.Text(""),
                        left.Text($"  {sec}{new string('━', 30)}{R}"),
                        left.Text(""),
                        left.Text($"  {hBg}{B}{acc}HELP{R}"),
                        left.Text(""),
                        left.Text($"    {D}?{R}            Toggle this help"),
                    ]).FillWidth(1).FillHeight()),

                    new BackgroundPanelWidget(detailBg, h.VStack(right =>
                    [
                        right.Text($"  {hBg}{B}{acc}ACTIONS{R}"),
                        right.Text(""),
                        right.Text($"    {D}Enter{R}        Drill into panel (Dashboard)"),
                        right.Text($"    {D}T{R}            Toggle theme"),
                        right.Text($"    {D}S{R}            Settings"),
                        right.Text($"    {D}E{R}            Edit charter"),
                        right.Text($"    {D}C{R}            Create squad"),
                        right.Text($"    {D}Q{R}            Quit SquadTUI"),
                    ]).FillWidth(1).FillHeight()),
                ]).Fill(),

                outer.Text(""),
                outer.Text($"  {sec}{new string('━', 44)}{R}"),
                outer.Text($"  {D}Press ? or Escape to dismiss{R}"),
            ])),

            // Narrow: single column
            r.Otherwise(r => new BackgroundPanelWidget(panelBg, r.VStack(stack =>
            [
                stack.Text($"  {hBg}{B}{acc} ❓  Help & Keybindings {R}"),
                stack.Text($"  {D}Quick reference for all keyboard shortcuts{R}"),
                stack.Text(""),
                stack.Text($"  {sec}{new string('━', 32)}{R}"),
                stack.Text(""),
                stack.Text($"  {hBg}{B}{acc}NAVIGATION{R}"),
                stack.Text(""),
                stack.Text($"    {D}1-6{R}          Jump to screen"),
                stack.Text($"    {D}h / l{R}        Previous / next screen"),
                stack.Text($"    {D}Tab / →{R}     Next panel (Dashboard)"),
                stack.Text($"    {D}Shift+Tab / ←{R} Prev panel (Dashboard)"),
                stack.Text($"    {D}Escape{R}       Go back"),
                stack.Text(""),
                stack.Text($"  {sec}{new string('━', 32)}{R}"),
                stack.Text(""),
                stack.Text($"  {hBg}{B}{acc}LIST NAVIGATION{R}"),
                stack.Text(""),
                stack.Text($"    {D}j / k{R}        Move down / up"),
                stack.Text($"    {D}↑ / ↓{R}        Arrow keys"),
                stack.Text(""),
                stack.Text($"  {sec}{new string('━', 32)}{R}"),
                stack.Text(""),
                stack.Text($"  {hBg}{B}{acc}ACTIONS{R}"),
                stack.Text(""),
                stack.Text($"    {D}Enter{R}        Drill into panel (Dashboard)"),
                stack.Text($"    {D}T{R}            Toggle theme"),
                stack.Text($"    {D}S{R}            Settings"),
                stack.Text($"    {D}E{R}            Edit charter"),
                stack.Text($"    {D}C{R}            Create squad"),
                stack.Text($"    {D}Q{R}            Quit SquadTUI"),
                stack.Text(""),
                stack.Text($"  {sec}{new string('━', 32)}{R}"),
                stack.Text(""),
                stack.Text($"  {hBg}{B}{acc}HELP{R}"),
                stack.Text(""),
                stack.Text($"    {D}?{R}            Toggle this help"),
                stack.Text(""),
                stack.Text($"  {sec}{new string('━', 32)}{R}"),
                stack.Text($"  {D}Press ? or Escape to dismiss{R}"),
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
