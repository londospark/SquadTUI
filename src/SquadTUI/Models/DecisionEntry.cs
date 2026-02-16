namespace SquadTUI.Models;

public record DecisionEntry(
    string Title,
    string Date,
    string Author,
    string Content,
    string? FilePath = null,
    int? LineNumber = null
);
