using SquadTUI.Screens;
using SquadTUI.Tests.Fixtures;

namespace SquadTUI.Tests.Unit.Screens;

public class MetricsChartDataTests
{
    [Fact]
    public void ShowBurndown_DefaultsToFalse()
    {
        var state = new AppState();
        Assert.False(state.ShowBurndown);
    }

    [Fact]
    public void ShowBurndown_CanBeToggled()
    {
        var state = new AppState();
        state.ShowBurndown = true;
        Assert.True(state.ShowBurndown);
        state.ShowBurndown = false;
        Assert.False(state.ShowBurndown);
    }

    [Fact]
    public void SprintHistory_HasData()
    {
        Assert.True(SampleData.SprintHistory.Count() >= 1);
    }

    [Fact]
    public void SprintHistory_HasVelocityData()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            Assert.True(sprint.PlannedTasks >= 0);
            Assert.True(sprint.CompletedTasks >= 0);
        }
    }

    [Fact]
    public void SprintHistory_HasContributions()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            Assert.True(sprint.Contributions.Count() >= 1);
        }
    }

    [Fact]
    public void OverallCompletionRate_InValidRange()
    {
        Assert.True(SampleData.OverallCompletionRate >= 0);
        Assert.True(SampleData.OverallCompletionRate <= 100);
    }

    [Fact]
    public void AverageVelocity_IsPositive()
    {
        Assert.True(SampleData.AverageVelocity >= 0);
    }

    [Fact]
    public void SprintCompletionRate_InValidRange()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            Assert.True(sprint.CompletionRate >= 0);
            Assert.True(sprint.CompletionRate <= 100);
        }
    }

    [Fact]
    public void CarriedOver_IsNonNegative()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            Assert.True(sprint.CarriedOver >= 0);
        }
    }
}
