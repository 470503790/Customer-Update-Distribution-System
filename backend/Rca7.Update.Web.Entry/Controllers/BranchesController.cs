using System;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 分支管理控制器，提供为客户添加分支的接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly CustomerTreeService _service;

    public BranchesController(CustomerTreeService service)
    {
        _service = service;
    }

    /// <summary>
    /// 为客户创建新分支
    /// </summary>
    [HttpPost]
    public ActionResult<Branch> Create([FromBody] BranchInput input)
    {
        var branch = _service.AddBranch(input);
        return CreatedAtAction(nameof(Create), new { id = branch.Id }, branch);
    }
}
