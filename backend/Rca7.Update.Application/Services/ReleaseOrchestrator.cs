using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

public class ReleaseOrchestrator : BackgroundService
{
    private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();
    private readonly IReleaseOrderRepository _orders;
    private readonly IPackageRepository _packages;
    private readonly AuditLogService _auditLogs;
    private readonly ILogger<ReleaseOrchestrator> _logger;

    public ReleaseOrchestrator(IReleaseOrderRepository orders, IPackageRepository packages, AuditLogService auditLogs, ILogger<ReleaseOrchestrator> logger)
    {
        _orders = orders;
        _packages = packages;
        _auditLogs = auditLogs;
        _logger = logger;
    }

    public void Enqueue(Guid orderId)
    {
        _queue.Writer.TryWrite(orderId);
    }

    public void RequestRollback(Guid orderId, string reason, string actor)
    {
        var order = _orders.Find(orderId) ?? throw new InvalidOperationException("Release order not found");
        order.Status = ReleaseStatus.RolledBack;
        order.RollbackReason = reason;
        order.MarkStep(AgentStep.TriggerRollback, AgentStepState.Completed, reason);
        _orders.Save(order);
        _auditLogs.Record("release.rollback", reason, actor, orderId.ToString());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var orderId in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            var order = _orders.Find(orderId);
            if (order == null)
            {
                continue;
            }

            try
            {
                await Process(order, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process release order {ReleaseOrderId}", order.Id);
            }
        }
    }

    private Task Process(ReleaseOrder order, CancellationToken cancellationToken)
    {
        order.Status = ReleaseStatus.InProgress;
        _auditLogs.Record("release.start", $"Starting release with strategy {order.Strategy}", "system", order.Id.ToString());
        _orders.Save(order);

        var steps = order.Steps.Any() ? order.Steps : BuildDefaultSteps();

        foreach (var step in steps)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            order.MarkStep(step.Step, AgentStepState.Running, step.Message);
            _orders.Save(order);

            try
            {
                // In this prototype we simulate success immediately after the step is marked running.
                if (RequiresPackage(step.Step) && !ValidatePackages(order))
                {
                    throw new InvalidOperationException("Required packages for deployment are missing.");
                }

                order.MarkStep(step.Step, AgentStepState.Completed, step.Message ?? "Completed");
            }
            catch (Exception ex)
            {
                order.MarkStep(step.Step, AgentStepState.Failed, ex.Message);
                order.Status = ReleaseStatus.Failed;
                _orders.Save(order);
                _auditLogs.Record("release.error", ex.Message, "system", order.Id.ToString());
                return Task.CompletedTask;
            }

            _orders.Save(order);
        }

        if (order.Status != ReleaseStatus.Failed && order.Status != ReleaseStatus.RolledBack)
        {
            order.Status = ReleaseStatus.Succeeded;
            _auditLogs.Record("release.complete", "Deployment completed", "system", order.Id.ToString());
            _orders.Save(order);
        }

        return Task.CompletedTask;
    }

    private static bool RequiresPackage(AgentStep step)
    {
        return step is AgentStep.DeployServer or AgentStep.DeployClient;
    }

    private bool ValidatePackages(ReleaseOrder order)
    {
        if (order.ServerPackageId.HasValue && _packages.Find(order.ServerPackageId.Value) == null)
        {
            return false;
        }

        if (order.ClientPackageId.HasValue && _packages.Find(order.ClientPackageId.Value) == null)
        {
            return false;
        }

        return true;
    }

    private static List<AgentStepProgress> BuildDefaultSteps()
    {
        return new List<AgentStepProgress>
        {
            new() { Step = AgentStep.ServiceStop, State = AgentStepState.Pending, Message = "Stopping services" },
            new() { Step = AgentStep.FullBackup, State = AgentStepState.Pending, Message = "Taking full backup" },
            new() { Step = AgentStep.DeployServer, State = AgentStepState.Pending, Message = "Deploying server.zip" },
            new() { Step = AgentStep.DeployClient, State = AgentStepState.Pending, Message = "Deploying client.zip" },
            new() { Step = AgentStep.RunSchemaScript, State = AgentStepState.Pending, Message = "Running 结构.sql" },
            new() { Step = AgentStep.RunDataScript, State = AgentStepState.Pending, Message = "Running data.sql" },
            new() { Step = AgentStep.Restart, State = AgentStepState.Pending, Message = "Restarting services" },
            new() { Step = AgentStep.ReportStatus, State = AgentStepState.Pending, Message = "Reporting status" },
        };
    }
}
