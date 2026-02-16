using FluentAssertions;
using SquadTUI.Screens;

namespace SquadTUI.Tests.Unit;

public class SampleDataTests
{
    [Fact]
    public void AllTaskAssignees_MatchAMemberName()
    {
        var memberNames = SampleData.Members.Select(m => m.Name).ToHashSet();
        foreach (var task in SampleData.Tasks)
        {
            if (task.Assignee is not null)
                memberNames.Should().Contain(task.Assignee,
                    $"task '{task.Title}' assignee '{task.Assignee}' should exist in Members");
        }
    }

    [Fact]
    public void AllLogEntryParticipants_ExistInMembers()
    {
        var memberNames = SampleData.Members.Select(m => m.Name).ToHashSet();
        foreach (var log in SampleData.LogEntries)
        {
            foreach (var participant in log.Participants)
            {
                memberNames.Should().Contain(participant,
                    $"log '{log.Topic}' participant '{participant}' should exist in Members");
            }
        }
    }

    [Fact]
    public void GetCharterFor_ReturnsNonEmptyForEachMember()
    {
        foreach (var member in SampleData.Members)
        {
            var charter = SampleData.GetCharterFor(member.Name);
            charter.Should().NotBeNullOrWhiteSpace(
                $"charter for '{member.Name}' should be non-empty");
        }
    }

    [Fact]
    public void NoMemberNames_AreNullOrEmpty()
    {
        foreach (var member in SampleData.Members)
        {
            member.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void NoMemberRoles_AreNullOrEmpty()
    {
        foreach (var member in SampleData.Members)
        {
            member.Role.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void Tasks_HaveUniqueIds()
    {
        var ids = SampleData.Tasks.Select(t => t.Id).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Decisions_HaveDates()
    {
        foreach (var decision in SampleData.Decisions)
        {
            decision.Date.Should().NotBeNullOrWhiteSpace(
                $"decision '{decision.Title}' should have a date");
        }
    }

    [Fact]
    public void Decisions_HaveAuthors()
    {
        foreach (var decision in SampleData.Decisions)
        {
            decision.Author.Should().NotBeNullOrWhiteSpace(
                $"decision '{decision.Title}' should have an author");
        }
    }

    [Fact]
    public void Members_HasAtLeastOneEntry()
    {
        SampleData.Members.Should().NotBeEmpty();
    }

    [Fact]
    public void Tasks_HasAtLeastOneEntry()
    {
        SampleData.Tasks.Should().NotBeEmpty();
    }

    [Fact]
    public void Decisions_HasAtLeastOneEntry()
    {
        SampleData.Decisions.Should().NotBeEmpty();
    }

    [Fact]
    public void Skills_HasAtLeastOneEntry()
    {
        SampleData.Skills.Should().NotBeEmpty();
    }

    [Fact]
    public void LogEntries_HasAtLeastOneEntry()
    {
        SampleData.LogEntries.Should().NotBeEmpty();
    }

    [Fact]
    public void Skills_HaveNonEmptyNames()
    {
        foreach (var skill in SampleData.Skills)
        {
            skill.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void Skills_HaveNonEmptyDescriptions()
    {
        foreach (var skill in SampleData.Skills)
        {
            skill.Description.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void GetCharterFor_UnknownMember_ReturnsFallback()
    {
        var charter = SampleData.GetCharterFor("UnknownPerson");
        charter.Should().Contain("No charter available yet.");
    }
}
