using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Tests.Unit;

/// <summary>
/// Ensures all per-theme ANSI escape codes have uniform string length
/// so that Hex1b text measurement doesn't cause layout shift when
/// the user cycles themes with T.
/// </summary>
public class ThemeEscapeCodeLengthTests
{
    private static readonly int ThemeCount = ThemeManager.ThemeNames.Length;

    [Fact]
    public void GetAccentCode_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetAccentCode(i).Length)
            .ToList();

        Assert.All(lengths, item => Assert.Equal(lengths[0], item));
    }

    [Fact]
    public void GetSecondaryAccent_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetSecondaryAccent(i).Length)
            .ToList();

        Assert.All(lengths, item => Assert.Equal(lengths[0], item));
    }

    [Fact]
    public void GetPanelHeaderBg_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetPanelHeaderBg(i).Length)
            .ToList();

        Assert.All(lengths, item => Assert.Equal(lengths[0], item));
    }

    [Fact]
    public void GetPanelDetailBg_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetPanelDetailBg(i).Length)
            .ToList();

        Assert.All(lengths, item => Assert.Equal(lengths[0], item));
    }

    [Fact]
    public void GetAccentCode_AllThemes_UseRgbFormat()
    {
        for (int i = 0; i < ThemeCount; i++)
        {
            var code = ThemeManager.GetAccentCode(i);
            Assert.StartsWith("\x1b[38;2;", code);
            Assert.EndsWith("m", code);
        }
    }

    [Fact]
    public void GetSecondaryAccent_AllThemes_UseRgbFormat()
    {
        for (int i = 0; i < ThemeCount; i++)
        {
            var code = ThemeManager.GetSecondaryAccent(i);
            Assert.StartsWith("\x1b[38;2;", code);
            Assert.EndsWith("m", code);
        }
    }

    [Fact]
    public void GetDimRule_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetDimRule(i).Length)
            .ToList();

        Assert.All(lengths, item => Assert.Equal(lengths[0], item));
    }

    [Fact]
    public void PanelTitle_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => PanelRenderer.PanelTitle(i, "📈", "Test Title").Length)
            .ToList();

        Assert.All(lengths, item => Assert.Equal(lengths[0], item));
    }
}
