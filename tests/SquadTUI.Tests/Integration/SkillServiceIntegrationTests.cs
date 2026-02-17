using LanguageExt;
using static LanguageExt.Prelude;
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

        Assert.Single(skills);
    }

    [Fact]
    public async Task FullParse_FixtureSkill_FieldsExtracted()
    {
        var svc = new SkillService(GetFixturesPath());
        var skills = await svc.GetSkillsAsync();

        var skill = skills.First();
        Assert.Equal("test-skill", skill.Name);
        Assert.Equal("A skill for testing purposes", skill.Description);
        Assert.Equal(Some("manual"), skill.Source);
        Assert.Equal("high", skill.Confidence);
        Assert.Equal(Some("test-skill"), skill.Slug);
    }

    [Fact]
    public async Task FullParse_FixtureSkill_ContentCaptured()
    {
        var svc = new SkillService(GetFixturesPath());
        var skills = await svc.GetSkillsAsync();

        Assert.Contains("integration tests", skills.First().Content.IfNone(""));
    }

    [Fact]
    public async Task GetSkillAsync_ReturnsFixtureSkillBySlug()
    {
        var svc = new SkillService(GetFixturesPath());
        var skill = await svc.GetSkillAsync("test-skill");

        Assert.NotNull(skill);
        Assert.Equal("test-skill", skill!.Name);
    }

    [Fact]
    public async Task GetSkillAsync_ReturnsNullForNonexistent()
    {
        var svc = new SkillService(GetFixturesPath());
        var skill = await svc.GetSkillAsync("nonexistent");

        Assert.Null(skill);
    }
}
