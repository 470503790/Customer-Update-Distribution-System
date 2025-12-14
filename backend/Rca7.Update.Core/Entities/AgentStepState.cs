namespace Rca7.Update.Core.Entities;

/// <summary>
/// 代理步骤状态枚举
/// </summary>
public enum AgentStepState
{
    /// <summary>
    /// 待执行
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// 执行中
    /// </summary>
    Running = 1,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// 已跳过
    /// </summary>
    Skipped = 4
}
