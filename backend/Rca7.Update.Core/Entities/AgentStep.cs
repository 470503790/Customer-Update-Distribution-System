namespace Rca7.Update.Core.Entities;

/// <summary>
/// 代理执行步骤枚举，定义发布流程中的各个阶段
/// </summary>
public enum AgentStep
{
    /// <summary>
    /// 停止服务
    /// </summary>
    ServiceStop = 1,
    
    /// <summary>
    /// 完整备份
    /// </summary>
    FullBackup = 2,
    
    /// <summary>
    /// 部署服务端
    /// </summary>
    DeployServer = 3,
    
    /// <summary>
    /// 部署客户端
    /// </summary>
    DeployClient = 4,
    
    /// <summary>
    /// 运行架构脚本
    /// </summary>
    RunSchemaScript = 5,
    
    /// <summary>
    /// 运行数据脚本
    /// </summary>
    RunDataScript = 6,
    
    /// <summary>
    /// 重启服务
    /// </summary>
    Restart = 7,
    
    /// <summary>
    /// 上报状态
    /// </summary>
    ReportStatus = 8,
    
    /// <summary>
    /// 触发回滚
    /// </summary>
    TriggerRollback = 9
}
