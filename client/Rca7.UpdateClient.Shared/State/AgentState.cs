using System.Text.Json.Serialization;

namespace Rca7.UpdateClient.Shared.State;

/// <summary>
/// Represents durable agent state that can be resumed after a crash or restart.
/// </summary>
public class AgentState
{
    public const string DefaultStateFileName = "agent-state.json";

    /// <summary>
    /// Identifier of the package currently being processed.
    /// </summary>
    public string? ActivePackageId { get; set; }

    /// <summary>
    /// Name of the SQL script last executed successfully.
    /// </summary>
    public string? LastAppliedSql { get; set; }

    /// <summary>
    /// When the state was last updated.
    /// </summary>
    public DateTimeOffset LastCheckpointUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// True when an update is in progress and can be resumed.
    /// </summary>
    public bool IsInProgress { get; set; }
}

/// <summary>
/// Snapshot that can be sent to UI clients showing current agent progress.
/// </summary>
public record AgentStatusSnapshot(
    string? ActivePackageId,
    string? Stage,
    double Progress,
    string? LastMessage,
    DateTimeOffset LastUpdatedUtc
);

/// <summary>
/// Snapshot of configuration shared for UI and diagnostics purposes.
/// </summary>
public record ConfigurationSnapshot(string DefaultUpdateDirectory, string BackupDirectory, IReadOnlyList<string> SqlExecutionOrder)
{
    [JsonIgnore]
    public bool HasCustomSqlOrder => SqlExecutionOrder.Count > 0;
}
