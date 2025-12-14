using System;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

public class PackageUploadRequest
{
    public Guid CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? NodeId { get; set; }
    public DeploymentEnvironment Environment { get; set; }
    public PackageAssetType AssetType { get; set; }
    public string Version { get; set; } = "1.0.0";
    public string FileName { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = "system";
}
