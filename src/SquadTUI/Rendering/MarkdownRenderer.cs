using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Rendering;

/// <summary>
/// Renders markdown content as Hex1b text widgets with ANSI styling.
/// </summary>
public static class MarkdownRenderer
{
    // ANSI escape codes for terminal styling
    private const string Bold = "\x1b[1m";
    private const string Dim = "\x1b[2m";
    private const string Italic = "\x1b[3m";
    private const string Reset = "\x1b[0m";
    private const string Cyan = "\x1b[36m";
    private const string BrightCyan = "\x1b[96m";
    private const string Yellow = "\x1b[33m";
    private const string DarkGray = "\x1b[90m";

    public static Hex1bWidget[] Render<T>(WidgetContext<T> ctx, string? markdown) where T : Hex1bWidget
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return [ctx.Text("")];

        var lines = markdown.Split('\n');
        var widgets = new List<Hex1bWidget>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            widgets.Add(RenderLine(ctx, line));
        }

        return widgets.ToArray();
    }

    private static Hex1bWidget RenderLine<T>(WidgetContext<T> ctx, string line) where T : Hex1bWidget
    {
        // Headings
        if (line.StartsWith("### "))
            return ctx.Text($"  {Bold}{Cyan}▪ {line[4..]}{Reset}");
        if (line.StartsWith("## "))
            return ctx.Text($"  {Bold}{Cyan}■ {line[3..]}{Reset}");
        if (line.StartsWith("# "))
            return ctx.Text($"  {Bold}{BrightCyan}█ {line[2..]}{Reset}");

        // Horizontal rule
        if (line.Trim() is "---" or "***" or "___")
            return ctx.Text($"  {DarkGray}{new string('─', 60)}{Reset}");

        // Blockquote
        if (line.StartsWith("> "))
            return ctx.Text($"  {Dim}{Italic}│ {line[2..]}{Reset}");
        if (line == ">")
            return ctx.Text($"  {Dim}│{Reset}");

        // Unordered list
        if (line.StartsWith("- ") || line.StartsWith("* "))
            return ctx.Text($"  • {line[2..]}");
        if (line.StartsWith("  - ") || line.StartsWith("  * "))
            return ctx.Text($"    ◦ {line[4..]}");

        // Bold text in line
        if (line.Contains("**"))
        {
            var styled = line.Replace("**", Bold, StringComparison.Ordinal);
            // Toggle bold on/off
            var parts = line.Split("**");
            var result = "  ";
            for (var i = 0; i < parts.Length; i++)
            {
                result += i % 2 == 1 ? $"{Bold}{parts[i]}{Reset}" : parts[i];
            }
            return ctx.Text(result);
        }

        // Empty line
        if (string.IsNullOrWhiteSpace(line))
            return ctx.Text("");

        // Default text
        return ctx.Text($"  {line}");
    }

    /// <summary>Parses markdown headings from text for display as plain text (no ANSI).</summary>
    public static IEnumerable<(int Level, string Text)> ExtractHeadings(string markdown)
    {
        foreach (var rawLine in markdown.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.StartsWith("### "))
                yield return (3, line[4..]);
            else if (line.StartsWith("## "))
                yield return (2, line[3..]);
            else if (line.StartsWith("# "))
                yield return (1, line[2..]);
        }
    }
}
