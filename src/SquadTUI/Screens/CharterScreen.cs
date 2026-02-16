using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;

namespace SquadTUI.Screens;

public static class CharterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var memberName = state.SelectedMemberName ?? "Danny";
        var charter = SampleData.GetCharterFor(memberName);

        return v.Border(b =>
        [
            ..MarkdownRenderer.Render(b, charter),
            b.Text(""),
            b.Text("\x1b[90m  [B] Back to Member Detail\x1b[0m"),
        ]).Title($"📜 Charter — {memberName}").Fill();
    }
}
