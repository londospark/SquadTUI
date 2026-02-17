using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class DecisionsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var decisions = state.Decisions.GetOrEmpty();
        if (decisions.Count == 0)
        {
            var D0 = PanelRenderer.Dim;
            var R0 = PanelRenderer.Reset;
            return v.VStack(empty => [
                empty.Text(""),
                empty.Text($"  {D0}No decisions found. Ensure your .squad/ directory contains decision files.{R0}"),
            ]).Fill();
        }
        var listItems = decisions.Select(d => $"  📋 {d.Date}  {d.Title}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.DecisionSelectedIndex, 0, decisions.Count - 1);
        var selected = decisions[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var detailBg = ThemeManager.GetPanelDetailBgColor(state.SelectedThemeIndex);

        return v.HStack(h =>
        [
            new BackgroundPanelWidget(panelBg, h.VStack(left =>
            [
                left.Text($"  {hBg}{B}{acc} 📋  Decisions {R}"),
                left.Text($"  {D}Team decisions and architectural choices{R}"),
                left.Text(""),
                left.Text($"  {sec}{new string('━', 30)}{R}"),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.DecisionSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight()),

            new BackgroundPanelWidget(detailBg, h.VStack(detail =>
            [
                detail.Text(""),
                detail.Text($"  {hBg}{B}{acc} 📋  {selected.Title} {R}"),
                detail.Text(""),
                detail.Text($"  {sec}{new string('━', 36)}{R}"),
                detail.Text(""),
                detail.Text($"    {D}Date:{R}      {B}{selected.Date}{R}"),
                detail.Text($"    {D}Author:{R}    👤 {B}{selected.Author}{R}"),
                detail.Text(""),
                detail.Text($"  {sec}{new string('━', 36)}{R}"),
                detail.Text(""),
                detail.Text($"  {hBg}{B}{acc}Content{R}"),
                detail.Text(""),
                ..MarkdownRenderer.Render(detail, selected.Content).Select(w => w),
            ]).FillWidth(2).FillHeight()),
        ]).Fill();
    }
}
