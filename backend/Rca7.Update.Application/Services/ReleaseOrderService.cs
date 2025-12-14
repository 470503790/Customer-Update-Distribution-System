using System;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

/// <summary>
/// 发布单服务，管理发布单的创建、查询和回滚
/// </summary>
public class ReleaseOrderService
{
    /// <summary>
    /// 最小版本号限制
    /// </summary>
    private static readonly Version MinimumVersion = new(1, 0, 0);
    
    /// <summary>
    /// 最大版本号限制
    /// </summary>
    private static readonly Version MaximumVersion = new(9, 9, 9);
    
    /// <summary>
    /// 默认执行步骤顺序：停服 → 备份 → 部署 → 数据脚本 → 重启 → 上报
    /// </summary>
    public static readonly IReadOnlyList<AgentStep> DefaultStepOrder = new List<AgentStep>
    {
        AgentStep.ServiceStop,
        AgentStep.FullBackup,
        AgentStep.DeployServer,
        AgentStep.DeployClient,
        AgentStep.RunSchemaScript,
        AgentStep.RunDataScript,
        AgentStep.Restart,
        AgentStep.ReportStatus
    };

    private readonly IReleaseOrderRepository _orders;
    private readonly ReleaseOrchestrator _orchestrator;
    private readonly AuditLogService _auditLogs;

    public ReleaseOrderService(IReleaseOrderRepository orders, ReleaseOrchestrator orchestrator, AuditLogService auditLogs)
    {
        _orders = orders;
        _orchestrator = orchestrator;
        _auditLogs = auditLogs;
    }

    /// <summary>
    /// 创建发布单并自动加入编排器队列
    /// </summary>
    public ReleaseOrder Create(ReleaseOrderRequest request)
    {
        var steps = BuildSteps(request);
        var order = new ReleaseOrder
        {
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            NodeId = request.NodeId,
            Environment = request.Environment,
            Strategy = request.Strategy,
            Deadline = request.Deadline,
            ServerPackageId = request.ServerPackageId,
            ClientPackageId = request.ClientPackageId,
            ConfigurationPackageId = request.ConfigurationPackageId,
            Steps = steps,
            Status = ReleaseStatus.Scheduled
        };

        order.Validate(MinimumVersion, MaximumVersion);

        _orders.Save(order);
        _auditLogs.Record("release.created", $"Release order {order.Id} created", request.RequestedBy, order.Id.ToString());
        _orchestrator.Enqueue(order.Id);
        return order;
    }

    /// <summary>
    /// 获取所有发布单
    /// </summary>
    public IEnumerable<ReleaseOrder> GetAll() => _orders.GetAll();

    /// <summary>
    /// 查找指定发布单
    /// </summary>
    public ReleaseOrder? Find(Guid id) => _orders.Find(id);

    /// <summary>
    /// 触发发布单回滚
    /// </summary>
    public ReleaseOrder TriggerRollback(Guid id, string reason, string actor)
    {
        var order = _orders.Find(id) ?? throw new InvalidOperationException("Release order not found");
        order.Status = ReleaseStatus.RolledBack;
        order.RollbackReason = reason;
        order.MarkStep(AgentStep.TriggerRollback, AgentStepState.Pending, reason);
        _orders.Save(order);
        _auditLogs.Record("release.rollback", reason, actor, order.Id.ToString());
        _orchestrator.RequestRollback(id, reason, actor);
        return order;
    }

    /// <summary>
    /// 构建步骤列表
    /// </summary>
    private static List<AgentStepProgress> BuildSteps(ReleaseOrderRequest request)
    {
        var normalizedSteps = NormalizeSteps(request.Steps);

        return normalizedSteps.Select(step => new AgentStepProgress
        {
            Step = step,
            State = AgentStepState.Pending,
            Message = $"Pending {step}"
        }).ToList();
    }

    /// <summary>
    /// 规范化步骤列表，去重并按默认顺序排列
    /// </summary>
    internal static List<AgentStep> NormalizeSteps(IEnumerable<AgentStep>? requestedSteps)
    {
        var requested = requestedSteps?.ToList() ?? new List<AgentStep>();
        var uniqueRequested = new List<AgentStep>();
        foreach (var step in requested)
        {
            if (!uniqueRequested.Contains(step))
            {
                uniqueRequested.Add(step);
            }
        }

        var ordered = new List<AgentStep>();

        foreach (var required in DefaultStepOrder)
        {
            ordered.Add(required);
            uniqueRequested.Remove(required);
        }

        ordered.AddRange(uniqueRequested);
        return ordered;
    }
}
