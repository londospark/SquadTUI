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
        {
            var D0 = PanelRenderer.Dim;
            var R0 = PanelRenderer.Reset;
            return v.VStack(empty => [
                empty.Text(""),
                empty.Text($"  {D0}No activity log entries found. Ensure your .squad/ directory contains log files.{R0}"),
            ]).Fill();
        }
        var em = state.Settings.ShowEmoji;
        var listItems = logs.Select(l => $"  {Icon("📅", "▪", em)} {l.Date}  {l.Topic}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.LogSelectedIndex, 0, logs.Count - 1);
        var selected = logs[selectedIdx];
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
                left.Text($"  {hBg}{B}{acc} {Icon("📊", "▪", em)}  Activity Log {R}"),
                left.Text($"  {D}Chronological record of squad interactions{R}"),
                left.Text(""),
                left.Text($"  {sec}{new string('━', 30)}{R}"),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.LogSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight()),

            new BackgroundPanelWidget(detailBg, h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text(""),
                    detail.Text($"  {hBg}{B}{acc} {Icon("📅", "▪", em)}  {selected.Topic} {R}"),
                    detail.Text(""),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"    {D}Date:{R}          {B}{selected.Date}{R}"),
                    detail.Text($"    {D}Participants:{R}  {string.Join(", ", selected.Participants)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {hBg}{B}{acc}Summary{R}"),
                    detail.Text(""),
                    detail.Text($"    {selected.Summary}{R}"),
                };

                if (selected.Decisions.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {hBg}{B}{acc}Decisions{R}"));
                    widgets.Add(detail.Text(""));
                    foreach (var d in selected.Decisions)
                        widgets.Add(detail.Text($"    • {d}{R}"));
                }

                if (selected.Outcomes.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {hBg}{B}{acc}Outcomes{R}"));
                    widgets.Add(detail.Text(""));
                    foreach (var o in selected.Outcomes)
                        widgets.Add(detail.Text($"    • {o}{R}"));
                }

                return widgets.ToArray();
            }).FillWidth(2).FillHeight()),
        ]).Fill();
    }
}
