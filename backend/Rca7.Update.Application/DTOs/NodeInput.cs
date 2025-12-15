using System;
using System.ComponentModel.DataAnnotations;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 节点输入 DTO
/// </summary>
public class NodeInput
{
    /// <summary>
    /// 所属分支唯一标识
    /// </summary>
    [Required]
    public Guid BranchId { get; set; }

    /// <summary>
    /// 部署环境（开发/测试/生产等）
    /// </summary>
    [Required]
    public DeploymentEnvironment Environment { get; set; }

    /// <summary>
    /// 节点版本号
    /// </summary>
    [Required]
    public string Version { get; set; } = string.Empty;
}
