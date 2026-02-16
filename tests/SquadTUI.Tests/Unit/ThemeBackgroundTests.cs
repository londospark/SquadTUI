using FluentAssertions;
using Hex1b.Theming;
using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit;

public class ThemeBackgroundTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void EachTheme_HasBackgroundColor(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        var bg = theme.Get(GlobalTheme.BackgroundColor);
        bg.Should().NotBeNull();
    }

    [Fact]
    public void AllThemes_HaveDistinctBackgroundColors()
    {
        var backgrounds = new List<string>();
        for (int i = 0; i < ThemeManager.ThemeNames.Length; i++)
        {
            var theme = ThemeManager.GetTheme(i);
            var bg = theme.Get(GlobalTheme.BackgroundColor);
            backgrounds.Add(bg.ToBackgroundAnsi());
        }
        backgrounds.Should().OnlyHaveUniqueItems("each theme should have a unique background color");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void EachTheme_DividerColorDiffersFromBackground(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        var bg = theme.Get(GlobalTheme.BackgroundColor);
        var divider = theme.Get(SplitterTheme.DividerColor);

        var bgAnsi = bg.ToBackgroundAnsi();
        var divAnsi = divider.ToBackgroundAnsi();
        divAnsi.Should().NotBe(bgAnsi, "divider color should differ from background color");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void EachTheme_DividerColorIsNotPureBlack(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        var divider = theme.Get(SplitterTheme.DividerColor);
        var pureBlack = Hex1bColor.FromRgb(0, 0, 0);

        // Compare ANSI output — pure black would be a specific sequence
        var divAnsi = divider.ToBackgroundAnsi();
        var blackAnsi = pureBlack.ToBackgroundAnsi();
        divAnsi.Should().NotBe(blackAnsi, "divider color should not be pure black");
    }

    [Fact]
    public void AllThemes_HaveDistinctDividerColors()
    {
        var dividers = new List<string>();
        for (int i = 0; i < ThemeManager.ThemeNames.Length; i++)
        {
            var theme = ThemeManager.GetTheme(i);
            var divider = theme.Get(SplitterTheme.DividerColor);
            dividers.Add(divider.ToBackgroundAnsi());
        }
        dividers.Should().OnlyHaveUniqueItems("each theme should have a unique divider color");
    }

    [Theory]
    [InlineData(0, "Ocean")]
    [InlineData(1, "Heist")]
    [InlineData(2, "Sunset")]
    [InlineData(3, "HighContrast")]
    public void Theme_HasExpectedName(int index, string expectedName)
    {
        ThemeManager.ThemeNames[index].Should().Be(expectedName);
    }
}
