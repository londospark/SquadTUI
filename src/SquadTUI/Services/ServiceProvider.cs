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

    private ServiceProvider(string aiTeamRootPath)
    {
        Team = new TeamService(aiTeamRootPath);
        Decisions = new DecisionService(aiTeamRootPath);
        Skills = new SkillService(aiTeamRootPath);
        OrchestrationLog = new OrchestrationLogService(aiTeamRootPath);
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
                    _instance ??= new ServiceProvider(DiscoverAiTeamPath());
                }
            }
            return _instance;
        }
    }

    private static string DiscoverAiTeamPath()
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

        // Fallback: walk up from current directory looking for .ai-team
        var current = Directory.GetCurrentDirectory();
        while (current != null)
        {
            var aiTeamPath = Path.Combine(current, ".ai-team");
            if (Directory.Exists(aiTeamPath))
                return current;

            var parent = Directory.GetParent(current);
            current = parent?.FullName;
        }

        // Last resort: assume relative path from executable
        var basePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        return Path.GetFullPath(basePath);
    }
}
