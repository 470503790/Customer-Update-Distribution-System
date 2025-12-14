namespace Rca7.Update.Core.Entities;

/// <summary>
/// 包资源类型枚举
/// </summary>
public enum PackageAssetType
{
    /// <summary>
    /// 服务端二进制文件
    /// </summary>
    ServerBinary = 1,
    
    /// <summary>
    /// 客户端二进制文件
    /// </summary>
    ClientBinary = 2,
    
    /// <summary>
    /// 节点配置文件
    /// </summary>
    NodeConfiguration = 3,
    
    /// <summary>
    /// 诊断包
    /// </summary>
    DiagnosticBundle = 4
}
