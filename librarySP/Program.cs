using System.IO;

using BusinessLayer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace librarySP
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webHostBuilder => {
                  webHostBuilder
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                 })
                    .Build();

            host.Run();

            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });



    }
}
