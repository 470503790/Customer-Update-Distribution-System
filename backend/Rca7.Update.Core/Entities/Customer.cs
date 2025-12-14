using System;
using System.Collections.Generic;
using SqlSugar;
using Rca7.Update.Core.Abstractions;

namespace Rca7.Update.Core.Entities;

[SugarTable("customers")]
public class Customer
{
    [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 128, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 32, IsNullable = false)]
    public string Version { get; set; } = "1.0.0";

    [SugarColumn(IsIgnore = true)]
    public List<Branch> Branches { get; set; } = new();

    public void Validate(Version minimum, Version maximum)
    {
        DomainValidations.EnsureNotEmpty(Name, nameof(Name));
        DomainValidations.EnsureVersionInRange(Version, minimum, maximum, nameof(Version));
    }
}
