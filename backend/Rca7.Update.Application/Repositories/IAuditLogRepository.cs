using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 审计日志仓储接口
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// 保存审计日志
    /// </summary>
    AuditLog Save(AuditLog entry);
    
    /// <summary>
    /// 获取最近的审计日志
    /// </summary>
    IEnumerable<AuditLog> GetRecent(int limit = 100);
}
