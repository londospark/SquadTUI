using LanguageExt;
using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

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
        var memberTasks = (from t in allTasks
                          where t.Assignee.Match(Some: a => a == member.Name, None: () => false)
                          select t).ToList();
        var logs = state.LogEntries.GetOrEmpty();
        var recentLogs = logs.Where(l => l.Participants.Contains(member.Name)).Take(3).ToList();

        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var em = state.Settings.ShowEmoji;

        return new BackgroundPanelWidget(panelBg, v.VStack(inner =>
        {
            var widgets = new List<Hex1bWidget>
            {
                inner.VStack(header =>
                [
                    header.Text($"  {B}{acc}{Icon("👤", "◆", em)} {member.Name}{R}"),
                    header.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                    header.Text(""),
                    header.Text($"  {GetStatusBadge(member.Status, em)} {B}{member.Name}{R}  {D}—{R}  {member.Role}{R}"),
                    header.Text($"  {D}Status:{R} {member.Status}    {D}Current Task:{R} {member.CurrentTask.IfNone($"{D}None{R}")}{R}"),
                ]),

                inner.VStack(taskSection =>
                {
                    var tw = new List<Hex1bWidget>
                    {
                        taskSection.Text(""),
                        taskSection.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                        taskSection.Text(""),
                        taskSection.Text($"  {B}{acc}{Icon("📋", "▪", em)} Tasks{R}"),
                        taskSection.Text(""),
                    };
                    if (memberTasks.Count > 0)
                        foreach (var t in memberTasks)
                            tw.Add(taskSection.Text($"  {GetTaskBadge(t.Status, em)} {B}{t.Title}{R}  {D}{t.Description.IfNone("")}{R}"));
                    else
                        tw.Add(taskSection.Text($"  {D}No tasks assigned{R}"));
                    return tw.ToArray();
                }),

                inner.VStack(charterSection =>
                [
                    charterSection.Text(""),
                    charterSection.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                    charterSection.Text(""),
                    charterSection.Text($"  {B}{acc}{Icon("📜", "▪", em)} Charter{R}"),
                    charterSection.Text(""),
                    ..MarkdownRenderer.Render(charterSection, charter)
                ]),
            };

            if (recentLogs.Count > 0)
            {
                widgets.Add(inner.VStack(logSection =>
                [
                    logSection.Text(""),
                    logSection.Text($"  {D}{sec}{new string('━', 44)}{R}"),
                    logSection.Text(""),
                    logSection.Text($"  {B}{acc}{Icon("📊", "▪", em)} Recent Activity{R}"),
                    logSection.Text(""),
                    ..recentLogs.Select(l => logSection.Text($"  {D}{l.Date}{R}  {l.Topic}  {D}{l.Summary}{R}"))
                ]));
            }

            widgets.Add(inner.Text(""));
            widgets.Add(inner.Text($"  {D}Esc Back to Roster    E Edit Charter{R}"));

            return widgets.ToArray();
        }).Fill());
    }

    private static string GetStatusBadge(Models.MemberStatus status, bool showEmoji) => status switch
    {
        Models.MemberStatus.Active => Icon("✅", "[+]", showEmoji),
        Models.MemberStatus.Idle => Icon("🟡", "[~]", showEmoji),
        Models.MemberStatus.Working => Icon("🔵", "[>]", showEmoji),
        Models.MemberStatus.Offline => Icon("⚫", "[-]", showEmoji),
        _ => Icon("⚪", "[ ]", showEmoji)
    };

    private static string GetTaskBadge(Models.SquadTaskStatus status, bool showEmoji) => status switch
    {
        Models.SquadTaskStatus.InProgress => Icon("🔄", ">", showEmoji),
        Models.SquadTaskStatus.Done => Icon("✅", "+", showEmoji),
        Models.SquadTaskStatus.Pending => Icon("⏳", "~", showEmoji),
        Models.SquadTaskStatus.Blocked => Icon("🚫", "-", showEmoji),
        _ => Icon("⚪", "[ ]", showEmoji)
    };
}
