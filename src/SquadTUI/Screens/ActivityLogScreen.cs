using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class ActivityLogScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var logs = state.LogEntries ?? SampleData.LogEntries;
        var listItems = logs.Select(l => $"📅 {l.Date}  {l.Topic}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.LogSelectedIndex, 0, logs.Count - 1);
        var selected = logs[selectedIdx];

        return v.HStack(h =>
        [
            h.Border(b =>
            [
                b.List(listItems)
                    .OnSelectionChanged(e => { state.LogSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).Title("📊 Activity Log").FillWidth(2).FillHeight(),

            h.Border(b =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    b.Text($"  Topic: {selected.Topic}"),
                    b.Text($"  Date:  {selected.Date}"),
                    b.Text($"  👥 Participants: {string.Join(", ", selected.Participants)}"),
                    b.Text(""),
                    b.Text($"  {selected.Summary}"),
                };

                if (selected.Decisions.Count > 0)
                {
                    widgets.Add(b.Text(""));
                    widgets.Add(b.Text("  Decisions:"));
                    foreach (var d in selected.Decisions)
                        widgets.Add(b.Text($"    • {d}"));
                }

                if (selected.Outcomes.Count > 0)
                {
                    widgets.Add(b.Text(""));
                    widgets.Add(b.Text("  Outcomes:"));
                    foreach (var o in selected.Outcomes)
                        widgets.Add(b.Text($"    • {o}"));
                }

                return widgets.ToArray();
            }).Title("Details").Fill(),
        ]).Fill();
    }
}
