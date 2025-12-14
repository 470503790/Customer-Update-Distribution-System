using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public class InMemoryAuditLogRepository : IAuditLogRepository
{
    private readonly ConcurrentDictionary<long, AuditLog> _entries = new();
    private long _sequence = 0;

    public AuditLog Save(AuditLog entry)
    {
        var id = System.Threading.Interlocked.Increment(ref _sequence);
        entry.Id = id;
        _entries[id] = entry;
        return entry;
    }

    public IEnumerable<AuditLog> GetRecent(int limit = 100)
    {
        return _entries.Values
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit);
    }
}
