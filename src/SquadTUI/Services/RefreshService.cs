using SquadTUI.Screens;

namespace SquadTUI.Services;

/// <summary>
/// Hybrid refresh: owns both a FileWatcherService (reactive) and a polling timer (fallback).
/// Smart polling skips the tick when the watcher already fired since the last poll.
/// </summary>
public sealed class RefreshService : IRefreshService
{
    private readonly DataBridge _bridge;
    private readonly AppState _state;
    private readonly FileWatcherService _watcher = new();
    private Timer? _timer;
    private DateTime _lastWatcherEvent = DateTime.MinValue;
    private DateTime _lastPollTime = DateTime.MinValue;
    private TimeSpan _pollingInterval;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public bool IsActive => _watcher.IsWatching;
    public DateTime LastRefreshTime { get; private set; }
    public event Action? OnDataRefreshed;

    public RefreshService(DataBridge bridge, AppState state)
    {
        _bridge = bridge;
        _state = state;
        _pollingInterval = TimeSpan.FromSeconds(state.Settings.RefreshIntervalSeconds);
    }

    public void Start(string squadRootPath)
    {
        _watcher.OnFilesChanged += OnWatcherTriggered;
        _watcher.Start(squadRootPath);
        _timer = new Timer(OnPollTick, null, _pollingInterval, _pollingInterval);
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
        _watcher.OnFilesChanged -= OnWatcherTriggered;
        _watcher.Stop();
    }

    public async Task RefreshNowAsync()
    {
        if (!_refreshLock.Wait(0)) return;
        try
        {
            await ReloadAllAsync();
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public void SetPollingInterval(TimeSpan interval)
    {
        _pollingInterval = interval;
        _timer?.Change(interval, interval);
    }

    public void Dispose()
    {
        Stop();
        _watcher.Dispose();
        _refreshLock.Dispose();
    }

    private void OnWatcherTriggered()
    {
        _lastWatcherEvent = DateTime.UtcNow;
        _state.HasPendingRefresh = true;
        _ = Task.Run(async () =>
        {
            if (!_refreshLock.Wait(0)) return;
            try { await ReloadAllAsync(); }
            finally { _refreshLock.Release(); }
        });
    }

    private void OnPollTick(object? _)
    {
        // Skip poll if the watcher already fired since the last poll
        if (_lastWatcherEvent > _lastPollTime)
        {
            _lastPollTime = DateTime.UtcNow;
            return;
        }

        _lastPollTime = DateTime.UtcNow;
        if (_state.SquadRootPath == null) return;

        _ = Task.Run(async () =>
        {
            if (!_refreshLock.Wait(0)) return;
            try { await ReloadAllAsync(); }
            finally { _refreshLock.Release(); }
        });
    }

    private async Task ReloadAllAsync()
    {
        var membersTask = _bridge.LoadRosterDataAsync();
        var tasksTask = _bridge.LoadTasksFromRosterAsync();
        var decisionsTask = _bridge.LoadDecisionsDataAsync();
        var logsTask = _bridge.LoadLogDataAsync();
        var skillsTask = _bridge.LoadSkillsDataAsync();
        await Task.WhenAll(membersTask, tasksTask, decisionsTask, logsTask, skillsTask);

        _state.Members = await membersTask;
        _state.Tasks = await tasksTask;
        _state.Decisions = await decisionsTask;
        _state.LogEntries = await logsTask;
        _state.Skills = await skillsTask;

        LastRefreshTime = DateTime.Now;
        _state.LastRefreshTime = LastRefreshTime;
        _state.HasPendingRefresh = false;
        OnDataRefreshed?.Invoke();
    }
}
