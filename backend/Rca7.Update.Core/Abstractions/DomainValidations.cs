using System;

namespace Rca7.Update.Core.Abstractions;

public static class DomainValidations
{
    public static void EnsureEnumDefined<TEnum>(TEnum value, string paramName) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            throw new ArgumentOutOfRangeException(paramName, $"Unsupported value for {typeof(TEnum).Name}: {value}");
        }
    }

    public static void EnsureNotEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be empty", paramName);
        }
    }

    public static Version EnsureVersionParsable(string version, string paramName)
    {
        if (!Version.TryParse(version, out var parsed))
        {
            throw new ArgumentException($"{paramName} must be a semantic version", paramName);
        }

        return parsed;
    }

    public static void EnsureVersionInRange(string version, Version minimum, Version maximum, string paramName)
    {
        var parsed = EnsureVersionParsable(version, paramName);

        if (parsed < minimum || parsed > maximum)
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be between {minimum} and {maximum} inclusive");
        }
    }
}
