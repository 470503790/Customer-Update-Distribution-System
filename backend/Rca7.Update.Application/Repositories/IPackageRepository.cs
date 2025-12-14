using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 包仓储接口
/// </summary>
public interface IPackageRepository
{
    /// <summary>
    /// 保存包信息
    /// </summary>
    PackageUpload Save(PackageUpload package);
    
    /// <summary>
    /// 查找指定包
    /// </summary>
    PackageUpload? Find(Guid id);
    
    /// <summary>
    /// 列出指定客户的所有包
    /// </summary>
    IEnumerable<PackageUpload> ListByCustomer(Guid customerId);
}
