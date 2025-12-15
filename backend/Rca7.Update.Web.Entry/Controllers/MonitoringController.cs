using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

/// <summary>
/// 监控控制器，提供发布单和包的监控查询接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly ReleaseOrderService _orders;
    private readonly PackageUploadService _packages;

    public MonitoringController(ReleaseOrderService orders, PackageUploadService packages)
    {
        _orders = orders;
        _packages = packages;
    }

    /// <summary>
    /// 获取所有发布单
    /// </summary>
    [HttpGet("releases")]
    public ActionResult<IEnumerable<ReleaseOrder>> Releases()
    {
        return Ok(_orders.GetAll());
    }

    /// <summary>
    /// 获取指定客户的所有包
    /// </summary>
    [HttpGet("packages/{customerId:guid}")]
    public ActionResult<IEnumerable<PackageUpload>> Packages(Guid customerId)
    {
        return Ok(_packages.List(customerId));
    }
}
