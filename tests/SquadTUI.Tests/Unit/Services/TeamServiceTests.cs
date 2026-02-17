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

        Assert.Equal(3, roster.Members.Count);
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

        Assert.Contains("Danny", roster.Members!.Select(m => m.Name));
        Assert.Contains("Linus", roster.Members!.Select(m => m.Name));
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

        Assert.Equal("Lead", roster.Members!.First().Role);
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
        Assert.Equal(MemberStatus.Active, members.First(m => m.Name == "Danny").Status);
        Assert.Equal(MemberStatus.Idle, members.First(m => m.Name == "Linus").Status);
        Assert.Equal(MemberStatus.Working, members.First(m => m.Name == "Rusty").Status);
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

        Assert.Equal(2, roster.Members.Count);
        Assert.Equal("Squad", roster.Members!.First().Name);
        Assert.Equal("Coordinator", roster.Members!.First().Role);
    }

    [Fact]
    public async Task HandleMissingTeamMd_ReturnsEmptyRoster()
    {
        var svc = new TeamService(SquadDir);
        var roster = await svc.GetRosterAsync();

        Assert.Null(roster.Members);
        Assert.Equal("SquadTUI", roster.ProjectName);
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

        Assert.NotNull(member);
        Assert.Equal("Danny", member!.Name);
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

        Assert.Null(member);
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

        Assert.Equal("A terminal user interface for managing AI squads", roster.Description);
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

        Assert.NotNull(roster.Members!.First().CharterPath);
        Assert.True(File.Exists(roster.Members!.First().CharterPath));
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

        Assert.Null(roster.Members!.First().CharterPath);
    }
}
