using Hex1b.Theming;
using SquadTUI.Themes;

namespace SquadTUI.Rendering;

/// <summary>
/// Bundles all theme-derived ANSI codes and colors for a given theme index.
/// Eliminates the 8-10 lines of boilerplate that every screen repeats.
/// </summary>
public sealed class ThemeContext
{
    public string Acc { get; }
    public string Sec { get; }
    public string R { get; } = PanelRenderer.Reset;
    public string B { get; } = PanelRenderer.Bold;
    public string D { get; } = PanelRenderer.Dim;
    public string HBg { get; }
    public Hex1bColor PanelBg { get; }
    public Hex1bColor DetailBg { get; }
    public Hex1bColor AltBg { get; }
    public string HlBg { get; }
    public string HlFg { get; }
    public bool Em { get; }
    public int ThemeIndex { get; }

    public ThemeContext(int themeIndex, bool showEmoji)
    {
        ThemeIndex = themeIndex;
        Acc = ThemeManager.GetAccentCode(themeIndex);
        Sec = ThemeManager.GetSecondaryAccent(themeIndex);
        HBg = ThemeManager.GetPanelHeaderBg(themeIndex);
        PanelBg = ThemeManager.GetPanelBgColor(themeIndex);
        DetailBg = ThemeManager.GetPanelDetailBgColor(themeIndex);
        AltBg = ThemeManager.GetPanelAltBgColor(themeIndex);
        HlBg = ThemeManager.GetHighlightBg(themeIndex);
        HlFg = ThemeManager.GetHighlightFg(themeIndex);
        Em = showEmoji;
    }

    /// <summary>Renders a separator rule line of the given width.</summary>
    public string Separator(int width = 36) =>
        $"  {Sec}{new string('━', width)}{R}";

    /// <summary>Renders a section header with hBg + bold + accent styling.</summary>
    public string SectionHeader(string emoji, string ascii, string title) =>
        $"  {HBg}{B}{Acc}{IconHelper.Icon(emoji, ascii, Em)} {title}{R}";

    /// <summary>Renders a section header with custom padding/prefix.</summary>
    public string SectionHeader(string title) =>
        $"  {HBg}{B}{Acc}{title}{R}";
}
