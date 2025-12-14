using System;
using System.ComponentModel.DataAnnotations;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 客户输入 DTO
/// </summary>
public class CustomerInput
{
    /// <summary>
    /// 客户名称
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户版本号
    /// </summary>
    [Required]
    public string Version { get; set; } = string.Empty;
}
