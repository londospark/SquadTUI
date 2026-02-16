using Hex1b.Theming;
using SquadTUI.Rendering;

namespace SquadTUI.Themes;

public static class ThemeManager
{
    public static readonly string[] ThemeNames = ["Ocean", "Heist", "Sunset", "HighContrast"];

    public static Hex1bTheme GetTheme(int index) => (index % ThemeNames.Length) switch
    {
        0 => CreateOceanTheme(),
        1 => CreateHeistTheme(),
        2 => CreateSunsetTheme(),
        3 => CreateHighContrastTheme(),
        _ => CreateOceanTheme()
    };

    /// <summary>Returns the ANSI foreground accent color code for the given theme.</summary>
    public static string GetAccentCode(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[38;2;88;196;220m",  // Bright cyan
        1 => "\x1b[38;2;255;199;95m",  // Warm gold
        2 => "\x1b[38;2;255;121;198m", // Hot pink
        3 => "\x1b[97m",               // BrightWhite
        _ => "\x1b[38;2;88;196;220m"
    };

    /// <summary>Returns a secondary accent for subtle highlights.</summary>
    public static string GetSecondaryAccent(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[38;2;56;132;170m",  // Muted teal
        1 => "\x1b[38;2;180;140;70m",  // Bronze
        2 => "\x1b[38;2;189;95;147m",  // Dusty rose
        3 => "\x1b[38;2;160;160;160m", // Silver
        _ => "\x1b[38;2;56;132;170m"
    };

    /// <summary>Returns a dim rule line in the theme's secondary accent.</summary>
    public static string GetDimRule(int index, int width = 40)
    {
        var sec = GetSecondaryAccent(index);
        return $"  \x1b[2m{sec}{new string('━', width)}\x1b[0m";
    }

    /// <summary>Returns a PanelColors record with accent code for backward compat.</summary>
    public static PanelColors GetPanelColors(int index) => new(GetAccentCode(index));

    /// <summary>Modern borderless style — borders use space chars to become invisible.</summary>
    private static Hex1bTheme WithModernBorders(Hex1bTheme theme) => theme
        .Set(BorderTheme.TopLeftCorner, " ")
        .Set(BorderTheme.TopRightCorner, " ")
        .Set(BorderTheme.BottomLeftCorner, " ")
        .Set(BorderTheme.BottomRightCorner, " ")
        .Set(BorderTheme.HorizontalLine, " ")
        .Set(BorderTheme.VerticalLine, " ");

    /// <summary>Ocean — Deep navy with vivid cyan accents.</summary>
    public static Hex1bTheme CreateOceanTheme() => WithModernBorders(
        new Hex1bTheme("Ocean")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(200, 210, 220))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(13, 17, 23))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(30, 40, 55))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(88, 196, 220))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(13, 17, 23))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(88, 196, 220))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(88, 196, 220))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(25, 32, 42))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(40, 55, 70)));

    /// <summary>Heist — Rich indigo with warm gold accents.</summary>
    public static Hex1bTheme CreateHeistTheme() => WithModernBorders(
        new Hex1bTheme("Heist")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(210, 205, 200))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(20, 18, 36))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(35, 30, 60))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(255, 199, 95))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(20, 18, 36))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(255, 199, 95))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(255, 199, 95))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(30, 25, 50))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(55, 45, 80)));

    /// <summary>Sunset — Charcoal with hot pink and warm accents.</summary>
    public static Hex1bTheme CreateSunsetTheme() => WithModernBorders(
        new Hex1bTheme("Sunset")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(215, 210, 210))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(22, 20, 24))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(50, 30, 45))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(255, 121, 198))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(22, 20, 24))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(255, 121, 198))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(255, 121, 198))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(38, 30, 40))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(70, 45, 65)));

    /// <summary>HighContrast — True black with crisp white accents.</summary>
    public static Hex1bTheme CreateHighContrastTheme() => WithModernBorders(
        new Hex1bTheme("HighContrast")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.Black)
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(40, 40, 40))
            .Set(BorderTheme.TitleColor, Hex1bColor.White)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.White)
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.White)
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(35, 35, 35))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(60, 60, 60)));
}
