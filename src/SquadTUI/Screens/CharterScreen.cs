using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class CharterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members ?? [];
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        var charter = state.CharterContent ?? "No charter loaded";

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);

        return new BackgroundPanelWidget(panelBg, v.VStack(inner =>
        [
            inner.Text($"  {B}{acc}📜 Charter — {memberName}{R}"),
            inner.Text($"  {D}{sec}{new string('━', 44)}{R}"),
            inner.VStack(scroll =>
            [
                ..MarkdownRenderer.Render(scroll, charter),
            ]).Fill(),
            inner.Text($"  {D}Esc Back{R}"),
        ]).Fill());
    }
}
