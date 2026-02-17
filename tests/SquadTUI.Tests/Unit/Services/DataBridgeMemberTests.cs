using LanguageExt;
using SquadTUI.Models;
using SquadTUI.Services;
using SquadTUI.Tests.Stubs;

namespace SquadTUI.Tests.Unit.Services;

public class DataBridgeMemberTests
{
    [Fact]
    public async Task AddMemberAsync_ReturnsRight_OnSuccess()
    {
        var teamService = new StubTeamService();
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.AddMemberAsync("Basher", "QA");
        Assert.True(result.IsRight);
    }

    [Fact]
    public async Task RemoveMemberAsync_ReturnsRight_OnSuccess()
    {
        var teamService = new StubTeamService();
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.RemoveMemberAsync("Basher");
        Assert.True(result.IsRight);
    }

    [Fact]
    public async Task AddMemberAsync_ReturnsLeft_OnError()
    {
        var teamService = new StubTeamService
        {
            AddMemberException = new InvalidOperationException("team.md does not exist")
        };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.AddMemberAsync("Basher", "QA");
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task RemoveMemberAsync_ReturnsLeft_OnError()
    {
        var teamService = new StubTeamService
        {
            RemoveMemberException = new IOException("Permission denied")
        };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.RemoveMemberAsync("Basher");
        Assert.True(result.IsLeft);
    }

    [Fact]
    public async Task AddMemberAsync_ErrorContainsServiceName()
    {
        var teamService = new StubTeamService { AddMemberException = new Exception("boom") };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.AddMemberAsync("X", "Y");
        result.IfLeft(err => Assert.Contains("AddMember", err.Message));
    }

    [Fact]
    public async Task RemoveMemberAsync_ErrorContainsServiceName()
    {
        var teamService = new StubTeamService { RemoveMemberException = new Exception("boom") };
        var sp = StubServiceProviderFactory.Create(team: teamService);
        var bridge = new DataBridge(sp);

        var result = await bridge.RemoveMemberAsync("X");
        result.IfLeft(err => Assert.Contains("RemoveMember", err.Message));
    }
}
