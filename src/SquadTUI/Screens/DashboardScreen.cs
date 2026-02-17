using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using SquadTUI.Themes;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class DashboardScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        // Handle pending refresh from file watcher
        if (state.HasPendingRefresh)
        {
            state.HasPendingRefresh = false;
            state.LastRefreshTime = DateTime.Now;
        }

        var members = state.Members.GetOrEmpty();
        var tasks = state.Tasks.GetOrEmpty();
        var activeCount = members.Count(m => m.Status == Models.MemberStatus.Active);
        var inProgressTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress);
        var completedTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.Done);
        var pendingTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending);
        var blockedTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked);
        var logEntries = state.LogEntries.GetOrEmpty();
        var decisions = state.Decisions.GetOrEmpty();
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var rule = ThemeManager.GetDimRule(state.SelectedThemeIndex, 36);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var detailBg = ThemeManager.GetPanelDetailBgColor(state.SelectedThemeIndex);
        var altBg = ThemeManager.GetPanelAltBgColor(state.SelectedThemeIndex);
        var hlBg = ThemeManager.GetHighlightBg(state.SelectedThemeIndex);
        var hlFg = ThemeManager.GetHighlightFg(state.SelectedThemeIndex);
        var em = state.Settings.ShowEmoji;

        var liveText = state.IsLiveEnabled ? $"\x1b[32m● LIVE\x1b[0m" : "";
        var refreshText = $"{D}Updated: {state.LastRefreshTime:HH:mm:ss}{R}";
        var focus = state.DashboardFocusedPanel;

        string PanelHeader(int panelIndex, string emoji, string ascii, string title) =>
            focus == panelIndex
                ? $"  {hlBg}{hlFg} {Icon(emoji, ascii, em)} {title} {R}"
                : $"  {hBg}{B}{acc}{Icon(emoji, ascii, em)} {title}{R}";

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): rich 3-column dashboard
            r.WhenMinWidth(120, r => r.VStack(outer =>
            [
                outer.Text($"  {B}{acc}{Icon("⚡", "▸", em)}  SquadTUI Dashboard{R}  {liveText}  {refreshText}"),
                outer.Text($"  {D}{sec}Your AI squad at a glance{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    // Left: Team roster with status + task
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text(PanelHeader(0, "👥", "◆", "Team Roster")),
                            left.Text($"  {sec}{new string('━', 28)}{R}"),
                            left.Text(""),
                        };
                        foreach (var m in members)
                        {
                            w.Add(left.Text($"  {GetStatusBadge(m.Status, em)} {B}{m.Name}{R}  {D}{m.Role}{R}"));
                            var task = m.CurrentTask ?? "No active task";
                            w.Add(left.Text($"     {D}↳ {task}{R}"));
                        }
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {D}{Icon("👥", "◆", em)} {members.Count} members  ·  {Icon("✅", "+", em)} {activeCount} active{R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),

                    // Center: Activity + tasks + progress
                    new BackgroundPanelWidget(detailBg, h.VStack(mid =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            mid.Text(PanelHeader(1, "📊", "▪", "Activity & Progress")),
                            mid.Text($"  {sec}{new string('━', 36)}{R}"),
                            mid.Text(""),
                            mid.Text($"  {D}Tasks:{R}  {B}{tasks.Count}{R}  total"),
                            mid.Text($"  {Icon("🔄", ">", em)} {B}{inProgressTasks}{R} active   {Icon("✅", "+", em)} {B}{completedTasks}{R} done   {Icon("⏳", "~", em)} {B}{pendingTasks}{R} pending   {Icon("🚫", "-", em)} {B}{blockedTasks}{R} blocked"),
                            mid.Text(""),
                        };

                        // Progress bar
                        var total = tasks.Count > 0 ? tasks.Count : 1;
                        var doneWidth = (int)(completedTasks * 30.0 / total);
                        var activeWidth = (int)(inProgressTasks * 30.0 / total);
                        var remaining = 30 - doneWidth - activeWidth;
                        if (remaining < 0) remaining = 0;
                        w.Add(mid.Text($"  \x1b[32m{new string('█', doneWidth)}\x1b[33m{new string('▓', activeWidth)}{D}{new string('░', remaining)}{R}  {completedTasks * 100 / total}%"));
                        w.Add(mid.Text(""));

                        w.Add(mid.Text($"  {sec}{new string('━', 36)}{R}"));
                        w.Add(mid.Text(""));
                        w.Add(mid.Text($"  {hBg}{B}{acc}{Icon("📅", "▪", em)} Recent Activity{R}"));
                        w.Add(mid.Text(""));
                        foreach (var l in logEntries.Take(5))
                            w.Add(mid.Text($"  {D}{l.Date}{R}  {l.Topic}  {D}({string.Join(", ", l.Participants.Take(2))}){R}"));

                        return w.ToArray();
                    }).FillWidth(2).FillHeight()),

                    // Right: Decisions + velocity
                    new BackgroundPanelWidget(altBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text(PanelHeader(2, "📋", "▪", "Decisions")),
                            right.Text($"  {sec}{new string('━', 28)}{R}"),
                            right.Text(""),
                        };
                        foreach (var d in decisions.Take(5))
                            w.Add(right.Text($"  {D}{d.Date}{R}  {d.Title}  {D}({d.Author}){R}"));

                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {sec}{new string('━', 28)}{R}"));
                        w.Add(right.Text(""));
                        w.Add(right.Text(PanelHeader(3, "📈", "▪", "Sprint Metrics")));
                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {D}Velocity:{R}    {B}{completedTasks}{R} {D}tasks/sprint{R}"));
                        w.Add(right.Text($"  {D}Throughput:{R}  {B}{completedTasks + inProgressTasks}{R} {D}active items{R}"));
                        w.Add(right.Text($"  {D}Blocked:{R}    {B}{blockedTasks}{R} {D}items{R}"));
                        w.Add(right.Text($"  {D}Backlog:{R}    {B}{pendingTasks}{R} {D}pending{R}"));

                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),
                ]).Fill()
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.VStack(outer =>
            [
                outer.Text($"  {B}{acc}{Icon("⚡", "▸", em)}  SquadTUI Dashboard{R}  {liveText}  {refreshText}"),
                outer.Text($"  {D}{sec}Your AI squad at a glance{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text(PanelHeader(0, "👥", "◆", "Team")),
                            left.Text($"  {sec}{new string('━', 32)}{R}"),
                            left.Text(""),
                        };
                        foreach (var m in members)
                            w.Add(left.Text($"  {GetStatusBadge(m.Status, em)} {B}{m.Name}{R}  {D}{m.Role}{R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {D}{Icon("📊", "▪", em)} Tasks:{R} {B}{tasks.Count}{R} {D}— {inProgressTasks} active, {completedTasks} done{R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {sec}{new string('━', 32)}{R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text(PanelHeader(1, "📅", "▪", "Recent")));
                        w.Add(left.Text(""));
                        foreach (var l in logEntries.Take(3))
                            w.Add(left.Text($"  {D}{l.Date}{R}  {l.Topic}"));
                        return w.ToArray();
                    }).FillWidth(2).FillHeight()),

                    new BackgroundPanelWidget(detailBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text(PanelHeader(2, "📋", "▪", "Decisions")),
                            right.Text($"  {sec}{new string('━', 24)}{R}"),
                            right.Text(""),
                        };
                        foreach (var d in decisions.Take(4))
                            w.Add(right.Text($"  {D}{d.Date}{R}  {d.Title}"));
                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {sec}{new string('━', 24)}{R}"));
                        w.Add(right.Text(""));
                        w.Add(right.Text(PanelHeader(3, "📈", "▪", "Metrics")));
                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {D}Velocity:{R}  {B}{completedTasks}{R} {D}tasks/sprint{R}"));
                        w.Add(right.Text($"  {D}Pending:{R}   {B}{pendingTasks}{R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),
                ]).Fill()
            ])),

            // Narrow layout: single column
            r.Otherwise(r => new BackgroundPanelWidget(panelBg, r.VStack(col =>
            {
                var w = new List<Hex1bWidget>
                {
                    col.Text($"  {hBg}{B}{acc}{Icon("⚡", "▸", em)}  SquadTUI{R}  {liveText}"),
                    col.Text($"  {sec}{new string('━', 24)}{R}"),
                    col.Text($"  {D}{Icon("👥", "◆", em)} Members:{R} {B}{members.Count}{R}  {D}Tasks:{R} {B}{tasks.Count}{R}"),
                    col.Text(""),
                    col.Text(PanelHeader(1, "📅", "▪", "Recent")),
                    col.Text(""),
                };
                foreach (var l in logEntries.Take(2))
                    w.Add(col.Text($"  {D}{l.Date}{R}  {l.Topic}"));
                w.Add(col.Text(""));
                w.Add(col.Text(PanelHeader(2, "📋", "▪", "Decisions")));
                w.Add(col.Text(""));
                foreach (var d in decisions.Take(2))
                    w.Add(col.Text($"  {D}{d.Date}{R}  {d.Title}  {D}({d.Author}){R}"));
                return w.ToArray();
            }))),
        ]).Fill().RedrawAfter(3000);
    }

    private static string GetStatusBadge(Models.MemberStatus status, bool showEmoji) => status switch
    {
        Models.MemberStatus.Active => Icon("✅", "[+]", showEmoji),
        Models.MemberStatus.Idle => Icon("🟡", "[~]", showEmoji),
        Models.MemberStatus.Working => Icon("🔵", "[>]", showEmoji),
        Models.MemberStatus.Offline => Icon("⚫", "[-]", showEmoji),
        _ => Icon("⚪", "[ ]", showEmoji)
    };
}
