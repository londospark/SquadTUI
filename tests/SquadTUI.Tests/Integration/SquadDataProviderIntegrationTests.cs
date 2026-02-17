using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class SquadDataProviderIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", ".ai-team");

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

        Assert.NotNull(dashboard.Team);
        Assert.True(dashboard.Team.TotalMembers > 0);
        Assert.NotEmpty(dashboard.Decisions);
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

        Assert.NotNull(roster.Members);
        Assert.Contains("Danny", roster.Members!.Select(m => m.Name));
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
        Assert.True(dashboard.Team.ActiveMembers > 0);
        Assert.True(dashboard.Team.ActiveMembers <= dashboard.Team.TotalMembers);
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

        Assert.NotEmpty(skills);
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

        Assert.NotEmpty(logs);
    }
}
