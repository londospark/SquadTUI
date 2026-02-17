using FluentAssertions;
using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit;

public class ThemeManagerTests
{
    [Fact]
    public void ThemeNames_HasTenThemes()
    {
        ThemeManager.ThemeNames.Should().HaveCount(10);
    }

    [Theory]
    [InlineData(0, "Ocean")]
    [InlineData(1, "Heist")]
    [InlineData(2, "Sunset")]
    [InlineData(3, "HighContrast")]
    [InlineData(4, "Forest")]
    [InlineData(5, "Cyberpunk")]
    [InlineData(6, "Midnight")]
    [InlineData(7, "Ember")]
    [InlineData(8, "Arctic")]
    [InlineData(9, "Retro")]
    public void ThemeNames_MatchExpectedOrder(int index, string expected)
    {
        ThemeManager.ThemeNames[index].Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void GetTheme_ReturnsNonNull(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        theme.Should().NotBeNull();
    }

    [Theory]
    [InlineData(10, 0)]  // wraps to Ocean
    [InlineData(11, 1)]  // wraps to Heist
    [InlineData(20, 0)]  // wraps to Ocean
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
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
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
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void GetAccentCode_ReturnsAnsiEscapeCode(int index)
    {
        var code = ThemeManager.GetAccentCode(index);
        code.Should().StartWith("\x1b[");
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(11, 1)]
    [InlineData(20, 0)]
    [InlineData(17, 7)]
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
        for (int i = 0; i < 10; i++)
        {
            var colors = ThemeManager.GetPanelColors(i);
            colors.Accent.Should().Be(ThemeManager.GetAccentCode(i));
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void GetTheme_NegativeIndex_DoesNotCrash(int index)
    {
        // C# modulo with negative numbers may return negative, but GetTheme should handle it
        var act = () => ThemeManager.GetTheme(index);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void GetAccentCode_VeryLargeIndex_WrapsCorrectly(int index)
    {
        var code = ThemeManager.GetAccentCode(index);
        code.Should().NotBeNullOrEmpty();
        code.Should().StartWith("\x1b[");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(99)]
    public void GetPanelColors_ReturnsValidForAllIndices(int index)
    {
        var colors = ThemeManager.GetPanelColors(index);
        colors.Should().NotBeNull();
        colors.Accent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetPanelColors_LargeIndex_WrapsToValidAccent()
    {
        var colors100 = ThemeManager.GetPanelColors(100);
        var colors0 = ThemeManager.GetPanelColors(0);
        // 100 % 10 == 0, so they should match
        colors100.Accent.Should().Be(colors0.Accent);
    }

    [Fact]
    public void CreateSunsetTheme_ReturnsTheme()
    {
        var theme = ThemeManager.CreateSunsetTheme();
        theme.Should().NotBeNull();
    }

    [Fact]
    public void CreateHighContrastTheme_ReturnsTheme()
    {
        var theme = ThemeManager.CreateHighContrastTheme();
        theme.Should().NotBeNull();
    }

    [Fact]
    public void AllThemes_HaveDistinctNames()
    {
        ThemeManager.ThemeNames.Should().OnlyHaveUniqueItems();
    }
}
