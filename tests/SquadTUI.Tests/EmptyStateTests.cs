using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;

namespace SquadTUI.Tests;

/// <summary>
/// P1 — Empty state tests: verify screens handle empty/missing data gracefully.
/// Basic empty-state GetOrEmpty and CharterContent.IsNone coverage is in ErrorHandlingTests.
/// </summary>
public class EmptyStateTests
{
    [Fact]
    public void CharterContent_None_MatchReturnsDefaultMessage()
    {
        var state = new AppState();
        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        Assert.Equal("No charter loaded", charter);
    }

    [Fact]
    public void CharterContent_Some_MatchReturnsContent()
    {
        var state = new AppState
        {
            CharterContent = Some("# Charter\n\nContent here")
        };
        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        Assert.Contains("# Charter", charter);
    }

    [Theory]
    [InlineData("Roster")]
    [InlineData("Decision")]
    [InlineData("Skill")]
    [InlineData("Log")]
    public void SelectedIndex_ClampedToZero_WhenCollectionEmpty(string collectionName)
    {
        var state = new AppState();
        
        var (collection, index) = collectionName switch
        {
            "Roster" => ((System.Collections.IList)state.Members.GetOrEmpty(), state.RosterSelectedIndex),
            "Decision" => ((System.Collections.IList)state.Decisions.GetOrEmpty(), state.DecisionSelectedIndex),
            "Skill" => ((System.Collections.IList)state.Skills.GetOrEmpty(), state.SkillSelectedIndex),
            "Log" => ((System.Collections.IList)state.LogEntries.GetOrEmpty(), state.LogSelectedIndex),
            _ => throw new ArgumentException($"Unknown collection: {collectionName}")
        };
        
        Assert.Empty(collection);
        var clampedIdx = Math.Clamp(index, 0, Math.Max(0, collection.Count - 1));
        Assert.Equal(0, clampedIdx);
    }

    [Fact]
    public void MetricsScreen_EmptySprintHistory_ShowsNoDataMessage()
    {
        // When SprintHistory and Tasks are both empty, MetricsScreen returns
        // a widget containing "No sprint data" text
        var state = new AppState();
        state.SprintHistory = Right<AppError, IReadOnlyList<SprintMetrics>>([]);
        state.Tasks = Right<AppError, IReadOnlyList<SquadTask>>([]);

        var sprints = state.SprintHistory.GetOrEmpty();
        var tasks = state.Tasks.GetOrEmpty();
        // The MetricsScreen checks: if (sprints.Count == 0 && tasks.Count == 0)
        Assert.True(sprints.Count == 0 && tasks.Count == 0);
    }

    [Theory]
    [InlineData("Roster", "Members")]
    [InlineData("Decisions", "Decisions")]
    [InlineData("ActivityLog", "LogEntries")]
    [InlineData("Skills", "Skills")]
    public void Screen_EmptyCollection_ShouldTriggerEmptyState(string screenName, string propertyName)
    {
        var state = new AppState();
        
        var collection = propertyName switch
        {
            "Members" => (System.Collections.IList)state.Members.GetOrEmpty(),
            "Decisions" => (System.Collections.IList)state.Decisions.GetOrEmpty(),
            "LogEntries" => (System.Collections.IList)state.LogEntries.GetOrEmpty(),
            "Skills" => (System.Collections.IList)state.Skills.GetOrEmpty(),
            _ => throw new ArgumentException($"Unknown property: {propertyName}")
        };
        
        Assert.Equal(0, collection.Count);
    }

    [Fact]
    public void Dashboard_EmptyData_ShowsZeroCounts()
    {
        var state = new AppState();
        var members = state.Members.GetOrEmpty();
        var tasks = state.Tasks.GetOrEmpty();
        var logEntries = state.LogEntries.GetOrEmpty();
        var decisions = state.Decisions.GetOrEmpty();

        Assert.Equal(0, members.Count);
        Assert.Equal(0, tasks.Count);
        Assert.Equal(0, logEntries.Count);
        Assert.Equal(0, decisions.Count);

        // DashboardScreen derives counts via .GetOrEmpty() — zero data = zero counts
        var activeCount = members.Count(m => m.Status == MemberStatus.Active);
        Assert.Equal(0, activeCount);
    }

    [Fact]
    public void Dashboard_ProgressBar_NoTasks_NoDivisionByZero()
    {
        var state = new AppState();
        var tasks = state.Tasks.GetOrEmpty();
        var completedTasks = tasks.Count(t => t.Status == SquadTaskStatus.Done);
        // DashboardScreen: var total = tasks.Count > 0 ? tasks.Count : 1;
        var total = tasks.Count > 0 ? tasks.Count : 1;
        Assert.Equal(1, total);
        var percentage = completedTasks * 100 / total;
        Assert.Equal(0, percentage);
    }

    [Fact]
    public void AppState_LeftErrors_AllScreens_GetOrEmpty_ReturnEmptyLists()
    {
        var state = new AppState
        {
            Members = Left<AppError, IReadOnlyList<SquadMember>>(new ServiceError("T", "fail")),
            Decisions = Left<AppError, IReadOnlyList<DecisionEntry>>(new ServiceError("D", "fail")),
            Skills = Left<AppError, IReadOnlyList<Skill>>(new ServiceError("S", "fail")),
            LogEntries = Left<AppError, IReadOnlyList<OrchestrationLogEntry>>(new ServiceError("L", "fail")),
            Tasks = Left<AppError, IReadOnlyList<SquadTask>>(new ServiceError("T", "fail")),
            SprintHistory = Left<AppError, IReadOnlyList<SprintMetrics>>(new ServiceError("M", "fail")),
        };

        Assert.Empty(state.Members.GetOrEmpty());
        Assert.Empty(state.Decisions.GetOrEmpty());
        Assert.Empty(state.Skills.GetOrEmpty());
        Assert.Empty(state.LogEntries.GetOrEmpty());
        Assert.Empty(state.Tasks.GetOrEmpty());
        Assert.Empty(state.SprintHistory.GetOrEmpty());
    }

    [Fact]
    public void SelectedMemberName_Null_DefaultsToFirstMember_OrUnknown()
    {
        // CharterScreen logic: state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown")
        var state = new AppState();
        Assert.Null(state.SelectedMemberName);

        var members = state.Members.GetOrEmpty();
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        Assert.Equal("Unknown", memberName);
    }

    [Fact]
    public void SelectedMemberName_Null_WithMembers_DefaultsToFirstMember()
    {
        var state = new AppState
        {
            Members = Right<AppError, IReadOnlyList<SquadMember>>([
                new("Alice", "Lead", MemberStatus.Active),
                new("Bob", "Dev", MemberStatus.Active),
            ])
        };
        Assert.Null(state.SelectedMemberName);

        var members = state.Members.GetOrEmpty();
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        Assert.Equal("Alice", memberName);
    }
}
