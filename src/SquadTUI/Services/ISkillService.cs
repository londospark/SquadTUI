using SquadTUI.Models;

namespace SquadTUI.Services;

public interface ISkillService
{
    Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken ct = default);
    Task<Skill?> GetSkillAsync(string slug, CancellationToken ct = default);
}
