using Hex1b;
using Hex1b.Charts;
using Hex1b.Widgets;
using SquadTUI.Models;
using SquadTUI.Rendering;
using static SquadTUI.Rendering.IconHelper;

namespace SquadTUI.Screens;

public static class MetricsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        // Handle pending refresh from file watcher
        if (state.HasPendingRefresh)
        {
            state.HasPendingRefresh = false;
            state.LastRefreshTime = DateTime.Now;
        }

        var sprints = state.SprintHistory.GetOrEmpty();
        var tasks = state.Tasks.GetOrEmpty();
        var members = state.Members.GetOrEmpty();
        if (sprints.Count == 0 && tasks.Count == 0)
            return ScreenHelper.EmptyState(v, "No sprint data available. Ensure your .squad/ directory contains task and log data.");

        var t = new ThemeContext(state.SelectedThemeIndex, state.Settings.ShowEmoji);

        // Task status counts
        var done = tasks.Count(tk => tk.Status == SquadTaskStatus.Done);
        var active = tasks.Count(tk => tk.Status == SquadTaskStatus.InProgress);
        var pending = tasks.Count(tk => tk.Status == SquadTaskStatus.Pending);
        var blocked = tasks.Count(tk => tk.Status == SquadTaskStatus.Blocked);

        // Sprint chart data
        var velocityData = sprints.Select(s => new ChartItem(s.SprintName.Split(' ')[0], s.CompletedTasks)).ToArray();
        var burndownData = sprints.Select(s => new ChartItem(s.SprintName.Split(' ')[0], s.CarriedOver)).ToArray();
        var chartData = state.ShowBurndown ? burndownData : velocityData;

        // Compute metrics
        var totalPlanned = sprints.Sum(s => s.PlannedTasks);
        var totalDone = sprints.Sum(s => s.CompletedTasks);
        var overallCompletionRate = totalPlanned > 0 ? Math.Round((double)totalDone / totalPlanned * 100, 1) : 0;
        var averageVelocity = sprints.Count > 0 ? Math.Round(sprints.Average(s => (double)s.Velocity), 1) : 0;
        var velocityTrend = sprints.Count >= 2 ? sprints[^1].Velocity - sprints[^2].Velocity : 0;

        var statusData = new ChartItem[]
        {
            new("Done", done), new("Active", active), new("Pending", pending), new("Blocked", blocked)
        };

        var modeLabel = state.ShowBurndown ? "Burndown View" : "Velocity View";
        var chartTitle = state.ShowBurndown ? "Remaining Work Trend" : "Tasks Completed per Sprint";
        var chartSubtitle = state.ShowBurndown ? "Remaining tasks carried over per sprint" : "Completed tasks per sprint cycle";
        var trendArrow = velocityTrend > 0 ? "\x1b[32m▲" : velocityTrend < 0 ? "\x1b[31m▼" : "\x1b[33m─";

        return v.Responsive(r =>
        [
            // Wide layout (≥120 cols): 3-column
            r.WhenMinWidth(120, r => r.VStack(outer =>
            [
                outer.Text($"  {t.HBg}{t.B}{t.Acc}{Icon("📈", "▪", t.Em)} Sprint Metrics — {modeLabel}{t.R}  {t.D}(press V to toggle){t.R}"),
                outer.Text($"  {t.D}{t.Sec}Performance overview across {sprints.Count} sprint cycles{t.R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(t.PanelBg, h.VStack(left =>
                    [
                        left.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}Sprint Overview{t.R}"),
                        left.Text(""),
                        left.Text($"  {t.D}Completion Rate:{t.R}  {t.B}{overallCompletionRate}%{t.R}"),
                        left.Text($"  {t.D}Avg Velocity:{t.R}    {t.B}{averageVelocity}{t.R} {t.D}tasks/sprint{t.R}"),
                        left.Text($"  {t.D}Trend:{t.R}           {trendArrow} {t.B}{Math.Abs(velocityTrend)}{t.R}{t.D} tasks{t.R}"),
                        left.Text(""),
                        left.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}Task Status{t.R}"),
                        left.Text(""),
                        left.BreakdownChart(statusData)
                            .ShowPercentages(true)
                            .Fill(),
                    ]).FillWidth(1).FillHeight()),

                    new BackgroundPanelWidget(t.DetailBg, h.VStack(mid =>
                    [
                        mid.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}{Icon("📊", "▪", t.Em)} {chartTitle}{t.R}"),
                        mid.Text($"  {t.D}{chartSubtitle}{t.R}"),
                        mid.Text(""),
                        mid.BarChart(chartData).Fill(),
                    ]).FillWidth(2).FillHeight()),

                    new BackgroundPanelWidget(t.AltBg, h.VStack(right =>
                    {
                        var w = new List<Hex1bWidget>
                        {
                            right.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}{Icon("👥", "◆", t.Em)} Member Contributions{t.R}"),
                            right.Text(""),
                        };
                        foreach (var s in sprints)
                        {
                            w.Add(right.Text($"  {t.B}{s.SprintName}{t.R}  {t.D}({s.CompletionRate}% done){t.R}"));
                            foreach (var c in s.Contributions.Where(c => c.TasksCompleted > 0))
                                w.Add(right.Text($"    {t.D}{c.MemberName}: {Icon("✅", "+", t.Em)} {c.TasksCompleted}/{c.TasksAssigned}{t.R}"));
                            w.Add(right.Text(""));
                        }
                        w.Add(right.Text(t.Separator(28)));
                        w.Add(right.Text($"  {t.D}Team size:{t.R}  {t.B}{members.Count}{t.R} {t.D}members{t.R}"));
                        return w.ToArray();
                    }).FillWidth(1).FillHeight()),
                ]).Fill(),
            ]).Fill()),

            // Medium layout (≥80 cols): 2-column
            r.WhenMinWidth(80, r => r.VStack(outer =>
            [
                outer.Text($"  {t.HBg}{t.B}{t.Acc}{Icon("📈", "▪", t.Em)} Sprint Metrics — {modeLabel}{t.R}  {t.D}(press V to toggle){t.R}"),
                outer.Text($"  {t.D}{t.Sec}Performance overview across {sprints.Count} sprint cycles{t.R}"),
                outer.Text(""),

                outer.HStack(h =>
                [
                    new BackgroundPanelWidget(t.PanelBg, h.VStack(left =>
                    [
                        left.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}Sprint Overview{t.R}"),
                        left.Text(""),
                        left.Text($"  {t.D}Completion Rate:{t.R}  {t.B}{overallCompletionRate}%{t.R}"),
                        left.Text($"  {t.D}Avg Velocity:{t.R}    {t.B}{averageVelocity}{t.R} {t.D}tasks/sprint{t.R}"),
                        left.Text($"  {t.D}Trend:{t.R}           {trendArrow} {t.B}{Math.Abs(velocityTrend)}{t.R}{t.D} tasks{t.R}"),
                        left.Text(""),
                        left.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}Task Status{t.R}"),
                        left.Text(""),
                        left.BreakdownChart(statusData)
                            .ShowPercentages(true)
                            .Fill(),
                    ]).FillWidth(1).FillHeight()),

                    new BackgroundPanelWidget(t.DetailBg, h.VStack(right =>
                    [
                        right.Text($"  {t.B}{t.Acc}▌{t.R} {t.B}{t.Acc}{Icon("📊", "▪", t.Em)} {chartTitle}{t.R}"),
                        right.Text($"  {t.D}{chartSubtitle}{t.R}"),
                        right.Text(""),
                        right.BarChart(chartData).Fill(),
                    ]).FillWidth(1).FillHeight()),
                ]).Fill(),
            ]).Fill()),

            // Narrow layout: single column
            r.Otherwise(r => new BackgroundPanelWidget(t.PanelBg, r.VStack(col =>
            {
                var w = new List<Hex1bWidget>
                {
                    col.Text($"  {t.HBg}{t.B}{t.Acc}{Icon("📈", "▪", t.Em)} Sprint Metrics{t.R}"),
                    col.Text($"  {t.D}{modeLabel} (V to toggle){t.R}"),
                    col.Text(""),
                    col.Text($"  {t.D}Completion:{t.R} {t.B}{overallCompletionRate}%{t.R}  {t.D}Velocity:{t.R} {t.B}{averageVelocity}{t.R}"),
                    col.Text($"  {t.D}Trend:{t.R} {trendArrow} {t.B}{Math.Abs(velocityTrend)}{t.R}{t.D} tasks{t.R}"),
                    col.Text(""),
                    col.Text($"  \x1b[32m{Icon("✅", "+", t.Em)} {done}{t.R}  \x1b[33m{Icon("🔄", ">", t.Em)} {active}{t.R}  {t.D}{Icon("⏳", "~", t.Em)} {pending}{t.R}  \x1b[31m{Icon("🚫", "-", t.Em)} {blocked}{t.R}"),
                    col.Text(""),
                    col.BreakdownChart(statusData)
                        .ShowPercentages(true)
                        .Fill(),
                };
                return w.ToArray();
            }).Fill())),
        ]).Fill().RedrawAfter(3000);
    }
}