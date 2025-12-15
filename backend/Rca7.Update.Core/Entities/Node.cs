using System;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 节点实体，代表客户树模型的叶子节点，对应实际部署的环境
/// </summary>
[SugarTable("nodes")]
public class Node
{
    /// <summary>
    /// 节点唯一标识
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属分支唯一标识
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public Guid BranchId { get; set; }

    /// <summary>
    /// 所属分支导航属性
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(BranchId))]
    public Branch? Branch { get; set; }

    /// <summary>
    /// 部署环境（开发/测试/生产等）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DeploymentEnvironment Environment { get; set; }

    /// <summary>
    /// 节点版本号（在同一分支下需唯一）
    /// </summary>
    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 节点令牌（一次性生成，用于客户端认证）
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = true)]
    public string? NodeToken { get; set; }

    /// <summary>
    /// 验证节点实体的有效性
    /// </summary>
    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureEnumDefined(Environment, nameof(Environment));
        DomainValidations.EnsureVersionInRange(Version, minimum, maximum, nameof(Version));
    }
}
