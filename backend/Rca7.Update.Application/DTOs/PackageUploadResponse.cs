using System;

namespace Rca7.Update.Application.DTOs;

public class PackageUploadResponse
{
    public Guid PackageId { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public string UploadUrl { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
}
