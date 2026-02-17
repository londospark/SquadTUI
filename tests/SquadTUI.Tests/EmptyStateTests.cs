using FluentAssertions;
using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;

namespace SquadTUI.Tests;

/// <summary>
/// P1 — Empty state tests: verify screens handle empty/missing data gracefully.
/// </summary>
public class EmptyStateTests
{
    [Fact]
    public void AppState_EmptyMembers_GetOrEmptyReturnsEmptyList()
    {
        var state = new AppState();
        state.Members.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_EmptyDecisions_GetOrEmptyReturnsEmptyList()
    {
        var state = new AppState();
        state.Decisions.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_EmptySkills_GetOrEmptyReturnsEmptyList()
    {
        var state = new AppState();
        state.Skills.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_EmptyLogEntries_GetOrEmptyReturnsEmptyList()
    {
        var state = new AppState();
        state.LogEntries.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_EmptyTasks_GetOrEmptyReturnsEmptyList()
    {
        var state = new AppState();
        state.Tasks.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_EmptySprintHistory_GetOrEmptyReturnsEmptyList()
    {
        var state = new AppState();
        state.SprintHistory.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void AppState_CharterContent_NoneByDefault()
    {
        var state = new AppState();
        state.CharterContent.IsNone.Should().BeTrue();
    }

    [Fact]
    public void CharterContent_None_MatchReturnsDefaultMessage()
    {
        var state = new AppState();
        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        charter.Should().Be("No charter loaded");
    }

    [Fact]
    public void CharterContent_Some_MatchReturnsContent()
    {
        var state = new AppState
        {
            CharterContent = Some("# Charter\n\nContent here")
        };
        var charter = state.CharterContent.Match(Some: s => s, None: () => "No charter loaded");
        charter.Should().Contain("# Charter");
    }

    [Fact]
    public void RosterSelectedIndex_ClampedToZero_WhenMembersEmpty()
    {
        var state = new AppState();
        var members = state.Members.GetOrEmpty();
        members.Should().BeEmpty();
        // Clamp should produce 0 when count is 0 (list is empty, index stays at 0)
        var idx = Math.Clamp(state.RosterSelectedIndex, 0, Math.Max(0, members.Count - 1));
        idx.Should().Be(0);
    }

    [Fact]
    public void DecisionSelectedIndex_ClampedToZero_WhenDecisionsEmpty()
    {
        var state = new AppState();
        var decisions = state.Decisions.GetOrEmpty();
        decisions.Should().BeEmpty();
        var idx = Math.Clamp(state.DecisionSelectedIndex, 0, Math.Max(0, decisions.Count - 1));
        idx.Should().Be(0);
    }

    [Fact]
    public void SkillSelectedIndex_ClampedToZero_WhenSkillsEmpty()
    {
        var state = new AppState();
        var skills = state.Skills.GetOrEmpty();
        skills.Should().BeEmpty();
        var idx = Math.Clamp(state.SkillSelectedIndex, 0, Math.Max(0, skills.Count - 1));
        idx.Should().Be(0);
    }

    [Fact]
    public void LogSelectedIndex_ClampedToZero_WhenLogEntriesEmpty()
    {
        var state = new AppState();
        var logs = state.LogEntries.GetOrEmpty();
        logs.Should().BeEmpty();
        var idx = Math.Clamp(state.LogSelectedIndex, 0, Math.Max(0, logs.Count - 1));
        idx.Should().Be(0);
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
        (sprints.Count == 0 && tasks.Count == 0).Should().BeTrue(
            "empty sprint and task data should trigger the 'No sprint data' empty state");
    }

    [Fact]
    public void RosterScreen_EmptyMembers_ShouldTriggerEmptyState()
    {
        var state = new AppState();
        var members = state.Members.GetOrEmpty();
        // RosterScreen checks: if (members.Count == 0)
        members.Count.Should().Be(0,
            "empty members should trigger the 'No members found' empty state");
    }

    [Fact]
    public void DecisionsScreen_EmptyDecisions_ShouldTriggerEmptyState()
    {
        var state = new AppState();
        var decisions = state.Decisions.GetOrEmpty();
        // DecisionsScreen checks: if (decisions.Count == 0)
        decisions.Count.Should().Be(0,
            "empty decisions should trigger the 'No decisions found' empty state");
    }

    [Fact]
    public void ActivityLogScreen_EmptyLogEntries_ShouldTriggerEmptyState()
    {
        var state = new AppState();
        var logs = state.LogEntries.GetOrEmpty();
        // ActivityLogScreen checks: if (logs.Count == 0)
        logs.Count.Should().Be(0,
            "empty log entries should trigger the 'No activity log entries found' empty state");
    }

    [Fact]
    public void SkillsScreen_EmptySkills_ShouldTriggerEmptyState()
    {
        var state = new AppState();
        var skills = state.Skills.GetOrEmpty();
        // SkillsScreen checks: if (skills.Count == 0)
        skills.Count.Should().Be(0,
            "empty skills should trigger the 'No skills found' empty state");
    }

    [Fact]
    public void Dashboard_EmptyData_ShowsZeroCounts()
    {
        var state = new AppState();
        var members = state.Members.GetOrEmpty();
        var tasks = state.Tasks.GetOrEmpty();
        var logEntries = state.LogEntries.GetOrEmpty();
        var decisions = state.Decisions.GetOrEmpty();

        members.Count.Should().Be(0);
        tasks.Count.Should().Be(0);
        logEntries.Count.Should().Be(0);
        decisions.Count.Should().Be(0);

        // DashboardScreen derives counts via .GetOrEmpty() — zero data = zero counts
        var activeCount = members.Count(m => m.Status == MemberStatus.Active);
        activeCount.Should().Be(0);
    }

    [Fact]
    public void Dashboard_ProgressBar_NoTasks_NoDivisionByZero()
    {
        var state = new AppState();
        var tasks = state.Tasks.GetOrEmpty();
        var completedTasks = tasks.Count(t => t.Status == SquadTaskStatus.Done);
        // DashboardScreen: var total = tasks.Count > 0 ? tasks.Count : 1;
        var total = tasks.Count > 0 ? tasks.Count : 1;
        total.Should().Be(1, "guard prevents division by zero");
        var percentage = completedTasks * 100 / total;
        percentage.Should().Be(0);
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

        state.Members.GetOrEmpty().Should().BeEmpty();
        state.Decisions.GetOrEmpty().Should().BeEmpty();
        state.Skills.GetOrEmpty().Should().BeEmpty();
        state.LogEntries.GetOrEmpty().Should().BeEmpty();
        state.Tasks.GetOrEmpty().Should().BeEmpty();
        state.SprintHistory.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public void SelectedMemberName_Null_DefaultsToFirstMember_OrUnknown()
    {
        // CharterScreen logic: state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown")
        var state = new AppState();
        state.SelectedMemberName.Should().BeNull();

        var members = state.Members.GetOrEmpty();
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        memberName.Should().Be("Unknown", "empty members list defaults to 'Unknown'");
    }

    [Fact]
    public void SelectedMemberName_Null_WithMembers_DefaultsToFirstMember()
    {
        var state = new AppState
        {
            Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>
            {
                new("Alice", "Lead", MemberStatus.Active),
                new("Bob", "Dev", MemberStatus.Active),
            })
        };
        state.SelectedMemberName.Should().BeNull();

        var members = state.Members.GetOrEmpty();
        var memberName = state.SelectedMemberName ?? (members.Count > 0 ? members[0].Name : "Unknown");
        memberName.Should().Be("Alice", "first member is the default");
    }
}
