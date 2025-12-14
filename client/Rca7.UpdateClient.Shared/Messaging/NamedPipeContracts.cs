using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rca7.UpdateClient.Shared.Messaging;

/// <summary>
/// Commands that can be sent over the agent named pipe.
/// </summary>
public enum PipeCommand
{
    Unknown = 0,
    CheckStatus = 1,
    RequestConfiguration = 2,
    PushConfiguration = 3,
    StartUpdate = 4,
    PauseUpdate = 5,
    ResumeUpdate = 6,
    CancelUpdate = 7,
    Shutdown = 8
}

/// <summary>
/// Outcome of a named pipe response.
/// </summary>
public enum PipeResponseStatus
{
    Accepted,
    Completed,
    Busy,
    Invalid,
    NotFound,
    Error
}

/// <summary>
/// Basic request envelope exchanged over IPC.
/// </summary>
public record NamedPipeRequest(PipeCommand Command, Guid CorrelationId, string? PayloadJson = null);

/// <summary>
/// Basic response envelope exchanged over IPC.
/// </summary>
public record NamedPipeResponse(PipeCommand Command, PipeResponseStatus Status, Guid CorrelationId, string? PayloadJson = null, string? ErrorMessage = null);

/// <summary>
/// Helper utilities for serializing messages consistently between the agent and tray.
/// </summary>
public static class NamedPipeSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string SerializeRequest(NamedPipeRequest request, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(request, options ?? DefaultOptions);
    }

    public static string SerializeResponse(NamedPipeResponse response, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(response, options ?? DefaultOptions);
    }

    public static bool TryDeserializeRequest(string message, out NamedPipeRequest? request, JsonSerializerOptions? options = null)
    {
        try
        {
            request = JsonSerializer.Deserialize<NamedPipeRequest>(message, options ?? DefaultOptions);
            return request != null;
        }
        catch (JsonException)
        {
            request = null;
            return false;
        }
    }

    public static bool TryDeserializeResponse(string message, out NamedPipeResponse? response, JsonSerializerOptions? options = null)
    {
        try
        {
            response = JsonSerializer.Deserialize<NamedPipeResponse>(message, options ?? DefaultOptions);
            return response != null;
        }
        catch (JsonException)
        {
            response = null;
            return false;
        }
    }
}
