using System;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

public interface ICosStorageService
{
    CosBucketConfiguration ResolveConfiguration(Guid customerId);
    string BuildObjectKey(PackageUploadRequest request, CosBucketConfiguration config);
    PackageUploadResponse GenerateUploadUrls(PackageUpload package, CosBucketConfiguration config);
}
