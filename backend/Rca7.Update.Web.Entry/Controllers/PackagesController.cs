using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 包管理控制器，提供包上传准备和查询接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly PackageUploadService _packages;

    public PackagesController(PackageUploadService packages)
    {
        _packages = packages;
    }

    /// <summary>
    /// 准备包上传，生成预签名 URL
    /// </summary>
    [HttpPost("upload-urls")]
    public ActionResult<PackageUploadResponse> PrepareUpload([FromBody] PackageUploadRequest request)
    {
        var response = _packages.PrepareUpload(request);
        return Ok(response);
    }

    /// <summary>
    /// 列出指定客户的所有包
    /// </summary>
    [HttpGet("customers/{customerId:guid}")]
    public ActionResult<IEnumerable<PackageUpload>> List(Guid customerId)
    {
        return Ok(_packages.List(customerId));
    }
}
