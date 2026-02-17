using SquadTUI.Screens;

namespace SquadTUI.Tests.Unit.Screens;

public class HelpScreenTests
{
    [Fact]
    public void Screen_Enum_IncludesHelp()
    {
        Assert.True(Enum.IsDefined(typeof(Screen), Screen.Help));
    }

    [Fact]
    public void AppState_PreviousScreen_DefaultsToNull()
    {
        var state = new AppState();
        Assert.Null(state.PreviousScreen);
    }

    [Fact]
    public void AppState_PreviousScreen_CanBeSet()
    {
        var state = new AppState();
        state.PreviousScreen = Screen.Roster;
        Assert.Equal(Screen.Roster, state.PreviousScreen);
    }

    [Fact]
    public void AppState_ShowHelp_DefaultsFalse()
    {
        var state = new AppState();
        Assert.False(state.ShowHelp);
    }
}
