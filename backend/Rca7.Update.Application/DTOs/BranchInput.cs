using System;
using System.ComponentModel.DataAnnotations;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 分支输入 DTO
/// </summary>
public class BranchInput
{
    /// <summary>
    /// 所属客户唯一标识
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 分支名称
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分支版本号
    /// </summary>
    [Required]
    public string Version { get; set; } = string.Empty;
}
