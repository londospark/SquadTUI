using FluentAssertions;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit.Rendering;

public class PanelRendererTests
{
    [Fact]
    public void Reset_ContainsAnsiResetCode()
    {
        PanelRenderer.Reset.Should().Contain("\x1b[0m");
    }

    [Fact]
    public void Bold_ContainsAnsiBoldCode()
    {
        PanelRenderer.Bold.Should().Contain("\x1b[1m");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetPanelColors_ReturnsValidColorsForAllThemes(int themeIndex)
    {
        var colors = ThemeManager.GetPanelColors(themeIndex);
        colors.Accent.Should().NotBeNullOrEmpty();
    }
}
