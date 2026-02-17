using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class OrchestrationLogServiceTests : IDisposable
{
    private readonly string _tempDir;

    public OrchestrationLogServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempDir, ".ai-team", "log"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string SquadDir => Path.Combine(_tempDir, ".ai-team");

    private void WriteLogFile(string fileName, string content)
    {
        File.WriteAllText(Path.Combine(_tempDir, ".ai-team", "log", fileName), content);
    }

    [Fact]
    public async Task ParseLogFile_DateTopicFromFilename()
    {
        WriteLogFile("2026-02-16-kickoff.md", """
            # Project Kickoff

            ## Participants

            - Danny
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        var entry = entries.First();
        Assert.Equal("2026-02-16", entry.Date);
        Assert.Equal("kickoff", entry.Topic);
    }

    [Fact]
    public async Task ParseLogFile_ParticipantsSection()
    {
        WriteLogFile("2026-02-16-kickoff.md", """
            # Kickoff

            ## Participants

            - Danny
            - Linus
            - Rusty
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Equal(3, entries.First().Participants.Count);
        Assert.Contains("Danny", entries.First().Participants);
        Assert.Contains("Linus", entries.First().Participants);
        Assert.Contains("Rusty", entries.First().Participants);
    }

    [Fact]
    public async Task ParseLogFile_DecisionsSection()
    {
        WriteLogFile("2026-02-16-planning.md", """
            # Planning

            ## Decisions

            - Use Hex1b framework
            - Target .NET 10
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Equal(2, entries.First().Decisions.Count);
        Assert.Contains("Use Hex1b framework", entries.First().Decisions);
    }

    [Fact]
    public async Task ParseLogFile_OutcomesSection()
    {
        WriteLogFile("2026-02-16-review.md", """
            # Architecture Review

            ## Outcomes

            - Repository initialized
            - Team roster created
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Equal(2, entries.First().Outcomes.Count);
        Assert.Contains("Repository initialized", entries.First().Outcomes);
    }

    [Fact]
    public async Task HandleEmptyLogDirectory_ReturnsEmptyList()
    {
        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Empty(entries);
    }

    [Fact]
    public async Task HandleMissingLogDirectory_ReturnsEmptyList()
    {
        var emptyDir = Path.Combine(Path.GetTempPath(), "SquadTUI_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(emptyDir);
        try
        {
            var svc = new OrchestrationLogService(emptyDir);
            var entries = await svc.GetEntriesAsync();
            Assert.Empty(entries);
        }
        finally
        {
            Directory.Delete(emptyDir, true);
        }
    }

    [Fact]
    public async Task ResultsSortedByTimestampDescending()
    {
        WriteLogFile("2026-02-15-early.md", """
            # Early Session
            """);
        WriteLogFile("2026-02-17-later.md", """
            # Later Session
            """);
        WriteLogFile("2026-02-16-middle.md", """
            # Middle Session
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Equal(3, entries.Count);
        Assert.Equal("2026-02-17", entries[0].Date);
        Assert.Equal("2026-02-16", entries[1].Date);
        Assert.Equal("2026-02-15", entries[2].Date);
    }

    [Fact]
    public async Task GetEntriesByDateAsync_FiltersCorrectly()
    {
        WriteLogFile("2026-02-16-kickoff.md", """
            # Kickoff
            """);
        WriteLogFile("2026-02-17-planning.md", """
            # Planning
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesByDateAsync("2026-02-16");

        Assert.Single(entries);
        Assert.Equal("2026-02-16", entries.First().Date);
    }

    [Fact]
    public async Task ParseLogFile_SummaryFromH1()
    {
        WriteLogFile("2026-02-16-kickoff.md", """
            # Project Kickoff Session

            ## Participants

            - Danny
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Equal("Project Kickoff Session", entries.First().Summary);
    }

    [Fact]
    public async Task ParseLogFile_TopicFromFilename_MultiWordTopic()
    {
        WriteLogFile("2026-02-16-sprint-planning-session.md", """
            # Sprint Planning

            Content.
            """);

        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        Assert.Equal("sprint planning session", entries.First().Topic);
    }
}
