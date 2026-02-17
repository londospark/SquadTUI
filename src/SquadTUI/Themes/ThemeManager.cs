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

    /// <summary>Returns a rule line in the theme's secondary accent.</summary>
    public static string GetDimRule(int index, int width = 40)
    {
        var sec = GetSecondaryAccent(index);
        return $"  {sec}{new string('━', width)}\x1b[0m";
    }

    /// <summary>Returns a PanelColors record with accent code for backward compat.</summary>
    public static PanelColors GetPanelColors(int index) => new(GetAccentCode(index));

    /// <summary>Returns an ANSI background code for a slightly lighter panel header shade.</summary>
    public static string GetPanelHeaderBg(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[48;2;22;28;38m",   // Ocean: slightly lighter navy
        1 => "\x1b[48;2;32;28;52m",   // Heist: slightly lighter indigo
        2 => "\x1b[48;2;42;28;36m",   // Sunset: slightly lighter charcoal-rose
        3 => "\x1b[48;2;18;18;18m",   // HighContrast: dark gray
        _ => "\x1b[48;2;22;28;38m"
    };

    /// <summary>Returns an ANSI background code for a secondary panel shade (e.g. detail pane).</summary>
    public static string GetPanelDetailBg(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[48;2;17;22;30m",   // Ocean: between base and header
        1 => "\x1b[48;2;26;23;44m",   // Heist: between base and header
        2 => "\x1b[48;2;35;23;30m",   // Sunset: between base and header
        3 => "\x1b[48;2;12;12;12m",   // HighContrast: near-black
        _ => "\x1b[48;2;17;22;30m"
    };

    /// <summary>Panel background color — primary panels (list panes, main sections).</summary>
    public static Hex1bColor GetPanelBgColor(int index) => (index % ThemeNames.Length) switch
    {
        0 => Hex1bColor.FromRgb(18, 23, 32),  // Ocean
        1 => Hex1bColor.FromRgb(26, 23, 44),  // Heist
        2 => Hex1bColor.FromRgb(35, 23, 30),  // Sunset
        3 => Hex1bColor.FromRgb(14, 14, 14),  // HighContrast
        _ => Hex1bColor.FromRgb(18, 23, 32)
    };

    /// <summary>Panel background color — detail/secondary panels.</summary>
    public static Hex1bColor GetPanelDetailBgColor(int index) => (index % ThemeNames.Length) switch
    {
        0 => Hex1bColor.FromRgb(22, 28, 38),  // Ocean
        1 => Hex1bColor.FromRgb(32, 28, 52),  // Heist
        2 => Hex1bColor.FromRgb(42, 28, 36),  // Sunset
        3 => Hex1bColor.FromRgb(20, 20, 20),  // HighContrast
        _ => Hex1bColor.FromRgb(22, 28, 38)
    };

    /// <summary>Panel background color — tertiary/accent panels.</summary>
    public static Hex1bColor GetPanelAltBgColor(int index) => (index % ThemeNames.Length) switch
    {
        0 => Hex1bColor.FromRgb(15, 20, 28),  // Ocean
        1 => Hex1bColor.FromRgb(22, 20, 38),  // Heist
        2 => Hex1bColor.FromRgb(30, 20, 26),  // Sunset
        3 => Hex1bColor.FromRgb(10, 10, 10),  // HighContrast
        _ => Hex1bColor.FromRgb(15, 20, 28)
    };

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
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(70, 90, 110)));

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
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(85, 75, 115)));

    /// <summary>Sunset — Charcoal with hot pink and warm accents.</summary>
    public static Hex1bTheme CreateSunsetTheme() => WithModernBorders(
        new Hex1bTheme("Sunset")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(215, 210, 210))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(28, 18, 22))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(50, 30, 45))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(255, 121, 198))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(22, 20, 24))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(255, 121, 198))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(255, 121, 198))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(38, 30, 40))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(100, 75, 95)));

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
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(95, 95, 95)));
}
