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
        0 => "\x1b[36m",  // Cyan
        1 => "\x1b[33m",  // Yellow
        2 => "\x1b[35m",  // Magenta
        3 => "\x1b[97m",  // BrightWhite
        _ => "\x1b[36m"
    };

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

    /// <summary>Ocean — Deep navy with cyan accents.</summary>
    public static Hex1bTheme CreateOceanTheme() => WithModernBorders(
        new Hex1bTheme("Ocean")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(13, 17, 23))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(22, 27, 34))
            .Set(BorderTheme.TitleColor, Hex1bColor.Cyan)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Cyan)
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Cyan)
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(33, 38, 45))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(33, 38, 45)));

    /// <summary>Heist — Dark charcoal with gold/amber accents.</summary>
    public static Hex1bTheme CreateHeistTheme() => WithModernBorders(
        new Hex1bTheme("Heist")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(26, 26, 46))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(22, 33, 62))
            .Set(BorderTheme.TitleColor, Hex1bColor.Yellow)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Yellow)
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Yellow)
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(15, 52, 96))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(15, 52, 96)));

    /// <summary>Sunset — Warm dark with magenta/orange accents.</summary>
    public static Hex1bTheme CreateSunsetTheme() => WithModernBorders(
        new Hex1bTheme("Sunset")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.FromRgb(26, 26, 26))
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(45, 27, 46))
            .Set(BorderTheme.TitleColor, Hex1bColor.Magenta)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Magenta)
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Magenta)
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(70, 38, 57))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(70, 38, 57)));

    /// <summary>HighContrast — Pure black with white accents.</summary>
    public static Hex1bTheme CreateHighContrastTheme() => WithModernBorders(
        new Hex1bTheme("HighContrast")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.Black)
            .Set(BorderTheme.BorderColor, Hex1bColor.FromRgb(26, 26, 26))
            .Set(BorderTheme.TitleColor, Hex1bColor.White)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.White)
            .Set(ListTheme.SelectedIndicator, "  ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.White)
            .Set(ScrollTheme.TrackColor, Hex1bColor.FromRgb(42, 42, 42))
            .Set(SplitterTheme.DividerColor, Hex1bColor.FromRgb(42, 42, 42)));
}
