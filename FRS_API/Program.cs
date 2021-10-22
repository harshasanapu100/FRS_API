using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                    .AddCommandLine(args)
                                    .SetBasePath(AppContext.BaseDirectory)
                                    .AddJsonFile("appsettings.json")
                                    .Build();

            return  Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configHost => {
                    configHost.SetBasePath(AppContext.BaseDirectory)
                                .Build();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
