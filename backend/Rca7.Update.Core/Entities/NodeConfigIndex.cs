using System;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

/// <summary>
/// Read-only index table for configuration versions bound to nodes.
/// </summary>
[SugarTable("node_config_index")]
public class NodeConfigIndex
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    [SugarColumn(IsNullable = false)]
    public Guid NodeId { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(NodeId))]
    public Node? Node { get; set; }

    [SugarColumn(Length = 32, IsNullable = false, IsOnlyIgnoreUpdate = true)]
    public string ConfigVersion { get; set; } = "1.0.0";

    [SugarColumn(IsNullable = false, IsOnlyIgnoreUpdate = true)]
    public bool IsLocked { get; set; }

    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureVersionInRange(ConfigVersion, minimum, maximum, nameof(ConfigVersion));
    }
}
