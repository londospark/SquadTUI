using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;
using SquadTUI.Services;
using SquadTUI.Tests.Stubs;

namespace SquadTUI.Tests;

/// <summary>
/// P0 — Monadic error handling: Either&lt;AppError, T&gt; paths, GetOrEmpty(), AppError subtypes.
/// </summary>
public class ErrorHandlingTests
{
    #region AppError Hierarchy

    [Fact]
    public void FileNotFoundError_HasCorrectMessage()
    {
        var err = new FileNotFoundError("/path/to/file");
        Assert.Contains("/path/to/file", err.Message);
        Assert.Equal("/path/to/file", err.Path);
    }

    [Fact]
    public void ParseError_HasCorrectMessage()
    {
        var err = new ParseError("roster.yaml", "invalid yaml");
        Assert.Contains("roster.yaml", err.Message);
        Assert.Contains("invalid yaml", err.Message);
        Assert.Equal("roster.yaml", err.File);
        Assert.Equal("invalid yaml", err.Details);
    }

    [Fact]
    public void ServiceError_HasCorrectMessage()
    {
        var err = new ServiceError("TeamService", "connection timeout");
        Assert.Contains("TeamService", err.Message);
        Assert.Contains("connection timeout", err.Message);
        Assert.Equal("TeamService", err.Service);
        Assert.Equal("connection timeout", err.Details);
    }

    [Fact]
    public void NoDataError_HasCorrectMessage()
    {
        var err = new NoDataError("members");
        Assert.Contains("members", err.Message);
        Assert.Equal("members", err.DataType);
    }

    [Fact]
    public void AllErrorTypes_AreAppError()
    {
        AppError[] errors =
        [
            new FileNotFoundError("/x"),
            new ParseError("f", "d"),
            new ServiceError("s", "d"),
            new NoDataError("t"),
        ];
        foreach (var e in errors)
            Assert.IsAssignableFrom<AppError>(e);
    }

    #endregion

    #region GetOrEmpty Extension

    [Fact]
    public void GetOrEmpty_ReturnsData_WhenRight()
    {
        var members = new List<SquadMember>
        {
            new("Alice", "Lead", MemberStatus.Active),
        };
        Either<AppError, IReadOnlyList<SquadMember>> either =
            Right<AppError, IReadOnlyList<SquadMember>>(members);

        var result = either.GetOrEmpty();
        Assert.Equal(1, result.Count);
        Assert.Equal("Alice", result[0].Name);
    }

    [Fact]
    public void GetOrEmpty_ReturnsEmptyList_WhenLeft()
    {
        Either<AppError, IReadOnlyList<SquadMember>> either =
            Left<AppError, IReadOnlyList<SquadMember>>(new ServiceError("Test", "fail"));

        var result = either.GetOrEmpty();
        Assert.Empty(result);
    }

    [Fact]
    public void GetOrEmpty_WorksWithDecisions()
    {
        Either<AppError, IReadOnlyList<DecisionEntry>> either =
            Left<AppError, IReadOnlyList<DecisionEntry>>(new NoDataError("decisions"));

        Assert.Empty(either.GetOrEmpty());
    }

    [Fact]
    public void GetOrEmpty_WorksWithSkills()
    {
        var skills = new List<Skill> { new("testing", "Writes tests") };
        Either<AppError, IReadOnlyList<Skill>> either =
            Right<AppError, IReadOnlyList<Skill>>(skills);

        Assert.Equal(1, either.GetOrEmpty().Count);
    }

    [Fact]
    public void GetOrEmpty_WorksWithTasks()
    {
        Either<AppError, IReadOnlyList<SquadTask>> either =
            Left<AppError, IReadOnlyList<SquadTask>>(new FileNotFoundError("tasks.yaml"));

        Assert.Empty(either.GetOrEmpty());
    }

    [Fact]
    public void GetOrEmpty_WorksWithLogEntries()
    {
        Either<AppError, IReadOnlyList<OrchestrationLogEntry>> either =
            Left<AppError, IReadOnlyList<OrchestrationLogEntry>>(new ParseError("log.md", "bad format"));

        Assert.Empty(either.GetOrEmpty());
    }

    [Fact]
    public void GetOrEmpty_WorksWithSprintHistory()
    {
        Either<AppError, IReadOnlyList<SprintMetrics>> either =
            Left<AppError, IReadOnlyList<SprintMetrics>>(new NoDataError("sprints"));

        Assert.Empty(either.GetOrEmpty());
    }

    #endregion

    #region DataBridge Returns Either

