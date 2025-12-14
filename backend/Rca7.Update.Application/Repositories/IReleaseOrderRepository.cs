using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public interface IReleaseOrderRepository
{
    ReleaseOrder Save(ReleaseOrder order);
    ReleaseOrder? Find(Guid id);
    IEnumerable<ReleaseOrder> GetAll();
}
