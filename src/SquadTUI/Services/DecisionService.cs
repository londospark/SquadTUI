using SquadTUI.Models;

namespace SquadTUI.Services;

public class DecisionService(string teamRootPath) : IDecisionService
{
    private readonly string _decisionsFilePath = Path.Combine(teamRootPath, ".ai-team", "decisions.md");
    private readonly string _inboxPath = Path.Combine(teamRootPath, ".ai-team", "decisions", "inbox");

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
        var contentLines = new List<string>();
        int? titleLineNumber = null;

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();

            if (trimmed.StartsWith("## "))
            {
                // Flush previous decision
                if (currentTitle is not null)
                {
                    decisions.Add(new DecisionEntry(
                        currentTitle,
                        currentDate ?? "",
                        currentAuthor ?? "",
                        string.Join('\n', contentLines).Trim(),
                        filePath,
                        titleLineNumber));
                }

                currentTitle = trimmed[3..].Trim();
                currentDate = null;
                currentAuthor = null;
                contentLines.Clear();
                titleLineNumber = i + 1;
                continue;
            }

            if (currentTitle is null)
                continue;

            if (trimmed.StartsWith("**Date:**") || trimmed.StartsWith("- **Date:**"))
            {
                currentDate = ExtractBoldValue(trimmed, "Date");
            }
            else if (trimmed.StartsWith("**Author:**") || trimmed.StartsWith("- **Author:**"))
            {
                currentAuthor = ExtractBoldValue(trimmed, "Author");
            }
            else
            {
                contentLines.Add(lines[i]);
            }
        }

        // Flush last decision
        if (currentTitle is not null)
        {
            decisions.Add(new DecisionEntry(
                currentTitle,
                currentDate ?? "",
                currentAuthor ?? "",
                string.Join('\n', contentLines).Trim(),
                filePath,
                titleLineNumber));
        }

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
