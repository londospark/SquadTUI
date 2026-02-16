namespace SquadTUI.Models;

public record SquadMember(
    string Name,
    string Role,
    MemberStatus Status = MemberStatus.Active,
    string? CurrentTask = null,
    string? CharterPath = null
);
