using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Screens;
using SquadTUI.Models;
using SquadTUI.Tests.Fixtures;

namespace SquadTUI.Tests.Unit.Screens;

public class AppStateTests
{
    [Fact]
    public void AppState_DefaultsToSquadDetected()
    {
        var state = new AppState();
        Assert.True(state.SquadDetected);
    }

    [Fact]
    public void AppState_DefaultsToDashboard()
    {
        var state = new AppState();
        Assert.Equal(Screen.Dashboard, state.CurrentScreen);
    }

    [Fact]
    public void AppState_HasSettingsProperty()
    {
        var state = new AppState();
        Assert.NotNull(state.Settings);
    }

    [Fact]
    public void Screen_Enum_IncludesNoSquad()
    {
        Assert.True(Enum.IsDefined(typeof(Screen), Screen.NoSquad));
    }

    [Fact]
    public void RosterSelectedIndex_DefaultsToZero()
    {
        var state = new AppState();
        Assert.Equal(0, state.RosterSelectedIndex);
    }

    [Fact]
    public void RosterSelectedIndex_ClampedToZero_WhenMembersEmpty()
    {
        var state = new AppState();
        state.Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>());
        // Simulate K press logic: Math.Max(index - 1, 0)
        state.RosterSelectedIndex = Math.Max(state.RosterSelectedIndex - 1, 0);
        Assert.Equal(0, state.RosterSelectedIndex);
    }

    [Fact]
    public void DecisionSelectedIndex_DoesNotGoNegative()
    {
        var state = new AppState();
        state.DecisionSelectedIndex = 0;
        // Simulate K press: Math.Max(index - 1, 0)
        state.DecisionSelectedIndex = Math.Max(state.DecisionSelectedIndex - 1, 0);
        Assert.Equal(0, state.DecisionSelectedIndex);
    }

    [Fact]
    public void SkillSelectedIndex_ClampedAtBoundary()
    {
        var state = new AppState();
        state.Skills = Right<AppError, IReadOnlyList<Skill>>(SampleData.Skills);
        int maxIndex = state.Skills.GetOrEmpty().Count - 1;
        // Simulate J press: Math.Min(index + 1, count - 1)
        state.SkillSelectedIndex = maxIndex;
        state.SkillSelectedIndex = Math.Min(state.SkillSelectedIndex + 1, maxIndex);
        Assert.Equal(maxIndex, state.SkillSelectedIndex);
    }

    [Fact]
    public void SettingsSelectedIndex_StaysInRange0To4()
    {
        var state = new AppState();
        // Simulate J press at max: Math.Min(index + 1, 4)
        state.SettingsSelectedIndex = 4;
        state.SettingsSelectedIndex = Math.Min(state.SettingsSelectedIndex + 1, 4);
        Assert.Equal(4, state.SettingsSelectedIndex);

        // Simulate K press at min: Math.Max(index - 1, 0)
        state.SettingsSelectedIndex = 0;
        state.SettingsSelectedIndex = Math.Max(state.SettingsSelectedIndex - 1, 0);
        Assert.Equal(0, state.SettingsSelectedIndex);
    }

    [Fact]
    public void LogSelectedIndex_ClampedCorrectly()
    {
        var state = new AppState();
        state.LogEntries = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(SampleData.LogEntries);
        int maxIndex = state.LogEntries.GetOrEmpty().Count - 1;

        // Try to go past the end
        state.LogSelectedIndex = maxIndex;
        state.LogSelectedIndex = Math.Min(state.LogSelectedIndex + 1, maxIndex);
        Assert.Equal(maxIndex, state.LogSelectedIndex);

        // Try to go before the beginning
        state.LogSelectedIndex = 0;
        state.LogSelectedIndex = Math.Max(state.LogSelectedIndex - 1, 0);
        Assert.Equal(0, state.LogSelectedIndex);
    }

    [Fact]
    public void IsLiveEnabled_DefaultsToTrue()
    {
        var state = new AppState();
        Assert.True(state.IsLiveEnabled);
    }

    [Fact]
    public void HasPendingRefresh_DefaultsToFalse()
    {
        var state = new AppState();
        Assert.False(state.HasPendingRefresh);
    }

    [Fact]
    public void LastRefreshTime_DefaultsToNow()
    {
        var before = DateTime.Now;
        var state = new AppState();
        Assert.True(state.LastRefreshTime >= before);
    }

    [Fact]
    public void ShowSettingsOverlay_DefaultsToFalse()
    {
        var state = new AppState();
        Assert.False(state.ShowSettingsOverlay);
    }

    [Fact]
    public void PreviewThemeIndex_DefaultsToNegativeOne()
    {
        var state = new AppState();
        Assert.Equal(-1, state.PreviewThemeIndex);
    }

    [Fact]
    public void OriginalThemeIndex_DefaultsToZero()
    {
        var state = new AppState();
        Assert.Equal(0, state.OriginalThemeIndex);
    }

    [Fact]
    public void AllSelectedIndices_DefaultToZero()
    {
        var state = new AppState();
        Assert.Equal(0, state.RosterSelectedIndex);
        Assert.Equal(0, state.DecisionSelectedIndex);
        Assert.Equal(0, state.LogSelectedIndex);
        Assert.Equal(0, state.SkillSelectedIndex);
        Assert.Equal(0, state.SettingsSelectedIndex);
    }

    [Fact]
    public void RosterSelectedIndex_CanNavigateFullRange()
    {
        var state = new AppState();
        state.Members = Right<AppError, IReadOnlyList<SquadMember>>(SampleData.Members);
        int count = state.Members.GetOrEmpty().Count;

        // Navigate all the way down
        for (int i = 0; i < count + 5; i++)
        {
            state.RosterSelectedIndex = Math.Min(state.RosterSelectedIndex + 1, count - 1);
        }
        Assert.Equal(count - 1, state.RosterSelectedIndex);

        // Navigate all the way back up
        for (int i = 0; i < count + 5; i++)
        {
            state.RosterSelectedIndex = Math.Max(state.RosterSelectedIndex - 1, 0);
        }
        Assert.Equal(0, state.RosterSelectedIndex);
    }
}
