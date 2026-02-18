# Live Update Strategy — Options for User Decision

**Author:** Solaire (Lead)
**Date:** 2025-01-27
**Status:** Awaiting user decision

> **This is your call.** I'm presenting trade-offs, not making the decision. Pick the option that matches how you actually use SquadTUI.

---

## Current Implementation (What We Have Today)

Three independent refresh mechanisms running simultaneously:

| Layer | Mechanism | Interval | Location |
|-------|-----------|----------|----------|
| **FileWatcher** | `FileSystemWatcher` on `.ai-team/` directory | 2s debounce | `FileWatcherService.cs` |
| **Polling Timer** | `System.Threading.Timer` | Every 30s | `Program.cs` line 69-86 |
| **UI Redraw** | `RedrawAfter(3000)` on Dashboard & Metrics | Every 3s | `DashboardScreen.cs`, `MetricsScreen.cs` |

**Problem:** All three fire independently. The polling timer reloads ALL data every 30 seconds even if nothing changed. The FileWatcher and polling timer both do the exact same reload (members, tasks, decisions, logs). The UI redraw ticks every 3 seconds regardless. This is redundant work.

---

## Option A: Hybrid — FileWatcher + Configurable Polling (Recommended)

Keep both mechanisms but make them cooperate instead of duplicating work.

### How It Works
- **FileWatcher** handles immediate updates (file saved → data reloads in ~2s)
- **Polling** acts as a safety net, but only reloads if FileWatcher hasn't triggered recently
- **Polling interval** is user-configurable in Settings: 15s / 30s / 60s / 120s / Off
- **`RedrawAfter()`** reduced to match polling interval or removed entirely (FileWatcher + polling already trigger redraws)

### Architecture Change
```
Program.cs → IRefreshService.Start(RefreshMode.Hybrid, interval: settings.PollInterval)
```

One service, one event, one reload path. No triple-copy.

### Pros
- ✅ Near-instant updates from FileWatcher when it works
- ✅ Polling catches anything FileWatcher misses
- ✅ User controls the CPU/freshness trade-off
- ✅ Eliminates duplicate reload logic
- ✅ "Off" polling option for users on battery / slow machines

### Cons
- ⚠️ FileSystemWatcher still unreliable on some platforms (macOS FSEvents, network drives, WSL1)
- ⚠️ Two mechanisms = slightly more code than pure polling
- ⚠️ Need to track "last watcher event" to skip redundant polls

### Effort: Low-Medium
Extract `IRefreshService`, add `PollInterval` to `AppSettings`, wire to Settings screen.

---

## Option B: Pure FileWatcher (No Polling)

Remove the polling timer entirely. Trust FileSystemWatcher.

### How It Works
- FileWatcher is the only automatic refresh trigger
- Add `R` key for manual refresh (safety valve)
- Add a heartbeat health-check: if no events in 5 minutes, show "⚠ Watcher may be stale — press R to refresh"

### Pros
- ✅ Zero CPU overhead when nothing changes
- ✅ Near-instant updates (~2s debounce)
- ✅ Simplest implementation — one mechanism
- ✅ Best for battery life on laptops

### Cons
- ❌ **FileSystemWatcher is unreliable.** Known to miss events on:
  - macOS (FSEvents has documented gaps)
  - Network/mounted drives (NFS, SMB, SSHFS)
  - WSL1 (no inotify support)
  - Containers with mounted volumes
- ❌ If watcher dies silently, user sees stale data with no indication
- ❌ Manual refresh (`R` key) is a poor UX — users forget it exists

### Effort: Low
Remove polling timer, add `R` key binding, add staleness indicator.

### My Take
Too risky as the sole mechanism. FileSystemWatcher's reliability issues are well-documented and platform-specific. You'd need to test on every target platform. Not worth the fragility for a 30-second polling timer.

---

## Option C: Configurable Polling Only (No FileWatcher)

Remove FileWatcher entirely. Poll at a user-configured interval.

