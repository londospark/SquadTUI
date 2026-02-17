using FluentAssertions;
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
        decisions.Should().HaveCount(4);
    }

    [Fact]
    public async Task FullParse_FixtureDecisionsMd_TitlesExtracted()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        var titles = decisions.Select(d => d.Title).ToList();
        titles.Should().Contain("Project Architecture");
        titles.Should().Contain("UX Design");
        titles.Should().Contain("Testing Strategy");
    }

    [Fact]
    public async Task FullParse_FixtureDecisionsMd_AuthorsExtracted()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        var arch = decisions.First(d => d.Title == "Project Architecture");
        arch.Author.Should().Be("Danny");
        arch.Date.Should().Be("2026-02-16");
    }

    [Fact]
    public async Task FullParse_InboxDecision_ContentAccurate()
    {
        var svc = new DecisionService(GetFixturesPath());
        var decisions = await svc.GetDecisionsAsync();

        var inbox = decisions.First(d => d.Title == "Data Layer Design");
        inbox.Author.Should().Be("Rusty");
        inbox.Content.Should().Contain("File-based data providers");
    }
}
