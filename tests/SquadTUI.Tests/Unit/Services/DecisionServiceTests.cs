using FluentAssertions;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class DecisionServiceTests : IDisposable
{
    private readonly string _tempDir;

    public DecisionServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempDir, ".ai-team"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private void WriteDecisionsFile(string content)
    {
        File.WriteAllText(Path.Combine(_tempDir, ".ai-team", "decisions.md"), content);
    }

    private void WriteInboxFile(string fileName, string content)
    {
        var dir = Path.Combine(_tempDir, ".ai-team", "decisions", "inbox");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }

    [Fact]
    public async Task ParseDecisionsMd_MultipleSections_CorrectCount()
    {
        WriteDecisionsFile("""
            # Decisions

            ## Architecture

            **Date:** 2026-02-16
            **Author:** Danny

            Use clean architecture.

            ## UX Design

            **Date:** 2026-02-16
            **Author:** Saul

            Navigation with number keys.

            ## Testing

            **Date:** 2026-02-17
            **Author:** Basher

            Unit and integration tests.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.Should().HaveCount(3);
    }

    [Fact]
    public async Task ParseDecisionsMd_ExtractsTitleDateAuthor()
    {
        WriteDecisionsFile("""
            # Decisions

            ## Architecture Decision

            **Date:** 2026-02-16
            **Author:** Danny

            Use Hex1b for TUI.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        var d = decisions.First();
        d.Title.Should().Be("Architecture Decision");
        d.Date.Should().Be("2026-02-16");
        d.Author.Should().Be("Danny");
    }

    [Fact]
    public async Task ParseDecisionsMd_MissingDateAndAuthor_EmptyStrings()
    {
        WriteDecisionsFile("""
            # Decisions

            ## Simple Decision

            Just a decision with no metadata.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        var d = decisions.First();
        d.Title.Should().Be("Simple Decision");
        d.Date.Should().BeEmpty();
        d.Author.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseDecisionsMd_ExtractsContent()
    {
        WriteDecisionsFile("""
            # Decisions

            ## My Decision

            **Date:** 2026-02-16
            **Author:** Danny

            This is the content of the decision.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.First().Content.Should().Contain("This is the content of the decision.");
    }

    [Fact]
    public async Task ParseInboxDirectory_MultipleFiles()
    {
        WriteInboxFile("decision-a.md", """
            # Decision A

            **Date:** 2026-02-16
            **Author:** Danny

            Content A.
            """);
        WriteInboxFile("decision-b.md", """
            # Decision B

            **Author:** Rusty

            Content B.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleEmptyDecisionsMd_ReturnsEmpty()
    {
        WriteDecisionsFile("""
            # Decisions

            > Shared decision log.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleMissingDecisionsMd_ReturnsEmpty()
    {
        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleMissingInboxDirectory_ReturnsEmpty()
    {
        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractDateFromFilename_WhenNotInContent()
    {
        WriteInboxFile("copilot-directive-20260216-test.md", """
            # Some Decision

            **Author:** Danny

            Content without date field.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.First().Date.Should().Be("2026-02-16");
    }

    [Fact]
    public async Task ParseDecisionsMd_BoldFieldsWithDashPrefix()
    {
        WriteDecisionsFile("""
            # Decisions

            ## Decision With Dashes

            - **Date:** 2026-02-17
            - **Author:** Saul

            Content here.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        var d = decisions.First();
        d.Date.Should().Be("2026-02-17");
        d.Author.Should().Be("Saul");
    }

    [Fact]
    public async Task ParseDecisionsMd_SetsFilePathAndLineNumber()
    {
        WriteDecisionsFile("""
            # Decisions

            ## First Decision

            Content.
            """);

        var svc = new DecisionService(_tempDir);
        var decisions = await svc.GetDecisionsAsync();

        decisions.First().FilePath.Should().NotBeNull();
        decisions.First().LineNumber.Should().BeGreaterThan(0);
    }
}
