namespace SquadTUI.Services;

/// <summary>
/// Concrete implementation of IFileLocationService.
/// Resolves .squad/ vs .ai-team/ transparently via SquadPathResolver.
/// </summary>
public class FileLocationService : IFileLocationService
{
    public FileLocationService(string projectRoot)
    {
        ProjectRoot = projectRoot;
        SquadDirectory = SquadPathResolver.Resolve(projectRoot);
    }

    private FileLocationService(string projectRoot, string squadDirectory)
    {
        ProjectRoot = projectRoot;
        SquadDirectory = squadDirectory;
    }

    /// <summary>
    /// Creates a FileLocationService from an already-resolved squad directory path.
    /// Used for backward compatibility when callers pass the squad dir directly.
    /// </summary>
    internal static FileLocationService FromSquadDirectory(string squadDirPath)
    {
        return new FileLocationService(
            Path.GetDirectoryName(squadDirPath) ?? squadDirPath,
            squadDirPath);
    }

    public string ProjectRoot { get; }
    public string SquadDirectory { get; }

    public string GetRosterPath() =>
        Path.Combine(SquadDirectory, "team.md");

    public string GetAgentsDirectory() =>
        Path.Combine(SquadDirectory, "agents");

    public string GetCharterPath(string agentName) =>
        Path.Combine(SquadDirectory, "agents", agentName.ToLowerInvariant(), "charter.md");

    public string GetHistoryPath(string agentName) =>
        Path.Combine(SquadDirectory, "agents", agentName.ToLowerInvariant(), "history.md");

    public string GetAgentDirectory(string agentName) =>
        Path.Combine(SquadDirectory, "agents", agentName.ToLowerInvariant());

    public string GetDecisionsFilePath() =>
        Path.Combine(SquadDirectory, "decisions.md");

    public string GetDecisionsInboxPath() =>
        Path.Combine(SquadDirectory, "decisions", "inbox");

    public string GetSkillsDirectory() =>
        Path.Combine(SquadDirectory, "skills");

    public string GetSkillFilePath(string slug) =>
        Path.Combine(SquadDirectory, "skills", slug, "SKILL.md");

    public string GetOrchestrationLogDirectory() =>
        Path.Combine(SquadDirectory, "orchestration-log");

    public string GetLogDirectory() =>
        Path.Combine(SquadDirectory, "log");

    public string GetSettingsDirectory() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "squadtui");

    public string GetSettingsFilePath() =>
        Path.Combine(GetSettingsDirectory(), "settings.json");

    public string ActiveDirectoryName =>
        SquadPathResolver.GetActiveDirectoryName(ProjectRoot);

    public bool NeedsMigration =>
        SquadPathResolver.NeedsMigration(ProjectRoot);

    public bool HasSquadDirectory =>
        SquadPathResolver.HasSquadDirectory(ProjectRoot);
}
