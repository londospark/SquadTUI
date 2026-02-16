namespace SquadTUI.Models;

public record DashboardData(
    TeamSummary Team,
    IReadOnlyList<BurndownDataPoint> Burndown,
    IReadOnlyList<VelocityDataPoint> Velocity,
    IReadOnlyList<ActivitySwimlane> Activity,
    IReadOnlyList<DecisionEntry> Decisions
);

public record TeamSummary(
    int TotalMembers,
    int ActiveMembers,
    IReadOnlyList<TeamMemberOverview> Members
);

public record TeamMemberOverview(
    string Name,
    string Role,
    MemberStatus Status,
    int TasksCompleted,
    int TasksInProgress
);

public record VelocityDataPoint(
    string Date,
    int Completed,
    int Added
);

public record ActivityHeatmapPoint(
    string Date,
    int Hour,
    int Count
);

public record TimelineTask(
    string Id,
    string Title,
    string Assignee,
    DateTimeOffset Start,
    DateTimeOffset? End
);

public record ActivitySwimlane(
    string MemberName,
    IReadOnlyList<TimelineTask> Tasks,
    IReadOnlyList<string> LogEntries
);

public record BurndownDataPoint(
    string Date,
    int Remaining,
    int Ideal
);

public record MilestoneBurndown(
    string Title,
    DateTimeOffset? DueDate,
    IReadOnlyList<BurndownDataPoint> DataPoints
);
