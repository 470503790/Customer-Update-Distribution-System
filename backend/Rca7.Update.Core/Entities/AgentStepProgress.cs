using System;
using SqlSugar;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 代理步骤进度，记录单个步骤的执行状态和结果
/// </summary>
public class AgentStepProgress
{
    /// <summary>
    /// 执行步骤
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public AgentStep Step { get; set; }

    /// <summary>
    /// 步骤状态（待定/运行中/完成/失败/跳过）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public AgentStepState State { get; set; }

    /// <summary>
    /// 步骤消息或错误信息
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string? Message { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
