using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rca7.UpdateClient.Shared.State;

/// <summary>
/// 表示可在崩溃或重启后恢复的持久化代理状态
/// Represents durable agent state that can be resumed after a crash or restart.
/// </summary>
public class AgentState
{
    /// <summary>
    /// 默认状态文件名
    /// </summary>
    public const string DefaultStateFileName = "agent-state.json";

    /// <summary>
    /// 当前正在处理的包标识
    /// Identifier of the package currently being processed.
    /// </summary>
    public string? ActivePackageId { get; set; }

    /// <summary>
    /// 最后成功执行的 SQL 脚本名称
    /// Name of the SQL script last executed successfully.
    /// </summary>
    public string? LastAppliedSql { get; set; }

    /// <summary>
    /// 状态最后更新时间
    /// When the state was last updated.
    /// </summary>
    public DateTimeOffset LastCheckpointUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 指示更新是否正在进行中且可以恢复
    /// True when an update is in progress and can be resumed.
    /// </summary>
    public bool IsInProgress { get; set; }
}

/// <summary>
/// 可发送给 UI 客户端显示当前代理进度的快照
/// Snapshot that can be sent to UI clients showing current agent progress.
/// </summary>
public record AgentStatusSnapshot(
    string? ActivePackageId,
    string? Stage,
    double Progress,
    string? LastMessage,
    DateTimeOffset LastUpdatedUtc
);

/// <summary>
/// 用于 UI 和诊断目的共享的配置快照
/// Snapshot of configuration shared for UI and diagnostics purposes.
/// </summary>
public record ConfigurationSnapshot(string DefaultUpdateDirectory, string BackupDirectory, IReadOnlyList<string> SqlExecutionOrder)
{
    /// <summary>
    /// 指示是否有自定义 SQL 执行顺序
    /// </summary>
    [JsonIgnore]
    public bool HasCustomSqlOrder => SqlExecutionOrder.Count > 0;
}
