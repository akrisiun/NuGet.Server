// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.ServiceModel.Activation;
using System.Diagnostics;
using System.Web.Routing;
using NuGet.Server.DataServices;
using NuGet.Server.Publishing;
using RouteMagic;
using NuGet.Server.Infrastructure;
using NuGet.Server.Logging;
using NuGet.Server;

[assembly: System.Web.PreApplicationStartMethod(typeof(NuGet.NuGetRoutes), "Start")]
// [assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NuGetRoutes), "Start")]

namespace NuGet
{
    public static class NuGetRoutes
    {
        public static void Start() { } // => Restart();
        public static void Restart()
        {
#if DEBUG
            if (Debugger.IsAttached)
                Debugger.Break();
#endif
            if (isStarted)
                return;

            ServiceResolver.SetServiceResolver(new NuGet.Server.DefaultServiceResolver());
            MapRoutes(RouteTable.Routes);

            //http://stackoverflow.com/questions/10523105/asp-net-routing-integration-feature-requires-asp-net-compatibility-with-webapi-0
            // <serviceHostingEnvironment aspNetCompatibilityEnabled = "true" />
        }

        static bool isStarted = false;

        private static void MapRoutes(RouteCollection routes)
        {
            isStarted = true;

            // Route to create a new package
            routes.MapDelegate("CreatePackage-Root",
                               "",
                               new { httpMethod = new HttpMethodConstraint("PUT") },
                               context => CreatePackageService().CreatePackage(context.HttpContext));

            routes.MapDelegate("CreatePackage",
                               "api/v2/package",
                               new { httpMethod = new HttpMethodConstraint("PUT") },
                               context => CreatePackageService().CreatePackage(context.HttpContext));

            // Route to delete packages
            routes.MapDelegate("DeletePackage-Root",
                                           "{packageId}/{version}",
                                           new { httpMethod = new HttpMethodConstraint("DELETE") },
                                           context => CreatePackageService().DeletePackage(context.HttpContext));

            routes.MapDelegate("DeletePackage",
                               "api/v2/package/{packageId}/{version}",
                               new { httpMethod = new HttpMethodConstraint("DELETE") },
                               context => CreatePackageService().DeletePackage(context.HttpContext));

            // Route to get packages
            routes.MapDelegate("DownloadPackage",
                               "api/v2/package/{packageId}/{version}",
                               new { httpMethod = new HttpMethodConstraint("GET") },
                               context => CreatePackageService().DownloadPackage(context.HttpContext));

            // Route to clear package cache
            routes.MapDelegate("ClearPackageCache",
                               "nugetserver/api/clear-cache",
                               new { httpMethod = new HttpMethodConstraint("GET") },
                               context => CreatePackageService().ClearCache(context.HttpContext));

#if DEBUG
            // Route to create a new package(http://{root}/nuget)
            routes.MapDelegate("CreatePackageNuGet",
                               "nuget",
                               new { httpMethod = new HttpMethodConstraint("PUT") },
                               context => CreatePackageService().CreatePackage(context.HttpContext));

            // The default route is http://{root}/nuget/Packages
            var factory = new System.Data.Services.DataServiceHostFactory();
            var serviceRoute = new ServiceRoute("nuget", factory, typeof(Packages));
            serviceRoute.Defaults = new RouteValueDictionary { { "serviceType", "odata" } };
            serviceRoute.Constraints = new RouteValueDictionary { { "serviceType", "odata" } };
            routes.Add("nuget", serviceRoute);

            ServiceRoute = serviceRoute;
#endif
        }

        public static ServiceRoute ServiceRoute { get; set; }

        public static IPackageService CreatePackageService()
        {
            return ServiceResolver.Resolve<IPackageService>();
        }

        public static IHashProvider ResolveHash()
        {
            return new CryptoHashProvider(NuGet.Server.Constants.HashAlgorithm);
        }

        public static ResolverData ResolveData()
        {
            var hash = ResolveHash();

            var data = new ResolverData
            {
                HashProvider = hash,
                PackageRepository = new ServerPackageRepository(PackageUtility.PackagePhysicalPath, hash, new TraceLogger()),
                PackageAuthenticationService = new PackageAuthenticationService()
            };
            data.PackageService = new PackageService(data.PackageRepository, data.PackageAuthenticationService);

            return data;
        }

    }

    public class ResolverData
    {
        public IHashProvider HashProvider { get; set; }
        public IServerPackageRepository PackageRepository { get; set; }
        public IPackageAuthenticationService PackageAuthenticationService { get; set; }
        public IPackageService PackageService { get; set; }
    }

}