namespace SquadTUI.Rendering;

/// <summary>
/// Provides ANSI escape sequences for text styling.
/// Background colors are handled by the Hex1b theme system (GlobalTheme.BackgroundColor).
/// </summary>
public static class PanelRenderer
{
    public const string Reset = "\x1b[0m";
    public const string Bold = "\x1b[1m";
    public const string Dim = "\x1b[2m";

    /// <summary>Render a styled panel title header (bold + accent colored).</summary>
    public static string PanelTitle(int themeIndex, string emoji, string title)
    {
        var accent = Themes.ThemeManager.GetAccentCode(themeIndex);
        return $"  {Bold}{accent}{emoji} {title}{Reset}";
    }

    /// <summary>Render a panel content line.</summary>
    public static string PanelLine(int themeIndex, string content)
    {
        return $"  {content}{Reset}";
    }
}

/// <summary>Holds the ANSI accent foreground color code for a theme.</summary>
public record PanelColors(string Accent);
