using SquadTUI.Models;

namespace SquadTUI.Services;

public class TeamService(string teamRootPath) : ITeamService
{
    private readonly string _aiTeamPath = Path.Combine(teamRootPath, ".ai-team");
    private readonly string _teamFilePath = Path.Combine(teamRootPath, ".ai-team", "team.md");

    public async Task<TeamRoster> GetRosterAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_teamFilePath))
            return new TeamRoster("SquadTUI");

        var content = await File.ReadAllTextAsync(_teamFilePath, ct);
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
        return roster.Members?.FirstOrDefault(m =>
            m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private string? GetCharterPath(string memberName)
    {
        var charterFile = Path.Combine(_aiTeamPath, "agents", memberName.ToLowerInvariant(), "charter.md");
        return File.Exists(charterFile) ? charterFile : null;
    }

    private static MemberStatus ParseMemberStatus(string text)
    {
        var cleaned = text.Replace("✅", "").Replace("📋", "").Replace("🔄", "").Trim().ToLowerInvariant();
        return cleaned switch
        {
            "active" => MemberStatus.Active,
            "idle" => MemberStatus.Idle,
            "working" => MemberStatus.Working,
            "offline" => MemberStatus.Offline,
            "silent" => MemberStatus.Idle,
            "monitor" => MemberStatus.Active,
            _ => MemberStatus.Active
        };
    }

    private static List<string> ParseTableRow(string line)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith('|')) trimmed = trimmed[1..];
        if (trimmed.EndsWith('|')) trimmed = trimmed[..^1];
        return trimmed.Split('|').Select(c => c.Trim()).ToList();
    }
}
