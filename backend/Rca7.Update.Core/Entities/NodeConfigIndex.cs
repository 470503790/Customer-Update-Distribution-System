using System;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// 节点配置索引实体，只读索引表，记录绑定到节点的配置版本
/// </summary>
[SugarTable("node_config_index")]
public class NodeConfigIndex
{
    /// <summary>
    /// 索引唯一标识（自增长）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>
    /// 节点唯一标识
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public Guid NodeId { get; set; }

    /// <summary>
    /// 节点导航属性
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(NodeId))]
    public Node? Node { get; set; }

    /// <summary>
    /// 配置版本号（仅插入时设置，不可更新）
    /// </summary>
    [SugarColumn(Length = 32, IsNullable = false, IsOnlyIgnoreUpdate = true)]
    public string ConfigVersion { get; set; } = "1.0.0";

    /// <summary>
    /// 是否锁定（仅插入时设置，不可更新）
    /// </summary>
    [SugarColumn(IsNullable = false, IsOnlyIgnoreUpdate = true)]
    public bool IsLocked { get; set; }

    /// <summary>
    /// 验证节点配置索引的有效性
    /// </summary>
    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureVersionInRange(ConfigVersion, minimum, maximum, nameof(ConfigVersion));
    }
}
