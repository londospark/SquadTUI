using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;

namespace SquadTUI.Services;

public class SkillService : ISkillService
{
    private readonly IFileLocationService _fileLocations;

    public SkillService(IFileLocationService fileLocations)
    {
        _fileLocations = fileLocations;
    }

    /// <summary>Legacy constructor for backward compatibility (tests).</summary>
    public SkillService(string squadDirPath)
        : this(FileLocationService.FromSquadDirectory(squadDirPath)) { }

    public async Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken ct = default)
    {
        var skills = new List<Skill>();
        var skillsPath = _fileLocations.GetSkillsDirectory();
        if (!Directory.Exists(skillsPath))
            return skills;

        foreach (var dir in Directory.EnumerateDirectories(skillsPath))
        {
            var skillFile = Path.Combine(dir, "SKILL.md");
            if (!File.Exists(skillFile))
                continue;

            var slug = Path.GetFileName(dir);
            var content = await File.ReadAllTextAsync(skillFile, ct);
            var skill = ParseSkillFile(content, slug);
            if (skill is not null)
                skills.Add(skill);
        }

        return skills;
    }

    public async Task<Skill?> GetSkillAsync(string slug, CancellationToken ct = default)
    {
        var skillFile = _fileLocations.GetSkillFilePath(slug);
        if (!File.Exists(skillFile))
            return null;

        var content = await File.ReadAllTextAsync(skillFile, ct);
        return ParseSkillFile(content, slug);
    }

    private static Skill? ParseSkillFile(string content, string slug)
    {
        var lines = content.Split('\n');
        string? name = null;
        string? description = null;
        Option<string> source = None;
        string confidence = "medium";

        var inFrontmatter = false;
        var bodyLines = new List<string>();

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();

            if (i == 0 && trimmed == "---")
            {
                inFrontmatter = true;
                continue;
            }

            if (inFrontmatter)
            {
                if (trimmed == "---")
                {
                    inFrontmatter = false;
                    continue;
                }

                var colonIdx = trimmed.IndexOf(':');
                if (colonIdx > 0)
                {
                    var key = trimmed[..colonIdx].Trim().ToLowerInvariant();
                    var value = trimmed[(colonIdx + 1)..].Trim().Trim('"');
                    switch (key)
                    {
                        case "name": name = value; break;
                        case "description": description = value; break;
                        case "source": source = Some(value); break;
                        case "confidence": confidence = value; break;
                    }
                }
                continue;
            }

            // Extract name from first heading if not in frontmatter
            if (name is null && trimmed.StartsWith("# "))
            {
                name = trimmed[2..].Trim();
                continue;
            }

            bodyLines.Add(lines[i]);
        }

        name ??= slug;
        description ??= "";

        var bodyContent = string.Join('\n', bodyLines).Trim();

        return new Skill(name, description, source, confidence,
            string.IsNullOrEmpty(bodyContent) ? None : Some(bodyContent),
            Some(slug));
    }
}
