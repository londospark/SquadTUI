using LanguageExt;

namespace SquadTUI.Models;

public record SquadTask(
    string Id,
    string Title,
    Option<string> Description = default,
    SquadTaskStatus Status = SquadTaskStatus.Pending,
    Option<string> Assignee = default,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? CompletedAt = null
);
