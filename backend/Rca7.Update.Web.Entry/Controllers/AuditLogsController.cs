using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;

namespace Rca7.Update.Web.Entry.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly AuditLogService _logs;

    public AuditLogsController(AuditLogService logs)
    {
        _logs = logs;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AuditLogResponse>> Get([FromQuery] int limit = 100)
    {
        return Ok(_logs.GetRecent(limit));
    }
}
