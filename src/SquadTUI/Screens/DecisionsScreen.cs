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
        var ti = state.SelectedThemeIndex;
        var c = ThemeManager.GetPanelColors(ti);
        var p1 = c.PanelBg;
        var p2 = c.NestedBg;
        var acc = c.Accent;
        var R = PanelRenderer.Reset;

        return v.HStack(h =>
        [
            h.VStack(left =>
            [
                left.Text($"{p1}  {PanelRenderer.Bold}{acc}📋 Decisions{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.DecisionSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            [
                detail.Text($"{p1}  {PanelRenderer.Bold}{acc}📋 {selected.Title}{R}"),
                detail.Text($"{p1}{R}"),
                detail.Text($"{p1}  \x1b[90mDate:\x1b[0m   {selected.Date}{R}"),
                detail.Text($"{p1}  \x1b[90mAuthor:\x1b[0m 👤 {selected.Author}{R}"),
                detail.Text($"{p1}{R}"),
                detail.Text($"{p2}  {PanelRenderer.Bold}{acc}Content{R}"),
                ..MarkdownRenderer.Render(detail, selected.Content).Select(w => w),
            ]).FillWidth(2).FillHeight(),
        ]).Fill();
    }
}
