using System;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NodesController : ControllerBase
{
    private readonly CustomerTreeService _service;

    public NodesController(CustomerTreeService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<Node> Create([FromBody] NodeInput input)
    {
        var node = _service.AddNode(input);
        return CreatedAtAction(nameof(Create), new { id = node.Id }, node);
    }

    [HttpPost("{nodeId:guid}/token")]
    public ActionResult<NodeTokenResponse> GenerateToken(Guid nodeId)
    {
        var token = _service.GenerateNodeToken(nodeId);
        return Ok(token);
    }
}
