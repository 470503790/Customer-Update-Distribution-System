using System;
using System.Collections.Generic;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Application.Repositories;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

public class PackageUploadService
{
    private static readonly Version MinimumVersion = new(1, 0, 0);
    private static readonly Version MaximumVersion = new(9, 9, 9);

    private readonly IPackageRepository _packages;
    private readonly ICosStorageService _storageService;
    private readonly AuditLogService _auditLogs;

    public PackageUploadService(IPackageRepository packages, ICosStorageService storageService, AuditLogService auditLogs)
    {
        _packages = packages;
        _storageService = storageService;
        _auditLogs = auditLogs;
    }

    public PackageUploadResponse PrepareUpload(PackageUploadRequest request)
    {
        var config = _storageService.ResolveConfiguration(request.CustomerId);
        var objectKey = _storageService.BuildObjectKey(request, config);
        var package = new PackageUpload
        {
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            NodeId = request.NodeId,
            Environment = request.Environment,
            AssetType = request.AssetType,
            ObjectKey = objectKey,
            FileName = request.FileName,
            UploadedBy = request.UploadedBy,
            Version = request.Version
        };

        package.Validate(MinimumVersion, MaximumVersion);
        _packages.Save(package);

        _auditLogs.Record("package.upload", $"Prepared upload for {request.AssetType} {request.FileName}", request.UploadedBy, package.Id.ToString());

        return _storageService.GenerateUploadUrls(package, config);
    }

    public IEnumerable<PackageUpload> List(Guid customerId)
    {
        return _packages.ListByCustomer(customerId);
    }
}
