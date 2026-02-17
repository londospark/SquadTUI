using System.Diagnostics;

namespace SquadTUI.Services;

/// <summary>
/// Handles migration from .ai-team/ to .squad/ directory.
/// Uses git mv if inside a git repo, otherwise uses filesystem move.
/// </summary>
public static class MigrationService
{
    public record MigrationResult(bool Success, string Message);

    /// <summary>
    /// Migrates .ai-team/ to .squad/ at the given project root.
    /// Returns a result indicating success or failure with a message.
    /// </summary>
    public static MigrationResult Migrate(string projectRoot)
    {
        var legacyPath = Path.Combine(projectRoot, SquadPathResolver.LegacyDirectoryName);
        var newPath = Path.Combine(projectRoot, SquadPathResolver.NewDirectoryName);

        if (!Directory.Exists(legacyPath))
            return new MigrationResult(false, "No .ai-team/ directory found to migrate.");

        if (Directory.Exists(newPath))
            return new MigrationResult(false, ".squad/ directory already exists. Migration not needed.");

        // Try git mv first (preserves history)
        if (IsGitRepo(projectRoot))
        {
            var gitResult = TryGitMv(projectRoot, legacyPath, newPath);
            if (gitResult.Success)
                return gitResult;
        }

        // Fallback to filesystem move
        try
        {
            Directory.Move(legacyPath, newPath);
            return new MigrationResult(true, "Migrated .ai-team/ → .squad/ (filesystem move).");
        }
        catch (Exception ex)
        {
            return new MigrationResult(false, $"Migration failed: {ex.Message}");
        }
    }

    private static bool IsGitRepo(string projectRoot)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --is-inside-work-tree",
                    WorkingDirectory = projectRoot,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return process.ExitCode == 0 && output == "true";
        }
        catch
        {
            return false;
        }
    }

    private static MigrationResult TryGitMv(string projectRoot, string legacyPath, string newPath)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"mv \"{SquadPathResolver.LegacyDirectoryName}\" \"{SquadPathResolver.NewDirectoryName}\"",
                    WorkingDirectory = projectRoot,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var stderr = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode == 0)
                return new MigrationResult(true, "Migrated .ai-team/ → .squad/ (git mv — history preserved).");

            return new MigrationResult(false, $"git mv failed: {stderr}");
        }
        catch (Exception ex)
        {
            return new MigrationResult(false, $"git mv failed: {ex.Message}");
        }
    }
}
