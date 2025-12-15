using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Rca7.Update.Web.Entry.Infrastructure;

/// <summary>
/// 数据库初始化器，负责应用数据库迁移脚本
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// 应用数据库迁移脚本，按文件名顺序执行
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="migrationsPath">迁移脚本目录路径</param>
    public static void ApplyMigrations(ISqlSugarClient db, ILogger logger, string migrationsPath)
    {
        if (!Directory.Exists(migrationsPath))
        {
            logger.LogWarning("Migrations directory {Path} was not found. Skipping database migrations.", migrationsPath);
            return;
        }

        var scripts = Directory.GetFiles(migrationsPath, "*.sql", SearchOption.TopDirectoryOnly)
            .OrderBy(f => f)
            .ToList();

        if (scripts.Count == 0)
        {
            logger.LogInformation("No migration scripts found in {Path}.", migrationsPath);
            return;
        }

        foreach (var scriptPath in scripts)
        {
            var script = File.ReadAllText(scriptPath);
            if (string.IsNullOrWhiteSpace(script))
            {
                continue;
            }

            logger.LogInformation("Applying database script {Script}...", Path.GetFileName(scriptPath));
            db.Ado.ExecuteCommand(script);
        }

        logger.LogInformation("Database migrations completed using {Count} script(s).", scripts.Count);
    }
}
