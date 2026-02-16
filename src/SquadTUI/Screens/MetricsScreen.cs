using Hex1b;
using Hex1b.Charts;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class MetricsScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var tasks = SampleData.Tasks;
        var members = state.Members ?? SampleData.Members;

        var chartData = members.Select(m =>
        {
            var completed = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.Done);
            var inProgress = tasks.Count(t => t.Assignee == m.Name && t.Status == Models.SquadTaskStatus.InProgress);
            return new ChartItem(m.Name, completed + inProgress);
        }).Where(c => c.Value > 0).ToArray();

        return v.VStack(inner =>
        [
            inner.Border(b =>
            [
                b.BarChart(chartData).Fill()
            ]).Title("📈 Task Activity by Member").Fill(),

            inner.Border(b =>
            [
                b.Text($"  📊 Total Tasks:     {tasks.Count}"),
                b.Text($"  ✅ Completed:       {tasks.Count(t => t.Status == Models.SquadTaskStatus.Done)}"),
                b.Text($"  🔄 In Progress:     {tasks.Count(t => t.Status == Models.SquadTaskStatus.InProgress)}"),
                b.Text($"  ⏳ Pending:         {tasks.Count(t => t.Status == Models.SquadTaskStatus.Pending)}"),
                b.Text($"  👥 Team Members:    {members.Count}"),
                b.Text($"  🎯 Velocity:        {tasks.Count(t => t.Status == Models.SquadTaskStatus.Done)} tasks/sprint"),
            ]).Title("Summary"),
        ]).Fill();
    }
}
