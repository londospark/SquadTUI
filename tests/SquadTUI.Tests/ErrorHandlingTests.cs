using FluentAssertions;
using LanguageExt;
using static LanguageExt.Prelude;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SquadTUI.Models;
using SquadTUI.Screens;
using SquadTUI.Services;

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
        err.Message.Should().Contain("/path/to/file");
        err.Path.Should().Be("/path/to/file");
    }

    [Fact]
    public void ParseError_HasCorrectMessage()
    {
        var err = new ParseError("roster.yaml", "invalid yaml");
        err.Message.Should().Contain("roster.yaml");
        err.Message.Should().Contain("invalid yaml");
        err.File.Should().Be("roster.yaml");
        err.Details.Should().Be("invalid yaml");
    }

    [Fact]
    public void ServiceError_HasCorrectMessage()
    {
        var err = new ServiceError("TeamService", "connection timeout");
        err.Message.Should().Contain("TeamService");
        err.Message.Should().Contain("connection timeout");
        err.Service.Should().Be("TeamService");
        err.Details.Should().Be("connection timeout");
    }

    [Fact]
    public void NoDataError_HasCorrectMessage()
    {
        var err = new NoDataError("members");
        err.Message.Should().Contain("members");
        err.DataType.Should().Be("members");
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
            e.Should().BeAssignableTo<AppError>();
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
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Alice");
    }

    [Fact]
    public void GetOrEmpty_ReturnsEmptyList_WhenLeft()
    {
        Either<AppError, IReadOnlyList<SquadMember>> either =
            Left<AppError, IReadOnlyList<SquadMember>>(new ServiceError("Test", "fail"));

        var result = either.GetOrEmpty();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOrEmpty_WorksWithDecisions()
    {
        Either<AppError, IReadOnlyList<DecisionEntry>> either =
            Left<AppError, IReadOnlyList<DecisionEntry>>(new NoDataError("decisions"));

        either.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void GetOrEmpty_WorksWithSkills()
    {
        var skills = new List<Skill> { new("testing", "Writes tests") };
        Either<AppError, IReadOnlyList<Skill>> either =
            Right<AppError, IReadOnlyList<Skill>>(skills);

        either.GetOrEmpty().Should().HaveCount(1);
    }

    [Fact]
    public void GetOrEmpty_WorksWithTasks()
    {
        Either<AppError, IReadOnlyList<SquadTask>> either =
            Left<AppError, IReadOnlyList<SquadTask>>(new FileNotFoundError("tasks.yaml"));

        either.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void GetOrEmpty_WorksWithLogEntries()
    {
        Either<AppError, IReadOnlyList<OrchestrationLogEntry>> either =
            Left<AppError, IReadOnlyList<OrchestrationLogEntry>>(new ParseError("log.md", "bad format"));

        either.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void GetOrEmpty_WorksWithSprintHistory()
    {
        Either<AppError, IReadOnlyList<SprintMetrics>> either =
            Left<AppError, IReadOnlyList<SprintMetrics>>(new NoDataError("sprints"));

        either.GetOrEmpty().Should().BeEmpty();
    }

    #endregion

    #region DataBridge Returns Either

    [Fact]
    public async Task DataBridge_LoadRoster_ReturnsLeft_WhenServiceThrows()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("disk error"));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadRosterDataAsync();
        result.IsLeft.Should().BeTrue("DataBridge should return Left on service exception");
        result.Match(
            Right: _ => throw new Exception("Should be Left"),
            Left: err =>
            {
                err.Should().BeOfType<ServiceError>();
                err.Message.Should().Contain("disk error");
            });
    }

    [Fact]
    public async Task DataBridge_LoadRoster_ReturnsRight_OnSuccess()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .Returns(new TeamRoster("TestProject", Members: new List<SquadMember>
            {
                new("Alice", "Dev", MemberStatus.Active),
            }));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadRosterDataAsync();
        result.IsRight.Should().BeTrue();
        var data = result.GetOrEmpty();
        data.Should().HaveCount(1);
        data[0].Name.Should().Be("Alice");
    }

    [Fact]
    public async Task DataBridge_LoadDecisions_ReturnsLeft_WhenServiceThrows()
    {
        var decisionService = Substitute.For<IDecisionService>();
        decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("file locked"));

        var sp = CreateServiceProvider(decisionService: decisionService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadDecisionsDataAsync();
        result.IsLeft.Should().BeTrue();
    }

    [Fact]
    public async Task DataBridge_LoadDecisions_ReturnsRight_OnSuccess()
    {
        var decisionService = Substitute.For<IDecisionService>();
        decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<DecisionEntry>
            {
                new("Use REST", "2026-03-01", "Alice", "REST chosen"),
            });

        var sp = CreateServiceProvider(decisionService: decisionService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadDecisionsDataAsync();
        result.IsRight.Should().BeTrue();
        result.GetOrEmpty().Should().HaveCount(1);
    }

    [Fact]
    public async Task DataBridge_LoadSkills_ReturnsLeft_WhenServiceThrows()
    {
        var skillService = Substitute.For<ISkillService>();
        skillService.GetSkillsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("parse error"));

        var sp = CreateServiceProvider(skillService: skillService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadSkillsDataAsync();
        result.IsLeft.Should().BeTrue();
    }

    [Fact]
    public async Task DataBridge_LoadLog_ReturnsLeft_WhenServiceThrows()
    {
        var logService = Substitute.For<IOrchestrationLogService>();
        logService.GetEntriesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("log error"));

        var sp = CreateServiceProvider(logService: logService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadLogDataAsync();
        result.IsLeft.Should().BeTrue();
    }

    [Fact]
    public async Task DataBridge_LoadTasks_ReturnsLeft_WhenServiceThrows()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("roster unavailable"));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadTasksFromRosterAsync();
        result.IsLeft.Should().BeTrue();
    }

    [Fact]
    public async Task DataBridge_LoadTasks_MapsStatusCorrectly()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .Returns(new TeamRoster("Test", Members: new List<SquadMember>
            {
                new("A", "Dev", MemberStatus.Working, "task-working"),
                new("B", "Dev", MemberStatus.Active, "task-active"),
                new("C", "Dev", MemberStatus.Idle, "task-idle"),
                new("D", "Dev", MemberStatus.Offline, "task-offline"),
                new("E", "Dev", MemberStatus.Active), // no current task — should be skipped
            }));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadTasksFromRosterAsync();
        result.IsRight.Should().BeTrue();
        var tasks = result.GetOrEmpty();
        tasks.Should().HaveCount(4, "member E has no CurrentTask and should be skipped");

        tasks[0].Status.Should().Be(SquadTaskStatus.InProgress, "Working → InProgress");
        tasks[1].Status.Should().Be(SquadTaskStatus.InProgress, "Active → InProgress");
        tasks[2].Status.Should().Be(SquadTaskStatus.Pending, "Idle → Pending");
        tasks[3].Status.Should().Be(SquadTaskStatus.Pending, "Offline → Pending");
    }

    #endregion

    #region AppState with Left errors

    [Fact]
    public void AppState_WithLeftMembers_GetOrEmptyReturnsEmpty()
    {
        var state = new AppState
        {
            Members = Left<AppError, IReadOnlyList<SquadMember>>(new ServiceError("Roster", "fail")),
        };
        state.Members.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_WithLeftDecisions_GetOrEmptyReturnsEmpty()
    {
        var state = new AppState
        {
            Decisions = Left<AppError, IReadOnlyList<DecisionEntry>>(new NoDataError("decisions")),
        };
        state.Decisions.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_WithLeftTasks_GetOrEmptyReturnsEmpty()
    {
        var state = new AppState
        {
            Tasks = Left<AppError, IReadOnlyList<SquadTask>>(new FileNotFoundError("tasks.yaml")),
        };
        state.Tasks.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_DefaultState_AllEithersAreRightEmpty()
    {
        var state = new AppState();
        state.Members.IsRight.Should().BeTrue();
        state.Decisions.IsRight.Should().BeTrue();
        state.Skills.IsRight.Should().BeTrue();
        state.LogEntries.IsRight.Should().BeTrue();
        state.Tasks.IsRight.Should().BeTrue();
        state.SprintHistory.IsRight.Should().BeTrue();

        state.Members.GetOrEmpty().Should().BeEmpty();
        state.Decisions.GetOrEmpty().Should().BeEmpty();
        state.Skills.GetOrEmpty().Should().BeEmpty();
        state.LogEntries.GetOrEmpty().Should().BeEmpty();
        state.Tasks.GetOrEmpty().Should().BeEmpty();
        state.SprintHistory.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_CharterContent_DefaultsToNone()
    {
        var state = new AppState();
        state.CharterContent.IsNone.Should().BeTrue();
    }

    #endregion

    #region Helpers

    private static ServiceProvider CreateServiceProvider(
        ITeamService? teamService = null,
        IDecisionService? decisionService = null,
        ISkillService? skillService = null,
        IOrchestrationLogService? logService = null)
    {
        teamService ??= Substitute.For<ITeamService>();
        decisionService ??= Substitute.For<IDecisionService>();
        skillService ??= Substitute.For<ISkillService>();
        logService ??= Substitute.For<IOrchestrationLogService>();

        var squadData = Substitute.For<ISquadDataProvider>();
        return new ServiceProvider(squadData, teamService, decisionService, skillService, logService);
    }

    #endregion
}
