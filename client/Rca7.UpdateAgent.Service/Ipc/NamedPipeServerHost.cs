using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rca7.UpdateClient.Shared.Messaging;
using Rca7.UpdateClient.Shared.State;

namespace Rca7.UpdateAgent.Service.Ipc;

/// <summary>
/// Placeholder host for named pipe communication. The actual handler will be plumbed in later.
/// </summary>
public class NamedPipeServerHost
{
    private readonly ILogger<NamedPipeServerHost> _logger;
    private readonly AgentStatusSnapshot _emptyStatus = new(null, "idle", 0, "Awaiting commands", DateTimeOffset.UtcNow);

    public string PipeName { get; }

    public NamedPipeServerHost(ILogger<NamedPipeServerHost> logger)
    {
        _logger = logger;
        PipeName = "Rca7.UpdateAgent";
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Named pipe host listening on {PipeName}. IPC skeleton ready for future implementation.", PipeName);
        // The implementation will create a NamedPipeServerStream and loop on incoming requests.
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Named pipe host shutting down. Pending pipe sessions will be cleaned up in future iterations.");
        return Task.CompletedTask;
    }

    public NamedPipeResponse BuildStatusResponse(Guid correlationId)
    {
        return new NamedPipeResponse(
            PipeCommand.CheckStatus,
            PipeResponseStatus.Completed,
            correlationId,
            JsonSerializer.Serialize(_emptyStatus)
        );
    }

    public NamedPipeResponse BuildConfigResponse(Guid correlationId, ConfigurationSnapshot snapshot)
    {
        return new NamedPipeResponse(
            PipeCommand.RequestConfiguration,
            PipeResponseStatus.Completed,
            correlationId,
            JsonSerializer.Serialize(snapshot)
        );
    }
}
