using System;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly CustomerTreeService _service;

    public BranchesController(CustomerTreeService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<Branch> Create([FromBody] BranchInput input)
    {
        var branch = _service.AddBranch(input);
        return CreatedAtAction(nameof(Create), new { id = branch.Id }, branch);
    }
}
