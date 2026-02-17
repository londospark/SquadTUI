using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;

namespace SquadTUI.Services;

public class TeamService : ITeamService
{
    private readonly IFileLocationService _fileLocations;

    public TeamService(IFileLocationService fileLocations)
    {
        _fileLocations = fileLocations;
    }

    /// <summary>Legacy constructor for backward compatibility (tests).</summary>
    public TeamService(string squadDirPath)
        : this(FileLocationService.FromSquadDirectory(squadDirPath)) { }

    public async Task<TeamRoster> GetRosterAsync(CancellationToken ct = default)
    {
        var teamFilePath = _fileLocations.GetRosterPath();
        if (!File.Exists(teamFilePath))
            return new TeamRoster("SquadTUI");

        var content = await File.ReadAllTextAsync(teamFilePath, ct);
        var lines = content.Split('\n');

        var projectName = "SquadTUI";
        string? description = null;
        var members = new List<SquadMember>();

        // Parse description from blockquote at top
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("> "))
            {
                description = trimmed[2..].Trim();
                break;
            }
        }

        // Parse project name from Project Context section
        var inProjectContext = false;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("## Project Context"))
            {
                inProjectContext = true;
                continue;
            }
            if (inProjectContext && trimmed.StartsWith("## "))
                break;
            if (inProjectContext && trimmed.StartsWith("- **Description:**"))
            {
                description = trimmed["- **Description:**".Length..].Trim();
            }
        }

        // Parse Members table
        var inMembers = false;
        var headerSkipped = false;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("## Members"))
            {
                inMembers = true;
                headerSkipped = false;
                continue;
            }
            if (inMembers && trimmed.StartsWith("## "))
                break;
            if (!inMembers || !trimmed.StartsWith('|'))
                continue;

            var cells = ParseTableRow(trimmed);
            if (cells.Count < 3)
                continue;

            // Skip header row and separator row
            if (cells[0].Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                cells[0].StartsWith('-'))
            {
                headerSkipped = true;
                continue;
            }
            if (!headerSkipped)
            {
                headerSkipped = true;
                continue;
            }

            var memberName = cells[0].Trim();
            var role = cells[1].Trim();
            var statusText = cells.Count >= 4 ? cells[3].Trim() : cells[2].Trim();
            var status = ParseMemberStatus(statusText);

            var charterPath = GetCharterPath(memberName);

            members.Add(new SquadMember(memberName, role, status, CharterPath: charterPath));
        }

        // Also parse Coordinator
        var inCoordinator = false;
        headerSkipped = false;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("## Coordinator"))
            {
                inCoordinator = true;
                headerSkipped = false;
                continue;
            }
            if (inCoordinator && trimmed.StartsWith("## "))
                break;
            if (!inCoordinator || !trimmed.StartsWith('|'))
                continue;

            var cells = ParseTableRow(trimmed);
            if (cells.Count < 2)
                continue;

            if (cells[0].Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                cells[0].StartsWith('-'))
            {
                headerSkipped = true;
                continue;
            }
            if (!headerSkipped)
            {
                headerSkipped = true;
                continue;
            }

            var coordName = cells[0].Trim();
            var coordRole = cells.Count >= 3 ? cells[1].Trim() : "Coordinator";
            members.Insert(0, new SquadMember(coordName, coordRole, MemberStatus.Active,
                CharterPath: GetCharterPath(coordName)));
        }

        return new TeamRoster(projectName, description, members);
    }

    public async Task<SquadMember?> GetMemberAsync(string name, CancellationToken ct = default)
    {
        var roster = await GetRosterAsync(ct);
        return (from m in roster.Members ?? []
                where m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                select m).FirstOrDefault();
    }

    private Option<string> GetCharterPath(string memberName)
    {
        var charterFile = _fileLocations.GetCharterPath(memberName);
        return File.Exists(charterFile) ? Some(charterFile) : None;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetCurrentTasksAsync(CancellationToken ct = default)
    {
        var tasks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Parse history.md files for recent work
        var agentsDir = _fileLocations.GetAgentsDirectory();
        if (Directory.Exists(agentsDir))
        {
            foreach (var agentDir in Directory.GetDirectories(agentsDir))
            {
                var agentName = Path.GetFileName(agentDir);
                var historyPath = _fileLocations.GetHistoryPath(agentName);
                if (File.Exists(historyPath))
                {
                    var task = await ExtractLatestTaskFromHistory(historyPath, ct);
                    if (task != null)
                        tasks[agentName] = task;
                }
            }
        }

        // Scan decision inbox for agent-specific mentions
        var inboxPath = _fileLocations.GetDecisionsInboxPath();
        if (Directory.Exists(inboxPath))
        {
            foreach (var file in Directory.GetFiles(inboxPath, "*.md"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var dashIdx = fileName.IndexOf('-');
                if (dashIdx > 0)
                {
                    var agentName = fileName[..dashIdx];
                    if (!tasks.ContainsKey(agentName))
                    {
                        var title = await ExtractTitleFromFile(file, ct);
                        if (title != null)
                            tasks[agentName] = title;
                    }
                }
            }
        }

        return tasks;
    }

    private static async Task<string?> ExtractLatestTaskFromHistory(string historyPath, CancellationToken ct)
    {
        var content = await File.ReadAllTextAsync(historyPath, ct);
        var lines = content.Split('\n');

        // Find the last ### heading (most recent learning entry)
        string? lastHeading = null;
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var trimmed = lines[i].Trim();
            if (trimmed.StartsWith("### "))
            {
                lastHeading = trimmed[4..].Trim();
                // Strip date prefix like "2026-02-18 — " or "2026-02-18: "
                var separators = new[] { " — ", ": " };
                foreach (var sep in separators)
                {
                    var sepIdx = lastHeading.IndexOf(sep, StringComparison.Ordinal);
                    if (sepIdx >= 8 && sepIdx <= 12)
                    {
                        lastHeading = lastHeading[(sepIdx + sep.Length)..];
                        break;
                    }
                }
                break;
            }
        }

        return lastHeading;
    }

    private static async Task<string?> ExtractTitleFromFile(string filePath, CancellationToken ct)
    {
        var content = await File.ReadAllTextAsync(filePath, ct);
        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("# "))
                return trimmed[2..].Trim();
        }
        return null;
    }

    public async Task AddMemberAsync(string name, string role, CancellationToken ct = default)
    {
        var teamFilePath = _fileLocations.GetRosterPath();
        if (!File.Exists(teamFilePath))
            throw new InvalidOperationException("team.md does not exist");

        var content = await File.ReadAllTextAsync(teamFilePath, ct);
        var lines = content.Split('\n').ToList();

        // Find the end of the Members table and insert a new row
        var inMembers = false;
        var insertIdx = -1;
        for (var i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Trim();
            if (trimmed.StartsWith("## Members"))
            {
                inMembers = true;
                continue;
            }
            if (inMembers && trimmed.StartsWith("## "))
            {
                insertIdx = i;
                break;
            }
            if (inMembers && trimmed.StartsWith('|'))
                insertIdx = i + 1;
        }

        if (insertIdx == -1)
            insertIdx = lines.Count;

        var agentDir = _fileLocations.GetAgentDirectory(name);
        var charterRelPath = $".ai-team/agents/{name.ToLowerInvariant()}/charter.md";

        // If the squad uses .squad/ directory, adjust the relative path
        if (_fileLocations.ActiveDirectoryName == ".squad")
            charterRelPath = $".squad/agents/{name.ToLowerInvariant()}/charter.md";

        var newRow = $"| {name} | {role} | `{charterRelPath}` | ✅ Active |";
        lines.Insert(insertIdx, newRow);

        await File.WriteAllTextAsync(teamFilePath, string.Join('\n', lines), ct);

        // Create agent directory with basic charter
        Directory.CreateDirectory(agentDir);
        var charterPath = _fileLocations.GetCharterPath(name);
        if (!File.Exists(charterPath))
        {
            var charterContent = $"""
                # {name} — {role}

                > New team member.

                ## Identity

                - **Name:** {name}
                - **Role:** {role}
                """;
            await File.WriteAllTextAsync(charterPath, charterContent, ct);
        }
    }

    public async Task RemoveMemberAsync(string name, CancellationToken ct = default)
    {
        var teamFilePath = _fileLocations.GetRosterPath();
        if (!File.Exists(teamFilePath))
            return;

        var content = await File.ReadAllTextAsync(teamFilePath, ct);
        var lines = content.Split('\n').ToList();

        // Find and remove the member's row from the Members table
        var inMembers = false;
        for (var i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Trim();
            if (trimmed.StartsWith("## Members"))
            {
                inMembers = true;
                continue;
            }
            if (inMembers && trimmed.StartsWith("## "))
                break;
            if (!inMembers || !trimmed.StartsWith('|'))
                continue;

            var cells = ParseTableRow(trimmed);
            if (cells.Count >= 2 && cells[0].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                lines.RemoveAt(i);
                break;
            }
        }

        await File.WriteAllTextAsync(teamFilePath, string.Join('\n', lines), ct);

        // Clean up agent directory
        var agentDir = _fileLocations.GetAgentDirectory(name);
        if (Directory.Exists(agentDir))
            Directory.Delete(agentDir, true);
    }

    private static MemberStatus ParseMemberStatus(string text) =>
        text.Replace("✅", "").Replace("📋", "").Replace("🔄", "").Trim().ToLowerInvariant() switch
        {
            "active" => MemberStatus.Active,
            "idle" => MemberStatus.Idle,
            "working" => MemberStatus.Working,
            "offline" => MemberStatus.Offline,
            "silent" => MemberStatus.Idle,
            "monitor" => MemberStatus.Active,
            _ => MemberStatus.Active
        };

    private static List<string> ParseTableRow(string line)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith('|')) trimmed = trimmed[1..];
        if (trimmed.EndsWith('|')) trimmed = trimmed[..^1];
        return trimmed.Split('|').Select(c => c.Trim()).ToList();
    }
}
