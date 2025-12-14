using System;
using System.Collections.Generic;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 分支实体，代表客户树模型的第二层节点
/// </summary>
[SugarTable("branches")]
public class Branch
{
    /// <summary>
    /// 分支唯一标识
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属客户唯一标识
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 所属客户导航属性
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(CustomerId))]
    public Customer? Customer { get; set; }

    /// <summary>
    /// 分支名称
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分支版本号（在同一客户下需唯一）
    /// </summary>
    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 分支下的节点列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Node> Nodes { get; set; } = new();

    /// <summary>
    /// 验证分支实体的有效性
    /// </summary>
    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureNotEmpty(Name, nameof(Name));
        DomainValidations.EnsureVersionInRange(Version, minimum, maximum, nameof(Version));
    }
}
