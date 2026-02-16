using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class DecisionsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var decisions = state.Decisions ?? SampleData.Decisions;
        var listItems = decisions.Select(d => $"📋 {d.Date}  {d.Title}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.DecisionSelectedIndex, 0, decisions.Count - 1);
        var selected = decisions[selectedIdx];

        return v.HStack(h =>
        [
            h.Border(b =>
            [
                b.List(listItems)
                    .OnSelectionChanged(e => { state.DecisionSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).Title("📋 Decisions").FillWidth(2).FillHeight(),

            h.Border(b =>
            [
                b.Text($"  \x1b[90mTitle:\x1b[0m  \x1b[1m{selected.Title}\x1b[0m"),
                b.Text($"  \x1b[90mDate:\x1b[0m   {selected.Date}"),
                b.Text($"  \x1b[90mAuthor:\x1b[0m 👤 {selected.Author}"),
                b.Text(""),
                b.Text($"  {selected.Content}"),
            ]).Title("Details").Fill(),
        ]).Fill();
    }
}
