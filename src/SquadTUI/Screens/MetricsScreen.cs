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

        var done = tasks.Count(t => t.Status == Models.SquadTaskStatus.Done);
        var active = tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress);
        var pending = tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending);

        var chartData = members.Select(m =>
        {
            var completed = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
            var inProgress = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
            return new ChartItem(m.Name, completed + inProgress);
        }).Where(c => c.Value > 0).ToArray();

        return v.VStack(inner =>
        [
            inner.Text($"  {B}{acc}📈 Sprint Metrics{R}"),
            inner.Text($"  {D}{sec}{new string('━', 44)}{R}"),
            inner.Text(""),

            inner.VStack(chartSection =>
            [
                chartSection.Text($"  {B}{acc}📊 Task Activity by Member{R}"),
                chartSection.BarChart(chartData).Fill()
            ]).Fill(),

            inner.Text($"  {D}{sec}{new string('━', 44)}{R}"),

            inner.VStack(summarySection =>
            [
                summarySection.Text($"  {B}{acc}📋 Summary{R}"),
                summarySection.Text($"  {D}Total Tasks:{R}     {B}{tasks.Count}{R}"),
                summarySection.Text($"  {D}Completed:{R}       \x1b[32m{B}{done}{R}"),
                summarySection.Text($"  {D}In Progress:{R}     \x1b[33m{B}{active}{R}"),
                summarySection.Text($"  {D}Pending:{R}         {B}{pending}{R}"),
                summarySection.Text($"  {D}Team Members:{R}    {B}{members.Count}{R}"),
                summarySection.Text($"  {D}Velocity:{R}        {B}{done}{R} {D}tasks/sprint{R}"),
            ]),
        ]).Fill();
    }
}
