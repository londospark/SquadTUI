using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class TeamServiceTaskTests : IDisposable
{
    private readonly string _tempDir;

    public TeamServiceTaskTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_TST_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempDir, ".ai-team"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string SquadDir => Path.Combine(_tempDir, ".ai-team");

    private void CreateAgentHistory(string agentName, string content)
    {
        var dir = Path.Combine(SquadDir, "agents", agentName.ToLowerInvariant());
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "history.md"), content);
    }

    private void CreateInboxFile(string fileName, string content)
    {
        var dir = Path.Combine(SquadDir, "decisions", "inbox");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_WithHistoryHeadings_ReturnsLastHeading()
    {
        CreateAgentHistory("danny", """
            # Danny History

            ### First task completed
            Did something.

            ### Second task in progress
            Working on it.
            """);

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.True(tasks.ContainsKey("danny"));
        Assert.Equal("Second task in progress", tasks["danny"]);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_StripsDatePrefix_EmDash()
    {
        CreateAgentHistory("linus", """
            # Linus History

            ### 2026-02-18 — Refactored authentication module
            Details here.
            """);

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.True(tasks.ContainsKey("linus"));
        Assert.Equal("Refactored authentication module", tasks["linus"]);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_StripsDatePrefix_Colon()
    {
        CreateAgentHistory("rusty", """
            # Rusty History

            ### 2026-02-18: Fixed build pipeline
            Details here.
            """);

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.True(tasks.ContainsKey("rusty"));
        Assert.Equal("Fixed build pipeline", tasks["rusty"]);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_WithDecisionInbox_ExtractsAgentName()
    {
        // No history files, but inbox has agent-prefixed files
        var inboxDir = Path.Combine(SquadDir, "decisions", "inbox");
        Directory.CreateDirectory(inboxDir);
        File.WriteAllText(Path.Combine(inboxDir, "basher-review-api.md"), """
            # Review API endpoints
            Need to check all endpoints.
            """);

        // Create agents dir (empty) so the agents scan runs
        Directory.CreateDirectory(Path.Combine(SquadDir, "agents"));

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.True(tasks.ContainsKey("basher"));
        Assert.Equal("Review API endpoints", tasks["basher"]);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_WhenNoHistoryFiles_ReturnsEmpty()
    {
        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.Empty(tasks);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_WithEmptyHistoryFile_ReturnsNoTaskForAgent()
    {
        CreateAgentHistory("danny", "");

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.False(tasks.ContainsKey("danny"));
    }

    [Fact]
    public async Task GetCurrentTasksAsync_WithMultipleAgents_ReturnsAllTasks()
    {
        CreateAgentHistory("danny", """
            # Danny History
            ### Lead review session
            """);
        CreateAgentHistory("linus", """
            # Linus History
            ### Frontend refactor
            """);
        CreateAgentHistory("rusty", """
            # Rusty History
            ### Backend optimization
            """);

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.Equal(3, tasks.Count);
        Assert.Equal("Lead review session", tasks["danny"]);
        Assert.Equal("Frontend refactor", tasks["linus"]);
        Assert.Equal("Backend optimization", tasks["rusty"]);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_HistoryTakesPriorityOverInbox()
    {
        CreateAgentHistory("danny", """
            # Danny History
            ### From history file
            """);

        CreateInboxFile("danny-some-task.md", """
            # From inbox file
            """);

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.Equal("From history file", tasks["danny"]);
    }

    [Fact]
    public async Task GetCurrentTasksAsync_HistoryWithNoHeadings_ReturnsNoTask()
    {
        CreateAgentHistory("danny", """
            # Danny History

            Just some plain text without ### headings.
            More text.
            """);

        var svc = new TeamService(SquadDir);
        var tasks = await svc.GetCurrentTasksAsync();

        Assert.False(tasks.ContainsKey("danny"));
    }
}
