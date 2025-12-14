using System;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;
using Xunit;

namespace Rca7.Update.Tests;

public class ReleaseOrderServiceTests
{
    private readonly ReleaseOrderService _service;
    private readonly Guid _customerId = Guid.NewGuid();

    public ReleaseOrderServiceTests()
    {
        var orderRepo = new InMemoryReleaseOrderRepository();
        var packageRepo = new InMemoryPackageRepository();
        var auditService = new AuditLogService(new InMemoryAuditLogRepository());
        var orchestrator = new ReleaseOrchestrator(orderRepo, packageRepo, auditService, NullLogger<ReleaseOrchestrator>.Instance);
        _service = new ReleaseOrderService(orderRepo, orchestrator, auditService);
    }

    [Fact]
    public void Should_InjectDefaultSteps_WhenRequestIsPartial()
    {
        var request = new ReleaseOrderRequest
        {
            CustomerId = _customerId,
            Environment = DeploymentEnvironment.Production,
            Strategy = ReleaseStrategy.Rolling,
            Deadline = DateTimeOffset.UtcNow.AddHours(1),
            ServerPackageId = Guid.NewGuid(),
            Steps = new[] { AgentStep.DeployServer }
        };

        var order = _service.Create(request);

        Assert.Equal(ReleaseOrderService.DefaultStepOrder, order.Steps.Select(s => s.Step));
    }

    [Fact]
    public void Should_AppendCustomStepsAfterDefaults()
    {
        var customStep = AgentStep.TriggerRollback;
        var request = new ReleaseOrderRequest
        {
            CustomerId = _customerId,
            Environment = DeploymentEnvironment.Staging,
            Strategy = ReleaseStrategy.BlueGreen,
            Deadline = DateTimeOffset.UtcNow.AddHours(2),
            ServerPackageId = Guid.NewGuid(),
            Steps = new[] { customStep, AgentStep.FullBackup, customStep }
        };

        var order = _service.Create(request);
        var normalized = order.Steps.Select(s => s.Step).ToList();

        Assert.Equal(customStep, normalized.Last());
        Assert.Equal(ReleaseOrderService.DefaultStepOrder, normalized.Take(ReleaseOrderService.DefaultStepOrder.Count));
    }
}
