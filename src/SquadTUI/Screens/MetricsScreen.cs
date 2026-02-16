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
        var ti = state.SelectedThemeIndex;
        var c = ThemeManager.GetPanelColors(ti);
        var p1 = c.PanelBg;
        var p2 = c.NestedBg;
        var acc = c.Accent;
        var R = PanelRenderer.Reset;

        var chartData = members.Select(m =>
        {
            var completed = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
            var inProgress = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
            return new ChartItem(m.Name, completed + inProgress);
        }).Where(c => c.Value > 0).ToArray();

        return v.VStack(inner =>
        [
            inner.VStack(chartSection =>
            [
                chartSection.Text($"{p1}  {PanelRenderer.Bold}{acc}📈 Task Activity by Member{R}"),
                chartSection.BarChart(chartData).Fill()
            ]).Fill(),

            inner.VStack(summarySection =>
            [
                summarySection.Text($"{p2}  {PanelRenderer.Bold}{acc}Summary{R}"),
                summarySection.Text($"{p2}  \x1b[90m📊 Total Tasks:\x1b[0m     \x1b[1m{tasks.Count}\x1b[0m{R}"),
                summarySection.Text($"{p2}  \x1b[90m✅ Completed:\x1b[0m       \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Done)}\x1b[0m{R}"),
                summarySection.Text($"{p2}  \x1b[90m🔄 In Progress:\x1b[0m     \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress)}\x1b[0m{R}"),
                summarySection.Text($"{p2}  \x1b[90m⏳ Pending:\x1b[0m         \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending)}\x1b[0m{R}"),
                summarySection.Text($"{p2}  \x1b[90m👥 Team Members:\x1b[0m    \x1b[1m{members.Count}\x1b[0m{R}"),
                summarySection.Text($"{p2}  \x1b[90m🎯 Velocity:\x1b[0m        \x1b[1m{tasks.Count(t => t.Status == Models.SquadTaskStatus.Done)}\x1b[0m \x1b[90mtasks/sprint\x1b[0m{R}"),
            ]),
        ]).Fill();
    }
}
