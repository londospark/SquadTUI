using LanguageExt;
using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class DashboardScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members.GetOrEmpty();
        var tasks = state.Tasks.GetOrEmpty();
        var activeCount = members.Count(m => m.Status == MemberStatus.Active);
        var inProgressTasks = tasks.Count(t => t.Status == SquadTaskStatus.InProgress);
        var completedTasks = tasks.Count(t => t.Status == SquadTaskStatus.Done);
        var pendingTasks = tasks.Count(t => t.Status == SquadTaskStatus.Pending);
        var blockedTasks = tasks.Count(t => t.Status == SquadTaskStatus.Blocked);
        var logEntries = state.LogEntries.GetOrEmpty();
        var decisions = state.Decisions.GetOrEmpty();
        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);

        var liveText = state.IsLiveEnabled ? $"\x1b[32m● LIVE\x1b[0m" : "";
        var refreshText = $"{t.D}Updated: {state.LastRefreshTime:HH:mm:ss}{t.R}";
        var focus = state.DashboardFocusedPanel;

        string PanelHeader(int panelIndex, string emoji, string ascii, string title) =>
            focus == panelIndex
                ? $"  {t.HlBg}{t.B}{t.HlFg}{Icon(emoji, ascii, t.Em)} {title}{t.R}"
                : $"  {t.HBg}{t.B}{t.Acc}{Icon(emoji, ascii, t.Em)} {title}{t.R}";

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): rich 3-column dashboard
            r.WhenMinWidth(120, r => r.VStack(outer =>
            [
                outer.Text($"  {t.B}{t.Acc}{Icon("⚡", "▸", t.Em)}  SquadTUI Dashboard{t.R}  {liveText}  {refreshText}"),
                outer.Text($"  {t.D}{t.Sec}Your AI squad at a glance{t.R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    // Left: Team roster with status + task
                    new BackgroundPanelWidget(t.PanelBg, h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text(PanelHeader(0, "👥", "◆", "Team Roster")),
                            left.Text(t.Separator(28)),
                            left.Text(""),
                        };
                        foreach (var m in members)
                        {
                            w.Add(left.Text($"  {StatusBadges.Member(m.Status, t.Em)} {t.B}{m.Name}{t.R}  {t.D}{m.Role}{t.R}"));
                            var task = m.CurrentTask.IfNone("No active task");
                            w.Add(left.Text($"     {t.D}↳ {task}{t.R}"));
                        }
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {t.D}{Icon("👥", "◆", t.Em)} {members.Count} members  ·  {Icon("✅", "+", t.Em)} {activeCount} active{t.R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),

                    // Center: Activity + tasks + progress
                    new BackgroundPanelWidget(t.DetailBg, h.VStack(mid =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            mid.Text(PanelHeader(1, "📊", "▪", "Activity & Progress")),
                            mid.Text(t.Separator()),
                            mid.Text(""),
                            mid.Text($"  {t.D}Tasks:{t.R}  {t.B}{tasks.Count}{t.R}  total"),
                            mid.Text($"  {Icon("🔄", ">", t.Em)} {t.B}{inProgressTasks}{t.R} active   {Icon("✅", "+", t.Em)} {t.B}{completedTasks}{t.R} done   {Icon("⏳", "~", t.Em)} {t.B}{pendingTasks}{t.R} pending   {Icon("🚫", "-", t.Em)} {t.B}{blockedTasks}{t.R} blocked"),
                            mid.Text(""),
                        };

                        // Progress bar
                        var total = tasks.Count > 0 ? tasks.Count : 1;
                        var doneWidth = (int)(completedTasks * 30.0 / total);
                        var activeWidth = (int)(inProgressTasks * 30.0 / total);
                        var remaining = 30 - doneWidth - activeWidth;
                        if (remaining < 0) remaining = 0;
                        w.Add(mid.Text($"  \x1b[32m{new string('█', doneWidth)}\x1b[33m{new string('▓', activeWidth)}{t.D}{new string('░', remaining)}{t.R}  {completedTasks * 100 / total}%"));
                        w.Add(mid.Text(""));

                        w.Add(mid.Text(t.Separator()));
                        w.Add(mid.Text(""));
                        w.Add(mid.Text($"  {t.HBg}{t.B}{t.Acc}{Icon("📅", "▪", t.Em)} Recent Activity{t.R}"));
                        w.Add(mid.Text(""));
                        foreach (var l in logEntries.Take(5))
                            w.Add(mid.Text($"  {t.D}{l.Date}{t.R}  {l.Topic}  {t.D}({string.Join(", ", l.Participants.Take(2))}){t.R}"));

                        return w.ToArray();
                    }).FillWidth(2).FillHeight()),

                    // Right: Decisions + velocity
                    new BackgroundPanelWidget(t.AltBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text(PanelHeader(2, "📋", "▪", "Decisions")),
                            right.Text(t.Separator(28)),
                            right.Text(""),
                        };
                        foreach (var d in decisions.Take(5))
                            w.Add(right.Text($"  {t.D}{d.Date}{t.R}  {d.Title}  {t.D}({d.Author}){t.R}"));

                        w.Add(right.Text(""));
                        w.Add(right.Text(t.Separator(28)));
                        w.Add(right.Text(""));
                        w.Add(right.Text(PanelHeader(3, "📈", "▪", "Sprint Metrics")));
                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {t.D}Velocity:{t.R}    {t.B}{completedTasks}{t.R} {t.D}tasks/sprint{t.R}"));
                        w.Add(right.Text($"  {t.D}Throughput:{t.R}  {t.B}{completedTasks + inProgressTasks}{t.R} {t.D}active items{t.R}"));
                        w.Add(right.Text($"  {t.D}Blocked:{t.R}    {t.B}{blockedTasks}{t.R} {t.D}items{t.R}"));
                        w.Add(right.Text($"  {t.D}Backlog:{t.R}    {t.B}{pendingTasks}{t.R} {t.D}pending{t.R}"));

                        w.Add(right.Text(""));
                        w.Add(right.Text(t.Separator(28)));
                        w.Add(right.Text(""));
                        var skillsList = state.Skills.GetOrEmpty();
                        w.Add(right.Text(PanelHeader(4, "🔧", "◇", "Skills")));
                        w.Add(right.Text(""));
                        if (skillsList.Count > 0)
                            foreach (var sk in skillsList.Take(3))
                                w.Add(right.Text($"  {t.D}{Icon("🔧", "◇", t.Em)} {sk.Name}{t.R}"));
                        else
                            w.Add(right.Text($"  {t.D}No skills installed{t.R}"));
                        w.Add(right.Text($"  {t.D}{Icon("🔧", "◇", t.Em)} {skillsList.Count} total{t.R}"));

                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),
                ]).Fill()
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.VStack(outer =>
            [
                outer.Text($"  {t.B}{t.Acc}{Icon("⚡", "▸", t.Em)}  SquadTUI Dashboard{t.R}  {liveText}  {refreshText}"),
                outer.Text($"  {t.D}{t.Sec}Your AI squad at a glance{t.R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(t.PanelBg, h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text(PanelHeader(0, "👥", "◆", "Team")),
                            left.Text(t.Separator(32)),
                            left.Text(""),
                        };
                        foreach (var m in members)
                            w.Add(left.Text($"  {StatusBadges.Member(m.Status, t.Em)} {t.B}{m.Name}{t.R}  {t.D}{m.Role}{t.R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {t.D}{Icon("📊", "▪", t.Em)} Tasks:{t.R} {t.B}{tasks.Count}{t.R} {t.D}— {inProgressTasks} active, {completedTasks} done{t.R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text(t.Separator(32)));
                        w.Add(left.Text(""));
                        w.Add(left.Text(PanelHeader(1, "📅", "▪", "Recent")));
                        w.Add(left.Text(""));
                        foreach (var l in logEntries.Take(3))
                            w.Add(left.Text($"  {t.D}{l.Date}{t.R}  {l.Topic}"));
                        return w.ToArray();
                    }).FillWidth(2).FillHeight()),

                    new BackgroundPanelWidget(t.DetailBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text(PanelHeader(2, "📋", "▪", "Decisions")),
                            right.Text(t.Separator(24)),
                            right.Text(""),
                        };
                        foreach (var d in decisions.Take(4))
                            w.Add(right.Text($"  {t.D}{d.Date}{t.R}  {d.Title}"));
                        w.Add(right.Text(""));
                        w.Add(right.Text(t.Separator(24)));
                        w.Add(right.Text(""));
                        w.Add(right.Text(PanelHeader(3, "📈", "▪", "Metrics")));
                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {t.D}Velocity:{t.R}  {t.B}{completedTasks}{t.R} {t.D}tasks/sprint{t.R}"));
                        w.Add(right.Text($"  {t.D}Pending:{t.R}   {t.B}{pendingTasks}{t.R}"));
                        w.Add(right.Text(""));
                        w.Add(right.Text(t.Separator(24)));
                        w.Add(right.Text(""));
                        var skillsList = state.Skills.GetOrEmpty();
                        w.Add(right.Text(PanelHeader(4, "🔧", "◇", "Skills")));
                        w.Add(right.Text($"  {t.D}{Icon("🔧", "◇", t.Em)} {skillsList.Count} installed{t.R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),
                ]).Fill()
            ])),

            // Narrow layout: single column
            r.Otherwise(r => new BackgroundPanelWidget(t.PanelBg, r.VStack(col =>
            {
                var w = new List<Hex1bWidget>
                {
                    col.Text($"  {t.HBg}{t.B}{t.Acc}{Icon("⚡", "▸", t.Em)}  SquadTUI{t.R}  {liveText}"),
                    col.Text(t.Separator(24)),
                    col.Text($"  {t.D}{Icon("👥", "◆", t.Em)} Members:{t.R} {t.B}{members.Count}{t.R}  {t.D}Tasks:{t.R} {t.B}{tasks.Count}{t.R}"),
                    col.Text(""),
                    col.Text(PanelHeader(1, "��", "▪", "Recent")),
                    col.Text(""),
                };
                foreach (var l in logEntries.Take(2))
                    w.Add(col.Text($"  {t.D}{l.Date}{t.R}  {l.Topic}"));
                w.Add(col.Text(""));
                w.Add(col.Text(PanelHeader(2, "📋", "▪", "Decisions")));
                w.Add(col.Text(""));
                foreach (var d in decisions.Take(2))
                    w.Add(col.Text($"  {t.D}{d.Date}{t.R}  {d.Title}  {t.D}({d.Author}){t.R}"));
                return w.ToArray();
            }))),
        ]).Fill().RedrawAfter(3000);
    }

}