namespace SquadTUI.Services;

public static class SquadDetector
{
    /// <summary>
    /// Check if a squad directory (.squad/ or .ai-team/) exists by walking up from CWD or checking git root.
    /// Returns the path to the project root, or null if not found.
    /// </summary>
    public static string? FindSquadRoot()
    {
        // Check git root first
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --show-toplevel",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var gitRoot = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(gitRoot))
            {
                gitRoot = gitRoot.Replace('/', Path.DirectorySeparatorChar);
                if (SquadPathResolver.HasSquadDirectory(gitRoot))
                    return gitRoot;
            }
        }
        catch { }

        // Walk up from CWD — check .squad/ first, then .ai-team/
        var current = Directory.GetCurrentDirectory();
        while (current != null)
        {
            if (SquadPathResolver.HasSquadDirectory(current))
                return current;
            current = Directory.GetParent(current)?.FullName;
        }

        return null;
    }

    /// <summary>Check if the required squad structure exists (supports both .squad/ and .ai-team/).</summary>
    public static bool HasValidSquad(string rootPath)
    {
        var squadDir = SquadPathResolver.Resolve(rootPath);
        return Directory.Exists(squadDir) &&
               (File.Exists(Path.Combine(squadDir, "team.md")) ||
                Directory.Exists(Path.Combine(squadDir, "agents")));
    }
}
