using System;
using SqlSugar;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 审计日志实体，用于记录系统操作和事件
/// </summary>
[SugarTable("audit_logs")]
public class AuditLog
{
    /// <summary>
    /// 审计日志唯一标识（自增长）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>
    /// 审计类别（如：创建、回滚、删除等）
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = false)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 审计消息内容
    /// </summary>
    [SugarColumn(Length = 256, IsNullable = false)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 操作者标识
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = false)]
    public string Actor { get; set; } = "system";

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 关联标识（用于追踪相关操作）
    /// </summary>
    [SugarColumn(IsNullable = true, Length = 256)]
    public string? CorrelationId { get; set; }
}
