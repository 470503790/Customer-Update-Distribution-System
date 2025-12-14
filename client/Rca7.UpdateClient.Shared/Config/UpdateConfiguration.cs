using System.Collections.Generic;
using System.IO;

namespace Rca7.UpdateClient.Shared.Config;

/// <summary>
/// Defines the core configuration shared by the agent service and tray client.
/// </summary>
public class UpdateConfiguration
{
    /// <summary>
    /// Directory that contains update packages by default.
    /// </summary>
    public string DefaultUpdateDirectory { get; set; } = Path.Combine(".", "updates");

    /// <summary>
    /// Directory used to store backups before applying updates.
    /// </summary>
    public string BackupDirectory { get; set; } = Path.Combine(".", "backups");

    /// <summary>
    /// Ordered list of SQL scripts to be executed for an update.
    /// </summary>
    public List<string> SqlExecutionOrder { get; set; } = new()
    {
        "00_Prepare.sql",
        "01_Update.sql",
        "99_Finalize.sql"
    };

    /// <summary>
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
