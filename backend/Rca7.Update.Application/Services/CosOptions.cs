using System;
using System.Collections.Generic;

namespace Rca7.Update.Application.Services;

public class CosOptions
{
    public CosBucketConfiguration Default { get; set; } = new();

    public Dictionary<Guid, CosBucketConfiguration> CustomerOverrides { get; set; } = new();
}

public class CosBucketConfiguration
{
    public string AppId { get; set; } = string.Empty;
    public string SecretId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string BasePath { get; set; } = "updates";
    public bool UseHttps { get; set; } = true;
    public int SignedUrlTtlSeconds { get; set; } = 600;
}
