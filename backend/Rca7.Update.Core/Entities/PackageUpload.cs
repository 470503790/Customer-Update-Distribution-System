using System;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

[SugarTable("package_uploads")]
public class PackageUpload
{
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false)]
    public Guid CustomerId { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? BranchId { get; set; }

    [SugarColumn(IsNullable = true)]
    public Guid? NodeId { get; set; }

    [SugarColumn(IsNullable = false)]
    public DeploymentEnvironment Environment { get; set; }

    [SugarColumn(IsNullable = false)]
    public PackageAssetType AssetType { get; set; }

    [SugarColumn(Length = 256, IsNullable = false)]
    public string ObjectKey { get; set; } = string.Empty;

    [SugarColumn(Length = 128, IsNullable = false)]
    public string FileName { get; set; } = string.Empty;

    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    [SugarColumn(Length = 64, IsNullable = false)]
    public string UploadedBy { get; set; } = "system";

    [SugarColumn(IsNullable = false)]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureEnumDefined(Environment, nameof(Environment));
        DomainValidations.EnsureEnumDefined(AssetType, nameof(AssetType));
        DomainValidations.EnsureVersionInRange(Version, minimum, maximum, nameof(Version));
        DomainValidations.EnsureNotEmpty(ObjectKey, nameof(ObjectKey));
        DomainValidations.EnsureNotEmpty(FileName, nameof(FileName));
        DomainValidations.EnsureNotEmpty(UploadedBy, nameof(UploadedBy));
    }
}
