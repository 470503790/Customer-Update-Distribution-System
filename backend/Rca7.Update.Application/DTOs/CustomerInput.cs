using System;
using System.ComponentModel.DataAnnotations;

namespace Rca7.Update.Application.DTOs;

public class CustomerInput
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = string.Empty;
}
