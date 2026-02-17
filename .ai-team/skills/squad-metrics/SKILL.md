---
name: "squad-metrics"
description: "Patterns and best practices for tracking squad velocity, burndown, and member contributions in a TUI dashboard"
domain: "metrics-analytics"
confidence: "high"
source: "manual"
---

# Squad Metrics Skill

## Purpose
Provides patterns and best practices for tracking squad velocity, burndown, and member contributions in a TUI dashboard.

## Data Model

### SprintMetrics
- `SprintNumber`, `SprintName`, `StartDate`, `EndDate`
- `PlannedTasks`, `CompletedTasks`, `CarriedOver`
- `Contributions` — list of `MemberContribution`
- Computed: `CompletionRate` (% of planned tasks completed), `Velocity` (completed tasks per sprint)

```csharp
public record SprintMetrics(
    int SprintNumber,
    string SprintName,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int PlannedTasks,
    int CompletedTasks,
    int CarriedOver,
    IReadOnlyList<MemberContribution> Contributions
)
{
    public double CompletionRate => PlannedTasks > 0
        ? Math.Round((double)CompletedTasks / PlannedTasks * 100, 1) : 0;
    public int Velocity => CompletedTasks;
}
```

### MemberContribution
- `MemberName`, `TasksCompleted`, `TasksAssigned`, `PointsEarned`
- Computed: `Utilization` (tasks completed / tasks assigned, 0–100%)

```csharp
public record MemberContribution(
    string MemberName,
    int TasksCompleted,
    int TasksAssigned,
    int PointsEarned
)
{
    public double Utilization => TasksAssigned > 0
        ? Math.Round((double)TasksCompleted / TasksAssigned * 100, 1) : 0;
}
```

## Metrics to Track

1. **Velocity** — completed tasks per sprint (trend over time)
2. **Burndown** — remaining work per sprint
3. **Completion Rate** — % of planned tasks completed
4. **Member Utilization** — tasks completed / tasks assigned per member
5. **Velocity Trend** — acceleration/deceleration between sprints

## Aggregate Helpers

These helpers in `SampleData` provide cross-sprint rollups:

| Helper | What it returns |
|--------|----------------|
| `OverallCompletionRate` | Total completed / total planned across all sprints |
| `AverageVelocity` | Mean completed tasks per sprint |
| `VelocityTrend` | Difference between last two sprints' velocity (positive = accelerating) |
| `TeamUtilization` | Per-member utilization across all sprints, sorted descending |

## Visualization Patterns (Hex1b)

- `BarChart` for per-member task contributions
- Progress bar (ANSI block characters `█▓░`) for sprint completion
- Responsive layouts: 3-column (≥120 cols), 2-column (≥80), single-column (<80)

```csharp
// Per-member bar chart data
var chartData = members.Select(m =>
{
    var completed = tasks.Count(t => t.Assignee == m.Name && t.Status == SquadTaskStatus.Done);
    var inProgress = tasks.Count(t => t.Assignee == m.Name && t.Status == SquadTaskStatus.InProgress);
    return new ChartItem(m.Name, completed + inProgress);
}).Where(c => c.Value > 0).ToArray();

v.BarChart(chartData).Fill();
```

## Data Collection

- Parse sprint data from `.ai-team/log/` entries
- Track task completion from `.ai-team/agents/` task assignments
- Aggregate contributions from decision and ceremony logs

## Best Practices

- Keep at least 3 sprints of history for trend analysis
- Use consistent sprint durations for fair velocity comparison
- Include carried-over items in burndown calculations
- Show utilization alongside velocity to spot overload
- Display both absolute numbers and percentages
- Use responsive layouts so metrics remain readable in narrow terminals

## Integration

This skill works with SquadTUI's `MetricsScreen` to provide:
- Real-time dashboard updates via `FileWatcherService`
- Toggle between velocity and burndown views (`V` key)
- Per-member contribution breakdowns with bar charts
- Responsive layouts for different terminal sizes (wide, medium, narrow)

## Anti-Patterns

- **Comparing velocity across squads** — Velocity is relative; only compare a squad against its own history.
- **Ignoring carried-over items** — Always include `CarriedOver` in burndown to reflect true remaining work.
- **Using points alone** — Track both task counts and points; one without the other hides bottlenecks.
- **Hardcoding sprint duration** — Use `StartDate`/`EndDate` from `SprintMetrics`, not assumptions.
