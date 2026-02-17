using SquadTUI.Models;

namespace SquadTUI.Screens;

public static class SampleData
{
    public static readonly IReadOnlyList<SquadMember> Members =
    [
        new("Sonic", "Lead", MemberStatus.Active, "Architecture planning"),
        new("Tails", "Frontend/TUI Dev", MemberStatus.Active, "Building TUI screens"),
        new("Knuckles", "Backend Dev", MemberStatus.Active, "Service implementations"),
        new("Amy", "Tester", MemberStatus.Active, "Writing test suite"),
        new("Shadow", "UX/Design", MemberStatus.Active, "Navigation design"),
        new("Eggman", "Eggman", MemberStatus.Idle),
    ];

    public static readonly IReadOnlyList<DecisionEntry> Decisions =
    [
        new("Project Architecture", "2026-02-16", "Sonic",
            "Use Hex1b for TUI framework. .NET 10 target. Clean separation of Models, Services, and Screens layers."),
        new("UX Design", "2026-02-16", "Shadow",
            "Sidebar navigation with number keys. HStack layout with list on left and detail preview on right for browse screens."),
        new("Data Layer", "2026-02-16", "Knuckles",
            "File-based data providers reading from .ai-team directory. Interfaces first, implementations second."),
        new("Testing Strategy", "2026-02-17", "Amy",
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
            ["Sonic", "Tails", "Knuckles", "Amy", "Shadow"],
            "Planned sprint 1 tasks. Assigned TUI to Tails, services to Knuckles.",
            ["Use Hex1b framework", "File-based data layer"],
            ["Sprint backlog created", "Roles assigned"]),
        new(DateTimeOffset.Parse("2026-02-16T14:00:00Z"), "2026-02-16", "Architecture Review",
            ["Sonic", "Knuckles"],
            "Reviewed project architecture. Decided on clean layered approach.",
            ["Models/Services/Screens separation"],
            ["Architecture document created"]),
        new(DateTimeOffset.Parse("2026-02-16T09:00:00Z"), "2026-02-16", "Project Kickoff",
            ["Sonic", "Tails", "Knuckles", "Amy", "Shadow", "Eggman"],
            "Initial project setup. Created repository structure and assigned roles.",
            ["Project name: SquadTUI", ".NET 10 target"],
            ["Repository initialized", "Team roster created"]),
    ];

    public static readonly IReadOnlyList<SquadTask> Tasks =
    [
        new("task-1", "Build TUI screens", "Create all screen components", SquadTaskStatus.InProgress, "Tails"),
        new("task-2", "Implement services", "Build data provider services", SquadTaskStatus.InProgress, "Knuckles"),
        new("task-3", "Write test suite", "Automated tests for all components", SquadTaskStatus.Pending, "Amy"),
        new("task-4", "UX review", "Review navigation flow", SquadTaskStatus.Done, "Shadow"),
        new("task-5", "Architecture doc", "Document system architecture", SquadTaskStatus.Done, "Sonic"),
    ];

    // --- Sprint velocity history (3 sprints, Sonic themed) ---