### How It Works
- Single `System.Threading.Timer` at configurable interval
- Settings: 5s / 15s / 30s / 60s
- Add `R` key for immediate manual refresh
- Show "Updated: HH:mm:ss" in footer (already exists)

### Pros
- ✅ **Most reliable** — works identically on all platforms
- ✅ Simplest code — one timer, no watcher setup, no event handlers
- ✅ Predictable CPU usage (proportional to interval)
- ✅ Works on network drives, WSL, containers, everything
- ✅ Easy to test — just mock the timer

### Cons
- ❌ Updates are delayed by up to the poll interval (5-60s)
- ❌ Short intervals (5s) waste CPU reading files that haven't changed
- ❌ No "instant" feedback when you save a file
- ❌ 60s interval feels sluggish for interactive use

### Effort: Low
Remove FileWatcher code, make timer interval configurable, add `R` key.

### My Take
Solid if you value reliability over responsiveness. The 15s sweet spot gives "good enough" freshness without noticeable CPU overhead. Most CI/CD dashboards use polling — it's a proven pattern.

---

## Option D: Event-Driven via Git Hooks

Trigger refresh only when git operations happen.

### How It Works
- Install `post-commit`, `post-merge`, `post-checkout` hooks in `.git/hooks/`
- Hooks signal the TUI via a named pipe, Unix socket, or temp file
- TUI refreshes only when git state changes

### Pros
- ✅ Zero overhead between git operations
- ✅ Semantically correct — refresh when repo state actually changes
- ✅ Good for CI/CD integration

### Cons
- ❌ **Doesn't catch direct file edits.** If someone edits `.ai-team/team.md` without committing, the TUI never updates.
- ❌ Complex setup — need to install hooks, handle hook permissions, deal with existing hooks
- ❌ Fragile — hooks can be overwritten by `git init`, `.husky`, or other hook managers
- ❌ Doesn't work if squad files are edited by AI agents that commit later, not immediately
- ❌ IPC mechanism (pipes/sockets) adds cross-platform complexity

### Effort: High
Hook installation, IPC mechanism, cross-platform socket handling, conflict with existing hooks.

### My Take
Wrong tool for this job. SquadTUI monitors a directory of markdown files that change frequently outside git. Git hooks are great for CI, terrible for interactive file monitoring. **I'd reject this for SquadTUI.**

---

## Comparison Matrix

| Criteria | A: Hybrid | B: Pure Watcher | C: Pure Polling | D: Git Hooks |
|----------|-----------|-----------------|-----------------|--------------|
| **Responsiveness** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |
| **Reliability** | ⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐ |
| **CPU Efficiency** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Cross-Platform** | ⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐ |
| **Simplicity** | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐ |
| **Implementation Effort** | Low-Med | Low | Low | High |

---

## My Recommendation

**Option A (Hybrid)** is the right default, but with a key refinement: make polling smarter.

Instead of blindly reloading every 30 seconds, the poll should:
1. Check if FileWatcher has fired since last poll — if yes, skip
2. Only reload data sources whose files have changed (compare last-modified timestamps)
3. Default to 30s, configurable down to 15s or up to 120s

This gives you the best of both worlds: instant updates when FileWatcher works (which is ~95% of the time on Windows and Linux), with polling as a reliable fallback for the edge cases.

But if you primarily run on a single platform (Windows) and don't care about network drives or containers, **Option C (Polling at 15s)** is the simplest path with no reliability surprises.

---

## Regardless of Choice: Quick Wins

These improvements apply no matter which option you pick:

1. **`R` key for manual refresh** — always useful, trivial to add
2. **Extract `IRefreshService`** — eliminates the triple-copy in Program.cs
3. **Add `RefreshInterval` to `AppSettings`** — one new property, wire to Settings screen
4. **Remove redundant `RedrawAfter(3000)`** — the refresh service already triggers redraws

---

*Your call. Tell me which option and I'll scope the implementation work. — Solaire*
