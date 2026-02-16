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
                    b.Text($"  \x1b[90m👥 Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m"),
                    b.Text($"  \x1b[90m✅ Active:\x1b[0m \x1b[1m{activeCount}\x1b[0m"),
                    b.Text(""),
                    ..members.Select(m => b.Text($"  {GetStatusBadge(m.Status)} \x1b[96m{m.Name}\x1b[0m \x1b[90m—\x1b[0m {m.Role}"))
                ]).Title("🏠 Team").FillWidth(1).FillHeight(),

                h.VStack(mid =>
                [
                    mid.Border(b =>
                    [
                        b.Text($"  \x1b[90m📊 Total:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m  \x1b[90m🔄 Active:\x1b[0m \x1b[1m{inProgressTasks}\x1b[0m  \x1b[90m✅ Done:\x1b[0m \x1b[1m{completedTasks}\x1b[0m"),
                        b.Text(""),
                        ..logEntries.Take(3).Select(l => b.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}"))
                    ]).Title("📊 Activity").FillHeight(),

                    mid.Border(b =>
                    [
                        ..decisions.Take(4).Select(d => b.Text($"  \x1b[90m📋 {d.Date}\x1b[0m  {d.Title} \x1b[90m({d.Author})\x1b[0m"))
                    ]).Title("📋 Decisions").FillHeight(),
                ]).FillWidth(2).FillHeight(),

                h.Border(b =>
                [
                    b.Text($"  \x1b[90m🎯 Velocity:\x1b[0m \x1b[1m{completedTasks}\x1b[0m \x1b[90mtasks/sprint\x1b[0m"),
                    b.Text($"  \x1b[90m⏳ Pending:\x1b[0m \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending)}\x1b[0m"),
                    b.Text($"  \x1b[90m🚫 Blocked:\x1b[0m \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked)}\x1b[0m"),
                    b.Text(""),
                    b.Text("\x1b[1m  ── Quick Actions ──\x1b[0m"),
                    b.Text("\x1b[90m  [1-6] Navigate screens\x1b[0m"),
                    b.Text("\x1b[90m  [T]   Cycle theme\x1b[0m"),
                    b.Text("\x1b[90m  [Q]   Quit\x1b[0m"),
                ]).Title("📈 Summary").FillWidth(1).FillHeight(),
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.HStack(h =>
            [
                h.Border(b =>
                [
                    b.Text($"  \x1b[90m👥 Team Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m \x1b[90m({activeCount} active)\x1b[0m"),
                    b.Text($"  \x1b[90m📊 Tasks:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m \x1b[90m— {inProgressTasks} active, {completedTasks} done\x1b[0m"),
                    b.Text(""),
                    ..logEntries.Take(3).Select(l => b.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}"))
                ]).Title("🏠 Dashboard").FillWidth(2).FillHeight(),

                h.Border(b =>
                [
                    ..decisions.Take(4).Select(d => b.Text($"  📋 {d.Title}"))
                ]).Title("📋 Recent").FillWidth(1).FillHeight(),
            ])),

            // Narrow layout: single column
            r.Otherwise(r => r.Border(b =>
            [
                b.Text($"  \x1b[90m👥 Team Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m \x1b[90m({activeCount} active)\x1b[0m"),
                b.Text($"  \x1b[90m📊 Tasks:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m \x1b[90mtotal — {inProgressTasks} in progress, {completedTasks} done\x1b[0m"),
                b.Text(""),
                b.Text("\x1b[1m  ── Recent Activity ──\x1b[0m"),
                ..logEntries.Take(2).Select(l => b.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}")),
                b.Text(""),
                b.Text("\x1b[1m  ── Recent Decisions ──\x1b[0m"),
                ..decisions.Take(2).Select(d => b.Text($"  \x1b[90m📋 {d.Date}\x1b[0m  {d.Title} \x1b[90m({d.Author})\x1b[0m")),
                b.Text(""),
                b.Text("\x1b[90m  Press number keys to navigate. Q to quit.\x1b[0m"),
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
