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
            b.Text("  [B] Back to Member Detail"),
        ]).Title($"📜 Charter — {memberName}").Fill();
    }
}
