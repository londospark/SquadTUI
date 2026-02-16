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
        var listItems = members.Select(m => $"{GetStatusBadge(m.Status)} {m.Name} — {m.Role}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
        var selected = members[selectedIdx];
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

        var charter = SampleData.GetCharterFor(selected.Name);
        var charterExcerpt = charter.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith('#')).Take(3);
        var memberTasks = SampleData.Tasks.Where(t => t.Assignee == selected.Name).ToList();
        var logs = state.LogEntries ?? SampleData.LogEntries;
        var recentLogs = logs.Where(l => l.Participants.Contains(selected.Name)).Take(3).ToList();

        return v.HStack(h =>
        [
            h.VStack(left =>
            [
                left.Text($"  {PanelRenderer.Bold}{acc}👥 Team Roster{R}"),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.RosterSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ]).FillWidth(1).FillHeight(),

            h.VStack(detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text($"  {PanelRenderer.Bold}{acc}👤 {selected.Name}{R}"),
                    detail.Text(""),
                    detail.Text($"  \x1b[90mRole:\x1b[0m     \x1b[1m{selected.Role}{R}"),
                    detail.Text($"  \x1b[90mStatus:\x1b[0m   {GetStatusBadge(selected.Status)} {selected.Status}{R}"),
                    detail.Text($"  \x1b[90mTask:\x1b[0m     {selected.CurrentTask ?? "\x1b[90mNone\x1b[0m"}{R}"),
                };

                // Charter excerpt
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text($"  {PanelRenderer.Bold}{acc}📜 Charter{R}"));
                foreach (var line in charterExcerpt)
                    widgets.Add(detail.Text($"  {line.Trim()}{R}"));

                // Tasks
                if (memberTasks.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {PanelRenderer.Bold}{acc}📋 Tasks{R}"));
                    foreach (var t in memberTasks)
                        widgets.Add(detail.Text($"  {GetTaskBadge(t.Status)} {t.Title} \x1b[90m— {t.Description}{R}"));
                }

                // Recent activity
                if (recentLogs.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {PanelRenderer.Bold}{acc}📊 Recent Activity{R}"));
                    foreach (var l in recentLogs)
                        widgets.Add(detail.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic} — {l.Summary}{R}"));
                }

                return widgets.ToArray();
            }).FillWidth(2).FillHeight(),
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
