using LanguageExt;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class TeamServiceIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", ".ai-team");

    [Fact]
    public async Task FullParse_FixtureTeamMd_AllMembersWithCorrectFields()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        Assert.NotNull(roster.Members);
        // 5 members + 1 coordinator = 6
        Assert.True(roster.Members!.Count >= 5);

        var names = roster.Members.Select(m => m.Name).ToList();
        Assert.Contains("Danny", names);
        Assert.Contains("Linus", names);
        Assert.Contains("Rusty", names);
        Assert.Contains("Basher", names);
        Assert.Contains("Saul", names);
    }

    [Fact]
    public async Task FullParse_FixtureTeamMd_CoordinatorIncluded()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        Assert.Equal("Squad", roster.Members!.First().Name);
        Assert.Equal("Coordinator", roster.Members!.First().Role);
    }

    [Fact]
    public async Task FullParse_FixtureTeamMd_StatusesCorrectlyParsed()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        var danny = roster.Members!.First(m => m.Name == "Danny");
        Assert.Equal(MemberStatus.Active, danny.Status);

        var rusty = roster.Members!.First(m => m.Name == "Rusty");
        Assert.Equal(MemberStatus.Working, rusty.Status);

        var saul = roster.Members!.First(m => m.Name == "Saul");
        Assert.Equal(MemberStatus.Idle, saul.Status);
    }

    [Fact]
    public async Task FullParse_FixtureTeamMd_DescriptionPopulated()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        Assert.False(string.IsNullOrEmpty(roster.Description));
        Assert.Contains("AI squads", roster.Description);
    }

    [Fact]
    public async Task CharterPaths_ResolveToActualFixtureFiles()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        var danny = roster.Members!.First(m => m.Name == "Danny");
        Assert.True(danny.CharterPath.IsSome);
        danny.CharterPath.IfSome(p => Assert.True(File.Exists(p)));
    }

    [Fact]
    public async Task GetMemberAsync_FindsDannyFromFixture()
    {
        var svc = new TeamService(GetFixturesPath());
        var member = await svc.GetMemberAsync("Danny");

        Assert.NotNull(member);
        Assert.Equal("Lead", member!.Role);
    }
}
