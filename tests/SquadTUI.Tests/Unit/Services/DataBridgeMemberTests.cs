using LanguageExt;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SquadTUI.Models;
using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class DataBridgeMemberTests
{
    private static ServiceProvider CreateServiceProvider(ITeamService? teamService = null)
    {
        teamService ??= Substitute.For<ITeamService>();
        var decisionService = Substitute.For<IDecisionService>();
        var skillService = Substitute.For<ISkillService>();
        var logService = Substitute.For<IOrchestrationLogService>();
        var squadData = Substitute.For<ISquadDataProvider>();
        return new ServiceProvider(squadData, teamService, decisionService, skillService, logService);
    }

    [Fact]
    public async Task AddMemberAsync_ReturnsRight_OnSuccess()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.AddMemberAsync("Basher", "QA", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sp = CreateServiceProvider(teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.AddMemberAsync("Basher", "QA");
        Assert.True(result.IsRight);
    }

    [Fact]
    public async Task RemoveMemberAsync_ReturnsRight_OnSuccess()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.RemoveMemberAsync("Basher", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sp = CreateServiceProvider(teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.RemoveMemberAsync("Basher");
        Assert.True(result.IsRight);
    }

    [Fact]
    public async Task AddMemberAsync_ReturnsLeft_OnError()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.AddMemberAsync("Basher", "QA", Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("team.md does not exist"));

        var sp = CreateServiceProvider(teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.AddMemberAsync("Basher", "QA");
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task RemoveMemberAsync_ReturnsLeft_OnError()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.RemoveMemberAsync("Basher", Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("Permission denied"));

        var sp = CreateServiceProvider(teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.RemoveMemberAsync("Basher");
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task AddMemberAsync_ErrorContainsServiceName()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.AddMemberAsync("X", "Y", Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("boom"));

        var sp = CreateServiceProvider(teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.AddMemberAsync("X", "Y");
        result.IfLeft(err => Assert.Contains("AddMember", err.Message));
    }

    [Fact]
    public async Task RemoveMemberAsync_ErrorContainsServiceName()
    {
        var teamService = Substitute.For<ITeamService>();
        teamService.RemoveMemberAsync("X", Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("boom"));

        var sp = CreateServiceProvider(teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.RemoveMemberAsync("X");
        result.IfLeft(err => Assert.Contains("RemoveMember", err.Message));
    }
}
