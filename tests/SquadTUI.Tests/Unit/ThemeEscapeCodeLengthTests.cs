using FluentAssertions;
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

        lengths.Should().AllBeEquivalentTo(lengths[0],
            "accent codes must have uniform length to prevent layout shift");
    }

    [Fact]
    public void GetSecondaryAccent_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetSecondaryAccent(i).Length)
            .ToList();

        lengths.Should().AllBeEquivalentTo(lengths[0],
            "secondary accent codes must have uniform length to prevent layout shift");
    }

    [Fact]
    public void GetPanelHeaderBg_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetPanelHeaderBg(i).Length)
            .ToList();

        lengths.Should().AllBeEquivalentTo(lengths[0],
            "panel header background codes must have uniform length to prevent layout shift");
    }

    [Fact]
    public void GetPanelDetailBg_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetPanelDetailBg(i).Length)
            .ToList();

        lengths.Should().AllBeEquivalentTo(lengths[0],
            "panel detail background codes must have uniform length to prevent layout shift");
    }

    [Fact]
    public void GetAccentCode_AllThemes_UseRgbFormat()
    {
        for (int i = 0; i < ThemeCount; i++)
        {
            var code = ThemeManager.GetAccentCode(i);
            code.Should().StartWith("\x1b[38;2;",
                $"theme {ThemeManager.ThemeNames[i]} accent should use RGB foreground format");
            code.Should().EndWith("m");
        }
    }

    [Fact]
    public void GetSecondaryAccent_AllThemes_UseRgbFormat()
    {
        for (int i = 0; i < ThemeCount; i++)
        {
            var code = ThemeManager.GetSecondaryAccent(i);
            code.Should().StartWith("\x1b[38;2;",
                $"theme {ThemeManager.ThemeNames[i]} secondary accent should use RGB foreground format");
            code.Should().EndWith("m");
        }
    }

    [Fact]
    public void GetDimRule_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => ThemeManager.GetDimRule(i).Length)
            .ToList();

        lengths.Should().AllBeEquivalentTo(lengths[0],
            "dim rule strings must have uniform length to prevent layout shift");
    }

    [Fact]
    public void PanelTitle_AllThemes_SameStringLength()
    {
        var lengths = Enumerable.Range(0, ThemeCount)
            .Select(i => PanelRenderer.PanelTitle(i, "📈", "Test Title").Length)
            .ToList();

        lengths.Should().AllBeEquivalentTo(lengths[0],
            "panel titles must have uniform length across themes to prevent layout shift");
    }
}
