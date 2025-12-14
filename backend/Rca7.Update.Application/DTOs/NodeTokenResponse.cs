using System;

namespace Rca7.Update.Application.DTOs;

public class NodeTokenResponse
{
    public Guid NodeId { get; set; }
    public string Token { get; set; } = string.Empty;
}
