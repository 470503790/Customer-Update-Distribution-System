using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rca7.UpdateAgent.Service.Ipc;
using Rca7.UpdateClient.Shared.Config;

namespace Rca7.UpdateAgent.Service.Services;

/// <summary>
/// 后台进程，将来会编排更新状态机
/// Background process that will eventually orchestrate the update state machine.
/// </summary>
public class AgentWorker : BackgroundService
{
    private readonly ILogger<AgentWorker> _logger;
    private readonly IOptionsMonitor<UpdateConfiguration> _configuration;
    private readonly AgentStateStore _stateStore;
    private readonly NamedPipeServerHost _pipeHost;

    public AgentWorker(
        ILogger<AgentWorker> logger,
        IOptionsMonitor<UpdateConfiguration> configuration,
        AgentStateStore stateStore,
        NamedPipeServerHost pipeHost)
    {
        _logger = logger;
        _configuration = configuration;
        _stateStore = stateStore;
        _pipeHost = pipeHost;
    }

    /// <summary>
    /// 执行后台任务，加载状态并启动命名管道主机
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Update agent starting with update directory: {Path}", _configuration.CurrentValue.DefaultUpdateDirectory);

        await _stateStore.LoadAsync(stoppingToken);
        await _pipeHost.StartAsync(stoppingToken);

        // Placeholder loop for the future state machine implementation.
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            _logger.LogDebug("Heartbeat - active package: {PackageId}", _stateStore.Current.ActivePackageId ?? "<none>");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update agent stopping. Flushing checkpoint to disk.");
        await _pipeHost.StopAsync(cancellationToken);
        await _stateStore.SaveAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
