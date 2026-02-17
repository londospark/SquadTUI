using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit;

public class ThemeHighlightTests
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
    public void GetHighlightBg_ReturnsValidAnsi_ForAllThemes(int index)
    {
        var result = ThemeManager.GetHighlightBg(index);
        Assert.False(string.IsNullOrEmpty(result));
        Assert.StartsWith("\x1b[", result);
        Assert.Contains("48;2;", result);
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
    public void GetHighlightFg_ReturnsValidAnsi_ForAllThemes(int index)
    {
        var result = ThemeManager.GetHighlightFg(index);
        Assert.False(string.IsNullOrEmpty(result));
        Assert.StartsWith("\x1b[", result);
        Assert.Contains("38;2;", result);
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
    public void HighlightBg_DiffersFromAccentCode_ForAllThemes(int index)
    {
        var accent = ThemeManager.GetAccentCode(index);
        var highlightBg = ThemeManager.GetHighlightBg(index);
        // Accent is foreground (38;2;), highlight bg is background (48;2;) — always different
        Assert.NotEqual(accent, highlightBg);
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
    public void HighlightFg_DiffersFromAccentCode_ForAllThemes(int index)
    {
        var accent = ThemeManager.GetAccentCode(index);
        var highlightFg = ThemeManager.GetHighlightFg(index);
        Assert.NotEqual(accent, highlightFg);
    }

    [Fact]
    public void AllThemes_HaveDistinctHighlightBg()
    {
        var highlights = Enumerable.Range(0, 10).Select(ThemeManager.GetHighlightBg).ToList();
        Assert.Equal(highlights.Count, highlights.Distinct().Count());
    }

    [Fact]
    public void AllThemes_HaveDistinctHighlightFg()
    {
        var highlights = Enumerable.Range(0, 10).Select(ThemeManager.GetHighlightFg).ToList();
        Assert.Equal(highlights.Count, highlights.Distinct().Count());
    }

    [Fact]
    public void HighlightBg_WrapsAtIndex10()
    {
        Assert.Equal(ThemeManager.GetHighlightBg(0), ThemeManager.GetHighlightBg(10));
        Assert.Equal(ThemeManager.GetHighlightBg(9), ThemeManager.GetHighlightBg(19));
    }

    [Fact]
    public void HighlightFg_WrapsAtIndex10()
    {
        Assert.Equal(ThemeManager.GetHighlightFg(0), ThemeManager.GetHighlightFg(10));
        Assert.Equal(ThemeManager.GetHighlightFg(9), ThemeManager.GetHighlightFg(19));
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
    public void HighlightBg_EndsWithAnsiTerminator(int index)
    {
        var result = ThemeManager.GetHighlightBg(index);
        Assert.EndsWith("m", result);
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
    public void HighlightFg_EndsWithAnsiTerminator(int index)
    {
        var result = ThemeManager.GetHighlightFg(index);
        Assert.EndsWith("m", result);
    }
}
