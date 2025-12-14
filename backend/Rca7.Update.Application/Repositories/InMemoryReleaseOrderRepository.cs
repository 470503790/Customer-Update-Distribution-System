using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public class InMemoryReleaseOrderRepository : IReleaseOrderRepository
{
    private readonly ConcurrentDictionary<Guid, ReleaseOrder> _store = new();

    public ReleaseOrder Save(ReleaseOrder order)
    {
        _store[order.Id] = order;
        return order;
    }

    public ReleaseOrder? Find(Guid id)
    {
        _store.TryGetValue(id, out var value);
        return value;
    }

    public IEnumerable<ReleaseOrder> GetAll()
    {
        return _store.Values.OrderByDescending(x => x.Deadline);
    }
}
