namespace Rca7.Update.Core.Entities;

/// <summary>
/// 发布策略枚举
/// </summary>
public enum ReleaseStrategy
{
    /// <summary>
    /// 滚动发布
    /// </summary>
    Rolling = 1,
    
    /// <summary>
    /// 蓝绿部署
    /// </summary>
    BlueGreen = 2,
    
    /// <summary>
    /// 热修复
    /// </summary>
    Hotfix = 3
}
