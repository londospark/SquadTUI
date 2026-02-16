using Hex1b;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

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
        var ti = state.SelectedThemeIndex;
        var c = ThemeManager.GetPanelColors(ti);
        var p1 = c.PanelBg;
        var p2 = c.NestedBg;
        var acc = c.Accent;
        var R = PanelRenderer.Reset;

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): 3-column dashboard
            r.WhenMinWidth(120, r => r.HStack(h =>
            [
                h.VStack(left =>
                [
                    left.Text($"{p1}  {PanelRenderer.Bold}{acc}🏠 Team{R}"),
                    left.Text($"{p1}  \x1b[90m👥 Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m{R}"),
                    left.Text($"{p1}  \x1b[90m✅ Active:\x1b[0m \x1b[1m{activeCount}\x1b[0m{R}"),
                    left.Text($"{p1}{R}"),
                    ..members.Select(m => left.Text($"{p1}  {GetStatusBadge(m.Status)} \x1b[96m{m.Name}\x1b[0m \x1b[90m—\x1b[0m {m.Role}{R}"))
                ]).FillWidth(1).FillHeight(),

                h.VStack(mid =>
                [
                    mid.VStack(act =>
                    [
                        act.Text($"{p1}  {PanelRenderer.Bold}{acc}📊 Activity{R}"),
                        act.Text($"{p1}  \x1b[90m📊 Total:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m  \x1b[90m🔄 Active:\x1b[0m \x1b[1m{inProgressTasks}\x1b[0m  \x1b[90m✅ Done:\x1b[0m \x1b[1m{completedTasks}\x1b[0m{R}"),
                        act.Text($"{p1}{R}"),
                        ..logEntries.Take(3).Select(l => act.Text($"{p1}  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}{R}"))
                    ]).FillHeight(),

                    mid.VStack(dec =>
                    [
                        dec.Text($"{p2}  {PanelRenderer.Bold}{acc}📋 Decisions{R}"),
                        ..decisions.Take(4).Select(d => dec.Text($"{p2}  \x1b[90m📋 {d.Date}\x1b[0m  {d.Title} \x1b[90m({d.Author})\x1b[0m{R}"))
                    ]).FillHeight(),
                ]).FillWidth(2).FillHeight(),

                h.VStack(right =>
                [
                    right.Text($"{p1}  {PanelRenderer.Bold}{acc}📈 Summary{R}"),
                    right.Text($"{p1}  \x1b[90m🎯 Velocity:\x1b[0m \x1b[1m{completedTasks}\x1b[0m \x1b[90mtasks/sprint\x1b[0m{R}"),
                    right.Text($"{p1}  \x1b[90m⏳ Pending:\x1b[0m \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending)}\x1b[0m{R}"),
                    right.Text($"{p1}  \x1b[90m🚫 Blocked:\x1b[0m \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked)}\x1b[0m{R}"),
                    right.Text($"{p1}{R}"),
                    right.Text($"{p1}  {PanelRenderer.Bold}── Quick Actions ──{R}"),
                    right.Text($"{p1}  \x1b[90m[1-6] Navigate screens\x1b[0m{R}"),
                    right.Text($"{p1}  \x1b[90m[T]   Cycle theme\x1b[0m{R}"),
                    right.Text($"{p1}  \x1b[90m[Q]   Quit\x1b[0m{R}"),
                ]).FillWidth(1).FillHeight(),
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.HStack(h =>
            [
                h.VStack(left =>
                [
                    left.Text($"{p1}  {PanelRenderer.Bold}{acc}🏠 Dashboard{R}"),
                    left.Text($"{p1}  \x1b[90m👥 Team Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m \x1b[90m({activeCount} active)\x1b[0m{R}"),
                    left.Text($"{p1}  \x1b[90m📊 Tasks:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m \x1b[90m— {inProgressTasks} active, {completedTasks} done\x1b[0m{R}"),
                    left.Text($"{p1}{R}"),
                    ..logEntries.Take(3).Select(l => left.Text($"{p1}  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}{R}"))
                ]).FillWidth(2).FillHeight(),

                h.VStack(right =>
                [
                    right.Text($"{p2}  {PanelRenderer.Bold}{acc}📋 Recent{R}"),
                    ..decisions.Take(4).Select(d => right.Text($"{p2}  📋 {d.Title}{R}"))
                ]).FillWidth(1).FillHeight(),
            ])),

            // Narrow layout: single column
            r.Otherwise(r => r.VStack(col =>
            [
                col.Text($"{p1}  {PanelRenderer.Bold}{acc}🏠 Dashboard{R}"),
                col.Text($"{p1}  \x1b[90m👥 Team Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m \x1b[90m({activeCount} active)\x1b[0m{R}"),
                col.Text($"{p1}  \x1b[90m📊 Tasks:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m \x1b[90mtotal — {inProgressTasks} in progress, {completedTasks} done\x1b[0m{R}"),
                col.Text($"{p1}{R}"),
                col.Text($"{p1}  {PanelRenderer.Bold}── Recent Activity ──{R}"),
                ..logEntries.Take(2).Select(l => col.Text($"{p1}  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}{R}")),
                col.Text($"{p1}{R}"),
                col.Text($"{p1}  {PanelRenderer.Bold}── Recent Decisions ──{R}"),
                ..decisions.Take(2).Select(d => col.Text($"{p1}  \x1b[90m📋 {d.Date}\x1b[0m  {d.Title} \x1b[90m({d.Author})\x1b[0m{R}")),
                col.Text($"{p1}{R}"),
                col.Text($"{p1}  \x1b[90mPress number keys to navigate. Q to quit.\x1b[0m{R}"),
            ])),
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
