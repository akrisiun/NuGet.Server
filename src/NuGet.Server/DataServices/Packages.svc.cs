// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using System.Diagnostics;
using NuGet.Server.Infrastructure;
using NuGet.Server.DataServices;

namespace NuGet.Server.DataServices
{
    // Disabled for live service
    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Packages : DataService<PackageContext>, IDataServiceStreamProvider, IServiceProvider
    {

        public Packages()
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        private IServerPackageRepository Repository {
            get {
                // It's bad to use the container directly but we aren't in the loop when this 
                // class is created
                return ServiceResolver.Resolve<IServerPackageRepository>();
            }
        }

        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("Packages", EntitySetRights.AllRead);
            config.SetEntitySetPageSize("Packages", 100);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            config.UseVerboseErrors = true;
            RegisterServices(config);
        }

        internal static void RegisterServices(IDataServiceConfiguration config)
        {
            config.SetServiceOperationAccessRule("Search", ServiceOperationRights.AllRead);
            config.SetServiceOperationAccessRule("FindPackagesById", ServiceOperationRights.AllRead);
            config.SetServiceOperationAccessRule("GetUpdates", ServiceOperationRights.AllRead);
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);

            // Determine the client compatibility based on the request URI. Note that the request URI may not be
            // exactly the same as the URI for the HTTP request itself. This edge case occurs when a batch request is
            // made. In batching, this method is called for each operation in the batch (which each have their own
            // request URI.
            CurrentDataSource.ClientCompatibility = ClientCompatibilityFactory.FromUri(args?.RequestUri);
        }

        // Summary:
        //     Gets the data source instance currently being used to process the request.
        public PackageContext CurrentDataSourceWrap { get { return base.CurrentDataSource; } }

        protected override PackageContext CreateDataSource()
        {
            return new PackageContext(Repository);
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            var package = (ODataPackage)entity;

            var rootUrlConfig = System.Configuration.ConfigurationManager.AppSettings["rootUrl"];
            var rootUrl = !string.IsNullOrWhiteSpace(rootUrlConfig)
                ? rootUrlConfig
                : HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);

            // the URI need to ends with a '/' to be correctly merged so we add it to the application if it 
            var downloadUrl = PackageUtility.GetPackageDownloadUrl(package);
            return new Uri(new Uri(rootUrl), downloadUrl);
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            return "application/zip";
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return null;
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public int StreamBufferSize {
            get {
                return 64000;
            }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return this;
            }
            return null;
        }

        [WebGet]
        public IQueryable<ODataPackage> Search(
            string searchTerm,
            string targetFramework,
            bool includePrerelease,
            bool? includeDelisted)
        {
            var targetFrameworks = string.IsNullOrEmpty(targetFramework) ? Enumerable.Empty<string>() : targetFramework.Split('|');
            var clientCompatibility = CurrentDataSource.ClientCompatibility;

            return Repository
                .Search(
                    searchTerm,
                    targetFrameworks,
                    includePrerelease,
                    clientCompatibility)
                .Select(package => package.AsODataPackage(clientCompatibility))
                .AsQueryable()
                .InterceptWith(new NormalizeVersionInterceptor());
        }

        [WebGet]
        public IQueryable<ODataPackage> FindPackagesById(string id)
        {
            var clientCompatibility = CurrentDataSource.ClientCompatibility;

            return Repository
                .FindPackagesById(id, clientCompatibility)
                .Where(package => package.Listed)
                .Select(package => package.AsODataPackage(clientCompatibility))
                .AsQueryable()
                .InterceptWith(new NormalizeVersionInterceptor());
        }

        [WebGet]
        public IQueryable<ODataPackage> GetUpdates(
            string packageIds,
            string versions,
            bool includePrerelease,
            bool includeAllVersions,
            string targetFrameworks,
            string versionConstraints)
        {
            return PackagesStatic.GetUpdates(this,
                packageIds,
                versions,
                includePrerelease,
                includeAllVersions,
                targetFrameworks,
                versionConstraints);
        }
    }

}