using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Rca7.Update.Application.DTOs;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Services;

public class CosStorageService : ICosStorageService
{
    private readonly CosOptions _options;

    public CosStorageService(IOptions<CosOptions> options)
    {
        _options = options.Value;
    }

    public CosBucketConfiguration ResolveConfiguration(Guid customerId)
    {
        if (_options.CustomerOverrides.TryGetValue(customerId, out var configuration))
        {
            return configuration;
        }

        return _options.Default;
    }

    public string BuildObjectKey(PackageUploadRequest request, CosBucketConfiguration config)
    {
        var scopeBuilder = new StringBuilder();
        scopeBuilder.Append(request.CustomerId).Append('/');

        if (request.BranchId.HasValue)
        {
            scopeBuilder.Append(request.BranchId.Value).Append('/');
        }

        if (request.NodeId.HasValue)
        {
            scopeBuilder.Append(request.NodeId.Value).Append('/');
        }

        scopeBuilder.Append(request.AssetType.ToString().ToLowerInvariant()).Append('/');
        scopeBuilder.Append(request.FileName);

        return $"{config.BasePath.TrimEnd('/')}/{scopeBuilder}";
    }

    public PackageUploadResponse GenerateUploadUrls(PackageUpload package, CosBucketConfiguration config)
    {
        var protocol = config.UseHttps ? "https" : "http";
        var host = $"{config.Bucket}.cos.{config.Region}.myqcloud.com";
        var resourceUrl = $"{protocol}://{host}/{package.ObjectKey}";
        var expires = DateTimeOffset.UtcNow.AddSeconds(config.SignedUrlTtlSeconds).ToUnixTimeSeconds();
        var signature = GenerateSignature(resourceUrl, expires, config.SecretKey, config.SecretId);

        var uploadUrl = $"{resourceUrl}?q-sign-algorithm=sha1&q-ak={Uri.EscapeDataString(config.SecretId)}&q-sign-time={expires}&q-signature={signature}";
        var downloadUrl = $"{resourceUrl}?q-signature={signature}";

        return new PackageUploadResponse
        {
            PackageId = package.Id,
            ObjectKey = package.ObjectKey,
            UploadUrl = uploadUrl,
            DownloadUrl = downloadUrl
        };
    }

    private static string GenerateSignature(string resourceUrl, long expires, string secretKey, string secretId)
    {
        var canonicalString = $"{resourceUrl}:{expires}:{secretId}";
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalString));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
