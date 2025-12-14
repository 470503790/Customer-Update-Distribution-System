using System;
using SqlSugar;

namespace Rca7.Update.Core.Entities;

[SugarTable("audit_logs")]
public class AuditLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    [SugarColumn(Length = 64, IsNullable = false)]
    public string Category { get; set; } = string.Empty;

    [SugarColumn(Length = 256, IsNullable = false)]
    public string Message { get; set; } = string.Empty;

    [SugarColumn(Length = 64, IsNullable = false)]
    public string Actor { get; set; } = "system";

    [SugarColumn(IsNullable = false)]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [SugarColumn(IsNullable = true, Length = 256)]
    public string? CorrelationId { get; set; }
}
