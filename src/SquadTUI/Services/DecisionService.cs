using SquadTUI.Models;

namespace SquadTUI.Services;

public class DecisionService(string squadDirPath) : IDecisionService
{
    private readonly string _decisionsFilePath = Path.Combine(squadDirPath, "decisions.md");
    private readonly string _inboxPath = Path.Combine(squadDirPath, "decisions", "inbox");

    public async Task<IReadOnlyList<DecisionEntry>> GetDecisionsAsync(CancellationToken ct = default)
    {
        var decisions = new List<DecisionEntry>();

        // Parse decisions.md
        if (File.Exists(_decisionsFilePath))
        {
            var content = await File.ReadAllTextAsync(_decisionsFilePath, ct);
            decisions.AddRange(ParseDecisionsMarkdown(content, _decisionsFilePath));
        }

        // Parse inbox files
        if (Directory.Exists(_inboxPath))
        {
            foreach (var file in Directory.EnumerateFiles(_inboxPath, "*.md"))
            {
                var content = await File.ReadAllTextAsync(file, ct);
                decisions.AddRange(ParseInboxDecision(content, file));
            }
        }

        return decisions;
    }

    private static List<DecisionEntry> ParseDecisionsMarkdown(string content, string filePath)
    {
        var decisions = new List<DecisionEntry>();
        var lines = content.Split('\n');

        string? currentTitle = null;
        string? currentDate = null;
        string? currentAuthor = null;
        string? currentWhat = null;
        string? currentWhy = null;
        var currentSection = "";
        var contentLines = new List<string>();
        int? titleLineNumber = null;

        void FlushDecision()
        {
            if (currentTitle is null) return;

            // Flush remaining section
            if (currentSection == "what" && contentLines.Count > 0 && currentWhat is null)
                currentWhat = string.Join('\n', contentLines).Trim();
            else if (currentSection == "why" && contentLines.Count > 0 && currentWhy is null)
                currentWhy = string.Join('\n', contentLines).Trim();
            else if (currentSection == "body" && contentLines.Count > 0)
                currentWhat = string.Join('\n', contentLines).Trim();

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(currentWhat))
                parts.Add(currentSection == "body" ? currentWhat : $"What: {currentWhat}");
            if (!string.IsNullOrWhiteSpace(currentWhy))
                parts.Add($"Why: {currentWhy}");

            decisions.Add(new DecisionEntry(
                currentTitle,
                currentDate ?? "",
                currentAuthor ?? "",
                string.Join("\n\n", parts),
                filePath,
                titleLineNumber));
        }

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();

            // Handle ## or ### headings as decision titles (skip # top-level headings)
            if ((trimmed.StartsWith("## ") && !trimmed.StartsWith("## #")) || trimmed.StartsWith("### "))
            {
                FlushDecision();

                var heading = trimmed.StartsWith("### ") ? trimmed[4..].Trim() : trimmed[3..].Trim();

                // Try "### date: title" format
                var colonIdx = heading.IndexOf(':');
                if (colonIdx > 0 && colonIdx <= 10)
                {
                    var possibleDate = heading[..colonIdx].Trim();
                    if (possibleDate.Contains('-') && possibleDate.Length >= 8)
                    {
                        currentDate = possibleDate;
                        currentTitle = heading[(colonIdx + 1)..].Trim();
                    }
                    else
                    {
                        currentDate = null;
                        currentTitle = heading;
                    }
                }
                else
                {
                    currentDate = null;
                    currentTitle = heading;
                }

                currentAuthor = null;
                currentWhat = null;
                currentWhy = null;
                currentSection = "body";
                contentLines.Clear();
                titleLineNumber = i + 1;
                continue;
            }

            if (trimmed == "---") continue;
            if (currentTitle is null) continue;

