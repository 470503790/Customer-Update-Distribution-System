using System;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 审计日志响应 DTO
/// </summary>
public class AuditLogResponse
{
    /// <summary>
    /// 审计日志唯一标识
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 审计类别（如：创建、回滚等）
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// 审计消息内容
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// 操作者标识
    /// </summary>
    public string Actor { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 关联标识（用于追踪相关操作）
    /// </summary>
    public string? CorrelationId { get; set; }
}
