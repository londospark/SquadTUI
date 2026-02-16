using SquadTUI.Models;

namespace SquadTUI.Screens;

public static class SampleData
{
    public static readonly IReadOnlyList<SquadMember> Members =
    [
        new("Solaire", "Lead", MemberStatus.Active, "Architecture planning"),
        new("Siegmeyer", "Frontend/TUI Dev", MemberStatus.Active, "Building TUI screens"),
        new("Andre", "Backend Dev", MemberStatus.Active, "Service implementations"),
        new("Patches", "Tester", MemberStatus.Active, "Writing test suite"),
        new("Firekeeper", "UX/Design", MemberStatus.Active, "Navigation design"),
        new("Scribe", "Scribe", MemberStatus.Idle),
    ];

    public static readonly IReadOnlyList<DecisionEntry> Decisions =
    [
        new("Project Architecture", "2026-02-16", "Solaire",
            "Use Hex1b for TUI framework. .NET 10 target. Clean separation of Models, Services, and Screens layers."),
        new("UX Design", "2026-02-16", "Firekeeper",
            "Sidebar navigation with number keys. HStack layout with list on left and detail preview on right for browse screens."),
        new("Data Layer", "2026-02-16", "Andre",
            "File-based data providers reading from .ai-team directory. Interfaces first, implementations second."),
        new("Testing Strategy", "2026-02-17", "Patches",
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
            ["Solaire", "Siegmeyer", "Andre", "Patches", "Firekeeper"],
            "Planned sprint 1 tasks. Assigned TUI to Siegmeyer, services to Andre.",
            ["Use Hex1b framework", "File-based data layer"],
            ["Sprint backlog created", "Roles assigned"]),
        new(DateTimeOffset.Parse("2026-02-16T14:00:00Z"), "2026-02-16", "Architecture Review",
            ["Solaire", "Andre"],
            "Reviewed project architecture. Decided on clean layered approach.",
            ["Models/Services/Screens separation"],
            ["Architecture document created"]),
        new(DateTimeOffset.Parse("2026-02-16T09:00:00Z"), "2026-02-16", "Project Kickoff",
            ["Solaire", "Siegmeyer", "Andre", "Patches", "Firekeeper", "Scribe"],
            "Initial project setup. Created repository structure and assigned roles.",
            ["Project name: SquadTUI", ".NET 10 target"],
            ["Repository initialized", "Team roster created"]),
    ];

    public static readonly IReadOnlyList<SquadTask> Tasks =
    [
        new("task-1", "Build TUI screens", "Create all screen components", SquadTaskStatus.InProgress, "Siegmeyer"),
        new("task-2", "Implement services", "Build data provider services", SquadTaskStatus.InProgress, "Andre"),
        new("task-3", "Write test suite", "Automated tests for all components", SquadTaskStatus.Pending, "Patches"),
        new("task-4", "UX review", "Review navigation flow", SquadTaskStatus.Done, "Firekeeper"),
        new("task-5", "Architecture doc", "Document system architecture", SquadTaskStatus.Done, "Solaire"),
    ];

    // --- Sprint velocity history (3 sprints, Dark Souls themed) ---

    public static readonly IReadOnlyList<SprintMetrics> SprintHistory =
    [
        new(1, "Undead Burg Sprint",
            DateTimeOffset.Parse("2026-02-03T00:00:00Z"),
            DateTimeOffset.Parse("2026-02-09T23:59:59Z"),
            PlannedTasks: 8, CompletedTasks: 5, CarriedOver: 3,
            Contributions:
            [
                new("Solaire",   TasksCompleted: 2, TasksAssigned: 2, PointsEarned: 5),
                new("Siegmeyer", TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 3),
                new("Andre",     TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 3),
                new("Patches",   TasksCompleted: 1, TasksAssigned: 1, PointsEarned: 2),
                new("Firekeeper",TasksCompleted: 0, TasksAssigned: 1, PointsEarned: 0),
            ]),
        new(2, "Sen's Fortress Sprint",
            DateTimeOffset.Parse("2026-02-10T00:00:00Z"),
            DateTimeOffset.Parse("2026-02-16T23:59:59Z"),
            PlannedTasks: 10, CompletedTasks: 7, CarriedOver: 3,
            Contributions:
            [
                new("Solaire",   TasksCompleted: 2, TasksAssigned: 2, PointsEarned: 5),
                new("Siegmeyer", TasksCompleted: 2, TasksAssigned: 3, PointsEarned: 5),
                new("Andre",     TasksCompleted: 2, TasksAssigned: 3, PointsEarned: 5),
                new("Patches",   TasksCompleted: 1, TasksAssigned: 1, PointsEarned: 2),
                new("Firekeeper",TasksCompleted: 0, TasksAssigned: 1, PointsEarned: 1),
            ]),
        new(3, "Anor Londo Sprint",
            DateTimeOffset.Parse("2026-02-17T00:00:00Z"),
            DateTimeOffset.Parse("2026-02-23T23:59:59Z"),
            PlannedTasks: 12, CompletedTasks: 4, CarriedOver: 8,
            Contributions:
            [
                new("Solaire",   TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 3),
                new("Siegmeyer", TasksCompleted: 1, TasksAssigned: 3, PointsEarned: 3),
                new("Andre",     TasksCompleted: 1, TasksAssigned: 3, PointsEarned: 3),
                new("Patches",   TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 2),
                new("Firekeeper",TasksCompleted: 0, TasksAssigned: 2, PointsEarned: 0),
            ]),
    ];

