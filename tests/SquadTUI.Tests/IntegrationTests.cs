using FluentAssertions;
using LanguageExt;
using static LanguageExt.Prelude;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SquadTUI.Models;
using SquadTUI.Screens;
using SquadTUI.Services;
using SquadTUI.Themes;

namespace SquadTUI.Tests;

/// <summary>
/// P2 — Integration tests: DataBridge with mock services, AppState initialization, settings persistence.
/// </summary>
public class IntegrationTests
{
    #region DataBridge with Mock Services

    [Fact]
    public async Task DataBridge_AllLoads_ReturnLeft_WhenAllServicesThrow()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("team fail"));

        var decisionService = Substitute.For<IDecisionService>();
        decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("decision fail"));

        var skillService = Substitute.For<ISkillService>();
        skillService.GetSkillsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("skill fail"));

        var logService = Substitute.For<IOrchestrationLogService>();
        logService.GetEntriesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("log fail"));

        var sp = CreateServiceProvider(teamService, decisionService, skillService, logService);
        var bridge = new DataBridge(sp);

        var roster = await bridge.LoadRosterDataAsync();
        var decisions = await bridge.LoadDecisionsDataAsync();
        var skills = await bridge.LoadSkillsDataAsync();
        var logs = await bridge.LoadLogDataAsync();
        var tasks = await bridge.LoadTasksFromRosterAsync();

        roster.IsLeft.Should().BeTrue();
        decisions.IsLeft.Should().BeTrue();
        skills.IsLeft.Should().BeTrue();
        logs.IsLeft.Should().BeTrue();
        tasks.IsLeft.Should().BeTrue();
    }

    [Fact]
    public async Task DataBridge_PartialFailure_OtherDataStillAvailable()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("team fail"));

        var decisionService = Substitute.For<IDecisionService>();
        decisionService.GetDecisionsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<DecisionEntry> { new("Decision1", "2026-03-01", "Alice", "Details") });

        var sp = CreateServiceProvider(teamService: teamService, decisionService: decisionService);
        var bridge = new DataBridge(sp);

        var roster = await bridge.LoadRosterDataAsync();
        var decisions = await bridge.LoadDecisionsDataAsync();

        roster.IsLeft.Should().BeTrue("team service failed");
        decisions.IsRight.Should().BeTrue("decision service succeeded");
        decisions.GetOrEmpty().Should().HaveCount(1);
    }

    [Fact]
    public async Task DataBridge_CharterContent_ReturnsNone_WhenMemberNotFound()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetMemberAsync("nobody", Arg.Any<CancellationToken>())
            .Returns((SquadMember?)null);

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadCharterContentAsync("nobody");
        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task DataBridge_CharterContent_ReturnsNone_WhenServiceThrows()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetMemberAsync("crash", Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("crash"));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadCharterContentAsync("crash");
        result.IsNone.Should().BeTrue("exception should be caught and return None");
    }

    [Fact]
    public async Task DataBridge_LoadRoster_EmptyRoster_ReturnsRightEmptyList()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .Returns(new TeamRoster("Empty", Members: new List<SquadMember>()));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadRosterDataAsync();
        result.IsRight.Should().BeTrue();
        result.GetOrEmpty().Should().BeEmpty();
    }

    [Fact]
    public async Task DataBridge_LoadRoster_NullMembers_ReturnsRightEmptyList()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.GetRosterAsync(Arg.Any<CancellationToken>())
            .Returns(new TeamRoster("NullMembers"));

        var sp = CreateServiceProvider(teamService: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.LoadRosterDataAsync();
        result.IsRight.Should().BeTrue();
        result.GetOrEmpty().Should().BeEmpty("null Members should become empty list");
    }

    #endregion

    #region AppState Initialization with Either Types

    [Fact]
    public void AppState_CanBeInitialized_WithRightData()
    {
        var state = new AppState
        {
            Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>
            {
                new("Alice", "Lead", MemberStatus.Active, "Review PR"),
            }),
            Tasks = Right<AppError, IReadOnlyList<SquadTask>>(new List<SquadTask>
            {
                new("t-1", "Review PR", "Review codebase", SquadTaskStatus.InProgress, "Alice"),
            }),
            Decisions = Right<AppError, IReadOnlyList<DecisionEntry>>(new List<DecisionEntry>
            {
                new("Use REST", "2026-03-01", "Alice", "Decided to use REST"),
            }),
        };

        state.Members.GetOrEmpty().Should().HaveCount(1);
        state.Tasks.GetOrEmpty().Should().HaveCount(1);
        state.Decisions.GetOrEmpty().Should().HaveCount(1);
    }

    [Fact]
    public void AppState_CanBeInitialized_WithLeftErrors()
    {
        var state = new AppState
        {
            Members = Left<AppError, IReadOnlyList<SquadMember>>(new ServiceError("Team", "down")),
            Tasks = Left<AppError, IReadOnlyList<SquadTask>>(new ServiceError("Tasks", "error")),
            Decisions = Left<AppError, IReadOnlyList<DecisionEntry>>(new NoDataError("decisions")),
        };

        state.Members.IsLeft.Should().BeTrue();
        state.Tasks.IsLeft.Should().BeTrue();
        state.Decisions.IsLeft.Should().BeTrue();
    }

    [Fact]
    public void AppState_MixedLeftRight_WorksCorrectly()
    {
        var state = new AppState
        {
            Members = Right<AppError, IReadOnlyList<SquadMember>>(new List<SquadMember>
            {
                new("Alice", "Lead", MemberStatus.Active),
            }),
            Tasks = Left<AppError, IReadOnlyList<SquadTask>>(new ServiceError("Tasks", "timeout")),
        };

        state.Members.IsRight.Should().BeTrue();
        state.Members.GetOrEmpty().Should().HaveCount(1);
        state.Tasks.IsLeft.Should().BeTrue();
        state.Tasks.GetOrEmpty().Should().BeEmpty();
    }

    #endregion

    #region Settings Persistence with New Theme Indices

    [Fact]
    public void AppSettings_DefaultThemeName_IsOcean()
    {
        var settings = new AppSettings();
        settings.ThemeName.Should().Be("Ocean");
    }

    [Theory]
    [InlineData(0, "Ocean")]
    [InlineData(1, "Heist")]
    [InlineData(2, "Sunset")]
    [InlineData(3, "HighContrast")]
    [InlineData(4, "Forest")]
    [InlineData(5, "Cyberpunk")]
    [InlineData(6, "Midnight")]
    [InlineData(7, "Ember")]
    [InlineData(8, "Arctic")]
    [InlineData(9, "Retro")]
    public void ThemeIndex_MapsToThemeName(int index, string expectedName)
    {
        ThemeManager.ThemeNames[index].Should().Be(expectedName);
    }

    [Fact]
    public void AppState_SelectedThemeIndex_RangeCoversAllThemes()
    {
        var state = new AppState();
        for (int i = 0; i < 10; i++)
        {
            state.SelectedThemeIndex = i;
            ThemeManager.ThemeNames[state.SelectedThemeIndex].Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void AppSettings_CanSetAllThemeNames()
    {
        var settings = new AppSettings();
        foreach (var name in ThemeManager.ThemeNames)
        {
            settings.ThemeName = name;
            settings.ThemeName.Should().Be(name);
        }
    }

    [Fact]
    public void AppSettings_JsonRoundTrip_PreservesThemeName()
    {
        var settings = new AppSettings { ThemeName = "Cyberpunk" };
        var json = System.Text.Json.JsonSerializer.Serialize(settings);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

        deserialized.Should().NotBeNull();
        deserialized!.ThemeName.Should().Be("Cyberpunk");
    }

    [Fact]
    public void AppSettings_JsonRoundTrip_PreservesAllProperties()
    {
        var settings = new AppSettings
        {
            ThemeName = "Retro",
            VimBindings = false,
            MouseEnabled = false,
            ShowEmoji = false,
            MarkdownRendering = false,
            DefaultScreen = "Roster",
        };
        var json = System.Text.Json.JsonSerializer.Serialize(settings);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

        deserialized.Should().NotBeNull();
        deserialized!.ThemeName.Should().Be("Retro");
        deserialized.VimBindings.Should().BeFalse();
        deserialized.MouseEnabled.Should().BeFalse();
        deserialized.ShowEmoji.Should().BeFalse();
        deserialized.MarkdownRendering.Should().BeFalse();
        deserialized.DefaultScreen.Should().Be("Roster");
    }

    [Fact]
    public void AppSettings_CorruptJson_DeserializesToNull()
    {
        var corrupt = "{ invalid json @@@ }";
        var act = () => System.Text.Json.JsonSerializer.Deserialize<AppSettings>(corrupt);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    #endregion

    #region Helpers

    private static ServiceProvider CreateServiceProvider(
        ITeamService? teamService = null,
        IDecisionService? decisionService = null,
        ISkillService? skillService = null,
        IOrchestrationLogService? logService = null)
    {
        teamService ??= Substitute.For<ITeamService>();
        decisionService ??= Substitute.For<IDecisionService>();
        skillService ??= Substitute.For<ISkillService>();
        logService ??= Substitute.For<IOrchestrationLogService>();

        var squadData = Substitute.For<ISquadDataProvider>();
        return new ServiceProvider(squadData, teamService, decisionService, skillService, logService);
    }

    #endregion
}
