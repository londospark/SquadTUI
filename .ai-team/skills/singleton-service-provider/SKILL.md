---
name: "singleton-service-provider"
description: "Pattern for initializing and providing access to service layer with automatic dependency resolution"
domain: "architecture, dependency-management"
confidence: "medium"
source: "earned"
---

## Context

Use this pattern when you need to:
- Initialize a suite of related services once and share them across an application
- Discover configuration or root paths at runtime (e.g., git root, .ai-team/ directory)
- Provide clean separation between service instantiation and service consumption
- Avoid passing service instances through multiple layers of constructors

Particularly useful in TUI/CLI applications where you don't have a full DI container but need organized service initialization.

## Patterns

**Singleton ServiceProvider with lazy initialization:**
```csharp
public class ServiceProvider
{
    private static ServiceProvider? _instance;
    private static readonly object _lock = new();
    
    public IFooService Foo { get; }
    public IBarService Bar { get; }
    
    private ServiceProvider(string rootPath)
    {
        Foo = new FooService(rootPath);
        Bar = new BarService(rootPath);
    }
    
    public static ServiceProvider Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new ServiceProvider(DiscoverRootPath());
                }
            }
            return _instance;
        }
    }
}
```

**Runtime path discovery strategy (fallback chain):**
1. Try `git rev-parse --show-toplevel` via Process — most reliable in repo
2. Walk up directory tree from `Directory.GetCurrentDirectory()` looking for marker (e.g., `.ai-team/`)
3. Fall back to relative path from `AppContext.BaseDirectory` (4 levels up for typical bin/Debug/net10.0 structure)

**DataBridge pattern — screen-friendly API over services:**
- Create a `DataBridge` class that wraps ServiceProvider
- Provide methods named for UI concerns: `LoadDashboardDataAsync()`, `LoadRosterDataAsync()`
- Translate service models into view-friendly formats
- UI layer depends on DataBridge, not on individual services

**Graceful fallback in UI:**
```csharp
var members = state.Members ?? SampleData.Members;
```
- AppState holds nullable loaded data
- Screens use null-coalescing to fall back to static sample data
- App works even if file system is missing/malformed

## Examples

**ServiceProvider usage in Program.cs:**
```csharp
var bridge = new DataBridge(ServiceProvider.Instance);
_ = Task.Run(async () =>
{
    state.Members = await bridge.LoadRosterDataAsync();
    state.Decisions = await bridge.LoadDecisionsDataAsync();
});
```

**Path normalization for cross-platform:**
```csharp
// Git returns forward slashes even on Windows
gitRoot = gitRoot.Replace('/', Path.DirectorySeparatorChar);
```

## Anti-Patterns

- **Don't expose raw services to UI layer** — wrap them in a DataBridge with UI-centric methods
- **Don't assume CWD is repo root** — always discover via git or directory walk
- **Don't throw if services fail to initialize** — catch exceptions, set error state, continue with fallback data
- **Don't instantiate services directly in screens** — always go through ServiceProvider
- **Don't make services stateful** — keep them pure file parsers; state lives in AppState
