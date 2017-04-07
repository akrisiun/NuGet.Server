

using NuGet.Server.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet.Server
{

    public static class PackagesStatic //  : DataService<PackageContext>, IDataServiceStreamProvider, IServiceProvider
    {

        public static IQueryable<ODataPackage> GetUpdates(this Packages @this,
            string packageIds,
            string versions,
            bool includePrerelease,
            bool includeAllVersions,
            string targetFrameworks,
            string versionConstraints)
        {

            if (String.IsNullOrEmpty(packageIds) || String.IsNullOrEmpty(versions))
            {
                return Enumerable.Empty<ODataPackage>().AsQueryable();
            }

            var idValues = packageIds.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var versionValues = versions.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var targetFrameworkValues = String.IsNullOrEmpty(targetFrameworks) ? null :
                                                                                 targetFrameworks.Split('|').Select(VersionUtility.ParseFrameworkName).ToList();
            var versionConstraintValues = String.IsNullOrEmpty(versionConstraints)
                                            ? new string[idValues.Length]
                                            : versionConstraints.Split('|');

            if (idValues.Length == 0 || idValues.Length != versionValues.Length || idValues.Length != versionConstraintValues.Length)
            {
                // Exit early if the request looks invalid
                return Enumerable.Empty<ODataPackage>().AsQueryable();
            }

            var packagesToUpdate = new List<IPackageMetadata>();
            for (var i = 0; i < idValues.Length; i++)
            {
                packagesToUpdate.Add(new PackageBuilder { Id = idValues[i], Version = new SemanticVersion(versionValues[i]) });
            }

            var versionConstraintsList = new IVersionSpec[versionConstraintValues.Length];
            for (var i = 0; i < versionConstraintsList.Length; i++)
            {
                if (!String.IsNullOrEmpty(versionConstraintValues[i]))
                {
                    VersionUtility.TryParseVersionSpec(versionConstraintValues[i], out versionConstraintsList[i]);
                }
            }

            var clientCompatibility = @this.CurrentDataSourceWrap.ClientCompatibility;

            return null;

            //        NuGet.Server.Infrastructure.ServerPackageRepositoryExtensions
            //        .GetUpdatesCore(Repository,
            //            packagesToUpdate,
            //            includePrerelease,
            //            includeAllVersions,
            //            targetFrameworkValues,
            //            versionConstraintsList,
            //            clientCompatibility)
            //        .Select(package => package.AsODataPackage(clientCompatibility))
            //        .AsQueryable()
            //        .InterceptWith(new NormalizeVersionInterceptor());

        }
    }
}

