using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 客户树仓储接口
/// </summary>
public interface ICustomerTreeRepository
{
    /// <summary>
    /// 获取所有客户及其子树
    /// </summary>
    IEnumerable<Customer> GetCustomers();
    
    /// <summary>
    /// 查找指定客户
    /// </summary>
    Customer? FindCustomer(Guid id);
    
    /// <summary>
    /// 查找指定分支
    /// </summary>
    Branch? FindBranch(Guid id);
    
    /// <summary>
    /// 查找指定节点
    /// </summary>
    Node? FindNode(Guid id);

    /// <summary>
    /// 检查客户版本号是否已存在
    /// </summary>
    bool CustomerVersionExists(string version);
    
    /// <summary>
    /// 检查分支版本号在指定客户下是否已存在
    /// </summary>
    bool BranchVersionExists(Guid customerId, string version);
    
    /// <summary>
    /// 检查节点版本号在指定分支下是否已存在
    /// </summary>
    bool NodeVersionExists(Guid branchId, string version);

    /// <summary>
    /// 保存客户
    /// </summary>
    void SaveCustomer(Customer customer);
    
    /// <summary>
    /// 保存分支
    /// </summary>
    void SaveBranch(Branch branch);
    
    /// <summary>
    /// 保存节点
    /// </summary>
    void SaveNode(Node node);
}
