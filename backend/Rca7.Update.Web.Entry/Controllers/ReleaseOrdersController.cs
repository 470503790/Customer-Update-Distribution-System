using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 发布单管理控制器，提供发布单的创建、查询和回滚接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReleaseOrdersController : ControllerBase
{
    private readonly ReleaseOrderService _service;

    public ReleaseOrdersController(ReleaseOrderService service)
    {
        _service = service;
    }

    /// <summary>
    /// 创建新发布单
    /// </summary>
    [HttpPost]
    public ActionResult<ReleaseOrderResponse> Create([FromBody] ReleaseOrderRequest request)
    {
        var order = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, ToResponse(order));
    }

    /// <summary>
    /// 列出所有发布单
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<ReleaseOrderResponse>> List()
    {
        return Ok(_service.GetAll().Select(ToResponse));
    }

    /// <summary>
    /// 获取指定发布单
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ReleaseOrderResponse> GetById(Guid id)
    {
        var order = _service.Find(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(ToResponse(order));
    }

    /// <summary>
    /// 触发发布单回滚
    /// </summary>
    [HttpPost("{id:guid}/rollback")]
    public ActionResult<ReleaseOrderResponse> Rollback(Guid id, [FromBody] string reason)
    {
        var order = _service.TriggerRollback(id, reason, "api");
        return Ok(ToResponse(order));
    }

    /// <summary>
    /// 将发布单实体转换为响应 DTO
    /// </summary>
    private static ReleaseOrderResponse ToResponse(ReleaseOrder order)
    {
        return new ReleaseOrderResponse
        {
            Id = order.Id,
            Deadline = order.Deadline,
            Status = order.Status,
            Steps = order.Steps,
            RollbackReason = order.RollbackReason
        };
    }
}
