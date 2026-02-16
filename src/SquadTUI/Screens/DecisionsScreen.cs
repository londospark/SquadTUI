using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class DecisionsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var decisions = state.Decisions ?? SampleData.Decisions;
        var listItems = decisions.Select(d => $"📋 {d.Date}  {d.Title}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.DecisionSelectedIndex, 0, decisions.Count - 1);
        var selected = decisions[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

        return v.HStack(h =>
        [
            h.VStack(left =>
            [
                left.Text($"  {PanelRenderer.Bold}{acc}📋 Decisions{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.DecisionSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            [
                detail.Text($"  {PanelRenderer.Bold}{acc}📋 {selected.Title}{R}"),
                detail.Text(""),
                detail.Text($"  \x1b[90mDate:\x1b[0m   {selected.Date}{R}"),
                detail.Text($"  \x1b[90mAuthor:\x1b[0m 👤 {selected.Author}{R}"),
                detail.Text(""),
                detail.Text($"  {PanelRenderer.Bold}{acc}Content{R}"),
                ..MarkdownRenderer.Render(detail, selected.Content).Select(w => w),
            ]).FillWidth(2).FillHeight(),
        ]).Fill();
    }
}
