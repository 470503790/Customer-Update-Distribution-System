using System;
using SqlSugar;

namespace Rca7.Update.Core.Entities;

public class AgentStepProgress
{
    [SugarColumn(IsIgnore = true)]
    public AgentStep Step { get; set; }

    [SugarColumn(IsIgnore = true)]
    public AgentStepState State { get; set; }

    [SugarColumn(IsIgnore = true)]
    public string? Message { get; set; }

    [SugarColumn(IsIgnore = true)]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
