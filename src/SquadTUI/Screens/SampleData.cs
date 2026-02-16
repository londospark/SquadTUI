using SquadTUI.Models;

namespace SquadTUI.Screens;

public static class SampleData
{
    public static readonly IReadOnlyList<SquadMember> Members =
    [
        new("Danny", "Lead", MemberStatus.Active, "Architecture planning"),
        new("Linus", "Frontend/TUI Dev", MemberStatus.Active, "Building TUI screens"),
        new("Rusty", "Backend Dev", MemberStatus.Active, "Service implementations"),
        new("Basher", "Tester", MemberStatus.Active, "Writing test suite"),
        new("Saul", "UX/Design", MemberStatus.Active, "Navigation design"),
        new("Scribe", "Scribe", MemberStatus.Idle),
    ];

    public static readonly IReadOnlyList<DecisionEntry> Decisions =
    [
        new("Project Architecture", "2026-02-16", "Danny",
            "Use Hex1b for TUI framework. .NET 10 target. Clean separation of Models, Services, and Screens layers."),
        new("UX Design", "2026-02-16", "Saul",
            "Sidebar navigation with number keys. HStack layout with list on left and detail preview on right for browse screens."),
        new("Data Layer", "2026-02-16", "Rusty",
            "File-based data providers reading from .ai-team directory. Interfaces first, implementations second."),
        new("Testing Strategy", "2026-02-17", "Basher",
            "Use Hex1b headless mode for TUI testing. Unit tests for services, integration tests for data providers."),
    ];

    public static readonly IReadOnlyList<Skill> Skills =
    [
        new("code-review", "Reviews code changes for quality and correctness"),
        new("testing", "Writes and runs automated tests"),
        new("documentation", "Generates and maintains project documentation"),
        new("refactoring", "Improves code structure without changing behavior"),
        new("debugging", "Diagnoses and fixes bugs in the codebase"),
    ];

    public static readonly IReadOnlyList<OrchestrationLogEntry> LogEntries =
    [
        new(DateTimeOffset.Parse("2026-02-17T10:00:00Z"), "2026-02-17", "Sprint Planning",
            ["Danny", "Linus", "Rusty", "Basher", "Saul"],
            "Planned sprint 1 tasks. Assigned TUI to Linus, services to Rusty.",
            ["Use Hex1b framework", "File-based data layer"],
            ["Sprint backlog created", "Roles assigned"]),
        new(DateTimeOffset.Parse("2026-02-16T14:00:00Z"), "2026-02-16", "Architecture Review",
            ["Danny", "Rusty"],
            "Reviewed project architecture. Decided on clean layered approach.",
            ["Models/Services/Screens separation"],
            ["Architecture document created"]),
        new(DateTimeOffset.Parse("2026-02-16T09:00:00Z"), "2026-02-16", "Project Kickoff",
            ["Danny", "Linus", "Rusty", "Basher", "Saul", "Scribe"],
            "Initial project setup. Created repository structure and assigned roles.",
            ["Project name: SquadTUI", ".NET 10 target"],
            ["Repository initialized", "Team roster created"]),
    ];

    public static readonly IReadOnlyList<SquadTask> Tasks =
    [
        new("task-1", "Build TUI screens", "Create all screen components", SquadTaskStatus.InProgress, "Linus"),
        new("task-2", "Implement services", "Build data provider services", SquadTaskStatus.InProgress, "Rusty"),
        new("task-3", "Write test suite", "Automated tests for all components", SquadTaskStatus.Pending, "Basher"),
        new("task-4", "UX review", "Review navigation flow", SquadTaskStatus.Done, "Saul"),
        new("task-5", "Architecture doc", "Document system architecture", SquadTaskStatus.Done, "Danny"),
    ];

    public static string GetCharterFor(string memberName) => memberName switch
    {
        "Danny" => "# Danny — Lead\n\nResponsible for project direction, architecture decisions, and team coordination.\n\n## Goals\n- Keep the team aligned\n- Make architectural decisions\n- Remove blockers",
        "Linus" => "# Linus — Frontend/TUI Dev\n\nBuilds all terminal UI screens using the Hex1b framework.\n\n## Goals\n- Create intuitive TUI navigation\n- Build all screen components\n- Ensure responsive layout",
        "Rusty" => "# Rusty — Backend Dev\n\nImplements service layer and data providers.\n\n## Goals\n- Build file-based data providers\n- Implement service interfaces\n- Ensure data integrity",
        _ => $"# {memberName}\n\nNo charter available yet."
    };
}
