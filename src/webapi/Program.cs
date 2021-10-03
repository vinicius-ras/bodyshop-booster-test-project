using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BodyShopBoosterTest
{
    /// <summary>Main class, containing the application's entrypoint.</summary>
    public class Program
    {
        /// <summary>Application's entrypoint.</summary>
        /// <remarks>The entrypoint is used to build and run the web API's host server.</remarks>
        /// <param name="args">Command-line arguments for the application (if any).</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        /// <summary>Utility method to create a default web host builder.</summary>
        /// <param name="args">Command line arguments which were passed to the application (if any).</param>
        /// <returns>Returns an <see cref="IHostBuilder"/> for further configuring and running the web host.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
