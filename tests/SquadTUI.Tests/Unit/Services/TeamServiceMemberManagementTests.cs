using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class TeamServiceMemberManagementTests : IDisposable
{
    private readonly string _tempDir;

    public TeamServiceMemberManagementTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_TSMM_" + Guid.NewGuid().ToString("N"));
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
        File.WriteAllText(Path.Combine(SquadDir, "team.md"), content);
    }

    private string ReadTeamFile()
    {
        return File.ReadAllText(Path.Combine(SquadDir, "team.md"));
    }

    private const string BasicTeamMd = """
        # Team Roster

        ## Members

        | Name | Role | Charter | Status |
        |------|------|---------|--------|
        | Danny | Lead | path | ✅ Active |
        | Linus | Frontend Dev | path | ✅ Active |

        ## Project Context

        - **Description:** Test project
        """;

    [Fact]
    public async Task AddMemberAsync_AddsRowToTeamMd()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.AddMemberAsync("Basher", "QA Engineer");

        var content = ReadTeamFile();
        Assert.Contains("Basher", content);
        Assert.Contains("QA Engineer", content);
    }

    [Fact]
    public async Task AddMemberAsync_CreatesAgentDirectory()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.AddMemberAsync("Basher", "QA Engineer");

        var agentDir = Path.Combine(SquadDir, "agents", "basher");
        Assert.True(Directory.Exists(agentDir));
    }

    [Fact]
    public async Task AddMemberAsync_CreatesCharterMd()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.AddMemberAsync("Basher", "QA Engineer");

        var charterPath = Path.Combine(SquadDir, "agents", "basher", "charter.md");
        Assert.True(File.Exists(charterPath));

        var charterContent = File.ReadAllText(charterPath);
        Assert.Contains("Basher", charterContent);
        Assert.Contains("QA Engineer", charterContent);
    }

    [Fact]
    public async Task AddMemberAsync_WithExistingMember_StillAdds()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.AddMemberAsync("Danny", "Lead");

        var content = ReadTeamFile();
        // Should contain Danny twice (original + added)
        var dannyCount = content.Split('\n').Count(l => l.Contains("| Danny |"));
        Assert.True(dannyCount >= 2);
    }

    [Fact]
    public async Task AddMemberAsync_DoesNotOverwriteExistingCharter()
    {
        WriteTeamFile(BasicTeamMd);
        var agentDir = Path.Combine(SquadDir, "agents", "basher");
        Directory.CreateDirectory(agentDir);
        File.WriteAllText(Path.Combine(agentDir, "charter.md"), "# Existing charter content");

        var svc = new TeamService(SquadDir);
        await svc.AddMemberAsync("Basher", "QA Engineer");

        var charterContent = File.ReadAllText(Path.Combine(agentDir, "charter.md"));
        Assert.Contains("Existing charter content", charterContent);
    }

    [Fact]
    public async Task RemoveMemberAsync_RemovesRowFromTeamMd()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.RemoveMemberAsync("Linus");

        var content = ReadTeamFile();
        Assert.DoesNotContain("| Linus |", content);
        Assert.Contains("| Danny |", content);
    }

    [Fact]
    public async Task RemoveMemberAsync_DeletesAgentDirectory()
    {
        WriteTeamFile(BasicTeamMd);
        var agentDir = Path.Combine(SquadDir, "agents", "linus");
        Directory.CreateDirectory(agentDir);
        File.WriteAllText(Path.Combine(agentDir, "charter.md"), "# Linus");

        var svc = new TeamService(SquadDir);
        await svc.RemoveMemberAsync("Linus");

        Assert.False(Directory.Exists(agentDir));
    }

    [Fact]
    public async Task RemoveMemberAsync_NonExistentMember_NoCrash()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        var ex = await Record.ExceptionAsync(() => svc.RemoveMemberAsync("Nobody"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task RemoveMemberAsync_WhenTeamMdDoesNotExist_NoCrash()
    {
        // Don't create team.md
        var svc = new TeamService(SquadDir);
        var ex = await Record.ExceptionAsync(() => svc.RemoveMemberAsync("Danny"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task AddMemberAsync_WhenTeamMdDoesNotExist_ThrowsInvalidOperation()
    {
        // Don't create team.md
        var svc = new TeamService(SquadDir);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.AddMemberAsync("Basher", "QA Engineer"));
    }

    [Fact]
    public async Task AddMemberAsync_WithSquadDir_UsesCorrectRelativePath()
    {
        // Create .squad directory instead of .ai-team
        var squadTempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_TSMM_Squad_" + Guid.NewGuid().ToString("N"));
        try
        {
            var squadDir = Path.Combine(squadTempDir, ".squad");
            Directory.CreateDirectory(squadDir);
            File.WriteAllText(Path.Combine(squadDir, "team.md"), BasicTeamMd);

            var svc = new TeamService(new FileLocationService(squadTempDir));
            await svc.AddMemberAsync("Saul", "Negotiator");

            var content = File.ReadAllText(Path.Combine(squadDir, "team.md"));
            Assert.Contains(".squad/agents/saul/charter.md", content);
        }
        finally
        {
            if (Directory.Exists(squadTempDir))
                Directory.Delete(squadTempDir, true);
        }
    }

    [Fact]
    public async Task AddMemberAsync_NewMemberAppearsInRoster()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.AddMemberAsync("Saul", "Negotiator");

        var roster = await svc.GetRosterAsync();
        Assert.Contains(roster.Members!, m => m.Name == "Saul");
    }

    [Fact]
    public async Task RemoveMemberAsync_MemberDisappearsFromRoster()
    {
        WriteTeamFile(BasicTeamMd);

        var svc = new TeamService(SquadDir);
        await svc.RemoveMemberAsync("Linus");

        var roster = await svc.GetRosterAsync();
        Assert.DoesNotContain(roster.Members!, m => m.Name == "Linus");
    }
}
