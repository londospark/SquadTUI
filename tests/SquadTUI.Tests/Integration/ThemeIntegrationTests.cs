using FluentAssertions;
using SquadTUI.Screens;
using SquadTUI.Themes;
using SquadTUI.Tests.Fixtures;

namespace SquadTUI.Tests.Integration;

public class ThemeIntegrationTests
{
    [Fact]
    public void AppState_DefaultThemeIndex_IsZero()
    {
        var state = new AppState();
        state.SelectedThemeIndex.Should().Be(0);
    }

    [Fact]
    public void ThemeCycling_WrapsAround()
    {
        var state = new AppState();
        for (int i = 0; i < ThemeManager.ThemeNames.Length + 1; i++)
        {
            state.SelectedThemeIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
        }
        state.SelectedThemeIndex.Should().Be(1);
    }

    [Fact]
    public void AllThemes_CanBeCreated()
    {
        for (int i = 0; i < ThemeManager.ThemeNames.Length; i++)
        {
            var theme = ThemeManager.GetTheme(i);
            theme.Should().NotBeNull($"theme at index {i} ({ThemeManager.ThemeNames[i]}) should be creatable");
        }
    }

    [Fact]
    public void SampleData_IsAvailable()
    {
        SampleData.Members.Should().NotBeEmpty();
        SampleData.Tasks.Should().NotBeEmpty();
        SampleData.Decisions.Should().NotBeEmpty();
        SampleData.Skills.Should().NotBeEmpty();
        SampleData.LogEntries.Should().NotBeEmpty();
    }

    [Fact]
    public void AppState_ScreenNavigation_Works()
    {
        var state = new AppState();
        state.CurrentScreen.Should().Be(Screen.Dashboard);

        state.CurrentScreen = Screen.Roster;
        state.CurrentScreen.Should().Be(Screen.Roster);

        state.CurrentScreen = Screen.Decisions;
        state.CurrentScreen.Should().Be(Screen.Decisions);
    }

    [Fact]
    public void AppState_MemberSelection_Works()
    {
        var state = new AppState();
        state.SelectedMemberName.Should().BeNull();

        state.SelectedMemberName = "Sonic";
        state.SelectedMemberName.Should().Be("Sonic");
    }
}
