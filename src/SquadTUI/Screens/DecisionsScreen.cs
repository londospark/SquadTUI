using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class DecisionsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var decisions = state.Decisions.GetOrEmpty();
        if (decisions.Count == 0)
            return ScreenHelper.EmptyState(v, "No decisions found. Ensure your .squad/ directory contains decision files.");

        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);
        var listItems = decisions.Select(d => $"  {Icon("📋", "▪", t.Em)} {d.Date}  {d.Title}").ToList() as IReadOnlyList<string>;
        var selectedIdx = Math.Clamp(state.DecisionSelectedIndex, 0, decisions.Count - 1);
        var selected = decisions[selectedIdx];

        return ScreenHelper.ListDetailLayout(v, t,
            listContent: left =>
            [
                left.Text(t.SectionHeader("📋", "▪", " Decisions ")),
                left.Text($"  {t.D}Team decisions and architectural choices{t.R}"),
                left.Text(""),
                left.Text(t.Separator(30)),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.DecisionSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ],
            detailContent: detail =>
            [
                detail.Text(""),
                detail.Text(t.SectionHeader("📋", "▪", $" {selected.Title} ")),
                detail.Text(""),
                detail.Text(t.Separator()),
                detail.Text(""),
                detail.Text($"    {t.D}Date:{t.R}      {t.B}{selected.Date}{t.R}"),
                detail.Text($"    {t.D}Author:{t.R}    {Icon("👤", "◆", t.Em)} {t.B}{selected.Author}{t.R}"),
                detail.Text(""),
                detail.Text(t.Separator()),
                detail.Text(""),
                detail.Text(t.SectionHeader("Content")),
                detail.Text(""),
                ..MarkdownRenderer.Render(detail, selected.Content).Select(w => w),
            ]);
    }
}
