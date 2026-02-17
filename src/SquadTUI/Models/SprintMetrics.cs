namespace SquadTUI.Models;

/// <summary>Historical metrics for a single sprint.</summary>
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
    /// <summary>Completion rate as a percentage (0–100).</summary>
    public double CompletionRate => PlannedTasks > 0
        ? Math.Round((double)CompletedTasks / PlannedTasks * 100, 1)
        : 0;

    /// <summary>Velocity expressed as completed tasks per sprint.</summary>
    public int Velocity => CompletedTasks;
}

/// <summary>A single member's contribution within a sprint.</summary>
public record MemberContribution(
    string MemberName,
    int TasksCompleted,
    int TasksAssigned,
    int PointsEarned
)
{
    /// <summary>Utilization: ratio of completed to assigned (0–100%).</summary>
    public double Utilization => TasksAssigned > 0
        ? Math.Round((double)TasksCompleted / TasksAssigned * 100, 1)
        : 0;
}
