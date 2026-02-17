namespace SquadTUI.Services;

/// <summary>
/// Central interface for ALL file path resolution in the application.
/// Handles .squad/ vs .ai-team/ transparently — single source of truth for every path.
/// </summary>
public interface IFileLocationService
{
    /// <summary>The project root directory (e.g. the git repo root).</summary>
    string ProjectRoot { get; }

    /// <summary>The resolved squad directory (.squad/ or .ai-team/).</summary>
    string SquadDirectory { get; }

    /// <summary>Path to team.md roster file.</summary>
    string GetRosterPath();

    /// <summary>Path to the agents/ directory.</summary>
    string GetAgentsDirectory();

    /// <summary>Path to a specific agent's charter file: agents/{name}/charter.md</summary>
    string GetCharterPath(string agentName);

    /// <summary>Path to decisions.md file.</summary>
    string GetDecisionsFilePath();

    /// <summary>Path to decisions/inbox/ directory.</summary>
    string GetDecisionsInboxPath();

    /// <summary>Path to the skills/ directory.</summary>
    string GetSkillsDirectory();

    /// <summary>Path to a specific skill's SKILL.md file.</summary>
    string GetSkillFilePath(string slug);

    /// <summary>Path to the orchestration-log/ directory.</summary>
    string GetOrchestrationLogDirectory();

    /// <summary>Path to the log/ directory (legacy log location).</summary>
    string GetLogDirectory();

    /// <summary>Path to the user-level settings directory (~/.config/squadtui/).</summary>
    string GetSettingsDirectory();

    /// <summary>Path to the settings JSON file.</summary>
    string GetSettingsFilePath();

    /// <summary>The active directory name (.squad or .ai-team) without the root path.</summary>
    string ActiveDirectoryName { get; }

    /// <summary>True if the project uses legacy .ai-team/ and needs migration.</summary>
    bool NeedsMigration { get; }

    /// <summary>True if either .squad/ or .ai-team/ exists.</summary>
    bool HasSquadDirectory { get; }
}
