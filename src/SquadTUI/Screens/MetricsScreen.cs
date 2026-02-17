using Hex1b;
using Hex1b.Charts;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class MetricsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var tasks = SampleData.Tasks;
        var members = state.Members ?? SampleData.Members;
        var acc = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
        var sec = ThemeManager.GetSecondaryAccent(state.SelectedThemeIndex);
        var R = PanelRenderer.Reset;
        var B = PanelRenderer.Bold;
        var D = PanelRenderer.Dim;
        var hBg = ThemeManager.GetPanelHeaderBg(state.SelectedThemeIndex);
        var panelBg = ThemeManager.GetPanelBgColor(state.SelectedThemeIndex);
        var detailBg = ThemeManager.GetPanelDetailBgColor(state.SelectedThemeIndex);
        var altBg = ThemeManager.GetPanelAltBgColor(state.SelectedThemeIndex);

        var done = tasks.Count(t => t.Status == Models.SquadTaskStatus.Done);
        var active = tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress);
        var pending = tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending);
        var blocked = tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked);
        var total = tasks.Count > 0 ? tasks.Count : 1;
        var pct = done * 100 / total;

        // Progress bar
        var barWidth = 30;
        var doneWidth = (int)(done * (double)barWidth / total);
        var activeWidth = (int)(active * (double)barWidth / total);
        var remaining = barWidth - doneWidth - activeWidth;
        if (remaining < 0) remaining = 0;
        var progressBar = $"\x1b[32m{new string('█', doneWidth)}\x1b[33m{new string('▓', activeWidth)}{D}{new string('░', remaining)}{R}";

        var chartData = members.Select(m =>
        {
            var completed = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
            var inProgress = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
            return new ChartItem(m.Name, completed + inProgress);
        }).Where(c => c.Value > 0).ToArray();

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): 3-column
            r.WhenMinWidth(120, r => r.VStack(outer =>
            [
                outer.Text($"  {hBg}{B}{acc}📈 Sprint Metrics{R}"),
                outer.Text($"  {D}{sec}Performance overview for the current sprint cycle{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    // Left: Completion overview
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    [
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Sprint Progress{R}"),
                        left.Text(""),
                        left.Text($"  {progressBar}  {B}{pct}%{R}"),
                        left.Text($"  {D}Completion: {done} of {tasks.Count} tasks done{R}"),
                        left.Text(""),
                        left.Text(""),
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Task Breakdown{R}"),
                        left.Text(""),
                        left.Text($"  {D}Total Tasks:{R}      {B}{tasks.Count}{R}"),
                        left.Text($"  \x1b[32m✅ Completed:{R}     {B}{done}{R}"),
                        left.Text($"  \x1b[33m🔄 In Progress:{R}   {B}{active}{R}"),
                        left.Text($"  {D}⏳ Pending:{R}       {B}{pending}{R}"),
                        left.Text($"  \x1b[31m🚫 Blocked:{R}       {B}{blocked}{R}"),
                        left.Text(""),
                        left.Text(""),
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Velocity{R}"),
                        left.Text(""),
                        left.Text($"  {B}{done}{R} {D}tasks completed per sprint cycle{R}"),
                        left.Text($"  {D}Measures how many tasks the team finishes{R}"),
                        left.Text($"  {D}in each sprint iteration.{R}"),
                    ]).FillWidth(1).FillHeight()),

                    // Center: Chart
                    new BackgroundPanelWidget(detailBg, h.VStack(mid =>
                    [
                        mid.Text($"  {B}{acc}▌{R} {B}{acc}📊 Tasks by Member{R}"),
                        mid.Text($"  {D}Completed + in-progress tasks per team member{R}"),
                        mid.Text(""),
                        mid.BarChart(chartData).Fill(),
                    ]).FillWidth(2).FillHeight()),

                    // Right: Per-member detail
                    new BackgroundPanelWidget(altBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text($"  {B}{acc}▌{R} {B}{acc}👥 Member Status{R}"),
                            right.Text(""),
                        };
                        foreach (var m in members)
                        {
                            var mDone = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
                            var mActive = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
                            var mPending = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Pending);
                            w.Add(right.Text($"  {B}{m.Name}{R}  {D}{m.Role}{R}"));
                            w.Add(right.Text($"    {D}✅ {mDone}  🔄 {mActive}  ⏳ {mPending}{R}"));
                            w.Add(right.Text(""));
                        }
                        w.Add(right.Text($"  {sec}{new string('━', 28)}{R}"));
                        w.Add(right.Text($"  {D}Team size:{R}  {B}{members.Count}{R} {D}members{R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),
                ]).Fill(),
            ])),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.VStack(outer =>
            [
                outer.Text($"  {hBg}{B}{acc}📈 Sprint Metrics{R}"),
                outer.Text($"  {D}{sec}Performance overview for the current sprint cycle{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            left.Text($"  {B}{acc}▌{R} {B}{acc}Sprint Progress{R}"),
                            left.Text(""),
                            left.Text($"  {progressBar}  {B}{pct}%{R}"),
                            left.Text($"  {D}Completion: {done} of {tasks.Count} tasks done{R}"),
                            left.Text(""),
                            left.Text(""),
                            left.Text($"  {B}{acc}▌{R} {B}{acc}Task Breakdown{R}"),
                            left.Text(""),
                            left.Text($"  \x1b[32m✅ {done} done{R}   \x1b[33m🔄 {active} active{R}   {D}⏳ {pending} pending{R}   \x1b[31m🚫 {blocked} blocked{R}"),
                            left.Text(""),
                            left.Text(""),
                            left.Text($"  {B}{acc}▌{R} {B}{acc}Velocity{R}"),
                            left.Text(""),
                            left.Text($"  {B}{done}{R} {D}tasks completed per sprint cycle{R}"),
                            left.Text(""),
                        };
                        foreach (var m in members)
                        {
                            var mDone = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
                            var mActive = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
                            w.Add(left.Text($"  {D}{m.Name}: ✅ {mDone}  🔄 {mActive}{R}"));
                        }
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),

                    new BackgroundPanelWidget(detailBg, h.VStack(right =>
                    [
                        right.Text($"  {B}{acc}▌{R} {B}{acc}📊 Tasks by Member{R}"),
                        right.Text($"  {D}Completed + active tasks per member{R}"),
                        right.Text(""),
                        right.BarChart(chartData).Fill(),
                    ]).FillWidth(1).FillHeight()),
                ]).Fill(),
            ])),

            // Narrow layout: single column
            r.Otherwise(r => new BackgroundPanelWidget(panelBg, r.VStack(col =>
            {
                var w = new List<Hex1bWidget>
                {
                    col.Text($"  {hBg}{B}{acc}📈 Sprint Metrics{R}"),
                    col.Text(""),
                    col.Text($"  {progressBar}  {B}{pct}%{R}"),
                    col.Text($"  {D}{done}/{tasks.Count} done{R}"),
                    col.Text(""),
                    col.Text($"  \x1b[32m✅ {done}{R}  \x1b[33m🔄 {active}{R}  {D}⏳ {pending}{R}  \x1b[31m🚫 {blocked}{R}"),
                    col.Text(""),
                    col.Text($"  {D}Velocity:{R} {B}{done}{R} {D}tasks/sprint{R}"),
                    col.Text(""),
                    col.Text($"  {hBg}{B}{acc}📊 Tasks by Member{R}"),
                };
                foreach (var m in members)
                {
                    var mDone = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
                    var mActive = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
                    w.Add(col.Text($"  {D}{m.Name}: ✅ {mDone}  🔄 {mActive}{R}"));
                }
                return w.ToArray();
            }))),
        ]).Fill();
    }
}
