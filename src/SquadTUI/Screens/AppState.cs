using SquadTUI.Models;

namespace SquadTUI.Screens;

public enum Screen
{
    Dashboard,
    Roster,
    MemberDetail,
    Decisions,
    Skills,
    ActivityLog,
    Metrics,
    Charter,
    NoSquad
}

public class AppState
{
    public Screen CurrentScreen { get; set; } = Screen.Dashboard;
    public string? SelectedMemberName { get; set; }
    public int RosterSelectedIndex { get; set; }
    public int DecisionSelectedIndex { get; set; }
    public int LogSelectedIndex { get; set; }
    public int SkillSelectedIndex { get; set; }

    // Loaded data from services
    public IReadOnlyList<SquadMember>? Members { get; set; }
    public IReadOnlyList<DecisionEntry>? Decisions { get; set; }
    public IReadOnlyList<Skill>? Skills { get; set; }
    public IReadOnlyList<OrchestrationLogEntry>? LogEntries { get; set; }
    public DashboardData? Dashboard { get; set; }

    // Loading state
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }

    // Theme and UI state
    public int SelectedThemeIndex { get; set; } = 0;
    public int ActiveTab { get; set; } = 0;
    public bool ShowHelp { get; set; } = false;

    // Squad detection
    public bool SquadDetected { get; set; } = true;
    public string? SquadRootPath { get; set; }
    public AppSettings Settings { get; set; } = new();
}
