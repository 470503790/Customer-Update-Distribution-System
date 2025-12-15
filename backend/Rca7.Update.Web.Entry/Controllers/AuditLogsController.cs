using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 审计日志控制器，提供审计日志查询接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly AuditLogService _logs;

    public AuditLogsController(AuditLogService logs)
    {
        _logs = logs;
    }

    /// <summary>
    /// 获取最近的审计日志
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<AuditLogResponse>> Get([FromQuery] int limit = 100)
    {
        return Ok(_logs.GetRecent(limit));
    }
}
