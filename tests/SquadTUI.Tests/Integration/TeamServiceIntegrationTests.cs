using FluentAssertions;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class TeamServiceIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    [Fact]
    public async Task FullParse_FixtureTeamMd_AllMembersWithCorrectFields()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        roster.Members.Should().NotBeNull();
        // 5 members + 1 coordinator = 6
        roster.Members!.Count.Should().BeGreaterThanOrEqualTo(5);

        var names = roster.Members.Select(m => m.Name).ToList();
        names.Should().Contain("Danny");
        names.Should().Contain("Linus");
        names.Should().Contain("Rusty");
        names.Should().Contain("Basher");
        names.Should().Contain("Saul");
    }

    [Fact]
    public async Task FullParse_FixtureTeamMd_CoordinatorIncluded()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        roster.Members!.First().Name.Should().Be("Squad");
        roster.Members!.First().Role.Should().Be("Coordinator");
    }

    [Fact]
    public async Task FullParse_FixtureTeamMd_StatusesCorrectlyParsed()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        var danny = roster.Members!.First(m => m.Name == "Danny");
        danny.Status.Should().Be(MemberStatus.Active);

        var rusty = roster.Members!.First(m => m.Name == "Rusty");
        rusty.Status.Should().Be(MemberStatus.Working);

        var saul = roster.Members!.First(m => m.Name == "Saul");
        saul.Status.Should().Be(MemberStatus.Idle);
    }

    [Fact]
    public async Task FullParse_FixtureTeamMd_DescriptionPopulated()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        roster.Description.Should().NotBeNullOrEmpty();
        roster.Description.Should().Contain("AI squads");
    }

    [Fact]
    public async Task CharterPaths_ResolveToActualFixtureFiles()
    {
        var svc = new TeamService(GetFixturesPath());
        var roster = await svc.GetRosterAsync();

        var danny = roster.Members!.First(m => m.Name == "Danny");
        danny.CharterPath.Should().NotBeNull();
        File.Exists(danny.CharterPath).Should().BeTrue();
    }

    [Fact]
    public async Task GetMemberAsync_FindsDannyFromFixture()
    {
        var svc = new TeamService(GetFixturesPath());
        var member = await svc.GetMemberAsync("Danny");

        member.Should().NotBeNull();
        member!.Role.Should().Be("Lead");
    }
}
