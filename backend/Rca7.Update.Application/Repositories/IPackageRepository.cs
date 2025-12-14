using System;
using System.Collections.Generic;
using Rca7.Update.Core.Entities;

namespace Rca7.Update.Application.Repositories;

public interface IPackageRepository
{
    PackageUpload Save(PackageUpload package);
    PackageUpload? Find(Guid id);
    IEnumerable<PackageUpload> ListByCustomer(Guid customerId);
}
