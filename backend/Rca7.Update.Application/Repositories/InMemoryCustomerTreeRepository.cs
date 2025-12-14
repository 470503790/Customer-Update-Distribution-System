using System;
using System.Collections.Generic;
using System.Linq;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

/// <summary>
/// 内存客户树仓储实现，用于测试和原型开发
/// </summary>
public class InMemoryCustomerTreeRepository : ICustomerTreeRepository
{
    /// <summary>
    /// 客户列表
    /// </summary>
    private readonly List<Customer> _customers = new();

    public IEnumerable<Customer> GetCustomers() => _customers;

    public Customer? FindCustomer(Guid id) => _customers.FirstOrDefault(x => x.Id == id);

    public Branch? FindBranch(Guid id) => _customers.SelectMany(c => c.Branches).FirstOrDefault(b => b.Id == id);

    public Node? FindNode(Guid id) => _customers.SelectMany(c => c.Branches).SelectMany(b => b.Nodes).FirstOrDefault(n => n.Id == id);

    public bool CustomerVersionExists(string version) => _customers.Any(c => string.Equals(c.Version, version, StringComparison.OrdinalIgnoreCase));

    public bool BranchVersionExists(Guid customerId, string version) => _customers
        .Where(c => c.Id == customerId)
        .SelectMany(c => c.Branches)
        .Any(b => string.Equals(b.Version, version, StringComparison.OrdinalIgnoreCase));

    public bool NodeVersionExists(Guid branchId, string version) => _customers
        .SelectMany(c => c.Branches)
        .Where(b => b.Id == branchId)
        .SelectMany(b => b.Nodes)
        .Any(n => string.Equals(n.Version, version, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// 保存客户，如不存在则添加，存在则更新
    /// </summary>
    public void SaveCustomer(Customer customer)
    {
        var existing = FindCustomer(customer.Id);
        if (existing == null)
        {
            _customers.Add(customer);
        }
        else
        {
            existing.Name = customer.Name;
            existing.Version = customer.Version;
            existing.Branches = customer.Branches;
        }
    }

    /// <summary>
    /// 保存分支，如不存在则添加，存在则更新
    /// </summary>
    public void SaveBranch(Branch branch)
    {
        var customer = FindCustomer(branch.CustomerId) ?? throw new InvalidOperationException("Customer not found for branch");
        var existing = customer.Branches.FirstOrDefault(b => b.Id == branch.Id);
        if (existing == null)
        {
            customer.Branches.Add(branch);
        }
        else
        {
            existing.Name = branch.Name;
            existing.Version = branch.Version;
            existing.Nodes = branch.Nodes;
        }
    }

    /// <summary>
    /// 保存节点，如不存在则添加，存在则更新
    /// </summary>
    public void SaveNode(Node node)
    {
        var branch = FindBranch(node.BranchId) ?? throw new InvalidOperationException("Branch not found for node");
        var existing = branch.Nodes.FirstOrDefault(n => n.Id == node.Id);
        if (existing == null)
        {
            branch.Nodes.Add(node);
        }
        else
        {
            existing.Environment = node.Environment;
            existing.Version = node.Version;
            existing.NodeToken = node.NodeToken;
        }
    }
}
