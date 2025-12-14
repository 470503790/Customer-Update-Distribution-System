using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Rca7.Update.Web.Entry.Infrastructure;

public static class DatabaseInitializer
{
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
