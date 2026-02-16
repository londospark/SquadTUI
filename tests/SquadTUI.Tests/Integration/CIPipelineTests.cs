using FluentAssertions;

namespace SquadTUI.Tests.Integration;

public class CIPipelineTests
{
    private readonly string _repoRoot;

    public CIPipelineTests()
    {
        // Find repository root
        var current = Directory.GetCurrentDirectory();
        while (current != null && !Directory.Exists(Path.Combine(current, ".git")))
        {
            current = Directory.GetParent(current)?.FullName;
        }

        _repoRoot = current ?? throw new InvalidOperationException("Could not find repository root");
    }

    [Fact]
    public void GitHubWorkflowsDirectory_Exists()
    {
        var workflowsDir = Path.Combine(_repoRoot, ".github", "workflows");
        Directory.Exists(workflowsDir).Should().BeTrue("GitHub Actions workflows directory should exist");
    }

    [Fact]
    public void CIWorkflowYaml_Exists()
    {
        var ciYaml = Path.Combine(_repoRoot, ".github", "workflows", "ci.yml");
        var ciYamlAlternate = Path.Combine(_repoRoot, ".github", "workflows", "ci.yaml");

        var exists = File.Exists(ciYaml) || File.Exists(ciYamlAlternate);
        exists.Should().BeTrue("CI workflow YAML file should exist");
    }

    [Fact]
    public void CIWorkflowYaml_IsValidYaml()
    {
        var workflowsDir = Path.Combine(_repoRoot, ".github", "workflows");
        if (!Directory.Exists(workflowsDir))
            return;

        var yamlFiles = Directory.GetFiles(workflowsDir, "*.yml")
            .Concat(Directory.GetFiles(workflowsDir, "*.yaml"))
            .ToList();

        yamlFiles.Should().NotBeEmpty("should have at least one workflow YAML file");

        foreach (var yamlFile in yamlFiles)
        {
            var content = File.ReadAllText(yamlFile);
            
            // Basic YAML validation - should have key workflow properties
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("name:", "workflow should have a name");
            content.Should().Contain("on:", "workflow should have triggers");
            content.Should().Contain("jobs:", "workflow should have jobs");
        }
    }

    [Fact]
    public void CIWorkflow_ContainsBuildStep()
    {
        var workflowsDir = Path.Combine(_repoRoot, ".github", "workflows");
        if (!Directory.Exists(workflowsDir))
            return;

        var yamlFiles = Directory.GetFiles(workflowsDir, "*.yml")
            .Concat(Directory.GetFiles(workflowsDir, "*.yaml"))
            .ToList();

        var hasBuildStep = false;
        foreach (var yamlFile in yamlFiles)
        {
            var content = File.ReadAllText(yamlFile);
            if (content.Contains("dotnet build") || content.Contains("dotnet-build"))
            {
                hasBuildStep = true;
                break;
            }
        }

        hasBuildStep.Should().BeTrue("CI workflow should contain a build step");
    }

    [Fact]
    public void CIWorkflow_ContainsTestStep()
    {
        var workflowsDir = Path.Combine(_repoRoot, ".github", "workflows");
        if (!Directory.Exists(workflowsDir))
            return;

        var yamlFiles = Directory.GetFiles(workflowsDir, "*.yml")
            .Concat(Directory.GetFiles(workflowsDir, "*.yaml"))
            .ToList();

        var hasTestStep = false;
        foreach (var yamlFile in yamlFiles)
        {
            var content = File.ReadAllText(yamlFile);
            if (content.Contains("dotnet test") || content.Contains("dotnet-test"))
            {
                hasTestStep = true;
                break;
            }
        }

        hasTestStep.Should().BeTrue("CI workflow should contain a test step");
    }

    [Fact]
    public void ProjectFile_Exists()
    {
        var projectFile = Path.Combine(_repoRoot, "src", "SquadTUI", "SquadTUI.csproj");
        File.Exists(projectFile).Should().BeTrue("main project file should exist");
    }

    [Fact]
    public void TestProjectFile_Exists()
    {
        var testProjectFile = Path.Combine(_repoRoot, "tests", "SquadTUI.Tests", "SquadTUI.Tests.csproj");
        File.Exists(testProjectFile).Should().BeTrue("test project file should exist");
    }

    [Fact]
    public void SolutionFile_Exists()
    {
        var slnFiles = Directory.GetFiles(_repoRoot, "*.sln")
            .Concat(Directory.GetFiles(_repoRoot, "*.slnx"))
            .ToList();

        slnFiles.Should().NotBeEmpty("solution file should exist");
    }

    // DotnetBuild_Succeeds and DotnetTest_Passes were removed.
    // They spawned child `dotnet build`/`dotnet test` processes, causing
    // infinite recursion in CI. The CI workflow itself already validates
    // that build and test succeed — these tests were circular.
}
