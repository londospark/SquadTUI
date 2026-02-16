using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class DashboardScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var members = state.Members ?? SampleData.Members;
        var tasks = SampleData.Tasks;
        var activeCount = members.Count(m => m.Status == Models.MemberStatus.Active);
        var inProgressTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress);
        var completedTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.Done);
        var logEntries = state.LogEntries ?? SampleData.LogEntries;
        var decisions = state.Decisions ?? SampleData.Decisions;

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): 3-column dashboard
            r.WhenMinWidth(120, r => r.HStack(h =>
            [
                h.Border(b =>
                [
                    b.Text($"  👥 Members: {members.Count}"),
                    b.Text($"  ✅ Active: {activeCount}"),
                    b.Text(""),
                    ..members.Select(m => b.Text($"  {GetStatusBadge(m.Status)} {m.Name} — {m.Role}"))
                ]).Title("🏠 Team").FillWidth(1).FillHeight(),

                h.VStack(mid =>
                [
                    mid.Border(b =>
                    [
                        b.Text($"  📊 Total: {tasks.Count}  🔄 Active: {inProgressTasks}  ✅ Done: {completedTasks}"),
                        b.Text(""),
                        ..logEntries.Take(3).Select(l => b.Text($"  📅 {l.Date}  {l.Topic}"))
                    ]).Title("📊 Activity").FillHeight(),

                    mid.Border(b =>
                    [
                        ..decisions.Take(4).Select(d => b.Text($"  📋 {d.Date}  {d.Title} ({d.Author})"))
                    ]).Title("📋 Decisions").FillHeight(),
                ]).FillWidth(2).FillHeight(),

                h.Border(b =>
                [
                    b.Text($"  🎯 Velocity: {completedTasks} tasks/sprint"),
                    b.Text($"  ⏳ Pending: {tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending)}"),
                    b.Text($"  🚫 Blocked: {tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked)}"),
                    b.Text(""),
                    b.Text("  ── Quick Actions ──"),
                    b.Text("  [1-6] Navigate screens"),
                    b.Text("  [T]   Cycle theme"),
                    b.Text("  [Q]   Quit"),
                ]).Title("📈 Summary").FillWidth(1).FillHeight(),
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.HStack(h =>
            [
                h.Border(b =>
                [
                    b.Text($"  👥 Team Members: {members.Count} ({activeCount} active)"),
                    b.Text($"  📊 Tasks: {tasks.Count} — {inProgressTasks} active, {completedTasks} done"),
                    b.Text(""),
                    ..logEntries.Take(3).Select(l => b.Text($"  📅 {l.Date}  {l.Topic}"))
                ]).Title("🏠 Dashboard").FillWidth(2).FillHeight(),

                h.Border(b =>
                [
                    ..decisions.Take(4).Select(d => b.Text($"  📋 {d.Title}"))
                ]).Title("📋 Recent").FillWidth(1).FillHeight(),
            ])),

            // Narrow layout: single column
            r.Otherwise(r => r.Border(b =>
            [
                b.Text($"  👥 Team Members: {members.Count} ({activeCount} active)"),
                b.Text($"  📊 Tasks: {tasks.Count} total — {inProgressTasks} in progress, {completedTasks} done"),
                b.Text(""),
                b.Text("  ── Recent Activity ──"),
                ..logEntries.Take(2).Select(l => b.Text($"  📅 {l.Date}  {l.Topic}")),
                b.Text(""),
                b.Text("  ── Recent Decisions ──"),
                ..decisions.Take(2).Select(d => b.Text($"  📋 {d.Date}  {d.Title} ({d.Author})")),
                b.Text(""),
                b.Text("  Press number keys to navigate. Q to quit."),
            ]).Title("🏠 Dashboard")),
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
}
