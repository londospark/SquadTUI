namespace SquadTUI.Models;

public record TeamRoster(
    string ProjectName,
    string? Description = null,
    IReadOnlyList<SquadMember>? Members = null
);
