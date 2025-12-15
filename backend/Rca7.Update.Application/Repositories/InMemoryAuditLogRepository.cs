using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 内存审计日志仓储实现，用于测试和原型开发
/// </summary>
public class InMemoryAuditLogRepository : IAuditLogRepository
{
    /// <summary>
    /// 审计日志存储
    /// </summary>
    private readonly ConcurrentDictionary<long, AuditLog> _entries = new();
    
    /// <summary>
    /// 自增序列号
    /// </summary>
    private long _sequence = 0;

    /// <summary>
    /// 保存审计日志并分配自增ID
    /// </summary>
    public AuditLog Save(AuditLog entry)
    {
        var id = System.Threading.Interlocked.Increment(ref _sequence);
        entry.Id = id;
        _entries[id] = entry;
        return entry;
    }

    /// <summary>
    /// 获取最近的审计日志，按创建时间倒序
    /// </summary>
    public IEnumerable<AuditLog> GetRecent(int limit = 100)
    {
        return _entries.Values
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit);
    }
}
