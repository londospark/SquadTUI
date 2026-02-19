using LanguageExt;

namespace SquadTUI.Models;

public record SquadMember(
    string Name,
    string Role,
    MemberStatus Status = MemberStatus.Active,
    Option<string> CurrentTask = default,
    Option<string> CharterPath = default,
    Option<DateTime> LastActivity = default
);
