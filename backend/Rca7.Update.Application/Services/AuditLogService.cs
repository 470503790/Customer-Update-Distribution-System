using System;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

public class AuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

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
