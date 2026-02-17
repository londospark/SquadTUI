using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;

namespace SquadTUI.Services;

public class DataBridge
{
    private readonly ServiceProvider _services;

    public DataBridge(ServiceProvider services)
    {
        _services = services;
    }

    public async Task<DashboardData> LoadDashboardDataAsync(CancellationToken ct = default) =>
        await _services.SquadData.GetDashboardAsync(ct);

    public async Task<Either<AppError, IReadOnlyList<SquadMember>>> LoadRosterDataAsync(CancellationToken ct = default)
    {
        try
        {
            var roster = await _services.Team.GetRosterAsync(ct);
            return Right<AppError, IReadOnlyList<SquadMember>>(roster.Members ?? []);
        }
        catch (Exception ex)
        {
            return Left<AppError, IReadOnlyList<SquadMember>>(new ServiceError("Roster", ex.Message));
        }
    }

    public async Task<Either<AppError, IReadOnlyList<SquadTask>>> LoadTasksFromRosterAsync(CancellationToken ct = default)
    {
        try
        {
            var roster = await _services.Team.GetRosterAsync(ct);
            var members = roster.Members ?? [];
            var currentTasks = await _services.Team.GetCurrentTasksAsync(ct)
                ?? (IReadOnlyDictionary<string, string>)new Dictionary<string, string>();
            var tasks = new List<SquadTask>();
            var idx = 1;
            foreach (var m in members)
            {
                // Resolve task title: prefer member's current task, fall back to history
                string? resolvedTitle = m.CurrentTask.IsSome
                    ? (string)m.CurrentTask
                    : currentTasks.TryGetValue(m.Name, out var fromHistory) ? fromHistory : null;

                if (!string.IsNullOrEmpty(resolvedTitle))
                {
                    var status = m.Status switch
                    {
                        MemberStatus.Working => SquadTaskStatus.InProgress,
                        MemberStatus.Active => SquadTaskStatus.InProgress,
                        MemberStatus.Idle => SquadTaskStatus.Pending,
                        _ => SquadTaskStatus.Pending
                    };
                    tasks.Add(new SquadTask($"task-{idx}", resolvedTitle, default, status, m.Name));
                }
                idx++;
            }
            return Right<AppError, IReadOnlyList<SquadTask>>(tasks);
        }
        catch (Exception ex)
        {
            return Left<AppError, IReadOnlyList<SquadTask>>(new ServiceError("Tasks", ex.Message));
        }
    }

    public async Task<Either<AppError, IReadOnlyList<DecisionEntry>>> LoadDecisionsDataAsync(CancellationToken ct = default)
    {
        try
        {
            return Right<AppError, IReadOnlyList<DecisionEntry>>(await _services.Decisions.GetDecisionsAsync(ct));
        }
        catch (Exception ex)
        {
            return Left<AppError, IReadOnlyList<DecisionEntry>>(new ServiceError("Decisions", ex.Message));
        }
    }

    public async Task<Either<AppError, IReadOnlyList<Skill>>> LoadSkillsDataAsync(CancellationToken ct = default)
    {
        try
        {
            return Right<AppError, IReadOnlyList<Skill>>(await _services.Skills.GetSkillsAsync(ct));
        }
        catch (Exception ex)
        {
            return Left<AppError, IReadOnlyList<Skill>>(new ServiceError("Skills", ex.Message));
        }
    }

    public async Task<Either<AppError, IReadOnlyList<OrchestrationLogEntry>>> LoadLogDataAsync(CancellationToken ct = default)
    {
        try
        {
            return Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(await _services.OrchestrationLog.GetEntriesAsync(ct));
        }
        catch (Exception ex)
        {
            return Left<AppError, IReadOnlyList<OrchestrationLogEntry>>(new ServiceError("Log", ex.Message));
        }
    }

    public async Task<Option<string>> LoadCharterContentAsync(string memberName, CancellationToken ct = default)
    {
        try
        {
            var member = await _services.Team.GetMemberAsync(memberName, ct);
            if (member == null)
                return None;
            var charterPath = member.CharterPath.Where(File.Exists);
            return charterPath.IsSome
                ? Some(await File.ReadAllTextAsync((string)charterPath, ct))
                : None;
        }
        catch
        {
            return None;
        }
    }

    public async Task<Either<AppError, Unit>> AddMemberAsync(string name, string role, CancellationToken ct = default)
    {
        try
        {
            await _services.Team.AddMemberAsync(name, role, ct);
            return Right<AppError, Unit>(Unit.Default);
        }
        catch (Exception ex)
        {
            return Left<AppError, Unit>(new ServiceError("AddMember", ex.Message));
        }
    }

    public async Task<Either<AppError, Unit>> RemoveMemberAsync(string name, CancellationToken ct = default)
    {
        try
        {
            await _services.Team.RemoveMemberAsync(name, ct);
            return Right<AppError, Unit>(Unit.Default);
        }
        catch (Exception ex)
        {
            return Left<AppError, Unit>(new ServiceError("RemoveMember", ex.Message));
        }
    }
}
