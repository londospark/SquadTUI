using LanguageExt;
using static LanguageExt.Prelude;
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
    NoSquad,
    Settings,
    Help
}

public class AppState
{
    public Screen CurrentScreen { get; set; } = Screen.Dashboard;
    public Screen? PreviousScreen { get; set; }
    public string? SelectedMemberName { get; set; }
    public int RosterSelectedIndex { get; set; }
    public int DecisionSelectedIndex { get; set; }
    public int LogSelectedIndex { get; set; }
    public int SkillSelectedIndex { get; set; }
    public int SettingsSelectedIndex { get; set; }
    public int DashboardFocusedPanel { get; set; } = 0;

    // Loaded data from services — monadic Either for error visibility
    public Either<AppError, IReadOnlyList<SquadMember>> Members { get; set; } = Right<AppError, IReadOnlyList<SquadMember>>([]);
    public Either<AppError, IReadOnlyList<DecisionEntry>> Decisions { get; set; } = Right<AppError, IReadOnlyList<DecisionEntry>>([]);
    public Either<AppError, IReadOnlyList<Skill>> Skills { get; set; } = Right<AppError, IReadOnlyList<Skill>>([]);
    public Either<AppError, IReadOnlyList<OrchestrationLogEntry>> LogEntries { get; set; } = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>([]);
    public Either<AppError, IReadOnlyList<SquadTask>> Tasks { get; set; } = Right<AppError, IReadOnlyList<SquadTask>>([]);
    public Either<AppError, IReadOnlyList<SprintMetrics>> SprintHistory { get; set; } = Right<AppError, IReadOnlyList<SprintMetrics>>([]);
    public Option<string> CharterContent { get; set; } = None;
    public DashboardData? Dashboard { get; set; }

    // Loading state
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }

    // Theme and UI state
    public int SelectedThemeIndex { get; set; } = 0;
    public int ActiveTab { get; set; } = 0;
    public bool ShowHelp { get; set; } = false;
    public bool ShowBurndown { get; set; } = false;

    // Theme modal overlay
    public bool ShowSettingsOverlay { get; set; } = false;
    public int PreviewThemeIndex { get; set; } = -1; // -1 means "use SelectedThemeIndex"
    public int OriginalThemeIndex { get; set; } = 0;

    // Live dashboard
    public bool IsLiveEnabled { get; set; } = true;
    public DateTime LastRefreshTime { get; set; } = DateTime.Now;
    public bool HasPendingRefresh { get; set; }

    // Add/Remove member flow
    public bool ConfirmingRemove { get; set; }
    public string? AddMemberMessage { get; set; }

    // Squad detection
    public bool SquadDetected { get; set; } = true;
    public string? SquadRootPath { get; set; }
    public bool NeedsMigration { get; set; }
    public string? MigrationMessage { get; set; }
    public AppSettings Settings { get; set; } = new();
}
