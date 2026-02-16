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
}
