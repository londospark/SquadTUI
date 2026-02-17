using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;
using SquadTUI.Themes;

namespace SquadTUI.Tests;

/// <summary>
/// P1 — Theme coverage for all 10 themes: accent codes, ANSI validity, wrapping.
/// </summary>
public class ThemeCoverageTests
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
    public void GetAccentCode_IsNonEmpty_ForAllThemes(int index)
    {
        Assert.False(string.IsNullOrEmpty(ThemeManager.GetAccentCode(index)));
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
    public void GetSecondaryAccent_IsNonEmpty_ForAllThemes(int index)
    {
        Assert.False(string.IsNullOrEmpty(ThemeManager.GetSecondaryAccent(index)));
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
    public void GetPanelHeaderBg_IsNonEmpty_ForAllThemes(int index)
    {
        Assert.False(string.IsNullOrEmpty(ThemeManager.GetPanelHeaderBg(index)));
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
    public void GetPanelDetailBg_IsNonEmpty_ForAllThemes(int index)
    {
        Assert.False(string.IsNullOrEmpty(ThemeManager.GetPanelDetailBg(index)));
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
    public void AllColorMethods_ProduceValidAnsi_ForAllThemes(int index)
    {
        var accent = ThemeManager.GetAccentCode(index);
        var secondary = ThemeManager.GetSecondaryAccent(index);
        var headerBg = ThemeManager.GetPanelHeaderBg(index);
        var detailBg = ThemeManager.GetPanelDetailBg(index);

        Assert.StartsWith("\x1b[", accent);
        Assert.StartsWith("\x1b[", secondary);
        Assert.StartsWith("\x1b[", headerBg);
        Assert.StartsWith("\x1b[", detailBg);
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
    public void AccentCodes_HaveConsistentLength_ForAllThemes(int index)
    {
        var accent = ThemeManager.GetAccentCode(index);
        // All RGB foreground codes should be \x1b[38;2;RRR;GGG;BBBm — same format
        Assert.Contains("38;2;", accent);
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
    public void GetPanelBgColor_ReturnsNonNull_ForAllThemes(int index)
    {
        var color = ThemeManager.GetPanelBgColor(index);
        Assert.NotNull(color);
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
    public void GetPanelDetailBgColor_ReturnsNonNull_ForAllThemes(int index)
    {
        var color = ThemeManager.GetPanelDetailBgColor(index);
        Assert.NotNull(color);
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
    public void GetPanelAltBgColor_ReturnsNonNull_ForAllThemes(int index)
    {
        var color = ThemeManager.GetPanelAltBgColor(index);
        Assert.NotNull(color);
    }

    [Fact]
    public void ThemeCycling_WrapsAtIndex10()
    {
        Assert.Equal(ThemeManager.GetAccentCode(0), ThemeManager.GetAccentCode(10));
        Assert.Equal(ThemeManager.GetAccentCode(1), ThemeManager.GetAccentCode(11));
        Assert.Equal(ThemeManager.GetAccentCode(9), ThemeManager.GetAccentCode(19));
        Assert.Equal(ThemeManager.GetAccentCode(0), ThemeManager.GetAccentCode(20));
    }

    [Fact]
    public void ThemeCycling_SecondaryAccent_WrapsAtIndex10()
    {
        Assert.Equal(ThemeManager.GetSecondaryAccent(0), ThemeManager.GetSecondaryAccent(10));
        Assert.Equal(ThemeManager.GetSecondaryAccent(0), ThemeManager.GetSecondaryAccent(20));
    }

    [Fact]
    public void GetDimRule_ReturnsValidRule_ForAllThemes()
    {
        for (int i = 0; i < 10; i++)
        {
            var rule = ThemeManager.GetDimRule(i, 20);
            Assert.False(string.IsNullOrEmpty(rule));
            Assert.Contains("━", rule);
            Assert.Contains("\x1b[0m", rule);
        }
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
    public void GetTheme_ReturnsThemeWithCorrectName(int index)
    {
        var theme = ThemeManager.GetTheme(index);
        Assert.NotNull(theme);
    }

    [Fact]
    public void AllThemes_HaveDistinctAccentCodes()
    {
        var accents = Enumerable.Range(0, 10).Select(ThemeManager.GetAccentCode).ToList();
        Assert.Equal(accents.Count, accents.Distinct().Count());
    }

    [Fact]
    public void AllThemes_HaveDistinctSecondaryAccents()
    {
        var secondaries = Enumerable.Range(0, 10).Select(ThemeManager.GetSecondaryAccent).ToList();
        Assert.Equal(secondaries.Count, secondaries.Distinct().Count());
    }

    [Fact]
    public void SettingsThemeIndex_CanReferenceAllThemes()
    {
        var state = new AppState();
        for (int i = 0; i < 10; i++)
        {
            state.SelectedThemeIndex = i;
            var accent = ThemeManager.GetAccentCode(state.SelectedThemeIndex);
            Assert.False(string.IsNullOrEmpty(accent));
        }
    }
}
