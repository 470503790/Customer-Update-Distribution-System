using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 发布单仓储接口
/// </summary>
public interface IReleaseOrderRepository
{
    /// <summary>
    /// 保存发布单
    /// </summary>
    ReleaseOrder Save(ReleaseOrder order);
    
    /// <summary>
    /// 查找指定发布单
    /// </summary>
    ReleaseOrder? Find(Guid id);
    
    /// <summary>
    /// 获取所有发布单
    /// </summary>
    IEnumerable<ReleaseOrder> GetAll();
}
