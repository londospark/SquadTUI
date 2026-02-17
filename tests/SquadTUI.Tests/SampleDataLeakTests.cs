using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;

namespace SquadTUI.Tests;

/// <summary>
/// P0 — Ensures SampleData never leaks into production code.
/// SampleData.cs must live exclusively in tests/SquadTUI.Tests/Fixtures/.
/// </summary>
public class SampleDataLeakTests
{
    private static readonly string[] SampleDataNames =
        ["Sonic", "Tails", "Knuckles", "Amy", "Shadow", "Eggman"];

    [Fact]
    public void ProductionProject_DoesNotContainSampleDataFile()
    {
        var srcDir = FindSrcDir();
        var sampleFiles = Directory.GetFiles(srcDir, "SampleData.cs", SearchOption.AllDirectories);
        Assert.Empty(sampleFiles);
    }

    [Fact]
    public void ProductionCsFiles_DoNotReferenceSampleData()
    {
        var srcDir = FindSrcDir();
        var csFiles = Directory.GetFiles(srcDir, "*.cs", SearchOption.AllDirectories);

        var violations = new List<string>();
        foreach (var file in csFiles)
        {
            var content = File.ReadAllText(file);
            if (content.Contains("SampleData", StringComparison.Ordinal))
                violations.Add(Path.GetFileName(file));
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void ProductionCsproj_DoesNotReferenceSampleData()
    {
        var srcDir = FindSrcDir();
        var csproj = Directory.GetFiles(srcDir, "*.csproj", SearchOption.TopDirectoryOnly);
        Assert.NotEmpty(csproj);

        var content = File.ReadAllText(csproj[0]);
        Assert.DoesNotContain("SampleData", content);
    }

    [Fact]
    public void AppState_WithRealData_NoSampleDataNamesInMembers()
    {
        var state = CreateStateWithRealData();
        var memberNames = state.Members.GetOrEmpty().Select(m => m.Name).ToList();

        foreach (var sampleName in SampleDataNames)
        {
            Assert.DoesNotContain(sampleName, memberNames);
        }
    }

    [Fact]
    public void AppState_WithRealData_NoSampleDataNamesInTasks()
    {
        var state = CreateStateWithRealData();
        var assignees = state.Tasks.GetOrEmpty()
            .Where(t => t.Assignee.IsSome)
            .Select(t => t.Assignee.IfNone(""))
            .ToList();

        foreach (var sampleName in SampleDataNames)
        {
            Assert.DoesNotContain(sampleName, assignees);
        }
    }

    [Fact]
    public void AppState_WithRealData_NoSampleDataNamesInDecisions()
    {
        var state = CreateStateWithRealData();
        var authors = state.Decisions.GetOrEmpty().Select(d => d.Author).ToList();

        foreach (var sampleName in SampleDataNames)
        {
            Assert.DoesNotContain(sampleName, authors);
        }
    }

    [Fact]
    public void SampleDataFixture_LivesInTestProject()
    {
        var testDir = FindTestDir();
        var fixtureFile = Path.Combine(testDir, "Fixtures", "SampleData.cs");
        Assert.True(File.Exists(fixtureFile));
    }

    private static AppState CreateStateWithRealData()
    {
        var state = new AppState();
        state.Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>
        {
            new("Alice", "Lead", MemberStatus.Active, "Architecture review"),
            new("Bob", "Developer", MemberStatus.Working, "Building API"),
            new("Carol", "Tester", MemberStatus.Active, "Writing tests"),
        });
        state.Tasks = Right<AppError, IReadOnlyList<SquadTask>>(new List<SquadTask>
        {
            new("t-1", "Build API", "REST endpoints", SquadTaskStatus.InProgress, "Bob"),
            new("t-2", "Write tests", "Unit tests", SquadTaskStatus.Pending, "Carol"),
        });
        state.Decisions = Right<AppError, IReadOnlyList<DecisionEntry>>(new List<DecisionEntry>
        {
            new("Use REST", "2026-03-01", "Alice", "Chose REST over gRPC"),
        });
        return state;
    }

    private static string FindSrcDir()
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var src = Path.Combine(dir, "src", "SquadTUI");
            if (Directory.Exists(src)) return src;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find src/SquadTUI from " + AppContext.BaseDirectory);
    }

    private static string FindTestDir()
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var tests = Path.Combine(dir, "tests", "SquadTUI.Tests");
            if (Directory.Exists(tests)) return tests;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find tests/SquadTUI.Tests from " + AppContext.BaseDirectory);
    }
}
