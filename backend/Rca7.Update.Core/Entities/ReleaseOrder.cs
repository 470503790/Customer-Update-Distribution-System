using System;
using System.Collections.Generic;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

[SugarTable("release_orders")]
public class ReleaseOrder
{
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false)]
    public Guid CustomerId { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? BranchId { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? NodeId { get; set; }

    [SugarColumn(IsNullable = false)]
    public DeploymentEnvironment Environment { get; set; }

    [SugarColumn(IsNullable = false)]
    public ReleaseStrategy Strategy { get; set; }

    [SugarColumn(IsNullable = false)]
    public ReleaseStatus Status { get; set; } = ReleaseStatus.Scheduled;

    [SugarColumn(IsNullable = false)]
    public DateTimeOffset Deadline { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? ServerPackageId { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? ClientPackageId { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? ConfigurationPackageId { get; set; }

    [SugarColumn(IsIgnore = true)]
    public List<AgentStepProgress> Steps { get; set; } = new();

    [SugarColumn(Length = 256, IsNullable = true)]
    public string? RollbackReason { get; set; }

    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureEnumDefined(Environment, nameof(Environment));
        DomainValidations.EnsureEnumDefined(Strategy, nameof(Strategy));

        if (Deadline <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(Deadline), "Deadline must be in the future");
        }

        if (ServerPackageId is null && ClientPackageId is null && ConfigurationPackageId is null)
        {
            throw new ArgumentException("At least one package must be linked to release order", nameof(ServerPackageId));
        }

        foreach (var step in Steps)
        {
            DomainValidations.EnsureEnumDefined(step.Step, nameof(AgentStep));
            DomainValidations.EnsureEnumDefined(step.State, nameof(AgentStepState));
        }
    }

    public void MarkStep(AgentStep step, AgentStepState state, string? message = null)
    {
        var progress = Steps.Find(s => s.Step == step);
        if (progress == null)
        {
            progress = new AgentStepProgress { Step = step };
            Steps.Add(progress);
        }

        progress.State = state;
        progress.Message = message;
        progress.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
