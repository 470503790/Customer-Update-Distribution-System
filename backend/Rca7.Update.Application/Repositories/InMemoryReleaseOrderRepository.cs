using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 内存发布单仓储实现，用于测试和原型开发
/// </summary>
public class InMemoryReleaseOrderRepository : IReleaseOrderRepository
{
    /// <summary>
    /// 发布单存储
    /// </summary>
    private readonly ConcurrentDictionary<Guid, ReleaseOrder> _store = new();

    /// <summary>
    /// 保存发布单
    /// </summary>
    public ReleaseOrder Save(ReleaseOrder order)
    {
        _store[order.Id] = order;
        return order;
    }

    /// <summary>
    /// 查找指定发布单
    /// </summary>
    public ReleaseOrder? Find(Guid id)
    {
        _store.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 获取所有发布单，按截止时间倒序
    /// </summary>
    public IEnumerable<ReleaseOrder> GetAll()
    {
        return _store.Values.OrderByDescending(x => x.Deadline);
    }
}
