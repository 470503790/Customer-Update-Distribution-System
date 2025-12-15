using System.Collections.Generic;
using System.IO;

namespace Rca7.UpdateClient.Shared.Config;

/// <summary>
/// 定义代理服务和托盘客户端共享的核心配置
/// Defines the core configuration shared by the agent service and tray client.
/// </summary>
public class UpdateConfiguration
{
    /// <summary>
    /// 默认包含更新包的目录
    /// Directory that contains update packages by default.
    /// </summary>
    public string DefaultUpdateDirectory { get; set; } = Path.Combine(".", "updates");

    /// <summary>
    /// 用于在应用更新前存储备份的目录
    /// Directory used to store backups before applying updates.
    /// </summary>
    public string BackupDirectory { get; set; } = Path.Combine(".", "backups");

    /// <summary>
    /// 更新时要执行的 SQL 脚本的有序列表
    /// Ordered list of SQL scripts to be executed for an update.
    /// </summary>
    public List<string> SqlExecutionOrder { get; set; } = new()
    {
        "00_Prepare.sql",
        "01_Update.sql",
        "99_Finalize.sql"
    };

    /// <summary>
    /// 使用提供的根目录创建配置实例，派生默认路径
    /// Creates a configuration instance using the provided root to derive default paths.
    /// </summary>
    public static UpdateConfiguration CreateDefaults(string baseDirectory)
    {
        return new UpdateConfiguration
        {
            DefaultUpdateDirectory = Path.Combine(baseDirectory, "updates"),
            BackupDirectory = Path.Combine(baseDirectory, "backups"),
            SqlExecutionOrder = new()
            {
                "00_Prepare.sql",
                "01_Update.sql",
                "99_Finalize.sql"
            }
        };
    }
}
