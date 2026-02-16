namespace SquadTUI.Models;

public record GitHubLabel(
    string Name,
    string? Color = null,
    string? Description = null
);

public record GitHubMilestone(
    int Number,
    string Title,
    string? Description = null,
    string? State = null,
    DateTimeOffset? DueOn = null
);

public record GitHubIssue(
    int Number,
    string Title,
    string? Body = null,
    string State = "open",
    string? Assignee = null,
    IReadOnlyList<GitHubLabel>? Labels = null,
    GitHubMilestone? Milestone = null,
    DateTimeOffset CreatedAt = default,
    DateTimeOffset? ClosedAt = null
);
