using System;
using System.Collections.Generic;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 发布单实体，定义一次更新发布的详细信息和执行步骤
/// </summary>
[SugarTable("release_orders")]
public class ReleaseOrder
{
    /// <summary>
    /// 发布单唯一标识
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 客户唯一标识
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 分支唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? BranchId { get; set; }

    /// <summary>
    /// 节点唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? NodeId { get; set; }

    /// <summary>
    /// 部署环境
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DeploymentEnvironment Environment { get; set; }

    /// <summary>
    /// 发布策略
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public ReleaseStrategy Strategy { get; set; }

    /// <summary>
    /// 发布状态
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public ReleaseStatus Status { get; set; } = ReleaseStatus.Scheduled;

    /// <summary>
    /// 截止时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTimeOffset Deadline { get; set; }

    /// <summary>
    /// 服务端包唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? ServerPackageId { get; set; }

    /// <summary>
    /// 客户端包唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? ClientPackageId { get; set; }

    /// <summary>
    /// 配置包唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? ConfigurationPackageId { get; set; }

    /// <summary>
    /// 执行步骤及进度列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<AgentStepProgress> Steps { get; set; } = new();

    /// <summary>
    /// 回滚原因（如果已回滚）
    /// </summary>
    [SugarColumn(Length = 256, IsNullable = true)]
    public string? RollbackReason { get; set; }

    /// <summary>
    /// 验证发布单实体的有效性
    /// </summary>
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

    /// <summary>
    /// 标记指定步骤的执行状态和结果
    /// </summary>
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
