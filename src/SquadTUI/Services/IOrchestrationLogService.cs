using SquadTUI.Models;

namespace SquadTUI.Services;

public interface IOrchestrationLogService
{
    Task<IReadOnlyList<OrchestrationLogEntry>> GetEntriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrchestrationLogEntry>> GetEntriesByDateAsync(string date, CancellationToken ct = default);
}
