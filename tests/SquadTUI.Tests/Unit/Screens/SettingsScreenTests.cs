using FluentAssertions;
using SquadTUI.Screens;

namespace SquadTUI.Tests.Unit.Screens;

public class SettingsScreenTests
{
    [Fact]
    public void Screen_Enum_IncludesSettings()
    {
        Enum.IsDefined(typeof(Screen), Screen.Settings).Should().BeTrue();
    }

    [Fact]
    public void AppState_SettingsSelectedIndex_DefaultsToZero()
    {
        var state = new AppState();
        state.SettingsSelectedIndex.Should().Be(0);
    }

    [Fact]
    public void AppState_Settings_HasDefaultThemeName()
    {
        var state = new AppState();
        state.Settings.ThemeName.Should().Be("Ocean");
    }

    [Fact]
    public void AppState_Settings_VimBindings_DefaultsTrue()
    {
        var state = new AppState();
        state.Settings.VimBindings.Should().BeTrue();
    }

    [Fact]
    public void AppState_Settings_MouseEnabled_DefaultsTrue()
    {
        var state = new AppState();
        state.Settings.MouseEnabled.Should().BeTrue();
    }

    [Fact]
    public void AppState_Settings_ShowEmoji_DefaultsTrue()
    {
        var state = new AppState();
        state.Settings.ShowEmoji.Should().BeTrue();
    }

    [Fact]
    public void AppState_Settings_MarkdownRendering_DefaultsTrue()
    {
        var state = new AppState();
        state.Settings.MarkdownRendering.Should().BeTrue();
    }
}
