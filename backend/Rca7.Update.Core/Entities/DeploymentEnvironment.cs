namespace Rca7.Update.Core.Entities;

/// <summary>
/// 部署环境枚举
/// </summary>
public enum DeploymentEnvironment
{
    /// <summary>
    /// 生产环境
    /// </summary>
    Production = 1,
    
    /// <summary>
    /// 预发布环境
    /// </summary>
    Staging = 2,
    
    /// <summary>
    /// 测试环境
    /// </summary>
    Testing = 3,
    
    /// <summary>
    /// 开发环境
    /// </summary>
    Development = 4
}
