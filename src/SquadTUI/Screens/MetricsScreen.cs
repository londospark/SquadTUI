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
        var sprints = SampleData.SprintHistory;
        var tasks = state.Tasks ?? SampleData.Tasks;
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

        // Task status counts
        var done = tasks.Count(t => t.Status == Models.SquadTaskStatus.Done);
        var active = tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress);
        var pending = tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending);
        var blocked = tasks.Count(t => t.Status == Models.SquadTaskStatus.Blocked);

        // Sprint chart data: velocity or burndown
        var velocityData = sprints.Select(s => new ChartItem(s.SprintName.Split(' ')[0], s.CompletedTasks)).ToArray();
        var burndownData = sprints.Select(s => new ChartItem(s.SprintName.Split(' ')[0], s.CarriedOver)).ToArray();
        var chartData = state.ShowBurndown ? burndownData : velocityData;

        // Task status breakdown
        var statusData = new ChartItem[]
        {
            new("Done", done), new("Active", active), new("Pending", pending), new("Blocked", blocked)
        };

        // Header labels
        var modeLabel = state.ShowBurndown ? "Burndown View" : "Velocity View";
        var chartTitle = state.ShowBurndown ? "Remaining Work Trend" : "Tasks Completed per Sprint";
        var chartSubtitle = state.ShowBurndown ? "Remaining tasks carried over per sprint" : "Completed tasks per sprint cycle";
        var trendArrow = SampleData.VelocityTrend > 0 ? "\x1b[32m▲" : SampleData.VelocityTrend < 0 ? "\x1b[31m▼" : "\x1b[33m─";

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): 3-column
            r.WhenMinWidth(120, r => r.VStack(outer =>
            [
                outer.Text($"  {hBg}{B}{acc}📈 Sprint Metrics — {modeLabel}{R}  {D}(press V to toggle){R}"),
                outer.Text($"  {D}{sec}Performance overview across {sprints.Count} sprint cycles{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    // Left: Sprint overview stats + task status breakdown
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    [
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Sprint Overview{R}"),
                        left.Text(""),
                        left.Text($"  {D}Completion Rate:{R}  {B}{SampleData.OverallCompletionRate}%{R}"),
                        left.Text($"  {D}Avg Velocity:{R}    {B}{SampleData.AverageVelocity}{R} {D}tasks/sprint{R}"),
                        left.Text($"  {D}Trend:{R}           {trendArrow} {B}{Math.Abs(SampleData.VelocityTrend)}{R}{D} tasks{R}"),
                        left.Text(""),
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Task Status{R}"),
                        left.Text(""),
                        left.BreakdownChart(statusData)
                            .ShowPercentages(true)
                            .Fill(),
                    ]).FillWidth(1).FillHeight()),

                    // Center: Velocity or Burndown chart
                    new BackgroundPanelWidget(detailBg, h.VStack(mid =>
                    [
                        mid.Text($"  {B}{acc}▌{R} {B}{acc}📊 {chartTitle}{R}"),
                        mid.Text($"  {D}{chartSubtitle}{R}"),
                        mid.Text(""),
                        mid.BarChart(chartData).Fill(),
                    ]).FillWidth(2).FillHeight()),

                    // Right: Per-member contributions across sprints
                    new BackgroundPanelWidget(altBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text($"  {B}{acc}▌{R} {B}{acc}👥 Member Contributions{R}"),
                            right.Text(""),
                        };
                        foreach (var s in sprints)
                        {
                            w.Add(right.Text($"  {B}{s.SprintName}{R}  {D}({s.CompletionRate}% done){R}"));
                            foreach (var c in s.Contributions.Where(c => c.TasksCompleted > 0))
                            {
                                w.Add(right.Text($"    {D}{c.MemberName}: ✅ {c.TasksCompleted}/{c.TasksAssigned}{R}"));
                            }
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
                outer.Text($"  {hBg}{B}{acc}📈 Sprint Metrics — {modeLabel}{R}  {D}(press V to toggle){R}"),
                outer.Text($"  {D}{sec}Performance overview across {sprints.Count} sprint cycles{R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    // Left: Sprint stats + breakdown
                    new BackgroundPanelWidget(panelBg, h.VStack(left =>
                    [
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Sprint Overview{R}"),
                        left.Text(""),
                        left.Text($"  {D}Completion Rate:{R}  {B}{SampleData.OverallCompletionRate}%{R}"),
                        left.Text($"  {D}Avg Velocity:{R}    {B}{SampleData.AverageVelocity}{R} {D}tasks/sprint{R}"),
                        left.Text($"  {D}Trend:{R}           {trendArrow} {B}{Math.Abs(SampleData.VelocityTrend)}{R}{D} tasks{R}"),
                        left.Text(""),
                        left.Text($"  {B}{acc}▌{R} {B}{acc}Task Status{R}"),
                        left.Text(""),
                        left.BreakdownChart(statusData)
                            .ShowPercentages(true)
                            .Fill(),
                    ]).FillWidth(1).FillHeight()),

                    // Right: Velocity/Burndown chart
                    new BackgroundPanelWidget(detailBg, h.VStack(right =>
                    [
                        right.Text($"  {B}{acc}▌{R} {B}{acc}📊 {chartTitle}{R}"),
                        right.Text($"  {D}{chartSubtitle}{R}"),
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
                    col.Text($"  {D}{modeLabel} (V to toggle){R}"),
                    col.Text(""),
                    col.Text($"  {D}Completion:{R} {B}{SampleData.OverallCompletionRate}%{R}  {D}Velocity:{R} {B}{SampleData.AverageVelocity}{R}"),
                    col.Text($"  {D}Trend:{R} {trendArrow} {B}{Math.Abs(SampleData.VelocityTrend)}{R}{D} tasks{R}"),
                    col.Text(""),
                    col.Text($"  \x1b[32m✅ {done}{R}  \x1b[33m🔄 {active}{R}  {D}⏳ {pending}{R}  \x1b[31m🚫 {blocked}{R}"),
                    col.Text(""),
                    col.BreakdownChart(statusData)
                        .ShowPercentages(true)
                        .Fill(),
                };
                return w.ToArray();
            }))),
        ]).Fill();
    }
}
