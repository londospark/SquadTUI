using SquadTUI.Models;

namespace SquadTUI.Services;

public interface ISquadDataProvider
{
    Task<TeamRoster> GetRosterAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrchestrationLogEntry>> GetLogEntriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<DecisionEntry>> GetDecisionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken ct = default);
    Task<DashboardData> GetDashboardAsync(CancellationToken ct = default);
}
