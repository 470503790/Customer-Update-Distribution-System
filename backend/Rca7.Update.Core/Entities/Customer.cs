using System;
using System.Collections.Generic;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 客户实体，代表客户树模型的根节点
/// </summary>
[SugarTable("customers")]
public class Customer
{
    /// <summary>
    /// 客户唯一标识
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 客户名称
    /// </summary>
    [SugarColumn(Length = 128, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户版本号（需唯一）
    /// </summary>
    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 客户下的分支列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Branch> Branches { get; set; } = new();

    /// <summary>
    /// 验证客户实体的有效性
    /// </summary>
    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureNotEmpty(Name, nameof(Name));
        DomainValidations.EnsureVersionInRange(Version, minimum, maximum, nameof(Version));
    }
}
