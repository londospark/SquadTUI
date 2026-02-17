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

    private string SquadDir => Path.Combine(_tempDir, ".ai-team");

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

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Equal(3, decisions.Count);
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

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        var d = decisions.First();
        Assert.Equal("Architecture Decision", d.Title);
        Assert.Equal("2026-02-16", d.Date);
        Assert.Equal("Danny", d.Author);
    }

    [Fact]
    public async Task ParseDecisionsMd_MissingDateAndAuthor_EmptyStrings()
    {
        WriteDecisionsFile("""
            # Decisions

            ## Simple Decision

            Just a decision with no metadata.
            """);

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        var d = decisions.First();
        Assert.Equal("Simple Decision", d.Title);
        Assert.Empty(d.Date);
        Assert.Empty(d.Author);
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

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Contains("This is the content of the decision.", decisions.First().Content);
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

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Equal(2, decisions.Count);
    }

    [Fact]
    public async Task HandleEmptyDecisionsMd_ReturnsEmpty()
    {
        WriteDecisionsFile("""
            # Decisions

            > Shared decision log.
            """);

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Empty(decisions);
    }

    [Fact]
    public async Task HandleMissingDecisionsMd_ReturnsEmpty()
    {
        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Empty(decisions);
    }

    [Fact]
    public async Task HandleMissingInboxDirectory_ReturnsEmpty()
    {
        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Empty(decisions);
    }

    [Fact]
    public async Task ExtractDateFromFilename_WhenNotInContent()
    {
        WriteInboxFile("copilot-directive-20260216-test.md", """
            # Some Decision

            **Author:** Danny

            Content without date field.
            """);

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.Equal("2026-02-16", decisions.First().Date);
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

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        var d = decisions.First();
        Assert.Equal("2026-02-17", d.Date);
        Assert.Equal("Saul", d.Author);
    }

    [Fact]
    public async Task ParseDecisionsMd_SetsFilePathAndLineNumber()
    {
        WriteDecisionsFile("""
            # Decisions

            ## First Decision

            Content.
            """);

        var svc = new DecisionService(SquadDir);
        var decisions = await svc.GetDecisionsAsync();

        Assert.NotNull(decisions.First().FilePath);
        Assert.True(decisions.First().LineNumber > 0);
    }
}