    public static readonly IReadOnlyList<SprintMetrics> SprintHistory =
    [
        new(1, "Green Hill Sprint",
            DateTimeOffset.Parse("2026-02-03T00:00:00Z"),
            DateTimeOffset.Parse("2026-02-09T23:59:59Z"),
            PlannedTasks: 8, CompletedTasks: 5, CarriedOver: 3,
            Contributions:
            [
                new("Sonic",   TasksCompleted: 2, TasksAssigned: 2, PointsEarned: 5),
                new("Tails", TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 3),
                new("Knuckles",     TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 3),
                new("Amy",   TasksCompleted: 1, TasksAssigned: 1, PointsEarned: 2),
                new("Shadow",TasksCompleted: 0, TasksAssigned: 1, PointsEarned: 0),
            ]),
        new(2, "Chemical Plant Sprint",
            DateTimeOffset.Parse("2026-02-10T00:00:00Z"),
            DateTimeOffset.Parse("2026-02-16T23:59:59Z"),
            PlannedTasks: 10, CompletedTasks: 7, CarriedOver: 3,
            Contributions:
            [
                new("Sonic",   TasksCompleted: 2, TasksAssigned: 2, PointsEarned: 5),
                new("Tails", TasksCompleted: 2, TasksAssigned: 3, PointsEarned: 5),
                new("Knuckles",     TasksCompleted: 2, TasksAssigned: 3, PointsEarned: 5),
                new("Amy",   TasksCompleted: 1, TasksAssigned: 1, PointsEarned: 2),
                new("Shadow",TasksCompleted: 0, TasksAssigned: 1, PointsEarned: 1),
            ]),
        new(3, "Casino Night Sprint",
            DateTimeOffset.Parse("2026-02-17T00:00:00Z"),
            DateTimeOffset.Parse("2026-02-23T23:59:59Z"),
            PlannedTasks: 12, CompletedTasks: 4, CarriedOver: 8,
            Contributions:
            [
                new("Sonic",   TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 3),
                new("Tails", TasksCompleted: 1, TasksAssigned: 3, PointsEarned: 3),
                new("Knuckles",     TasksCompleted: 1, TasksAssigned: 3, PointsEarned: 3),
                new("Amy",   TasksCompleted: 1, TasksAssigned: 2, PointsEarned: 2),
                new("Shadow",TasksCompleted: 0, TasksAssigned: 2, PointsEarned: 0),
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
        "Sonic" => "# Sonic — Lead\n\nGotta go fast! Responsible for project direction, architecture decisions, and team coordination. Speed is everything.\n\nSonic brings momentum to system design and keeps the team aligned on shared goals. He reviews all architectural decisions, removes blockers, and ensures the codebase stays clean.\n\n## Goals\n- Keep the team aligned on architecture\n- Make sound technical decisions\n- Remove blockers and maintain momentum",
        "Tails" => "# Tails — Frontend/TUI Dev\n\nThe two-tailed fox genius of the terminal. Builds all TUI screens using the Hex1b framework with a focus on usability and visual polish.\n\nTails turns UX designs into working terminal interfaces. He implements responsive layouts, keyboard navigation, and theme support across all screens.\n\n## Goals\n- Create intuitive TUI navigation\n- Build all screen components\n- Ensure responsive layout across terminal sizes",
        "Knuckles" => "# Knuckles — Backend Dev\n\nThe guardian who protects the service layer. Implements data providers, service interfaces, and all backend infrastructure.\n\nAndre builds the bridge between raw `.ai-team/` files and the UI layer. His services parse markdown, YAML, and structured data into clean domain models.\n\n## Goals\n- Build reliable file-based data providers\n- Implement all service interfaces\n- Ensure data integrity and error handling",
        "Amy" => "# Amy — Tester\n\nTrusty Patches — surprisingly reliable when it comes to testing. Writes and maintains the automated test suite covering unit, integration, and E2E scenarios.\n\nPatches ensures every feature has proper test coverage. He uses Hex1b headless mode for TUI testing and maintains test fixtures for service validation.\n\n## Goals\n- Write comprehensive automated tests\n- Maintain test fixtures and infrastructure\n- Catch regressions before they ship",
        "Shadow" => "# Shadow — UX/Design\n\nThe ultimate lifeform who crafts the user experience. Designs navigation flows, responsive layouts, color language, and interaction patterns.\n\nShadow creates the UX blueprints that guide Tails' implementation. She defines keyboard shortcuts, status indicators, and progressive disclosure patterns.\n\n## Goals\n- Design intuitive navigation flows\n- Define responsive layout breakpoints\n- Establish consistent visual language",
        "Eggman" => "# Eggman — Scribe\n\nThe genius chronicler who records the team's decisions and progress. Maintains decision logs, merges inbox entries, and keeps documentation current.\n\nScribe ensures institutional knowledge is captured and accessible. All team decisions flow through the Scribe for archival.\n\n## Goals\n- Maintain the decision log\n- Merge inbox decisions promptly\n- Keep documentation accurate and current",
        _ => $"# {memberName}\n\nNo charter available yet."
    };
}