            // Handle metadata fields (supports both "**Key:**" and "- **Key:**" prefixes)
            if (trimmed.StartsWith("**By:**") || trimmed.Contains("**By:**"))
            {
                currentAuthor = ExtractBoldValue(trimmed, "By");
                continue;
            }
            if (trimmed.StartsWith("**Author:**") || trimmed.Contains("**Author:**"))
            {
                currentAuthor = ExtractBoldValue(trimmed, "Author");
                continue;
            }
            if (trimmed.StartsWith("**Date:**") || trimmed.Contains("**Date:**"))
            {
                currentDate = ExtractBoldValue(trimmed, "Date");
                continue;
            }

            // Handle **What:** section
            if (trimmed.StartsWith("**What:**") || trimmed.Contains("**What:**"))
            {
                if (currentSection == "body" && contentLines.Count > 0)
                    contentLines.Clear(); // switch from body to structured
                if (currentSection == "what" && contentLines.Count > 0)
                {
                    currentWhat = string.Join('\n', contentLines).Trim();
                    contentLines.Clear();
                }
                currentSection = "what";
                var inline = ExtractBoldValue(trimmed, "What");
                if (!string.IsNullOrWhiteSpace(inline)) contentLines.Add(inline);
                continue;
            }

            // Handle **Why:** section
            if (trimmed.StartsWith("**Why:**") || trimmed.Contains("**Why:**"))
            {
                if (currentSection == "what" && contentLines.Count > 0)
                {
                    currentWhat = string.Join('\n', contentLines).Trim();
                    contentLines.Clear();
                }
                currentSection = "why";
                var inline = ExtractBoldValue(trimmed, "Why");
                if (!string.IsNullOrWhiteSpace(inline)) contentLines.Add(inline);
                continue;
            }

            // Skip empty lines before content starts
            if (string.IsNullOrWhiteSpace(trimmed) && contentLines.Count == 0) continue;

            contentLines.Add(lines[i]);
        }

        FlushDecision();
        return decisions;
    }

    private static List<DecisionEntry> ParseInboxDecision(string content, string filePath)
    {
        var decisions = new List<DecisionEntry>();
        var lines = content.Split('\n');

        string? title = null;
        string? date = null;
        string? author = null;
        var contentLines = new List<string>();
        int? titleLineNumber = null;

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();

            // Inbox files may use ### or # or ## for title
            if (title is null && (trimmed.StartsWith("# ") || trimmed.StartsWith("### ")))
            {
                title = trimmed.TrimStart('#').Trim();
                titleLineNumber = i + 1;
                continue;
            }

            if (trimmed.StartsWith("**Date:**") || trimmed.StartsWith("- **Date:**"))
            {
                date = ExtractBoldValue(trimmed, "Date");
            }
            else if (trimmed.StartsWith("**Author:**") || trimmed.StartsWith("- **Author:**"))
            {
                author = ExtractBoldValue(trimmed, "Author");
            }
            else if (trimmed.StartsWith("**By:**"))
            {
                author = ExtractBoldValue(trimmed, "By");
            }
            else if (trimmed.StartsWith("- **Status:**"))
            {
                // skip status lines
            }
            else
            {
                contentLines.Add(lines[i]);
            }
        }

        // Extract date from filename if not found in content (e.g., "copilot-directive-20260216-...")
        if (date is null)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var dateMatch = System.Text.RegularExpressions.Regex.Match(fileName, @"(\d{8})");
            if (dateMatch.Success)
            {
                var d = dateMatch.Value;
                date = $"{d[..4]}-{d[4..6]}-{d[6..8]}";
            }
        }

        title ??= Path.GetFileNameWithoutExtension(filePath);

        decisions.Add(new DecisionEntry(
            title,
            date ?? "",
            author ?? "",
            string.Join('\n', contentLines).Trim(),
            filePath,
            titleLineNumber));

        return decisions;
    }

    private static string ExtractBoldValue(string line, string key)
    {
        // Handles both "**Key:** value" and "- **Key:** value"
        var marker = $"**{key}:**";
        var idx = line.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return "";
        return line[(idx + marker.Length)..].Trim();
    }
}
