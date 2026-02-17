using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class SettingsServiceTests
{
    [Fact]
    public void DefaultSettings_HaveExpectedValues()
    {
        var settings = new AppSettings();
        Assert.Equal("Ocean", settings.ThemeName);
        Assert.True(settings.VimBindings);
        Assert.True(settings.MouseEnabled);
        Assert.True(settings.ShowEmoji);
        Assert.True(settings.MarkdownRendering);
        Assert.Equal("Dashboard", settings.DefaultScreen);
    }
}
