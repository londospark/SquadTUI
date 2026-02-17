using LanguageExt;

namespace SquadTUI.Models;

public record Skill(
    string Name,
    string Description,
    Option<string> Source = default,
    string Confidence = "medium",
    Option<string> Content = default,
    Option<string> Slug = default
);
