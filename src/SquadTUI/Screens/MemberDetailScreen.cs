using LanguageExt;
using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class MemberDetailScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members.GetOrEmpty();
        if (members.Count == 0)
            return ScreenHelper.EmptyState(v, "No member selected. Ensure your .squad/ directory contains a roster.");

        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);
        var memberName = state.SelectedMemberName ?? members[0].Name;
        var member = members.FirstOrDefault(m => m.Name == memberName) ?? members[0];

        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        var allTasks = state.Tasks.GetOrEmpty();
        var memberTasks = (from task in allTasks
                          where task.Assignee.Match(Some: a => a == member.Name, None: () => false)
                          select task).ToList();
        var logs = state.LogEntries.GetOrEmpty();
        var recentLogs = logs.Where(l => l.Participants.Contains(member.Name)).Take(3).ToList();

        return new BackgroundPanelWidget(t.PanelBg, v.VStack(inner =>
        {
            var widgets = new List<Hex1bWidget>
            {
                inner.VStack(header =>
                [
                    header.Text(t.SectionHeader("👤", "◆", member.Name)),
                    header.Text(t.Separator(44)),
                    header.Text(""),
                    header.Text($"  {StatusBadges.Member(member.Status, t.Em)} {t.B}{member.Name}{t.R}  {t.D}—{t.R}  {member.Role}{t.R}"),
                    header.Text($"  {t.D}Status:{t.R} {member.Status}    {t.D}Current Task:{t.R} {member.CurrentTask.IfNone($"{t.D}None{t.R}")}{t.R}"),
                ]),

                inner.VStack(taskSection =>
                {
                    var tw = new List<Hex1bWidget>
                    {
                        taskSection.Text(""),
                        taskSection.Text(t.Separator(44)),
                        taskSection.Text(""),
                        taskSection.Text(t.SectionHeader("📋", "▪", "Tasks")),
                        taskSection.Text(""),
                    };
                    if (memberTasks.Count > 0)
                        foreach (var task in memberTasks)
                            tw.Add(taskSection.Text($"  {StatusBadges.Task(task.Status, t.Em)} {t.B}{task.Title}{t.R}  {t.D}{task.Description.IfNone("")}{t.R}"));
                    else
                        tw.Add(taskSection.Text($"  {t.D}No tasks assigned{t.R}"));
                    return tw.ToArray();
                }),

                inner.VStack(charterSection =>
                [
                    charterSection.Text(""),
                    charterSection.Text(t.Separator(44)),
                    charterSection.Text(""),
                    charterSection.Text(t.SectionHeader("📜", "▪", "Charter")),
                    charterSection.Text(""),
                    ..MarkdownRenderer.Render(charterSection, charter)
                ]),
            };

            if (recentLogs.Count > 0)
            {
                widgets.Add(inner.VStack(logSection =>
                [
                    logSection.Text(""),
                    logSection.Text(t.Separator(44)),
                    logSection.Text(""),
                    logSection.Text(t.SectionHeader("📊", "▪", "Recent Activity")),
                    logSection.Text(""),
                    ..recentLogs.Select(l => logSection.Text($"  {t.D}{l.Date}{t.R}  {l.Topic}  {t.D}{l.Summary}{t.R}"))
                ]));
            }

            widgets.Add(inner.Text(""));
            widgets.Add(inner.Text($"  {t.D}Esc Back to Roster    E Edit Charter{t.R}"));

            return widgets.ToArray();
        }).Fill());
    }
}
