using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;

namespace SquadTUI.Tests.Unit.Screens;

public class RosterScreenTests
{
    private static AppState CreateStateWithMembers(bool showEmoji = false)
    {
        return new AppState
        {
            Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>
            {
                new("Danny", "Lead", MemberStatus.Active, "Review PR"),
                new("Linus", "Frontend Dev", MemberStatus.Idle),
                new("Rusty", "Backend Dev", MemberStatus.Working),
            }),
            Tasks = Right<AppError, IReadOnlyList<SquadTask>>(new List<SquadTask>
            {
                new("t-1", "Review PR", "Review codebase", SquadTaskStatus.InProgress, "Danny"),
            }),
            LogEntries = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(new List<OrchestrationLogEntry>()),
            CharterContent = Some("Charter content line\nSecond line\nThird line"),
            Settings = new AppSettings { ShowEmoji = showEmoji },
        };
    }

    [Fact]
    public void AppState_ConfirmingRemove_DefaultsFalse()
    {
        var state = new AppState();
        Assert.False(state.ConfirmingRemove);
    }

    [Fact]
    public void AppState_AddMemberMessage_DefaultsNull()
    {
        var state = new AppState();
        Assert.Null(state.AddMemberMessage);
    }

    [Fact]
    public void AppState_ConfirmingRemove_CanBeSet()
    {
        var state = new AppState { ConfirmingRemove = true };
        Assert.True(state.ConfirmingRemove);
    }

    [Fact]
    public void AppState_AddMemberMessage_CanBeSet()
    {
        var state = new AppState { AddMemberMessage = "Added Basher successfully" };
        Assert.Equal("Added Basher successfully", state.AddMemberMessage);
    }

    [Fact]
    public void ShowEmoji_DefaultsTrue()
    {
        var settings = new AppSettings();
        Assert.True(settings.ShowEmoji);
    }

    [Fact]
    public void ShowEmoji_CanBeToggled()
    {
        var settings = new AppSettings { ShowEmoji = true };
        Assert.True(settings.ShowEmoji);
    }

    [Fact]
    public void StateWithEmojiOff_HasCorrectSetting()
    {
        var state = CreateStateWithMembers(showEmoji: false);
        Assert.False(state.Settings.ShowEmoji);
    }

    [Fact]
    public void StateWithEmojiOn_HasCorrectSetting()
    {
        var state = CreateStateWithMembers(showEmoji: true);
        Assert.True(state.Settings.ShowEmoji);
    }

    [Fact]
    public void State_WithConfirmingRemove_FlagIsTrue()
    {
        var state = CreateStateWithMembers();
        state.ConfirmingRemove = true;
        Assert.True(state.ConfirmingRemove);
    }

    [Fact]
    public void State_WithAddMemberMessage_MessageIsSet()
    {
        var state = CreateStateWithMembers();
        state.AddMemberMessage = "Member Saul added as Negotiator";
        Assert.Equal("Member Saul added as Negotiator", state.AddMemberMessage);
    }

    [Fact]
    public void State_WithEmptyMembers_ShowsNoMembers()
    {
        var state = new AppState
        {
            Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>()),
        };
        var members = state.Members.GetOrEmpty();
        Assert.Empty(members);
    }

    [Fact]
    public void State_SelectedIndex_ClampedForRoster()
    {
        var state = CreateStateWithMembers();
        var members = state.Members.GetOrEmpty();
        state.RosterSelectedIndex = 5;

        var clamped = Math.Clamp(state.RosterSelectedIndex, 0, members.Count - 1);
        Assert.Equal(2, clamped);
    }

    [Fact]
    public void ConfirmingRemoveAndAddMemberMessage_MutuallyExclusive_InPractice()
    {
        var state = CreateStateWithMembers();
        // When confirming remove, add message should be null
        state.ConfirmingRemove = true;
        state.AddMemberMessage = null;
        Assert.True(state.ConfirmingRemove);
        Assert.Null(state.AddMemberMessage);

        // When showing add message, confirming should be false
        state.ConfirmingRemove = false;
        state.AddMemberMessage = "Success";
        Assert.False(state.ConfirmingRemove);
        Assert.NotNull(state.AddMemberMessage);
    }
}
