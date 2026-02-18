namespace SquadTUI.Services;

public interface IRefreshService : IDisposable
{
    bool IsActive { get; }
    DateTime LastRefreshTime { get; }
    event Action? OnDataRefreshed;
    void Start(string squadRootPath);
    void Stop();
    Task RefreshNowAsync();
    void SetPollingInterval(TimeSpan interval);
}
