using FluentAssertions;
using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class SquadDataProviderIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    [Fact]
    public async Task EndToEnd_FixtureFiles_DashboardDataPopulated()
    {
        var fixturesPath = GetFixturesPath();
        var teamService = new TeamService(fixturesPath);
        var logService = new OrchestrationLogService(fixturesPath);
        var decisionService = new DecisionService(fixturesPath);
        var skillService = new SkillService(fixturesPath);
        var provider = new SquadDataProvider(teamService, logService, decisionService, skillService);

        var dashboard = await provider.GetDashboardAsync();

        dashboard.Team.Should().NotBeNull();
        dashboard.Team.TotalMembers.Should().BeGreaterThan(0);
        dashboard.Decisions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task EndToEnd_FixtureFiles_RosterPopulated()
    {
        var fixturesPath = GetFixturesPath();
        var teamService = new TeamService(fixturesPath);
        var logService = new OrchestrationLogService(fixturesPath);
        var decisionService = new DecisionService(fixturesPath);
        var skillService = new SkillService(fixturesPath);
        var provider = new SquadDataProvider(teamService, logService, decisionService, skillService);

        var roster = await provider.GetRosterAsync();

        roster.Members.Should().NotBeNull();
        roster.Members!.Select(m => m.Name).Should().Contain("Danny");
    }

    [Fact]
    public async Task EndToEnd_FixtureFiles_ActiveMemberCount()
    {
        var fixturesPath = GetFixturesPath();
        var teamService = new TeamService(fixturesPath);
        var logService = new OrchestrationLogService(fixturesPath);
        var decisionService = new DecisionService(fixturesPath);
        var skillService = new SkillService(fixturesPath);
        var provider = new SquadDataProvider(teamService, logService, decisionService, skillService);

        var dashboard = await provider.GetDashboardAsync();

        // Active + Working members
        dashboard.Team.ActiveMembers.Should().BeGreaterThan(0);
        dashboard.Team.ActiveMembers.Should().BeLessThanOrEqualTo(dashboard.Team.TotalMembers);
    }

    [Fact]
    public async Task EndToEnd_FixtureFiles_SkillsPopulated()
    {
        var fixturesPath = GetFixturesPath();
        var teamService = new TeamService(fixturesPath);
        var logService = new OrchestrationLogService(fixturesPath);
        var decisionService = new DecisionService(fixturesPath);
        var skillService = new SkillService(fixturesPath);
        var provider = new SquadDataProvider(teamService, logService, decisionService, skillService);

        var skills = await provider.GetSkillsAsync();

        skills.Should().NotBeEmpty();
    }

    [Fact]
    public async Task EndToEnd_FixtureFiles_LogEntriesPopulated()
    {
        var fixturesPath = GetFixturesPath();
        var teamService = new TeamService(fixturesPath);
        var logService = new OrchestrationLogService(fixturesPath);
        var decisionService = new DecisionService(fixturesPath);
        var skillService = new SkillService(fixturesPath);
        var provider = new SquadDataProvider(teamService, logService, decisionService, skillService);

        var logs = await provider.GetLogEntriesAsync();

        logs.Should().NotBeEmpty();
    }
}
