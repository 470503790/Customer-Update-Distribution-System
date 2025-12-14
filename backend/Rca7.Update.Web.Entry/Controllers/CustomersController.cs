using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 客户管理控制器，提供客户树的查询和创建接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerTreeService _service;

    public CustomersController(CustomerTreeService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取完整客户树
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Customer>> GetTree()
    {
        return Ok(_service.GetTree());
    }

    /// <summary>
    /// 创建新客户
    /// </summary>
    [HttpPost]
    public ActionResult<Customer> Create([FromBody] CustomerInput input)
    {
        var result = _service.CreateCustomer(input);
        return CreatedAtAction(nameof(GetTree), new { id = result.Id }, result);
    }
}
