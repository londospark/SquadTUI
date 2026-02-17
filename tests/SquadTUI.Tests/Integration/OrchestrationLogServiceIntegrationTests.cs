using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class OrchestrationLogServiceIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", ".ai-team");

    [Fact]
    public async Task FullParse_FixtureLogFiles_CorrectEntries()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        Assert.NotEmpty(entries);
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_DateParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        Assert.Equal("2026-02-16", kickoff.Date);
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_ParticipantsParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        Assert.Contains("Danny", kickoff.Participants);
        Assert.Contains("Linus", kickoff.Participants);
        Assert.Contains("Rusty", kickoff.Participants);
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_DecisionsParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        Assert.Contains("Use Hex1b framework", kickoff.Decisions);
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_OutcomesParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        Assert.Contains("Repository initialized", kickoff.Outcomes);
    }
}
