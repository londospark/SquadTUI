using SquadTUI.Models;

namespace SquadTUI.Services;

public class OrchestrationLogService(string teamRootPath) : IOrchestrationLogService
{
    private readonly string _orchestrationLogPath = Path.Combine(teamRootPath, ".ai-team", "orchestration-log");
    private readonly string _logPath = Path.Combine(teamRootPath, ".ai-team", "log");

    public async Task<IReadOnlyList<OrchestrationLogEntry>> GetEntriesAsync(CancellationToken ct = default)
    {
        var entries = new List<OrchestrationLogEntry>();
        entries.AddRange(await ParseLogDirectory(_orchestrationLogPath, ct));
        entries.AddRange(await ParseLogDirectory(_logPath, ct));
        return entries.OrderByDescending(e => e.Timestamp).ToList();
    }

    public async Task<IReadOnlyList<OrchestrationLogEntry>> GetEntriesByDateAsync(string date, CancellationToken ct = default)
    {
        var all = await GetEntriesAsync(ct);
        return all.Where(e => e.Date == date).ToList();
    }

    private static async Task<List<OrchestrationLogEntry>> ParseLogDirectory(string dirPath, CancellationToken ct)
    {
        var entries = new List<OrchestrationLogEntry>();
        if (!Directory.Exists(dirPath))
            return entries;

        foreach (var file in Directory.EnumerateFiles(dirPath, "*.md"))
        {
            var content = await File.ReadAllTextAsync(file, ct);
            entries.AddRange(ParseLogFile(content, file));
        }

        return entries;
    }

    private static List<OrchestrationLogEntry> ParseLogFile(string content, string filePath)
    {
        var entries = new List<OrchestrationLogEntry>();
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        // Extract date and topic from filename: {YYYY-MM-DD}-{topic}
        var date = "";
        var topic = fileName;
        if (fileName.Length >= 10 && fileName[4] == '-' && fileName[7] == '-')
        {
            date = fileName[..10];
            topic = fileName.Length > 11 ? fileName[11..].Replace('-', ' ') : "";
        }

        var timestamp = DateTimeOffset.MinValue;
        if (DateTimeOffset.TryParse(date, out var parsed))
            timestamp = parsed;

        var lines = content.Split('\n');
        var participants = new List<string>();
        var decisions = new List<string>();
        var outcomes = new List<string>();
        var summary = "";
        string? whatWasDone = null;

        var inParticipants = false;
        var inDecisions = false;
        var inOutcomes = false;
        var inWhatWasDone = false;
        var whatWasDoneLines = new List<string>();
        var summaryLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Detect section headers
            if (trimmed.StartsWith("## ") || trimmed.StartsWith("### "))
            {
                inParticipants = false;
                inDecisions = false;
                inOutcomes = false;
                inWhatWasDone = false;

                var heading = trimmed.TrimStart('#').Trim().ToLowerInvariant();
                if (heading.Contains("participant"))
                    inParticipants = true;
                else if (heading.Contains("decision"))
                    inDecisions = true;
                else if (heading.Contains("outcome") || heading.Contains("result"))
                    inOutcomes = true;
                else if (heading.Contains("what was done") || heading.Contains("summary") || heading.Contains("work done"))
                    inWhatWasDone = true;
                continue;
            }

            // Title from H1
            if (trimmed.StartsWith("# ") && string.IsNullOrEmpty(summary))
            {
                summary = trimmed[2..].Trim();
                continue;
            }

            if (inParticipants && trimmed.StartsWith("- "))
                participants.Add(trimmed[2..].Trim());
            else if (inDecisions && trimmed.StartsWith("- "))
                decisions.Add(trimmed[2..].Trim());
            else if (inOutcomes && trimmed.StartsWith("- "))
                outcomes.Add(trimmed[2..].Trim());
            else if (inWhatWasDone)
                whatWasDoneLines.Add(line);
        }

        if (whatWasDoneLines.Count > 0)
            whatWasDone = string.Join('\n', whatWasDoneLines).Trim();

        if (string.IsNullOrEmpty(summary))
            summary = topic;

        entries.Add(new OrchestrationLogEntry(
            timestamp,
            date,
            topic,
            participants,
            summary,
            decisions,
            outcomes,
            whatWasDone));

        return entries;
    }
}
