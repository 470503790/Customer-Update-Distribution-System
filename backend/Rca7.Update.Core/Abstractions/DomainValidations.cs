using System;

namespace Rca7.Update.Core.Abstractions;

/// <summary>
/// 领域验证辅助类，提供通用的实体验证方法
/// </summary>
public static class DomainValidations
{
    /// <summary>
    /// 确保枚举值已定义
    /// </summary>
    public static void EnsureEnumDefined<TEnum>(TEnum value, string paramName) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            throw new ArgumentOutOfRangeException(paramName, $"Unsupported value for {typeof(TEnum).Name}: {value}");
        }
    }

    /// <summary>
    /// 确保字符串非空
    /// </summary>
    public static void EnsureNotEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be empty", paramName);
        }
    }

    /// <summary>
    /// 确保版本号可解析
    /// </summary>
    public static Version EnsureVersionParsable(string version, string paramName)
    {
        if (!Version.TryParse(version, out var parsed))
        {
            throw new ArgumentException($"{paramName} must be a semantic version", paramName);
        }

        return parsed;
    }

    /// <summary>
    /// 确保版本号在指定范围内
    /// </summary>
    public static void EnsureVersionInRange(string version, Version minimum, Version maximum, string paramName)
    {
        var parsed = EnsureVersionParsable(version, paramName);

        if (parsed < minimum || parsed > maximum)
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be between {minimum} and {maximum} inclusive");
        }
    }
}
