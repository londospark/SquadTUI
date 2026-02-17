using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class DecisionServiceIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", ".ai-team");

    [Fact]
    public async Task FullParse_FixtureDecisionsMdAndInbox_CombinedResults()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        // 3 from decisions.md + 1 from inbox
        Assert.Equal(4, decisions.Count);
    }

    [Fact]
    public async Task FullParse_FixtureDecisionsMd_TitlesExtracted()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        var titles = decisions.Select(d => d.Title).ToList();
        Assert.Contains("Project Architecture", titles);
        Assert.Contains("UX Design", titles);
        Assert.Contains("Testing Strategy", titles);
    }

    [Fact]
    public async Task FullParse_FixtureDecisionsMd_AuthorsExtracted()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        var arch = decisions.First(d => d.Title == "Project Architecture");
        Assert.Equal("Danny", arch.Author);
        Assert.Equal("2026-02-16", arch.Date);
    }

    [Fact]
    public async Task FullParse_InboxDecision_ContentAccurate()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        var inbox = decisions.First(d => d.Title == "Data Layer Design");
        Assert.Equal("Rusty", inbox.Author);
        Assert.Contains("File-based data providers", inbox.Content);
    }
}
