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
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): 3-column dashboard
            r.WhenMinWidth(120, r => r.HStack(h =>
            [
                h.VStack(left =>
                [
                    left.Text($"  {PanelRenderer.Bold}{acc}🏠 Team{R}"),
                    left.Text($"  \x1b[90m👥 Members:\x1b[0m \x1b[1m{members.Count}{R}"),
                    left.Text($"  \x1b[90m✅ Active:\x1b[0m \x1b[1m{activeCount}{R}"),
                    left.Text(""),
                    ..members.Select(m => left.Text($"  {GetStatusBadge(m.Status)} \x1b[96m{m.Name}\x1b[0m \x1b[90m—\x1b[0m {m.Role}{R}"))
                ]).FillWidth(1).FillHeight(),

                h.VStack(mid =>
                [
                    mid.VStack(act =>
                    [
                        act.Text($"  {PanelRenderer.Bold}{acc}📊 Activity{R}"),
                        act.Text($"  \x1b[90m📊 Total:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m  \x1b[90m🔄 Active:\x1b[0m \x1b[1m{inProgressTasks}\x1b[0m  \x1b[90m✅ Done:\x1b[0m \x1b[1m{completedTasks}{R}"),
                        act.Text(""),
                        ..logEntries.Take(3).Select(l => act.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}{R}"))
                    ]).FillHeight(),

                    mid.VStack(dec =>
                    [
                        dec.Text($"  {PanelRenderer.Bold}{acc}📋 Decisions{R}"),
                        ..decisions.Take(4).Select(d => dec.Text($"  \x1b[90m📋 {d.Date}\x1b[0m  {d.Title} \x1b[90m({d.Author}){R}"))
                    ]).FillHeight(),
                ]).FillWidth(2).FillHeight(),

                h.VStack(right =>
                [
                    right.Text($"  {PanelRenderer.Bold}{acc}📈 Summary{R}"),
                    right.Text($"  \x1b[90m🎯 Velocity:\x1b[0m \x1b[1m{completedTasks}\x1b[0m \x1b[90mtasks/sprint{R}"),
                    right.Text($"  \x1b[90m⏳ Pending:\x1b[0m \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending)}{R}"),
                    right.Text($"  \x1b[90m🚫 Blocked:\x1b[0m \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked)}{R}"),
                ]).FillWidth(1).FillHeight(),
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.HStack(h =>
            [
                h.VStack(left =>
                [
                    left.Text($"  {PanelRenderer.Bold}{acc}🏠 Dashboard{R}"),
                    left.Text($"  \x1b[90m👥 Team Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m \x1b[90m({activeCount} active){R}"),
                    left.Text($"  \x1b[90m📊 Tasks:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m \x1b[90m— {inProgressTasks} active, {completedTasks} done{R}"),
                    left.Text(""),
                    ..logEntries.Take(3).Select(l => left.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}{R}"))
                ]).FillWidth(2).FillHeight(),

                h.VStack(right =>
                [
                    right.Text($"  {PanelRenderer.Bold}{acc}📋 Recent{R}"),
                    ..decisions.Take(4).Select(d => right.Text($"  📋 {d.Title}{R}"))
                ]).FillWidth(1).FillHeight(),
            ])),

            // Narrow layout: single column
            r.Otherwise(r => r.VStack(col =>
            [
                col.Text($"  {PanelRenderer.Bold}{acc}🏠 Dashboard{R}"),
                col.Text($"  \x1b[90m👥 Team Members:\x1b[0m \x1b[1m{members.Count}\x1b[0m \x1b[90m({activeCount} active){R}"),
                col.Text($"  \x1b[90m📊 Tasks:\x1b[0m \x1b[1m{tasks.Count}\x1b[0m \x1b[90mtotal — {inProgressTasks} in progress, {completedTasks} done{R}"),
                col.Text(""),
                col.Text($"  {PanelRenderer.Bold}── Recent Activity ──{R}"),
                ..logEntries.Take(2).Select(l => col.Text($"  \x1b[90m📅 {l.Date}\x1b[0m  {l.Topic}{R}")),
                col.Text(""),
                col.Text($"  {PanelRenderer.Bold}── Recent Decisions ──{R}"),
                ..decisions.Take(2).Select(d => col.Text($"  \x1b[90m📋 {d.Date}\x1b[0m  {d.Title} \x1b[90m({d.Author}){R}")),
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
