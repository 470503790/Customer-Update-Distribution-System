using System;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 包上传请求 DTO
/// </summary>
public class PackageUploadRequest
{
    /// <summary>
    /// 客户唯一标识
    /// </summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// 分支唯一标识（可选）
    /// </summary>
    public Guid? BranchId { get; set; }
    
    /// <summary>
    /// 节点唯一标识（可选）
    /// </summary>
    public Guid? NodeId { get; set; }
    
    /// <summary>
    /// 部署环境
    /// </summary>
    public DeploymentEnvironment Environment { get; set; }
    
    /// <summary>
    /// 包资源类型（服务端/客户端/SQL脚本等）
    /// </summary>
    public PackageAssetType AssetType { get; set; }
    
    /// <summary>
    /// 包版本号
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// 上传者标识
    /// </summary>
    public string UploadedBy { get; set; } = "system";
}
