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

/// <summary>
/// 发布编排器，后台服务，负责执行发布单的各个步骤
/// </summary>
public class ReleaseOrchestrator : BackgroundService
{
    /// <summary>
    /// 发布单队列
    /// </summary>
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

    /// <summary>
    /// 将发布单加入执行队列
    /// </summary>
    public void Enqueue(Guid orderId)
    {
        _queue.Writer.TryWrite(orderId);
    }

    /// <summary>
    /// 请求回滚指定发布单
    /// </summary>
    public void RequestRollback(Guid orderId, string reason, string actor)
    {
        var order = _orders.Find(orderId) ?? throw new InvalidOperationException("Release order not found");
        order.Status = ReleaseStatus.RolledBack;
        order.RollbackReason = reason;
        order.MarkStep(AgentStep.TriggerRollback, AgentStepState.Completed, reason);
        _orders.Save(order);
        _auditLogs.Record("release.rollback", reason, actor, orderId.ToString());
    }

    /// <summary>
    /// 后台执行任务，从队列中读取发布单并处理
    /// </summary>
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

    /// <summary>
    /// 处理单个发布单，执行所有步骤
    /// </summary>
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
                // 原型中模拟步骤标记为运行后立即成功 In this prototype we simulate success immediately after the step is marked running.
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

    /// <summary>
    /// 判断步骤是否需要包文件
    /// </summary>
    private static bool RequiresPackage(AgentStep step)
    {
        return step is AgentStep.DeployServer or AgentStep.DeployClient;
    }

    /// <summary>
    /// 验证发布单关联的包是否存在
    /// </summary>
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

    /// <summary>
    /// 构建默认执行步骤列表
    /// </summary>
    private static List<AgentStepProgress> BuildDefaultSteps()
    {
        return ReleaseOrderService.DefaultStepOrder
            .Select(step => new AgentStepProgress
            {
                Step = step,
                State = AgentStepState.Pending,
                Message = DefaultMessageFor(step)
            })
            .ToList();
    }

    /// <summary>
    /// 获取步骤的默认消息
    /// </summary>
    private static string DefaultMessageFor(AgentStep step)
    {
        return step switch
        {
            AgentStep.ServiceStop => "Stopping services",
            AgentStep.FullBackup => "Taking full backup",
            AgentStep.DeployServer => "Deploying server.zip",
            AgentStep.DeployClient => "Deploying client.zip",
            AgentStep.RunSchemaScript => "Running 结构.sql",
            AgentStep.RunDataScript => "Running data.sql",
            AgentStep.Restart => "Restarting services",
            AgentStep.ReportStatus => "Reporting status",
            _ => $"Pending {step}"
        };
    }
}
