using System;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

/// <summary>
/// 审计日志服务，记录和查询系统操作日志
/// </summary>
public class AuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 记录审计日志
    /// </summary>
    public AuditLog Record(string category, string message, string actor, string? correlationId = null)
    {
        var entry = new AuditLog
        {
            Category = category,
            Message = message,
            Actor = actor,
            CorrelationId = correlationId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        return _repository.Save(entry);
    }

    /// <summary>
    /// 获取最近的审计日志
    /// </summary>
    public IEnumerable<AuditLogResponse> GetRecent(int limit = 100)
    {
        return _repository.GetRecent(limit)
            .Select(x => new AuditLogResponse
            {
                Id = x.Id,
                Category = x.Category,
                Message = x.Message,
                Actor = x.Actor,
                CreatedAt = x.CreatedAt,
                CorrelationId = x.CorrelationId
            });
    }
}
