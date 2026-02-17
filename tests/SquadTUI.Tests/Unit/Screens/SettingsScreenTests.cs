using SquadTUI.Screens;

namespace SquadTUI.Tests.Unit.Screens;

public class SettingsScreenTests
{
    [Fact]
    public void Screen_Enum_IncludesSettings()
    {
        Assert.True(Enum.IsDefined(typeof(Screen), Screen.Settings));
    }

    [Fact]
    public void AppState_SettingsSelectedIndex_DefaultsToZero()
    {
        var state = new AppState();
        Assert.Equal(0, state.SettingsSelectedIndex);
    }

    [Fact]
    public void AppState_Settings_HasDefaultThemeName()
    {
        var state = new AppState();
        Assert.Equal("Ocean", state.Settings.ThemeName);
    }

    [Fact]
    public void AppState_Settings_VimBindings_DefaultsTrue()
    {
        var state = new AppState();
        Assert.True(state.Settings.VimBindings);
    }

    [Fact]
    public void AppState_Settings_MouseEnabled_DefaultsTrue()
    {
        var state = new AppState();
        Assert.True(state.Settings.MouseEnabled);
    }

    [Fact]
    public void AppState_Settings_ShowEmoji_DefaultsFalse()
    {
        var state = new AppState();
        Assert.False(state.Settings.ShowEmoji);
    }

    [Fact]
    public void AppState_Settings_MarkdownRendering_DefaultsTrue()
    {
        var state = new AppState();
        Assert.True(state.Settings.MarkdownRendering);
    }
}
