using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class ActivityLogScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var logs = state.LogEntries ?? SampleData.LogEntries;
        var listItems = logs.Select(l => $"📅 {l.Date}  {l.Topic}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.LogSelectedIndex, 0, logs.Count - 1);
        var selected = logs[selectedIdx];
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
                left.Text($"{p1}  {PanelRenderer.Bold}{acc}📊 Activity Log{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.LogSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"{p1}  {PanelRenderer.Bold}{acc}📅 {selected.Topic}{R}"),
                    detail.Text($"{p1}{R}"),
                    detail.Text($"{p1}  \x1b[90mDate:\x1b[0m  {selected.Date}{R}"),
                    detail.Text($"{p1}  \x1b[90m👥 Participants:\x1b[0m {string.Join(", ", selected.Participants)}{R}"),
                    detail.Text($"{p1}{R}"),
                    detail.Text($"{p2}  {PanelRenderer.Bold}{acc}Summary{R}"),
                    detail.Text($"{p2}  {selected.Summary}{R}"),
                };

                if (selected.Decisions.Count > 0)
                {
                    widgets.Add(detail.Text($"{p1}{R}"));
                    widgets.Add(detail.Text($"{p1}  {PanelRenderer.Bold}{acc}Decisions{R}"));
                    foreach (var d in selected.Decisions)
                        widgets.Add(detail.Text($"{p1}    • {d}{R}"));
                }

                if (selected.Outcomes.Count > 0)
                {
                    widgets.Add(detail.Text($"{p1}{R}"));
                    widgets.Add(detail.Text($"{p2}  {PanelRenderer.Bold}{acc}Outcomes{R}"));
                    foreach (var o in selected.Outcomes)
                        widgets.Add(detail.Text($"{p2}    • {o}{R}"));
                }

                return widgets.ToArray();
            }).FillWidth(2).FillHeight(),
        ]).Fill();
    }
}
