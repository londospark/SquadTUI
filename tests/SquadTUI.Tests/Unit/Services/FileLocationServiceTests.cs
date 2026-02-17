using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class FileLocationServiceTests : IDisposable
{
    private readonly string _tempDir;

    public FileLocationServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_FLS_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private FileLocationService CreateWithSquadDir()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, ".squad"));
        return new FileLocationService(_tempDir);
    }

    private FileLocationService CreateWithLegacyDir()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, ".ai-team"));
        return new FileLocationService(_tempDir);
    }

    [Fact]
    public void GetRosterPath_ReturnsTeamMdInsideSquadDir()
    {
        var svc = CreateWithSquadDir();
        var result = svc.GetRosterPath();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "team.md"), result);
    }

    [Fact]
    public void GetRosterPath_UsesLegacyDir_WhenNoSquadDir()
    {
        var svc = CreateWithLegacyDir();
        var result = svc.GetRosterPath();
        Assert.Equal(Path.Combine(_tempDir, ".ai-team", "team.md"), result);
    }

    [Theory]
    [InlineData("Danny")]
    [InlineData("Linus")]
    [InlineData("RUSTY")]
    public void GetCharterPath_ReturnsLowercasedAgentCharter(string agentName)
    {
        var svc = CreateWithSquadDir();
        var result = svc.GetCharterPath(agentName);
        var expected = Path.Combine(_tempDir, ".squad", "agents", agentName.ToLowerInvariant(), "charter.md");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetDecisionsFilePath_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "decisions.md"), svc.GetDecisionsFilePath());
    }

    [Fact]
    public void GetDecisionsInboxPath_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "decisions", "inbox"), svc.GetDecisionsInboxPath());
    }

    [Fact]
    public void GetSkillsDirectory_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "skills"), svc.GetSkillsDirectory());
    }

    [Fact]
    public void GetSkillFilePath_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        var result = svc.GetSkillFilePath("code-review");
        Assert.Equal(Path.Combine(_tempDir, ".squad", "skills", "code-review", "SKILL.md"), result);
    }

    [Fact]
    public void GetLogDirectory_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "log"), svc.GetLogDirectory());
    }

    [Fact]
    public void GetOrchestrationLogDirectory_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "orchestration-log"), svc.GetOrchestrationLogDirectory());
    }

    [Fact]
    public void GetSettingsDirectory_ReturnsUserProfilePath()
    {
        var svc = CreateWithSquadDir();
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config", "squadtui");
        Assert.Equal(expected, svc.GetSettingsDirectory());
    }

    [Fact]
    public void GetSettingsFilePath_ReturnsJsonInsideSettingsDir()
    {
        var svc = CreateWithSquadDir();
        var result = svc.GetSettingsFilePath();
        Assert.EndsWith("settings.json", result);
        Assert.Contains(".config", result);
    }

    [Fact]
    public void ActiveDirectoryName_ReturnsSquad_WhenSquadExists()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(".squad", svc.ActiveDirectoryName);
    }

    [Fact]
    public void ActiveDirectoryName_ReturnsAiTeam_WhenOnlyLegacyExists()
    {
        var svc = CreateWithLegacyDir();
        Assert.Equal(".ai-team", svc.ActiveDirectoryName);
    }

    [Fact]
    public void NeedsMigration_False_WhenSquadDirExists()
    {
        var svc = CreateWithSquadDir();
        Assert.False(svc.NeedsMigration);
    }

    [Fact]
    public void NeedsMigration_True_WhenOnlyLegacyDirExists()
    {
        var svc = CreateWithLegacyDir();
        Assert.True(svc.NeedsMigration);
    }

    [Fact]
    public void HasSquadDirectory_True_WhenSquadDirExists()
    {
        var svc = CreateWithSquadDir();
        Assert.True(svc.HasSquadDirectory);
    }

    [Fact]
    public void HasSquadDirectory_True_WhenLegacyDirExists()
    {
        var svc = CreateWithLegacyDir();
        Assert.True(svc.HasSquadDirectory);
    }

    [Fact]
    public void HasSquadDirectory_False_WhenNeitherExists()
    {
        var svc = new FileLocationService(_tempDir);
        Assert.False(svc.HasSquadDirectory);
    }

    [Fact]
    public void GetAgentsDirectory_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad", "agents"), svc.GetAgentsDirectory());
    }

    [Fact]
    public void GetHistoryPath_ReturnsCorrectPath()
    {
        var svc = CreateWithSquadDir();
        var result = svc.GetHistoryPath("Danny");
        Assert.Equal(Path.Combine(_tempDir, ".squad", "agents", "danny", "history.md"), result);
    }

    [Fact]
    public void GetAgentDirectory_ReturnsLowercasedPath()
    {
        var svc = CreateWithSquadDir();
        var result = svc.GetAgentDirectory("Danny");
        Assert.Equal(Path.Combine(_tempDir, ".squad", "agents", "danny"), result);
    }

    [Fact]
    public void FromSquadDirectory_CreatesServiceWithCorrectPaths()
    {
        var squadDir = Path.Combine(_tempDir, ".squad");
        Directory.CreateDirectory(squadDir);
        var svc = FileLocationService.FromSquadDirectory(squadDir);

        Assert.Equal(squadDir, svc.SquadDirectory);
        Assert.Equal(_tempDir, svc.ProjectRoot);
    }

    [Fact]
    public void ProjectRoot_IsSetCorrectly()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(_tempDir, svc.ProjectRoot);
    }

    [Fact]
    public void SquadDirectory_ResolvedCorrectly_ForSquadDir()
    {
        var svc = CreateWithSquadDir();
        Assert.Equal(Path.Combine(_tempDir, ".squad"), svc.SquadDirectory);
    }

    [Fact]
    public void SquadDirectory_ResolvedCorrectly_ForLegacyDir()
    {
        var svc = CreateWithLegacyDir();
        Assert.Equal(Path.Combine(_tempDir, ".ai-team"), svc.SquadDirectory);
    }
}
