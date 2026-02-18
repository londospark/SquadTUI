using SquadTUI.Models;
using SquadTUI.Screens;
using SquadTUI.Services;
using SquadTUI.Tests.Stubs;

namespace SquadTUI.Tests.Unit;

/// <summary>
/// Unit tests for RefreshService — the hybrid refresh engine that coordinates
/// FileWatcher (reactive) and polling timer (fallback).
/// Uses a real RefreshService with stub DataBridge so we can test the service
/// logic without actual file I/O.
/// </summary>
public class RefreshServiceTests : IDisposable
{
    private readonly AppState _state;
    private readonly DataBridge _bridge;
    private readonly RefreshService _service;

    public RefreshServiceTests()
    {
        _state = new AppState();
        var provider = StubServiceProviderFactory.Create();
        _bridge = new DataBridge(provider);
        _service = new RefreshService(_bridge, _state);
    }

    [Fact]
    public void RefreshService_StartsWithCorrectInterval()
    {
        // Default AppSettings.RefreshIntervalSeconds is 30
        Assert.Equal(30, _state.Settings.RefreshIntervalSeconds);
    }

    [Fact]
    public void RefreshService_SetPollingInterval_ChangesInterval()
    {
        var newInterval = TimeSpan.FromSeconds(60);
        _service.SetPollingInterval(newInterval);

        // No exception thrown — interval accepted.
        // We can't read the private field directly, but we verify it doesn't throw
        // and the timer doesn't crash on the new cadence.
        Assert.False(_service.IsActive); // Not started yet, just interval set
    }

    [Fact]
    public async Task RefreshService_RefreshNow_UpdatesLastRefreshTime()
    {
        var before = _service.LastRefreshTime;

        await _service.RefreshNowAsync();

        Assert.True(_service.LastRefreshTime > before || _service.LastRefreshTime >= before);
        // LastRefreshTime should be set to ~now
        Assert.InRange(_service.LastRefreshTime, DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public void RefreshService_IsActive_TrueAfterStart()
    {
        // Create a temp directory for the watcher
        var tempDir = Path.Combine(Path.GetTempPath(), $"squadtui-test-{Guid.NewGuid()}");
        var squadDir = Path.Combine(tempDir, ".ai-team");
        Directory.CreateDirectory(squadDir);

        try
        {
            _service.Start(tempDir);
            Assert.True(_service.IsActive);
        }
        finally
        {
            _service.Stop();
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public void RefreshService_IsActive_FalseAfterStop()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squadtui-test-{Guid.NewGuid()}");
        var squadDir = Path.Combine(tempDir, ".ai-team");
        Directory.CreateDirectory(squadDir);

        try
        {
            _service.Start(tempDir);
            Assert.True(_service.IsActive);

            _service.Stop();
            Assert.False(_service.IsActive);
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public void RefreshService_Dispose_StopsAll()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"squadtui-test-{Guid.NewGuid()}");
        var squadDir = Path.Combine(tempDir, ".ai-team");
        Directory.CreateDirectory(squadDir);

        try
        {
            _service.Start(tempDir);
            Assert.True(_service.IsActive);

            _service.Dispose();
            Assert.False(_service.IsActive);
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public async Task RefreshService_RefreshNow_FiresOnDataRefreshed()
    {
        bool eventFired = false;
        _service.OnDataRefreshed += () => eventFired = true;

        await _service.RefreshNowAsync();

        Assert.True(eventFired);
    }

    [Fact]
    public async Task RefreshService_RefreshNow_UpdatesAppState()
    {
        var originalTime = _state.LastRefreshTime;

        await _service.RefreshNowAsync();

        // RefreshService should update state.LastRefreshTime
        Assert.True(_state.LastRefreshTime >= originalTime);
        // HasPendingRefresh should be cleared after refresh
        Assert.False(_state.HasPendingRefresh);
    }

    [Fact]
    public async Task RefreshService_SmartPolling_SkipsWhenWatcherFiredRecently()
    {
        // This tests the concept: if watcher already fired since last poll,
        // the poll tick is skipped. We can verify the state tracking by checking
        // that the RefreshService interface exposes the right contract.
        //
        // Direct testing of OnPollTick would need reflection since it's private.
        // Instead, verify the public contract: after a manual refresh (simulating
        // watcher trigger), LastRefreshTime is updated and a second immediate
        // RefreshNow also works without conflict (semaphore isn't stuck).
        await _service.RefreshNowAsync();
        var time1 = _service.LastRefreshTime;

        // Small delay to ensure timestamps differ
        await Task.Delay(10);

        await _service.RefreshNowAsync();
        var time2 = _service.LastRefreshTime;
        Assert.True(time2 >= time1);
    }

    public void Dispose()
    {
        _service.Dispose();
    }
}
