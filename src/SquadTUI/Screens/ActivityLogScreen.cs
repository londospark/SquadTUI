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
        var listItems = logs.Select(l => $"  📅 {l.Date}  {l.Topic}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.LogSelectedIndex, 0, logs.Count - 1);
        var selected = logs[selectedIdx];
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
                left.Text($"  {B}{acc}{RV} 📊  Activity Log {R}"),
                left.Text($"  {D}Chronological record of squad interactions{R}"),
                left.Text(""),
                left.Text($"  {sec}{new string('━', 30)}{R}"),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.LogSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text(""),
                    detail.Text($"  {B}{acc}{RV} 📅  {selected.Topic} {R}"),
                    detail.Text(""),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"    {D}Date:{R}          {B}{selected.Date}{R}"),
                    detail.Text($"    {D}Participants:{R}  {string.Join(", ", selected.Participants)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {B}{acc}Summary{R}"),
                    detail.Text(""),
                    detail.Text($"    {selected.Summary}{R}"),
                };

                if (selected.Decisions.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {B}{acc}Decisions{R}"));
                    widgets.Add(detail.Text(""));
                    foreach (var d in selected.Decisions)
                        widgets.Add(detail.Text($"    • {d}{R}"));
                }

                if (selected.Outcomes.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {B}{acc}Outcomes{R}"));
                    widgets.Add(detail.Text(""));
                    foreach (var o in selected.Outcomes)
                        widgets.Add(detail.Text($"    • {o}{R}"));
                }

                return widgets.ToArray();
            }).FillWidth(2).FillHeight(),
        ]).Fill();
    }
}
