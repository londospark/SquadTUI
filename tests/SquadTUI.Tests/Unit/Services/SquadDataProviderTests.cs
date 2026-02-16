using FluentAssertions;
using NSubstitute;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class SquadDataProviderTests
{
    private readonly ITeamService _teamService = Substitute.For<ITeamService>();
    private readonly IOrchestrationLogService _logService = Substitute.For<IOrchestrationLogService>();
    private readonly IDecisionService _decisionService = Substitute.For<IDecisionService>();
    private readonly ISkillService _skillService = Substitute.For<ISkillService>();
    private readonly SquadDataProvider _provider;

    public SquadDataProviderTests()
    {
        _provider = new SquadDataProvider(_teamService, _logService, _decisionService, _skillService);
    }

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

        _teamService.GetRosterAsync(Arg.Any<CancellationToken>()).Returns(roster);
        _decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>()).Returns(decisions);
        _logService.GetEntriesAsync(Arg.Any<CancellationToken>()).Returns(logEntries);

        var dashboard = await _provider.GetDashboardAsync();

        dashboard.Team.Should().NotBeNull();
        dashboard.Decisions.Should().NotBeEmpty();
        dashboard.Activity.Should().NotBeEmpty();
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

        _teamService.GetRosterAsync(Arg.Any<CancellationToken>()).Returns(roster);
        _decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>()).Returns(new List<DecisionEntry>());
        _logService.GetEntriesAsync(Arg.Any<CancellationToken>()).Returns(new List<OrchestrationLogEntry>());

        var dashboard = await _provider.GetDashboardAsync();

        dashboard.Team.TotalMembers.Should().Be(3);
        dashboard.Team.ActiveMembers.Should().Be(2); // Active + Working
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

        _teamService.GetRosterAsync(Arg.Any<CancellationToken>()).Returns(roster);
        _decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>()).Returns(new List<DecisionEntry>());
        _logService.GetEntriesAsync(Arg.Any<CancellationToken>()).Returns(logEntries);

        var dashboard = await _provider.GetDashboardAsync();

        dashboard.Activity.Should().ContainSingle();
        dashboard.Activity[0].MemberName.Should().Be("Danny");
        dashboard.Activity[0].LogEntries.Should().ContainSingle();
    }

    [Fact]
    public async Task GetRosterAsync_DelegatesToTeamService()
    {
        var roster = new TeamRoster("Test");
        _teamService.GetRosterAsync(Arg.Any<CancellationToken>()).Returns(roster);

        var result = await _provider.GetRosterAsync();

        result.Should().Be(roster);
    }

    [Fact]
    public async Task GetDecisionsAsync_DelegatesToDecisionService()
    {
        var decisions = new List<DecisionEntry> { new("D1", "2026-01-01", "Danny", "Content") };
        _decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>()).Returns(decisions);

        var result = await _provider.GetDecisionsAsync();

        result.Should().BeEquivalentTo(decisions);
    }

    [Fact]
    public async Task GetLogEntriesAsync_DelegatesToLogService()
    {
        var logs = new List<OrchestrationLogEntry>
        {
            new(DateTimeOffset.Now, "2026-01-01", "test", [], "Summary", [], [])
        };
        _logService.GetEntriesAsync(Arg.Any<CancellationToken>()).Returns(logs);

        var result = await _provider.GetLogEntriesAsync();

        result.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public async Task GetSkillsAsync_DelegatesToSkillService()
    {
        var skills = new List<Skill> { new("test", "desc") };
        _skillService.GetSkillsAsync(Arg.Any<CancellationToken>()).Returns(skills);

        var result = await _provider.GetSkillsAsync();

        result.Should().BeEquivalentTo(skills);
    }
}
