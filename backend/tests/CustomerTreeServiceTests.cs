using System;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;
using Xunit;

namespace Rca7.Update.Tests;

public class CustomerTreeServiceTests
{
    private readonly CustomerTreeService _service;
    private readonly Guid _customerId;
    private readonly Guid _branchId;

    public CustomerTreeServiceTests()
    {
        _service = new CustomerTreeService(new InMemoryCustomerTreeRepository());
        var customer = _service.CreateCustomer(new CustomerInput { Name = "Acme", Version = "1.0.0" });
        _customerId = customer.Id;
        var branch = _service.AddBranch(new BranchInput { CustomerId = _customerId, Name = "Main", Version = "1.0.0" });
        _branchId = branch.Id;
    }

    [Fact]
    public void Should_EnforceCustomerVersionUniqueness()
    {
        var input = new CustomerInput { Name = "Acme Clone", Version = "1.0.0" };
        Assert.Throws<InvalidOperationException>(() => _service.CreateCustomer(input));
    }

    [Fact]
    public void Should_RejectBranchVersionBelowMinimum()
    {
        var input = new BranchInput { CustomerId = _customerId, Name = "Legacy", Version = "0.9.0" };
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.AddBranch(input));
    }

    [Fact]
    public void Should_ValidateEnvironmentEnum()
    {
        var input = new NodeInput { BranchId = _branchId, Environment = (DeploymentEnvironment)999, Version = "1.0.1" };
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.AddNode(input));
    }

    [Fact]
    public void Should_Generate_NodeToken_OnlyOnce()
    {
        var node = _service.AddNode(new NodeInput { BranchId = _branchId, Environment = DeploymentEnvironment.Production, Version = "1.0.2" });
        var tokenResponse = _service.GenerateNodeToken(node.Id);
        Assert.False(string.IsNullOrWhiteSpace(tokenResponse.Token));
        Assert.Throws<InvalidOperationException>(() => _service.GenerateNodeToken(node.Id));
    }
}
