using Hex1b.Theming;
using SquadTUI.Rendering;

namespace SquadTUI.Themes;

public static class ThemeManager
{
    public static readonly string[] ThemeNames = [
        "Ocean", "Heist", "Sunset", "HighContrast",
        "Forest", "Cyberpunk", "Midnight", "Ember", "Arctic", "Retro"
    ];

    public static Hex1bTheme GetTheme(int index) => (index % ThemeNames.Length) switch
    {
        0 => CreateOceanTheme(),
        1 => CreateHeistTheme(),
        2 => CreateSunsetTheme(),
        3 => CreateHighContrastTheme(),
        4 => CreateForestTheme(),
        5 => CreateCyberpunkTheme(),
        6 => CreateMidnightTheme(),
        7 => CreateEmberTheme(),
        8 => CreateArcticTheme(),
        9 => CreateRetroTheme(),
        _ => CreateOceanTheme()
    };

    /// <summary>Returns the ANSI foreground accent color code for the given theme.</summary>
    public static string GetAccentCode(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[38;2;088;196;220m", // Bright cyan
        1 => "\x1b[38;2;255;199;095m", // Warm gold
        2 => "\x1b[38;2;255;121;198m", // Hot pink
        3 => "\x1b[38;2;255;255;255m", // BrightWhite (RGB)
        4 => "\x1b[38;2;072;199;110m", // Emerald green
        5 => "\x1b[38;2;255;050;200m", // Neon pink
        6 => "\x1b[38;2;100;160;255m", // Ice blue
        7 => "\x1b[38;2;255;160;050m", // Amber
        8 => "\x1b[38;2;000;100;180m", // Cool blue
        9 => "\x1b[38;2;080;255;080m", // Phosphor green
        _ => "\x1b[38;2;088;196;220m"
    };

    /// <summary>Returns a secondary accent for subtle highlights.</summary>
    public static string GetSecondaryAccent(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[38;2;056;132;170m", // Muted teal
        1 => "\x1b[38;2;180;140;070m", // Bronze
        2 => "\x1b[38;2;189;095;147m", // Dusty rose
        3 => "\x1b[38;2;160;160;160m", // Silver
        4 => "\x1b[38;2;045;130;075m", // Dark green
        5 => "\x1b[38;2;180;040;140m", // Deep magenta
        6 => "\x1b[38;2;070;110;180m", // Steel blue
        7 => "\x1b[38;2;180;110;035m", // Dark amber
        8 => "\x1b[38;2;000;070;130m", // Deep blue
        9 => "\x1b[38;2;050;180;050m", // Dim green
        _ => "\x1b[38;2;056;132;170m"
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
        0 => "\x1b[48;2;022;028;038m",   // Ocean: slightly lighter navy
        1 => "\x1b[48;2;032;028;052m",   // Heist: slightly lighter indigo
        2 => "\x1b[48;2;042;028;036m",   // Sunset: slightly lighter charcoal-rose
        3 => "\x1b[48;2;018;018;018m",   // HighContrast: dark gray
        4 => "\x1b[48;2;020;035;025m",   // Forest: lighter woodland
        5 => "\x1b[48;2;035;018;040m",   // Cyberpunk: lighter purple
        6 => "\x1b[48;2;018;022;042m",   // Midnight: lighter navy
        7 => "\x1b[48;2;040;032;025m",   // Ember: lighter charcoal
        8 => "\x1b[48;2;210;220;230m",   // Arctic: light blue-gray
        9 => "\x1b[48;2;018;022;015m",   // Retro: dark olive
        _ => "\x1b[48;2;022;028;038m"
    };

    /// <summary>Returns an ANSI background code for a secondary panel shade (e.g. detail pane).</summary>
    public static string GetPanelDetailBg(int index) => (index % ThemeNames.Length) switch
    {
        0 => "\x1b[48;2;017;022;030m",   // Ocean: between base and header
        1 => "\x1b[48;2;026;023;044m",   // Heist: between base and header
        2 => "\x1b[48;2;035;023;030m",   // Sunset: between base and header
        3 => "\x1b[48;2;012;012;012m",   // HighContrast: near-black
        4 => "\x1b[48;2;016;028;020m",   // Forest: between base and header
        5 => "\x1b[48;2;028;014;034m",   // Cyberpunk: between base and header
        6 => "\x1b[48;2;014;018;035m",   // Midnight: between base and header
        7 => "\x1b[48;2;034;026;020m",   // Ember: between base and header
        8 => "\x1b[48;2;200;210;220m",   // Arctic: light blue-white
        9 => "\x1b[48;2;014;018;010m",   // Retro: dark green-black
        _ => "\x1b[48;2;017;022;030m"
    };

    /// <summary>Panel background color — primary panels (list panes, main sections).</summary>
    public static Hex1bColor GetPanelBgColor(int index) => (index % ThemeNames.Length) switch
    {
        0 => Hex1bColor.FromRgb(18, 23, 32),  // Ocean
        1 => Hex1bColor.FromRgb(26, 23, 44),  // Heist
        2 => Hex1bColor.FromRgb(35, 23, 30),  // Sunset
        3 => Hex1bColor.FromRgb(14, 14, 14),  // HighContrast
        4 => Hex1bColor.FromRgb(14, 24, 18),  // Forest
        5 => Hex1bColor.FromRgb(22, 12, 30),  // Cyberpunk
        6 => Hex1bColor.FromRgb(12, 15, 28),  // Midnight
        7 => Hex1bColor.FromRgb(30, 22, 16),  // Ember
        8 => Hex1bColor.FromRgb(230, 235, 240), // Arctic
        9 => Hex1bColor.FromRgb(10, 14, 8),   // Retro
        _ => Hex1bColor.FromRgb(18, 23, 32)
    };

    /// <summary>Panel background color — detail/secondary panels.</summary>
    public static Hex1bColor GetPanelDetailBgColor(int index) => (index % ThemeNames.Length) switch
    {
        0 => Hex1bColor.FromRgb(22, 28, 38),  // Ocean
        1 => Hex1bColor.FromRgb(32, 28, 52),  // Heist
        2 => Hex1bColor.FromRgb(42, 28, 36),  // Sunset
        3 => Hex1bColor.FromRgb(20, 20, 20),  // HighContrast
        4 => Hex1bColor.FromRgb(20, 35, 25),  // Forest
        5 => Hex1bColor.FromRgb(35, 18, 40),  // Cyberpunk
        6 => Hex1bColor.FromRgb(18, 22, 42),  // Midnight
        7 => Hex1bColor.FromRgb(40, 32, 25),  // Ember
        8 => Hex1bColor.FromRgb(210, 220, 230), // Arctic
        9 => Hex1bColor.FromRgb(18, 22, 15),  // Retro
        _ => Hex1bColor.FromRgb(22, 28, 38)
    };

    /// <summary>Panel background color — tertiary/accent panels.</summary>
    public static Hex1bColor GetPanelAltBgColor(int index) => (index % ThemeNames.Length) switch
    {
        0 => Hex1bColor.FromRgb(15, 20, 28),  // Ocean
        1 => Hex1bColor.FromRgb(22, 20, 38),  // Heist
        2 => Hex1bColor.FromRgb(30, 20, 26),  // Sunset
        3 => Hex1bColor.FromRgb(10, 10, 10),  // HighContrast
        4 => Hex1bColor.FromRgb(10, 20, 14),  // Forest
        5 => Hex1bColor.FromRgb(18, 10, 25),  // Cyberpunk
        6 => Hex1bColor.FromRgb(10, 12, 22),  // Midnight
        7 => Hex1bColor.FromRgb(25, 18, 12),  // Ember
        8 => Hex1bColor.FromRgb(220, 225, 232), // Arctic
        9 => Hex1bColor.FromRgb(8, 12, 6),    // Retro
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

    /// <summary>Forest — Deep green woodland with emerald accents.</summary>
    public static Hex1bTheme CreateForestTheme() => WithModernBorders(
        new Hex1bTheme("Forest")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(200, 215, 200))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(14, 24, 18))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(30, 50, 35))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(72, 199, 110))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(14, 24, 18))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(72, 199, 110))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(72, 199, 110))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(20, 35, 25))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(55, 95, 65)));

    /// <summary>Cyberpunk — Neon pink on dark purple.</summary>
    public static Hex1bTheme CreateCyberpunkTheme() => WithModernBorders(
        new Hex1bTheme("Cyberpunk")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(220, 200, 220))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(22, 12, 30))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(45, 25, 55))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(255, 50, 200))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(22, 12, 30))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(255, 50, 200))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(255, 50, 200))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(30, 18, 38))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(100, 55, 110)));

    /// <summary>Midnight — Deep blue-black with ice blue accents.</summary>
    public static Hex1bTheme CreateMidnightTheme() => WithModernBorders(
        new Hex1bTheme("Midnight")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(195, 205, 220))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(12, 15, 28))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(25, 32, 55))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(100, 160, 255))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(12, 15, 28))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(100, 160, 255))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(100, 160, 255))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(20, 25, 40))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(60, 75, 110)));

    /// <summary>Ember — Warm charcoal with amber accents.</summary>
    public static Hex1bTheme CreateEmberTheme() => WithModernBorders(
        new Hex1bTheme("Ember")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(215, 205, 195))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(30, 22, 16))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(55, 40, 28))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(255, 160, 50))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(30, 22, 16))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(255, 160, 50))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(255, 160, 50))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(38, 30, 22))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(105, 80, 55)));

    /// <summary>Arctic — Light theme with cool blue accents.</summary>
    public static Hex1bTheme CreateArcticTheme() => WithModernBorders(
        new Hex1bTheme("Arctic")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(30, 40, 55))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(230, 235, 240))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(180, 195, 210))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(0, 100, 180))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(240, 245, 250))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(0, 100, 180))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(0, 100, 180))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(200, 210, 220))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(150, 170, 190)));

    /// <summary>Retro — CRT green phosphor on black.</summary>
    public static Hex1bTheme CreateRetroTheme() => WithModernBorders(
        new Hex1bTheme("Retro")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.FromRgb(180, 220, 180))
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(10, 14, 8))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(30, 45, 25))
            .Set(BorderTheme.TitleColor, Hex1bColor.FromRgb(80, 255, 80))
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.FromRgb(10, 14, 8))
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.FromRgb(80, 255, 80))
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.FromRgb(80, 255, 80))
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(18, 25, 15))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(50, 90, 45)));
}
