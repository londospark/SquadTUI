using SquadTUI.Services;

namespace SquadTUI.Tests.Unit.Services;

public class FileWatcherServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _squadDir;

    public FileWatcherServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "SquadTUI_FW_" + Guid.NewGuid().ToString("N"));
        _squadDir = Path.Combine(_tempDir, ".squad");
        Directory.CreateDirectory(_squadDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void IsWatching_IsFalse_ByDefault()
    {
        using var svc = new FileWatcherService();
        Assert.False(svc.IsWatching);
    }

    [Fact]
    public void Start_SetsIsWatchingTrue()
    {
        using var svc = new FileWatcherService();
        svc.Start(_tempDir);
        Assert.True(svc.IsWatching);
    }

    [Fact]
    public void Stop_SetsIsWatchingFalse()
    {
        using var svc = new FileWatcherService();
        svc.Start(_tempDir);
        svc.Stop();
        Assert.False(svc.IsWatching);
    }

    [Fact]
    public void Start_DoesNothing_WhenDirectoryMissing()
    {
        using var svc = new FileWatcherService();
        var nonExistent = Path.Combine(Path.GetTempPath(), "SquadTUI_NoExist_" + Guid.NewGuid().ToString("N"));
        svc.Start(nonExistent);
        Assert.False(svc.IsWatching);
    }

    [Fact]
    public void Start_CalledTwice_DoesNotThrow()
    {
        using var svc = new FileWatcherService();
        svc.Start(_tempDir);
        svc.Start(_tempDir); // idempotent
        Assert.True(svc.IsWatching);
    }

    [Fact]
    public void Dispose_StopsWatching()
    {
        var svc = new FileWatcherService();
        svc.Start(_tempDir);
        svc.Dispose();
        Assert.False(svc.IsWatching);
    }

    [Fact]
    public async Task OnFilesChanged_FiresWhenFileCreated()
    {
        using var svc = new FileWatcherService();
        var fired = false;
        svc.OnFilesChanged += () => fired = true;
        svc.Start(_tempDir);

        await File.WriteAllTextAsync(Path.Combine(_squadDir, "test.md"), "hello");

        // Wait for the event to propagate (FileSystemWatcher is async)
        await Task.Delay(500);
        Assert.True(fired);
    }

    [Fact]
    public async Task OnFilesChanged_DebouncesPreviousEvents()
    {
        using var svc = new FileWatcherService();
        var fireCount = 0;
        svc.OnFilesChanged += () => Interlocked.Increment(ref fireCount);
        svc.Start(_tempDir);

        // Rapid writes should be debounced
        for (var i = 0; i < 5; i++)
        {
            await File.WriteAllTextAsync(Path.Combine(_squadDir, $"test{i}.md"), $"content{i}");
            await Task.Delay(50);
        }

        await Task.Delay(500);
        // Debounce (2s window) means only 1 event should fire for rapid writes
        Assert.True(fireCount > 0 && fireCount <= 2);
    }
}
