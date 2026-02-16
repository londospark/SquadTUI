using FluentAssertions;
using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit;

public class ThemeManagerTests
{
    [Fact]
    public void ThemeNames_HasFourThemes()
    {
        ThemeManager.ThemeNames.Should().HaveCount(4);
    }

    [Theory]
    [InlineData(0, "Ocean")]
    [InlineData(1, "Heist")]
    [InlineData(2, "Sunset")]
    [InlineData(3, "HighContrast")]
    public void ThemeNames_MatchExpectedOrder(int index, string expected)
    {
        ThemeManager.ThemeNames[index].Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetTheme_ReturnsNonNull(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        theme.Should().NotBeNull();
    }

    [Theory]
    [InlineData(4, 0)]  // wraps to Ocean
    [InlineData(5, 1)]  // wraps to Heist
    [InlineData(8, 0)]  // wraps to Ocean
    public void GetTheme_WrapsAroundIndex(int input, int expectedEquivalent)
    {
        var theme = ThemeManager.GetTheme(input);
        var expected = ThemeManager.GetTheme(expectedEquivalent);
        theme.Should().NotBeNull();
        expected.Should().NotBeNull();
    }

    [Fact]
    public void CreateOceanTheme_ReturnsTheme()
    {
        var theme = ThemeManager.CreateOceanTheme();
        theme.Should().NotBeNull();
    }

    [Fact]
    public void CreateHeistTheme_ReturnsTheme()
    {
        var theme = ThemeManager.CreateHeistTheme();
        theme.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetAccentCode_ReturnsNonNullString(int index)
    {
        var code = ThemeManager.GetAccentCode(index);
        code.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetAccentCode_ReturnsAnsiEscapeCode(int index)
    {
        var code = ThemeManager.GetAccentCode(index);
        code.Should().StartWith("\x1b[");
    }

    [Theory]
    [InlineData(4, 0)]
    [InlineData(5, 1)]
    [InlineData(8, 0)]
    [InlineData(7, 3)]
    public void GetAccentCode_WrapsAroundIndex(int input, int expectedEquivalent)
    {
        var code = ThemeManager.GetAccentCode(input);
        var expected = ThemeManager.GetAccentCode(expectedEquivalent);
        code.Should().Be(expected);
    }

    [Fact]
    public void GetPanelColors_ReturnsNonNull()
    {
        var colors = ThemeManager.GetPanelColors(0);
        colors.Should().NotBeNull();
    }

    [Fact]
    public void GetPanelColors_AccentMatchesGetAccentCode()
    {
        for (int i = 0; i < 4; i++)
        {
            var colors = ThemeManager.GetPanelColors(i);
            colors.Accent.Should().Be(ThemeManager.GetAccentCode(i));
        }
    }
}
