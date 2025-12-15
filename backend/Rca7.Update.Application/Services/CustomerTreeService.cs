using System;
using System.Collections.Generic;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

/// <summary>
/// 客户树服务，管理客户-分支-节点三层树模型
/// </summary>
public class CustomerTreeService
{
    /// <summary>
    /// 最小版本号限制
    /// </summary>
    private static readonly Version MinimumVersion = new(1, 0, 0);
    
    /// <summary>
    /// 最大版本号限制
    /// </summary>
    private static readonly Version MaximumVersion = new(9, 9, 9);

    private readonly ICustomerTreeRepository _repository;

    public CustomerTreeService(ICustomerTreeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取完整客户树
    /// </summary>
    public IEnumerable<Customer> GetTree() => _repository.GetCustomers();

    /// <summary>
    /// 创建新客户，版本号需唯一
    /// </summary>
    public Customer CreateCustomer(CustomerInput input)
    {
        var customer = new Customer
        {
            Name = input.Name,
            Version = input.Version
        };

        ValidateCustomer(customer);

        if (_repository.CustomerVersionExists(customer.Version))
        {
            throw new InvalidOperationException($"Customer version {customer.Version} already exists");
        }

        _repository.SaveCustomer(customer);
        return customer;
    }

    /// <summary>
    /// 为客户添加分支，版本号在同一客户下需唯一
    /// </summary>
    public Branch AddBranch(BranchInput input)
    {
        var parent = _repository.FindCustomer(input.CustomerId) ?? throw new InvalidOperationException("Customer not found");
        var branch = new Branch
        {
            CustomerId = input.CustomerId,
            Name = input.Name,
            Version = input.Version
        };

        ValidateBranch(branch);

        if (_repository.BranchVersionExists(branch.CustomerId, branch.Version))
        {
            throw new InvalidOperationException($"Branch version {branch.Version} already exists under customer {parent.Name}");
        }

        parent.Branches.Add(branch);
        _repository.SaveCustomer(parent);
        return branch;
    }

    /// <summary>
    /// 为分支添加节点，版本号在同一分支下需唯一
    /// </summary>
    public Node AddNode(NodeInput input)
    {
        var branch = _repository.FindBranch(input.BranchId) ?? throw new InvalidOperationException("Branch not found");
        var node = new Node
        {
            BranchId = input.BranchId,
            Environment = input.Environment,
            Version = input.Version
        };

        ValidateNode(node);

        if (_repository.NodeVersionExists(node.BranchId, node.Version))
        {
            throw new InvalidOperationException($"Node version {node.Version} already exists under branch {branch.Name}");
        }

        branch.Nodes.Add(node);
        _repository.SaveBranch(branch);
        return node;
    }

    /// <summary>
    /// 为节点生成一次性令牌，用于客户端认证
    /// </summary>
    public NodeTokenResponse GenerateNodeToken(Guid nodeId)
    {
        var node = _repository.FindNode(nodeId) ?? throw new InvalidOperationException("Node not found");
        if (!string.IsNullOrWhiteSpace(node.NodeToken))
        {
            throw new InvalidOperationException("Node token has already been generated and cannot be displayed again.");
        }

        node.NodeToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        _repository.SaveNode(node);

        return new NodeTokenResponse
        {
            NodeId = node.Id,
            Token = node.NodeToken
        };
    }

    /// <summary>
    /// 验证客户实体
    /// </summary>
    private void ValidateCustomer(Customer customer) => customer.Validate(MinimumVersion, MaximumVersion);

    /// <summary>
    /// 验证分支实体
    /// </summary>
    private void ValidateBranch(Branch branch) => branch.Validate(MinimumVersion, MaximumVersion);

    /// <summary>
    /// 验证节点实体
    /// </summary>
    private void ValidateNode(Node node) => node.Validate(MinimumVersion, MaximumVersion);
}
