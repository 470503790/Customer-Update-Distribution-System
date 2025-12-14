using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 内存包仓储实现，用于测试和原型开发
/// </summary>
public class InMemoryPackageRepository : IPackageRepository
{
    /// <summary>
    /// 包存储
    /// </summary>
    private readonly ConcurrentDictionary<Guid, PackageUpload> _store = new();

    /// <summary>
    /// 保存包信息
    /// </summary>
    public PackageUpload Save(PackageUpload package)
    {
        _store[package.Id] = package;
        return package;
    }

    /// <summary>
    /// 查找指定包
    /// </summary>
    public PackageUpload? Find(Guid id)
    {
        _store.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 列出指定客户的所有包，按创建时间倒序
    /// </summary>
    public IEnumerable<PackageUpload> ListByCustomer(Guid customerId)
    {
        return _store.Values.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt);
    }
}
