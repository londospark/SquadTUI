using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;

namespace SquadTUI.Tests.Unit.Models;

public class ModelTests
{
    [Fact]
    public void SquadMember_DefaultsToActiveStatus()
    {
        var member = new SquadMember("Test", "Dev");
        Assert.Equal(MemberStatus.Active, member.Status);
    }

    [Fact]
    public void SquadMember_DefaultsNoneCurrentTask()
    {
        var member = new SquadMember("Test", "Dev");
        Assert.True(member.CurrentTask.IsNone);
    }

    [Fact]
    public void SquadMember_DefaultsNoneCharterPath()
    {
        var member = new SquadMember("Test", "Dev");
        Assert.True(member.CharterPath.IsNone);
    }

    [Fact]
    public void SquadTask_DefaultsToPendingStatus()
    {
        var task = new SquadTask("t1", "Test Task");
        Assert.Equal(SquadTaskStatus.Pending, task.Status);
    }

    [Fact]
    public void SquadTask_DefaultsNoneAssignee()
    {
        var task = new SquadTask("t1", "Test Task");
        Assert.True(task.Assignee.IsNone);
    }

    [Fact]
    public void SquadTask_DefaultsNullTimestamps()
    {
        var task = new SquadTask("t1", "Test Task");
        Assert.Null(task.StartedAt);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void SquadMember_RecordEquality()
    {
        var a = new SquadMember("Danny", "Lead", MemberStatus.Active);
        var b = new SquadMember("Danny", "Lead", MemberStatus.Active);
        Assert.Equal(b, a);
    }

    [Fact]
    public void SquadMember_RecordInequality_DifferentStatus()
    {
        var a = new SquadMember("Danny", "Lead", MemberStatus.Active);
        var b = new SquadMember("Danny", "Lead", MemberStatus.Idle);
        Assert.NotEqual(b, a);
    }

    [Fact]
    public void SquadTask_RecordEquality()
    {
        var a = new SquadTask("t1", "Task", Status: SquadTaskStatus.Done);
        var b = new SquadTask("t1", "Task", Status: SquadTaskStatus.Done);
        Assert.Equal(b, a);
    }

    [Fact]
    public void SquadMember_WithExpression_CreatesNewInstance()
    {
        var original = new SquadMember("Danny", "Lead");
        var modified = original with { Status = MemberStatus.Working };
        Assert.Equal(MemberStatus.Working, modified.Status);
        Assert.Equal(MemberStatus.Active, original.Status);
    }

    [Fact]
    public void DecisionEntry_StoresAllFields()
    {
        var entry = new DecisionEntry("Title", "2026-01-01", "Author", "Content", "/path", 5);
        Assert.Equal("Title", entry.Title);
        Assert.Equal("2026-01-01", entry.Date);
        Assert.Equal("Author", entry.Author);
        Assert.Equal("Content", entry.Content);
        Assert.Equal("/path", entry.FilePath);
        Assert.Equal(5, entry.LineNumber);
    }

    [Fact]
    public void Skill_StoresAllFields()
    {
        var skill = new Skill("test", "desc", Some("manual"), "high", Some("body"), Some("test-slug"));
        Assert.Equal("test", skill.Name);
        Assert.Equal("desc", skill.Description);
        Assert.Equal(Some("manual"), skill.Source);
        Assert.Equal("high", skill.Confidence);
        Assert.Equal(Some("body"), skill.Content);
        Assert.Equal(Some("test-slug"), skill.Slug);
    }

    [Fact]
    public void OrchestrationLogEntry_StoresAllFields()
    {
        var entry = new OrchestrationLogEntry(
            DateTimeOffset.Parse("2026-02-16T00:00:00Z"),
            "2026-02-16",
            "kickoff",
            ["Danny", "Linus"],
            "Summary",
            ["Decision 1"],
            ["Outcome 1"],
            "Done stuff");
        Assert.Equal(DateTimeOffset.Parse("2026-02-16T00:00:00Z"), entry.Timestamp);
        Assert.Equal(2, entry.Participants.Count);
        Assert.Single(entry.Decisions);
        Assert.Single(entry.Outcomes);
        Assert.Equal("Done stuff", entry.WhatWasDone);
    }

    [Fact]
    public void TeamRoster_DefaultsNullMembers()
    {
        var roster = new TeamRoster("Test");
        Assert.Null(roster.Members);
        Assert.Null(roster.Description);
    }
}
