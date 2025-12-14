using System;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

public class ReleaseOrderService
{
    private static readonly Version MinimumVersion = new(1, 0, 0);
    private static readonly Version MaximumVersion = new(9, 9, 9);
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

    public IEnumerable<ReleaseOrder> GetAll() => _orders.GetAll();

    public ReleaseOrder? Find(Guid id) => _orders.Find(id);

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
