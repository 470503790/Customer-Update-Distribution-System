using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

public class ReleaseOrderResponse
{
    public Guid Id { get; set; }
    public ReleaseStatus Status { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public IEnumerable<AgentStepProgress> Steps { get; set; } = Array.Empty<AgentStepProgress>();
    public string? RollbackReason { get; set; }
}
