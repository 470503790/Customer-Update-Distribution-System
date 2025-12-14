using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public interface IAuditLogRepository
{
    AuditLog Save(AuditLog entry);
    IEnumerable<AuditLog> GetRecent(int limit = 100);
}
