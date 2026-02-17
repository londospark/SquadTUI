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
        var pendingTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending);
        var blockedTasks = tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked);
        var logEntries = state.LogEntries ?? SampleData.LogEntries;
        var decisions = state.Decisions ?? SampleData.Decisions;
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var rule = ThemeManager.GetDimRule(state.SelectedThemeIndex, 36);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): rich 3-column dashboard
            r.WhenMinWidth(120, r => r.VStack(outer =>
            [
                outer.Text($"  {B}{acc}☀️  SquadTUI Dashboard{R}"),
                outer.Text($"  {D}{sec}Your AI squad at a glance{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    // Left: Team roster with status + task
                    h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text($"  {hBg}{B}{acc}👥 Team Roster{R}"),
                            left.Text($"  {sec}{new string('━', 28)}{R}"),
                        };
                        foreach (var m in members)
                        {
                            w.Add(left.Text($"  {GetStatusBadge(m.Status)} {B}{m.Name}{R}  {D}{m.Role}{R}"));
                            var task = m.CurrentTask ?? "No active task";
                            w.Add(left.Text($"     {D}↳ {task}{R}"));
                        }
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {D}👥 {members.Count} members  ·  ✅ {activeCount} active{R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight(),

                    // Center: Activity + tasks + progress
                    h.VStack(mid =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            mid.Text($"  {hBg}{B}{acc}📊 Activity & Progress{R}"),
                            mid.Text($"  {sec}{new string('━', 36)}{R}"),
                            mid.Text(""),
                            mid.Text($"  {D}Tasks:{R}  {B}{tasks.Count}{R}  total"),
                            mid.Text($"  🔄 {B}{inProgressTasks}{R} active   ✅ {B}{completedTasks}{R} done   ⏳ {B}{pendingTasks}{R} pending   🚫 {B}{blockedTasks}{R} blocked"),
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
                        w.Add(mid.Text($"  {hBg}{B}{acc}📅 Recent Activity{R}"));
                        foreach (var l in logEntries.Take(5))
                            w.Add(mid.Text($"  {D}{l.Date}{R}  {l.Topic}  {D}({string.Join(", ", l.Participants.Take(2))}){R}"));

                        return w.ToArray();
                    }).FillWidth(2).FillHeight(),

                    // Right: Decisions + velocity
                    h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text($"  {hBg}{B}{acc}📋 Decisions{R}"),
                            right.Text($"  {sec}{new string('━', 28)}{R}"),
                        };
                        foreach (var d in decisions.Take(5))
                            w.Add(right.Text($"  {D}{d.Date}{R}  {d.Title}  {D}({d.Author}){R}"));

                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {sec}{new string('━', 28)}{R}"));
                        w.Add(right.Text($"  {hBg}{B}{acc}📈 Sprint Metrics{R}"));
                        w.Add(right.Text($"  {D}Velocity:{R}    {B}{completedTasks}{R} {D}tasks/sprint{R}"));
                        w.Add(right.Text($"  {D}Throughput:{R}  {B}{completedTasks + inProgressTasks}{R} {D}active items{R}"));
                        w.Add(right.Text($"  {D}Blocked:{R}    {B}{blockedTasks}{R} {D}items{R}"));
                        w.Add(right.Text($"  {D}Backlog:{R}    {B}{pendingTasks}{R} {D}pending{R}"));

                        return w.ToArray();
                    }).FillWidth(1).FillHeight(),
                ]).Fill()
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.VStack(outer =>
            [
                outer.Text($"  {B}{acc}☀️  SquadTUI Dashboard{R}"),
                outer.Text($"  {D}{sec}Your AI squad at a glance{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text($"  {hBg}{B}{acc}👥 Team{R}  {D}({members.Count} members, {activeCount} active){R}"),
                            left.Text($"  {sec}{new string('━', 32)}{R}"),
                        };
                        foreach (var m in members)
                            w.Add(left.Text($"  {GetStatusBadge(m.Status)} {B}{m.Name}{R}  {D}{m.Role}{R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {D}📊 Tasks:{R} {B}{tasks.Count}{R} {D}— {inProgressTasks} active, {completedTasks} done{R}"));
                        w.Add(left.Text(""));
                        w.Add(left.Text($"  {sec}{new string('━', 32)}{R}"));
                        w.Add(left.Text($"  {hBg}{B}{acc}📅 Recent{R}"));
                        foreach (var l in logEntries.Take(3))
                            w.Add(left.Text($"  {D}{l.Date}{R}  {l.Topic}"));
                        return w.ToArray();
                    }).FillWidth(2).FillHeight(),

                    h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text($"  {hBg}{B}{acc}📋 Decisions{R}"),
                            right.Text($"  {sec}{new string('━', 24)}{R}"),
                        };
                        foreach (var d in decisions.Take(4))
                            w.Add(right.Text($"  {D}{d.Date}{R}  {d.Title}"));
                        w.Add(right.Text(""));
                        w.Add(right.Text($"  {sec}{new string('━', 24)}{R}"));
                        w.Add(right.Text($"  {hBg}{B}{acc}📈 Metrics{R}"));
                        w.Add(right.Text($"  {D}Velocity:{R}  {B}{completedTasks}{R} {D}tasks/sprint{R}"));
                        w.Add(right.Text($"  {D}Pending:{R}   {B}{pendingTasks}{R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight(),
                ]).Fill()
            ])),

            // Narrow layout: single column
            r.Otherwise(r => r.VStack(col =>
            {
                var w = new List<Hex1bWidget>
                {
                    col.Text($"  {hBg}{B}{acc}☀️  SquadTUI{R}"),
                    col.Text($"  {sec}{new string('━', 24)}{R}"),
                    col.Text($"  {D}👥 Members:{R} {B}{members.Count}{R}  {D}Tasks:{R} {B}{tasks.Count}{R}"),
                    col.Text(""),
                    col.Text($"  {hBg}{B}{acc}📅 Recent{R}"),
                };
                foreach (var l in logEntries.Take(2))
                    w.Add(col.Text($"  {D}{l.Date}{R}  {l.Topic}"));
                w.Add(col.Text(""));
                w.Add(col.Text($"  {hBg}{B}{acc}📋 Decisions{R}"));
                foreach (var d in decisions.Take(2))
                    w.Add(col.Text($"  {D}{d.Date}{R}  {d.Title}  {D}({d.Author}){R}"));
                return w.ToArray();
            })),
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
