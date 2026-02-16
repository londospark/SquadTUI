using FluentAssertions;
using SquadTUI.Models;

namespace SquadTUI.Tests.Unit.Models;

public class ModelTests
{
    [Fact]
    public void SquadMember_DefaultsToActiveStatus()
    {
        var member = new SquadMember("Test", "Dev");
        member.Status.Should().Be(MemberStatus.Active);
    }

    [Fact]
    public void SquadMember_DefaultsNullCurrentTask()
    {
        var member = new SquadMember("Test", "Dev");
        member.CurrentTask.Should().BeNull();
    }

    [Fact]
    public void SquadMember_DefaultsNullCharterPath()
    {
        var member = new SquadMember("Test", "Dev");
        member.CharterPath.Should().BeNull();
    }

    [Fact]
    public void SquadTask_DefaultsToPendingStatus()
    {
        var task = new SquadTask("t1", "Test Task");
        task.Status.Should().Be(SquadTaskStatus.Pending);
    }

    [Fact]
    public void SquadTask_DefaultsNullAssignee()
    {
        var task = new SquadTask("t1", "Test Task");
        task.Assignee.Should().BeNull();
    }

    [Fact]
    public void SquadTask_DefaultsNullTimestamps()
    {
        var task = new SquadTask("t1", "Test Task");
        task.StartedAt.Should().BeNull();
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void SquadMember_RecordEquality()
    {
        var a = new SquadMember("Danny", "Lead", MemberStatus.Active);
        var b = new SquadMember("Danny", "Lead", MemberStatus.Active);
        a.Should().Be(b);
    }

    [Fact]
    public void SquadMember_RecordInequality_DifferentStatus()
    {
        var a = new SquadMember("Danny", "Lead", MemberStatus.Active);
        var b = new SquadMember("Danny", "Lead", MemberStatus.Idle);
        a.Should().NotBe(b);
    }

    [Fact]
    public void SquadTask_RecordEquality()
    {
        var a = new SquadTask("t1", "Task", Status: SquadTaskStatus.Done);
        var b = new SquadTask("t1", "Task", Status: SquadTaskStatus.Done);
        a.Should().Be(b);
    }

    [Fact]
    public void SquadMember_WithExpression_CreatesNewInstance()
    {
        var original = new SquadMember("Danny", "Lead");
        var modified = original with { Status = MemberStatus.Working };
        modified.Status.Should().Be(MemberStatus.Working);
        original.Status.Should().Be(MemberStatus.Active);
    }

    [Fact]
    public void DecisionEntry_StoresAllFields()
    {
        var entry = new DecisionEntry("Title", "2026-01-01", "Author", "Content", "/path", 5);
        entry.Title.Should().Be("Title");
        entry.Date.Should().Be("2026-01-01");
        entry.Author.Should().Be("Author");
        entry.Content.Should().Be("Content");
        entry.FilePath.Should().Be("/path");
        entry.LineNumber.Should().Be(5);
    }

    [Fact]
    public void Skill_StoresAllFields()
    {
        var skill = new Skill("test", "desc", "manual", "high", "body", "test-slug");
        skill.Name.Should().Be("test");
        skill.Description.Should().Be("desc");
        skill.Source.Should().Be("manual");
        skill.Confidence.Should().Be("high");
        skill.Content.Should().Be("body");
        skill.Slug.Should().Be("test-slug");
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
        entry.Timestamp.Should().Be(DateTimeOffset.Parse("2026-02-16T00:00:00Z"));
        entry.Participants.Should().HaveCount(2);
        entry.Decisions.Should().ContainSingle();
        entry.Outcomes.Should().ContainSingle();
        entry.WhatWasDone.Should().Be("Done stuff");
    }

    [Fact]
    public void TeamRoster_DefaultsNullMembers()
    {
        var roster = new TeamRoster("Test");
        roster.Members.Should().BeNull();
        roster.Description.Should().BeNull();
    }
}
