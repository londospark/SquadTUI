using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class CharterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var memberName = state.SelectedMemberName ?? "Danny";
        var charter = SampleData.GetCharterFor(memberName);

        var ti = state.SelectedThemeIndex;
        var c = ThemeManager.GetPanelColors(ti);
        var p1 = c.PanelBg;
        var acc = c.Accent;
        var R = PanelRenderer.Reset;

        return v.VStack(inner =>
        [
            inner.Text($"{p1}  {PanelRenderer.Bold}{acc}📜 Charter — {memberName}{R}"),
            inner.VStack(scroll =>
            [
                ..MarkdownRenderer.Render(scroll, charter),
            ]).Fill(),
            inner.Text($"\x1b[90m  [Esc] Back\x1b[0m"),
        ]).Fill();
    }
}
