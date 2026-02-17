using SquadTUI.Tests.Fixtures;

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
                Assert.Contains(task.Assignee, memberNames);
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
                Assert.Contains(participant, memberNames);
            }
        }
    }

    [Fact]
    public void GetCharterFor_ReturnsNonEmptyForEachMember()
    {
        foreach (var member in SampleData.Members)
        {
            var charter = SampleData.GetCharterFor(member.Name);
            Assert.False(string.IsNullOrWhiteSpace(charter));
        }
    }

    [Fact]
    public void NoMemberNames_AreNullOrEmpty()
    {
        foreach (var member in SampleData.Members)
        {
            Assert.False(string.IsNullOrWhiteSpace(member.Name));
        }
    }

    [Fact]
    public void NoMemberRoles_AreNullOrEmpty()
    {
        foreach (var member in SampleData.Members)
        {
            Assert.False(string.IsNullOrWhiteSpace(member.Role));
        }
    }

    [Fact]
    public void Tasks_HaveUniqueIds()
    {
        var ids = SampleData.Tasks.Select(t => t.Id).ToList();
        Assert.Equal(ids.Distinct().Count(), ids.Count());
    }

    [Fact]
    public void Decisions_HaveDates()
    {
        foreach (var decision in SampleData.Decisions)
        {
            Assert.False(string.IsNullOrWhiteSpace(decision.Date));
        }
    }

    [Fact]
    public void Decisions_HaveAuthors()
    {
        foreach (var decision in SampleData.Decisions)
        {
            Assert.False(string.IsNullOrWhiteSpace(decision.Author));
        }
    }

    [Fact]
    public void Members_HasAtLeastOneEntry()
    {
        Assert.NotEmpty(SampleData.Members);
    }

    [Fact]
    public void Tasks_HasAtLeastOneEntry()
    {
        Assert.NotEmpty(SampleData.Tasks);
    }

    [Fact]
    public void Decisions_HasAtLeastOneEntry()
    {
        Assert.NotEmpty(SampleData.Decisions);
    }

    [Fact]
    public void Skills_HasAtLeastOneEntry()
    {
        Assert.NotEmpty(SampleData.Skills);
    }

    [Fact]
    public void LogEntries_HasAtLeastOneEntry()
    {
        Assert.NotEmpty(SampleData.LogEntries);
    }

    [Fact]
    public void Skills_HaveNonEmptyNames()
    {
        foreach (var skill in SampleData.Skills)
        {
            Assert.False(string.IsNullOrWhiteSpace(skill.Name));
        }
    }

    [Fact]
    public void Skills_HaveNonEmptyDescriptions()
    {
        foreach (var skill in SampleData.Skills)
        {
            Assert.False(string.IsNullOrWhiteSpace(skill.Description));
        }
    }

    [Fact]
    public void GetCharterFor_UnknownMember_ReturnsFallback()
    {
        var charter = SampleData.GetCharterFor("UnknownPerson");
        Assert.Contains("No charter available yet.", charter);
    }

    [Fact]
    public void SprintHistory_Has3Entries()
    {
        Assert.Equal(3, SampleData.SprintHistory.Count);
    }

    [Fact]
    public void OverallCompletionRate_IsBetween0And100()
    {
        Assert.InRange(SampleData.OverallCompletionRate, 0, 100);
    }

    [Fact]
    public void AverageVelocity_IsPositive()
    {
        Assert.True(SampleData.AverageVelocity > 0);
    }

    [Fact]
    public void VelocityTrend_ReturnsValidNumber()
    {
        var trend = SampleData.VelocityTrend;
        Assert.False(double.IsNaN(trend));
        Assert.False(double.IsInfinity(trend));
    }

    [Fact]
    public void TeamUtilization_ReturnsEntryForEachMember()
    {
        var memberNames = SampleData.Members.Select(m => m.Name).ToHashSet();
        var utilization = SampleData.TeamUtilization;
        foreach (var entry in utilization)
        {
            Assert.Contains(entry.Name, memberNames);
            Assert.InRange(entry.Utilization, 0, 100);
        }
    }
}
