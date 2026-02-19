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
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void EachTheme_HasBackgroundColor(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        var bg = theme.Get(GlobalTheme.BackgroundColor);
        Assert.NotNull(bg);
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
        Assert.Equal(backgrounds.Distinct().Count(), backgrounds.Count());
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
    public void EachTheme_DividerColorDiffersFromBackground(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        var bg = theme.Get(GlobalTheme.BackgroundColor);
        var divider = theme.Get(SplitterTheme.DividerColor);

        var bgAnsi = bg.ToBackgroundAnsi();
        var divAnsi = divider.ToBackgroundAnsi();
        Assert.NotEqual(bgAnsi, divAnsi);
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
    public void EachTheme_DividerColorIsNotPureBlack(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        var divider = theme.Get(SplitterTheme.DividerColor);
        var pureBlack = Hex1bColor.FromRgb(0, 0, 0);

        // Compare ANSI output — pure black would be a specific sequence
        var divAnsi = divider.ToBackgroundAnsi();
        var blackAnsi = pureBlack.ToBackgroundAnsi();
        Assert.NotEqual(blackAnsi, divAnsi);
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
        Assert.Equal(dividers.Distinct().Count(), dividers.Count());
    }

    // Theme_HasExpectedName Theory consolidated to ThemeManagerTests.ThemeNames_MatchExpectedOrder
}
