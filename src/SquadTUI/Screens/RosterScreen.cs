using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class RosterScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members.GetOrEmpty();
        if (members.Count == 0)
            return ScreenHelper.EmptyState(v, "No members found. Ensure your .squad/ directory contains a roster.");

        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);
        var listItems = members.Select(m => $"  {StatusBadges.Member(m.Status, t.Em)} {m.Name} — {m.Role}").ToList() as IReadOnlyList<string>;

        var selectedIdx = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
        var selected = members[selectedIdx];

        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        var charterExcerpt = charter.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith('#')).Take(3);
        var allTasks = state.Tasks.GetOrEmpty();
        var memberTasks = (from task in allTasks
                          where task.Assignee.Match(Some: a => a == selected.Name, None: () => false)
                          select task).ToList();
        var logs = state.LogEntries.GetOrEmpty();
        var recentLogs = logs.Where(l => l.Participants.Contains(selected.Name)).Take(3).ToList();

        return ScreenHelper.ListDetailLayout(v, t,
            listContent: left =>
            [
                left.Text(t.SectionHeader("👥", "◆", "Team Roster")),
                left.Text(t.Separator(30)),
                left.Text(""),
                left.List(listItems)
                    .OnSelectionChanged(e => { state.RosterSelectedIndex = e.SelectedIndex; })
                    .Fill()
            ],
            detailContent: detail =>
            {
                var widgets = new List<Hex1bWidget>
                {
                    detail.Text(t.SectionHeader("👤", "◆", selected.Name)),
                    detail.Text(t.Separator()),
                    detail.Text(""),
                    detail.Text($"  {t.D}Role:{t.R}      {t.B}{selected.Role}{t.R}"),
                    detail.Text($"  {t.D}Status:{t.R}    {StatusBadges.Member(selected.Status, t.Em)} {selected.Status}{t.R}"),
                    detail.Text($"  {t.D}Task:{t.R}      {(memberTasks.Count > 0 ? memberTasks[0].Title : selected.CurrentTask.IfNone($"{t.D}None{t.R}"))}{t.R}"),
                };

                // Tasks section
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text(t.Separator()));
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text(t.SectionHeader("📋", "▪", "Tasks")));
                widgets.Add(detail.Text(""));
                if (memberTasks.Count > 0)
                    foreach (var task in memberTasks)
                        widgets.Add(detail.Text($"  {StatusBadges.Task(task.Status, t.Em)} {task.Title}  {t.D}{task.Description.IfNone("")}{t.R}"));
                else
                    widgets.Add(detail.Text($"  {t.D}No tasks assigned{t.R}"));

                // Charter excerpt
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text(t.Separator()));
                widgets.Add(detail.Text(""));
                widgets.Add(detail.Text(t.SectionHeader("📜", "▪", "Charter")));
                widgets.Add(detail.Text(""));
                foreach (var line in charterExcerpt)
                    widgets.Add(detail.Text($"  {t.D}{line.Trim()}{t.R}"));

                // Recent activity
                if (recentLogs.Count > 0)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.Separator()));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.SectionHeader("📊", "▪", "Recent Activity")));
                    widgets.Add(detail.Text(""));
                    foreach (var l in recentLogs)
                        widgets.Add(detail.Text($"  {t.D}{l.Date}{t.R}  {l.Topic}  {t.D}{l.Summary}{t.R}"));
                }

                // Confirmation/status messages
                if (state.ConfirmingRemove)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.Separator()));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {t.B}\x1b[93mRemove {selected.Name}? (Y/N){t.R}"));
                }
                else if (state.AddMemberMessage != null)
                {
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text(t.Separator()));
                    widgets.Add(detail.Text(""));
                    widgets.Add(detail.Text($"  {t.D}{state.AddMemberMessage}{t.R}"));
                }

                return widgets.ToArray();
            },
            listWeight: 2,
            detailWeight: 3);
    }
}