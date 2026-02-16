using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class CharterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var memberName = state.SelectedMemberName ?? "Solaire";
        var charter = SampleData.GetCharterFor(memberName);

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;

        return v.VStack(inner =>
        [
            inner.Text($"  {B}{acc}📜 Charter — {memberName}{R}"),
            inner.Text($"  {D}{sec}{new string('━', 44)}{R}"),
            inner.VStack(scroll =>
            [
                ..MarkdownRenderer.Render(scroll, charter),
            ]).Fill(),
            inner.Text($"  {D}Esc Back{R}"),
        ]).Fill();
    }
}
