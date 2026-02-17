using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class MemberDetailScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members.GetOrEmpty();
        if (members.Count == 0)
        {
            var D0 = PanelRenderer.Dim;
            var R0 = PanelRenderer.Reset;
            return v.VStack(empty => [
                empty.Text(""),
                empty.Text($"  {D0}No member selected. Ensure your .squad/ directory contains a roster.{R0}"),
            ]).Fill();
        }
        var memberName = state.SelectedMemberName ?? members[0].Name;
        var member = members.FirstOrDefault(m => m.Name == memberName) ?? members[0];

        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        var allTasks = state.Tasks.GetOrEmpty();
        var memberTasks = allTasks.Where(t => t.Assignee == member.Name).ToList();
        var logs = state.LogEntries.GetOrEmpty();
        var recentLogs = logs.Where(l => l.Participants.Contains(member.Name)).Take(3).ToList();

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);

        return new BackgroundPanelWidget(panelBg, v.VStack(inner =>
        {
            var widgets = new List<Hex1bWidget>
            {
                inner.VStack(header =>
                [
                    header.Text($"  {B}{acc}👤 {member.Name}{R}"),
                    header.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                    header.Text($"  {GetStatusBadge(member.Status)} {B}{member.Name}{R}  {D}—{R}  {member.Role}{R}"),
                    header.Text($"  {D}Status:{R} {member.Status}    {D}Current Task:{R} {member.CurrentTask ?? $"{D}None{R}"}{R}"),
                ]),

                inner.VStack(taskSection =>
                {
                    var tw = new List<Hex1bWidget>
                    {
                        taskSection.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                        taskSection.Text($"  {B}{acc}📋 Tasks{R}"),
                    };
                    if (memberTasks.Count > 0)
                        foreach (var t in memberTasks)
                            tw.Add(taskSection.Text($"  {GetTaskBadge(t.Status)} {B}{t.Title}{R}  {D}{t.Description}{R}"));
                    else
                        tw.Add(taskSection.Text($"  {D}No tasks assigned{R}"));
                    return tw.ToArray();
                }),

                inner.VStack(charterSection =>
                [
                    charterSection.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                    charterSection.Text($"  {B}{acc}📜 Charter{R}"),
                    ..MarkdownRenderer.Render(charterSection, charter)
                ]),
            };

            if (recentLogs.Count > 0)
            {
                widgets.Add(inner.VStack(logSection =>
                [
                    logSection.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                    logSection.Text($"  {B}{acc}📊 Recent Activity{R}"),
                    ..recentLogs.Select(l => logSection.Text($"  {D}{l.Date}{R}  {l.Topic}  {D}{l.Summary}{R}"))
                ]));
            }

            widgets.Add(inner.Text(""));
            widgets.Add(inner.Text($"  {D}Esc Back to Roster    E Edit Charter{R}"));

            return widgets.ToArray();
        }).Fill());
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
