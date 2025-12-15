namespace Rca7.Update.Core.Entities;

/// <summary>
/// 发布状态枚举
/// </summary>
public enum ReleaseStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// 已排期
    /// </summary>
    Scheduled = 1,
    
    /// <summary>
    /// 进行中
    /// </summary>
    InProgress = 2,
    
    /// <summary>
    /// 成功
    /// </summary>
    Succeeded = 3,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// 已回滚
    /// </summary>
    RolledBack = 5
}
