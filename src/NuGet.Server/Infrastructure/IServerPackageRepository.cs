// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

//using NuGet.Server.DataServices;
//using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;

namespace NuGet.Server.Infrastructure
{
    public interface IServerPackageRepository // : IServiceBasedRepository
    {
        void ClearCache();

        void RemovePackage(string packageId, SemanticVersion version);

        void AddPackage(LocalPackage package);

        IQueryable<ServerPackage> GetPackages(ClientCompatibility compatibility);

        IPackage FindPackage(string packageId, SemanticVersion version);

        IEnumerable<ServerPackage> FindPackagesById(string packageId, ClientCompatibility compatibility);

        IQueryable<IPackage> Search(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPrereleaseVersions,
            ClientCompatibility compatibility);
    }
}