    [Fact]
    public async Task DataBridge_LoadRoster_ReturnsLeft_WhenServiceThrows()
    {
        var teamService = new StubTeamService { RosterException = new InvalidOperationException("disk error") };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadRosterDataAsync();
        Assert.True(result.IsLeft);
        result.Match(
            Right: _ => throw new Exception("Should be Left"),
            Left: err =>
            {
                Assert.IsType<ServiceError>(err);
                Assert.Contains("disk error", err.Message);
            });
    }

    [Fact]
    public async Task DataBridge_LoadRoster_ReturnsRight_OnSuccess()
    {
        var teamService = new StubTeamService
        {
            RosterResult = new TeamRoster("TestProject", Members: new List<SquadMember>
            {
                new("Alice", "Dev", MemberStatus.Active),
            })
        };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadRosterDataAsync();
        Assert.True(result.IsRight);
        var data = result.GetOrEmpty();
        Assert.Equal(1, data.Count);
        Assert.Equal("Alice", data[0].Name);
    }

    [Fact]
    public async Task DataBridge_LoadDecisions_ReturnsLeft_WhenServiceThrows()
    {
        var decisionService = new StubDecisionService { Exception = new IOException("file locked") };
        var sp = StubServiceProviderFactory.Create(decisions: decisionService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadDecisionsDataAsync();
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task DataBridge_LoadDecisions_ReturnsRight_OnSuccess()
    {
        var decisionService = new StubDecisionService
        {
            Result = new List<DecisionEntry> { new("Use REST", "2026-03-01", "Alice", "REST chosen") }
        };
        var sp = StubServiceProviderFactory.Create(decisions: decisionService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadDecisionsDataAsync();
        Assert.True(result.IsRight);
        Assert.Equal(1, result.GetOrEmpty().Count);
    }

    [Fact]
    public async Task DataBridge_LoadSkills_ReturnsLeft_WhenServiceThrows()
    {
        var skillService = new StubSkillService { Exception = new Exception("parse error") };
        var sp = StubServiceProviderFactory.Create(skills: skillService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadSkillsDataAsync();
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task DataBridge_LoadLog_ReturnsLeft_WhenServiceThrows()
    {
        var logService = new StubOrchestrationLogService { Exception = new Exception("log error") };
        var sp = StubServiceProviderFactory.Create(log: logService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadLogDataAsync();
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task DataBridge_LoadTasks_ReturnsLeft_WhenServiceThrows()
    {
        var teamService = new StubTeamService { RosterException = new Exception("roster unavailable") };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadTasksFromRosterAsync();
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task DataBridge_LoadTasks_MapsStatusCorrectly()
    {
        var teamService = new StubTeamService
        {
            RosterResult = new TeamRoster("Test", Members: new List<SquadMember>
            {
                new("A", "Dev", MemberStatus.Working, "task-working"),
                new("B", "Dev", MemberStatus.Active, "task-active"),
                new("C", "Dev", MemberStatus.Idle, "task-idle"),
                new("D", "Dev", MemberStatus.Offline, "task-offline"),
                new("E", "Dev", MemberStatus.Active), // no current task — should be skipped
            }),
            CurrentTasksResult = new Dictionary<string, string>()
        };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadTasksFromRosterAsync();
        Assert.True(result.IsRight);
        var tasks = result.GetOrEmpty();
        Assert.Equal(4, tasks.Count);

        Assert.Equal(SquadTaskStatus.InProgress, tasks[0].Status);
        Assert.Equal(SquadTaskStatus.InProgress, tasks[1].Status);
        Assert.Equal(SquadTaskStatus.Pending, tasks[2].Status);
        Assert.Equal(SquadTaskStatus.Pending, tasks[3].Status);
    }

    #endregion

    #region AppState with Left errors
    // Individual WithLeft* tests consolidated to EmptyStateTests.AppState_LeftErrors_AllScreens_GetOrEmpty_ReturnEmptyLists

    [Fact]
    public void AppState_DefaultState_AllEithersAreRightEmpty()
    {
        var state = new AppState();
        Assert.True(state.Members.IsRight);
        Assert.True(state.Decisions.IsRight);
        Assert.True(state.Skills.IsRight);
        Assert.True(state.LogEntries.IsRight);
        Assert.True(state.Tasks.IsRight);
        Assert.True(state.SprintHistory.IsRight);

        Assert.Empty(state.Members.GetOrEmpty());
        Assert.Empty(state.Decisions.GetOrEmpty());
        Assert.Empty(state.Skills.GetOrEmpty());
        Assert.Empty(state.LogEntries.GetOrEmpty());
        Assert.Empty(state.Tasks.GetOrEmpty());
        Assert.Empty(state.SprintHistory.GetOrEmpty());
    }

    [Fact]
    public void AppState_CharterContent_DefaultsToNone()
    {
        var state = new AppState();
        Assert.True(state.CharterContent.IsNone);
    }

    #endregion

}
