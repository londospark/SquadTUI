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
        Assert.True(Directory.Exists(workflowsDir));
    }

    [Fact]
    public void CIWorkflowYaml_Exists()
    {
        var ciYaml = Path.Combine(_repoRoot, ".github", "workflows", "ci.yml");
        var ciYamlAlternate = Path.Combine(_repoRoot, ".github", "workflows", "ci.yaml");

        var exists = File.Exists(ciYaml) || File.Exists(ciYamlAlternate);
        Assert.True(exists);
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

        Assert.NotEmpty(yamlFiles);

        foreach (var yamlFile in yamlFiles)
        {
            var content = File.ReadAllText(yamlFile);
            
            // Basic YAML validation - should have key workflow properties
            Assert.False(string.IsNullOrEmpty(content));
            Assert.Contains("name:", content);
            Assert.Contains("on:", content);
            Assert.Contains("jobs:", content);
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

        Assert.True(hasBuildStep);
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

        Assert.True(hasTestStep);
    }

    [Fact]
    public void ProjectFile_Exists()
    {
        var projectFile = Path.Combine(_repoRoot, "src", "SquadTUI", "SquadTUI.csproj");
        Assert.True(File.Exists(projectFile));
    }

    [Fact]
    public void TestProjectFile_Exists()
    {
        var testProjectFile = Path.Combine(_repoRoot, "tests", "SquadTUI.Tests", "SquadTUI.Tests.csproj");
        Assert.True(File.Exists(testProjectFile));
    }

    [Fact]
    public void SolutionFile_Exists()
    {
        var slnFiles = Directory.GetFiles(_repoRoot, "*.sln")
            .Concat(Directory.GetFiles(_repoRoot, "*.slnx"))
            .ToList();

        Assert.NotEmpty(slnFiles);
    }

    // DotnetBuild_Succeeds and DotnetTest_Passes were removed.
    // They spawned child `dotnet build`/`dotnet test` processes, causing
    // infinite recursion in CI. The CI workflow itself already validates
    // that build and test succeed — these tests were circular.
}
