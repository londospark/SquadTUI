using FluentAssertions;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class SettingsServiceTests
{
    [Fact]
    public void DefaultSettings_HaveExpectedValues()
    {
        var settings = new AppSettings();
        settings.ThemeName.Should().Be("Ocean");
        settings.VimBindings.Should().BeTrue();
        settings.MouseEnabled.Should().BeTrue();
        settings.ShowEmoji.Should().BeTrue();
        settings.MarkdownRendering.Should().BeTrue();
        settings.DefaultScreen.Should().Be("Dashboard");
    }
}
