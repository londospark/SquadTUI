using FluentAssertions;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class SkillServiceTests : IDisposable
{
    private readonly string _tempDir;

    public SkillServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempDir, ".ai-team", "skills"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private void WriteSkillFile(string slug, string content)
    {
        var dir = Path.Combine(_tempDir, ".ai-team", "skills", slug);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "SKILL.md"), content);
    }

    [Fact]
    public async Task ParseSkillMd_WithYamlFrontmatter_ExtractsFields()
    {
        WriteSkillFile("test-skill", """
            ---
            name: "Test Skill"
            description: "A test skill"
            source: "manual"
            confidence: "high"
            ---

            # Test Skill

            Body content here.
            """);

        var svc = new SkillService(_tempDir);
        var skills = await svc.GetSkillsAsync();

        var skill = skills.First();
        skill.Name.Should().Be("Test Skill");
        skill.Description.Should().Be("A test skill");
        skill.Source.Should().Be("manual");
        skill.Confidence.Should().Be("high");
        skill.Slug.Should().Be("test-skill");
    }

    [Fact]
    public async Task ParseSkillMd_WithoutFrontmatter_ExtractsNameFromHeading()
    {
        WriteSkillFile("my-skill", """
            # My Custom Skill

            This is a skill without frontmatter.
            """);

        var svc = new SkillService(_tempDir);
        var skills = await svc.GetSkillsAsync();

        var skill = skills.First();
        skill.Name.Should().Be("My Custom Skill");
        skill.Description.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleMissingSkillsDirectory_ReturnsEmptyList()
    {
        var emptyDir = Path.Combine(Path.GetTempPath(), "SquadTUI_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(emptyDir);
        try
        {
            var svc = new SkillService(emptyDir);
            var skills = await svc.GetSkillsAsync();
            skills.Should().BeEmpty();
        }
        finally
        {
            Directory.Delete(emptyDir, true);
        }
    }

    [Fact]
    public async Task HandleSkillDirectoryWithoutSkillMd_Skipped()
    {
        var dir = Path.Combine(_tempDir, ".ai-team", "skills", "empty-skill");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "README.md"), "Not a SKILL.md");

        var svc = new SkillService(_tempDir);
        var skills = await svc.GetSkillsAsync();

        skills.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSkillAsync_ReturnsNullForUnknownSlug()
    {
        var svc = new SkillService(_tempDir);
        var skill = await svc.GetSkillAsync("nonexistent");

        skill.Should().BeNull();
    }

    [Fact]
    public async Task GetSkillAsync_ReturnsCorrectSkillBySlug()
    {
        WriteSkillFile("code-review", """
            ---
            name: "Code Review"
            description: "Reviews code"
            ---

            Body.
            """);

        var svc = new SkillService(_tempDir);
        var skill = await svc.GetSkillAsync("code-review");

        skill.Should().NotBeNull();
        skill!.Name.Should().Be("Code Review");
        skill.Slug.Should().Be("code-review");
    }

    [Fact]
    public async Task ParseSkillMd_ContentCaptured()
    {
        WriteSkillFile("test-skill", """
            ---
            name: "Test"
            description: "desc"
            ---

            # Test

            Body content line 1.
            Body content line 2.
            """);

        var svc = new SkillService(_tempDir);
        var skills = await svc.GetSkillsAsync();

        skills.First().Content.Should().Contain("Body content line 1.");
        skills.First().Content.Should().Contain("Body content line 2.");
    }

    [Fact]
    public async Task ParseSkillMd_NoHeadingNoFrontmatter_UsesSlugAsName()
    {
        WriteSkillFile("fallback-name", """
            Just some content without a heading.
            """);

        var svc = new SkillService(_tempDir);
        var skills = await svc.GetSkillsAsync();

        skills.First().Name.Should().Be("fallback-name");
    }

    [Fact]
    public async Task GetSkillsAsync_MultipleSkills()
    {
        WriteSkillFile("skill-a", """
            # Skill A

            Content A.
            """);
        WriteSkillFile("skill-b", """
            ---
            name: "Skill B"
            description: "B"
            ---

            Content B.
            """);

        var svc = new SkillService(_tempDir);
        var skills = await svc.GetSkillsAsync();

        skills.Should().HaveCount(2);
    }
}
