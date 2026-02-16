namespace SquadTUI.Rendering;

/// <summary>
/// Provides ANSI escape sequences for modern opencode-inspired panel styling
/// using background color shading instead of visible borders.
/// </summary>
public static class PanelRenderer
{
    public const string Reset = "\x1b[0m";
    public const string Bold = "\x1b[1m";
    public const string Dim = "\x1b[2m";

    /// <summary>Returns ANSI bg color escape for a given theme and panel depth.</summary>
    public static string PanelBg(int themeIndex, int depth) =>
        Themes.ThemeManager.GetPanelColors(themeIndex).GetBg(depth);

    /// <summary>Render a styled panel title header (bold + accent colored).</summary>
    public static string PanelTitle(int themeIndex, string emoji, string title)
    {
        var colors = Themes.ThemeManager.GetPanelColors(themeIndex);
        return $"{colors.PanelBg}  {Bold}{colors.Accent}{emoji} {title}{Reset}";
    }

    /// <summary>Render a panel content line with background.</summary>
    public static string PanelLine(int themeIndex, string content, int depth = 1)
    {
        var bg = Themes.ThemeManager.GetPanelColors(themeIndex).GetBg(depth);
        return $"{bg}  {content}{Reset}";
    }
}

/// <summary>Holds ANSI escape sequences for panel background colors.</summary>
public record PanelColors(string BaseBg, string PanelBg, string NestedBg, string Accent)
{
    public string GetBg(int depth) => depth switch
    {
        0 => BaseBg,
        1 => PanelBg,
        2 => NestedBg,
        _ => BaseBg
    };
}
