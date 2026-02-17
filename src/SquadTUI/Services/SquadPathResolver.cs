namespace SquadTUI.Services;

/// <summary>
/// Resolves which squad directory name (.squad/ or .ai-team/) is active for a given project root.
/// Prefers .squad/ (new name), falls back to .ai-team/ (legacy).
/// </summary>
public static class SquadPathResolver
{
    public const string NewDirectoryName = ".squad";
    public const string LegacyDirectoryName = ".ai-team";

    /// <summary>
    /// Returns the resolved squad directory path for a given project root.
    /// Checks for .squad/ first, then .ai-team/.
    /// </summary>
    public static string Resolve(string projectRoot)
    {
        var squadPath = Path.Combine(projectRoot, NewDirectoryName);
        if (Directory.Exists(squadPath))
            return squadPath;

        var legacyPath = Path.Combine(projectRoot, LegacyDirectoryName);
        if (Directory.Exists(legacyPath))
            return legacyPath;

        // Default to new name if neither exists
        return squadPath;
    }

    /// <summary>True if the project uses the legacy .ai-team/ directory and .squad/ does not exist.</summary>
    public static bool NeedsMigration(string projectRoot)
    {
        var hasSquad = Directory.Exists(Path.Combine(projectRoot, NewDirectoryName));
        var hasLegacy = Directory.Exists(Path.Combine(projectRoot, LegacyDirectoryName));
        return !hasSquad && hasLegacy;
    }

    /// <summary>True if either .squad/ or .ai-team/ exists at the given root.</summary>
    public static bool HasSquadDirectory(string projectRoot)
    {
        return Directory.Exists(Path.Combine(projectRoot, NewDirectoryName)) ||
               Directory.Exists(Path.Combine(projectRoot, LegacyDirectoryName));
    }

    /// <summary>Returns the active directory name (.squad or .ai-team) without the root path.</summary>
    public static string GetActiveDirectoryName(string projectRoot)
    {
        if (Directory.Exists(Path.Combine(projectRoot, NewDirectoryName)))
            return NewDirectoryName;
        if (Directory.Exists(Path.Combine(projectRoot, LegacyDirectoryName)))
            return LegacyDirectoryName;
        return NewDirectoryName;
    }
}
