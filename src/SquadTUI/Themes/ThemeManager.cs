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

    /// <summary>Apply rounded border corners to a theme for modern look.</summary>
    private static Hex1bTheme WithRoundedBorders(Hex1bTheme theme) => theme
        .Set(BorderTheme.TopLeftCorner, "╭")
        .Set(BorderTheme.TopRightCorner, "╮")
        .Set(BorderTheme.BottomLeftCorner, "╰")
        .Set(BorderTheme.BottomRightCorner, "╯")
        .Set(BorderTheme.HorizontalLine, "─")
        .Set(BorderTheme.VerticalLine, "│");

    /// <summary>Ocean — Deep blue/cyan palette. Default theme.</summary>
    public static Hex1bTheme CreateOceanTheme() => WithRoundedBorders(
        new Hex1bTheme("Ocean")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.Black)
            .Set(BorderTheme.BorderColor, Hex1bColor.Cyan)
            .Set(BorderTheme.TitleColor, Hex1bColor.Cyan)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Cyan)
            .Set(ListTheme.SelectedIndicator, "▶ ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Cyan)
            .Set(ScrollTheme.TrackColor, Hex1bColor.DarkGray)
            .Set(SplitterTheme.DividerColor, Hex1bColor.Cyan));

    /// <summary>Heist — Dark with gold/amber accents. The Ocean's Eleven vibe.</summary>
    public static Hex1bTheme CreateHeistTheme() => WithRoundedBorders(
        new Hex1bTheme("Heist")
            .Set(GlobalTheme.ForegroundColor, Hex1bColor.White)
            .Set(GlobalTheme.BackgroundColor, Hex1bColor.Black)
            .Set(BorderTheme.BorderColor, Hex1bColor.Yellow)
            .Set(BorderTheme.TitleColor, Hex1bColor.Yellow)
            .Set(ListTheme.SelectedForegroundColor, Hex1bColor.Black)
            .Set(ListTheme.SelectedBackgroundColor, Hex1bColor.Yellow)
            .Set(ListTheme.SelectedIndicator, "▸ ")
            .Set(ScrollTheme.ThumbColor, Hex1bColor.Yellow)
            .Set(ScrollTheme.TrackColor, Hex1bColor.DarkGray)
            .Set(SplitterTheme.DividerColor, Hex1bColor.Yellow));
}
