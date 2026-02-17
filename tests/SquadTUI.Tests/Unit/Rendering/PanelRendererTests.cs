using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit.Rendering;

public class PanelRendererTests
{
    [Fact]
    public void Reset_ContainsAnsiResetCode()
    {
        Assert.Contains("\x1b[0m", PanelRenderer.Reset);
    }

    [Fact]
    public void Bold_ContainsAnsiBoldCode()
    {
        Assert.Contains("\x1b[1m", PanelRenderer.Bold);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetPanelColors_ReturnsValidColorsForAllThemes(int themeIndex)
    {
        var colors = ThemeManager.GetPanelColors(themeIndex);
        Assert.False(string.IsNullOrEmpty(colors.Accent));
    }
}
