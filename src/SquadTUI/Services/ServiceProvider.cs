using System.Diagnostics;

namespace SquadTUI.Services;

public class ServiceProvider
{
    private static ServiceProvider? _instance;
    private static readonly object _lock = new();

    public ISquadDataProvider SquadData { get; }
    public ITeamService Team { get; }
    public IDecisionService Decisions { get; }
    public ISkillService Skills { get; }
    public IOrchestrationLogService OrchestrationLog { get; }

    private ServiceProvider(string teamRootPath)
    {
        var squadDir = SquadPathResolver.Resolve(teamRootPath);
        Team = new TeamService(squadDir);
        Decisions = new DecisionService(squadDir);
        Skills = new SkillService(squadDir);
        OrchestrationLog = new OrchestrationLogService(squadDir);
        SquadData = new SquadDataProvider(Team, OrchestrationLog, Decisions, Skills);
    }

    public static ServiceProvider Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new ServiceProvider(DiscoverProjectRoot());
                }
            }
            return _instance;
        }
    }

    /// <summary>Reset the singleton (used after migration to re-resolve paths).</summary>
    public static void Reset()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }

    private static string DiscoverProjectRoot()
    {
        // First, try git root
        try
        {
            var gitProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --show-toplevel",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            gitProcess.Start();
            var gitRoot = gitProcess.StandardOutput.ReadToEnd().Trim();
            gitProcess.WaitForExit();

            if (gitProcess.ExitCode == 0 && !string.IsNullOrWhiteSpace(gitRoot))
            {
                // Git returns forward slashes even on Windows; normalize to backslashes
                gitRoot = gitRoot.Replace('/', Path.DirectorySeparatorChar);
                return gitRoot;
            }
        }
        catch
        {
            // Git command failed, fall through to directory search
        }

        // Fallback: walk up from current directory looking for .squad/ or .ai-team/
        var current = Directory.GetCurrentDirectory();
        while (current != null)
        {
            if (SquadPathResolver.HasSquadDirectory(current))
                return current;

            var parent = Directory.GetParent(current);
            current = parent?.FullName;
        }

        // Last resort: assume relative path from executable
        var basePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        return Path.GetFullPath(basePath);
    }
}
