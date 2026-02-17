using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class RosterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members ?? SampleData.Members;
        var listItems = members.Select(m => $"  {GetStatusBadge(m.Status)} {m.Name} — {m.Role}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
        var selected = members[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;

        var charter = SampleData.GetCharterFor(selected.Name);
        var charterExcerpt = charter.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith('#')).Take(3);
        var allTasks = state.Tasks ?? SampleData.Tasks;
        var memberTasks = allTasks.Where(t => t.Assignee == selected.Name).ToList();
        var logs = state.LogEntries ?? SampleData.LogEntries;
        var recentLogs = logs.Where(l => l.Participants.Contains(selected.Name)).Take(3).ToList();
        var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var detailBg = ThemeManager.GetPanelDetailBgColor(state.SelectedThemeIndex);

        return v.HStack(h =>
        [
            new BackgroundPanelWidget(panelBg, h.VStack(left =>
            [
                left.Text($"  {hBg}{B}{acc}👥 Team Roster{R}"),
                left.Text($"  {sec}{new string('━', 30)}{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.RosterSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(2).FillHeight()),

            new BackgroundPanelWidget(detailBg, h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"  {hBg}{B}{acc}👤 {selected.Name}{R}"),
                    detail.Text($"  {sec}{new string('━', 36)}{R}"),
                    detail.Text(""),
                    detail.Text($"  {D}Role:{R}      {B}{selected.Role}{R}"),
                    detail.Text($"  {D}Status:{R}    {GetStatusBadge(selected.Status)} {selected.Status}{R}"),
                    detail.Text($"  {D}Task:{R}      {selected.CurrentTask ?? $"{D}None{R}"}{R}"),
                };

                // Tasks section
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                widgets.Add(detail.Text($"  {hBg}{B}{acc}📋 Tasks{R}"));
                if (memberTasks.Count > 0)
                {
                    foreach (var t in memberTasks)
                        widgets.Add(detail.Text($"  {GetTaskBadge(t.Status)} {t.Title}  {D}{t.Description}{R}"));
                }
                else
                {
                    widgets.Add(detail.Text($"  {D}No tasks assigned{R}"));
                }

                // Charter excerpt
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                widgets.Add(detail.Text($"  {hBg}{B}{acc}📜 Charter{R}"));
                foreach (var line in charterExcerpt)
                    widgets.Add(detail.Text($"  {D}{line.Trim()}{R}"));

                // Recent activity
                if (recentLogs.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {sec}{new string('━', 36)}{R}"));
                    widgets.Add(detail.Text($"  {hBg}{B}{acc}📊 Recent Activity{R}"));
                    foreach (var l in recentLogs)
                        widgets.Add(detail.Text($"  {D}{l.Date}{R}  {l.Topic}  {D}{l.Summary}{R}"));
                }

                return widgets.ToArray();
            }).FillWidth(3).FillHeight()),
        ]).Fill();
    }

    private static string GetStatusBadge(Models.MemberStatus status) => status switch
    {
        Models.MemberStatus.Active => "✅",
        Models.MemberStatus.Idle => "🟡",
        Models.MemberStatus.Working => "🔵",
        Models.MemberStatus.Offline => "⚫",
        _ => "⚪"
    };

    private static string GetTaskBadge(Models.SquadTaskStatus status) => status switch
    {
        Models.SquadTaskStatus.InProgress => "🔄",
        Models.SquadTaskStatus.Done => "✅",
        Models.SquadTaskStatus.Pending => "⏳",
        Models.SquadTaskStatus.Blocked => "🚫",
        _ => "⚪"
    };
}
