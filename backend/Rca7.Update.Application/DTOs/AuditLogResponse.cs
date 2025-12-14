using System;

namespace Rca7.Update.Application.DTOs;

public class AuditLogResponse
{
    public long Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? CorrelationId { get; set; }
}
