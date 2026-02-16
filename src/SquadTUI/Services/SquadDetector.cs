namespace SquadTUI.Services;

public static class SquadDetector
{
    /// <summary>
    /// Check if an .ai-team directory exists by walking up from CWD or checking git root.
    /// Returns the path to the project root containing .ai-team, or null if not found.
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
                var aiTeamPath = Path.Combine(gitRoot, ".ai-team");
                if (Directory.Exists(aiTeamPath))
                    return gitRoot;
            }
        }
        catch { }

        // Walk up from CWD
        var current = Directory.GetCurrentDirectory();
        while (current != null)
        {
            if (Directory.Exists(Path.Combine(current, ".ai-team")))
                return current;
            current = Directory.GetParent(current)?.FullName;
        }

        return null;
    }

    /// <summary>Check if the required squad structure exists.</summary>
    public static bool HasValidSquad(string rootPath)
    {
        var aiTeamDir = Path.Combine(rootPath, ".ai-team");
        return Directory.Exists(aiTeamDir) &&
               (File.Exists(Path.Combine(aiTeamDir, "team.md")) ||
                Directory.Exists(Path.Combine(aiTeamDir, "agents")));
    }
}
