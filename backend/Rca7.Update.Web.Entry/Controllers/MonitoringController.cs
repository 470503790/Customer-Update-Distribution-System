using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rca7.Update.Application.Services;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Web.Entry.Controllers;

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

    [HttpGet("releases")]
    public ActionResult<IEnumerable<ReleaseOrder>> Releases()
    {
        return Ok(_orders.GetAll());
    }

    [HttpGet("packages/{customerId:guid}")]
    public ActionResult<IEnumerable<PackageUpload>> Packages(Guid customerId)
    {
        return Ok(_packages.List(customerId));
    }
}
