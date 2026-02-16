using FluentAssertions;
using SquadTUI.Screens;

namespace SquadTUI.Tests.Unit.Screens;

public class HelpScreenTests
{
    [Fact]
    public void Screen_Enum_IncludesHelp()
    {
        Enum.IsDefined(typeof(Screen), Screen.Help).Should().BeTrue();
    }

    [Fact]
    public void AppState_PreviousScreen_DefaultsToNull()
    {
        var state = new AppState();
        state.PreviousScreen.Should().BeNull();
    }

    [Fact]
    public void AppState_PreviousScreen_CanBeSet()
    {
        var state = new AppState();
        state.PreviousScreen = Screen.Roster;
        state.PreviousScreen.Should().Be(Screen.Roster);
    }

    [Fact]
    public void AppState_ShowHelp_DefaultsFalse()
    {
        var state = new AppState();
        state.ShowHelp.Should().BeFalse();
    }
}
