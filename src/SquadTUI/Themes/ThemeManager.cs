using Hex1b.Theming;

namespace SquadTUI.Themes;

public static class ThemeManager
{
    public static readonly string[] ThemeNames = ["Ocean", "Heist", "Sunset", "HighContrast"];

    public static Hex1bTheme GetTheme(int index) => (index % ThemeNames.Length) switch
    {
        0 => CreateOceanTheme(),
        1 => CreateHeistTheme(),
        2 => Hex1bThemes.Sunset,
        3 => Hex1bThemes.HighContrast,
        _ => CreateOceanTheme()
    };

    /// <summary>Ocean — Deep blue/cyan palette. Default theme.</summary>
    public static Hex1bTheme CreateOceanTheme() =>
        new Hex1bTheme("Ocean")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Cyan)
            .Set(ListTheme.SelectedIndicator, "▶ ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Cyan)
            .Set(ScrollTheme.TrackColor, Hex1bColor.DarkGray)
            .Set(SplitterTheme.DividerColor, Hex1bColor.Cyan);

    /// <summary>Heist — Dark with gold/amber accents. The Ocean's Eleven vibe.</summary>
    public static Hex1bTheme CreateHeistTheme() =>
        new Hex1bTheme("Heist")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Yellow)
            .Set(ListTheme.SelectedIndicator, "▸ ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Yellow)
            .Set(ScrollTheme.TrackColor, Hex1bColor.DarkGray)
            .Set(SplitterTheme.DividerColor, Hex1bColor.Yellow);
}
