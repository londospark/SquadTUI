using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class SquadDetectorTests
{
    [Fact]
    public void HasValidSquad_WithTeamMd_ReturnsTrue()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squad-test-{Guid.NewGuid():N}");
        try
        {
            var aiTeamDir = Path.Combine(tempDir, ".ai-team");
            Directory.CreateDirectory(aiTeamDir);
            File.WriteAllText(Path.Combine(aiTeamDir, "team.md"), "# Team");

            Assert.True(SquadDetector.HasValidSquad(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void HasValidSquad_WithAgentsDir_ReturnsTrue()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squad-test-{Guid.NewGuid():N}");
        try
        {
            var agentsDir = Path.Combine(tempDir, ".ai-team", "agents");
            Directory.CreateDirectory(agentsDir);

            Assert.True(SquadDetector.HasValidSquad(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void HasValidSquad_WithEmptyDir_ReturnsFalse()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squad-test-{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(tempDir);
            Assert.False(SquadDetector.HasValidSquad(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void HasValidSquad_WithEmptyAiTeam_ReturnsFalse()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squad-test-{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(Path.Combine(tempDir, ".ai-team"));
            // No team.md and no agents dir
            Assert.False(SquadDetector.HasValidSquad(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }
}
