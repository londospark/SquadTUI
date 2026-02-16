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
        var listItems = decisions.Select(d => $"  📋 {d.Date}  {d.Title}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.DecisionSelectedIndex, 0, decisions.Count - 1);
        var selected = decisions[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var RV = "\x1b[7m"; // reverse video

        return v.HStack(h =>
        [
            h.VStack(left =>
            [
                left.Text($"  {B}{acc}{RV} 📋  Decisions {R}"),
                left.Text($"  {D}Team decisions and architectural choices{R}"),
                left.Text(""),
                left.Text($"  {sec}{new string('━', 30)}{R}"),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.DecisionSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            [
                detail.Text(""),
                detail.Text($"  {B}{acc}{RV} 📋  {selected.Title} {R}"),
                detail.Text(""),
                detail.Text($"  {sec}{new string('━', 36)}{R}"),
                detail.Text(""),
                detail.Text($"    {D}Date:{R}      {B}{selected.Date}{R}"),
                detail.Text($"    {D}Author:{R}    👤 {B}{selected.Author}{R}"),
                detail.Text(""),
                detail.Text($"  {sec}{new string('━', 36)}{R}"),
                detail.Text(""),
                detail.Text($"  {B}{acc}Content{R}"),
                detail.Text(""),
                ..MarkdownRenderer.Render(detail, selected.Content).Select(w => w),
            ]).FillWidth(2).FillHeight(),
        ]).Fill();
    }
}
