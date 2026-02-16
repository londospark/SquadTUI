using FluentAssertions;
using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class OrchestrationLogServiceIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    [Fact]
    public async Task FullParse_FixtureLogFiles_CorrectEntries()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        entries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_DateParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        kickoff.Date.Should().Be("2026-02-16");
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_ParticipantsParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        kickoff.Participants.Should().Contain(["Danny", "Linus", "Rusty"]);
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_DecisionsParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        kickoff.Decisions.Should().Contain("Use Hex1b framework");
    }

    [Fact]
    public async Task FullParse_FixtureLogFile_OutcomesParsed()
    {
        var svc = new OrchestrationLogService(GetFixturesPath());
        var entries = await svc.GetEntriesAsync();

        var kickoff = entries.First(e => e.Topic == "kickoff");
        kickoff.Outcomes.Should().Contain("Repository initialized");
    }
}
