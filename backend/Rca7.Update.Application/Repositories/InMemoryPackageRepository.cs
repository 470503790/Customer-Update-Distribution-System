using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public class InMemoryPackageRepository : IPackageRepository
{
    private readonly ConcurrentDictionary<Guid, PackageUpload> _store = new();

    public PackageUpload Save(PackageUpload package)
    {
        _store[package.Id] = package;
        return package;
    }

    public PackageUpload? Find(Guid id)
    {
        _store.TryGetValue(id, out var value);
        return value;
    }

    public IEnumerable<PackageUpload> ListByCustomer(Guid customerId)
    {
        return _store.Values.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt);
    }
}
