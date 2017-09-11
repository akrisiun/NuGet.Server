using System.Diagnostics;

namespace Server
{
    using NuGet.Server.DataServices;
    using System;
    using System.Web;

    // Server.ConsoleWeb.IsDebug
    public class ConsoleWeb
    {
        public static Exception LastError { get; set; }
        public static bool IsDebug {
            [DebuggerStepThrough]
            get;
            set;
        }

        public static void Write(object data)
        {
            if (data == null) return;
            string str = data as string;
            str = str ?? data.ToString();

            var ctx = HttpContext.Current;
            if (str == null || ctx == null) return;

            ctx.Response.Write(str);
        }

        public static void PackagesAll()
        {
            //var nupkg = Packages.GetAll();
            //var ctx = HttpContext.Current;
            //if (nupkg == null || ctx == null) return;

            //var resp = ctx.Response;
            //foreach (var item in nupkg)
            //{
            //    resp.Write($"Title= {item.Title}");
            //    resp.Write($"Version= {item.Version}");
            //}
        }
    }
}
