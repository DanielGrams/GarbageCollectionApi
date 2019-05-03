namespace GarbageCollectionApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Program
    /// </summary>
#pragma warning disable CA1052, S1118
    public class Program
#pragma warning restore CA1052, S1118
    {
        /// <summary>
        /// Entry method
        /// </summary>
        /// <param name="args">Arguments</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates web host
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Web host builder</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseAzureAppServices()
                .UseStartup<Startup>();
    }
}
