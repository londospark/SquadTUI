using FluentAssertions;
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
        entry.Date.Should().Be("2026-02-16");
        entry.Topic.Should().Be("kickoff");
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

        entries.First().Participants.Should().HaveCount(3);
        entries.First().Participants.Should().Contain(["Danny", "Linus", "Rusty"]);
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

        entries.First().Decisions.Should().HaveCount(2);
        entries.First().Decisions.Should().Contain("Use Hex1b framework");
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

        entries.First().Outcomes.Should().HaveCount(2);
        entries.First().Outcomes.Should().Contain("Repository initialized");
    }

    [Fact]
    public async Task HandleEmptyLogDirectory_ReturnsEmptyList()
    {
        var svc = new OrchestrationLogService(SquadDir);
        var entries = await svc.GetEntriesAsync();

        entries.Should().BeEmpty();
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
            entries.Should().BeEmpty();
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

        entries.Should().HaveCount(3);
        entries[0].Date.Should().Be("2026-02-17");
        entries[1].Date.Should().Be("2026-02-16");
        entries[2].Date.Should().Be("2026-02-15");
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

        entries.Should().ContainSingle();
        entries.First().Date.Should().Be("2026-02-16");
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

        entries.First().Summary.Should().Be("Project Kickoff Session");
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

        entries.First().Topic.Should().Be("sprint planning session");
    }
}
