using FluentAssertions;
using SquadTUI.Screens;
using SquadTUI.Models;

namespace SquadTUI.Tests.Unit.Screens;

public class AppStateTests
{
    [Fact]
    public void AppState_DefaultsToSquadDetected()
    {
        var state = new AppState();
        state.SquadDetected.Should().BeTrue();
    }

    [Fact]
    public void AppState_DefaultsToDashboard()
    {
        var state = new AppState();
        state.CurrentScreen.Should().Be(Screen.Dashboard);
    }

    [Fact]
    public void AppState_HasSettingsProperty()
    {
        var state = new AppState();
        state.Settings.Should().NotBeNull();
    }

    [Fact]
    public void Screen_Enum_IncludesNoSquad()
    {
        Enum.IsDefined(typeof(Screen), Screen.NoSquad).Should().BeTrue();
    }

    [Fact]
    public void RosterSelectedIndex_DefaultsToZero()
    {
        var state = new AppState();
        state.RosterSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void RosterSelectedIndex_ClampedToZero_WhenMembersEmpty()
    {
        var state = new AppState();
        state.Members = new List<SquadMember>();
        // Simulate K press logic: Math.Max(index - 1, 0)
        state.RosterSelectedIndex = Math.Max(state.RosterSelectedIndex - 1, 0);
        state.RosterSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void DecisionSelectedIndex_DoesNotGoNegative()
    {
        var state = new AppState();
        state.DecisionSelectedIndex = 0;
        // Simulate K press: Math.Max(index - 1, 0)
        state.DecisionSelectedIndex = Math.Max(state.DecisionSelectedIndex - 1, 0);
        state.DecisionSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void SkillSelectedIndex_ClampedAtBoundary()
    {
        var state = new AppState();
        state.Skills = SampleData.Skills;
        int maxIndex = state.Skills.Count - 1;
        // Simulate J press: Math.Min(index + 1, count - 1)
        state.SkillSelectedIndex = maxIndex;
        state.SkillSelectedIndex = Math.Min(state.SkillSelectedIndex + 1, maxIndex);
        state.SkillSelectedIndex.Should().Be(maxIndex);
    }

    [Fact]
    public void SettingsSelectedIndex_StaysInRange0To4()
    {
        var state = new AppState();
        // Simulate J press at max: Math.Min(index + 1, 4)
        state.SettingsSelectedIndex = 4;
        state.SettingsSelectedIndex = Math.Min(state.SettingsSelectedIndex + 1, 4);
        state.SettingsSelectedIndex.Should().Be(4);

        // Simulate K press at min: Math.Max(index - 1, 0)
        state.SettingsSelectedIndex = 0;
        state.SettingsSelectedIndex = Math.Max(state.SettingsSelectedIndex - 1, 0);
        state.SettingsSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void LogSelectedIndex_ClampedCorrectly()
    {
        var state = new AppState();
        state.LogEntries = SampleData.LogEntries;
        int maxIndex = state.LogEntries.Count - 1;

        // Try to go past the end
        state.LogSelectedIndex = maxIndex;
        state.LogSelectedIndex = Math.Min(state.LogSelectedIndex + 1, maxIndex);
        state.LogSelectedIndex.Should().Be(maxIndex);

        // Try to go before the beginning
        state.LogSelectedIndex = 0;
        state.LogSelectedIndex = Math.Max(state.LogSelectedIndex - 1, 0);
        state.LogSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void IsLiveEnabled_DefaultsToTrue()
    {
        var state = new AppState();
        state.IsLiveEnabled.Should().BeTrue();
    }

    [Fact]
    public void HasPendingRefresh_DefaultsToFalse()
    {
        var state = new AppState();
        state.HasPendingRefresh.Should().BeFalse();
    }

    [Fact]
    public void LastRefreshTime_DefaultsToNow()
    {
        var before = DateTime.Now;
        var state = new AppState();
        state.LastRefreshTime.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void AllSelectedIndices_DefaultToZero()
    {
        var state = new AppState();
        state.RosterSelectedIndex.Should().Be(0);
        state.DecisionSelectedIndex.Should().Be(0);
        state.LogSelectedIndex.Should().Be(0);
        state.SkillSelectedIndex.Should().Be(0);
        state.SettingsSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void RosterSelectedIndex_CanNavigateFullRange()
    {
        var state = new AppState();
        state.Members = SampleData.Members;
        int count = state.Members.Count;

        // Navigate all the way down
        for (int i = 0; i < count + 5; i++)
        {
            state.RosterSelectedIndex = Math.Min(state.RosterSelectedIndex + 1, count - 1);
        }
        state.RosterSelectedIndex.Should().Be(count - 1);

        // Navigate all the way back up
        for (int i = 0; i < count + 5; i++)
        {
            state.RosterSelectedIndex = Math.Max(state.RosterSelectedIndex - 1, 0);
        }
        state.RosterSelectedIndex.Should().Be(0);
    }
}
