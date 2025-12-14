using System;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 包上传实体，记录上传的更新包信息
/// </summary>
[SugarTable("package_uploads")]
public class PackageUpload
{
    /// <summary>
    /// 包唯一标识
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 客户唯一标识
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 分支唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? BranchId { get; set; }

    /// <summary>
    /// 节点唯一标识（可选）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? NodeId { get; set; }

    /// <summary>
    /// 部署环境
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DeploymentEnvironment Environment { get; set; }

    /// <summary>
    /// 包资源类型
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public PackageAssetType AssetType { get; set; }

    /// <summary>
    /// COS 对象存储键
    /// </summary>
    [SugarColumn(Length = 256, IsNullable = false)]
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// 文件名
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = false)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 包版本号
    /// </summary>
    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 上传者标识
    /// </summary>
    [SugarColumn(Length = 64, IsNullable = false)]
    public string UploadedBy { get; set; } = "system";

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 验证包上传实体的有效性
    /// </summary>
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
