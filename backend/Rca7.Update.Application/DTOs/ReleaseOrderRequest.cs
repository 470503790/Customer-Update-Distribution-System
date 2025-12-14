using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 发布单请求 DTO
/// </summary>
public class ReleaseOrderRequest
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
    /// 发布策略（全量发布/增量发布等）
    /// </summary>
    public ReleaseStrategy Strategy { get; set; }
    
    /// <summary>
    /// 截止时间
    /// </summary>
    public DateTimeOffset Deadline { get; set; }
    
    /// <summary>
    /// 服务端包唯一标识（可选）
    /// </summary>
    public Guid? ServerPackageId { get; set; }
    
    /// <summary>
    /// 客户端包唯一标识（可选）
    /// </summary>
    public Guid? ClientPackageId { get; set; }
    
    /// <summary>
    /// 配置包唯一标识（可选）
    /// </summary>
    public Guid? ConfigurationPackageId { get; set; }
    
    /// <summary>
    /// 自定义执行步骤（可选）
    /// </summary>
    public IEnumerable<AgentStep>? Steps { get; set; }
    
    /// <summary>
    /// 请求者标识
    /// </summary>
    public string RequestedBy { get; set; } = "system";
}
