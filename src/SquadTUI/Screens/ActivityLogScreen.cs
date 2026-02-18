using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class ActivityLogScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var logs = state.LogEntries.GetOrEmpty();
        if (logs.Count == 0)
            return ScreenHelper.EmptyState(v, "No activity log entries found. Ensure your .squad/ directory contains log files.");

        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);
        var listItems = logs.Select(l => $"  {Icon("📅", "▪", t.Em)} {l.Date}  {l.Topic}").ToList() as IReadOnlyList<string>;
        var selectedIdx = Math.Clamp(state.LogSelectedIndex, 0, logs.Count - 1);
        var selected = logs[selectedIdx];

        return ScreenHelper.ListDetailLayout(v, t,
            listContent: left =>
            [
                left.Text(t.SectionHeader("📊", "▪", " Activity Log ")),
                left.Text($"  {t.D}Chronological record of squad interactions{t.R}"),
                left.Text(""),
                left.Text(t.Separator(30)),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.LogSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ],
            detailContent: detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text(""),
                    detail.Text(t.SectionHeader("📅", "▪", $" {selected.Topic} ")),
                    detail.Text(""),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text($"    {t.D}Date:{t.R}          {t.B}{selected.Date}{t.R}"),
                    detail.Text($"    {t.D}Participants:{t.R}  {string.Join(", ", selected.Participants)}{t.R}"),
                    detail.Text(""),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text(t.SectionHeader("Summary")),
                    detail.Text(""),
                    detail.Text($"    {selected.Summary}{t.R}"),
                };

                if (selected.Decisions.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.Separator()));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.SectionHeader("Decisions")));
                    widgets.Add(detail.Text(""));
                    foreach (var d in selected.Decisions)
                        widgets.Add(detail.Text($"    • {d}{t.R}"));
                }

                if (selected.Outcomes.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.Separator()));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.SectionHeader("Outcomes")));
                    widgets.Add(detail.Text(""));
                    foreach (var o in selected.Outcomes)
                        widgets.Add(detail.Text($"    • {o}{t.R}"));
                }

                return widgets.ToArray();
            });
    }
}
