namespace SquadTUI.Models;

public record OrchestrationLogEntry(
    DateTimeOffset Timestamp,
    string Date,
    string Topic,
    IReadOnlyList<string> Participants,
    string Summary,
    IReadOnlyList<string> Decisions,
    IReadOnlyList<string> Outcomes,
    string? WhatWasDone = null
);
