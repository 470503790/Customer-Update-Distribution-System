using System;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 包上传响应 DTO
/// </summary>
public class PackageUploadResponse
{
    /// <summary>
    /// 包唯一标识
    /// </summary>
    public Guid PackageId { get; set; }
    
    /// <summary>
    /// COS 对象存储键
    /// </summary>
    public string ObjectKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 预签名上传 URL
    /// </summary>
    public string UploadUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// 预签名下载 URL（可选）
    /// </summary>
    public string? DownloadUrl { get; set; }
}
