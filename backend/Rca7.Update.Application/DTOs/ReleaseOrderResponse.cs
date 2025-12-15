using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 发布单响应 DTO
/// </summary>
public class ReleaseOrderResponse
{
    /// <summary>
    /// 发布单唯一标识
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 发布状态（待定/进行中/成功/失败/已回滚等）
    /// </summary>
    public ReleaseStatus Status { get; set; }
    
    /// <summary>
    /// 截止时间
    /// </summary>
    public DateTimeOffset Deadline { get; set; }
    
    /// <summary>
    /// 执行步骤及进度列表
    /// </summary>
    public IEnumerable<AgentStepProgress> Steps { get; set; } = Array.Empty<AgentStepProgress>();
    
    /// <summary>
    /// 回滚原因（如果已回滚）
    /// </summary>
    public string? RollbackReason { get; set; }
}
