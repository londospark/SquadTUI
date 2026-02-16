namespace SquadTUI.Models;

public record Skill(
    string Name,
    string Description,
    string? Source = null,
    string? Confidence = null,
    string? Content = null,
    string? Slug = null
);
