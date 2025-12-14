using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

public class ReleaseOrderRequest
{
    public Guid CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? NodeId { get; set; }
    public DeploymentEnvironment Environment { get; set; }
    public ReleaseStrategy Strategy { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public Guid? ServerPackageId { get; set; }
    public Guid? ClientPackageId { get; set; }
    public Guid? ConfigurationPackageId { get; set; }
    public IEnumerable<AgentStep>? Steps { get; set; }
    public string RequestedBy { get; set; } = "system";
}
