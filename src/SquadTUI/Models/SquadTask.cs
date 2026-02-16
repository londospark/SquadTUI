namespace SquadTUI.Models;

public record SquadTask(
    string Id,
    string Title,
    string? Description = null,
    SquadTaskStatus Status = SquadTaskStatus.Pending,
    string? Assignee = null,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? CompletedAt = null
);
