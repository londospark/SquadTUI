using SquadTUI.Models;

namespace SquadTUI.Services;

public interface ITeamService
{
    Task<TeamRoster> GetRosterAsync(CancellationToken ct = default);
    Task<SquadMember?> GetMemberAsync(string name, CancellationToken ct = default);
}
