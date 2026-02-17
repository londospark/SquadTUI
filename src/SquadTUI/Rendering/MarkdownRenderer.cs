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
        var i = 0;

        while (i < lines.Length)
        {
            var line = lines[i].TrimEnd('\r');

            // Detect markdown tables (lines starting with |)
            if (line.TrimStart().StartsWith('|'))
            {
                var tableLines = new List<string>();
                while (i < lines.Length && lines[i].TrimEnd('\r').TrimStart().StartsWith('|'))
                {
                    tableLines.Add(lines[i].TrimEnd('\r'));
                    i++;
                }
                widgets.AddRange(RenderTable(ctx, tableLines));
                continue;
            }

            widgets.Add(RenderLine(ctx, line));
            i++;
        }

        return widgets.ToArray();
    }

    private static Hex1bWidget[] RenderTable<T>(WidgetContext<T> ctx, List<string> tableLines) where T : Hex1bWidget
    {
        if (tableLines.Count == 0) return [];

        // Parse rows, skipping separator lines (|---|---|)
        var rows = new List<string[]>();
        var separatorIndex = -1;
        for (var i = 0; i < tableLines.Count; i++)
        {
            var trimmed = tableLines[i].Trim();
            var cells = ParseTableRow(trimmed);
            if (cells.Length > 0 && cells.All(c => c.Trim().All(ch => ch == '-' || ch == ':' || ch == ' ')))
            {
                separatorIndex = i;
                continue; // skip separator
            }
            rows.Add(cells);
        }

        if (rows.Count == 0) return [];

        // Calculate column widths
        var colCount = rows.Max(r => r.Length);
        var colWidths = new int[colCount];
        foreach (var row in rows)
        {
            for (var c = 0; c < row.Length; c++)
                colWidths[c] = Math.Max(colWidths[c], row[c].Trim().Length);
        }

        var widgets = new List<Hex1bWidget>();

        // Top border: ┌─────┬─────┐
        widgets.Add(ctx.Text($"  {DarkGray}┌{string.Join("┬", colWidths.Select(w => new string('─', w + 2)))}┐{Reset}"));

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            var cellTexts = new string[colCount];
            for (var c = 0; c < colCount; c++)
            {
                var cell = c < row.Length ? row[c].Trim() : "";
                cellTexts[c] = cell.PadRight(colWidths[c]);
            }

            if (r == 0 && separatorIndex == 1)
            {
                // Header row: bold
                widgets.Add(ctx.Text($"  {DarkGray}│{Reset} {string.Join($" {DarkGray}│{Reset} ", cellTexts.Select(c => $"{Bold}{c}{Reset}"))} {DarkGray}│{Reset}"));
                // Header separator: ├─────┼─────┤
                widgets.Add(ctx.Text($"  {DarkGray}├{string.Join("┼", colWidths.Select(w => new string('─', w + 2)))}┤{Reset}"));
            }
            else
            {
                // Data row
                widgets.Add(ctx.Text($"  {DarkGray}│{Reset} {string.Join($" {DarkGray}│{Reset} ", cellTexts)} {DarkGray}│{Reset}"));
            }
        }

        // Bottom border: └─────┴─────┘
        widgets.Add(ctx.Text($"  {DarkGray}└{string.Join("┴", colWidths.Select(w => new string('─', w + 2)))}┘{Reset}"));

        return widgets.ToArray();
    }

    private static string[] ParseTableRow(string line)
    {
        // Remove leading/trailing | and split
        if (line.StartsWith('|')) line = line[1..];
        if (line.EndsWith('|')) line = line[..^1];
        return line.Split('|');
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
