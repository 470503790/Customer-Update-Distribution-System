using System;
using System.ComponentModel.DataAnnotations;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

public class NodeInput
{
    [Required]
    public Guid BranchId { get; set; }

    [Required]
    public DeploymentEnvironment Environment { get; set; }

    [Required]
    public string Version { get; set; } = string.Empty;
}