    // --- Metric helpers ---

    /// <summary>Overall task completion rate across all sprints.</summary>
    public static double OverallCompletionRate
    {
        get
        {
            var totalPlanned = SprintHistory.Sum(s => s.PlannedTasks);
            var totalDone = SprintHistory.Sum(s => s.CompletedTasks);
            return totalPlanned > 0 ? Math.Round((double)totalDone / totalPlanned * 100, 1) : 0;
        }
    }

    /// <summary>Average velocity (completed tasks per sprint).</summary>
    public static double AverageVelocity =>
        SprintHistory.Count > 0 ? Math.Round(SprintHistory.Average(s => s.Velocity), 1) : 0;

    /// <summary>Velocity trend: positive means accelerating, negative means slowing.</summary>
    public static double VelocityTrend =>
        SprintHistory.Count >= 2
            ? SprintHistory[^1].Velocity - SprintHistory[^2].Velocity
            : 0;

    /// <summary>Per-member utilization across all sprints.</summary>
    public static IReadOnlyList<(string Name, double Utilization)> TeamUtilization =>
        SprintHistory
            .SelectMany(s => s.Contributions)
            .GroupBy(c => c.MemberName)
            .Select(g => (
                Name: g.Key,
                Utilization: g.Sum(c => c.TasksAssigned) > 0
                    ? Math.Round((double)g.Sum(c => c.TasksCompleted) / g.Sum(c => c.TasksAssigned) * 100, 1)
                    : 0.0
            ))
            .OrderByDescending(x => x.Utilization)
            .ToList();

    public static string GetCharterFor(string memberName) => memberName switch
    {
        "Solaire" => "# Solaire — Lead\n\nThe one who never goes hollow. Responsible for project direction, architecture decisions, and team coordination. Praise the sun!\n\nSolaire brings clarity to system design and keeps the team aligned on shared goals. He reviews all architectural decisions, removes blockers, and ensures the codebase remains clean and maintainable.\n\n## Goals\n- Keep the team aligned on architecture\n- Make sound technical decisions\n- Remove blockers and maintain momentum",
        "Siegmeyer" => "# Siegmeyer — Frontend/TUI Dev\n\nThe adventurous onion knight of the terminal. Builds all TUI screens using the Hex1b framework with a focus on usability and visual polish.\n\nSiegmeyer turns UX designs into working terminal interfaces. He implements responsive layouts, keyboard navigation, and theme support across all screens.\n\n## Goals\n- Create intuitive TUI navigation\n- Build all screen components\n- Ensure responsive layout across terminal sizes",
        "Andre" => "# Andre — Backend Dev\n\nThe steadfast blacksmith who forges the service layer. Implements data providers, service interfaces, and all backend infrastructure.\n\nAndre builds the bridge between raw `.ai-team/` files and the UI layer. His services parse markdown, YAML, and structured data into clean domain models.\n\n## Goals\n- Build reliable file-based data providers\n- Implement all service interfaces\n- Ensure data integrity and error handling",
        "Patches" => "# Patches — Tester\n\nTrusty Patches — surprisingly reliable when it comes to testing. Writes and maintains the automated test suite covering unit, integration, and E2E scenarios.\n\nPatches ensures every feature has proper test coverage. He uses Hex1b headless mode for TUI testing and maintains test fixtures for service validation.\n\n## Goals\n- Write comprehensive automated tests\n- Maintain test fixtures and infrastructure\n- Catch regressions before they ship",
        "Firekeeper" => "# Firekeeper — UX/Design\n\nThe keeper of the flame who tends to the user experience. Designs navigation flows, responsive layouts, color language, and interaction patterns.\n\nFirekeeper creates the UX blueprints that guide Siegmeyer's implementation. She defines keyboard shortcuts, status indicators, and progressive disclosure patterns.\n\n## Goals\n- Design intuitive navigation flows\n- Define responsive layout breakpoints\n- Establish consistent visual language",
        "Scribe" => "# Scribe — Scribe\n\nThe silent chronicler who records the team's decisions and progress. Maintains decision logs, merges inbox entries, and keeps documentation current.\n\nScribe ensures institutional knowledge is captured and accessible. All team decisions flow through the Scribe for archival.\n\n## Goals\n- Maintain the decision log\n- Merge inbox decisions promptly\n- Keep documentation accurate and current",
        _ => $"# {memberName}\n\nNo charter available yet."
    };
}
