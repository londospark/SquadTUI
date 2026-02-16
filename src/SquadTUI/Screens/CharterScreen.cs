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

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

        return v.VStack(inner =>
        [
            inner.Text($"  {PanelRenderer.Bold}{acc}📜 Charter — {memberName}{R}"),
            inner.VStack(scroll =>
            [
                ..MarkdownRenderer.Render(scroll, charter),
            ]).Fill(),
            inner.Text($"\x1b[90m  Esc Back\x1b[0m"),
        ]).Fill();
    }
}
