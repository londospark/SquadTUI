namespace SquadTUI.Rendering;

/// <summary>
/// Renders a themed progress bar using block characters.
/// </summary>
public static class ProgressBarRenderer
{
    private const string Reset = "\x1b[0m";
    private const string DimGray = "\x1b[90m";

    /// <summary>
    /// Renders a progress bar string for a given value between 0.0 and 1.0.
    /// </summary>
    /// <param name="value">Progress value from 0.0 to 1.0</param>
    /// <param name="width">Total width in characters (default 10)</param>
    /// <param name="filledColor">ANSI color code for the filled portion (default green)</param>
    /// <returns>A tuple of (Bar, Label) where Label is "Low", "Medium", or "High"</returns>
    public static (string Bar, string Label) Render(float value, int width = 10, string? filledColor = null)
    {
        value = Math.Clamp(value, 0f, 1f);
        var filled = (int)Math.Round(value * width);
        var empty = width - filled;

        var color = filledColor ?? GetColorForValue(value);
        var bar = $"{color}{new string('█', filled)}{DimGray}{new string('░', empty)}{Reset}";
        var label = GetLabelForValue(value);

        return (bar, label);
    }

    /// <summary>
    /// Converts a confidence string ("low", "medium", "high") to a float value.
    /// </summary>
    public static float ConfidenceToFloat(string? confidence) => confidence?.ToLowerInvariant() switch
    {
        "high" => 0.8f,
        "medium" => 0.5f,
        "low" => 0.3f,
        _ => 0.5f
    };

    private static string GetColorForValue(float value) => value switch
    {
        >= 0.7f => "\x1b[32m", // green
        >= 0.4f => "\x1b[33m", // yellow
        _ => "\x1b[31m"        // red
    };

    private static string GetLabelForValue(float value) => value switch
    {
        >= 0.7f => "High",
        >= 0.4f => "Medium",
        _ => "Low"
    };
}
