using SquadTUI.Models;
using SquadTUI.Services;
using SquadTUI.Tests.Stubs;

namespace SquadTUI.Tests.Unit.Services;

public class SquadDataProviderTests
{
    [Fact]
    public async Task GetDashboardAsync_AggregatesAllServices()
    {
        var members = new List<SquadMember>
        {
            new("Danny", "Lead", MemberStatus.Active),
            new("Linus", "Dev", MemberStatus.Working),
        };
        var roster = new TeamRoster("Test", "desc", members);
        var decisions = new List<DecisionEntry> { new("D1", "2026-01-01", "Danny", "Content") };
        var logEntries = new List<OrchestrationLogEntry>
        {
            new(DateTimeOffset.Now, "2026-01-01", "kickoff", ["Danny"], "Summary", [], [])
        };

        var teamService = new StubTeamService { RosterResult = roster };
        var decisionService = new StubDecisionService { Result = decisions };
        var logService = new StubOrchestrationLogService { Result = logEntries };

        var provider = new SquadDataProvider(teamService, logService, decisionService, new StubSkillService());
        var dashboard = await provider.GetDashboardAsync();

        Assert.NotNull(dashboard.Team);
        Assert.NotEmpty(dashboard.Decisions);
        Assert.NotEmpty(dashboard.Activity);
    }

    [Fact]
    public async Task GetDashboardAsync_ActiveMemberCountCalculatedCorrectly()
    {
        var members = new List<SquadMember>
        {
            new("Danny", "Lead", MemberStatus.Active),
            new("Linus", "Dev", MemberStatus.Working),
            new("Scribe", "Logger", MemberStatus.Idle),
        };
        var roster = new TeamRoster("Test", null, members);
        var teamService = new StubTeamService { RosterResult = roster };

        var provider = new SquadDataProvider(teamService, new StubOrchestrationLogService(), new StubDecisionService(), new StubSkillService());
        var dashboard = await provider.GetDashboardAsync();

        Assert.Equal(3, dashboard.Team.TotalMembers);
        Assert.Equal(2, dashboard.Team.ActiveMembers);
    }

    [Fact]
    public async Task GetDashboardAsync_SwimlanesBuiltFromLogEntries()
    {
        var members = new List<SquadMember>
        {
            new("Danny", "Lead", MemberStatus.Active),
        };
        var roster = new TeamRoster("Test", null, members);
        var logEntries = new List<OrchestrationLogEntry>
        {
            new(DateTimeOffset.Now, "2026-01-01", "kickoff", ["Danny"], "Summary", [], [])
        };

        var teamService = new StubTeamService { RosterResult = roster };
        var logService = new StubOrchestrationLogService { Result = logEntries };

        var provider = new SquadDataProvider(teamService, logService, new StubDecisionService(), new StubSkillService());
        var dashboard = await provider.GetDashboardAsync();

        Assert.Single(dashboard.Activity);
        Assert.Equal("Danny", dashboard.Activity[0].MemberName);
        Assert.Single(dashboard.Activity[0].LogEntries);
    }

    [Fact]
    public async Task GetRosterAsync_DelegatesToTeamService()
    {
        var roster = new TeamRoster("Test");
        var teamService = new StubTeamService { RosterResult = roster };

        var provider = new SquadDataProvider(teamService, new StubOrchestrationLogService(), new StubDecisionService(), new StubSkillService());
        var result = await provider.GetRosterAsync();

        Assert.Equal(roster, result);
    }

    [Fact]
    public async Task GetDecisionsAsync_DelegatesToDecisionService()
    {
        var decisions = new List<DecisionEntry> { new("D1", "2026-01-01", "Danny", "Content") };
        var decisionService = new StubDecisionService { Result = decisions };

        var provider = new SquadDataProvider(new StubTeamService(), new StubOrchestrationLogService(), decisionService, new StubSkillService());
        var result = await provider.GetDecisionsAsync();

        Assert.Equal(decisions, result);
    }

    [Fact]
    public async Task GetLogEntriesAsync_DelegatesToLogService()
    {
        var logs = new List<OrchestrationLogEntry>
        {
            new(DateTimeOffset.Now, "2026-01-01", "test", [], "Summary", [], [])
        };
        var logService = new StubOrchestrationLogService { Result = logs };

        var provider = new SquadDataProvider(new StubTeamService(), logService, new StubDecisionService(), new StubSkillService());
        var result = await provider.GetLogEntriesAsync();

        Assert.Equal(logs, result);
    }

    [Fact]
    public async Task GetSkillsAsync_DelegatesToSkillService()
    {
        var skills = new List<Skill> { new("test", "desc") };
        var skillService = new StubSkillService { Result = skills };

        var provider = new SquadDataProvider(new StubTeamService(), new StubOrchestrationLogService(), new StubDecisionService(), skillService);
        var result = await provider.GetSkillsAsync();

        Assert.Equal(skills, result);
    }
}
