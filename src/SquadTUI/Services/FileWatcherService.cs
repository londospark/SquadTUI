namespace SquadTUI.Services;

/// <summary>
/// Watches the squad directory for file changes and raises events with debouncing.
/// Used to trigger live dashboard refreshes when squad files are modified.
/// </summary>
public class FileWatcherService : IDisposable
{
    private FileSystemWatcher? _watcher;
    private DateTime _lastChange = DateTime.MinValue;
    private readonly TimeSpan _debounce = TimeSpan.FromSeconds(2);

    public event Action? OnFilesChanged;
    public bool IsWatching { get; private set; }

    public void Start(string squadRootPath)
    {
        if (_watcher != null) return;

        var squadDir = SquadPathResolver.Resolve(squadRootPath);
        if (!Directory.Exists(squadDir)) return;

        _watcher = new FileSystemWatcher(squadDir)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            EnableRaisingEvents = true
        };
        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += (s, e) => OnChanged(s, e);
        IsWatching = true;
    }

    private void OnChanged(object? sender, FileSystemEventArgs e)
    {
        var now = DateTime.UtcNow;
        if (now - _lastChange < _debounce) return;
        _lastChange = now;
        OnFilesChanged?.Invoke();
    }

    public void Stop()
    {
        _watcher?.Dispose();
        _watcher = null;
        IsWatching = false;
    }

    public void Dispose() => Stop();
}
