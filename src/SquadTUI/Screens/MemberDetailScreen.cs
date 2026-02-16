using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class MemberDetailScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var memberName = state.SelectedMemberName ?? "Solaire";
        var members = state.Members ?? SampleData.Members;
        var member = members.FirstOrDefault(m => m.Name == memberName) ?? members[0];

        var charter = SampleData.GetCharterFor(member.Name);
        var memberTasks = SampleData.Tasks.Where(t => t.Assignee == member.Name).ToList();
        var logs = state.LogEntries ?? SampleData.LogEntries;
        var recentLogs = logs.Where(l => l.Participants.Contains(member.Name)).Take(3).ToList();

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

        return v.VStack(inner =>
        {
            var widgets = new List<Hex1bWidget>
            {
                inner.VStack(header =>
                [
                    header.Text($"  {PanelRenderer.Bold}{acc}👤 Member{R}"),
                    header.Text($"  {GetStatusBadge(member.Status)} \x1b[1m{member.Name}\x1b[0m \x1b[90m—\x1b[0m {member.Role}{R}"),
                    header.Text($"  \x1b[90mStatus:\x1b[0m {member.Status}    \x1b[90mCurrent Task:\x1b[0m {member.CurrentTask ?? "\x1b[90mNone\x1b[0m"}{R}"),
                ]),

                inner.VStack(charterSection =>
                [
                    charterSection.Text($"  {PanelRenderer.Bold}{acc}📜 Charter{R}"),
                    ..MarkdownRenderer.Render(charterSection, charter)
                ]),
            };

            if (memberTasks.Count > 0)
            {
                widgets.Add(inner.VStack(taskSection =>
                [
                    taskSection.Text($"  {PanelRenderer.Bold}{acc}📋 Tasks{R}"),
                    ..memberTasks.Select(t => taskSection.Text($"  {GetTaskBadge(t.Status)} {t.Title}{R}"))
                ]));
            }

            if (recentLogs.Count > 0)
            {
                widgets.Add(inner.VStack(logSection =>
                [
                    logSection.Text($"  {PanelRenderer.Bold}{acc}📊 Recent Activity{R}"),
                    ..recentLogs.Select(l => logSection.Text($"  📅 {l.Date}  {l.Topic} — {l.Summary}{R}"))
                ]));
            }

            widgets.Add(inner.Text($"\x1b[90m  Esc Back to Roster    E Edit Charter\x1b[0m"));

            return widgets.ToArray();
        }).Fill();
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
