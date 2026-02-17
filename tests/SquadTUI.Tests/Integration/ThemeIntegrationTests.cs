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
        Assert.Equal(0, state.SelectedThemeIndex);
    }

    [Fact]
    public void ThemeCycling_WrapsAround()
    {
        var state = new AppState();
        for (int i = 0; i < ThemeManager.ThemeNames.Length + 1; i++)
        {
            state.SelectedThemeIndex = (state.SelectedThemeIndex + 1) % ThemeManager.ThemeNames.Length;
        }
        Assert.Equal(1, state.SelectedThemeIndex);
    }

    [Fact]
    public void AllThemes_CanBeCreated()
    {
        for (int i = 0; i < ThemeManager.ThemeNames.Length; i++)
        {
            var theme = ThemeManager.GetTheme(i);
            Assert.NotNull(theme);
        }
    }

    [Fact]
    public void SampleData_IsAvailable()
    {
        Assert.NotEmpty(SampleData.Members);
        Assert.NotEmpty(SampleData.Tasks);
        Assert.NotEmpty(SampleData.Decisions);
        Assert.NotEmpty(SampleData.Skills);
        Assert.NotEmpty(SampleData.LogEntries);
    }

    [Fact]
    public void AppState_ScreenNavigation_Works()
    {
        var state = new AppState();
        Assert.Equal(Screen.Dashboard, state.CurrentScreen);

        state.CurrentScreen = Screen.Roster;
        Assert.Equal(Screen.Roster, state.CurrentScreen);

        state.CurrentScreen = Screen.Decisions;
        Assert.Equal(Screen.Decisions, state.CurrentScreen);
    }

    [Fact]
    public void AppState_MemberSelection_Works()
    {
        var state = new AppState();
        Assert.Null(state.SelectedMemberName);

        state.SelectedMemberName = "Sonic";
        Assert.Equal("Sonic", state.SelectedMemberName);
    }
}
