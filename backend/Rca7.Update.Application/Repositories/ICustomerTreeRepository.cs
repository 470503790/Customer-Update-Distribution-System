using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public interface ICustomerTreeRepository
{
    IEnumerable<Customer> GetCustomers();
    Customer? FindCustomer(Guid id);
    Branch? FindBranch(Guid id);
    Node? FindNode(Guid id);

    bool CustomerVersionExists(string version);
    bool BranchVersionExists(Guid customerId, string version);
    bool NodeVersionExists(Guid branchId, string version);

    void SaveCustomer(Customer customer);
    void SaveBranch(Branch branch);
    void SaveNode(Node node);
}
