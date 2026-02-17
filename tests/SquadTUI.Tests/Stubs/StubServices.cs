using LanguageExt;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Stubs;

/// <summary>Hand-written stubs replacing NSubstitute. Each stub can be configured
/// with return values or exceptions to throw.</summary>

public class StubTeamService : ITeamService
{
    public TeamRoster? RosterResult { get; set; }
    public Exception? RosterException { get; set; }
    public SquadMember? MemberResult { get; set; }
    public Exception? MemberException { get; set; }
    public IReadOnlyDictionary<string, string>? CurrentTasksResult { get; set; }
    public Exception? CurrentTasksException { get; set; }
    public Exception? AddMemberException { get; set; }
    public Exception? RemoveMemberException { get; set; }

    public Task<TeamRoster> GetRosterAsync(CancellationToken ct = default) =>
        RosterException is not null
            ? throw RosterException
            : Task.FromResult(RosterResult ?? new TeamRoster("Stub"));

    public Task<SquadMember?> GetMemberAsync(string name, CancellationToken ct = default) =>
        MemberException is not null
            ? throw MemberException
            : Task.FromResult(MemberResult);

    public Task<IReadOnlyDictionary<string, string>> GetCurrentTasksAsync(CancellationToken ct = default) =>
        CurrentTasksException is not null
            ? throw CurrentTasksException
            : Task.FromResult(CurrentTasksResult
                ?? (IReadOnlyDictionary<string, string>)new Dictionary<string, string>());

    public Task AddMemberAsync(string name, string role, CancellationToken ct = default) =>
        AddMemberException is not null
            ? throw AddMemberException
            : Task.CompletedTask;

    public Task RemoveMemberAsync(string name, CancellationToken ct = default) =>
        RemoveMemberException is not null
            ? throw RemoveMemberException
            : Task.CompletedTask;
}

public class StubDecisionService : IDecisionService
{
    public IReadOnlyList<DecisionEntry>? Result { get; set; }
    public Exception? Exception { get; set; }

    public Task<IReadOnlyList<DecisionEntry>> GetDecisionsAsync(CancellationToken ct = default) =>
        Exception is not null
            ? throw Exception
            : Task.FromResult(Result ?? (IReadOnlyList<DecisionEntry>)[]);
}

public class StubSkillService : ISkillService
{
    public IReadOnlyList<Skill>? Result { get; set; }
    public Exception? Exception { get; set; }
    public Skill? SingleResult { get; set; }

    public Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken ct = default) =>
        Exception is not null
            ? throw Exception
            : Task.FromResult(Result ?? (IReadOnlyList<Skill>)[]);

    public Task<Skill?> GetSkillAsync(string slug, CancellationToken ct = default) =>
        Task.FromResult(SingleResult);
}

public class StubOrchestrationLogService : IOrchestrationLogService
{
    public IReadOnlyList<OrchestrationLogEntry>? Result { get; set; }
    public Exception? Exception { get; set; }

    public Task<IReadOnlyList<OrchestrationLogEntry>> GetEntriesAsync(CancellationToken ct = default) =>
        Exception is not null
            ? throw Exception
            : Task.FromResult(Result ?? (IReadOnlyList<OrchestrationLogEntry>)[]);

    public Task<IReadOnlyList<OrchestrationLogEntry>> GetEntriesByDateAsync(string date, CancellationToken ct = default) =>
        Task.FromResult(Result ?? (IReadOnlyList<OrchestrationLogEntry>)[]);
}

public class StubSquadDataProvider : ISquadDataProvider
{
    public TeamRoster? RosterResult { get; set; }
    public IReadOnlyList<OrchestrationLogEntry>? LogResult { get; set; }
    public IReadOnlyList<DecisionEntry>? DecisionResult { get; set; }
    public IReadOnlyList<Skill>? SkillResult { get; set; }
    public DashboardData? DashboardResult { get; set; }

    public Task<TeamRoster> GetRosterAsync(CancellationToken ct = default) =>
        Task.FromResult(RosterResult ?? new TeamRoster("Stub"));

    public Task<IReadOnlyList<OrchestrationLogEntry>> GetLogEntriesAsync(CancellationToken ct = default) =>
        Task.FromResult(LogResult ?? (IReadOnlyList<OrchestrationLogEntry>)[]);

    public Task<IReadOnlyList<DecisionEntry>> GetDecisionsAsync(CancellationToken ct = default) =>
        Task.FromResult(DecisionResult ?? (IReadOnlyList<DecisionEntry>)[]);

    public Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken ct = default) =>
        Task.FromResult(SkillResult ?? (IReadOnlyList<Skill>)[]);

    public Task<DashboardData> GetDashboardAsync(CancellationToken ct = default) =>
        Task.FromResult(DashboardResult ?? new DashboardData(
            new TeamSummary(0, 0, []),
            [], [], [], []));
}

/// <summary>Helper to build ServiceProvider with stubs.</summary>
public static class StubServiceProviderFactory
{
    public static ServiceProvider Create(
        ITeamService? team = null,
        IDecisionService? decisions = null,
        ISkillService? skills = null,
        IOrchestrationLogService? log = null,
        ISquadDataProvider? squadData = null) =>
        new(
            squadData ?? new StubSquadDataProvider(),
            team ?? new StubTeamService(),
            decisions ?? new StubDecisionService(),
            skills ?? new StubSkillService(),
            log ?? new StubOrchestrationLogService());
}
