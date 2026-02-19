using SquadTUI.Models;

namespace SquadTUI.Services;

public interface ITeamService
{
    Task<TeamRoster> GetRosterAsync(CancellationToken ct = default);
    Task<SquadMember?> GetMemberAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, string>> GetCurrentTasksAsync(CancellationToken ct = default);
    /// <summary>
    /// Returns last file modification times for each agent's history.md and inbox files.
    /// Used to infer real-time activity instead of relying on static team.md status.
    /// </summary>
    Task<IReadOnlyDictionary<string, DateTime>> GetAgentActivityTimesAsync(CancellationToken ct = default);
    Task AddMemberAsync(string name, string role, CancellationToken ct = default);
    Task RemoveMemberAsync(string name, CancellationToken ct = default);
}
