using SquadTUI.Models;

namespace SquadTUI.Services;

public class SquadDataProvider(
    ITeamService teamService,
    IOrchestrationLogService logService,
    IDecisionService decisionService,
    ISkillService skillService
) : ISquadDataProvider
{
    public Task<TeamRoster> GetRosterAsync(CancellationToken ct = default)
        => teamService.GetRosterAsync(ct);

    public Task<IReadOnlyList<OrchestrationLogEntry>> GetLogEntriesAsync(CancellationToken ct = default)
        => logService.GetEntriesAsync(ct);

    public Task<IReadOnlyList<DecisionEntry>> GetDecisionsAsync(CancellationToken ct = default)
        => decisionService.GetDecisionsAsync(ct);

    public Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken ct = default)
        => skillService.GetSkillsAsync(ct);

    public async Task<DashboardData> GetDashboardAsync(CancellationToken ct = default)
    {
        var rosterTask = teamService.GetRosterAsync(ct);
        var decisionsTask = decisionService.GetDecisionsAsync(ct);
        var logTask = logService.GetEntriesAsync(ct);

        await Task.WhenAll(rosterTask, decisionsTask, logTask);

        var roster = rosterTask.Result;
        var decisions = decisionsTask.Result;
        var logEntries = logTask.Result;

        var members = roster.Members ?? [];
        var activeCount = members.Count(m => m.Status == MemberStatus.Active || m.Status == MemberStatus.Working);

        var memberOverviews = members.Select(m => new TeamMemberOverview(
            m.Name,
            m.Role,
            m.Status,
            TasksCompleted: 0,
            TasksInProgress: m.Status == MemberStatus.Working ? 1 : 0
        )).ToList();

        var teamSummary = new TeamSummary(members.Count, activeCount, memberOverviews);

        // Build activity swimlanes from log entries
        var swimlanes = members.Select(m =>
        {
            var memberLogs = logEntries
                .Where(e => e.Participants.Any(p => p.Equals(m.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(e => $"[{e.Date}] {e.Summary}")
                .ToList();
            return new ActivitySwimlane(m.Name, [], memberLogs);
        }).ToList();

        return new DashboardData(
            Team: teamSummary,
            Burndown: [],
            Velocity: [],
            Activity: swimlanes,
            Decisions: decisions.ToList()
        );
    }
}
