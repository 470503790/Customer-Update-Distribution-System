using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rca7.UpdateClient.Shared.Messaging;
using Rca7.UpdateClient.Shared.State;

namespace Rca7.UpdateAgent.Service.Ipc;

/// <summary>
/// 命名管道通信的占位主机，实际处理程序将在后续接入
/// Placeholder host for named pipe communication. The actual handler will be plumbed in later.
/// </summary>
public class NamedPipeServerHost
{
    private readonly ILogger<NamedPipeServerHost> _logger;
    private readonly AgentStatusSnapshot _emptyStatus = new(null, "idle", 0, "Awaiting commands", DateTimeOffset.UtcNow);

    /// <summary>
    /// 命名管道名称
    /// </summary>
    public string PipeName { get; }

    public NamedPipeServerHost(ILogger<NamedPipeServerHost> logger)
    {
        _logger = logger;
        PipeName = "Rca7.UpdateAgent";
    }

    /// <summary>
    /// 启动命名管道主机
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Named pipe host listening on {PipeName}. IPC skeleton ready for future implementation.", PipeName);
        // 实现将创建 NamedPipeServerStream 并循环处理传入请求
        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止命名管道主机
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Named pipe host shutting down. Pending pipe sessions will be cleaned up in future iterations.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// 构建状态响应
    /// </summary>
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
