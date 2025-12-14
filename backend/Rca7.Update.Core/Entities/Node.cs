using System;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

[SugarTable("nodes")]
public class Node
{
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false)]
    public Guid BranchId { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(BranchId))]
    public Branch? Branch { get; set; }

    [SugarColumn(IsNullable = false)]
    public DeploymentEnvironment Environment { get; set; }

    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    [SugarColumn(Length = 128, IsNullable = true)]
    public string? NodeToken { get; set; }

    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureEnumDefined(Environment, nameof(Environment));
        DomainValidations.EnsureVersionInRange(Version, minimum, maximum, nameof(Version));
    }
}
