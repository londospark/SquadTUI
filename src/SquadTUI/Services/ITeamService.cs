using SquadTUI.Models;

namespace SquadTUI.Services;

public interface ITeamService
{
    Task<TeamRoster> GetRosterAsync(CancellationToken ct = default);
    Task<SquadMember?> GetMemberAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, string>> GetCurrentTasksAsync(CancellationToken ct = default);
    Task AddMemberAsync(string name, string role, CancellationToken ct = default);
    Task RemoveMemberAsync(string name, CancellationToken ct = default);
}
