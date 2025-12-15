using System;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

/// <summary>
/// COS 对象存储服务接口
/// </summary>
public interface ICosStorageService
{
    /// <summary>
    /// 解析客户对应的存储桶配置
    /// </summary>
    CosBucketConfiguration ResolveConfiguration(Guid customerId);
    
    /// <summary>
    /// 构建对象存储键
    /// </summary>
    string BuildObjectKey(PackageUploadRequest request, CosBucketConfiguration config);
    
    /// <summary>
    /// 生成预签名上传 URL
    /// </summary>
    PackageUploadResponse GenerateUploadUrls(PackageUpload package, CosBucketConfiguration config);
}
