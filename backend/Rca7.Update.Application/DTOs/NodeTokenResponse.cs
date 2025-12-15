using System;

namespace Rca7.Update.Application.DTOs;

/// <summary>
/// 节点令牌响应 DTO
/// </summary>
public class NodeTokenResponse
{
    /// <summary>
    /// 节点唯一标识
    /// </summary>
    public Guid NodeId { get; set; }
    
    /// <summary>
    /// 生成的一次性令牌
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
