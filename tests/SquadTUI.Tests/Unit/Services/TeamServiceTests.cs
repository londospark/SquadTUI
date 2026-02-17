using FluentAssertions;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class TeamServiceTests : IDisposable
{
    private readonly string _tempDir;

    public TeamServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempDir, ".ai-team"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string SquadDir => Path.Combine(_tempDir, ".ai-team");

    private void WriteTeamFile(string content)
    {
        File.WriteAllText(Path.Combine(_tempDir, ".ai-team", "team.md"), content);
    }

    private void CreateCharterFile(string memberName)
    {
        var dir = Path.Combine(_tempDir, ".ai-team", "agents", memberName.ToLowerInvariant());
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "charter.md"), $"# {memberName} Charter");
    }

    [Fact]
    public async Task ParseValidTeamMd_ReturnsCorrectMemberCount()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | ✅ Active |
            | Linus | Frontend Dev | path | ✅ Active |
            | Rusty | Backend Dev | path | 🔄 Working |
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members.Should().HaveCount(3);
    }

    [Fact]
    public async Task ParseValidTeamMd_ReturnsCorrectNames()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            | Linus | Frontend Dev | path | Active |
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members!.Select(m => m.Name).Should().Contain(["Danny", "Linus"]);
    }

    [Fact]
    public async Task ParseValidTeamMd_ReturnsCorrectRoles()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members!.First().Role.Should().Be("Lead");
    }

    [Fact]
    public async Task ParseTeamMdWithEmojiStatuses_StatusCorrectlyMapped()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | ✅ Active |
            | Linus | Dev | path | 📋 Silent |
            | Rusty | Dev | path | 🔄 Working |
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        var members = roster.Members!;
        members.First(m => m.Name == "Danny").Status.Should().Be(MemberStatus.Active);
        members.First(m => m.Name == "Linus").Status.Should().Be(MemberStatus.Idle);
        members.First(m => m.Name == "Rusty").Status.Should().Be(MemberStatus.Working);
    }

    [Fact]
    public async Task ParseTeamMdWithCoordinator_CoordinatorIncluded()
    {
        WriteTeamFile("""
            # Team Roster

            ## Coordinator

            | Name | Role | Notes |
            |------|------|-------|
            | Squad | Coordinator | Routes work |

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members.Should().HaveCount(2);
        roster.Members!.First().Name.Should().Be("Squad");
        roster.Members!.First().Role.Should().Be("Coordinator");
    }

    [Fact]
    public async Task HandleMissingTeamMd_ReturnsEmptyRoster()
    {
        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members.Should().BeNull();
        roster.ProjectName.Should().Be("SquadTUI");
    }

    [Fact]
    public async Task GetMemberAsync_FindsByName_CaseInsensitive()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            """);

        var svc = new TeamService(SquadDir);
        var member = await svc.GetMemberAsync("danny");

        member.Should().NotBeNull();
        member!.Name.Should().Be("Danny");
    }

    [Fact]
    public async Task GetMemberAsync_ReturnsNullForUnknownMember()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            """);

        var svc = new TeamService(SquadDir);
        var member = await svc.GetMemberAsync("Unknown");

        member.Should().BeNull();
    }

    [Fact]
    public async Task ParseTeamMdWithProjectContext_DescriptionPopulated()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |

            ## Project Context

            - **Description:** A terminal user interface for managing AI squads
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Description.Should().Be("A terminal user interface for managing AI squads");
    }

    [Fact]
    public async Task ParseTeamMd_CharterPathResolvesWhenFileExists()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            """);
        CreateCharterFile("Danny");

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members!.First().CharterPath.Should().NotBeNull();
        File.Exists(roster.Members!.First().CharterPath).Should().BeTrue();
    }

    [Fact]
    public async Task ParseTeamMd_CharterPathNullWhenFileDoesNotExist()
    {
        WriteTeamFile("""
            # Team Roster

            ## Members

            | Name | Role | Charter | Status |
            |------|------|---------|--------|
            | Danny | Lead | path | Active |
            """);

        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        roster.Members!.First().CharterPath.Should().BeNull();
    }
}
