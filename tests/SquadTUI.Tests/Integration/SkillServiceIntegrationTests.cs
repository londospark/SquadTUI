using FluentAssertions;
using SquadTUI.Services;

namespace SquadTUI.Tests.Integration;

public class SkillServiceIntegrationTests
{
    private static string GetFixturesPath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", ".ai-team");

    [Fact]
    public async Task FullParse_FixtureSkillsDirectory_CorrectSkills()
    {
        var svc = new SkillService(GetFixturesPath());
        var skills = await svc.GetSkillsAsync();

        skills.Should().ContainSingle();
    }

    [Fact]
    public async Task FullParse_FixtureSkill_FieldsExtracted()
    {
        var svc = new SkillService(GetFixturesPath());
        var skills = await svc.GetSkillsAsync();

        var skill = skills.First();
        skill.Name.Should().Be("test-skill");
        skill.Description.Should().Be("A skill for testing purposes");
        skill.Source.Should().Be("manual");
        skill.Confidence.Should().Be("high");
        skill.Slug.Should().Be("test-skill");
    }

    [Fact]
    public async Task FullParse_FixtureSkill_ContentCaptured()
    {
        var svc = new SkillService(GetFixturesPath());
        var skills = await svc.GetSkillsAsync();

        skills.First().Content.Should().Contain("integration tests");
    }

    [Fact]
    public async Task GetSkillAsync_ReturnsFixtureSkillBySlug()
    {
        var svc = new SkillService(GetFixturesPath());
        var skill = await svc.GetSkillAsync("test-skill");

        skill.Should().NotBeNull();
        skill!.Name.Should().Be("test-skill");
    }

    [Fact]
    public async Task GetSkillAsync_ReturnsNullForNonexistent()
    {
        var svc = new SkillService(GetFixturesPath());
        var skill = await svc.GetSkillAsync("nonexistent");

        skill.Should().BeNull();
    }
}
