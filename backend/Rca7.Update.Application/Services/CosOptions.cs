using System;
using System.Collections.Generic;

namespace Rca7.Update.Application.Services;

/// <summary>
/// COS 对象存储配置选项
/// </summary>
public class CosOptions
{
    /// <summary>
    /// 默认存储桶配置
    /// </summary>
    public CosBucketConfiguration Default { get; set; } = new();

    /// <summary>
    /// 客户特定的存储桶覆盖配置
    /// </summary>
    public Dictionary<Guid, CosBucketConfiguration> CustomerOverrides { get; set; } = new();
}

/// <summary>
/// COS 存储桶配置
/// </summary>
public class CosBucketConfiguration
{
    /// <summary>
    /// 应用 ID
    /// </summary>
    public string AppId { get; set; } = string.Empty;
    
    /// <summary>
    /// 密钥 ID
    /// </summary>
    public string SecretId { get; set; } = string.Empty;
    
    /// <summary>
    /// 密钥
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 区域
    /// </summary>
    public string Region { get; set; } = string.Empty;
    
    /// <summary>
    /// 存储桶名称
    /// </summary>
    public string Bucket { get; set; } = string.Empty;
    
    /// <summary>
    /// 基础路径前缀
    /// </summary>
    public string BasePath { get; set; } = "updates";
    
    /// <summary>
    /// 是否使用 HTTPS
    /// </summary>
    public bool UseHttps { get; set; } = true;
    
    /// <summary>
    /// 预签名 URL 有效期（秒）
    /// </summary>
    public int SignedUrlTtlSeconds { get; set; } = 600;
}
