using SquadTUI.Models;

namespace SquadTUI.Services;

public class DataBridge
{
    private readonly ServiceProvider _services;

    public DataBridge(ServiceProvider services)
    {
        _services = services;
    }

    public async Task<DashboardData> LoadDashboardDataAsync(CancellationToken ct = default)
    {
        return await _services.SquadData.GetDashboardAsync(ct);
    }

    public async Task<IReadOnlyList<SquadMember>> LoadRosterDataAsync(CancellationToken ct = default)
    {
        var roster = await _services.Team.GetRosterAsync(ct);
        return roster.Members ?? [];
    }

    public async Task<IReadOnlyList<SquadTask>> LoadTasksFromRosterAsync(CancellationToken ct = default)
    {
        var roster = await _services.Team.GetRosterAsync(ct);
        var members = roster.Members ?? [];
        var tasks = new List<SquadTask>();
        var idx = 1;
        foreach (var m in members)
        {
            if (!string.IsNullOrEmpty(m.CurrentTask))
            {
                var status = m.Status switch
                {
                    MemberStatus.Working => SquadTaskStatus.InProgress,
                    MemberStatus.Active => SquadTaskStatus.InProgress,
                    MemberStatus.Idle => SquadTaskStatus.Pending,
                    _ => SquadTaskStatus.Pending
                };
                tasks.Add(new SquadTask($"task-{idx}", m.CurrentTask, null, status, m.Name));
            }
            idx++;
        }
        return tasks;
    }

    public async Task<IReadOnlyList<DecisionEntry>> LoadDecisionsDataAsync(CancellationToken ct = default)
    {
        return await _services.Decisions.GetDecisionsAsync(ct);
    }

    public async Task<IReadOnlyList<Skill>> LoadSkillsDataAsync(CancellationToken ct = default)
    {
        return await _services.Skills.GetSkillsAsync(ct);
    }

    public async Task<IReadOnlyList<OrchestrationLogEntry>> LoadLogDataAsync(CancellationToken ct = default)
    {
        return await _services.OrchestrationLog.GetEntriesAsync(ct);
    }

    public async Task<string?> LoadCharterContentAsync(string memberName, CancellationToken ct = default)
    {
        var member = await _services.Team.GetMemberAsync(memberName, ct);
        if (member?.CharterPath == null || !File.Exists(member.CharterPath))
            return null;

        return await File.ReadAllTextAsync(member.CharterPath, ct);
    }
}
