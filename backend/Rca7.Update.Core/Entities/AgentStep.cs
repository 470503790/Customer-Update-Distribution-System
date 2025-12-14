namespace Rca7.Update.Core.Entities;

public enum AgentStep
{
    ServiceStop = 1,
    FullBackup = 2,
    DeployServer = 3,
    DeployClient = 4,
    RunSchemaScript = 5,
    RunDataScript = 6,
    Restart = 7,
    ReportStatus = 8,
    TriggerRollback = 9
}
