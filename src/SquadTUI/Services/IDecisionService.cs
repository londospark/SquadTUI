using SquadTUI.Models;

namespace SquadTUI.Services;

public interface IDecisionService
{
    Task<IReadOnlyList<DecisionEntry>> GetDecisionsAsync(CancellationToken ct = default);
}
