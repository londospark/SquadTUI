using FluentAssertions;
using SquadTUI.Screens;
using SquadTUI.Tests.Fixtures;

namespace SquadTUI.Tests.Unit.Screens;

public class MetricsChartDataTests
{
    [Fact]
    public void ShowBurndown_DefaultsToFalse()
    {
        var state = new AppState();
        state.ShowBurndown.Should().BeFalse();
    }

    [Fact]
    public void ShowBurndown_CanBeToggled()
    {
        var state = new AppState();
        state.ShowBurndown = true;
        state.ShowBurndown.Should().BeTrue();
        state.ShowBurndown = false;
        state.ShowBurndown.Should().BeFalse();
    }

    [Fact]
    public void SprintHistory_HasData()
    {
        SampleData.SprintHistory.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void SprintHistory_HasVelocityData()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            sprint.PlannedTasks.Should().BeGreaterThanOrEqualTo(0);
            sprint.CompletedTasks.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public void SprintHistory_HasContributions()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            sprint.Contributions.Should().HaveCountGreaterThanOrEqualTo(1);
        }
    }

    [Fact]
    public void OverallCompletionRate_InValidRange()
    {
        SampleData.OverallCompletionRate.Should().BeGreaterThanOrEqualTo(0);
        SampleData.OverallCompletionRate.Should().BeLessThanOrEqualTo(100);
    }

    [Fact]
    public void AverageVelocity_IsPositive()
    {
        SampleData.AverageVelocity.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void SprintCompletionRate_InValidRange()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            sprint.CompletionRate.Should().BeGreaterThanOrEqualTo(0);
            sprint.CompletionRate.Should().BeLessThanOrEqualTo(100);
        }
    }

    [Fact]
    public void CarriedOver_IsNonNegative()
    {
        foreach (var sprint in SampleData.SprintHistory)
        {
            sprint.CarriedOver.Should().BeGreaterThanOrEqualTo(0);
        }
    }
}
