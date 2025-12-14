using System;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 节点管理控制器，提供为分支添加节点和生成令牌的接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NodesController : ControllerBase
{
    private readonly CustomerTreeService _service;

    public NodesController(CustomerTreeService service)
    {
        _service = service;
    }

    /// <summary>
    /// 为分支创建新节点
    /// </summary>
    [HttpPost]
    public ActionResult<Node> Create([FromBody] NodeInput input)
    {
        var node = _service.AddNode(input);
        return CreatedAtAction(nameof(Create), new { id = node.Id }, node);
    }

    /// <summary>
    /// 为节点生成一次性令牌
    /// </summary>
    [HttpPost("{nodeId:guid}/token")]
    public ActionResult<NodeTokenResponse> GenerateToken(Guid nodeId)
    {
        var token = _service.GenerateNodeToken(nodeId);
        return Ok(token);
    }
}
