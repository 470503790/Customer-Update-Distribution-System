using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rca7.UpdateClient.Shared.State;

namespace Rca7.UpdateAgent.Service.Services;

/// <summary>
/// Persists checkpoints for crash recovery and manual inspection.
/// </summary>
public class AgentStateStore
{
    private readonly ILogger<AgentStateStore> _logger;
    private readonly string _stateFilePath;

    public AgentStateStore(ILogger<AgentStateStore> logger)
    {
        _logger = logger;
        _stateFilePath = Path.Combine(AppContext.BaseDirectory, AgentState.DefaultStateFileName);
    }

    public AgentState Current { get; private set; } = new();

    public async Task<AgentState> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_stateFilePath))
        {
            _logger.LogInformation("State file not found at {Path}. Starting fresh.", _stateFilePath);
            Current = new AgentState();
            return Current;
        }

        await using var stream = File.OpenRead(_stateFilePath);
        var state = await JsonSerializer.DeserializeAsync<AgentState>(stream, cancellationToken: cancellationToken);
        Current = state ?? new AgentState();
        _logger.LogInformation("Loaded agent state. Active package: {PackageId}", Current.ActivePackageId ?? "<none>");
        return Current;
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_stateFilePath) ?? ".");
        Current.LastCheckpointUtc = DateTimeOffset.UtcNow;
        await using var stream = File.Create(_stateFilePath);
        await JsonSerializer.SerializeAsync(stream, Current, cancellationToken: cancellationToken);
        _logger.LogInformation("Persisted agent state to {Path}.", _stateFilePath);
    }
}
