using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly PackageUploadService _packages;

    public PackagesController(PackageUploadService packages)
    {
        _packages = packages;
    }

    [HttpPost("upload-urls")]
    public ActionResult<PackageUploadResponse> PrepareUpload([FromBody] PackageUploadRequest request)
    {
        var response = _packages.PrepareUpload(request);
        return Ok(response);
    }

    [HttpGet("customers/{customerId:guid}")]
    public ActionResult<IEnumerable<PackageUpload>> List(Guid customerId)
    {
        return Ok(_packages.List(customerId));
    }
}
