using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 数据库客户树仓储实现，使用 SqlSugar ORM
/// </summary>
public class DatabaseCustomerTreeRepository : ICustomerTreeRepository
{
    private readonly ISqlSugarClient _db;

    public DatabaseCustomerTreeRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取所有客户及其完整树结构
    /// </summary>
    public IEnumerable<Customer> GetCustomers()
    {
        return BuildCustomerTree();
    }

    /// <summary>
    /// 查找指定客户及其树结构
    /// </summary>
    public Customer? FindCustomer(Guid id)
    {
        return BuildCustomerTree(customers => customers.Where(c => c.Id == id)).FirstOrDefault();
    }

    /// <summary>
    /// 查找指定分支及其节点
    /// </summary>
    public Branch? FindBranch(Guid id)
    {
        var branch = _db.Queryable<Branch>().First(b => b.Id == id);
        if (branch != null)
        {
            branch.Nodes = _db.Queryable<Node>().Where(n => n.BranchId == branch.Id).ToList();
        }

        return branch;
    }

    /// <summary>
    /// 查找指定节点
    /// </summary>
    public Node? FindNode(Guid id)
    {
        return _db.Queryable<Node>().First(n => n.Id == id);
    }

    /// <summary>
    /// 检查客户版本号是否已存在（忽略大小写）
    /// </summary>
    public bool CustomerVersionExists(string version)
    {
        var normalized = NormalizeVersion(version);
        return _db.Queryable<Customer>().Any(c => SqlFunc.ToLower(c.Version) == normalized);
    }

    /// <summary>
    /// 检查分支版本号在指定客户下是否已存在（忽略大小写）
    /// </summary>
    public bool BranchVersionExists(Guid customerId, string version)
    {
        var normalized = NormalizeVersion(version);
        return _db.Queryable<Branch>()
            .Any(b => b.CustomerId == customerId && SqlFunc.ToLower(b.Version) == normalized);
    }

    /// <summary>
    /// 检查节点版本号在指定分支下是否已存在（忽略大小写）
    /// </summary>
    public bool NodeVersionExists(Guid branchId, string version)
    {
        var normalized = NormalizeVersion(version);
        return _db.Queryable<Node>()
            .Any(n => n.BranchId == branchId && SqlFunc.ToLower(n.Version) == normalized);
    }

    /// <summary>
    /// 保存客户及其所有子分支和节点
    /// </summary>
    public void SaveCustomer(Customer customer)
    {
        UpsertCustomer(customer);

        foreach (var branch in customer.Branches)
        {
            branch.CustomerId = customer.Id;
            SaveBranch(branch);
        }
    }

    /// <summary>
    /// 保存分支及其所有子节点
    /// </summary>
    public void SaveBranch(Branch branch)
    {
        UpsertBranch(branch);

        foreach (var node in branch.Nodes)
        {
            node.BranchId = branch.Id;
            SaveNode(node);
        }
    }

    /// <summary>
    /// 保存节点，如不存在则插入，存在则更新
    /// </summary>
    public void SaveNode(Node node)
    {
        var exists = _db.Queryable<Node>().Any(n => n.Id == node.Id);
        if (exists)
        {
            _db.Updateable(node).IgnoreColumns(n => n.Id).ExecuteCommand();
        }
        else
        {
            _db.Insertable(node).ExecuteCommand();
        }
    }

    /// <summary>
    /// 规范化版本号为小写
    /// </summary>
    private static string NormalizeVersion(string version) => version.Trim().ToLowerInvariant();

    /// <summary>
    /// 插入或更新客户
    /// </summary>
    private void UpsertCustomer(Customer customer)
    {
        var exists = _db.Queryable<Customer>().Any(c => c.Id == customer.Id);
        if (exists)
        {
            _db.Updateable(customer).IgnoreColumns(c => c.Id).ExecuteCommand();
        }
        else
        {
            _db.Insertable(customer).ExecuteCommand();
        }
    }

    /// <summary>
    /// 插入或更新分支
    /// </summary>
    private void UpsertBranch(Branch branch)
    {
        var exists = _db.Queryable<Branch>().Any(b => b.Id == branch.Id);
        if (exists)
        {
            _db.Updateable(branch).IgnoreColumns(b => b.Id).ExecuteCommand();
        }
        else
        {
            _db.Insertable(branch).ExecuteCommand();
        }
    }

    /// <summary>
    /// 构建客户树结构，高效加载客户-分支-节点三层关系
    /// </summary>
    private IEnumerable<Customer> BuildCustomerTree(Func<ISugarQueryable<Customer>, ISugarQueryable<Customer>>? customerQuery = null)
    {
        var customersQueryable = _db.Queryable<Customer>();
        var customers = (customerQuery != null ? customerQuery(customersQueryable) : customersQueryable).ToList();

        if (customers.Count == 0)
        {
            return customers;
        }

        var customerIds = customers.Select(c => c.Id).ToList();
        var branches = _db.Queryable<Branch>().In(b => b.CustomerId, customerIds).ToList();
        var branchIds = branches.Select(b => b.Id).ToList();
        var nodes = branchIds.Count == 0
            ? new List<Node>()
            : _db.Queryable<Node>().In(n => n.BranchId, branchIds).ToList();

        var nodeLookup = nodes.ToLookup(n => n.BranchId);
        foreach (var branch in branches)
        {
            branch.Nodes = nodeLookup[branch.Id].ToList();
        }

        var branchLookup = branches.ToLookup(b => b.CustomerId);
        foreach (var customer in customers)
        {
            customer.Branches = branchLookup[customer.Id].ToList();
        }

        return customers;
    }
}
